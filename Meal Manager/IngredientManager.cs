using Meal_Planner.Client_Manager;
using Meal_Planner.Essential;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Meal_Planner.Meal_Manager
{
    public struct IngredientData
    {
        public string Name { get; private set; }
        public string Energy { get; private set; }
        public string Protein { get; private set; }
        public string Fat { get; private set; }
        public string Carbohydrate { get; private set; }
        public string PastActionMass { get; private set; }
        public string Mass { get; private set; }
        public string Path { get; private set; }

        private string Content;

        public IngredientData(string ingredient_id) 
        {
            Path = $"{Paths.ingredients_d}/{ingredient_id}";
            Mass = ingredient_id.Split('-')[1].Trim();
            string[] content = null;
            if (!File.Exists(Path)) content = new IngredientData("ERROR", "-1", "-1", "-1", "-1", "-1", "-1").GetContent().Split("\t");
            else content = File.ReadAllText(Path).Split('\t');
            Name = content[0];
            Energy = content[1];
            Protein = content[2];
            Fat = content[3];
            Carbohydrate = content[4];
            PastActionMass = content[5];
            Content = $"{Name}\t{Energy}\t{Protein}\t{Fat}\t{Carbohydrate}\t{PastActionMass}";
        }

        public IngredientData(string ingredient_name, string energy_value, string protein_value, string fat_value, string carbohydrate_value, string past_action_value ,string portion_type )
        {
            
            Name = ingredient_name;
            Energy = energy_value;
            Protein = protein_value;
            Fat = fat_value;
            Carbohydrate = carbohydrate_value;
            Mass = portion_type;
            PastActionMass = past_action_value;
            Path = $"{Paths.ingredients_d}/ingredient#{Directory.GetFiles(Paths.ingredients_d).Length} - {Mass} - {DateTime.Now.ToString("yyyy.MM.dd HH-mm-ss")}";
            Content = $"{Name}\t{Energy}\t{Protein}\t{Fat}\t{Carbohydrate}\t{PastActionMass}";
        }


        public void SaveAsNew()
        {
            Path = $"{Paths.ingredients_d}/ingredient#{Directory.GetFiles(Paths.ingredients_d).Length} - {Mass} - {DateTime.Now.ToString("yyyy.MM.dd HH-mm-ss")}.mp";
            File.WriteAllText(Path, Content);
        }
        public void SaveOverride(string override_path)
        {
            File.WriteAllText(override_path, Content);
        }

        public string GetContent() { return Content ; }
    }
    internal class IngredientManager
    {
        public static IngredientPreview selectedIngredientPreview;
        public static List<IngredientPreview> ingredient_list = new List<IngredientPreview>();
        public static string prev_search = "";
        public static void AddIngredientChildToGrid(Grid grid, string ingredientPath, int i, Ingredients ingredients)
        {
            FileInfo ingredientInfo = new FileInfo(ingredientPath);
            IngredientData data = new IngredientData(ingredientInfo.Name);
            IngredientPreview ingredientPreview = new IngredientPreview(ref data);
            ingredient_list.Add(ingredientPreview);
            ingredientPreview.HorizontalAlignment = HorizontalAlignment.Left;
            ingredientPreview.VerticalAlignment = VerticalAlignment.Top;
            ingredientPreview.Height = 120;
            ingredientPreview.Width = 1900;
            ingredientPreview.Margin = new Thickness(0, 130 * i, 0, 0);

            ingredientPreview.MouseDown += (o, e) =>
            {
                selectedIngredientPreview.SetColorBG("#FFA7A7A7");
                if (selectedIngredientPreview != null) foreach (Control control in selectedIngredientPreview.main_grid.Children) control.SetColorFG(Colors.Black);
                if (selectedIngredientPreview == ingredientPreview)
                {
                    selectedIngredientPreview = null;
                    if (ingredients.enable_search.IsChecked == true)
                    {
                        ingredients.ingredient_name.Text = prev_search;
                    }
                    return;
                }
                ingredientPreview.SetColorBG("#FF212121");
                foreach (UIElement control in ingredientPreview.main_grid.Children) ((Control)control).SetColorFG(Colors.White);

                string name = ingredients.ingredient_name.Text;
                ingredients.ingredient_name.Text = data.Name;
                if (selectedIngredientPreview == null)
                {
                    prev_search = name;

                }
                if (ingredients.enable_search.IsChecked == true) LoadSortedIngredients(prev_search);
                ingredients.energy_value.Text = data.Energy;
                ingredients.protein_value.Text = data.Protein;
                ingredients.fat_value.Text = data.Fat;
                ingredients.carbohydrate_value.Text = data.Carbohydrate;
                ingredients.past_action_value.Text = data.PastActionMass;
                switch (data.Mass)
                {
                    case "100g":
                        ingredients.portion_type.Text = data.Mass;
                        break;
                    case "db":
                        ingredients.portion_type.Text = data.Mass + " (50g)";
                        break;
                    default:
                        ingredients.portion_type.SelectedIndex = 2;
                        ingredients.custom_mass_value.Text = data.Mass;
                        break;
                }


                selectedIngredientPreview = ingredientPreview;
            };

            //ingredients.SizeChanged += (o, e) =>
            //{
            //    //ingredientPreview.SetRelativeSize(780, 50);
            //};

            grid.Children.Add(ingredientPreview);
        }
        public static void ReloadIngredientFiles(Grid grid, Ingredients ingredient)
        {
            ingredient_list.Clear();
            grid.Children.Clear();
            string[] ingredients = Directory.GetFiles(Paths.ingredients_d, "*.mp");
            new Thread(() =>
            {

                for (int i = 0; i < ingredients.Length; i++)
                {
                    ingredient.Dispatcher.Invoke(() =>
                    {
                        AddIngredientChildToGrid(grid, ingredients[i], i, ingredient);
                        
                    });
                    Thread.Sleep(2);
                }

            }).Start();
        }
        public static void LoadIngredients(Ingredients ingredients)
        {
            for (int i = 0; i < ingredient_list.Count; i++)
            {
                IngredientPreview ip = ingredient_list[i];
                ip.Visibility = Visibility.Visible;
                ip.Margin = new Thickness(0, 130 * i, 0, 0);
                if(ip == selectedIngredientPreview)
                {
                    ingredients.scrollviewer.ScrollToTop();
                    ingredients.scrollviewer.ScrollToVerticalOffset(i * 130);
                }
            }
        }

        public static void LoadSortedIngredients(string sortByName)
        {
            int hit = 0;
            for(int i = 0; i < ingredient_list.Count; i++)
            {
                IngredientPreview ip = ingredient_list[i];
                IngredientData id = ip.ingredient_data;
                if (!id.Name.ToLower().Contains(sortByName.ToLower()) && sortByName != "") ip.Visibility = Visibility.Collapsed;
                else
                {
                    ip.Visibility = Visibility.Visible;
                    ip.Margin = new Thickness(0, 130 * hit, 0, 0);
                    
                    hit++;
                }
            }
        }

        public static void DeleteIngredient(Grid grid)
        {
            if (selectedIngredientPreview == null)
            {
                MessageBox.Show("Nincs kiválasztott alapanyag.", "Error");
                return;
            }
            try
            {
                FileInfo fi = new FileInfo(selectedIngredientPreview.ingredient_data.Path);
                fi.Delete();
                ingredient_list.Remove(selectedIngredientPreview);
                grid.Children.Remove(selectedIngredientPreview);
                selectedIngredientPreview = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR");
            }
            
        }

        public static void ImportFromClipboard()
        {
            string cpt = Clipboard.GetText();
            string[] lines = cpt.Split(Environment.NewLine);
            string formatted_clipboard = "";
            foreach (string line in lines)
            {
                formatted_clipboard += line.Replace("\t"," // ") + "\n\n";
            }
            MessageBoxResult result = MessageBox.Show($"Beimportálod a következő adatokat?\n{formatted_clipboard}", "Warning", MessageBoxButton.YesNo);
            if (result != MessageBoxResult.Yes) return;
            bool error = false;
            string errormsg = "Nem sikerült be importálni a következő elemeket:";
            lines.ToList().ForEach(line =>
            {
                string[] datas = line.Split('\t');
                IngredientData data;
                try
                {
                    data = new IngredientData(datas[0], datas[1], datas[2], datas[3], datas[4], "100g", "100g");
                    data.SaveAsNew();
                }
                catch (Exception ex)
                {
                    error = true;
                    errormsg += $"\n\nTábla:\n{line.Replace("\t"," // ")}" +
                    $"\nHibaüzenet ->\n{ex.Message}\n------------------------------------------------------";
                }
            });
            if(error)
            {
                MessageBox.Show(errormsg,"Error");
            }
        }
    }
}
