using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Text;
using Android.Views;
using Android.Widget;
using MobiDigits;

namespace MobiCurrency
{
    [Activity(Label = "MobiCurrency", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity
    {
        EditText fromCurrency;
        TextView toCurrency;

        Spinner leftSpinner;
        Spinner rightSpinner;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Main);

            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);
            ActionBar.Title = "MobiCurrency";

            leftSpinner = FindViewById<Spinner>(Resource.Id.spinnerLeft);
            rightSpinner = FindViewById<Spinner>(Resource.Id.spinnerRight);

            leftSpinner.ItemSelected += SpinnerItemSelected;
            rightSpinner.ItemSelected += SpinnerItemSelected;

            var spinnerAdapter = ArrayAdapter.CreateFromResource
                (this, Resource.Array.currencies, Android.Resource.Layout.SimpleSpinnerItem);
            spinnerAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);

            leftSpinner.Adapter = spinnerAdapter;
            rightSpinner.Adapter = spinnerAdapter;

            fromCurrency = FindViewById<EditText>(Resource.Id.editTextCurrencyFrom);
            fromCurrency.TextChanged += ConvertCurrency;

            toCurrency = FindViewById<TextView>(Resource.Id.textViewCurrencyTo);

            RetrieveKey();

            CurrencyConverter.rate = CurrencyGetter.GetRate(leftSpinner.SelectedItem.ToString(),
                rightSpinner.SelectedItem.ToString());
        }

        void ConvertCurrency(object sender, TextChangedEventArgs e)
        {
            ConvertCurrency();
        }

        void ConvertCurrency()
        {
            CurrencyGetter.GetRate(leftSpinner.SelectedItem.ToString(), rightSpinner.SelectedItem.ToString());
            toCurrency.Text = (string.IsNullOrEmpty((fromCurrency.Text)) ? "" :
                CurrencyConverter.ConvertCurrency((fromCurrency.Text)));
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.top_menus, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.menu_settings:
                    var settingsWindow = new Intent(this, typeof(SettingsActivity));
                    StartActivityForResult(settingsWindow, 1);
                    return true;
                case Resource.Id.menu_about:
                    Toast.MakeText(this, ActionBar.TitleFormatted + " is a currency converter!", 
                                   ToastLength.Short).Show();
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode != Result.Ok) return;

            var txtViewKey = FindViewById<TextView>(Resource.Id.textViewKey);
            var key = data.GetStringExtra("fromSettingsActivity");
            txtViewKey.Text = key;
            CurrencyGetter.urlKey = key;
            StoreKey(key);
        }

        void SpinnerItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            CurrencyConverter.rate = CurrencyGetter.GetRate(leftSpinner.SelectedItem.ToString(),
                rightSpinner.SelectedItem.ToString());

            ConvertCurrency();
        }

        static void StoreKey(string key)
        {
            var prefs = Application.Context.GetSharedPreferences("MobiCurrency", FileCreationMode.Private);
            var prefEditor = prefs.Edit();
            prefEditor.PutString("Apilayer_key", key);
            prefEditor.Commit();
        }

        void RetrieveKey()
        {
            var prefs = Application.Context.GetSharedPreferences("MobiCurrency", FileCreationMode.Private);              
            var retrievedKey = prefs.GetString("Apilayer_key", "");
            var txtViewKey = FindViewById<TextView>(Resource.Id.textViewKey);
            txtViewKey.Text = retrievedKey;
            CurrencyGetter.urlKey = retrievedKey;
        }
    }
}