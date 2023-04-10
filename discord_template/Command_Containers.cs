namespace docker_on_discord
{
    internal class Command_Containers
    {
        public static string ListUp() 
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
