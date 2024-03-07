using MeetingHelper.Tables;
using System;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MeetingHelper.Models
{
    public class SqlWhereCondition : StackLayout
    {
        private const string Classes = "xmlns =\"http://xamarin.com/schemas/2014/forms\"";
        public Picker Field { get; set; } = new Picker().LoadFromXaml("<Picker " + Classes + " SelectedIndex=\"0\" Title =\"اسم الحقل\" FlexLayout.Basis=\"32%\"/>");
        public Picker Operator { get; set; } = new Picker().LoadFromXaml("<Picker " + Classes + " SelectedIndex=\"0\" FlexLayout.Basis=\"32%\"/>");
        public View Value { get; set; } = new Entry().LoadFromXaml("<Entry " + Classes + " Placeholder=\"القيمة\" FlexLayout.Basis=\"32%\" Keyboard=\"Text\"/>");
        public Picker LogicOperator { get; set; } = new Picker();
        public FlexLayout SL { get; set; } = new FlexLayout() { Direction = FlexDirection.RowReverse, AlignItems = FlexAlignItems.Center, JustifyContent = FlexJustify.SpaceEvenly, HorizontalOptions = LayoutOptions.Fill };
        public int Type { get; private set; }
        public SqlWhereCondition(int type)
        {
            Type = type;
            if (type == 0)
            {
                List<string> Fields;
                try
                {
                    Fields = App.Data.Table<Fields>().ToList().Select(F => F.FieldName).ToList();
                    Fields.Remove("الصورة");
                    Fields.Add("درجة التفاعل");
                    Fields.Add("درجة السلوك");
                    Fields.Add("درجة الغياب والحضور");
                }
                catch (NullReferenceException)
                {
                    Fields = new List<string>();
                }
                
                Field.ItemsSource = Fields;
                Field.SelectedIndexChanged += Field_SelectedIndexChanged;

                Operator.SelectedIndexChanged += Picker_SelectedIndexChanged;
                Operator.ItemsSource = new List<string>() { "يساوي", "يحتوي على", "طوله أكبر من", "طوله أصغر من" };

                LogicOperator.ItemsSource = new List<string>() { "", "أو أن", "وأيضًا أن يكون" };
                LogicOperator.SelectedIndexChanged += LogicOperator_SelectedIndexChanged;

                SL.Children.Add(Field);
                SL.Children.Add(Operator);
                SL.Children.Add(Value);

                Children.Add(SL);
                Children.Add(LogicOperator);

                Field.SelectedIndex = 0;
                Operator.SelectedIndex = 0;
                LogicOperator.SelectedIndex = 0;
            }
            else //if (type == 1)
            {
                List<string> Fields = new List<string>
                {
                    "الحضور",
                    "وقت المجئ",
                    "درجة المجئ",
                    "درجة السلوك",
                    "سبب درجة السلوك",
                    "درجة التفاعل",
                    "شرح درجة التفاعل"
                };
                Field.ItemsSource = Fields;
                Field.SelectedIndexChanged += Field_SelectedIndexChanged;

                Operator.SelectedIndexChanged += Picker_SelectedIndexChanged;
                Operator.ItemsSource = new List<string>();

                LogicOperator.ItemsSource = new List<string>() { "", "أو أن", "وأيضًا أن يكون" };
                LogicOperator.SelectedIndexChanged += LogicOperator_SelectedIndexChanged;

                SL.Children.Add(Field);
                SL.Children.Add(Operator);
                SL.Children.Add(Value);

                Children.Add(SL);
                Children.Add(LogicOperator);

                Field.SelectedIndex = 0;
                Operator.SelectedIndex = 0;
                LogicOperator.SelectedIndex = 0;
            }
        }

        public string GetType2()
        {
            if (Type == 1)
            {
                if (Field.SelectedIndex < 3)
                {
                    return "_A";
                }
                else if (Field.SelectedIndex == 3 | Field.SelectedIndex == 4)
                {
                    return "_B";
                }
                return "_AC";
            }
            return "";
        }

        private void LogicOperator_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (LogicOperator.SelectedIndex == 0 & base.Children.Count > 2)
            {
                base.Children.RemoveAt(2);
            }
            else if (LogicOperator.SelectedIndex == 1 | LogicOperator.SelectedIndex == 2 & base.Children.Count == 2)
            {
                if (base.Children.Count > 2)
                {
                    base.Children.RemoveAt(2);
                }
                base.Children.Add(new SqlWhereCondition(Type));
            }
        }

        private void Field_SelectedIndexChanged(object sender, EventArgs e)
        {
            Picker_SelectedIndexChanged(sender, e);
            if (Type == 0)
            {
                if (Field.SelectedItem.ToString().Contains("درجة"))
                {
                    Value = new Entry().LoadFromXaml("<Entry " + Classes + " Placeholder=\"القيمة\" FlexLayout.Basis=\"32%\" Keyboard=\"Text\"/>");
                    SL.Children.RemoveAt(2);
                    SL.Children.Add(Value);
                    Operator.ItemsSource = new List<string>() { "=", ">", "<" };
                    (Value as Entry).Keyboard = Keyboard.Numeric;
                    (Value as Entry).Text = "0";
                }
                else
                {
                    switch (App.Data.Table<Fields>().ToList().Where(x => x.FieldName == Field.SelectedItem.ToString()).ToList()[0].FieldType)
                    {
                        case FieldType.Srting:
                            Value = new Entry().LoadFromXaml("<Entry " + Classes + " Placeholder=\"القيمة\" FlexLayout.Basis=\"32%\" Keyboard=\"Text\"/>");
                            SL.Children.RemoveAt(2);
                            SL.Children.Add(Value);
                            Operator.ItemsSource = new List<string>() { "يساوي =", "طوله أكبر من >", "طوله أصغر من <", "يحتوي على" };
                            (Value as Entry).Keyboard = Keyboard.Text;
                            (Value as Entry).Text = "";
                            break;
                        case FieldType.TelephoneNumber:
                            Value = new Entry().LoadFromXaml("<Entry " + Classes + " Placeholder=\"القيمة\" FlexLayout.Basis=\"32%\" Keyboard=\"Text\"/>");
                            SL.Children.RemoveAt(2);
                            SL.Children.Add(Value);
                            Operator.ItemsSource = new List<string>() { "يساوي =", "طوله أكبر من >", "طوله أصغر من <", "يحتوي على" };
                            (Value as Entry).Keyboard = Keyboard.Text;
                            (Value as Entry).Text = "";
                            break;
                        case FieldType.MultiChoice:
                            Value = new Picker().LoadFromXaml("<Picker " + Classes + " FlexLayout.Basis=\"32%\"/>");
                            SL.Children.RemoveAt(2);
                            SL.Children.Add(Value);
                            Operator.ItemsSource = new List<string>() { "=" };
                            Operator.SelectedIndex = 0;
                            (Value as Picker).ItemsSource = App.Data.Table<Fields>().ToList().Where(x => x.FieldName == Field.SelectedItem.ToString()).ToList()[0]._FieldChoices;
                            (Value as Picker).SelectedIndex = 0;
                            break;
                        case FieldType.Date:
                            Value = new DatePicker().LoadFromXaml("<DatePicker " + Classes + " FlexLayout.Basis=\"32%\"/>");
                            SL.Children.RemoveAt(2);
                            SL.Children.Add(Value);
                            Operator.ItemsSource = new List<string>() { "=", ">", "<" };
                            Operator.SelectedIndex = 0;
                            break;
                        case FieldType.Time:
                            Value = new TimePicker().LoadFromXaml("<TimePicker " + Classes + " FlexLayout.Basis=\"32%\"/>");
                            SL.Children.RemoveAt(2);
                            SL.Children.Add(Value);
                            Operator.ItemsSource = new List<string>() { "=", ">", "<" };
                            Operator.SelectedIndex = 0;
                            break;
                        default:
                            Value = new Entry().LoadFromXaml("<Entry " + Classes + " Placeholder=\"القيمة\" FlexLayout.Basis=\"32%\" Keyboard=\"Text\"/>");
                            SL.Children.RemoveAt(2);
                            SL.Children.Add(Value);
                            Operator.ItemsSource = new List<string>() { "=", ">", "<" };
                            (Value as Entry).Keyboard = Keyboard.Numeric;
                            (Value as Entry).Text = "0";
                            break;
                    }
                }
            }
            else
            {
                Value = new Entry().LoadFromXaml("<Entry " + Classes + " Placeholder=\"القيمة\" FlexLayout.Basis=\"32%\" Keyboard=\"Text\"/>");
                SL.Children.RemoveAt(2);
                SL.Children.Add(Value);
                if (Field.SelectedItem.ToString() == "الحضور")
                {
                    SL.Children.RemoveAt(2);
                    SL.Children.Add(new Picker().LoadFromXaml("<Picker " + Classes + " Title=\"القيمة\" FlexLayout.Basis=\"32%\"/>"));
                    (SL.Children[2] as Picker).ItemsSource = new List<string>() { "لم يحضر", "حضر" };
                    (SL.Children[2] as Picker).SelectedIndexChanged += Picker_SelectedIndexChanged;
                    (SL.Children[1] as Picker).ItemsSource = new List<string>() { "=" };
                    (SL.Children[1] as Picker).SelectedIndex = 0;
                    (SL.Children[2] as Picker).SelectedIndex = 0;
                }
                else if (Field.SelectedItem.ToString() == "وقت المجئ")
                {
                    SL.Children.RemoveAt(2);
                    SL.Children.Add(new TimePicker().LoadFromXaml("<TimePicker " + Classes + " FlexLayout.Basis=\"32%\"/>"));
                    (SL.Children[1] as Picker).ItemsSource = new List<string>() { "بعد", "في تمام الساعة", "قبل" };
                    (SL.Children[1] as Picker).SelectedIndexChanged += Picker_SelectedIndexChanged;
                    (SL.Children[1] as Picker).SelectedIndex = 0;
                }
                else if (Field.SelectedItem.ToString() == "درجة المجئ")
                {
                    SL.Children.RemoveAt(2);
                    (Value as Entry).Keyboard = Keyboard.Numeric;
                    SL.Children.Add(Value);
                    (SL.Children[1] as Picker).ItemsSource = new List<string>() { "=", ">", "<" };
                    (SL.Children[1] as Picker).SelectedIndexChanged += Picker_SelectedIndexChanged;
                    (SL.Children[1] as Picker).SelectedIndex = 0;
                }
                else if (Field.SelectedItem.ToString() == "درجة السلوك")
                {
                    SL.Children.RemoveAt(2);
                    (Value as Entry).Keyboard = Keyboard.Numeric;
                    SL.Children.Add(Value);
                    (SL.Children[1] as Picker).ItemsSource = new List<string>() { "=", ">", "<" };
                    (SL.Children[1] as Picker).SelectedIndexChanged += Picker_SelectedIndexChanged;
                    (SL.Children[1] as Picker).SelectedIndex = 0;
                }
                else if (Field.SelectedItem.ToString() == "سبب درجة السلوك")
                {
                    SL.Children.RemoveAt(2);
                    (Value as Entry).Keyboard = Keyboard.Text;
                    SL.Children.Add(Value);
                    (SL.Children[1] as Picker).ItemsSource = new List<string>() { "يساوي", "يحتوي على", "طوله أكبر من", "طوله أصغر من" };
                    (SL.Children[1] as Picker).SelectedIndexChanged += Picker_SelectedIndexChanged;
                    (SL.Children[1] as Picker).SelectedIndex = 0;
                }
                else if (Field.SelectedItem.ToString() == "درجة التفاعل")
                {
                    SL.Children.RemoveAt(2);
                    (Value as Entry).Keyboard = Keyboard.Numeric;
                    SL.Children.Add(Value);
                    (SL.Children[1] as Picker).ItemsSource = new List<string>() { "=", ">", "<" };
                    (SL.Children[1] as Picker).SelectedIndexChanged += Picker_SelectedIndexChanged;
                    (SL.Children[1] as Picker).SelectedIndex = 0;
                }
                else if (Field.SelectedItem.ToString() == "شرح درجة التفاعل")
                {
                    SL.Children.RemoveAt(2);
                    (Value as Entry).Keyboard = Keyboard.Text;
                    SL.Children.Add(Value);
                    (SL.Children[1] as Picker).ItemsSource = new List<string>() { "يساوي", "يحتوي على", "طوله أكبر من", "طوله أصغر من" };
                    (SL.Children[1] as Picker).SelectedIndexChanged += Picker_SelectedIndexChanged;
                    (SL.Children[1] as Picker).SelectedIndex = 0;
                }
            }
        }

        private void Picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((sender as Picker).SelectedItem != null)
            {
                //(sender as Picker).WidthRequest = App.CalculateStringWidth((sender as Picker).SelectedItem.ToString(), new Font());
            }
        }

        public string GetCondition()
        {
            string Result = "";
            if (Type == 0)
            {
                switch ((SL.Children[0] as Picker).SelectedItem.ToString())
                {
                    case "درجة التفاعل":
                        Result = " Activity " + Invert((SL.Children[1] as Picker).SelectedItem.ToString()) + " " + int.Parse((SL.Children[2] as Entry).Text);
                        break;
                    case "درجة السلوك":
                        Result = " Behaviour " + Invert((SL.Children[1] as Picker).SelectedItem.ToString()) + " " + int.Parse((SL.Children[2] as Entry).Text);
                        break;
                    case "درجة الغياب والحضور":
                        Result = " Absense " + Invert((SL.Children[1] as Picker).SelectedItem.ToString()) + " " + int.Parse((SL.Children[2] as Entry).Text);
                        break;
                    default:
                        //Backup: App.Data.Execute("Create View IF Not exists PersonsQuery As Select * From(With split(PFData, PFDataStr, PID, PData, PDataStr) AS (\nSELECT '', PersonFieldData|| ';', Id, '', PersonData|| ';' From Persons\nUNION ALL SELECT\nsubstr(PFDataStr, 0, instr(PFDataStr, ';')),\nsubstr(PFDataStr, instr(PFDataStr, ';') + 1),\nPID,\nsubstr(PDataStr, 0, instr(PDataStr, ';')),\nsubstr(PDataStr, instr(PDataStr, ';') + 1)\nFROM split WHERE PFDataStr!='' and PDataStr!='' )\nSELECT PID, PFData, PData FROM split where PData!='' And PFData!='');");
                        ////This query is for producing:
                        ////Create View IF Not exists PersonsQuery As Select * From(With split(PFData, PFDataStr, PID, PData, PDataStr) AS (
                        ////SELECT '', PersonFieldData|| ';', Id, '', PersonData|| ';' From Persons
                        ////UNION ALL SELECT
                        ////substr(PFDataStr, 0, instr(PFDataStr, ';')),
                        ////substr(PFDataStr, instr(PFDataStr, ';') + 1),
                        ////PID,
                        ////substr(PDataStr, 0, instr(PDataStr, ';')),
                        ////substr(PDataStr, instr(PDataStr, ';') + 1)
                        ////FROM split WHERE PFDataStr!='' and PDataStr!='' )
                        ////SELECT PID, PFData, PData FROM split where PData!='' And PFData!='');
                        ////
                        ////Ex.: Where (SL.Children[0) as Picker).SelectedItem.ToString() = (SL.Children[2) as Entry).Text
                        switch (App.Data.Table<Fields>().ToList().Where(x => x.FieldName == Field.SelectedItem.ToString()).ToList()[0].FieldType)
                        {
                            case FieldType.Srting:
                                if ((SL.Children[1] as Picker).SelectedIndex == 0)
                                {
                                    Result = $" `{(SL.Children[0] as Picker).SelectedItem.ToString()}` = '{(SL.Children[2] as Entry).Text}'";
                                }
                                else if ((SL.Children[1] as Picker).SelectedIndex == 1)
                                {
                                    Result = $" length(`{(SL.Children[0] as Picker).SelectedItem.ToString()}`) > length('{(SL.Children[2] as Entry).Text}')";
                                }
                                else if ((SL.Children[1] as Picker).SelectedIndex == 2)
                                {
                                    Result = $" length(`{(SL.Children[0] as Picker).SelectedItem.ToString()}`) < length('{(SL.Children[2] as Entry).Text}')";
                                }
                                else
                                {
                                    Result = $" `{(SL.Children[0] as Picker).SelectedItem.ToString()}` LIKE '%{(SL.Children[2] as Entry).Text}%'";
                                }
                                break;
                            case FieldType.TelephoneNumber:
                                if ((SL.Children[1] as Picker).SelectedIndex == 0)
                                {
                                    Result = $" `{(SL.Children[0] as Picker).SelectedItem.ToString()}` = '{(SL.Children[2] as Entry).Text}'";
                                }
                                else if ((SL.Children[1] as Picker).SelectedIndex == 1)
                                {
                                    Result = $" length(`{(SL.Children[0] as Picker).SelectedItem.ToString()}`) > length('{(SL.Children[2] as Entry).Text}')";
                                }
                                else if ((SL.Children[1] as Picker).SelectedIndex == 2)
                                {
                                    Result = $" length(`{(SL.Children[0] as Picker).SelectedItem.ToString()}`) < length('{(SL.Children[2] as Entry).Text}')";
                                }
                                else
                                {
                                    Result = $" `{(SL.Children[0] as Picker).SelectedItem.ToString()}` LIKE '%{(SL.Children[2] as Entry).Text}%'";
                                }
                                break;
                            case FieldType.MultiChoice:
                                Result = $" `{(SL.Children[0] as Picker).SelectedItem.ToString()}` = '{(SL.Children[2] as Picker).SelectedIndex}'";
                                break;
                            case FieldType.Date:
                                Result = $" `{(SL.Children[0] as Picker).SelectedItem.ToString()}` " + Invert((SL.Children[1] as Picker).SelectedItem.ToString()) + " " + (SL.Children[2] as DatePicker).Date.Ticks.ToString();
                                break;
                            case FieldType.Time:
                                Result = $" `{(SL.Children[0] as Picker).SelectedItem.ToString()}` " + Invert((SL.Children[1] as Picker).SelectedItem.ToString()) + " " + (SL.Children[2] as TimePicker).Time.Ticks.ToString();
                                break;
                            default:
                                Result = $" `{(SL.Children[0] as Picker).SelectedItem.ToString()}` " + Invert((SL.Children[1] as Picker).SelectedItem.ToString()) + " " + int.Parse((SL.Children[2] as Entry).Text).ToString();
                                break;
                        }
                        
                        break;
                }
            }
            else
            {
                switch ((SL.Children[0] as Picker).SelectedItem)
                {
                    case "الحضور":
                        Result = " NameCheck = " + (SL.Children[2] as Picker).SelectedIndex;
                        break;
                    case "وقت المجئ":
                        if ((SL.Children[1] as Picker).SelectedIndex == 1)
                        {
                            //Just in Time
                            Result = " `Time` = '" + (SL.Children[2] as TimePicker).Time.Ticks.ToString() + "'";
                        }
                        else if ((SL.Children[1] as Picker).SelectedIndex == 2)
                        {
                            //Before
                            Result = " `Time` < '" + (SL.Children[2] as TimePicker).Time.Ticks.ToString() + "'";
                        }
                        else
                        {
                            //After
                            Result = " `Time` > '" + (SL.Children[2] as TimePicker).Time.Ticks.ToString() + "'";
                        }
                        break;
                    case "درجة المجئ":
                        Result = " Bonus " + Invert((SL.Children[1] as Picker).SelectedItem.ToString()) + " " + int.Parse((SL.Children[2] as Entry).Text).ToString();
                        break;
                    case "درجة السلوك":
                        Result = " Degrees " + Invert((SL.Children[1] as Picker).SelectedItem.ToString()) + " " + int.Parse((SL.Children[2] as Entry).Text).ToString();
                        break;
                    case "سبب درجة السلوك":
                        if ((SL.Children[1] as Picker).SelectedIndex == 0)
                        {
                            Result = " Description = '" + (SL.Children[2] as Entry).Text + "'";
                        }
                        else if ((SL.Children[1] as Picker).SelectedIndex == 1)
                        {
                            Result = " Description LIKE '%" + (SL.Children[2] as Entry).Text + "%'";
                        }
                        else if ((SL.Children[1] as Picker).SelectedIndex == 2)
                        {
                            Result = " length(Description) > " + (SL.Children[2] as Entry).Text;
                        }
                        else //if ((SL.Children[1] as Picker).SelectedIndex == 3)
                        {
                            Result = " length(Description) < " + (SL.Children[2] as Entry).Text;
                        }
                        break;
                    case "درجة التفاعل":
                        Result = " Degrees " + Invert((SL.Children[1] as Picker).SelectedItem.ToString()) + " " + int.Parse((SL.Children[2] as Entry).Text).ToString();
                        break;
                    case "شرح درجة التفاعل":
                        if ((SL.Children[1] as Picker).SelectedIndex == 0)
                        {
                            Result = " Description = '" + (SL.Children[2] as Entry).Text + "'";
                        }
                        else if ((SL.Children[1] as Picker).SelectedIndex == 1)
                        {
                            Result = " Description LIKE '%" + (SL.Children[2] as Entry).Text + "%'";
                        }
                        else if ((SL.Children[1] as Picker).SelectedIndex == 2)
                        {
                            Result = " length(Description) > " + (SL.Children[2] as Entry).Text;
                        }
                        else //if ((SL.Children[1] as Picker).SelectedIndex == 3)
                        {
                            Result = " length(Description) < " + (SL.Children[2] as Entry).Text;
                        }
                        break;
                }
            }
            if (base.Children.Count > 2)
            {
                Result += ((base.Children[1] as Picker).SelectedIndex == 1 ? " OR " : " AND ") + (base.Children[2] as SqlWhereCondition).GetCondition();
            }
            return Result;
        }

        private string Invert(string v)
        {
            return v == ">" ? "<" : (v == "=" ? "=" : ">");
        }
    }
}