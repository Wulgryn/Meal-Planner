using Meal_Planner.Client_Manager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Meal_Planner.Meal_Manager
{
    public struct RecipeData
    {
        public string Name { get; set; }
        public string Path { get; private set; }
        public string IngredientsPath { get; private set; }
        public string DescriptionPath { get; private set; }

        public List<RecipeIngredientPreview> ingredients = new List<RecipeIngredientPreview>();

        public RecipeData(string recipe_id)
        {
            Path = $"{Paths.recipes_d}/{recipe_id}";
            IngredientsPath = Path + "/ingredients.mp";
            DescriptionPath = Path + "/description.txt";
            //string[] content = ;
            Name = recipe_id.Split('-')[1];
        }
        public void CalcValues(RecipePreview rp)
        {
            rp.NullValues();
            foreach (RecipeIngredientPreview ingredientPreview in ingredients)
            {
                IngredientData id = ingredientPreview.IngredientData;
                double mass = ingredientPreview.mass_value.Text.cstd();
                double portion = id.Mass == "db" ? "50".cstd() : id.Mass.Trim('g').cstd();
                double rate = mass / portion;

                double energy = id.Energy.EqualsOR("","-") ? 0 : id.Energy.cstd();
                double protein = id.Protein.EqualsOR("", "-") ? 0 : id.Protein.cstd();
                double fat = id.Fat.EqualsOR("", "-") ? 0 : id.Fat.cstd();
                double carbohydrate = id.Carbohydrate.EqualsOR("", "-") ? 0 : id.Carbohydrate.cstd();
                double past_action_mass = id.PastActionMass.EqualsOR("", "-") ? 0 
                    : id.PastActionMass[0].EqualsOR('x','X') ? id.PastActionMass.Trim('x').cstd() * mass 
                    : id.PastActionMass.Contains("g") ? (id.PastActionMass.Trim('g').cstd() / portion) * mass : mass;

                rp.mass_value.Content = (rp.mass_value.Content.ToString().cstd() + past_action_mass).ToString("0.###");
                rp.energy_value.Content = (rp.energy_value.Content.ToString().cstd() + energy * rate).ToString("0.###");
                rp.protein_value.Content = (rp.protein_value.Content.ToString().cstd() + protein * rate).ToString("0.###");
                rp.fat_value.Content = (rp.fat_value.Content.ToString().cstd() + fat * rate).ToString("0.###");
                rp.carbohydrate_value.Content = (rp.carbohydrate_value.Content.ToString().cstd() + carbohydrate * rate).ToString("0.###");
            }
        }
        public void ReloadIngredientList(RecipePreview rp = null)
        {
            ingredients.Clear();
            string[] ingredient_names = null;
            if ( rp != null ) rp.ingredient_list.Children.Clear();
            try
            {
                ingredient_names = File.ReadAllLines(IngredientsPath);
            }
            catch (Exception)
            {
                ingredient_names = new string[] { "ERROR\\mass-1" };
            }

            foreach (string ingredient_name in ingredient_names)
            {
                string file = ingredient_name.Split("\\mass")[0];
                string mass = ingredient_name.Split("\\mass")[1];
                FileInfo ingredient = new FileInfo(file);
                IngredientData id;
                if (file != "ERROR") id = new IngredientData(ingredient.Name);
                else id = new IngredientData("ERROR", "-1", "-1", "-1", "-1", "-1", "-1");
                if (rp != null) rp.ingredient_list.Children.Add(LoadIngredients(id, mass, rp));
            }
            if (rp != null)
            {
                rp.ingredient_list.Height = ingredients.Count * 130 + 38;
                CalcValues(rp);
            }

            //
            //On error create fake recipe
            //


        }
        public RecipeIngredientPreview LoadIngredients(IngredientData ingredientData, string recipeIngredientPreviewMass,RecipePreview rp)
        {
            RecipeIngredientPreview rip = new RecipeIngredientPreview(ref ingredientData,rp);
            
            rip.Width = 1700;
            rip.Height = 120;
            rip.VerticalAlignment = VerticalAlignment.Top;
            rip.HorizontalAlignment = HorizontalAlignment.Left;
            rip.Margin = new Thickness(0,130 * ingredients.Count,0,0);
            rip.mass_value.Text = recipeIngredientPreviewMass == "db" ? "50" : recipeIngredientPreviewMass.Trim('g');

            rip.mass_value.LostFocus += (sender, args) =>
            {
                rip.recipePreview.recipe_data.SaveOverride();
            };

            ingredients.Add(rip);
            return rip;
        }

        public void AddIngredient(IngredientData ingredientData,RecipePreview rp = null)
        {
            List<string> ingredients = File.ReadAllLines(IngredientsPath).ToList();
            ingredients.Add(ingredientData.Path + "\\mass" + ingredientData.Mass);
            File.WriteAllLines(IngredientsPath, ingredients);
            ReloadIngredientList(rp);
        }


        public void SaveAsNew()
        {
            Path = $"{Paths.recipes_d}/recipe#{Directory.GetDirectories(Paths.recipes_d).Length} - {Name} - {DateTime.Now.ToString("yyyy.MM.dd HH-mm-ss")}";
            IngredientsPath = Path + "/ingredients.mp";
            Directory.CreateDirectory(Path);
            FileStream fs = File.Create(IngredientsPath);
            
            fs.Flush();
            fs.Close();
        }
        public void SaveNewName(string newName)
        {
            DirectoryInfo di = new DirectoryInfo(Path);
            string newPath = di.Parent.FullName + "/" + di.Name.Replace(Name,$" {newName} ");
            Name = newName;
            Directory.Move(Path,newPath);
            Path = newPath;
        }
        public void SaveOverride()
        {
            List<string> lines = new List<string>();
            foreach(RecipeIngredientPreview rip in ingredients)
            {
                if(rip.IngredientData.Name == "ERROR")
                {
                    MessageBox.Show("Hibás érték(ek)! Ezek nem kerülnek mentésre.","Error");
                    continue;
                }
                lines.Add($"{rip.IngredientData.Path}\\mass{rip.mass_value.Text}");
            }
            File.WriteAllLines(IngredientsPath, lines);
        }
    }
    class RecipeManager
    {
        public static List<RecipePreview> recipe_list = new List<RecipePreview>();
        public static RecipePreview selectedRecipe = null;
        public static RecipeIngredientPreview selectedRIP = null;
        public static void AddRecipeChildToGrid(Grid grid, string recipePath, Recipes recipes, int i)
        {
            DirectoryInfo ingredientInfo = new DirectoryInfo(recipePath);
            RecipeData data = new RecipeData(ingredientInfo.Name);
            RecipePreview recipePreview = new RecipePreview(ref data,recipes);
            recipe_list.Add(recipePreview);
            recipePreview.HorizontalAlignment = HorizontalAlignment.Left;
            recipePreview.VerticalAlignment = VerticalAlignment.Top;
            recipePreview.Height = 168;
            recipePreview.Width = 1900;
            recipePreview.Margin = new Thickness(0, 168 * i, 0, 0);

            recipePreview.values_grid.MouseDown += (o, e) =>
            {
                
                if(selectedRecipe != null)
                {
                    selectedRecipe.values_grid.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF90C3C8"));
                    foreach (Control control in selectedRecipe.values_grid.Children) control.SetColorFG(Colors.Black);
                }
                if(selectedRecipe == recipePreview)
                {
                    selectedRecipe.values_grid.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF90C3C8"));
                    foreach (Control control in selectedRecipe.values_grid.Children) control.SetColorFG(Colors.Black);
                    selectedRecipe = null;
                }
                else
                {
                    recipePreview.values_grid.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3D5659"));
                    foreach (Control control in recipePreview.values_grid.Children) control.SetColorFG(Colors.White);
                    selectedRecipe = recipePreview;
                    recipes.recipe_name.Text = recipePreview.recipe_data.Name;

                    if (selectedRIP != null)
                    {
                        selectedRIP.main_grid.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF90E0C8"));
                        selectedRIP.mass_value.SetColorBG("#FF90E0C8");
                        foreach (Control control in selectedRIP.main_grid.Children) control.SetColorFG(Colors.Black);
                        selectedRIP = null;
                        recipePreview.recipes.delete_recipe.Content = "Törlés";
                    }
                }
            };

            

            grid.Children.Add(recipePreview);
        }

        public static void ReloadRecipeFolder(Grid grid, Recipes recipes)
        {
            recipe_list.Clear();
            grid.Children.Clear();
            string[] recipe_dirs = Directory.GetDirectories(Paths.recipes_d);
            int off = 0;
            new Thread(() =>
            {

                for (int i = 0; i < recipe_dirs.Length; i++)
                {
                    DirectoryInfo ingredientInfo = new DirectoryInfo(recipe_dirs[i]);
                    string[] data_del = ingredientInfo.Name.Split("-");
                    if (data_del[data_del.Length - 1].Trim() == "del")
                    {
                        off++;
                        continue;
                    }
                    recipes.Dispatcher.Invoke(() =>
                    {

                        AddRecipeChildToGrid(grid, recipe_dirs[i], recipes, i - off);
                    });
                    Thread.Sleep(2);
                }

            }).Start();
        }

        public static void LoadRecipes(Recipes recipes)
        {
            for (int i = 0; i < recipe_list.Count; i++)
            {
                RecipePreview rp = recipe_list[i];
                rp.Visibility = Visibility.Visible;
                rp.Margin = new Thickness(0, 168 * i, 0, 0);
                if (rp == selectedRecipe)
                {
                    recipes.scrollviewer.ScrollToTop();
                    recipes.scrollviewer.ScrollToVerticalOffset(i * 168);
                }
            }
        }

        public static void SaveRecipe(Recipes recipes)
        {
            if(selectedRecipe != null) 
            {
                selectedRecipe.recipe_data.SaveNewName(recipes.recipe_name.Text);
                selectedRecipe.recipe_name.Content = recipes.recipe_name.Text;
                return;
            }
            RecipeData data = new RecipeData();
            data.Name = recipes.recipe_name.Text;
            data.SaveAsNew();
            AddRecipeChildToGrid(recipes.recipe_list, data.Path,recipes, recipe_list.Count);
        }

        public static void DeleteRecipe()
        {
            if (selectedRecipe == null)
            {
                MessageBox.Show("Nincs kiválasztott recept.", "Error");
                return;
            }
            try
            {
                DirectoryInfo di = new DirectoryInfo(selectedRecipe.recipe_data.Path);
                di.MoveTo(di.FullName + " - del");
                DeleteDirectory(di.FullName);
                recipe_list.Remove(selectedRecipe);
                selectedRecipe = null;
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
}
