using MeetingHelper.Services;
using MeetingHelper.Tables;
using MeetingHelper.Views;
using Syncfusion.ListView.XForms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Settings = MeetingHelper.Tables.Settings;

namespace MeetingHelper
{
    public partial class App : Application
    {
        public static SQLite.SQLiteConnection Data = new SQLite.SQLiteConnection(new SQLite.SQLiteConnectionString(DependencyService.Get<ISQLiteGetPath>().GetPath(), true, "UU*9393p45QY73$3T4#82g5!q94997C27eExNK56"));
        public static List<Settings> Settings => (from x in Data.Table<Settings>() select x).ToList();
        public static readonly string RepHomeDir = DependencyService.Get<ISQLiteGetPath>().GetDefault();
        public static readonly Random R = new Random();
        //internal static App Instance { get; private set; }
        private readonly bool DontLoad = false;
        public App()
        {
            InitializeComponent();
            DependencyService.Register<ISQLiteGetPath>();
            DependencyService.Register<IFileManagement>();
            DependencyService.Register<IPlatform>();
            DependencyService.Register<IUserContactsService>();
            DependencyService.Register<IFaceOperations>();


        }
        public App(System.EventHandler<string> NextLoadText)
        {
            try
            {
                InitializeComponent();

                DontLoad = true;

                DependencyService.Register<ISQLiteGetPath>();
                DependencyService.Register<IFileManagement>();
                DependencyService.Register<IPlatform>();
                DependencyService.Register<IUserContactsService>();
                DependencyService.Register<IFaceOperations>();

                Plugin.Permissions.CrossPermissions.Current.RequestPermissionsAsync(Plugin.Permissions.Abstractions.Permission.Storage);

                NextLoadText?.Invoke(this, "جار تحميل الوجوه ...");
                if (!File.Exists(Path.Combine(DependencyService.Get<ISQLiteGetPath>().GetDefault(), "MeetingHelper.OpenCV.haarcascade_frontalface_alt.xml")))
                {
                    MemoryStream tmp = new MemoryStream();
                    Assembly.GetAssembly(typeof(App)).GetManifestResourceStream("MeetingHelper.OpenCV.haarcascade_frontalface_alt.xml").CopyTo(tmp);
                    File.WriteAllBytes(Path.Combine(DependencyService.Get<ISQLiteGetPath>().GetDefault(), "MeetingHelper.OpenCV.haarcascade_frontalface_alt.xml"), tmp.ToArray());
                    tmp.Dispose();
                }

                NextLoadText?.Invoke(this, "جار تحميل قاعدة البيانات ...");

                Data.CreateTable<Settings>();
                Data.CreateTable<Fields>();
                Data.CreateTable<SQLSavedQuery>();

                Data.CreateTable<PersonTable>();
                Data.CreateTable<Days>();

                NextLoadText?.Invoke(this, "جار البدء ...");
            }
            catch (Exception ex)
            {
                NextLoadText?.Invoke(this, "حدث خطأ...\n" + ex.Message + "\n" + ex.StackTrace);
                Debugger.Break();
                System.Threading.Thread.Sleep(10000);
            }
        }
        public static void PrepareSettings()
        {
            try
            {
                Data.Insert(new Settings() { SettingName = "DAtTime", SettingValue = "25" });
                Data.Insert(new Settings() { SettingName = "DBehavior", SettingValue = "0" });
                Data.Insert(new Settings() { SettingName = "DBehaviorDesc", SettingValue = "" });
                Data.Insert(new Settings() { SettingName = "DActivity", SettingValue = "0" });
                Data.Insert(new Settings() { SettingName = "DActivityDesc", SettingValue = "" });
                Data.Insert(new Settings() { SettingName = "ShareText", SettingValue = "{الاسم}" });
                Data.Insert(new Settings() { SettingName = "ShareTextHead", SettingValue = "مبروك لكل من:" });

                Data.Insert(new Fields() { FieldName = "الصورة", FieldType = FieldType.Image });
                Data.Insert(new Fields() { FieldName = "الاسم", FieldType = FieldType.Srting });
                Data.Insert(new Fields() { FieldName = "رقم الهاتف", FieldType = FieldType.Srting });
                Data.Insert(new Fields() { FieldName = "العنوان", FieldType = FieldType.Srting });
                Data.Execute("alter table Persons add column 'العنوان' varchar");
            }
            catch (Exception ex)
            {
                Debugger.Break();
            }
        }

