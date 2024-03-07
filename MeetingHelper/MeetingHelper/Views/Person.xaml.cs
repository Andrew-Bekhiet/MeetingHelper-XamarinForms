using Acr.UserDialogs;
using MeetingHelper.Models;
using MeetingHelper.Services;
using MeetingHelper.Tables;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MeetingHelper
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Person : ContentPage
    {
        public bool IsPresent = false;
        private readonly StreamPath Image = new StreamPath() { Path = Path.Combine(Path.GetDirectoryName(DependencyService.Get<ISQLiteGetPath>().GetPath()), "MeetingHelper.Person.png"), Stream = null };
        private readonly string OldImagePath;
        private new readonly int Id = 0;
        private readonly string UWP = DependencyService.Get<IPlatform>().UWPResult();
        private bool FaceRecognized;
        private readonly PersonTable This = null;
        public Person()
        {
            InitializeComponent();
            if (DependencyService.Get<IPlatform>().UWPResult() == "UWP")
            {
                FlowDirection = FlowDirection.LeftToRight;
            }
            if (!IsPresent) { ToolbarItems.Remove(PDelete); }
            //Label_Clicked.Tapped += Fields;
            PImage.Clicked += Image_Tapped;
            PImage.Source = ImageSource.FromResource("MeetingHelper.Person.png", typeof(App));
            PSave.IconImageSource = ImageSource.FromResource("MeetingHelper.Save" + UWP + ".png");
            //PAFB.ImageSource = ImageSource.FromResource("MeetingHelper.AddField.png", typeof(App));
            OldImagePath = Image.Path;
            Title = "مخدوم جديد";
            FaceRecognized = false;
            SizeChanged += Person_SizeChanged;
            PersonTable P = new PersonTable();
            for (int i = 0; i < P.PFData.Count - 1; i++)
            {
                Label l = new Label()
                {
                    Text = P.PFData.ElementAt(i + 1),
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalTextAlignment = TextAlignment.Center,
                    VerticalOptions = LayoutOptions.Fill,
                    FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label))
                };
                StackLayout StL = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Start,
                    Children =
                    {
                        l
                    }
                };
                switch (App.Data.Table<Fields>().ToList().ElementAt(i + 1).FieldType)
                {
                    case Tables.FieldType.Date:
                        StL.Children.Add(new DatePicker()
                        {
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            VerticalOptions = LayoutOptions.Fill,
                            Margin = 5,
                            FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(DatePicker))
                        });
                        break;
                    case Tables.FieldType.MultiChoice:
                        StL.Children.Add(new Picker()
                        {
                            ItemsSource = App.Data.Table<Fields>().ToList().Find(x => x.FieldName == P.PFData.ElementAt(i + 1))._FieldChoices.ToList(),
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            VerticalOptions = LayoutOptions.Fill,
                            Margin = 5,
                            FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Picker)),
                        });
                        break;
                    case Tables.FieldType.Integer:
                        StL.Children.Add(new Entry()
                        {
                            HorizontalTextAlignment = TextAlignment.Center,
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            VerticalOptions = LayoutOptions.Fill,
                            Margin = 5,
                            Keyboard = Keyboard.Numeric,
                            FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                            ReturnType = ReturnType.Next
                        });
                        break;
                    case Tables.FieldType.Srting:
                        StL.Children.Add(new Entry()
                        {
                            Placeholder = P.PFData.ElementAt(i + 1),
                            HorizontalTextAlignment = TextAlignment.Center,
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            VerticalOptions = LayoutOptions.Fill,
                            Margin = 5,
                            FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                            ReturnType = ReturnType.Next
                        });
                        break;
                    case Tables.FieldType.TelephoneNumber:
                        StL.Children.Add(new Entry()
                        {
                            HorizontalTextAlignment = TextAlignment.Center,
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            VerticalOptions = LayoutOptions.Fill,
                            Margin = 5,
                            FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                            ReturnType = ReturnType.Next
                        });
                        break;
                    case Tables.FieldType.Time:
                        StL.Children.Add(new TimePicker()
                        {
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            VerticalOptions = LayoutOptions.Fill,
                            Margin = 5,
                            FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                        });
                        break;
                }

                PAFLL.Children.Add(StL);
            }
        }
        public Person(PersonTable P)
        {
            InitializeComponent();
            IsPresent = true;
            //Label_Clicked.Tapped += Fields;
            PImage.Clicked += Image_Tapped;
            PImage.Source = ImageSource.FromResource("MeetingHelper.Person.png", typeof(App));
            PSave.IconImageSource = ImageSource.FromResource("MeetingHelper.Save.png", typeof(App));
            PDelete.IconImageSource = ImageSource.FromResource("MeetingHelper.Delete.png", typeof(App));
            //PAFB.ImageSource = ImageSource.FromResource("MeetingHelper.AddField.png", typeof(App));
            OldImagePath = Image.Path;
            Title = "مخدوم جديد";
            SizeChanged += Person_SizeChanged;

            This = P;
            FaceRecognized = P.FaceId != -1;
            Title = P.GetName;
            Id = P.Id;
            Image.Path = P.GetImagePath;
            PImage.Source = ImageSource.FromFile(Image.Path);

            for (int i = 0; i < P.PData.Count - 1; i++)
            {
                Label l = new Label()
                {
                    Text = P.PFData.ElementAt(i + 1),
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalTextAlignment = TextAlignment.Center,
                    VerticalOptions = LayoutOptions.Fill,
                    FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label))
                };
                StackLayout StL = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Start,
                    Children =
                    {
                        l
                    }
                };
                switch (App.Data.Table<Fields>().ToList().ElementAt(i + 1).FieldType)
                {
                    case Tables.FieldType.Date:
                        StL.Children.Add(new DatePicker()
                        {
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            VerticalOptions = LayoutOptions.Fill,
                            Margin = 5,
                            Date = new DateTime(long.Parse(P.PData.ElementAt(i + 1))),
                            FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(DatePicker))
                        });
                        break;
                    case Tables.FieldType.MultiChoice:
                        StL.Children.Add(new Picker()
                        {
                            ItemsSource = App.Data.Table<Fields>().ToList().Find(x => x.FieldName == P.PFData.ElementAt(i + 1))._FieldChoices.ToList(),
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            VerticalOptions = LayoutOptions.Fill,
                            Margin = 5,
                            FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Picker)),
                            SelectedIndex = int.Parse(P.PData.ElementAt(i + 1))
                        });
                        break;
                    case Tables.FieldType.Integer:
                        StL.Children.Add(new Entry()
                        {
                            Text = P.PData.ElementAt(i + 1),
                            HorizontalTextAlignment = TextAlignment.Center,
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            VerticalOptions = LayoutOptions.Fill,
                            Margin = 5,
                            Keyboard = Keyboard.Numeric,
                            FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                            ReturnType = ReturnType.Next
                        });
                        break;
                    case Tables.FieldType.Srting:
                        StL.Children.Add(new Entry()
                        {
                            Text = P.PData.ElementAt(i + 1),
                            HorizontalTextAlignment = TextAlignment.Center,
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            VerticalOptions = LayoutOptions.Fill,
                            Margin = 5,
                            FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                            ReturnType = ReturnType.Next
                        });
                        break;
                    case Tables.FieldType.TelephoneNumber:
                        StL.Children.Add(new Entry()
                        {
                            Text = P.PData.ElementAt(i + 1),
                            HorizontalTextAlignment = TextAlignment.Center,
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            VerticalOptions = LayoutOptions.Fill,
                            Margin = 5,
                            FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                            ReturnType = ReturnType.Next
                        });
                        break;
                    case Tables.FieldType.Time:
                        StL.Children.Add(new TimePicker()
                        {
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            VerticalOptions = LayoutOptions.Fill,
                            Margin = 5,
                            Time = TimeSpan.FromTicks(long.Parse(P.PData.ElementAt(i + 1))),
                            FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                        });
                        break;
                }

                PAFLL.Children.Add(StL);
            }
            PActivity.Text = P.Activity.ToString();
            PAbsense.Text = P.Absense.ToString();
            PBehav.Text = P.Behaviour.ToString();
        }
        public Person(string Name, string PhoneNum, ImageSource Image, string Address = "")
        {
            InitializeComponent();
            ToolbarItems.Remove(PDelete);
            //Label_Clicked.Tapped += Fields;
            PImage.Clicked += Image_Tapped;
            PImage.Source = ImageSource.FromResource("MeetingHelper.Person.png", typeof(App));
            PSave.IconImageSource = ImageSource.FromResource("MeetingHelper.Save.png", typeof(App));
            //PAFB.ImageSource = ImageSource.FromResource("MeetingHelper.AddField.png", typeof(App));
            OldImagePath = this.Image.Path;
            Title = "مخدوم جديد";
            SizeChanged += Person_SizeChanged;

            FaceRecognized = false;
            Title = Name;
            PImage.Source = Image;
            PersonTable P = new PersonTable();
            for (int i = 0; i < P.PFData.Count - 1; i++)
            {
                Label l = new Label()
                {
                    Text = P.PFData.ElementAt(i + 1),
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalTextAlignment = TextAlignment.Center,
                    VerticalOptions = LayoutOptions.Fill,
                    FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label))
                };
                StackLayout StL = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Start,
                    Children =
                    {
                        l
                    }
                };
                switch (App.Data.Table<Fields>().ToList().ElementAt(i + 1).FieldType)
                {
                    case Tables.FieldType.Date:
                        StL.Children.Add(new DatePicker()
                        {
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            VerticalOptions = LayoutOptions.Fill,
                            Margin = 5,
                            FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(DatePicker))
                        });
                        break;
                    case Tables.FieldType.MultiChoice:
                        StL.Children.Add(new Picker()
                        {
                            ItemsSource = App.Data.Table<Fields>().ToList().Find(x => x.FieldName == P.PFData.ElementAt(i + 1))._FieldChoices.ToList(),
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            VerticalOptions = LayoutOptions.Fill,
                            Margin = 5,
                            FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Picker)),
                        });
                        break;
                    case Tables.FieldType.Integer:
                        StL.Children.Add(new Entry()
                        {
                            HorizontalTextAlignment = TextAlignment.Center,
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            VerticalOptions = LayoutOptions.Fill,
                            Margin = 5,
                            Keyboard = Keyboard.Numeric,
                            FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                            ReturnType = ReturnType.Next,
                            Placeholder = P.PFData.ElementAt(i + 1)
                        });
                        break;
                    case Tables.FieldType.Srting:
                        StL.Children.Add(new Entry()
                        {
                            Text = i == 0 ? Name : (i == 1 ? PhoneNum : (i == 2 ? Address : "")),
                            HorizontalTextAlignment = TextAlignment.Center,
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            VerticalOptions = LayoutOptions.Fill,
                            Margin = 5,
                            FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                            ReturnType = ReturnType.Next,
                            Placeholder = P.PFData.ElementAt(i + 1)
                        });
                        break;
                    case Tables.FieldType.TelephoneNumber:
                        StL.Children.Add(new Entry()
                        {
                            HorizontalTextAlignment = TextAlignment.Center,
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            VerticalOptions = LayoutOptions.Fill,
                            Margin = 5,
                            FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                            ReturnType = ReturnType.Next,
                            Placeholder = P.PFData.ElementAt(i + 1)
                        });
                        break;
                    case Tables.FieldType.Time:
                        StL.Children.Add(new TimePicker()
                        {
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            VerticalOptions = LayoutOptions.Fill,
                            Margin = 5,
                            FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label))
                        });
                        break;
                }

                PAFLL.Children.Add(StL);
            }
        }

        private async void Image_Tapped(object sender, EventArgs e)
        {
            await SelectImage(sender);
        }
        private async Task SelectImage(object sender)
        {
            if (sender != null)
            {
                ((View)sender).IsEnabled = false;
            }

            string Result = await UserDialogs.Instance.ActionSheetAsync("اختيار صورة", "إلغاء الأمر", null, null, "اختيار صورة من المعرض", "إلتقاط صورة من الكاميرا");
            if (Result == "اختيار صورة من المعرض")
            {
                Picture_Selected(await Plugin.Media.CrossMedia.Current.PickPhotoAsync(), sender);
            }
            else if (Result == "إلتقاط صورة من الكاميرا")
            {
                Picture_Selected(await Plugin.Media.CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions() { AllowCropping = true, DefaultCamera = CameraDevice.Rear }), sender);
            }
            if (sender != null)
            {
                ((View)sender).IsEnabled = true;
            }
        }
        private void Picture_Selected(MediaFile File, object sender)
        {
            if (sender != null)
            {
                ((View)sender).IsEnabled = true;
            }

            if (File != null)
            {
                Image.Path = File.Path;
                Image.Stream = File.GetStream();
                FaceRecognized = false;
                if (sender != null)
                {
                    ((ImageButton)sender).Source = ImageSource.FromStream(() => File.GetStream());
                }

                if (sender != null)
                {
                    ((ImageButton)sender).Aspect = Aspect.AspectFit;
                }
            }
        }

        private async void PDelete_Clicked(object sender, EventArgs e)
        {
            if (await UserDialogs.Instance.ConfirmAsync("هل أنت متأكد أنك تريد حذف " + ((PAFLL.Children[0] as StackLayout).Children[1] as Entry).Text + "؟", "تأكيد", "نعم", "لا"))
            {
                //await App.FaceClient.FaceList.DeleteFaceAsync(App.ListId, This.FaceId);
                App.Data.Delete(This);
                PCancel_Clicked(null, null);
            }
        }
        private async Task ShowLoading()
        {
            UserDialogs.Instance.ShowLoading("جار التحميل ...", MaskType.Black);
        }
        private async void PSave_Clicked(object sender, EventArgs e)
        {
            try
            {
                List<FieldDescription> keys = Verify();
                if (keys.ToList().TrueForAll(x => x.IsFilled))
                {
                    await ShowLoading().ConfigureAwait(false);
                    PersonTable This = new PersonTable()
                    {
                        Absense = int.Parse(PAbsense.Text),
                        Activity = int.Parse(PActivity.Text),
                        Behaviour = int.Parse(PBehav.Text)
                    };
                    try
                    {
                        if (!FaceRecognized)
                        {
                            try
                            {
                                This.FaceId = App.Data.Table<PersonTable>().ToList().Count() == 0 ? 1 : (IsPresent ? Id : App.Data.Table<PersonTable>().ToList().OrderByDescending(x => x.FaceId).ToList().ElementAt(0).FaceId + 1);
                            }
                            catch (NullReferenceException)
                            {
                                This.FaceId = 1;
                            }
                            if (!await RecognizeFace(This.FaceId).ConfigureAwait(false))
                            {
                                This.FaceId = -1;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (!await RecognizeFace(This.FaceId, false))
                        {
                            This.FaceId = -1;
                        }
                        Debug.WriteLine(ex);
                        Debugger.Break();
                    }
                    string newFullPath = Path.Combine(Path.GetDirectoryName(DependencyService.Get<ISQLiteGetPath>().GetPath()), Path.GetFileName(Image.Path));
                    await Plugin.Permissions.CrossPermissions.Current.RequestPermissionsAsync(Plugin.Permissions.Abstractions.Permission.Storage);
                    if (Image.Path.Contains(DependencyService.Get<ISQLiteGetPath>().GetPath()) & IsPresent)
                    {
                        newFullPath = Image.Path;
                    }
                    if (await Plugin.Permissions.CrossPermissions.Current.CheckPermissionStatusAsync(Plugin.Permissions.Abstractions.Permission.Storage) == Plugin.Permissions.Abstractions.PermissionStatus.Granted & Image.Path != OldImagePath)
                    {
                        int count = 0;

                        string fileNameOnly = Path.GetFileNameWithoutExtension(Image.Path);
                        string extension = Path.GetExtension(Image.Path);
                        string path = Path.GetDirectoryName(DependencyService.Get<ISQLiteGetPath>().GetPath());
                        if (DependencyService.Get<IPlatform>().UWPResult() == "UWP")
                        {
                            while (DependencyService.Get<IFileManagement>().FileExists(newFullPath))
                            {
                                string tempFileName = string.Format("{0}_{1}", fileNameOnly, count++);
                                newFullPath = Path.Combine(path, tempFileName + extension);
                            }
                        }
                        else
                        {
                            while (File.Exists(newFullPath))
                            {
                                string tempFileName = string.Format("{0}_{1}", fileNameOnly, count++);
                                newFullPath = Path.Combine(path, tempFileName + extension);
                            }
                        }
                        if (DependencyService.Get<IPlatform>().UWPResult() == "UWP")
                        {
                            await DependencyService.Get<IFileManagement>().Copy(Image.Path, newFullPath);
                        }
                        else
                        {
                            File.Copy(Image.Path, newFullPath);
                        }
                    }
                    This.Image = newFullPath.Replace(App.RepHomeDir, ".");
                    //foreach (StackLayout item in PAFLL.Children)
                    //{
                    //    This.PData.Add(((Entry)item.Children.ElementAt(1)).Text);
                    //    This.PFData.Add(((Label)item.Children.ElementAt(0)).Text);
                    //}
                    if (IsPresent)
                    {
                        This.Id = Id;
                        List<string> F = This.PFData;
                        F.Remove("الصورة");
                        F.Remove("الاسم");
                        F.Remove("رقم الهاتف");
                        for (int i = 0; i < F.Count; i++)
                        {
                            F[i] = $"`{F[i]}`";
                        }
                        List<string> LData = GetFieldValues().Split(',').ToList();
                        LData.Insert(0, $"'{This.Image}'");
                        LData.Insert(0, This.Behaviour.ToString());
                        LData.Insert(0, This.Absense.ToString());
                        LData.Insert(0, This.Activity.ToString());
                        LData.Insert(0, This.FaceId.ToString());
                        List<string> cols = $"FaceId, Activity, Absense, Behaviour, Image, `الاسم`, `رقم الهاتف`, { Seperator(F)}".Split(',').ToList();
                        for (int i = 0; i < cols.Count; i++)
                        {
                            cols[i] = $"{cols[i]} = {LData[i]}";
                        }
                        string Q = $"update Persons set {Seperator(cols)} where Id = {This.Id};";
                        App.Data.Execute(Q);
                    }
                    else
                    {
                        List<string> v = This.PFData;
                        v.Remove("الصورة");
                        v.Remove("الاسم");
                        v.Remove("رقم الهاتف");
                        for (int i = 0; i < v.Count; i++)
                        {
                            v[i] = $"`{v[i]}`";
                        }
                        string Q = $"insert into Persons (FaceId, Activity, Absense, Behaviour, Image, `الاسم`, `رقم الهاتف`, {Seperator(v)}) values ({This.FaceId}, {This.Activity}, {This.Absense}, {This.Behaviour}, '{This.Image}', {GetFieldValues()})";
                        App.Data.Execute(Q);
                    }
                    UserDialogs.Instance.HideLoading();
                    UserDialogs.Instance.Toast("تم الحفظ بنجاح");
                    PCancel_Clicked(null, null);
                }
                else
                {
                    StackLayout Errors = new StackLayout();
                    Errors.Children.Add(new Label() { Text = "يجب ملئ الحقول التالية:", FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)) });
                    foreach (FieldDescription item in keys)
                    {
                        if (!item.IsFilled)
                        {
                            Errors.Children.Add(new Label() { Text = item.FieldName, FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)) });
                        }
                    }
                    Rg.Plugins.Popup.Pages.PopupPage Alert = new Rg.Plugins.Popup.Pages.PopupPage()
                    {
                        Padding = new Thickness(20 * Width / 360, 40 * Height / 560, 20 * Width / 360, 70 * Height / 560),
                        Content = new ScrollView() { Content = Errors, BackgroundColor = Color.White },
                    };
                    await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(Alert);
                }
            }
            catch (Exception)
            {
                Debugger.Break();
            }
        }

        private string Seperator(List<string> v)
        {
            StringBuilder result = new StringBuilder("");
            foreach (string item in v)
            {
                result.Append(item + ", ");
            }
            return result.Remove(result.Length - 2, 2).ToString();//Build and deploy
        }

        private async Task<bool> RecognizeFace(int v)
        {
            if (!await DependencyService.Get<IFaceOperations>().DetectAndWriteToFile(Image.Path, v))
            {
                if (await Acr.UserDialogs.UserDialogs.Instance.ConfirmAsync("هل تريد اختيار صورة أخرى؟", "لم يتم التعرف على أي وجه بالصورة", "نعم", "لا"))
                {
                    await SelectImage(null);
                    return await RecognizeFace(v);
                }
                return false;
            }
            return true;
        }

        private async Task<bool> RecognizeFace(int v, bool _)
        {
            if (await Acr.UserDialogs.UserDialogs.Instance.ConfirmAsync("هل تريد اختيار صورة أخرى؟", "لم يتم التعرف على أي وجه بالصورة", "نعم", "لا"))
            {
                await SelectImage(null);
                return await RecognizeFace(v);
            }
            return false;
        }

        private string GetFieldValues()
        {
            StringBuilder values = new StringBuilder("");
            foreach (View item in PAFLL.Children)
            {
                string FType = App.Data.Table<Fields>().ToList().ElementAt(PAFLL.Children.IndexOf(item) + 1).FieldType;
                if (FType == FieldType.Date)
                {
                    values.Append(((item as StackLayout).Children[1] as DatePicker).Date.Ticks);
                }
                else if (FType == FieldType.MultiChoice)
                {
                    values.Append(((item as StackLayout).Children[1] as Picker).SelectedIndex);
                }
                else if (FType == FieldType.Time)
                {
                    values.Append(((item as StackLayout).Children[1] as TimePicker).Time.Ticks);
                }
                else
                {
                    values.Append("'" + ((item as StackLayout).Children[1] as Entry).Text + "'");
                }
                values.Append(", ");
            }
            return values.Remove(values.Length - 2, 2).ToString();
        }

        private List<FieldDescription> Verify()
        {
            List<FieldDescription> Verification = new List<FieldDescription>();
            foreach (StackLayout item in PAFLL.Children)
            {
                try
                {
                    Verification.Add(new FieldDescription() { FieldName = ((Label)item.Children.ElementAt(0)).Text.Replace(":", ""), IsFilled = ((Entry)item.Children.ElementAt(1)).Text != "" & ((Entry)item.Children.ElementAt(1)).Text != null, FDescription = "الحقل فارغ!" });
                }
                catch (InvalidCastException)
                {
                    Verification.Add(new FieldDescription() { FieldName = ((Label)item.Children.ElementAt(0)).Text.Replace(":", ""), IsFilled = true, FDescription = "الحقل فارغ!" });
                }
            }

            Verification.Add(new FieldDescription() { FieldName = "درجة التفاعل", IsFilled = PActivity.Text != null & float.TryParse(PActivity.Text, out _), FDescription = "الحقل فارغ أو ليس رقم!" });
            Verification.Add(new FieldDescription() { FieldName = "درجة السلوك", IsFilled = PBehav.Text != null & float.TryParse(PActivity.Text, out _), FDescription = "الحقل فارغ أو ليس رقم!" });
            Verification.Add(new FieldDescription() { FieldName = "درجة الغياب والحضور", IsFilled = PAbsense.Text != null & float.TryParse(PActivity.Text, out _), FDescription = "الحقل فارغ أو ليس رقم!" });
            return Verification;
        }
        private void Next(object sender, EventArgs e)
        {
            Entry entry = (Entry)sender;
            StackLayout SenderParent = (StackLayout)entry.Parent;
            StackLayout SenderParentParent = (StackLayout)SenderParent.Parent;
            if (SenderParentParent.Children.IndexOf(SenderParent) + 1 == SenderParentParent.Children.Count)
            {
                PActivity.Focus();
            }
            else
            {
                ((Entry)((StackLayout)SenderParentParent.Children.ElementAt(SenderParentParent.Children.IndexOf(SenderParent) + 1)).Children.ElementAt(1)).Focus();
            }
        }
        private void NextD(object sender, EventArgs e)
        {
            if (sender == PActivity)
            {
                PBehav.Focus();
            }
            else if (sender == PBehav)
            {
                PAbsense.Focus();
            }
            else
            {
                PAbsense.Unfocus();
            }
        }
        public void PCancel_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync(true);
        }
        //        public void PAFB_Clicked(object sender, EventArgs e)
        //        {
        //            Label l = new Label()
        //            {
        //                Text = "اسم الحقل:",
        //                HorizontalOptions = LayoutOptions.Start,
        //                VerticalTextAlignment = TextAlignment.Center,
        //                VerticalOptions = LayoutOptions.Fill,
        //                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label))
        //            };
        //
        //            StackLayout StL = new StackLayout()
        //            {
        //                Orientation = StackOrientation.Horizontal,
        //                HorizontalOptions = LayoutOptions.Fill,
        //                VerticalOptions = LayoutOptions.Start,
        //                Children =
        //                {
        //                    l,
        //                    new Entry() { Text = "القيمة",
        //                        HorizontalTextAlignment = TextAlignment.Center,
        //                        HorizontalOptions = LayoutOptions.FillAndExpand,
        //                        VerticalOptions = LayoutOptions.Fill,
        //                        Margin = 5,
        //                        FontSize = Device.GetNamedSize (NamedSize.Medium, typeof(Label)),
        //                        ReturnType = ReturnType.Next
        //                    }
        //                }
        //            };
        //            l.GestureRecognizers.Add(Label_Clicked);
        //            (StL.Children.ElementAt(1) as Entry).Completed += Next;
        //            PAFLL.Children.Add(StL);
        //#pragma warning disable CS0612 // Type or member is obsolete
        //            Fields(l, null);
        //            AddFields.Append(l.Text + ",");
        //#pragma warning restore CS0612 // Type or member is obsolete
        //        }

        //private void Fields(object sender, EventArgs e)
        //{
        //    StackLayout Mstack = new StackLayout()
        //    {
        //        BackgroundColor = Color.White,
        //        HorizontalOptions = LayoutOptions.Fill,
        //        VerticalOptions = LayoutOptions.Fill,
        //        Children =
        //            {
        //                new Label()
        //                {
        //                    Text = "تسمية الحقل:",
        //                    HorizontalOptions = LayoutOptions.Fill,
        //                    VerticalOptions = LayoutOptions.Start,
        //                    FontSize = Device.GetNamedSize (NamedSize.Medium, typeof(Label))
        //                },
        //                new Entry()
        //                {
        //                    Text = ((Label)((StackLayout)((View)sender).Parent).Children.ElementAt(0)).Text,
        //                    HorizontalOptions = LayoutOptions.Fill,
        //                    VerticalOptions = LayoutOptions.Start,
        //                    FontSize = Device.GetNamedSize (NamedSize.Medium, typeof(Label)),
        //                    ReturnType = ReturnType.Done,
        //                    CursorPosition = 0,
        //                    SelectionLength = ((Label)((StackLayout)((View)sender).Parent).Children.ElementAt(0)).Text.Length
        //                }
        //            }
        //    };
        //    Button Delete = new Button()
        //    {
        //        Text = "حذف الحقل",
        //        HorizontalOptions = LayoutOptions.Fill,
        //        VerticalOptions = LayoutOptions.Start,
        //        TextColor = Color.Accent,
        //        FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
        //        BackgroundColor = Color.Transparent
        //    };
        //    Button OK = new Button()
        //    {
        //        Text = "تم",
        //        HorizontalOptions = LayoutOptions.Fill,
        //        VerticalOptions = LayoutOptions.Start,
        //        TextColor = Color.Accent,
        //        FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
        //        BackgroundColor = Color.Transparent
        //    };
        //    Rg.Plugins.Popup.Pages.PopupPage page = new Rg.Plugins.Popup.Pages.PopupPage()
        //    {
        //        Content = new StackLayout
        //        {
        //            Children =
        //            {
        //                Mstack, new StackLayout()
        //                {
        //                    BackgroundColor = Color.White,
        //                    Orientation = StackOrientation.Horizontal,
        //                    Children =
        //                    {
        //                        OK, Delete
        //                    }
        //                }
        //            }
        //        },
        //        Padding = new Thickness(20 * Width / 360, 195 * Height / 560, 20 * Width / 360, 195 * Height / 560)
        //    };
        //    OK.Clicked += delegate
        //    {
        //        AddFields.Replace(((Label)((StackLayout)((View)sender).Parent).Children.ElementAt(0)).Text, ((Entry)Mstack.Children.ElementAt(1)).Text);
        //        ((Label)((StackLayout)((View)sender).Parent).Children.ElementAt(0)).Text = ((Entry)Mstack.Children.ElementAt(1)).Text;
        //        Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAsync(true);
        //    };
        //    ((Entry)Mstack.Children.ElementAt(1)).Completed += delegate
        //    {
        //        AddFields.Replace(((Label)((StackLayout)((View)sender).Parent).Children.ElementAt(0)).Text, ((Entry)Mstack.Children.ElementAt(1)).Text);
        //        ((Label)((StackLayout)((View)sender).Parent).Children.ElementAt(0)).Text = ((Entry)Mstack.Children.ElementAt(1)).Text;
        //        Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAsync(true);
        //    };
        //    Delete.Clicked += delegate
        //    {
        //        AddFields.Replace(((Label)((StackLayout)((View)sender).Parent).Children.ElementAt(0)).Text + ",", "");
        //        ((StackLayout)((View)sender).Parent.Parent).Children.Remove(((StackLayout)((View)sender).Parent));
        //        Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAsync(true);
        //    };
        //    ((Entry)Mstack.Children.ElementAt(1)).Keyboard = Keyboard.Create(KeyboardFlags.Spellcheck | KeyboardFlags.Suggestions);
        //    Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(page, true);
        //}
        private void Person_SizeChanged(object sender, EventArgs e)
        {
            if (Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopupStack.Count != 0)
            {
                Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopupStack[0].Padding = new Thickness(20 * Width / 360, 40 * Height / 560, 20 * Width / 360, 70 * Height / 560);
            }
        }
    }
}