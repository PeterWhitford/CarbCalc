using Android.App;
using Android.OS;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Environment = System.Environment;

namespace CarbCalc
{
    [Activity(Label = "Carb Calc", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
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

        private void InitialiseDatabase()
        {
            var docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

            var dbFile = Path.Combine(docFolder, SqlLiteDroid.FileName); // FILE NAME TO USE WHEN COPIED

            if (File.Exists(dbFile)) return;

            //if (File.Exists(dbFile))
            //{
            //    File.Delete(dbFile);
            //}

            var s = Resources.OpenRawResource(Resource.Raw.CarbCalc); // DATA FILE RESOURCE ID

            var writeStream = new FileStream(dbFile, FileMode.OpenOrCreate, FileAccess.Write);

            ReadWriteStream(s, writeStream);
        }

        private void ReadWriteStream(Stream readStream, Stream writeStream)
        {
            const int length = 256;

            var buffer = new byte[length];

            var bytesRead = readStream.Read(buffer, 0, length);

            while (bytesRead > 0)
            {
                writeStream.Write(buffer, 0, bytesRead);
                bytesRead = readStream.Read(buffer, 0, length);
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

                InitialiseDatabase();

                ReadItemsFromDatabase();

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
                CurrentSelected.Items);
        }

        private void SearchItems(string searchString)
        {
            switch (searchString.Length)
            {
                case 0:
                    CurrentSelected.Items = new List<FoodItem>(CurrentSelected.OriginalItems);
                    break;

                case 1:
                    CurrentSelected.Items =
                        CurrentSelected.OriginalItems.Where(x => x.ItemName.StartsWith(searchString, StringComparison.OrdinalIgnoreCase))
                            .ToList();
                    break;

                default:
                    CurrentSelected.Items =
                        CurrentSelected.OriginalItems.Where(x => x.ItemName.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0)
                            .ToList();
                    break;
            }
        }

        private void List_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            CurrentSelected.CurrentFoodItem = CurrentSelected.Items[e.Position];

            StartActivity(typeof(Calc));

        }

        private void ReadItemsFromDatabase()
        {
            var sql = SqlLiteDroid.GetSqLiteConnection();

            var cmd = sql.CreateCommand("Select * from FoodItem");

            CurrentSelected.Items = cmd.ExecuteQuery<FoodItem>().OrderBy(x => x.ItemName).ToList();
            CurrentSelected.OriginalItems = CurrentSelected.Items.ToList();

            List.Adapter = new FoodItemAdapter(this, CurrentSelected.Items);

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

