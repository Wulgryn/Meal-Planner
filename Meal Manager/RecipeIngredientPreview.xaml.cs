using Meal_Planner.Client_Manager;
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

namespace Meal_Planner.Meal_Manager
{
    /// <summary>
    /// Interaction logic for RecipeIngredientPreview.xaml
    /// </summary>
    public partial class RecipeIngredientPreview : UserControl
    {
        public RecipeIngredientPreview()
        {
            InitializeComponent();
        }
        public IngredientData IngredientData;
        public RecipePreview recipePreview;
        public RecipeIngredientPreview(ref IngredientData ingredientData, RecipePreview rp)
        {
            InitializeComponent();
            SetIngredientData(ingredientData);
            recipePreview = rp;
        }

        public void SetIngredientData(IngredientData ingredientData)
        {
            this.IngredientData = ingredientData;
            ingredient_name.Content = ingredientData.Name;
            energy_value.Content = ingredientData.Energy;
            protein_value.Content = ingredientData.Protein;
            fat_value.Content = ingredientData.Fat;
            carbohydrate_value.Content = ingredientData.Carbohydrate;
            portion_type.Content = ingredientData.Mass;
            past_action_value.Content = ingredientData.PastActionMass;
        }

        private void mass_value_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsLoaded) return;
            if (mass_value.Text == "") mass_value.Text = "0";
            mass_value.Text = mass_value.Text.Replace('.', ',');
            recipePreview.recipe_data.CalcValues(recipePreview);
            
        }

        private void main_grid_MouseEnter(object sender, MouseEventArgs e)
        {
            recipePreview.recipes.ingredient_name.Content = ingredient_name.Content;
        }

        private void main_grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (RecipeManager.selectedRIP != null)
            {
                RecipeManager.selectedRIP.main_grid.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF90E0C8"));
                RecipeManager.selectedRIP.mass_value.SetColorBG("#FF90E0C8");
                foreach (Control control in RecipeManager.selectedRIP.main_grid.Children) control.SetColorFG(Colors.Black);
            }
            if (RecipeManager.selectedRIP == this)
            {
                RecipeManager.selectedRIP.main_grid.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF90E0C8"));
                RecipeManager.selectedRIP.mass_value.SetColorBG("#FF90E0C8");
                foreach (Control control in RecipeManager.selectedRIP.main_grid.Children) control.SetColorFG(Colors.Black);
                RecipeManager.selectedRIP = null;
                recipePreview.recipes.delete_recipe.Content = "Törlés";
            }
            else
            {
                main_grid.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF274D42"));
                foreach (Control control in main_grid.Children) control.SetColorFG(Colors.White);
                mass_value.SetColorBG("#FF274D42");
                RecipeManager.selectedRIP = this;
                recipePreview.recipes.delete_recipe.Content = "Alapanyag törlése";
                if (RecipeManager.selectedRecipe != null)
                {
                    RecipeManager.selectedRecipe.values_grid.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF90C3C8"));
                    foreach (Control control in RecipeManager.selectedRecipe.values_grid.Children) control.SetColorFG(Colors.Black);
                    RecipeManager.selectedRecipe = null;
                }
            }
        }
    }
}
