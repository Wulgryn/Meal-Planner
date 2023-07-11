using Meal_Planner.Client_Manager;
using Meal_Planner.Essential;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Interaction logic for Ingredients.xaml
    /// </summary>
    public partial class Ingredients : Window
    {
        public Ingredients()
        {
            InitializeComponent();
            IsOwnerShown = false;
            IngredientManager.ReloadIngredientFiles(ingredient_list, this);
            custom_mass_value.Visibility = Visibility.Hidden;
            ReloadIngredientList();
        }
        public RecipePreview recipePreview;
        public Ingredients(RecipePreview recipePreview)
        {
            InitializeComponent();
            custom_mass_value.Visibility = Visibility.Hidden;
            this.recipePreview = recipePreview;
            IsOwnerShown = false;
            IngredientManager.ReloadIngredientFiles(ingredient_list, this);
            ReloadIngredientList();
            save.Content = "Hozzáadás";
            Owner = recipePreview.recipes;
            IsOwnerShown = true;
        }
        bool IsOwnerShown = false;
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (IsOwnerShown) return;
            Owner.Close();
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            Owner.Show();
            IsOwnerShown = true;
            Close();
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            if(save.Content == "Hozzáadás")
            {
                if(IngredientManager.selectedIngredientPreview == null)
                {
                    MessageBox.Show("Nincs kiválasztott alapanyag.", "Error");
                    return;
                }
                recipePreview.recipe_data.AddIngredient(IngredientManager.selectedIngredientPreview.ingredient_data,recipePreview);
                recipePreview.Expand();
                recipePreview.recipes.RearrangeRecipes();
                recipePreview.recipe_data.SaveOverride();
                recipePreview.recipe_data.CalcValues(recipePreview);
                //recipePreview.LoadAllRecipeIngredientPreview();
                return;
            }
            IngredientData data = new IngredientData(ingredient_name.Text,energy_value.Text
                ,protein_value.Text,fat_value.Text,carbohydrate_value.Text
                ,past_action_value.Text,custom_mass_value.Text);
            if (IngredientManager.selectedIngredientPreview == null)
            {
                data.SaveAsNew();
                IngredientManager.AddIngredientChildToGrid(ingredient_list, data.Path,
                    IngredientManager.ingredient_list.Count, this);
                ReloadIngredientList();
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Felülírod az előző értékeket?", "Warning", MessageBoxButton.YesNo);
                if (result != MessageBoxResult.Yes) return;
                data.SaveOverride(IngredientManager.selectedIngredientPreview.ingredient_data.Path);
                IngredientManager.selectedIngredientPreview.SetIngredientData(data);
                ReloadIngredientList();
            }
            
            
        }

        public void ReloadIngredientList()
        {
            IngredientManager.LoadIngredients(this);
        }

        public void ReloadSortedIngredientList()
        {
            IngredientManager.LoadSortedIngredients(ingredient_name.Text);
        }

        private void DeleteIngredient()
        {
            MessageBoxResult result = MessageBox.Show("Biztos vagy benne?\nAz alapanyag örökre el fogn veszni! (Az sok idő)", "Warning", MessageBoxButton.YesNo);
            if (result != MessageBoxResult.Yes) return;
            IngredientManager.DeleteIngredient(ingredient_list);
            
            if(enable_search.IsChecked == false) ReloadIngredientList();
            if (enable_search.IsChecked == true) ReloadSortedIngredientList();
        }

        private void delete_ingredient_Click(object sender, RoutedEventArgs e)
        {
            DeleteIngredient();
        }

        private void ingredient_import_Click(object sender, RoutedEventArgs e)
        {
            IngredientManager.ImportFromClipboard();
            IngredientManager.ReloadIngredientFiles(ingredient_list, this);
            ReloadIngredientList();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(IngredientManager.selectedIngredientPreview == null || e.Key != Key.Delete) return;
            DeleteIngredient();
        }

        private void enable_search_Checked(object sender, RoutedEventArgs e)
        {
            ingredient_name.Text = "";
            energy_value.Text = "0";
            protein_value.Text = "0";
            fat_value.Text = "0";
            carbohydrate_value.Text = "0";
            past_action_value.Text = "0";
            portion_type.SelectedIndex = 0;
            scrollviewer.ScrollToTop();
            ReloadSortedIngredientList();
        }

        private void enable_search_Unchecked(object sender, RoutedEventArgs e)
        {   
            ReloadIngredientList();
        }

        private void ingredient_name_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(IsLoaded && enable_search.IsChecked == true)
            {
                ReloadSortedIngredientList();
            }
        }

        private void ingredient_name_KeyDown(object sender, KeyEventArgs e)
        {
            IngredientManager.prev_search = ingredient_name.Text;
            IngredientManager.selectedIngredientPreview.SetColorBG("#FFA7A7A7");
            if (IngredientManager.selectedIngredientPreview != null) foreach (Control control in IngredientManager.selectedIngredientPreview.main_grid.Children) control.SetColorFG(Colors.Black);
            IngredientManager.selectedIngredientPreview = null;
        }

        private void portion_type_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(IsLoaded)switch (portion_type.SelectedIndex)
            {
                case 0:
                    custom_mass_value.Visibility = Visibility.Hidden;
                    custom_mass_value.Text = "100g";
                    break; 
                case 1:
                    custom_mass_value.Visibility = Visibility.Hidden;
                    custom_mass_value.Text = "db";
                    break;
                case 2:
                    custom_mass_value.Visibility = Visibility.Visible;
                    custom_mass_value.Text = "100g";
                    break;
                default:
                    break;
            }

        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }
    }
}
