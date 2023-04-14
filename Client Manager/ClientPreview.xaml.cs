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
    /// Interaction logic for ClientPreview.xaml
    /// </summary>
    public partial class ClientPreview : UserControl
    {
        public ClientPreview()
        {
            InitializeComponent();
        }
        public ClientData clientData;
        public ClientPreview(ref ClientData client_data)
        {
            InitializeComponent();
            client_name.Content = client_data.Name;
            created_date.Content = client_data.GetCreationDate();
            modified_date.Content = client_data.GetModifiedDate();
            clientData = client_data;
        }
    }
}
