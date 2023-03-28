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
        public static string ContainersReload()
        {
            string result = "warn: result write";
            try
            {
                string ContainerNames = ConsoleCommandRunner.GetCommandResult("\"docker ps -a --format {{.Names}}\"");
                string[] ContainerArray = ContainerNames.Split('\n');
                Settings.Shared.m_ContainerList = new List<string>(ContainerArray);

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
