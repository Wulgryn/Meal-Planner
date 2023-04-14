using GRM_Build_Tracker;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Meal_Planner
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Az argumentumokat tartalmazó tömb
            string[] args = e.Args;
            BuildTracker.AddBuildNumber(ref args);
            // A további inicializációs logika
            // ...
            CheckBaseFolders(Paths.Data_d);
            CheckBaseFolders(Paths.bin_d);
            CheckBaseFolders(Paths.clients_d);
            CheckBaseFolders(Paths.ingredients_d);
            CheckBaseFolders(Paths.recipes_d);
        }

        private void CheckBaseFolders(string path)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            if (!directoryInfo.Exists) directoryInfo.Create();
        }
    }
}
