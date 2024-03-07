using IntelliAbb.Xamarin.Controls;
using Xamarin.Forms;

namespace MeetingHelper.Models
{
    public class AbsenceCell : ViewCell
    {
        public AbsenceCell()
        {
            Image im = new Image() { HorizontalOptions = LayoutOptions.Start, WidthRequest = 100, HeightRequest = 100 };
            im.SetBinding(Image.SourceProperty, "Image");
            Checkbox nameC = new Checkbox() { HorizontalOptions = LayoutOptions.Start, FillColor = Color.Accent, OutlineColor = Color.Accent };
            nameC.SetBinding(Checkbox.IsCheckedProperty, "NameCheck");
            Label name = new Label() { FontSize = Device.GetNamedSize(NamedSize.Title, typeof(Label)), HorizontalOptions = LayoutOptions.Fill, HorizontalTextAlignment = TextAlignment.Start, TextColor = Color.Black };
            name.SetBinding(Label.TextProperty, "Name");
            TimePicker TimeP = new TimePicker() { FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(TimePicker)), HorizontalOptions = LayoutOptions.Fill };
            TimeP.SetBinding(TimePicker.TimeProperty, "Time");
            Entry bonus = new Entry() { HorizontalTextAlignment = TextAlignment.Center, FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Entry)), HorizontalOptions = LayoutOptions.Fill };
            bonus.SetBinding(Entry.TextProperty, "Bonus");
            View = new StackLayout()
            {
                IsEnabled = false,
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Children =
                {
                    im,
                    new StackLayout()
                    {
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        Children =
                        {
                            new StackLayout()
                            {
                                HorizontalOptions = LayoutOptions.FillAndExpand,
                                Orientation = StackOrientation.Horizontal,
                                Children =
                                {
                                    nameC, name
                                }
                            },
                            TimeP, bonus
                        }
                    }
                }
            };
        }
    }
}