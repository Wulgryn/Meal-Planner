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
using System.Windows.Shapes;

namespace Meal_Planner.Meal_Manager
{
    /// <summary>
    /// Interaction logic for Recipes.xaml
    /// </summary>
    public partial class Recipes : Window
    {
        public Recipes()
        {
            InitializeComponent();
            RecipeManager.ReloadRecipeFolder(recipe_list, this);
            RecipeManager.LoadRecipes(this);
        }
        bool IsOwnerShown = false;
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (IsOwnerShown) return;
            Owner.Close();
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            RecipeManager.SaveRecipe(this);
            RearrangeRecipes();
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            Owner.Show();
            IsOwnerShown = true;
            Close();
        }

        public void RearrangeRecipes()
        {
            int offset = 0;
            for(int i = 0; i < RecipeManager.recipe_list.Count; i++)
            {
                RecipePreview rp_ = RecipeManager.recipe_list[i];
                rp_.Margin = new Thickness(0, 168 * i + offset, 0, 0);
                if(!rp_.IsCollapsed)
                {
                    offset += rp_.recipe_data.ingredients.Count * 130 + 168 + (rp_.recipe_data.ingredients.Count == 0 ? 10 : 0);
                }
            }
        }

        private void delete_recipe_Click(object sender, RoutedEventArgs e)
        {
            if(delete_recipe.Content == "Alapanyag törlése")
            {
                MessageBoxResult result_ = MessageBox.Show("Biztos vagy benne? Ezzel elmented a jelenlegi értékeket is.", "Warning", MessageBoxButton.YesNo);
                if (result_ != MessageBoxResult.Yes) return;
                RecipeIngredientPreview rip = RecipeManager.selectedRIP;
                RecipePreview rp = rip.recipePreview;
                rp.recipe_data.ingredients.Remove(rip);
                rp.recipe_data.SaveOverride();
                rp.recipe_data.ReloadIngredientList(rp);
                rp.Expand();
                RearrangeRecipes();
                return;
            }
            MessageBoxResult result = MessageBox.Show("Biztos vagy benne?\nA recept örökre el fogn veszni! (Az sok idő)", "Warning", MessageBoxButton.YesNo);
            if (result != MessageBoxResult.Yes) return;
            recipe_list.Children.Remove(RecipeManager.selectedRecipe);
            RecipeManager.DeleteRecipe();
            RecipeManager.LoadRecipes(this);
            RearrangeRecipes();
        }
    }
}
