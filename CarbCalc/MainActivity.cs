using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Text.Method;
using Java.IO;

namespace CarbCalc
{
    [Activity(Label = "Carb Calc", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private List<FoodItem> _items;
        private Button NewButton => FindViewById<Button>(Resource.Id.newFood);
        private const string FileName = "Carbs.txt";
        private ListView List => FindViewById<ListView>(Resource.Id.foodlist);

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

            NewButton.Click += (sender, e) =>
            {
                CurrentSelected.CurrentFoodItem = new FoodItem
                {
                    Category = "Manual"
                };
                StartActivity(typeof(ManualCalc));
            };

            try
            {
                var input = Assets.Open(FileName);

                GetFoodItemsFromCsvFile(input);

                List.Adapter = new FoodItemAdapter(this, _items);

                List.ItemClick += List_ItemClick;

            }
            catch (Exception e)
            {
                var message = Toast.MakeText(this, "Could not load list of foods: " + e.Message, ToastLength.Long);
                message.Show();
            }
        }

        private void List_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            CurrentSelected.CurrentFoodItem = _items[e.Position];

            StartActivity(typeof(Calc));

        }

        private void GetFoodItemsFromCsvFile(Stream input)
        {
            var message = Toast.MakeText(this, "Loading FoodItem Items", ToastLength.Short);

            message.Show();

            using (var sr = new StreamReader(input))
            {
                var str = sr.ReadToEnd();

                var lines = Regex.Split(str, "\r\n");

                _items = lines.Select(x =>

                    {
                        var field = x.Split(',');
                        return new FoodItem
                        {
                            Category = field[0],
                            SubCategory = field[1],
                            ItemName = field[2],
                            CarbCounterSize = int.Parse(field[3]),
                            CarbCounterGrams = int.Parse(field[4])
                        };
                    }
                ).OrderBy(x => x.ItemName).ToList();

            }
            message.Cancel();
        }

        public override void OnBackPressed()
        {
            if (FragmentManager.BackStackEntryCount > 0)
                base.OnBackPressed();
        }

    }
}

