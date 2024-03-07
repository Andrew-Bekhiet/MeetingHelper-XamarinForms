using MeetingHelper.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MeetingHelper.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Randomizer : ContentPage
    {
        public Randomizer()
        {
            InitializeComponent();
            NumMax.Text = App.Data.Table<MeetingHelper.Tables.PersonTable>().ToList().Count.ToString(System.Globalization.NumberFormatInfo.InvariantInfo);
        }

        private void GenerateN_Clicked(object sender, EventArgs e)
        {
            try
            {
                Number.Text = App.R.Next(int.Parse(NumMin.Text), int.Parse(NumMax.Text) + 1).ToString(System.Globalization.NumberFormatInfo.InvariantInfo);
            }
            catch (ArgumentOutOfRangeException)
            {
                Number.Text = App.R.Next(int.Parse(NumMax.Text), int.Parse(NumMin.Text) + 1).ToString(System.Globalization.NumberFormatInfo.InvariantInfo);
            }
        }

        private void GenerateP_Clicked(object sender, EventArgs e)
        {
            App.ChooseRandomPerson(App.Data.Table<MeetingHelper.Tables.PersonTable>().ToList(), Width, Height, Navigation);
        }
    }
}