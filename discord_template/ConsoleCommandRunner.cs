﻿using System.Diagnostics;

namespace docker_on_discord
{
    internal class ConsoleCommandRunner
    {
        public static string GetCommandResult(string command)
        {
            Console.WriteLine($"/bin/bash command.sh {command}");
            if (Tools.IsNullOrEmpty(command)) { throw new ArgumentNullException(nameof(command)); }

            string result = "";
            ProcessStartInfo processStartInfo = new ProcessStartInfo("/bin/bash", "command.sh " + command);
            processStartInfo.CreateNoWindow = true;
            processStartInfo.UseShellExecute = false;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardError = true;

            Process process = Process.Start(processStartInfo)!;

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            process.WaitForExit();
            process.Close();

            if (output != "")
            {
                result += output;
            }
            if (error != "")
            {
                result += error;
            }

            return result;
        }
    }
}
