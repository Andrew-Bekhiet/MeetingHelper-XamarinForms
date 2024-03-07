using MeetingHelper.Views;
using System.Collections.Generic;
using Xamarin.Forms;

namespace MeetingHelper.Models
{
    public class ActivityCell : ViewCell
    {
        public ActivityCell()
        {
            Image im = new Image() { HorizontalOptions = LayoutOptions.Start, WidthRequest = 100, HeightRequest = 100 };
            im.SetBinding(Image.SourceProperty, "Image");
            Label name = new Label() { FontSize = Device.GetNamedSize(NamedSize.Title, typeof(Label)), TextColor = Color.Black, HorizontalOptions = LayoutOptions.StartAndExpand };
            name.SetBinding(Label.TextProperty, "Name");
            Picker picker = new Picker() { FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Picker)), HorizontalOptions = LayoutOptions.Start };
            picker.SetBinding(Picker.SelectedIndexProperty, "_add", converter: new AddConvert());
            picker.ItemsSource = new List<string>() { "+", "-" };
            Label entry = new Label() { HorizontalTextAlignment = TextAlignment.Center, FontSize = picker.FontSize, HorizontalOptions = LayoutOptions.Fill };
            entry.SetBinding(Label.TextProperty, "Degrees", converter: new DegreeConverter());
            Label entry2 = new Label() { HorizontalTextAlignment = TextAlignment.Center, FontSize = picker.FontSize, HorizontalOptions = LayoutOptions.Fill };
            entry2.SetBinding(Label.TextProperty, "Description");
            View = new StackLayout()
            {
                IsEnabled = false,
                VerticalOptions = LayoutOptions.Fill,
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.Fill,
                Children =
                {
                    im,
                    new StackLayout()
                    {
                        HorizontalOptions = LayoutOptions.Fill,
                        Children =
                        {
                            name,
                            new StackLayout()
                            {
                                Orientation = StackOrientation.Horizontal,
                                VerticalOptions = LayoutOptions.Start,
                                HorizontalOptions = LayoutOptions.Fill,
                                Children =
                                {
                                    picker,
                                    entry
                                }
                            },
                            entry2
                        }
                    }
                }
            };
        }
    }
}