using System.Collections.Generic;
using System.IO;
using ServiceStack.Text;

namespace CarbCalc
{
    public static class CurrentSelected
    {
        public const string FileName = "Carbs.csv";

        public static FoodItem CurrentFoodItem;

        public static List<FoodItem> ItemisedMeal { get; set; } = new List<FoodItem>();

        public static void SaveCurrent(Stream input)
        {
            using (var sw = new StreamWriter(input))
            {
                var csv = CsvSerializer.SerializeToCsv(new List<FoodItem>(new [] { CurrentFoodItem}));
                sw.WriteLine(csv);

                sw.Close();
            }
        }
    }
}