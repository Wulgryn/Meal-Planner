using System.Diagnostics;

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
                    Process.Start("Data/bin/Meal Planner.exe");
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