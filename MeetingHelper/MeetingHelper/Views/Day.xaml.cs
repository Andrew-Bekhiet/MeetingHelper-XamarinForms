using IntelliAbb.Xamarin.Controls;
using MeetingHelper.Models;
using MeetingHelper.Services;
using MeetingHelper.Tables;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Behavior = MeetingHelper.Models.Behavior;

namespace MeetingHelper.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Day : TabbedPage
    {
        public ObservableCollection<Absence> PeopleA { get; set; }
        public ObservableCollection<Behavior> PeopleB { get; set; }
        public ObservableCollection<_Activity> PeopleAC { get; set; }

        private readonly List<PersonTable> Details;

        private TimeSpan TimeCame { get; set; } = DateTime.Now.TimeOfDay;
        private TimeSpan StartsIn { get; set; }
        private TimeSpan FinishIn { get; set; }
        public Day()
        {
            InitializeComponent();
            if ((((MasterDetailPage)Application.Current.MainPage).Master as ItemsPage).GetCurrentSelection() != "إضافة اليوم")
            {
                (((MasterDetailPage)Application.Current.MainPage).Master as ItemsPage).Navigate("إضافة اليوم");
            }
            if (DependencyService.Get<IPlatform>().UWPResult() == "UWP")
            {
                FlowDirection = FlowDirection.LeftToRight;
            }
            DaySave.IconImageSource = ImageSource.FromResource("MeetingHelper.Save" + DependencyService.Get<IPlatform>().UWPResult() + ".png", typeof(App).GetTypeInfo().Assembly);
            try
            {
                Details = (from row in App.Data.Table<PersonTable>().ToList() orderby row.GetName select row).ToList();
            }
            catch (NullReferenceException)
            {
                Details = new List<PersonTable>();
            }

            SizeChanged += Day_SizeChanged;

            InitVars();
        }

        private async void InitVars()
        {
            StartsIn = (await Acr.UserDialogs.UserDialogs.Instance.TimePromptAsync(new Acr.UserDialogs.TimePromptConfig() { IsCancellable = false, OkText = "ضبط", Title = "يجب القدوم في تمام الساعة:", SelectedTime = DateTime.Now.TimeOfDay })).Value;
            FinishIn = (await Acr.UserDialogs.UserDialogs.Instance.TimePromptAsync(new Acr.UserDialogs.TimePromptConfig() { IsCancellable = false, OkText = "ضبط", Title = "ينتهي الاجتماع في تمام الساعة:", SelectedTime = StartsIn.Add(new TimeSpan(2, 0, 0)) })).Value;

            Title = DateTime.Now.Date.Day + "-" + DateTime.Now.Date.Month + "-" + DateTime.Now.Date.Year + " " + StartsIn;

            PeopleA = new ObservableCollection<Absence>();
            foreach (PersonTable item in Details)
            {
                PeopleA.Add(new Absence(item.GetImage, item.GetName, DateTime.Now.TimeOfDay, int.Parse(App.Settings.Where(x => x.SettingName == "DAtTime").ElementAt(0).SettingValue), item.Id, item.FaceId, StartsIn, FinishIn, false, true, item.GetImagePath));
            }
            APersons.BeginRefresh();

            PeopleB = new ObservableCollection<Behavior>();
            foreach (PersonTable item in Details)
            {
                PeopleB.Add(new Behavior(item.GetImage, item.GetName, int.Parse(App.Settings.Where(x => x.SettingName == "DBehavior").ElementAt(0).SettingValue), item.Id, App.Settings.Where(x => x.SettingName == "DBehaviorDesc").ElementAt(0).SettingValue));
            }
            BPersons.BeginRefresh();

            PeopleAC = new ObservableCollection<_Activity>();
            foreach (PersonTable item in Details)
            {
                PeopleAC.Add(new _Activity(item.GetImage, item.GetName, int.Parse(App.Settings.Where(x => x.SettingName == "DActivity").ElementAt(0).SettingValue), item.Id, App.Settings.Where(x => x.SettingName == "DActivityDesc").ElementAt(0).SettingValue));
            }
            ACPersons.BeginRefresh();
        }


        public Day(Days day)
        {
            InitializeComponent();
            if (DependencyService.Get<IPlatform>().UWPResult() == "UWP")
            {
                FlowDirection = FlowDirection.LeftToRight;
            }

            List<Absence> AList = App.Data.Query<Absence>("SELECT Id, NameCheck, Time AS Time2, Bonus, Enabled FROM " + day.TableName + "_A");
            List<Behavior> BList = App.Data.Query<Behavior>("SELECT * FROM " + day.TableName + "_B");
            List<_Activity> ACList = App.Data.Query<_Activity>("SELECT * FROM " + day.TableName + "_AC");

            DaySave.IconImageSource = ImageSource.FromResource("MeetingHelper.Save.png", typeof(App));
            try
            {
                Details = App.Data.Table<PersonTable>().ToList();
            }
            catch (NullReferenceException)
            {
                Details = new List<PersonTable>();
            }
            Title = day.DayName + " " + day.Date;

            SizeChanged += Day_SizeChanged;

            PeopleA = new ObservableCollection<Absence>();
            foreach (Absence item in AList)
            {
                item.Name = App.Data.Get<PersonTable>(item.Id).GetName;
                item.Image = App.Data.Get<PersonTable>(item.Id).GetImage;
                PeopleA.Add(item);
            }
            APersons.BeginRefresh();

            PeopleB = new ObservableCollection<Behavior>();
            foreach (Behavior item in BList)
            {
                item.Name = App.Data.Get<PersonTable>(item.Id).GetName;
                item.Image = App.Data.Get<PersonTable>(item.Id).GetImage;
                item.Degrees = Math.Abs(item.Degrees);
                PeopleB.Add(item);
            }
            BPersons.BeginRefresh();

            PeopleAC = new ObservableCollection<_Activity>();
            foreach (_Activity item in ACList)
            {
                item.Name = App.Data.Get<PersonTable>(item.Id).GetName;
                item.Image = App.Data.Get<PersonTable>(item.Id).GetImage;
                item.Degrees = Math.Abs(item.Degrees);
                PeopleAC.Add(item);
            }
            ACPersons.BeginRefresh();
            DaySave.IsEnabled = false;
            UseFaceRec.IsEnabled = false;
        }

        private async void UseFID(object sender, EventArgs e)
        {
            try
            {
                await Plugin.Permissions.CrossPermissions.Current.RequestPermissionsAsync(new Plugin.Permissions.Abstractions.Permission[] { Plugin.Permissions.Abstractions.Permission.Camera, Plugin.Permissions.Abstractions.Permission.Storage });
                if (await Plugin.Permissions.CrossPermissions.Current.CheckPermissionStatusAsync(Plugin.Permissions.Abstractions.Permission.Camera) == Plugin.Permissions.Abstractions.PermissionStatus.Granted)
                {
                    MediaFile ImageF = await Plugin.Media.CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions() { DefaultCamera = CameraDevice.Front });
                    if (ImageF != null)
                    {
                        TimeCame = DateTime.Now.TimeOfDay;
                        int Face = await DependencyService.Get<IFaceOperations>().RecognizeFace(ImageF.Path, PeopleA.ToList().Where(x => x.FaceId != -1).Select(GetFaces).ToList());
                        CheckOnPerson(Face);
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("خطأ غير متوقع", ex.Message + "\n" + ex.StackTrace, "حسنًا");
            }
        }

        private (int, string) GetFaces(Absence arg)
        {
            return new ValueTuple<int, string>(arg.FaceId, arg.ImagePath);
        }

        private async void CheckOnPerson(int Face)
        {
            try
            {
                foreach (Absence Person in PeopleA)
                {
                    if (Person.FaceId == Face)
                    {
                        Person.NameCheck = true;
                        Person.Time = TimeCame;
                        APersons.ItemsSource = PeopleA;
                        Acr.UserDialogs.UserDialogs.Instance.Toast("تم اكتشاف وجه " + Person.Name);
                        return;
                    }
                }
                await DisplayAlert("خطأ", "لم يتم التعرف على هذا الوجه!", "حسنًا");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                Debug.WriteLine(ex.StackTrace);
                Debugger.Break();
            }
        }

        //private async Task<KeyValuePair<bool, Guid>> Verify(Guid Face)
        //{
        //    IList<SimilarFace> Result;
        //    try
        //    {
        //        Result = await App.FaceClient.Face.FindSimilarAsync(Face, App.ListId, maxNumOfCandidatesReturned:1);
        //        if (Result.Count >= 1)
        //        {
        //            return new KeyValuePair<bool, Guid>(Result[0].Confidence > 0.875, Result[0].PersistedFaceId ?? Result[0].FaceId ?? default);
        //        }
        //        else
        //        {
        //            return new KeyValuePair<bool, Guid>(false, default);
        //        }
        //    }
        //    // Catch and display Face API errors.
        //    catch (APIErrorException f)
        //    {
        //        await DisplayAlert("حدث خطأ غير متوقع", f.Message + "\n" + f.InnerException, "حسنًا");
        //        return new KeyValuePair<bool, Guid>(false, default);
        //    }
        //    // Catch and display all other errors.
        //    catch (Exception e)
        //    {
        //        await DisplayAlert("حدث خطأ غير متوقع", e.Message + "\n" + e.InnerException, "حسنًا");
        //        return new KeyValuePair<bool, Guid>(false, default);
        //    }
        //}

        private void Name(object sender, EventArgs e)
        {
            (((sender as View).Parent as StackLayout).Children.ElementAt(0) as Checkbox).IsChecked = !(((sender as View).Parent as StackLayout).Children.ElementAt(0) as Checkbox).IsChecked;
        }

        private async void DaySave_Clicked(object sender, EventArgs e)
        {
            try
            {
                string TableName = "Day_" + StartsIn.Ticks;
                Acr.UserDialogs.PromptConfig dayName = new Acr.UserDialogs.PromptConfig
                {
                    OkText = "حسنًا",
                    CancelText = "إلغاء الأمر",
                    Placeholder = "مثال: اجتماع إعدادي بنين",
                    Title = "يُرجى كتابة اسم اليوم"
                };
                string dayNameS = (await Acr.UserDialogs.UserDialogs.Instance.PromptAsync(dayName)).Value;
                if (dayNameS != "" & dayNameS != null)
                {
                    App.Data.Insert(new Days() { DayName = dayNameS, Date = DateTime.Now.Year.ToString() + "/" + DateTime.Now.Month.ToString() + "/" + DateTime.Now.Day.ToString() + " " + StartsIn.Hours.ToString() + ":" + StartsIn.Minutes.ToString(), TableName = TableName });

                    App.Data.Execute("CREATE TABLE IF NOT EXISTS " + TableName + "_A(RowId INTEGER Primary Key AUTOINCREMENT, Id INTEGER, NameCheck BOOL, Time LONG, Bonus INTEGER, Enabled BOOL DEFAULT FALSE);");
                    App.Data.Execute("CREATE TABLE IF NOT EXISTS " + TableName + "_B(RowId INTEGER Primary Key AUTOINCREMENT, Id INTEGER, Degrees INTEGER, Description VARCHAR, Enabled BOOL DEFAULT FALSE);");
                    App.Data.Execute("CREATE TABLE IF NOT EXISTS " + TableName + "_AC(RowId INTEGER Primary Key AUTOINCREMENT, Id INTEGER, Degrees INTEGER, Description VARCHAR, Enabled BOOL DEFAULT FALSE);");

                    foreach (Absence item in PeopleA)
                    {
                        App.Data.Execute("INSERT INTO " + TableName + "_A(Id, NameCheck, Time, Bonus) VALUES" + string.Format("('{0}', {1}, {2}, '{3}')", item.Id, new AddConvert().Convert(!item.NameCheck, null, null, null), item.Time2, item.Bonus));

                        if (item.NameCheck)
                        {
                            PersonTable P = App.Data.Get<PersonTable>(item.Id);
                            P.Absense += item.Bonus;
                            App.Data.Update(P);
                        }
                    }

                    foreach (Behavior item in PeopleB)
                    {
                        App.Data.Execute("INSERT INTO " + TableName + "_B(Id, Degrees, Description) VALUES" + string.Format("('{0}', '{1}', '{2}')", item.Id, item._add ? item.Degrees : -item.Degrees, item.Description));

                        PersonTable P = App.Data.Get<PersonTable>(item.Id);
                        P.Behaviour += item._add ? item.Degrees : -item.Degrees;
                        App.Data.Update(P);
                    }

                    foreach (_Activity item in PeopleAC)
                    {
                        App.Data.Execute("INSERT INTO " + TableName + "_AC(Id, Degrees, Description) VALUES" + string.Format("('{0}', '{1}', '{2}')", item.Id, item._add ? item.Degrees : -item.Degrees, item.Description));

                        PersonTable P = App.Data.Get<PersonTable>(item.Id);
                        P.Activity += item._add ? item.Degrees : -item.Degrees;
                        App.Data.Update(P);
                    }
                    App.Current.MainPage.SendBackButtonPressed();
                    Acr.UserDialogs.UserDialogs.Instance.Toast("تم الحفظ بنجاح");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("حدث خطأ غير متوقع!", ex + "\n" + ex.InnerException + "\n" + ex.StackTrace, "حسنًا");
            }

        }

        private void DASearch_SearchButtonPressed(object sender, EventArgs e)
        {
            ObservableCollection<Absence> SearchResult = new ObservableCollection<Absence>(PeopleA.ToList().FindAll(Criteria => Criteria.ToString().Contains(DASearch.Text)));
            APersons.ItemsSource = SearchResult;
        }
        private void DASearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (DASearch.Text == "")
            {
                APersons.BeginRefresh();
            }
        }
        private void APersons_Refreshing(object sender, EventArgs e)
        {
            APersons.ItemsSource = PeopleA;
            APersons.EndRefresh();
        }
        private void APersons_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null) { return; }
            PersonTable SelectedP = Details.Find(Person => Person.Id == (e.SelectedItem as Absence).Id);
            App.ShowInfoAbout(SelectedP, sender, Width, Height, Navigation);
        }


        private void DBSearch_SearchButtonPressed(object sender, EventArgs e)
        {
            ObservableCollection<Behavior> SearchResult = new ObservableCollection<Behavior>(PeopleB.ToList().FindAll(Criteria => Criteria.ToString().Contains(DBSearch.Text)));
            BPersons.ItemsSource = SearchResult;
        }
        private void DBSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (DBSearch.Text == "")
            {
                BPersons.BeginRefresh();
            }
        }
        private void BPersons_Refreshing(object sender, EventArgs e)
        {
            BPersons.ItemsSource = PeopleB;
            BPersons.EndRefresh();
        }
        private void BPersons_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null) { return; }
            PersonTable SelectedP = Details.Find(Person => Person.Id == (e.SelectedItem as Behavior).Id);

            App.ShowInfoAbout(SelectedP, sender, Width, Height, Navigation);
        }


        private void DACSearch_SearchButtonPressed(object sender, EventArgs e)
        {
            ObservableCollection<_Activity> SearchResult = new ObservableCollection<_Activity>(PeopleAC.ToList().FindAll(Criteria => Criteria.ToString().Contains(DACSearch.Text)));
            ACPersons.ItemsSource = SearchResult;
        }
        private void DACSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (DACSearch.Text == "")
            {
                ACPersons.BeginRefresh();
            }
        }
        private void ACPersons_Refreshing(object sender, EventArgs e)
        {
            ACPersons.ItemsSource = PeopleAC;
            ACPersons.EndRefresh();
        }
        private void ACPersons_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null) { return; }
            PersonTable SelectedP = Details.Find(Person => Person.Id == (e.SelectedItem as _Activity).Id);

            App.ShowInfoAbout(SelectedP, sender, Width, Height, Navigation);
        }

        private void Day_SizeChanged(object sender, EventArgs e)
        {
            if (Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopupStack.Count != 0)
            {
                Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopupStack[0].Padding = new Thickness(20 * Width / 360, 40 * Height / 560, 20 * Width / 360, 70 * Height / 560);
            }
        }
    }

    public class AddConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? 0 : 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value == 0;
        }
    }
    //public class Tunnel : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        return value;
    //    }
    //
    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        return value;
    //    }
    //}
}