using System;
using System.ComponentModel;

using Xamarin.Forms;

namespace MeetingHelper.Views
{
    public partial class ItemsPage : ContentPage
    {
        public ItemsPage()
        {
            InitializeComponent();
            Navigate("الرئيسية");
            Items.ItemSelected += Items_ItemSelected;
        }
        public string GetCurrentSelection()
        {
            if (Items.SelectedItem != null)
            {
                return Items.SelectedItem.ToString(); 
            }
            return "";
        }
        public void Navigate(string Obj)
        {
            if (Items.SelectedItem != null)
            {
                if (Items.SelectedItem.ToString() == Obj)
                {
                    Items.SelectedItem = null;
                }
            }
            Items.SelectedItem = Obj;
        }

        private void Items_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItemIndex == 0)
            {
                ((MasterDetailPage)App.Current.MainPage).Detail.Navigation.PopToRootAsync(true);
            }
            else if (e.SelectedItemIndex == 1)
            {
                ((MasterDetailPage)App.Current.MainPage).Detail.Navigation.PushAsync(new Persons(), true);
            }
            else if (e.SelectedItemIndex == 2)
            {
                ((MasterDetailPage)App.Current.MainPage).Detail.Navigation.PushAsync(new Day(), true);
            }
            else if (e.SelectedItemIndex == 3)
            {
                ((MasterDetailPage)App.Current.MainPage).Detail.Navigation.PushAsync(new History(), true);
            }
            else if (e.SelectedItemIndex == 4)
            {
                ((MasterDetailPage)App.Current.MainPage).Detail.Navigation.PushAsync(new SQLiteQ(), true);
            }
            else if (e.SelectedItemIndex == 5)
            {
                ((MasterDetailPage)App.Current.MainPage).Detail.Navigation.PushAsync(new Randomizer(), true);
            }
            else if (e.SelectedItemIndex == 6)
            {
                ((MasterDetailPage)App.Current.MainPage).Detail.Navigation.PushAsync(new Check4Updates(), true);
            }
            else if (e.SelectedItemIndex == 7)
            {
                ((MasterDetailPage)App.Current.MainPage).Detail.Navigation.PushAsync(new Settings(), true);
            }
            else if (e.SelectedItemIndex == 8)
            {
                ((MasterDetailPage)App.Current.MainPage).Detail.Navigation.PushAsync(new AboutPage(), true);
            }
            HideItems();
        }

        private void HideItems()
        {
            try
            {
                ((MasterDetailPage)App.Current.MainPage).IsPresented = false;
            }
            catch (Exception)
            {
            }
        }
    }
}