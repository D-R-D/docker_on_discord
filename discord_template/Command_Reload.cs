using Discord.WebSocket;
using discord_template;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace docker_on_discord
{
    internal class Command_Reload
    {
        public static string ContainersReload(SocketSlashCommand command, AppSettingsReader reader)
        {
            string result = "warn: result write";
            try
            {
                CommandBuilder commandbuilder = new CommandBuilder("docker.json");
                commandbuilder.CommandPush();

                Ids ids = new Ids(reader);

                CommandSender commandSender = new CommandSender(Directory.GetCurrentDirectory() + "/commands", ids);
                commandSender.RequestSender();

                result = "Reload Finish.";
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            return result;
        }
    }
}
