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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Meal_Planner.Client_Manager
{
    /// <summary>
    /// Interaction logic for ClientCreator.xaml
    /// </summary>
    public partial class ClientCreator : Page
    {
        Clients main_window;
        OptionsPage options;
        public ClientCreator(Clients main,OptionsPage optionsPage)
        {
            InitializeComponent();
            main_window = main;
            options = optionsPage;
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("A változtatások el fognak veszni, biztos vissza mész?", "Warning", MessageBoxButton.YesNo);
            if (result != MessageBoxResult.Yes) return;
            main_window.menu_frame.Content = options;
        }

        private void create_Click(object sender, RoutedEventArgs e)
        {
            string name = client_name.Text;
            string description = new TextRange(client_description.Document.ContentStart, client_description.Document.ContentEnd).Text;
            if(ClientManager.ClientExist(name)) 
            {
                MessageBoxResult result = MessageBox.Show("Már létezik kliens ilyen névvel. Ajánlatos megkülönböztetésűl valamit mögé" +
                    " írni a könnyeb felismerés végett.\nBiztos létrehozod így?", "Warning", MessageBoxButton.YesNo);
                if (result != MessageBoxResult.Yes) return;
                name = name + "$0";
                int i = 0;
                while(ClientManager.ClientExist(name)) 
                {
                    name = name.Split('$')[0] + "$" + i;
                    i++;
                }
            }
            ClientManager.CreateClient(name, description);
            main_window.LoadCLients();
            main_window.menu_frame.Content = options;
        }
    }
}
