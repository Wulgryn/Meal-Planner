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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Meal_Planner.Meal_Manager
{
    /// <summary>
    /// Interaction logic for RecipePreview.xaml
    /// </summary>
    public partial class RecipePreview : UserControl
    {
        public RecipeData recipe_data;
        public RecipePreview()
        {
            InitializeComponent();
            ingredient_list.Visibility = Visibility.Collapsed;
        }
        public Recipes recipes;
        public RecipePreview(ref RecipeData recipeData,Recipes recipes)
        {
            InitializeComponent();
            ingredient_list.Visibility = Visibility.Collapsed;
            recipe_data = recipeData;
            
            
            this.recipes = recipes;
            add_ingredient.Visibility = Visibility.Collapsed;
            edit_description.Visibility = Visibility.Visible;
            recipe_name.Content = recipe_data.Name.Trim();

            NullValues();
            recipe_data.ReloadIngredientList(this);
        }

        public void NullValues()
        {
            energy_value.Content = "0";
            protein_value.Content = "0";
            fat_value.Content = "0";
            carbohydrate_value.Content = "0";
            mass_value.Content = "0";
        }
        public bool IsCollapsed = true;
        public void Expand()
        {
            int height = recipe_data.ingredients.Count * 130 + 158 + (recipe_data.ingredients.Count == 0 ? 10 : 0);
            Height = height;
            IsCollapsed = false;
            
        }

        public void Collapse()
        {
            Height = 168;
            IsCollapsed = true;
        }

        private void expand_collapse_Click(object sender, RoutedEventArgs e)
        {
            switch(expand_collapse.Content)
            {
                case "Kibontás":
                    ingredient_list.Visibility = Visibility.Visible;
                    add_ingredient.Visibility = Visibility.Visible;
                    //save_recipe.Visibility = Visibility.Visible;
                    expand_collapse.Content = "Bezárás";
                    Expand();
                    break;
                case "Bezárás":
                    ingredient_list.Visibility = Visibility.Collapsed;
                    add_ingredient.Visibility = Visibility.Collapsed;
                    //edit_description.Visibility = Visibility.Collapsed;
                    expand_collapse.Content = "Kibontás";
                    Collapse();
                    break;
            }
            recipes.RearrangeRecipes();
        }

        private void add_ingredient_Click(object sender, RoutedEventArgs e)
        {
            Ingredients ingredients = new Ingredients(this);
            ingredients.Show();
        }

        private void edit_description_Click(object sender, RoutedEventArgs e)
        {
            RecipeDescription description = new RecipeDescription(ref recipe_data);
            description.Show();
        }
    }
}
