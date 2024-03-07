using MeetingHelper.Services;
using MeetingHelper.Tables;
using MeetingHelper.Views;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MeetingHelper
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Persons : ContentPage
    {

        private List<PersonTable> Details;
        public Persons()
        {
            try
            {
                InitializeComponent();
                if ((((MasterDetailPage)Application.Current.MainPage).Master as ItemsPage).GetCurrentSelection() != "المخدومين")
                {
                    (((MasterDetailPage)Application.Current.MainPage).Master as ItemsPage).Navigate("المخدومين");
                }
                if (DependencyService.Get<IPlatform>().UWPResult() == "UWP")
                {
                    FlowDirection = FlowDirection.LeftToRight;
                }
                this.Title = "المخدومين";
                Plugin.Permissions.CrossPermissions.Current.RequestPermissionsAsync(Plugin.Permissions.Abstractions.Permission.Storage);

                Add.IconImageSource = ImageSource.FromResource("MeetingHelper.AddP" + DependencyService.Get<IPlatform>().UWPResult() + ".png", typeof(App).GetTypeInfo().Assembly);
                Add.Clicked += delegate { Navigation.PushAsync(new Person(), true); };

                AddFContacts.IconImageSource = ImageSource.FromResource("MeetingHelper.AddFC" + DependencyService.Get<IPlatform>().UWPResult() + ".png", typeof(App).GetTypeInfo().Assembly);
                AddFContacts.Clicked += delegate { Navigation.PushAsync(new Contacts(), true); };
                
                SizeChanged += Persons_SizeChanged;
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", ex.Message + ex.StackTrace, "Ok");
            }
        }

        private void PsSearch_Search(object sender, EventArgs e)
        {
            List<PersonTable> SearchCriteria = Details.FindAll(x => x.ToString().ToUpperInvariant().Contains(PsSearch.Text.ToUpperInvariant()));
            PsList.ItemsSource = SearchCriteria;
        }

        private void PsSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(PsSearch.Text))
            {
                PsList.BeginRefresh();
            }
        }

        private void PsList_Refreshing(object sender, EventArgs e)
        {
            try
            {
                Details = App.Data.Table<PersonTable>().ToList().OrderBy(x => x.GetName).ToList();
            }
            catch(NullReferenceException)
            {
                Details = new List<PersonTable>();
            }
            PsList.ItemsSource = Details;
            PsList.EndRefresh();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                Details = App.Data.Table<PersonTable>().ToList().OrderBy(x => x.GetName).ToList();
            }
            catch (NullReferenceException)
            {
                Details = new List<PersonTable>();
            }
            PsList.ItemsSource = Details;
            PsList.BeginRefresh();
        }

        private void PsList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null) { return; }
            PersonTable SelectedP = Details.Find(Person => Person.Id == (e.SelectedItem as PersonTable).Id);
            PsList.SelectedItem = null;
            App.ShowInfoAbout(SelectedP, sender, Width, Height, Navigation);
        }
        private void Persons_SizeChanged(object sender, EventArgs e)
        {
            if (Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopupStack.Count != 0)
            {
                Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopupStack[0].Padding = new Thickness(20 * Width / 360, 40 * Height / 560, 20 * Width / 360, 70 * Height / 560);
            }
        }
    }

    public class ListViewItemModel
    {
        public string Name { get; set; }
        public string TeNum { get; set; }
        public ImageSource Image { get; set; }
        public int Id { get; set; }
    }
}