using MeetingHelper.Models;
using MeetingHelper.Services;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MeetingHelper.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Contacts : ContentPage
    {
        private List<Contact> contacts;
        public Contacts()
        {
            InitializeComponent();
            if (DependencyService.Get<IPlatform>().UWPResult() == "UWP")
            {
                FlowDirection = FlowDirection.LeftToRight;
            }
            CList.BeginRefresh();
        }

        private void CSearch_TextChanged(object sender, EventArgs e)
        {
            if (CSearch.Text == "")
            {
                CList.BeginRefresh();
            }
        }

        private void CList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            //Navigation.PopAsync();
            Navigation.PushAsync(new Person(((Contact)e.SelectedItem).Name, ((Contact)e.SelectedItem).PhoneN, ((Contact)e.SelectedItem).Image, ((Contact)e.SelectedItem).Address));
        }

        private async void CList_Refreshing(object sender, EventArgs e)
        {
            await Plugin.Permissions.CrossPermissions.Current.RequestPermissionsAsync(Plugin.Permissions.Abstractions.Permission.Contacts);
            contacts = await DependencyService.Get<IUserContactsService>().GetAllContacts();
            CList.ItemsSource = contacts;
            CList.EndRefresh();
        }

        private void CSearch_SearchButtonPressed(object sender, EventArgs e)
        {
            List<Contact> SearchCriteria = contacts.FindAll(x => x.ToString().ToLower().Contains(CSearch.Text.ToLower()));
            CList.ItemsSource = SearchCriteria;
        }
    }
}