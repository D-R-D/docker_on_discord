using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace docker_on_discord
{
    internal class Command_Containers
    {
        public static string ListUp(SocketSlashCommand command) 
        {
            string result = "warn: result write";
            try
            {
                result = ConsoleCommandRunner.GetCommandResult("\"docker ps -a\"");
                result = "```\n" + result + "\n```";
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }
    }
}
