using Xamarin.Forms;

namespace MeetingHelper.Models
{
    public class PersonCell : ViewCell
    {
        public PersonCell()
        {
            Image im = new Image() { HorizontalOptions = LayoutOptions.Start, WidthRequest = 80 };
            im.SetBinding(Image.SourceProperty, "GetImage");
            Label name = new Label() { FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)), HorizontalOptions = LayoutOptions.StartAndExpand };
            name.SetBinding(Label.TextProperty, "GetName");
            Label phone = new Label() { FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)), HorizontalOptions = LayoutOptions.StartAndExpand };
            phone.SetBinding(Label.TextProperty, "GetPhoneNumber");
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
                            name, phone
                        }
                    }
                }
            };
        }
    }
}