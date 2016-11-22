using Android.App;
using Android.OS;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace CarbCalc
{
    [Activity(Label = "Carb Calc", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private List<FoodItem> _originalItems;
        private List<FoodItem> _items;
        private Button NewButton => FindViewById<Button>(Resource.Id.newFood);
        private Button ClearButton => FindViewById<Button>(Resource.Id.clear);
        private ListView List => FindViewById<ListView>(Resource.Id.foodlist);
        private EditText Search => FindViewById<EditText>(Resource.Id.searchBox);

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

            Initialise();
        }

        private void Initialise()
        {
            NewButton.Click += (sender, e) =>
            {
                CurrentSelected.CurrentFoodItem = new FoodItem
                {
                    Category = "Manual"
                };
                StartActivity(typeof(ManualCalc));
            };

            ClearButton.Click += ClearButton_Click;

            try
            {
                GetFoodItemsFromCsvFile();

                //var autoCompleteAdapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleDropDownItem1Line, 
                //    _items.Select(x=>x.ItemName).ToList());

                //Search.Adapter = autoCompleteAdapter;

            }
            catch (Exception e)
            {
                var message = Toast.MakeText(this, "Could not load list of foods: " + e.Message, ToastLength.Long);
                message.Show();
            }
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            Search.Text = string.Empty;
        }

        private void Search_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            var searchString = e.Text.ToString();


            //((FoodItemAdapter)List.Adapter).Filter.InvokeFilter(searchString);

            switch (searchString.Length)
            {
                case 0:
                    _items = new List<FoodItem>(_originalItems);
                    break;

                case 1:
                    _items = _originalItems.Where(x => x.ItemName.StartsWith(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;

                default:
                    _items = _originalItems.Where(x => x.ItemName.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                    break;
            }

            List.Adapter = new FoodItemAdapter(this,
                _items);
        }

        private void List_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            CurrentSelected.CurrentFoodItem = _items[e.Position];

            StartActivity(typeof(Calc));

        }

        private void GetFoodItemsFromCsvFile()
        {
            var input = Assets.Open(CurrentSelected.FileName);

            var message = Toast.MakeText(this, "Loading FoodItem Items", ToastLength.Short);

            message.Show();

            using (var sr = new StreamReader(input))
            {
                var str = sr.ReadToEnd();

                var lines = Regex.Split(str, "\r\n");

                _originalItems = lines.Select(x =>

                    {
                        var field = x.Split(',');
                        return new FoodItem
                        {
                            Category = field[0],
                            //SubCategory = field[1],
                            ItemName = field[1],
                            CarbCounterSize = int.Parse(field[2]),
                            CarbCounterGrams = int.Parse(field[3])
                        };
                    }
                ).OrderBy(x => x.ItemName).ToList();

            }
            message.Cancel();

            _items = new List<FoodItem>(_originalItems);

            List.Adapter = new FoodItemAdapter(this, _items);

            List.ItemClick += List_ItemClick;
            Search.TextChanged += Search_TextChanged;

        }

        public override void OnBackPressed()
        {
            if (FragmentManager.BackStackEntryCount > 0)
                base.OnBackPressed();
        }

    }
}

