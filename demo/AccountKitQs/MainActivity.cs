using Android.App;
using Android.Widget;
using Android.OS;
using Com.Facebook.Accountkit;
using Android.Content;
using Com.Facebook.Accountkit.UI;
using Java.Lang;
using Android.Support.V7.App;
using Android.Gms.Auth.Api;

namespace AccountKitQs
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true, Icon = "@mipmap/icon")]
    public partial class MainActivity : AppCompatActivity
    {
        public static int APP_REQUEST_CODE = 99;

        TextView txtResult;
        Button btnLoginWithSms;
        Button btnLoginWithEmail;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Auth x;

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            txtResult = FindViewById<TextView>(Resource.Id.txtResult);
            btnLoginWithSms = FindViewById<Button>(Resource.Id.btnLoginWithPhoneNumber);
            btnLoginWithEmail = FindViewById<Button>(Resource.Id.btnLoginWithEmail);

            btnLoginWithSms.Click += delegate
            {
                txtResult.Visibility = Android.Views.ViewStates.Invisible;
                LoginWithPhoneNumber();
            };
            btnLoginWithEmail.Click += delegate
            {
                txtResult.Visibility = Android.Views.ViewStates.Invisible;
                LoginWithEmail();
            };
        }

        public void LoginWithPhoneNumber()
        {
            var intent = new Intent(this, typeof(AccountKitActivity));

            var configurationBuilder = new AccountKitConfiguration.AccountKitConfigurationBuilder(
                    LoginType.Phone,
                    AccountKitActivity.ResponseType.Token);
            intent.PutExtra(
                AccountKitActivityBase.AccountKitActivityConfiguration,
                configurationBuilder.Build());

            StartActivityForResult(intent, APP_REQUEST_CODE);
        }

        public void LoginWithEmail()
        {
            var intent = new Intent(this, typeof(AccountKitActivity));
            var configurationBuilder = new AccountKitConfiguration.AccountKitConfigurationBuilder(
                            LoginType.Email,
                            AccountKitActivity.ResponseType.Token);
            intent.PutExtra(
                AccountKitActivityBase.AccountKitActivityConfiguration,
                configurationBuilder.Build());

            StartActivityForResult(intent, APP_REQUEST_CODE);
        }

        public void Logout()
        {
            AccountKit.LogOut();
        }

        public void GetCurrentAccount()
        {
            AccountKit.GetCurrentAccount(this);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Android.Content.Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);


            if (requestCode != APP_REQUEST_CODE)
            {
                return;
            }

            var jobj = data.GetParcelableExtra(AccountKitLoginResult.ResultKey);
            IAccountKitLoginResult loginResult = Android.Runtime.Extensions.JavaCast<IAccountKitLoginResult>(jobj);
            var toastMessage = "Hello, AccountKit.";

            if (loginResult.Error != null)
            {
                toastMessage = loginResult.Error.ErrorType.Message;

                //ShowErrorActivity(loginResult.Error);
            }
            else if (loginResult.WasCancelled())
            {
                toastMessage = "Login Cancelled";
            }
            else
            {
                if (loginResult.AccessToken != null)
                {
                    toastMessage = "Success:" + loginResult.AccessToken.AccountId;
                }
                else
                {
                    toastMessage = string.Format(
                        "Success:{0}...",
                        loginResult.AuthorizationCode.Substring(0, 10));
                }

                GetCurrentAccount();
            }

            // Surface the result to your user in an appropriate way.
            Toast.MakeText(
                    this,
                    toastMessage,
                    ToastLength.Long)
                    .Show();
        }
    }

    partial class MainActivity : IAccountKitCallback
    {
        public void OnError(AccountKitError p0)
        {
            Toast.MakeText(
                this,
                p0.ToString(),
                ToastLength.Short
            ).Show();
        }

        public void OnSuccess(Object p0)
        {
            var account = Android.Runtime.Extensions.JavaCast<Account>(p0);

            RunOnUiThread(delegate
            {
                txtResult.Visibility = Android.Views.ViewStates.Visible;
                txtResult.Text = account.Email ?? account.PhoneNumber.GetPhoneNumber();
            });
        }
    }
}

