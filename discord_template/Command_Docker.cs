namespace docker_on_discord
{
    internal class Command_Docker
    {
        public static string DockerCtrl(string funcName, string containerName)
        {
            string result = "warn: result write";

            try
            {
                Console.WriteLine($"docker ctrl: F[{funcName}]/L[{containerName}]");

                if (funcName == "start")
                {
                    result = ConsoleCommandRunner.GetCommandResult($"\"docker start {containerName}\"");
                    return result;
                }

                if (funcName == "stop")
                {
                    result = ConsoleCommandRunner.GetCommandResult($"\"docker stop {containerName}\"");
                    return result;
                }

                if (funcName == "restart")
                {
                    result = ConsoleCommandRunner.GetCommandResult($"\"docker restart {containerName}\"");
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