        protected override void OnStart()
        {
            //File.Copy(DependencyService.Get<ISQLiteGetPath>().GetPath(), Path.Combine(DependencyService.Get<IPlatform>().HomeDir(), "Data.db"));
            if (!DontLoad)
            {
                Plugin.Permissions.CrossPermissions.Current.RequestPermissionsAsync(Plugin.Permissions.Abstractions.Permission.Storage);

                try
                {
                    Data.CreateTable<Settings>();
                }
                catch (SQLite.SQLiteException ex)
                {
                    Data = new SQLite.SQLiteConnection(new SQLite.SQLiteConnectionString(DependencyService.Get<ISQLiteGetPath>().GetPath(), true, "UU*9393p45QY73$3T4#82g5!q94997C27eExNK56"));
                    Data.CreateTable<Settings>();
                }
                Data.CreateTable<Fields>();
                Data.CreateTable<SQLSavedQuery>();

                Data.CreateTable<PersonTable>();
                Data.CreateTable<Days>();

                if (!File.Exists(Path.Combine(DependencyService.Get<ISQLiteGetPath>().GetDefault(), "MeetingHelper.OpenCV.haarcascade_frontalface_alt.xml")))
                {
                    MemoryStream tmp = new MemoryStream();
                    Assembly.GetAssembly(typeof(App)).GetManifestResourceStream("MeetingHelper.OpenCV.haarcascade_frontalface_alt.xml").CopyTo(tmp);
                    File.WriteAllBytes(Path.Combine(DependencyService.Get<ISQLiteGetPath>().GetDefault(), "MeetingHelper.OpenCV.haarcascade_frontalface_alt.xml"), tmp.ToArray());
                    tmp.Dispose();
                }

                MemoryStream tmp2 = new MemoryStream();
                Assembly.GetAssembly(typeof(App)).GetManifestResourceStream("MeetingHelper.Person.png").CopyTo(tmp2);
                File.WriteAllBytes(Path.Combine(DependencyService.Get<ISQLiteGetPath>().GetDefault(), "MeetingHelper.Person.png"), tmp2.ToArray());
                tmp2.Dispose();

                if (Settings.Count < 1)
                {
                    PrepareSettings();
                }
                MainPage = new MainPage();
            }
        }

        protected override void OnSleep()
        {
            //Close Data
            Data.Close();
        }

        protected override void OnResume()
        {
            //Open Data again
            Data = new SQLite.SQLiteConnection(new SQLite.SQLiteConnectionString(DependencyService.Get<ISQLiteGetPath>().GetPath(), true, "UU*9393p45QY73$3T4#82g5!q94997C27eExNK56"));
        }

