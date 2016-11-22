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
using Java.Lang;
using Object = Java.Lang.Object;

namespace CarbCalc
{
    public class FoodItemFilter : Filter
    {
        readonly FoodItemAdapter _foodItemAdapter;

        public FoodItemFilter(FoodItemAdapter adapter)
        {
            _foodItemAdapter = adapter;
        }

        protected override FilterResults PerformFiltering(ICharSequence constraint)
        {
            var results = new FilterResults();

            if (constraint == null) return results;

            var searchFor = constraint.ToString();

            var matches = _foodItemAdapter.Items.Where(x =>x.ItemName.IndexOf(searchFor, StringComparison.OrdinalIgnoreCase) >= 0).ToList();

            _foodItemAdapter.MatchItems = matches;

            var matchObjects = new Object[matches.Count];

            for (var i = 0; i < matches.Count; i++)
            {
                matchObjects[i] = new Java.Lang.String(matches[i].ItemName);
            }

            results.Values = matchObjects;
            results.Count = matches.Count;
            return results;
        }

        protected override void PublishResults(ICharSequence constraint, FilterResults results)
        {
            _foodItemAdapter.NotifyDataSetChanged();
        }
    }
}