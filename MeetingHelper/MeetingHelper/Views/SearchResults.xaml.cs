using MeetingHelper.Models;
using MeetingHelper.Services;
using MeetingHelper.Tables;
using MeetingHelper.ViewModels;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Behavior = MeetingHelper.Models.Behavior;

namespace MeetingHelper.Views
{
    public class DegreeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Math.Abs(int.Parse(value.ToString()));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchResults : ContentPage
    {
        private readonly Type type;
        public SearchResults(List<PersonTable> result)
        {
            InitializeComponent();
            ShareB.IconImageSource = ImageSource.FromResource("MeetingHelper.Share.png", typeof(App));
            Title = "النتائج";
            if (DependencyService.Get<IPlatform>().UWPResult() == "UWP")
            {
                FlowDirection = FlowDirection.LeftToRight;
            }
            Results.ItemTemplate = new DataTemplate(typeof(PersonCell));
            Results.ItemsSource = result;
            type = typeof(PersonTable);
        }

        public SearchResults(List<Absence> result)
        {
            InitializeComponent();
            ShareB.IconImageSource = ImageSource.FromResource("MeetingHelper.Share.png", typeof(App));
            Title = "النتائج";
            if (DependencyService.Get<IPlatform>().UWPResult() == "UWP")
            {
                FlowDirection = FlowDirection.LeftToRight;
            }
            Results.ItemTemplate = new DataTemplate(typeof(AbsenceCell));
            Results.ItemsSource = result;
            Results.RowHeight = 150;
            type = typeof(Absence);
        }

        public SearchResults(List<_Activity> result)
        {
            InitializeComponent();
            ShareB.IconImageSource = ImageSource.FromResource("MeetingHelper.Share.png", typeof(App));
            Title = "النتائج";
            if (DependencyService.Get<IPlatform>().UWPResult() == "UWP")
            {
                FlowDirection = FlowDirection.LeftToRight;
            }
            Results.ItemTemplate = new DataTemplate(typeof(ActivityCell));
            Results.ItemsSource = result;
            Results.RowHeight = 150;
            type = typeof(_Activity);
        }

        public SearchResults(List<Behavior> result)
        {
            InitializeComponent();
            ShareB.IconImageSource = ImageSource.FromResource("MeetingHelper.Share.png", typeof(App));
            Title = "النتائج";
            if (DependencyService.Get<IPlatform>().UWPResult() == "UWP")
            {
                FlowDirection = FlowDirection.LeftToRight;
            }
            Results.ItemTemplate = new DataTemplate(typeof(BehaviorCell));
            Results.ItemsSource = result;
            Results.RowHeight = 300;
            type = typeof(Models.Behavior);
        }

        public SearchResults(List<DB_Info> result)
        {
            InitializeComponent();
            ShareB.IsEnabled = false;
            ShareB.IconImageSource = ImageSource.FromResource("MeetingHelper.Share.png", typeof(App));
            Title = "النتائج";
            if (DependencyService.Get<IPlatform>().UWPResult() == "UWP")
            {
                FlowDirection = FlowDirection.LeftToRight;
            }
            Results.ItemTemplate = new DataTemplate(typeof(SQLite_MasterCell));
            Results.ItemsSource = result;
            Results.RowHeight = 150;
            type = typeof(DB_Info);
        }

        private void Results_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null) { return; }
            if (type == typeof(PersonTable))
            {
                App.ShowInfoAbout(e.SelectedItem as PersonTable, sender, Width, Height, Navigation);
            }
            else if (type == typeof(Absence))
            {
                App.ShowInfoAbout(App.Data.Get<PersonTable>((e.SelectedItem as Absence).Id), sender, Width, Height, Navigation);
            }
            else if (type == typeof(_Activity))
            {
                App.ShowInfoAbout(App.Data.Get<PersonTable>((e.SelectedItem as _Activity).Id), sender, Width, Height, Navigation);
            }
            else if (type == typeof(Behavior))
            {
                App.ShowInfoAbout(App.Data.Get<PersonTable>((e.SelectedItem as Behavior).Id), sender, Width, Height, Navigation);
            }
        }

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            string RList = "";
            if (type == typeof(PersonTable))
            {
                List<PersonTable> list = Results.ItemsSource as List<PersonTable>;
                foreach (PersonTable item in list)
                {
                    RList += "\n" + Format(item);
                }
            }
            else if (type == typeof(Absence))
            {
                List<Absence> list = Results.ItemsSource as List<Absence>;
                foreach (Absence item in list)
                {
                    RList += "\n" + Format(item.Id);
                }
            }
            else if (type == typeof(_Activity))
            {
                List<_Activity> list = Results.ItemsSource as List<_Activity>;
                foreach (_Activity item in list)
                {
                    RList += "\n" + Format(item.Id);
                }
            }
            else if (type == typeof(Behavior))
            {
                List<Behavior> list = Results.ItemsSource as List<Behavior>;
                foreach (Behavior item in list)
                {
                    RList += "\n" + Format(item.Id);
                }
            }
            Share.RequestAsync(App.Data.Get<Tables.Settings>("ShareTextHead").SettingValue + RList, "مشاركة عبر");
        }

        //private string Format(Behavior item)
        //{
        //    return Format(App.Data.Get<PersonTable>(item.Id));
        //}

        //private string Format(_Activity item)
        //{
        //    return Format(App.Data.Get<PersonTable>(item.Id));
        //}

        private string Format(int item)
        {
            return Format(App.Data.Get<PersonTable>(item));
        }

        private string Format(PersonTable item)
        {
            string Setting = App.Settings.Where(x => x.SettingName == "ShareText").ElementAt(0).SettingValue;
            Setting = Setting.Replace("{درجة الحضور}", item.Absense.ToString(CultureInfo.CurrentCulture));
            Setting = Setting.Replace("{درجة التفاعل}", item.Activity.ToString(CultureInfo.CurrentCulture));
            Setting = Setting.Replace("{درجة السلوك}", item.Behaviour.ToString(CultureInfo.CurrentCulture));
            Setting = Setting.Replace("{رقم الهاتف}", item.TNum);
            Setting = Setting.Replace("{الاسم}", item.Name);
            foreach (var item2 in App.Data.Table<Fields>().ToList().Select(F => F.FieldName).ToList())
            {
                if(item2 == "الصورة" | !Setting.Contains(item2)) { continue; }
                Setting = Setting.Replace("{" + item2 + "}", App.Data.Query<Data>($"select `{item2}` as Value from Persons where Id = {item.Id.ToString(CultureInfo.InvariantCulture)};")[0].Value);
            }
            return Setting;
        }

        private void RandomB_Clicked(object sender, EventArgs e)
        {
            App.ChooseRandomPerson(Results.ItemsSource as List<PersonTable>, Width, Height, Navigation);
        }
    }
}