        public static void ShowInfoAbout(PersonTable SelectedP, object sender, double Width, double Height, INavigation Navigation)
        {
            StackLayout SL = new StackLayout()
            {
                Children =
                {
                    new Label(){ Text = "الصورة:", HorizontalOptions = LayoutOptions.Fill, FontAttributes = FontAttributes.Bold, FontSize = Device.GetNamedSize (NamedSize.Large, typeof(Label))},
                    new Xamarin.Forms.Image(){ Source = SelectedP.GetImage, HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.StartAndExpand, HeightRequest = 80}
                }
            };

            for (int i = 1; i < SelectedP.PData.Count; i++)
            {
                SL.Children.Add(new Label() { FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)), FontAttributes = FontAttributes.Bold, Text = SelectedP.PFData.ElementAt(i), HorizontalOptions = LayoutOptions.Fill });
                switch (App.Data.Table<Fields>().ToList().ElementAt(i).FieldType)
                {
                    case FieldType.Integer:
                        SL.Children.Add(new Label() { FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)), Text = SelectedP.PData.ElementAt(i), HorizontalOptions = LayoutOptions.Fill, HorizontalTextAlignment = TextAlignment.Start });
                        break;
                    case FieldType.Srting:
                        SL.Children.Add(new Label() { FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)), Text = SelectedP.PData.ElementAt(i), HorizontalOptions = LayoutOptions.Fill, HorizontalTextAlignment = TextAlignment.Start });
                        break;
                    case FieldType.TelephoneNumber:
                        SL.Children.Add(new Label() { FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)), Text = SelectedP.PData.ElementAt(i), HorizontalOptions = LayoutOptions.Fill, HorizontalTextAlignment = TextAlignment.Start });
                        break;
                    case FieldType.Date:
                        SL.Children.Add(new Label() { FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)), Text = new DateTime(long.Parse(SelectedP.PData.ElementAt(i))).ToLongDateString(), HorizontalOptions = LayoutOptions.Fill, HorizontalTextAlignment = TextAlignment.Start });
                        break;
                    case FieldType.Time:
                        SL.Children.Add(new Label() { FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)), Text = new TimeSpan(long.Parse(SelectedP.PData.ElementAt(i))).ToString(), HorizontalOptions = LayoutOptions.Fill, HorizontalTextAlignment = TextAlignment.Start });
                        break;
                    case FieldType.MultiChoice:
                        SL.Children.Add(new Label() { FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)), Text = App.Data.Table<Fields>().ToList().Find(x => x.FieldName == SelectedP.PFData.ElementAt(i))._FieldChoices.ToList()[int.Parse(SelectedP.PData.ElementAt(i))], HorizontalOptions = LayoutOptions.Fill, HorizontalTextAlignment = TextAlignment.Start });
                        break;
                }
            }
            SL.Children.Add(new Label() { Text = "الدرجات:", HorizontalOptions = LayoutOptions.Fill, FontAttributes = FontAttributes.Bold, FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)) });
            SL.Children.Add(new Label() { Text = "التفاعل:", HorizontalOptions = LayoutOptions.Fill, FontAttributes = FontAttributes.Bold, FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)) });
            SL.Children.Add(new Label() { Text = SelectedP.Activity.ToString(), HorizontalOptions = LayoutOptions.Fill, FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)), HorizontalTextAlignment = TextAlignment.Start });
            SL.Children.Add(new Label() { Text = "السلوك:", HorizontalOptions = LayoutOptions.Fill, FontAttributes = FontAttributes.Bold, FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)) });
            SL.Children.Add(new Label() { Text = SelectedP.Behaviour.ToString(), HorizontalOptions = LayoutOptions.Fill, FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)), HorizontalTextAlignment = TextAlignment.Start });
            SL.Children.Add(new Label() { Text = "الغياب والحضور:", HorizontalOptions = LayoutOptions.Fill, FontAttributes = FontAttributes.Bold, FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)) });
            SL.Children.Add(new Label() { Text = SelectedP.Absense.ToString(), HorizontalOptions = LayoutOptions.Fill, FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)), HorizontalTextAlignment = TextAlignment.Start });

            SL.BackgroundColor = Xamarin.Forms.Color.FromRgba(255, 255, 255, 245);

            Button Edit = new Button()
            {
                HorizontalOptions = LayoutOptions.Start,
                BackgroundColor = Xamarin.Forms.Color.Transparent,
                TextColor = Xamarin.Forms.Color.Accent,
                Text = "تعديل"
            };
            Button Dial = new Button()
            {
                HorizontalOptions = LayoutOptions.Center,
                BackgroundColor = Xamarin.Forms.Color.Transparent,
                TextColor = Xamarin.Forms.Color.Accent,
                Text = "الإنتقال إلى لوحة الاتصال"
            };
            Button OK = new Button()
            {
                HorizontalOptions = LayoutOptions.End,
                BackgroundColor = Xamarin.Forms.Color.Transparent,
                TextColor = Xamarin.Forms.Color.Accent,
                Text = "حسنًا"
            };

            Rg.Plugins.Popup.Pages.PopupPage dialog = new Rg.Plugins.Popup.Pages.PopupPage
            {
                Content = new StackLayout()
                {
                    Children =
                            {
                                new ScrollView()
                                {
                                    Content = SL
                                },
                                new StackLayout()
                                {
                                    BackgroundColor = Xamarin.Forms.Color.FromRgba(255, 255, 255, 245),
                                    Orientation = StackOrientation.Horizontal,
                                    Children =
                                    {
                                        Edit,
                                        Dial,
                                        OK
                                    }
                                }
                            }
                },
                Padding = new Thickness(20 * Width / 360, 40 * Height / 560, 20 * Width / 360, 70 * Height / 560)
            };
            Edit.Clicked += delegate
            {
                Navigation.PushAsync(new Person(SelectedP));
                ((ListView)sender).SelectedItem = null;
                Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAllAsync(true);
            };
            Dial.Clicked += delegate
            {
                ((ListView)sender).SelectedItem = null;
                Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAllAsync(true);
                Xamarin.Essentials.PhoneDialer.Open(SelectedP.TNum);
            };
            OK.Clicked += delegate
            {
                ((ListView)sender).SelectedItem = null;
                Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAllAsync(true);
            };
            dialog.BackgroundClicked += delegate
            {
                ((ListView)sender).SelectedItem = null;
                Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAllAsync(true);
            };
            Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(dialog, true);
            dialog.BackgroundClicked += delegate { ((ListView)sender).SelectedItem = null; };
        }

        public static void ChooseRandomPerson(List<PersonTable> list, double Width, double Height, INavigation Navigation)
        {
            StackLayout S = new StackLayout().LoadFromXaml("<StackLayout xmlns=\"http://xamarin.com/schemas/2014/forms\" xmlns:x=\"http://schemas.microsoft.com/winfx/2009/xaml\" x:Class=\"MeetingHelper.Persons\" Orientation=\"Horizontal\" HorizontalOptions=\"Fill\"><Image Source=\"{Binding GetImage}\" HorizontalOptions=\"Start\" WidthRequest=\"80\" /><StackLayout Orientation=\"Vertical\" HorizontalOptions=\"FillAndExpand\"><Label Text = \"{Binding GetName}\" FontSize=\"Large\" HorizontalOptions=\"StartAndExpand\"/><Label Text = \"{Binding GetPhoneNumber}\" FontSize=\"Small\" HorizontalOptions=\"StartAndExpand\"/></StackLayout></StackLayout>");
            TapGestureRecognizer tgr = new TapGestureRecognizer();
            tgr.Tapped += delegate
            {
                App.ShowInfoAbout((S.BindingContext as PersonTable), new ListView(), Width, Height, Navigation);
            };
            S.Children.ToList().ForEach(v => v.GestureRecognizers.Add(tgr));
            S.BindingContext = list?[App.R.Next(0, App.Data.Table<MeetingHelper.Tables.PersonTable>().ToList().Count)];
            S.BackgroundColor = Color.White;

            Rg.Plugins.Popup.Pages.PopupPage Person = new Rg.Plugins.Popup.Pages.PopupPage()
            {
                Content = S,
                Padding = new Thickness(Width * 10 / 100, Height * 40 / 100)
            };
            Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(Person, true);
        }

        //public static async Task<IList<DetectedFace>> IsFace(string imageFilePath, byte[] Image = null)
        //{
        //    try
        //    {
        //        
        //    }
        //    catch (Exception e)
        //    {
        //        Debug.WriteLine(e.Message, "IsFace Error");
        //        return new List<DetectedFace>();
        //    }
        //}
    }
}
