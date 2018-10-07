using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using MobiDigits;

namespace MobiCurrency
{
    [Activity(Label = "SettingsActivity")]
    public class SettingsActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Settings);

            var buttonOpenSettings = FindViewById<Button>(Resource.Id.buttonConfirmKey);

            buttonOpenSettings.Click += delegate
            {
                var editTextPostKey = FindViewById<EditText>(Resource.Id.editTextKey);
                var settingsIntent = new Intent(this, typeof(MainActivity));
                settingsIntent.PutExtra("fromSettingsActivity", editTextPostKey.Text);
                SetResult(Result.Ok, settingsIntent);
                Finish();
            };
        }
    }
}