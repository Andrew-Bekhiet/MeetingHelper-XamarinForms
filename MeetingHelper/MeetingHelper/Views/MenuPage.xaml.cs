using MeetingHelper.Services;
using System;
using System.Diagnostics;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MeetingHelper.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuPage : ContentPage
    {
        public MenuPage()
        {
            InitializeComponent();
            if (DependencyService.Get<IPlatform>().UWPResult() == "UWP")
            {
                FlowDirection = FlowDirection.LeftToRight;
            }
            EP.ImageSource = ImageSource.FromResource("MeetingHelper.PersonsI.png", typeof(App));
            AddToday.ImageSource = ImageSource.FromResource("MeetingHelper.AddTodayI.png", typeof(App));
            History.ImageSource = ImageSource.FromResource("MeetingHelper.HistoryI.png", typeof(App));
            Search.ImageSource = ImageSource.FromResource("MeetingHelper.SearchI.png", typeof(App));
            Settings.ImageSource = ImageSource.FromResource("MeetingHelper.SettingsI.png", typeof(App));
            Update.ImageSource = ImageSource.FromResource("MeetingHelper.Update.png", typeof(App));
            About.ImageSource = ImageSource.FromResource("MeetingHelper.AboutI.png", typeof(App));

        }
        private void Button_Click(object sender, EventArgs e)
        {
            try
            {
                (((MasterDetailPage)Application.Current.MainPage).Master as ItemsPage).Navigate((sender as Button).Text);
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", ex.Message + ex.StackTrace, "Ok");
                Debugger.Break();
            }
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                if (((App.Current.MainPage as MasterDetailPage).Master as ItemsPage).GetCurrentSelection() != "الرئيسية")
                {
                    ((App.Current.MainPage as MasterDetailPage).Master as ItemsPage).Navigate("الرئيسية");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}