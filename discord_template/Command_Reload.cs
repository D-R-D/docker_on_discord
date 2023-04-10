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
