using System;
using System.ComponentModel;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MeetingHelper.Views
{
    [DesignTimeVisible(true)]
    public partial class AboutPage : ContentPage
    {
        public AboutPage()
        {
            InitializeComponent();
            Logo.Source = ImageSource.FromResource("MeetingHelper.Logo.ico", typeof(App));
        }

        private async void Error_Tapped(object sender, EventArgs e)
        {
            await Xamarin.Essentials.Launcher.OpenAsync(new Uri((sender as Label).Text));
        }

        private async void Feedback_Tapped(object sender, EventArgs e)
        {
            await Xamarin.Essentials.Launcher.OpenAsync(new Uri("https://docs.google.com/forms/d/e/1FAIpQLSe0RXjSbOe7Xym3sqL4oGwjlbncl5AxkMRu6a3g7NTJniaL3w/viewform"));
        }
    }
}