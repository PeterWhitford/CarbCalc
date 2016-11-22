using SQLite.Net.Attributes;

namespace CarbCalc
{
    public class FoodItem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [MaxLength(100)]
        public string Category { get; set; }
        [MaxLength(100)]
        public string SubCategory { get; set; }
        [MaxLength(100)]
        public string ItemName { get; set; }

        public int CarbCounterSize { get; set; }

        public int CarbCounterGrams { get; set; }
        public double ServingSize { get; set; }
        public double ServingCarbs { get; set; }
    }
}