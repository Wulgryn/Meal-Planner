using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace Meal_Planner.Client_Manager
{
    /// <summary>
    /// Interaction logic for Clients.xaml
    /// </summary>
    public partial class Clients : Window
    {
        public Clients()
        {
            InitializeComponent();
            menu_frame.Content = new OptionsPage(this);
            LoadCLients();
        }

        public void LoadCLients()
        {
            ClientManager.selectedClient = null;
            menu_frame.Content = new OptionsPage(this);
            client_list.Children.Clear();
            ClientManager.LoadClientPreviews(client_list,this);
        }
    }
}
