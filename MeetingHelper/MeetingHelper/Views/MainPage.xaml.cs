using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Xamarin.Forms;

namespace MeetingHelper.Views
{
    public partial class MainPage : MasterDetailPage
    {
        public MainPage()
        {
            InitializeComponent();

            Title = "مساعد الاجتماع";
        }
        protected override bool OnBackButtonPressed()
        {
            if (Detail.Navigation.NavigationStack.Count == 1)
            {
                return false;
            }
            if (Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopupStack.Count != 0)
            {
                Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAllAsync(true);
            }
            else if (Detail.Navigation.NavigationStack[Detail.Navigation.NavigationStack.Count - 1].GetType() == typeof(Settings))
            {
                if (!(Detail.Navigation.NavigationStack[Detail.Navigation.NavigationStack.Count - 1] as Settings).DilgRslt) { (Detail.Navigation.NavigationStack[Detail.Navigation.NavigationStack.Count - 1] as Settings).DispAlrt(); }
                else { Detail.Navigation.PopAsync(true); }
            }
            else
            {
                Detail.Navigation.PopAsync(true);
            }
            return true;
        }
       
    }
}