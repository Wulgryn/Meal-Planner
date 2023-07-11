using Meal_Planner.Meal_Manager;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Meal_Planner.Client_Manager
{
    /// <summary>
    /// Interaction logic for OptionsPage.xaml
    /// </summary>
    public partial class OptionsPage : Page
    {
        Clients main_window;
        public OptionsPage(Clients main,string description = "Még nincs kliens kiválasztva...")
        {
            InitializeComponent();
            main_window = main;
            Paragraph p = new Paragraph(new Run(description));
            p.LineHeight = 1;
            client_description.Document.Blocks.Clear();
            client_description.Document.Blocks.Add(p);
        }

        private void create_client_Click(object sender, RoutedEventArgs e)
        {
            main_window.menu_frame.Content = new ClientCreator(main_window,this);
        }

        private void delete_client_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Biztos vagy benne?\nA kliens és adatai örökre el fognak veszni! (Az sok idő)", "Warning", MessageBoxButton.YesNo);
            if (result != MessageBoxResult.Yes) return;
            ClientManager.DeleteClient();
            main_window.LoadCLients();
        }

        private void ingredients_Click(object sender, RoutedEventArgs e)
        {
            Ingredients ingredients = new Ingredients();
            ingredients.Activate();
            ingredients.Owner = main_window;
            ingredients.Show();
            main_window.Hide();
        }

        private void recipes_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("Munkálatok alatt...");
            //return;
            Recipes recipes = new Recipes();
            recipes.Activate();
            recipes.Owner = main_window;
            recipes.Show();
            main_window.Hide();
        }

        private void load_client_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Munkálatok alatt...");
        }
    }
}
