using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;

namespace CarbCalc
{
    [Activity(Label = "Manual Calculation - New Food", Icon = "@drawable/icon")]
    public class ManualCalc : Activity
    {
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
                CurrentSelected.ItemisedMeal.Clear();
                Grid.Adapter = new MealCalcAdapter(this, CurrentSelected.ItemisedMeal);
                CarbCalcText.Text = "";
                ClearButton.Visibility =ViewStates.Invisible;
            };

            if (!CurrentSelected.ItemisedMeal.Any()) return;

            ClearButton.Visibility = ViewStates.Visible;

            GramsPerSize.RequestFocus();

            CarbCalcText.Text =
                $"Total: {Math.Round(CurrentSelected.ItemisedMeal.Sum(x => x.ServingCarbs), 1)}g Carbs";
        }

        public void DismissKeyboard(Activity activity)
        {
            var imm = (InputMethodManager)activity.GetSystemService(InputMethodService);
            if (null != activity.CurrentFocus)
                imm.HideSoftInputFromWindow(activity.CurrentFocus
                    .WindowToken, 0);
        }

        private TextView CarbCalcText => FindViewById<TextView>(Resource.Id.textCarbTotal);
        private EditText NewFoodItem => FindViewById<EditText>(Resource.Id.food);
        private EditText Size => FindViewById<EditText>(Resource.Id.size);
        private EditText GramsPerSize => FindViewById<EditText>(Resource.Id.gramspersize);
        private EditText Grams => FindViewById<EditText>(Resource.Id.Grams);
        private EditText Portion => FindViewById<EditText>(Resource.Id.Portion);
        private Button CalcButton => FindViewById<Button>(Resource.Id.calculate);
        private Button ClearButton => FindViewById<Button>(Resource.Id.clear);
        private ListView Grid => FindViewById<ListView>(Resource.Id.listview);

      
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
            CurrentSelected.CurrentFoodItem.CarbCounterGrams = int.Parse(GramsPerSize.Text);

            var grams = (!string.IsNullOrEmpty(Grams.Text))
                ? (double) int.Parse(Grams.Text)
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
        }
    }
}