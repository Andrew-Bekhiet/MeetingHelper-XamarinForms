using System.Collections.Generic;
using Xamarin.Forms;

namespace MeetingHelper.ViewModels
{
    public class FieldControl : StackLayout
    {
        public bool Disabled { get; set; } = false;
        public string FieldName { get; set; } = "";
        public string FieldType { get; set; } = "";
        public bool CHoicesChng { get; set; } = false;
        public List<string> FieldChoices { get; set; } = null;
        private readonly Button ShowChoices = new Button() { Text = "الاختيارات", VerticalOptions = LayoutOptions.Fill, HorizontalOptions = LayoutOptions.Start };
        public FieldControl()
        {
            base.Orientation = StackOrientation.Horizontal;
        }
        public FieldControl(string fieldType, string fieldName, List<string> fieldChoices = null, bool Enabled = true)
        {
            FieldName = fieldName;
            FieldType = fieldType;
            FieldChoices = fieldChoices;
            Disabled = !Enabled;

            base.Orientation = StackOrientation.Horizontal;

            //Field Name:
            var entry = new Entry() { IsEnabled = Enabled, Text = fieldName, VerticalOptions = LayoutOptions.Start, HorizontalOptions = LayoutOptions.Start };
            base.Children.Add(entry);
            entry.TextChanged += delegate
            {
                FieldName = entry.Text;
            };

            //Field Type:
            List<string> Types = new List<string>(6)
            {
                "نص",
                "رقم هاتف",
                "رقم",
                "وقت",
                "تاريخ",
                "اختياري"
            };
            Picker P = new Picker() { IsEnabled = Enabled, ItemsSource = Types, VerticalOptions = LayoutOptions.Fill, HorizontalOptions = LayoutOptions.Fill };
            P.SelectedIndexChanged += delegate
            {
                ShowChoices.IsVisible = P.SelectedIndex == 5;
                FieldType = FromUserValue(P.ItemsSource[P.SelectedIndex].ToString());
            };
            P.SelectedIndex = Types.IndexOf(FromType(FieldType));
            base.Children.Add(P);
            base.Children.Add(ShowChoices);
            ShowChoices.Clicked += delegate
            {
                Rg.Plugins.Popup.Pages.PopupPage popupPage = new Rg.Plugins.Popup.Pages.PopupPage();
                StackLayout stack = new StackLayout
                {
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Fill
                };
                popupPage.BackgroundColor = Color.White;

                if (FieldChoices != null)
                {
                    foreach (string item in FieldChoices)
                    {
                        Entry tmp = new Entry() { VerticalOptions = LayoutOptions.Start, HorizontalOptions = LayoutOptions.Fill, Text = item };
                        tmp.TextChanged += delegate
                        {
                            CHoicesChng = true;
                        };
                        stack.Children.Add(tmp);
                    }
                }
                Button btnAdd = new Button() { VerticalOptions = LayoutOptions.Start, HorizontalOptions = LayoutOptions.Fill, Text = "إضافة اختيار" };
                stack.Children.Add(btnAdd);
                btnAdd.Clicked += delegate
                {
                    CHoicesChng = true;
                    stack.Children.Insert(stack.Children.IndexOf(btnAdd), new Entry() { VerticalOptions = LayoutOptions.Start, HorizontalOptions = LayoutOptions.Fill });
                };
                popupPage.Content = new ScrollView() { Content = stack };
                popupPage.Disappearing += delegate
                {
                    FieldChoices = new List<string>();
                    foreach (View item in stack.Children)
                    {
                        if (stack.Children.IndexOf(item) + 1 == stack.Children.Count)
                        {
                            break;
                        }
                        FieldChoices.Add((item as Entry).Text);
                    }
                };
                Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(popupPage);
            };
        }

        private string FromType(string type)
        {
            switch (type)
            {
                case Tables.FieldType.Date:
                    return "تاريخ";
                case Tables.FieldType.MultiChoice:
                    return "اختياري";
                case Tables.FieldType.Integer:
                    return "رقم";
                case Tables.FieldType.Srting:
                    return "نص";
                case Tables.FieldType.TelephoneNumber:
                    return "رقم هاتف";
                case Tables.FieldType.Time:
                    return "وقت";
                default:
                    return "";
            }
        }

        private string FromUserValue(string value)
        {
            switch (value)
            {
                case "تاريخ":
                    return Tables.FieldType.Date;
                case "اختياري":
                    return Tables.FieldType.MultiChoice;
                case "رقم":
                    return Tables.FieldType.Integer;
                case "نص":
                    return Tables.FieldType.Srting;
                case "رقم هاتف":
                    return Tables.FieldType.TelephoneNumber;
                case "وقت":
                    return Tables.FieldType.Time;
                default:
                    return "";
            }
        }
    }
}
