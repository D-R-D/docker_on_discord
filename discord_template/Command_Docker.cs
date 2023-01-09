using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace docker_on_discord
{
    internal class Command_Docker
    {
        public static string DockerCtrl(SocketSlashCommand command)
        {
            string result = "warn: result write";

            try
            {
                string firstval = command.Data.Options.First().Value.ToString()!;
                string lastval = command.Data.Options.Last().Value.ToString()!;

                Console.WriteLine($"docker ctrl: F[{firstval}]/L[{lastval}]");

                if (firstval == "start")
                {
                    result = ConsoleCommandRunner.GetCommandResult($"\"docker start {lastval}\"");
                    return result;
                }

                if (firstval == "stop")
                {
                    result = ConsoleCommandRunner.GetCommandResult($"\"docker stop {lastval}\"");
                    return result;
                }

                if (firstval == "restart")
                {
                    result = ConsoleCommandRunner.GetCommandResult($"\"docker restart {lastval}\"");
                    return result;
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            return result;
        }
    }
}
