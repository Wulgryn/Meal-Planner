using System.Diagnostics;
using System.Reflection;

namespace Updater
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Updating Meal Planner...");
            try
            {
                Directory.CreateDirectory("Data/bin");
                Process process = Process.Start("Data\\GRM\\Github Release Manger.exe", $"download 'MP' '{new DirectoryInfo(".").FullName}/Data/bin'");

                process.WaitForExit();

                if (File.Exists("Data/bin/Meal Planner.exe"))
                {
                    Console.WriteLine("Starting application...");
                    ProcessStartInfo psi = new ProcessStartInfo();
                    psi.FileName = "cmd.exe";
                    psi.WorkingDirectory = $"{new DirectoryInfo(".").FullName}/Data/bin";
                    psi.Arguments = "/c \"call \"Meal Planner.exe\"\"";
                    psi.CreateNoWindow = true;
                    Process.Start(psi);
                    Environment.Exit(0);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\n\nERROR -> Message: {0}\n\n", ex.Message);
            }
            Console.Write("Press any key to continue...");
            Console.CursorVisible = false;
            Console.ReadKey();
        }
    }
}
