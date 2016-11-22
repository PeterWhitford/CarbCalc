using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace CarbCalc
{
    public class MealCalcAdapter : BaseAdapter<FoodItem>
    {
        List<FoodItem> items;
        Activity context;
        public MealCalcAdapter(Activity context, List<FoodItem> items) : base()
        {
            this.context = context;

            FillItems(items);
        }

        private void FillItems(List<FoodItem> items)
        {
            this.items = items ?? new List<FoodItem>();


        }

        public override long GetItemId(int position)
        {
            return position;
        }
        public override FoodItem this[int position]
        {
            get { return items[position]; }
        }
        public override int Count
        {
            get { return items.Count; }
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView ?? context.LayoutInflater.Inflate(
                Resource.Layout.MealItem, parent, false);
            var foodItemName = view.FindViewById<TextView>(Resource.Id.FoodItemName);
            var foodServingSize = view.FindViewById<TextView>(Resource.Id.Grams);
            var foodCarbs = view.FindViewById<TextView>(Resource.Id.Carbs);

            foodItemName.Text = items[position].ItemName;
            foodServingSize.Text = $"{Math.Round(items[position].ServingSize, 1)}g";
            foodCarbs.Text = $"{Math.Round(items[position].ServingCarbs, 1)}g";

            return view;
        }
    }
}