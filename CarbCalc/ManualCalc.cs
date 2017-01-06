using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Environment = System.Environment;

namespace CarbCalc
{
    [Activity(Label = "Manual Calculation - New Food", Icon = "@drawable/icon")]
    public class ManualCalc : Activity
    {
        private TextView CarbCalcText => FindViewById<TextView>(Resource.Id.textCarbTotal);
        private EditText NewFoodItem => FindViewById<EditText>(Resource.Id.food);
        private EditText Size => FindViewById<EditText>(Resource.Id.size);
        private EditText GramsPerSize => FindViewById<EditText>(Resource.Id.gramspersize);
        private EditText Grams => FindViewById<EditText>(Resource.Id.Grams);
        private EditText Portion => FindViewById<EditText>(Resource.Id.Portion);
        private Button CalcButton => FindViewById<Button>(Resource.Id.calculate);
        private Button ClearButton => FindViewById<Button>(Resource.Id.clear);
        private ListView Grid => FindViewById<ListView>(Resource.Id.listview);

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Title = CurrentSelected.CurrentFoodItem.ItemName;

            InitialiseFields();
        }

        private void InitialiseFields()
        {
            SetContentView(Resource.Layout.ManualCalc);

            Grid.Adapter= new MealCalcAdapter(this, CurrentSelected.ItemisedMeal);

            CalcButton.Click += (sender, e) =>
            {
                DismissKeyboard(this);
                CalculateCarbs();
            };

            ClearButton.Click += (sender, e) =>
            {
                ClearFields();
            };

            if (!CurrentSelected.ItemisedMeal.Any()) return;

            ClearButton.Visibility = ViewStates.Visible;

            GramsPerSize.RequestFocus();

            Portion.FocusChange += (sender, args) => { Grams.Text = ""; };
            Grams.FocusChange += (sender, args) => { Portion.Text = ""; };

            CarbCalcText.Text =
                $"Total: {Math.Round(CurrentSelected.ItemisedMeal.Sum(x => x.ServingCarbs), 1)}g Carbs";
        }

        private void ClearFields()
        {
            CurrentSelected.ItemisedMeal.Clear();
            Grid.Adapter = new MealCalcAdapter(this, CurrentSelected.ItemisedMeal);
            CarbCalcText.Text = "";
            ClearButton.Visibility = ViewStates.Invisible;
            NewFoodItem.Text = "";
            Size.Text = "";
            GramsPerSize.Text = "";
        }

        public void DismissKeyboard(Activity activity)
        {
            var imm = (InputMethodManager)activity.GetSystemService(InputMethodService);
            if (null != activity.CurrentFocus)
                imm.HideSoftInputFromWindow(activity.CurrentFocus
                    .WindowToken, 0);
        }

      
        private void CalculateCarbs()
        {
            if (string.IsNullOrEmpty(NewFoodItem.Text))
                NewFoodItem.Text = "New Food";

            if (string.IsNullOrEmpty(Size.Text))
                Size.Text = "100";

            if (string.IsNullOrEmpty(GramsPerSize.Text))
            {
                var message = Toast.MakeText(this, "You must enter a grams per unit size", ToastLength.Long);
                message.Show();
                return;
            }

            CurrentSelected.CurrentFoodItem.ItemName = NewFoodItem.Text;
            CurrentSelected.CurrentFoodItem.CarbCounterSize = int.Parse(Size.Text);
            CurrentSelected.CurrentFoodItem.CarbCounterGrams =Convert.ToDouble(GramsPerSize.Text);

            var grams = (!string.IsNullOrEmpty(Grams.Text))
                ? Convert.ToDouble(Grams.Text)
                : 0;

            var portion = grams > 0 ? grams / CurrentSelected.CurrentFoodItem.CarbCounterSize
                : (!string.IsNullOrEmpty(Portion.Text)) ? Convert.ToDouble(Portion.Text) : 0;

            var carbs = (portion > 0 ? portion : 1) * CurrentSelected.CurrentFoodItem.CarbCounterGrams ;

            CurrentSelected.CurrentFoodItem.ServingSize = grams > 0 ? grams : portion * CurrentSelected.CurrentFoodItem.CarbCounterSize;
            CurrentSelected.CurrentFoodItem.ServingCarbs = carbs;

            if (!CurrentSelected.ItemisedMeal.Contains(CurrentSelected.CurrentFoodItem))
                CurrentSelected.ItemisedMeal.Add(CurrentSelected.CurrentFoodItem);

            Grid.Adapter = new MealCalcAdapter(this, CurrentSelected.ItemisedMeal);

            CarbCalcText.Text = $"{Math.Round(CurrentSelected.ItemisedMeal.Sum(x=> x.ServingCarbs), 1)}g Carbs";
            ClearButton.Visibility = ViewStates.Visible;

            //SaveNewFoodItem();
        }

        private void SaveFoodItemToCsvFile()
        {
            //var input = Assets.Open(CurrentSelected.FileName);

            //using (var sr = new StreamWriter(input))
            //{
            //    var str = sr.ReadToEnd();

            //    var lines = Regex.Split(str, Environment.NewLine);

            //    _originalItems = lines.Select(CreateFoodItem)
            //        .OrderBy(x => x.ItemName).ToList();

            //}

            //_items = new List<FoodItem>(_originalItems);

            //List.Adapter = new FoodItemAdapter(this, _items);

            //List.ItemClick += List_ItemClick;
            //Search.TextChanged += Search_TextChanged;

        }

        private void SaveNewFoodItem()
        {
            var input = Assets.Open(CurrentSelected.FileName);

            CurrentSelected.SaveCurrent(input);
        }
    }
}