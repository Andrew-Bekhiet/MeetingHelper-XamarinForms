using MeetingHelper.Services;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace MeetingHelper.Tables
{
    public class Data
    {
        public string Value { get; set; }
        public Data()
        {
        }
        public override string ToString()
        {
            return Value;
        }
    }
    [Table("Persons")]
    public class PersonTable
    {
        public PersonTable()
        {
        }
        public PersonTable(PersonObject personObject)
        {
            if (personObject != null)
            {
                var Me = App.Data.Get<PersonTable>(personObject.Id);
                Id = Me.Id;
                FaceId = Me.Id;
                Activity = Me.Activity;
                Absense = Me.Absense;
                Behaviour = Me.Behaviour;
                Image = Me.Image;
                Name = Me.Name;
                TNum = Me.TNum;
            }
            else
            {
                throw new NullReferenceException("Object 'personObject' is null");
            }
        }

        [PrimaryKey, AutoIncrement, Column("Id")]
        public int Id { get; set; }
        public int FaceId { get; set; }
        public int Activity { get; set; }
        public int Absense { get; set; }
        public int Behaviour { get; set; }
        public string Image { get; set; }

        [Column("الاسم")]
        public string Name { get; set; }

        [Column("رقم الهاتف")]
        public string TNum { get; set; }

        [Ignore]
        public List<string> PData
        {
            get
            {
                List<string> result = new List<string>(PFData.Count);
                foreach (string item in PFData)
                {
                    result.Add(App.Data.Query<Data>($"select `{item.Replace("الصورة", "Image")}` as Value from Persons where Id = {Id}")[0].Value);
                }
                return result;
            }
        }
        [Ignore]
        public List<string> PFData => App.Data.Table<Fields>().ToList().Select(F => F.FieldName).ToList();
        public List<string> GetPFDataWoutPic => PFData.Where(x => x != "الصورة").ToList();
        public string GetName => Name;
        public ImageSource GetImage => ImageSource.FromFile(GetImagePath);
        public string GetImagePath => string.IsNullOrEmpty(Image) ? Path.Combine(DependencyService.Get<ISQLiteGetPath>().GetDefault(), "MeetingHelper.Person.png") : App.RepHomeDir + Image.Remove(0, 1);
        public string GetPhoneNumber => TNum;
        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            TableMapping.Column[] Columns = App.Data.Table<PersonTable>().Table.Columns;
            foreach (TableMapping.Column item in Columns)
            {
                result.Append(item.GetValue(this).ToString());
            }
            return result.ToString();
        }
        public string ToFormattedString(string Seperator)
        {
            StringBuilder result = new StringBuilder();
            TableMapping.Column[] Columns = App.Data.Table<PersonTable>().Table.Columns;
            foreach (TableMapping.Column item in Columns)
            {
                result.Append(item.GetValue(this).ToString() + Seperator);
            }
            return result.ToString();
        }

        //public string PersonData
        //{
        //    get
        //    {
        //        string value = "";
        //        foreach (string S in PData)
        //        {
        //            value += S + ";";
        //        }
        //        return value.Remove(value.Length - 1);
        //    }
        //    set => PData = value.Split(';').ToList<string>();
        //}
        //public string PersonFieldData
        //{
        //    get
        //    {
        //        string value = "";
        //        foreach (string S in PFData)
        //        {
        //            value += S + ";";
        //        }
        //        return value.Remove(value.Length - 1);
        //    }
        //    set => PFData = value.Split(';').ToList<string>();
        //}
    }
    public class PersonObject
    {
        public int Id { get; set; }
        public PersonObject(int Id)
        {
            this.Id = Id;
        }
        public PersonObject(PersonTable personTable)
        {
            if (personTable != null)
            {
                Id = personTable.Id;
            }
            else
            {
                throw new NullReferenceException();
            }
        }
        public PersonTable GetPersonTable()
        {
            return new PersonTable(this);
        }
    }

    [Table("Days")]
    public class Days
    {
        [PrimaryKey, AutoIncrement, Column("Id")]
        public int Id { get; set; }
        public string Date { get; set; }
        public string DayName { get; set; }
        public string TableName { get; set; }
        public override string ToString()
        {
            return DayName + " " + Date;
        }
    }

    [Table("Settings")]
    public class Settings
    {
        [PrimaryKey, Column("Id")]
        public string SettingName { get; set; }
        public string SettingValue { get; set; }
    }

    [Table("Fields")]
    public class Fields
    {
        [PrimaryKey]
        public string FieldName { get; set; }
        public string FieldType { get; set; }
        public string FieldChoices
        {
            get
            {
                string value = "";
                foreach (string S in _FieldChoices)
                {
                    value += S + "|";
                }
                if (!string.IsNullOrEmpty(value))
                {
                    return value.Remove(value.Length - 1);
                }

                return "";
            }
            set => _FieldChoices = string.IsNullOrEmpty(value) ? System.Array.Empty<string>() : value.Split('|');
        }
        [Ignore]
        public string[] _FieldChoices { get; set; } = System.Array.Empty<string>();
    }

    [Table("SQLiteSavedQueries")]
    public class SQLSavedQuery
    {
        [PrimaryKey, Column("Name")]
        public string FriendlyUserName { get; set; }
        public string Command { get; set; }
        public bool IsFromPersons { get; set; }
    }

    public abstract class FieldType
    {
        public const string Image = "Image";
        public const string @Srting = "String";
        public const string TelephoneNumber = "TN";

        public const string MultiChoice = "MultiChoice";

        public const string @Integer = "Int";
        public const string Date = "Date";
        public const string Time = "Time";
    }

#pragma warning disable IDE1006 // Naming Styles
    public class DB_Info
    {
        public string type { get; set; }
        public string name { get; set; }
        public int rootpage { get; set; }
        public string sql { get; set; }
        public override string ToString()
        {
            return name;
        }
    }
#pragma warning restore IDE1006 // Naming Styles
}
