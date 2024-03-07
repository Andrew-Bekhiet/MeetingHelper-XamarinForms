using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Xamarin.Forms;

namespace MeetingHelper.Models
{
    public class Absence : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        public int FaceId { get; set; }
        public int Id { get; set; }
        public bool NameCheck
        {
            get
            {
                return PNameCheck;
            }
            set
            {
                if (PNameCheck != value)
                {
                    PNameCheck = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NameCheck)));
                    if (value)
                    {
                        time = DateTime.Now.TimeOfDay;
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Time)));

                        Bonus = (int)Math.Round(DefaultDegrees - ((DateTime.Now.TimeOfDay - MustComeIn).TotalMinutes / (FinishIn - MustComeIn).TotalMinutes * DefaultDegrees), MidpointRounding.AwayFromZero);
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Bonus)));
                    }
                }
            }
        }
        private bool PNameCheck
        {
            get;
            set;
        }
        public ImageSource Image { get; set; }
        public string ImagePath { get; set; }
        public string Name { get; set; }

        private TimeSpan time { get; set; }
        public long Time2 { set { Time = TimeSpan.FromTicks(value); } get { return Time.Ticks; } }
        public TimeSpan Time
        {
            get => time;
            set
            {
                time = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Time)));
        
                if (Enabled)
                {
                    Bonus = (int)Math.Round(DefaultDegrees - ((time - MustComeIn).TotalMinutes / (FinishIn - MustComeIn).TotalMinutes * DefaultDegrees), MidpointRounding.AwayFromZero);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Bonus))); 
                }
            }
        }
        public TimeSpan MustComeIn { get; set; }
        public TimeSpan FinishIn { get; set; }
        public int Bonus { get; set; }
        public bool Enabled { get; }
        private readonly int DefaultDegrees;
        public Absence(ImageSource image, string name, TimeSpan time, int bonus, int Id, int FaceId, TimeSpan must_come_in, TimeSpan FinishesIn, bool nameCheck = false, bool enabled = true, string ImagePath = "")
        {
            this.FaceId = FaceId;
            this.Id = Id;
            Image = image;
            Name = name;
            Time = time;
            Bonus = bonus;
            NameCheck = nameCheck;
            Enabled = enabled;
            MustComeIn = must_come_in;
            FinishIn = FinishesIn;
            DefaultDegrees = int.Parse(App.Settings.Where(x => x.SettingName == "DAtTime").ElementAt(0).SettingValue, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
            this.ImagePath = ImagePath;
        }
        public Absence()
        {
            this.FaceId = default;
            Id = default;
            Image = default;
            Name = default;
            Time = DateTime.Now.TimeOfDay;
            Bonus = default;
            NameCheck = default;
            Enabled = false;
            DefaultDegrees = int.Parse(App.Settings.Where(x => x.SettingName == "DAtTime").ElementAt(0).SettingValue, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
        }
        public override string ToString()
        {
            return Name + Time;
        }
    }
    public class Behavior : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public int Id { get; set; }
        public int Degrees { get; set; }

        public bool _add
        {
            get => PAdd;
            set { PAdd = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(_add))); }
        }

        private bool PAdd;
        public string Description { get; set; }
        public ImageSource Image { get; set; }
        public string Name { get; set; }
        public bool Enabled { get; }
        public Behavior(ImageSource image, string name, int degrees, int Id, string description = "", bool add = false, bool enabled = true)
        {
            Image = image;
            Name = name;
            Degrees = degrees;
            this.Id = Id;
            _add = add;
            Description = description;
            Enabled = enabled;
        }
        public Behavior()
        {
            Image = default;
            Name = default;
            Degrees = default;
            Id = default;
            _add = default;
            Enabled = false;
        }
    }
    public class _Activity : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public int Id { get; set; }
        public string Name { get; set; }
        public int Degrees { get; set; }
        public bool _add
        {
            get => PAdd;
            set { PAdd = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(_add))); }
        }

        public bool PAdd { get; set; }
        public string Description { get; set; }
        public ImageSource Image { get; set; }
        public bool Enabled { get; }
        public _Activity(ImageSource image, string name, int degrees, int Id, string description = "", bool add = true, bool enabled = true)
        {
            Image = image;
            Name = name;
            Degrees = degrees;
            this.Id = Id;
            _add = add;
            Description = description;
            Enabled = enabled;
        }
        public _Activity()
        {
            Image = default;
            Name = default;
            Degrees = default;
            Id = default;
            _add = true;
            Description = default;
            Enabled = false;
        }
    }
}
