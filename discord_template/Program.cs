using Discord;
using Discord.Commands;
using Discord.WebSocket;
using docker_on_discord;
using System.Configuration;
using System.Reflection.Emit;
using System.Text;

namespace discord_template
{
    class Program
    {
        public static AppSettingsReader reader = new AppSettingsReader();

        private DiscordSocketClient? _client;
        private static CommandService? _commands;

        public static void Main(string[] args)
        {
            CommandSender.RegisterGuildCommands();

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
            _client.SelectMenuExecuted += SelectMenuHandler;

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
            _ = Task.Run(async () =>
            {
                try
                {
                    await command.RespondAsync("PROCESSING...");

                    string result = string.Empty;
                    if (command.CommandName == "docker" && Settings.Shared.m_AdminIds.Contains(command.User.Id.ToString()))
                    {
                        string firstval = command.Data.Options.First().Value.ToString()!;

                        Console.WriteLine(firstval);

                        SelectMenuBuilder menuBuilder = await SelectMenuEditor.CreateContainerMenu(firstval, 0);
                        ComponentBuilder builder = new ComponentBuilder().WithSelectMenu(menuBuilder);

                        //await command.RespondAsync("以下の選択肢からコンテナを選択してください", components: builder.Build(), ephemeral: true);
                        await command.ModifyOriginalResponseAsync(m =>
                        {
                            m.Content = "以下の選択肢からコンテナを選択してください";
                            m.Components = builder.Build();
                        });
                        return;
                    }

                    if (command.CommandName == "containers")
                    {
                        result = Command_Containers.ListUp();
                        string[] resultline = result.Split('\n');
                        string linedresult = string.Empty;
                        foreach (string line in resultline)
                        {
                            linedresult += $"{line}\n";
                        }
                        result = linedresult.TrimEnd('\r', '\n');

                        if (result.Length > 2000)
                        {
                            Optional<IEnumerable<FileAttachment>> optional = new();
                            using (Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(result)))
                            {
                                //ファイル添付に必用な処理
                                FileAttachment fa = new FileAttachment(stream, DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".txt");
                                List<FileAttachment> flis = new List<FileAttachment>();
                                flis.Add(fa);
                                optional = new Optional<IEnumerable<FileAttachment>>(flis);

                                result = $"文字数上限に達しました。sshから直接確認してください。\n文字数:[{result.Length}]";

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
                        result = Command_Reload.ContainersReload();
                        await command.ModifyOriginalResponseAsync(m => { m.Content = result; });
                        return;
                    }

                    await command.ModifyOriginalResponseAsync(m => { m.Content = command.CommandName + " is not fonund."; });
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    if (command.HasResponded)
                    {
                        await command.ModifyOriginalResponseAsync(m => { m.Content = ex.Message; });
                        return;
                    }
                    await command.RespondAsync(ex.Message);
                }
            });

            await Task.CompletedTask;
        }

        //
        //セレクトメニューのイベント処理
        private static async Task SelectMenuHandler(SocketMessageComponent arg)
        {
            _ = Task.Run(async () =>
            {
                await arg.DeferAsync(ephemeral: true);
                try
                {
                    string funcName = arg.Data.CustomId;
                    string[] selecteditem = arg.Data.Values.First().Split('@');

                    if (selecteditem[0] == "page")
                    {
                        SelectMenuBuilder menuBuilder = await SelectMenuEditor.CreateContainerMenu(funcName, int.Parse(selecteditem[1]));
                        ComponentBuilder builder = new ComponentBuilder().WithSelectMenu(menuBuilder);

                        await arg.ModifyOriginalResponseAsync(m =>
                        {
                            m.Content = "以下の選択肢からコンテナを選択してください";
                            m.Components = builder.Build();
                        });
                        return;
                    }

                    string result = Command_Docker.DockerCtrl(funcName, selecteditem[1]);
                    await arg.ModifyOriginalResponseAsync(m => { m.Content = result; });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    if (arg.HasResponded)
                    {
                        await arg.ModifyOriginalResponseAsync(m => { m.Content = ex.Message; });
                        return;
                    }
                    await arg.RespondAsync(ex.Message);
                }
            });

            await Task.CompletedTask;
        }
    }
}