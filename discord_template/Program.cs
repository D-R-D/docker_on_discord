using Discord;
using Discord.Commands;
using Discord.WebSocket;
using docker_on_discord;
using System.Configuration;
using static System.Net.Mime.MediaTypeNames;
using System.IO;

namespace discord_template
{
    class Program
    {
        static Ids? ids = null;
        public static AppSettingsReader reader = new AppSettingsReader();

        private DiscordSocketClient? _client;
        private static CommandService? _commands;

        public static void Main(string[] args)
        {
            CommandBuilder commandbuilder = new CommandBuilder("docker.json");
            commandbuilder.CommandPush();

            ids = new Ids(reader);

            CommandSender commandSender = new CommandSender(Directory.GetCurrentDirectory() + "/commands", ids, reader.GetValue("discordapi_version", typeof(string)).ToString()!);
            commandSender.RequestSender();

            Thread.Sleep(1000);

            _ = new Program().MainAsync();

            Thread.Sleep(-1);
        }

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();
            _commands = new CommandService();

            _client.Log += Log;
            _commands.Log += Log;

            _client.Ready += Client_Ready;
            _client.SlashCommandExecuted += SlashCommandHandler;

            await _client.LoginAsync(TokenType.Bot, reader.GetValue("token", typeof(string)).ToString());
            await _client.StartAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        private Task Log(LogMessage message)
        {
            if (message.Exception is CommandException cmdException)
            {
                Console.WriteLine($"[Command/{message.Severity}] {cmdException.Command.Aliases.First()}" + $" failed to execute in {cmdException.Context.Channel}.");
                Console.WriteLine(cmdException);
            } 
            else { Console.WriteLine($"[General/{message.Severity}] {message}"); }

            return Task.CompletedTask;
        }
        public async Task Client_Ready()
        {
            //クライアント立ち上げ時の処理
            await Task.CompletedTask;
        }
        private async Task SlashCommandHandler(SocketSlashCommand command)
        {
            await command.RespondAsync("PROCESSING...");

            string result = string.Empty;
            if (command.CommandName == "docker")
            {
                result = Command_Docker.DockerCtrl(command);
                await command.ModifyOriginalResponseAsync(m => { m.Content = result; });
                return;
            }

            if (command.CommandName == "containers")
            {
                result = Command_Containers.ListUp(command);

                if (result.Length > 2000)
                {
                    Optional<IEnumerable<FileAttachment>> optional = new();
                    using (Stream stream = new MemoryStream()) 
                    {
                        StreamWriter sw = new StreamWriter(stream);
                        sw.Write(result);
                        stream.Position = 0;

                        //ファイル添付に必用な処理
                        FileAttachment fa = new FileAttachment(stream, DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".txt");
                        List<FileAttachment> flis = new List<FileAttachment>();
                        flis.Add(fa);
                        optional = new Optional<IEnumerable<FileAttachment>>(flis);

                        result = "文字数上限に達しました。sshから直接確認してください。";

                        await command.ModifyOriginalResponseAsync(m =>
                        {
                            m.Content = result;
                            m.Attachments = optional;
                        });
                    }
                    return;
                }

                await command.ModifyOriginalResponseAsync(m =>
                {
                    m.Content = $"```\n{result}\n```";
                });
                return;
            }

            if (command.CommandName == "reload")
            {
                result = Command_Reload.ContainersReload(command, reader);
                await command.ModifyOriginalResponseAsync(m => { m.Content = result; });
                return;
            }

            await command.ModifyOriginalResponseAsync(m => { m.Content = command.CommandName + " is not fonund."; });
        }
    }
}