using Android.App;
using Android.Widget;
using Android.OS;
using Com.Facebook.Accountkit;

namespace AccountKitQs
{
    [Activity(Label = "AccountKitQs", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity
    {
        int count = 1;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.myButton);

            button.Click += delegate { button.Text = $"{count++} clicks!"; };
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Android.Content.Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            //var jobj = data.GetParcelableExtra(AccountKitLoginResult.ResultKey);
            //IAccountKitLoginResult result = Android.Runtime.Extensions.JavaCast<IAccountKitLoginResult>(jobj);
        }
    }
}

