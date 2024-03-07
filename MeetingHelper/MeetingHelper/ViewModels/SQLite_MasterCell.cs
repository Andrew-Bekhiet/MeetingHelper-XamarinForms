using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MeetingHelper.ViewModels
{
    public class SQLite_MasterCell : ViewCell
    {
        public SQLite_MasterCell()
        {
            Label type = new Label() { FontSize = Device.GetNamedSize(NamedSize.Title, typeof(Label)), TextColor = Color.Black, HorizontalOptions = LayoutOptions.StartAndExpand };
            Label name = new Label() { FontSize = Device.GetNamedSize(NamedSize.Title, typeof(Label)), TextColor = Color.Black, HorizontalOptions = LayoutOptions.StartAndExpand };
            Label rootpage = new Label() { FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)), TextColor = Color.Black, HorizontalOptions = LayoutOptions.StartAndExpand };
            Label sql = new Label() { FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)), TextColor = Color.Black, HorizontalOptions = LayoutOptions.StartAndExpand };

            type.SetBinding(Label.TextProperty, "type");
            name.SetBinding(Label.TextProperty, "name");
            rootpage.SetBinding(Label.TextProperty, "rootpage");
            sql.SetBinding(Label.TextProperty, "sql");

            View = new StackLayout()
            {
                Children =
                {
                    type,
                    name,
                    rootpage,
                    sql
                }
            };
        }
    }
}
