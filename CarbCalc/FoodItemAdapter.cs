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
    public class FoodItemAdapter : BaseAdapter<FoodItem>, IFilterable
    {
        public readonly List<FoodItem> Items;
        public List<FoodItem> MatchItems;
        private readonly Activity _context;
        public Filter Filter { get; }

        public FoodItemAdapter(Activity context, List<FoodItem> items)
        {
            _context = context;
            Items = items;

            Filter = new FoodItemFilter(this);
        }
        public override long GetItemId(int position)
        {
            return position;
        }

        public override FoodItem this[int position] => Items[position];

        public override int Count => Items.Count;

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            // re-use an existing view, if one is available
            var view = convertView ?? _context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItem1, null); 
            view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = Items[position].ItemName;
            return view;
        }

    }
}