using MeetingHelper.Services;
using MeetingHelper.Tables;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MeetingHelper.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class History : ContentPage
    {
        public static readonly BindableProperty TagProperty = BindableProperty.Create("Tag", typeof(string), typeof(History), "");
        public History()
        {
            InitializeComponent();
            if ((((MasterDetailPage)Application.Current.MainPage).Master as ItemsPage).GetCurrentSelection() != "السجل")
            {
                (((MasterDetailPage)Application.Current.MainPage).Master as ItemsPage).Navigate("السجل");
            }
            if (DependencyService.Get<IPlatform>().UWPResult() == "UWP")
            {
                FlowDirection = FlowDirection.LeftToRight;
            }
            HistoryL.BeginRefresh();
        }

        private void HistoryL_Refreshing(object sender, System.EventArgs e)
        {
            try
            {
                HistoryL.ItemsSource = (from rows in App.Data.Table<Days>().ToList() select rows).ToList();
            }
            catch (System.NullReferenceException)
            {}
            HistoryL.EndRefresh();
        }

        private void HistoryL_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (HistoryL.SelectedItem != null)
            {
                Navigation.PushAsync(new Day(e.SelectedItem as Days));
                HistoryL.SelectedItem = null;
            }
        }
        public static string GetTag(BindableObject bindable)
        {
            return (string)bindable.GetValue(TagProperty);
        }
        public static void SetTag(BindableObject bindable, string value)
        {
            bindable.SetValue(TagProperty, value);
        }
    }
}