using System;
using Xamarin.Forms;

namespace MeetingHelper.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            try
            {
                this.InitializeComponent();
                LoadApplication(new MeetingHelper.App());
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }

        private async void DisplayError(Exception ex)
        {
            if (await Acr.UserDialogs.UserDialogs.Instance.ConfirmAsync(ex + "\n" + ex.InnerException + "\n" + ex.StackTrace, "حدث خطأ داخلي بالبرنامج!", "إبلاغ عن الخطأ", "إغلاق"))
            {
                Device.OpenUri(new Uri("https://docs.google.com/forms/d/e/1FAIpQLScpSkF514EycFoSzlr9BmK35rq8MsaMfFqbZ6r4JRBa3-Gvtw/viewform"));
            }
        }
    }
}
