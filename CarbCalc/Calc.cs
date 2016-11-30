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
    [Activity(Label = "Calculate Carbs", Icon = "@drawable/icon")]
    public class Calc : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Title = CurrentSelected.CurrentFoodItem.ItemName;

            InitialiseFields();
        }

        private void InitialiseFields()
        {
            SetContentView(Resource.Layout.Calc);

            SetDescription();

            //Grams.KeyPress += HandleEntryTextUpdate;
            //Portion.KeyPress += HandleEntryTextUpdate;

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

            Portion.FocusChange += (sender, args) => { Grams.Text = ""; };
            Grams.FocusChange += (sender, args) => { Portion.Text = ""; };

            if (CurrentSelected.ItemisedMeal.Any())
            {
                ClearButton.Visibility = ViewStates.Visible;
                CarbCalcText.Text =
                    $"Total: {Math.Round(CurrentSelected.ItemisedMeal.Sum(x => x.ServingCarbs), 1)}g Carbs";
            }
        }

        public void DismissKeyboard(Activity activity)
        {
            var imm = (InputMethodManager)activity.GetSystemService(InputMethodService);
            if (null != activity.CurrentFocus)
                imm.HideSoftInputFromWindow(activity.CurrentFocus
                    .WindowToken, 0);
        }

        private void HandleEntryTextUpdate(object sender, View.KeyEventArgs e)
        {
            e.Handled = false;

            if (e.Event.Action != KeyEventActions.Down && e.KeyCode != Keycode.Enter && e.KeyCode != Keycode.Tab)
                return;

            CalculateCarbs();

            e.Handled = true;
        }

        private TextView CarbCalcText => FindViewById<TextView>(Resource.Id.textCarbTotal);
        private EditText Grams => FindViewById<EditText>(Resource.Id.Grams);
        private EditText Portion => FindViewById<EditText>(Resource.Id.Portion);
        private TextView FoodItemDesc => FindViewById<TextView>(Resource.Id.textFoodItemDesc);
        private Button CalcButton => FindViewById<Button>(Resource.Id.calculate);
        private Button ClearButton => FindViewById<Button>(Resource.Id.clear);
        private ListView Grid => FindViewById<ListView>(Resource.Id.listview);

        private void SetDescription()
        {
            FoodItemDesc.Text =
                $"{CurrentSelected.CurrentFoodItem.CarbCounterSize}g equates to {CurrentSelected.CurrentFoodItem.CarbCounterGrams}g Carbs";
        }
        
        private void CalculateCarbs()
        {
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
        }
    }
}