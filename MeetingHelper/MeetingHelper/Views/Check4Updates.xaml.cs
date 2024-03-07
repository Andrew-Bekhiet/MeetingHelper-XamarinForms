using MeetingHelper.Services;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MeetingHelper.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Check4Updates : ContentPage
    {
        static readonly Version currentversion = new Version(2, 2, 1250, 7);
        public Check4Updates()
        {
            InitializeComponent();
            CheckAgain.ImageSource = ImageSource.FromResource("MeetingHelper.Update.png", typeof(App));
            CurrentV.Text += currentversion.ToString();
            
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await CheckForUpdates();
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await CheckForUpdates();
        }

        private async Task CheckForUpdates()
        {
            if (Xamarin.Essentials.Connectivity.NetworkAccess == Xamarin.Essentials.NetworkAccess.Internet)
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("جار التحقق من وجود تحديثات جديدة ...", Acr.UserDialogs.MaskType.Black);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://pastebin.com/raw/2JpVMBDc");
                HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
                StreamReader sr = new StreamReader(response.GetResponseStream());
                Version newestversion = new Version(sr.ReadLine());
                string[] VersionDetails = sr.ReadToEnd().Split('\n');

                if (newestversion > currentversion)
                {
                    Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                    if (await Acr.UserDialogs.UserDialogs.Instance.ConfirmAsync("يوجد إصدار جديد. هل تريد تنزيله؟" + "\nالإصدار الحالي: " + currentversion.ToString() + "\nالإصدار الجديد: " + newestversion.ToString() + "\nما لجديد:\n" + VersionDetails[4].Replace(';', '\n'), null, "نعم", "لا"))
                    {
                        if (DependencyService.Get<IPlatform>().AndroidResult() == "Android")
                        {
                            //Android
                            request = (HttpWebRequest)WebRequest.Create(VersionDetails[1]);
                        }
                        else
                        {
                            //UWP
                            request = (HttpWebRequest)WebRequest.Create(VersionDetails[3]);
                        }
                        response = (HttpWebResponse)await request.GetResponseAsync();
                        await Xamarin.Essentials.Launcher.OpenAsync(response.ResponseUri);
                    }
                }
                else
                {
                    Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                    await DisplayAlert("لا يوجد تحديثات جديدة", "", "حسنًا");
                }
                sr.Dispose();
            }
            else
            {
                await DisplayAlert("لا يوجد اتصال بالإنترنت!", "", "حسنًا");
            }
        }
    }
}