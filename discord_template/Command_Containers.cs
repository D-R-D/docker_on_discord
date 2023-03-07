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
            Console.WriteLine($"container listup");
            try
            {
                result = ConsoleCommandRunner.GetCommandResult("\"docker ps -a\"");
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            return result;
        }
    }
}
