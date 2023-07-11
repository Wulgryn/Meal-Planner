using Meal_Planner.Client_Manager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Meal_Planner.Meal_Manager
{
    /// <summary>
    /// Interaction logic for RecipeDescription.xaml
    /// </summary>
    public partial class RecipeDescription : Window
    {
        public RecipeDescription()
        {
            InitializeComponent();
        }
        public RecipeData recipe_data;
        public RecipeDescription(ref RecipeData recipeData)
        {
            InitializeComponent();
            recipe_data = recipeData;
            FileInfo fi = new FileInfo(recipe_data.DescriptionPath);
            Run run = new Run("");
            
            if(fi.Exists) run = new Run(File.ReadAllText(fi.FullName).Trim());
            Paragraph p = new Paragraph(run);
            p.LineHeight = 1;
            description.Document.Blocks.Clear();
            description.Document.Blocks.Add(p);
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(!saved && !PromptClose()) e.Cancel = true;
        }

        public bool PromptClose()
        {
            MessageBoxResult result = MessageBox.Show("Biztos bezárod mentés nélkül?\nMinden változtatásod el fog veszni.","Warning",MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes) return true;
            return false;
        }
        bool saved = false;
        private void save_Click(object sender, RoutedEventArgs e)
        {
            FileInfo fi = new FileInfo(recipe_data.DescriptionPath);
            File.WriteAllText(fi.FullName, new TextRange(description.Document.ContentStart, description.Document.ContentEnd).Text.Trim());
            saved = true;
            Close();
        }
    }
}
