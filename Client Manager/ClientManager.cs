using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Meal_Planner.Client_Manager
{
    /// <summary>
    /// Build:<br/>
    /// Data/bin/clients<br/>
    /// -----------------/client name<br/>
    /// ------------------------------/sesion.lock  :: created date \mod modified date<br/>
    /// ------------------------------/days   :: the whole table stored<br/>
    /// ------------------------------/description.txt :: description of client
    /// </summary>
    public struct ClientData
    {
        public string FolderName { get; private set; }
        public string Name { get; private set; }
        public string CreatedDate { get; private set; }
        public string LastAccessedDate { get; private set; }
        public string Description { get; private set; }
        public string Path { get; private set; }

        public bool ErrorLoad { get; private set; }

        public ClientData(string client_name)
        {
            Path = Paths.clients_d + $"/{client_name}";
            FolderName = client_name;
            Name = client_name.Split('$')[0];
            string[] session = File.ReadAllText($"{Path}/session.lock").Split("/mod");
            CreatedDate = session[0];
            LastAccessedDate = session[0] == session[1] ? "-" : session[1];
            Description = File.ReadAllText($"{Path}/description.txt");
            ErrorLoad = false;
        }

        public ClientData(string client_name, bool errorload)
        {
            Path = Paths.clients_d + $"/{client_name}";
            FolderName = client_name;
            Name = client_name.Split('$')[0];
            if(!errorload)
            {
                string[] session = File.ReadAllText($"{Path}/session.lock").Split("/mod");
                CreatedDate = session[0];
                LastAccessedDate = session[0] == session[1] ? "-" : session[1];
                Description = File.ReadAllText($"{Path}/description.txt");
            }
            CreatedDate = "ERROR";
            LastAccessedDate = "ERROR";
            Description = "A fájlokat nem lehetett betölteni...";
            ErrorLoad = errorload;
        }

        public string GetCreationDate()
        {
            if (ErrorLoad) return "ERROR";
            return DateTime.Parse(CreatedDate).ToString("yyyy.MM.dd");
        }

        public string GetModifiedDate()
        {
            if (ErrorLoad) return "ERROR";
            return LastAccessedDate == "-" ? "-" : DateTime.Parse(LastAccessedDate).ToString("yyyy.MM.dd");
        }

        public void SaveDescription(string description)
        {
            File.WriteAllText($"{Path}/description.txt",description);
        }
        public void SaveDescription(RichTextBox description)
        {
            File.WriteAllText($"{Path}/description.txt", description.ToString());
        }
    }

    class ClientManager
    {
        public static ClientPreview selectedClient = null;
        public static void LoadClientPreviews(Grid client_list, Clients clients)
        {
            ClientData[] clientDatas = ClientDatas();
            for(int i = 0; i < clientDatas.Length; i++)
            {
                ClientPreview clientPreview = new ClientPreview(ref clientDatas[i]);
                clientPreview.Margin = new Thickness(10,10 + 55 * i,0,0);
                clientPreview.Width = 460;
                clientPreview.Height = 45;
                clientPreview.VerticalAlignment = VerticalAlignment.Top;
                clientPreview.HorizontalAlignment = HorizontalAlignment.Left;

                clientPreview.MouseDown += (o, e) =>
                {
                    selectedClient.SetColorBG("#FFA7A7A7");
                    if(selectedClient != null)foreach (Control control in selectedClient.main_grid.Children) control.SetColorFG(Colors.Black);
                    clientPreview.SetColorBG("#FF212121");
                    foreach (UIElement control in clientPreview.main_grid.Children) ((Control)control).SetColorFG(Colors.White);
                    selectedClient = clientPreview;
                    clients.menu_frame.Content = new OptionsPage(clients, clientPreview.clientData.Description);
                };

                client_list.Children.Add(clientPreview);
            }
        }

        public static ClientData[] ClientDatas()
        {
            string[] client_dirs = Directory.GetDirectories(Paths.clients_d);
            ClientData[] clientDatas = new ClientData[client_dirs.Length];
            for (int i = 0; i < client_dirs.Length; i++)
            {
                DirectoryInfo client_dir = new DirectoryInfo(client_dirs[i]);
                try
                {
                    clientDatas[i] = new ClientData(client_dir.Name);
                }
                catch (FileNotFoundException fex)
                {
                    clientDatas[i] = new ClientData(client_dir.Name,true);
                }
                catch (Exception ex) 
                {
                    MessageBox.Show(ex.Message,"Error");
                }
                
            }
            return clientDatas;
        }

        public static bool ClientExist(string clientname)
        {
            return Directory.Exists(Paths.clients_d + "/" + clientname);
        }

        public static void CreateClient(string clientname,string description) 
        {
            DirectoryInfo client_d = new DirectoryInfo(Paths.clients_d + "/" + clientname);
            client_d.Create();
            File.WriteAllText(Paths.clients_d + "/" + clientname + "/description.txt", description);
            File.WriteAllText(Paths.clients_d + "/" + clientname + "/session.lock", $"{client_d.CreationTime}/mod{client_d.CreationTime}");
        }
    
        public static void DeleteClient()
        {
            if(selectedClient == null)
            {
                MessageBox.Show("Nincs kiválasztott kliens.","Error");
                return;
            }
            try
            {
                DirectoryInfo di = new DirectoryInfo(selectedClient.clientData.Path);
                DeleteDirectory(di.FullName);
                selectedClient = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR");
            }
        }
        public static void DeleteDirectory(string target_dir)
        {
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(target_dir, false);
        }
    }

    static class Ext
    {
        public static void SetColorBG(this Control control,string hex)
        {
            if (control == null) return;
            control.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(hex));
        }
        public static void SetColorBG(this Control control, Color color)
        {
            if (control == null) return;
            control.Background = new SolidColorBrush(color);
        }
        public static void SetColorFG(this Control control, string hex)
        {
            if (control == null) return;
            control.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(hex));
        }
        public static void SetColorFG(this Control control, Color color)
        {
            if (control == null) return;
            control.Foreground = new SolidColorBrush(color);
        }
        public static string ToString(this RichTextBox rtb)
        {
            return new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd).Text;
        }
    }
}
