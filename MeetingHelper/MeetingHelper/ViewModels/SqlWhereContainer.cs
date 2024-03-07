using MeetingHelper.Tables;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MeetingHelper.Models
{
    public class SqlWhereContainer : StackLayout
    {
        private const string Classes = "xmlns =\"http://xamarin.com/schemas/2014/forms\" xmlns:x=\"http://schemas.microsoft.com/winfx/2009/xaml\" xmlns:local=\"clr-namespace:MeetingHelper.Views\" x:Class=\"MeetingHelper.Views.SQLiteQ\"";
        private Picker Order { get; set; }
        private Picker OrderMode { get; set; }
        public int Type => (Children[0] as SqlWhereCondition).Type;
        public string GetType2 => (Children[0] as SqlWhereCondition).GetType2();
        public SqlWhereContainer()
        {
            //Label label = new Label().LoadFromXaml("<Label " + Classes + " FlexLayout.Basis=\"32%\" FontSize=\"Small\">ترتيب حسب:</Label>");

            //Order = new Picker().LoadFromXaml("<Picker " + Classes + " FlexLayout.Basis=\"32%\" />");
            //Order.ItemsSource = new List<string>() { "درجة الغياب والحضور", "درجة السلوك", "درجة التفاعل" };
            //Order.SelectedIndex = 0;

            //OrderMode = new Picker().LoadFromXaml("<Picker " + Classes + " FlexLayout.Basis=\"32%\" />");
            //OrderMode.ItemsSource = new List<string>() { "تصاعدي", "تنازلي" };
            //OrderMode.SelectedIndex = 0;

            //Children.Add(new SqlWhereCondition(0));
            //Children.Add(new FlexLayout() { Direction = FlexDirection.RowReverse, AlignItems = FlexAlignItems.Center, JustifyContent = FlexJustify.SpaceEvenly, Children = { label, Order, OrderMode } });
            Construct(0);
        }
        public SqlWhereContainer(int type = 0)
        {
            Construct(type);
        }

        private void Construct(int type)
        {
            Label label = new Label().LoadFromXaml("<Label " + Classes + " FlexLayout.Basis=\"32%\" FontSize=\"Small\">ترتيب حسب:</Label>");

            Order = new Picker().LoadFromXaml("<Picker " + Classes + " FlexLayout.Basis=\"32%\" />");
            if (type == 0)
            {
                List<string> tmp = App.Data.Table<Fields>().ToList().Select(x => x.FieldName).Where(x => x != "الصورة").ToList();
                tmp.Add("درجة التفاعل");
                tmp.Add("درجة الغياب والحضور");
                tmp.Add("درجة السلوك");
                Order.ItemsSource = tmp;
            }
            else
            {
                Order.ItemsSource = new List<string>()
                {
                    "الحضور",
                    "وقت المجئ",
                    "درجة المجئ",
                    "درجة السلوك",
                    "درجة التفاعل"
                };
            }
            Order.SelectedIndex = 0;

            OrderMode = new Picker().LoadFromXaml("<Picker " + Classes + " FlexLayout.Basis=\"32%\" />");
            OrderMode.ItemsSource = new List<string>() { "تصاعدي", "تنازلي" };
            OrderMode.SelectedIndex = 0;

            Children.Add(new SqlWhereCondition(type));
            Children.Add(new FlexLayout() { Direction = FlexDirection.RowReverse, AlignItems = FlexAlignItems.Center, JustifyContent = FlexJustify.SpaceEvenly, Children = { label, Order, OrderMode } });
        }

        public void ChangeType(int NewType)
        {
            Children.Clear();
            Construct(NewType);
        }
        public string GetWhereStatment()
        {
            string orderBy;
            if (Type == 0)
            {
                orderBy = $"`{Order.SelectedItem.ToString().Replace("درجة التفاعل", "Activity").Replace("درجة الغياب والحضور", "Absense").Replace("درجة التفاعل", "Behaviour")}`";
            }
            else
            {
                switch (Order.SelectedIndex)
                {
                    case 0:
                        orderBy = "`الاسم`";
                        break;
                    case 1:
                        orderBy = "Time";
                        break;
                    case 2:
                        orderBy = "Bonus";
                        break;
                    case 3 | 4:
                        orderBy = "Degrees";
                        break;
                    default:
                        orderBy = "True";
                        break;
                }
            }
            orderBy = " ORDER BY " + orderBy;
            return (Children[0] as SqlWhereCondition).GetCondition() + orderBy + " " + (OrderMode.SelectedIndex == 0 ? "ASC" : "DESC") + ";";
        }
    }
}