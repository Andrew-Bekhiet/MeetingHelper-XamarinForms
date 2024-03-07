using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using MeetingHelper.Models;
using Org.Opencv.Core;
//using Org.Opencv.Core;
using Plugin.Permissions;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MeetingHelper.Droid
{
    [Activity(Theme = "@style/MainTheme", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        internal static MainActivity Instance { get; private set; }
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                TabLayoutResource = Resource.Layout.Tabbar;
                ToolbarResource = Resource.Layout.Toolbar;

                Acr.UserDialogs.UserDialogs.Init(this);
                Forms.Init(this, savedInstanceState);

                base.OnCreate(savedInstanceState);

                Rg.Plugins.Popup.Popup.Init(this, savedInstanceState);
                Plugin.CurrentActivity.CrossCurrentActivity.Current.Init(this, savedInstanceState);
                
                Instance = this;

                Xamarin.Essentials.Platform.Init(this, savedInstanceState);

                LoadApplication(new App());
            }
            catch (System.Exception ex)
            {
                if (await Acr.UserDialogs.UserDialogs.Instance.ConfirmAsync(ex + "\n" + ex.InnerException + "\n" + ex.StackTrace, "حدث خطأ داخلي بالبرنامج!", "إبلاغ عن الخطأ", "إغلاق"))
                {
                    await Xamarin.Essentials.Launcher.OpenAsync(new System.Uri("https://docs.google.com/forms/d/e/1FAIpQLScpSkF514EycFoSzlr9BmK35rq8MsaMfFqbZ6r4JRBa3-Gvtw/viewform"));
                }
            }
        }

        public TaskCompletionSource<StreamPath> PickImageTaskCompletionSource { set; get; }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }

    [Activity(Label = "مساعد الإجتماع", Icon = "@mipmap/icon", Theme = "@style/Splash", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, NoHistory = true)]
    public class SplashActivity : Activity
    {
        Bundle bundle;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SplashScreen);
            bundle = savedInstanceState;
        }
        protected override async void OnResume()
        {
            base.OnResume();
            try
            {
                FindViewById<TextView>(Resource.Id.LoadingText).Text = "جار تحميل مكتبة التعرف على الوجوه ...";
                Java.Lang.JavaSystem.LoadLibrary("opencv_java3");
                System.Threading.Thread.Sleep(100);
                FindViewById<TextView>(Resource.Id.LoadingText).Text = "جار تحميل التطبيق ...";
                System.EventHandler<string> text = new System.EventHandler<string>(App_NextLoadText);
                await Task.Run(() =>
                {
                    Forms.Init(this.ApplicationContext, bundle);
                    App app = new App(text);
                    StartActivity(new Intent(Application.ApplicationContext, typeof(MainActivity)));
                });
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                Debugger.Break();
            }
        }

        private void App_NextLoadText(object sender, string e)
        {
            RunOnUiThread(() => FindViewById<TextView>(Resource.Id.LoadingText).Text = e);
        }
    }
}