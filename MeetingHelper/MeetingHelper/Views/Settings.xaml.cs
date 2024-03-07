using MeetingHelper.Services;
using MeetingHelper.Tables;
using MeetingHelper.ViewModels;
using Plugin.FilePicker.Abstractions;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MeetingHelper.Views
{
    [DesignTimeVisible(true)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Settings : ContentPage
    {
        public bool DilgRslt = true;
        public Settings()
        {
            InitializeComponent();
            if ((((MasterDetailPage)Application.Current.MainPage).Master as ItemsPage).GetCurrentSelection() != "الإعدادات")
            {
                (((MasterDetailPage)Application.Current.MainPage).Master as ItemsPage).Navigate("الإعدادات");
            }
            if (DependencyService.Get<IPlatform>().UWPResult() == "UWP")
            {
                FlowDirection = FlowDirection.LeftToRight;
            }
            Title = "الإعدادات";
            Save.IconImageSource = ImageSource.FromResource("MeetingHelper.Save" + DependencyService.Get<IPlatform>().UWPResult() + ".png");
            Export.IconImageSource = ImageSource.FromResource("MeetingHelper.Export" + DependencyService.Get<IPlatform>().UWPResult() + ".png");
            Import.IconImageSource = ImageSource.FromResource("MeetingHelper.Import" + DependencyService.Get<IPlatform>().UWPResult() + ".png");

            DAtTime.Text = App.Settings.Where(x => x.SettingName == "DAtTime").ElementAt(0).SettingValue;
            DBehavior.Text = App.Settings.Where(x => x.SettingName == "DBehavior").ElementAt(0).SettingValue;
            DBehaviorDesc.Text = App.Settings.Where(x => x.SettingName == "DBehaviorDesc").ElementAt(0).SettingValue;
            DActivity.Text = App.Settings.Where(x => x.SettingName == "DActivity").ElementAt(0).SettingValue;
            DActivityDesc.Text = App.Settings.Where(x => x.SettingName == "DActivityDesc").ElementAt(0).SettingValue;
            ShareText.Text = App.Settings.Where(x => x.SettingName == "ShareText").ElementAt(0).SettingValue;
            ShareTextHead.Text = App.Settings.Where(x => x.SettingName == "ShareTextHead").ElementAt(0).SettingValue;

            foreach (Fields item in App.Data.Table<Fields>().ToList())
            {
                if (item.FieldName == "الصورة") { continue; }
                Fields.Children.Add(new MeetingHelper.ViewModels.FieldControl(item.FieldType, item.FieldName, item._FieldChoices != System.Array.Empty<string>() ? (item._FieldChoices[0] == "" & item._FieldChoices.Count() == 1 ? null : item._FieldChoices.ToList()) : null, false));
            }

            Button btnAdd = new Button() { Text = "اضافة حقل" };
            All.Children.Insert(2, btnAdd);
            btnAdd.Clicked += delegate { Fields.Children.Insert(Fields.Children.IndexOf(btnAdd), new MeetingHelper.ViewModels.FieldControl(FieldType.Srting, "حقل جديد")); };
        }
        public async void DispAlrt()
        {
            if (DAtTime.Text != App.Settings.Where(x => x.SettingName == "DAtTime").ElementAt(0).SettingValue | DBehavior.Text != App.Settings.Where(x => x.SettingName == "DBehavior").ElementAt(0).SettingValue | DBehaviorDesc.Text != App.Settings.Where(x => x.SettingName == "DBehaviorDesc").ElementAt(0).SettingValue | DActivity.Text != App.Settings.Where(x => x.SettingName == "DActivity").ElementAt(0).SettingValue | DActivityDesc.Text != App.Settings.Where(x => x.SettingName == "DActivityDesc").ElementAt(0).SettingValue | ShareText.Text != App.Settings.Where(x => x.SettingName == "ShareText").ElementAt(0).SettingValue | ShareTextHead.Text != App.Settings.Where(x => x.SettingName == "ShareTextHead").ElementAt(0).SettingValue)
            {
                DilgRslt = await DisplayAlert("", "هل تريد الخروج دون حفظ التغييرات؟", "خروج بدون حفظ", "حفظ");
                if (DilgRslt)
                {
                    ((MasterDetailPage)Application.Current.MainPage).SendBackButtonPressed();
                    return;
                }
                Save_Clicked(null, null);
            }
            DilgRslt = true;
            ((MasterDetailPage)Application.Current.MainPage).SendBackButtonPressed();
            return;
        }
        public void Save_Clicked(object sender, EventArgs e)
        {
            try
            {
                App.Data.Update(new Tables.Settings() { SettingName = "DAtTime", SettingValue = DAtTime.Text });
                App.Data.Update(new Tables.Settings() { SettingName = "DBehavior", SettingValue = DBehavior.Text });
                App.Data.Update(new Tables.Settings() { SettingName = "DBehaviorDesc", SettingValue = DBehaviorDesc.Text });
                App.Data.Update(new Tables.Settings() { SettingName = "DActivity", SettingValue = DActivity.Text });
                App.Data.Update(new Tables.Settings() { SettingName = "DActivityDesc", SettingValue = DActivityDesc.Text });
                App.Data.Update(new Tables.Settings() { SettingName = "ShareText", SettingValue = ShareText.Text });
                App.Data.Update(new Tables.Settings() { SettingName = "ShareTextHead", SettingValue = ShareTextHead.Text });

                var tmp = App.Data.Table<Fields>().ToList();
                foreach (FieldControl item in Fields.Children)
                {
                    if (item.FieldName == "" | item.FieldName == null)
                        continue;
                    if (item.Disabled)
                    {
                        if (item.CHoicesChng)
                        {
                            tmp.Where(x => x.FieldName == item.FieldName).ElementAt(0)._FieldChoices = item.FieldChoices.ToArray();
                        }
                        continue;
                    }
                    if (item.FieldChoices == null)
                    {
                        App.Data.Insert(new Fields() { FieldName = item.FieldName, FieldType = item.FieldType }); 
                    }
                    else
                    {
                        App.Data.Insert(new Fields() { FieldName = item.FieldName, FieldType = item.FieldType, _FieldChoices = item.FieldChoices.Where(x => x != "" & x != null).ToArray() });
                    }
                    App.Data.Execute($"alter table Persons add `{item.FieldName}` {ToSqlType(item.FieldType)}");
                }
                App.Data.UpdateAll(tmp);
                Acr.UserDialogs.UserDialogs.Instance.Toast("تم الحفظ بنجاح");
            }
            catch (Exception ex)
            {
                DisplayAlert("حدث خطأ غير متوقع", ex.Message + "\n" + ex.StackTrace, "حسنًا");
            }
        }

        private object ToSqlType(string fieldType)
        {
            switch (fieldType)
            {
                case Tables.FieldType.Date:
                    return "BIGINT";
                case Tables.FieldType.MultiChoice:
                    return "integer";
                case Tables.FieldType.Integer:
                    return "integer";
                case Tables.FieldType.Srting:
                    return "text";
                case Tables.FieldType.TelephoneNumber:
                    return "text";
                case Tables.FieldType.Time:
                    return "BIGINT";
                default:
                    return "";
            }
        }

        private async void Import_Clicked(object sender, EventArgs e)
        {
            try
            {
                await Plugin.Permissions.CrossPermissions.Current.RequestPermissionsAsync(Plugin.Permissions.Abstractions.Permission.Storage);
                FileData File = await Plugin.FilePicker.CrossFilePicker.Current.PickFile(new string[] { ".mhd" });
                if (File != null)
                {
                    if (!File.FilePath.ToLower().EndsWith(".mhd"))
                    {
                        Acr.UserDialogs.UserDialogs.Instance.Toast("الملف ليس من النوع المطلوب");
                        return;
                    }
                    if (await DisplayAlert("هل تريد المتابعة؟", "سيؤدي استيراد قاعدة البيانات إلى فقدان البيانات الحالية وتحميل البيانات الجديدة", "متابعة", "تراجع"))
                    {
                        App.Data.Close();
                        string[] Files = Directory.GetFiles(Path.GetDirectoryName(DependencyService.Get<ISQLiteGetPath>().GetPath()));
                        foreach (string item in Files)
                        {
                            System.IO.File.Delete(item);
                        }
                        if (DependencyService.Get<IPlatform>().UWPResult() == "UWP")
                        {
                            await DependencyService.Get<IFileManagement>().Copy(File.FilePath, Path.Combine(Path.GetDirectoryName(DependencyService.Get<ISQLiteGetPath>().GetPath()), "Impotr.mhd"));
                            System.IO.Compression.ZipFile.ExtractToDirectory(Path.Combine(Path.GetDirectoryName(DependencyService.Get<ISQLiteGetPath>().GetPath()), "Impotr.mhd"), Path.GetDirectoryName(DependencyService.Get<ISQLiteGetPath>().GetPath()));
                        }
                        else
                        {
                            System.IO.Compression.ZipFile.ExtractToDirectory(File.FilePath, Path.GetDirectoryName(DependencyService.Get<ISQLiteGetPath>().GetPath()));
                        }
                        await DependencyService.Get<IFileManagement>().Delete(Path.Combine(Path.GetDirectoryName(DependencyService.Get<ISQLiteGetPath>().GetPath()), "Impotr.mhd"));
                        App.Data = new SQLite.SQLiteConnection(new SQLite.SQLiteConnectionString(DependencyService.Get<ISQLiteGetPath>().GetPath(), true, "UU*9393p45QY73$3T4#82g5!q94997C27eExNK56"));
                        App.Data.CreateTable<PersonTable>();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                Debug.WriteLine(ex.InnerException);
                await DisplayAlert("حدث خطأ", ex.Message + "\n" + ex.InnerException + "\n" + ex.StackTrace, "حسنًا");
            }
        }

        private async void Export_Clicked(object sender, EventArgs e)
        {
            await Plugin.Permissions.CrossPermissions.Current.RequestPermissionsAsync(Plugin.Permissions.Abstractions.Permission.Storage);
            if (DependencyService.Get<IPlatform>().UWPResult() == "UWP")
            {
                await DependencyService.Get<IFileManagement>().Export();
            }
            else if (await Plugin.Permissions.CrossPermissions.Current.CheckPermissionStatusAsync(Plugin.Permissions.Abstractions.Permission.Storage) == Plugin.Permissions.Abstractions.PermissionStatus.Granted)
            {
                string SPath = Path.Combine(DependencyService.Get<IPlatform>().HomeDir(), (await Acr.UserDialogs.UserDialogs.Instance.PromptAsync(new Acr.UserDialogs.PromptConfig() { Text = "بيانات الاجتماع", Message = "اسم الملف:", OkText = "حسنًا", IsCancellable = false })).Value + ".mhd");
                int count = 1;
                while (File.Exists(SPath))
                {
                    string tempFileName = string.Format("{0} {1}", "مساعد الاجتماع", count++);
                    SPath = Path.Combine(Path.GetDirectoryName(SPath), tempFileName + ".mhd");
                }
                System.IO.Compression.ZipFile.CreateFromDirectory(Path.GetDirectoryName(DependencyService.Get<ISQLiteGetPath>().GetPath()), SPath, System.IO.Compression.CompressionLevel.Fastest, false);
                Acr.UserDialogs.UserDialogs.Instance.Toast("تم التصدير إلى " + SPath, new TimeSpan(0, 0, 4));
            }
            else
            {
                Acr.UserDialogs.UserDialogs.Instance.Toast("لم يتم الحصول على صلاحية الدخول إلى الذاكرة الداخلية", new TimeSpan(0, 0, 3));
            }
        }

        private void Setting_TextChanged(object sender, TextChangedEventArgs e)
        {
            Task.Run(DidChange);
        }

        private void DidChange()
        {
            if (DAtTime.Text != App.Settings.Where(x => x.SettingName == "DAtTime").ElementAt(0).SettingValue | DBehavior.Text != App.Settings.Where(x => x.SettingName == "DBehavior").ElementAt(0).SettingValue | DBehaviorDesc.Text != App.Settings.Where(x => x.SettingName == "DBehaviorDesc").ElementAt(0).SettingValue | DActivity.Text != App.Settings.Where(x => x.SettingName == "DActivity").ElementAt(0).SettingValue | DActivityDesc.Text != App.Settings.Where(x => x.SettingName == "DActivityDesc").ElementAt(0).SettingValue | ShareText.Text != App.Settings.Where(x => x.SettingName == "ShareText").ElementAt(0).SettingValue | ShareTextHead.Text != App.Settings.Where(x => x.SettingName == "ShareTextHead").ElementAt(0).SettingValue)
            {
                DilgRslt = false;
            }
            else
            {
                DilgRslt = true;
            }
        }
    }
}