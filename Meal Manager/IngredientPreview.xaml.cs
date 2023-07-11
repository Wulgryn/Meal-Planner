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
    /// Interaction logic for IngredientPreview.xaml
    /// </summary>
    public partial class IngredientPreview : UserControl
    {
        public IngredientPreview()
        {
            InitializeComponent();
        }
        public IngredientData ingredient_data;
        public IngredientPreview(ref IngredientData ingredientData)
        {
            InitializeComponent();
            SetIngredientData(ingredientData);
        }

        public void SetIngredientData(IngredientData ingredientData)
        {
            ingredient_data = ingredientData;
            ingredient_name.Content = ingredient_data.Name;
            energy_value.Content = ingredient_data.Energy;
            protein_value.Content = ingredient_data.Protein;
            fat_value.Content = ingredient_data.Fat;
            carbohydrate_value.Content = ingredient_data.Carbohydrate;
            past_action_value.Content = ingredient_data.PastActionMass;
            portion_type.Content = ingredient_data.Mass;
        }
    }
}
