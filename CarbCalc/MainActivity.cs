using Android.App;
using Android.OS;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Environment = System.Environment;

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

            InitialiseDatabase();

            ReadItemsFromDatabase();
        }

        private void InitialiseDatabase()
        {
            var docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

            var dbFile = Path.Combine(docFolder, SqlLiteDroid.FileName); // FILE NAME TO USE WHEN COPIED

            //if (File.Exists(dbFile)) return;
            if (File.Exists(dbFile))
            {
                File.Delete(dbFile);
            }

            var s = Resources.OpenRawResource(Resource.Raw.CarbCalc); // DATA FILE RESOURCE ID

            var writeStream = new FileStream(dbFile, FileMode.OpenOrCreate, FileAccess.Write);

            ReadWriteStream(s, writeStream);
        }

        private void ReadWriteStream(Stream readStream, Stream writeStream)
        {
            int Length = 256;
            Byte[] buffer = new Byte[Length];
            int bytesRead = readStream.Read(buffer, 0, Length);
            // write the required bytes
            while (bytesRead > 0)
            {
                writeStream.Write(buffer, 0, bytesRead);
                bytesRead = readStream.Read(buffer, 0, Length);
            }
            readStream.Close();
            writeStream.Close();
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
                //ReadItemsFromDatabase();

                GetFoodItemsFromCsvFile();
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

            SearchItems(searchString);

            List.Adapter = new FoodItemAdapter(this,
                _items);
        }

        private void SearchItems(string searchString)
        {
            switch (searchString.Length)
            {
                case 0:
                    _items = new List<FoodItem>(_originalItems);
                    break;

                case 1:
                    _items =
                        _originalItems.Where(x => x.ItemName.StartsWith(searchString, StringComparison.OrdinalIgnoreCase))
                            .ToList();
                    break;

                default:
                    _items =
                        _originalItems.Where(x => x.ItemName.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0)
                            .ToList();
                    break;
            }
        }

        private void List_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            CurrentSelected.CurrentFoodItem = _items[e.Position];

            StartActivity(typeof(Calc));

        }


        private void GetFoodItemsFromCsvFile()
        {
            var input = Assets.Open(CurrentSelected.FileName);

            using (var sr = new StreamReader(input))
            {
                var str = sr.ReadToEnd();

                var lines = Regex.Split(str, Environment.NewLine);

                _originalItems = lines.Select(CreateFoodItem)
                    .OrderBy(x => x.ItemName).ToList();

            }

            _items = new List<FoodItem>(_originalItems);

            List.Adapter = new FoodItemAdapter(this, _items);

            List.ItemClick += List_ItemClick;
            Search.TextChanged += Search_TextChanged;

        }

        private static FoodItem CreateFoodItem(string foodString)
        {
            var field = foodString.Split(',');
            return new FoodItem
            {
                Category = field[0],
                //SubCategory = field[1],
                ItemName = field[1],
                CarbCounterSize = int.Parse(field[2]),
                CarbCounterGrams = int.Parse(field[3])
            };
        }

        private void ReadItemsFromDatabase()
        {
            //var dbPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

            //var message = Toast.MakeText(this, "Loading FoodItem Items from " + dbPath, ToastLength.Short);

            //message.Show();

            //SqlLiteDroid.ExtractDb(Assets, dbPath);

            var sql = SqlLiteDroid.GetSqLiteConnection();

            var cmd = sql.CreateCommand("Select * from FoodItem");

            _items = cmd.ExecuteQuery<FoodItem>().OrderBy(x => x.ItemName).ToList();

            List.Adapter = new FoodItemAdapter(this, _items);

            //message.Cancel();

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

