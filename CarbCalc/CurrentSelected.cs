using System.Collections.Generic;

namespace CarbCalc
{
    public static class CurrentSelected
    {

        public static FoodItem CurrentFoodItem;

        public static List<FoodItem> ItemisedMeal { get; set; } = new List<FoodItem>();
    }
}