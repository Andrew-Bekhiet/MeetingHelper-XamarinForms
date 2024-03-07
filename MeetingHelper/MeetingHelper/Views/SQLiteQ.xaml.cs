using MeetingHelper.Models;
using MeetingHelper.Services;
using MeetingHelper.Tables;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Behavior = MeetingHelper.Models.Behavior;

namespace MeetingHelper.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SQLiteQ : TabbedPage
    {
        private readonly List<Days> AvaliableDs = new List<Days>() { new Days() };
        private readonly List<ValueTuple<ToolbarItem, string>> Queries = new List<(ToolbarItem, string)>();
        public SQLiteQ()
        {
            InitializeComponent();
            if ((((MasterDetailPage)Application.Current.MainPage).Master as ItemsPage).GetCurrentSelection() != "بحث")
            {
                (((MasterDetailPage)Application.Current.MainPage).Master as ItemsPage).Navigate("بحث");
            }
            Title = "بحث";
            if (DependencyService.Get<IPlatform>().UWPResult() == "UWP")
            {
                FlowDirection = FlowDirection.LeftToRight;
            }
            foreach (SQLSavedQuery item in App.Data.Table<SQLSavedQuery>().ToList())
            {
                ToolbarItem tmp = new ToolbarItem
                {
                    Order = ToolbarItemOrder.Secondary,
                    Text = item.FriendlyUserName
                };
                tmp.Clicked += Tmp_Clicked;
                ToolbarItems.Add(tmp);
                Queries.Add((ToolbarItems[ToolbarItems.Count - 1], item.Command));
            }
            AddQ.ImageSource = ImageSource.FromResource("MeetingHelper.AddField.png", typeof(App));
            DeleteQ.ImageSource = ImageSource.FromResource("MeetingHelper.Delete.png", typeof(App));
            DeleteQAdv.ImageSource = ImageSource.FromResource("MeetingHelper.Delete.png", typeof(App));
            AddQAdv.ImageSource = ImageSource.FromResource("MeetingHelper.AddField.png", typeof(App));
            ExecuteQuery.ImageSource = ImageSource.FromResource("MeetingHelper.SearchI.png", typeof(App));
            ExecuteAdvanced.ImageSource = ImageSource.FromResource("MeetingHelper.SearchI.png", typeof(App));
            List<string> TablesToSearch;
            try
            {
                TablesToSearch = (from rows in App.Data.Table<Days>().ToList() select rows.ToString()).ToList();
                AvaliableDs.AddRange((from rows in App.Data.Table<Days>().ToList() select rows).ToList());
            }
            catch (NullReferenceException)
            {
                TablesToSearch = new List<string>();
            }
            TablesToSearch.Insert(0, "المخدومين");
            AvaliableTables.ItemsSource = TablesToSearch;
            AvaliableTables.SelectedIndex = 0;
        }

        private void Tmp_Clicked(object sender, EventArgs e)
        {
            string Query = Queries[ToolbarItems.IndexOf(sender as ToolbarItem)].Item2;
            if (Query.Contains("Persons"))
            {
                List<PersonTable> Result = App.Data.Query<PersonTable>(Query);
                Navigation.PushAsync(new SearchResults(Result));
            }
            else if (Queries[ToolbarItems.IndexOf(sender as ToolbarItem)].Item2.Contains("_AC"))
            {
                List<_Activity> Result = App.Data.Query<_Activity>(Query);
                foreach (_Activity item in Result)
                {
                    item.Name = App.Data.Get<PersonTable>(item.Id).GetName;
                    item.Image = App.Data.Get<PersonTable>(item.Id).GetImage;
                }
                Navigation.PushAsync(new SearchResults(Result));
            }
            else if (Queries[ToolbarItems.IndexOf(sender as ToolbarItem)].Item2.Contains("_B"))
            {
                List<Behavior> Result = App.Data.Query<Behavior>(Query);
                foreach (Behavior item in Result)
                {
                    item.Name = App.Data.Get<PersonTable>(item.Id).GetName;
                    item.Image = App.Data.Get<PersonTable>(item.Id).GetImage;
                }
                Navigation.PushAsync(new SearchResults(Result));
            }
            else if (Queries[ToolbarItems.IndexOf(sender as ToolbarItem)].Item2.Contains("_A"))
            {
                List<Absence> Result = App.Data.Query<Absence>(Query);
                foreach (Absence item in Result)
                {
                    item.Name = App.Data.Get<PersonTable>(item.Id).GetName;
                    item.Image = App.Data.Get<PersonTable>(item.Id).GetImage;
                }
                Navigation.PushAsync(new SearchResults(Result));
            }
            else
            {
                var Result = App.Data.Query<Tables.DB_Info>(Query);
                Navigation.PushAsync(new SearchResults(Result));
            }
        }

        public override string ToString()
        {
            return string.Empty;
        }

        private void AvaliableTables_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (AvaliableTables.SelectedIndex >= 1)
            {
                Columns.ChangeType(1);
            }
            else
            {
                Columns.ChangeType(0);
            }
        }

        private void ExecuteQuery_Clicked(object sender, EventArgs e)
        {
            try
            {
                string Q = Columns.GetWhereStatment();
                if (AvaliableTables.SelectedIndex == 0)
                {
                    string Query = "Select * From Persons Where " + Q;
                    Debug.WriteLine(Query);
                    List<PersonTable> Result = App.Data.Query<PersonTable>(Query);
                    Navigation.PushAsync(new SearchResults(Result));
                }
                else
                {
                    string Query;
                    if (Columns.GetType2 == "_A")
                    {
                        Query = "Select Id, NameCheck, time(Time), Bonus From '" + AvaliableDs[AvaliableTables.SelectedIndex].TableName + Columns.GetType2 + "' Where " + Q;
                        List<Absence> Result = App.Data.Query<Absence>(Query);
                        foreach (Absence item in Result)
                        {
                            item.Name = App.Data.Get<PersonTable>(item.Id).GetName;
                            item.Image = App.Data.Get<PersonTable>(item.Id).GetImage;
                        }
                        Navigation.PushAsync(new SearchResults(Result));
                    }
                    else if (Columns.GetType2 == "_B")
                    {
                        Query = "Select * From '" + AvaliableDs[AvaliableTables.SelectedIndex].TableName + Columns.GetType2 + "' Where " + Q;
                        List<Behavior> Result = App.Data.Query<Behavior>(Query);
                        foreach (Behavior item in Result)
                        {
                            item.Name = App.Data.Get<PersonTable>(item.Id).GetName;
                            item.Image = App.Data.Get<PersonTable>(item.Id).GetImage;
                        }
                        Navigation.PushAsync(new SearchResults(Result));
                    }
                    else
                    {
                        Query = "Select * From '" + AvaliableDs[AvaliableTables.SelectedIndex].TableName + Columns.GetType2 + "' Where " + Q;
                        List<_Activity> Result = App.Data.Query<_Activity>(Query);
                        foreach (_Activity item in Result)
                        {
                            item.Name = App.Data.Get<PersonTable>(item.Id).GetName;
                            item.Image = App.Data.Get<PersonTable>(item.Id).GetImage;
                        }
                        Navigation.PushAsync(new SearchResults(Result));
                    }
                    Debug.WriteLine(Query);
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("حدث خطأ غير متوقع", ex.ToString(), "حسنًا");
            }
        }

        private async void ExecuteAdvanced_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (TableType.SelectedIndex == 0)
                {
                    List<PersonTable> Result = App.Data.Query<PersonTable>(SQLCommand.Text);
                    await Navigation.PushAsync(new SearchResults(Result));
                }
                else if (TableType.SelectedIndex == 1)
                {
                    List<Absence> Result = App.Data.Query<Absence>(SQLCommand.Text);
                    foreach (Absence item in Result)
                    {
                        item.Name = App.Data.Get<PersonTable>(item.Id).GetName;
                        item.Image = App.Data.Get<PersonTable>(item.Id).GetImage;
                    }
                    await Navigation.PushAsync(new SearchResults(Result));
                }
                else if (TableType.SelectedIndex == 2)
                {
                    List<_Activity> Result = App.Data.Query<_Activity>(SQLCommand.Text);
                    foreach (_Activity item in Result)
                    {
                        item.Name = App.Data.Get<PersonTable>(item.Id).GetName;
                        item.Image = App.Data.Get<PersonTable>(item.Id).GetImage;
                    }
                    await Navigation.PushAsync(new SearchResults(Result));
                }
                else if (TableType.SelectedIndex == 3)
                {
                    List<Behavior> Result = App.Data.Query<Behavior>(SQLCommand.Text);
                    foreach (Behavior item in Result)
                    {
                        item.Name = App.Data.Get<PersonTable>(item.Id).GetName;
                        item.Image = App.Data.Get<PersonTable>(item.Id).GetImage;
                    }
                    await Navigation.PushAsync(new SearchResults(Result));
                }
                else
                {
                    await Navigation.PushAsync(new SearchResults(App.Data.Query<DB_Info>(SQLCommand.Text)));
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("حدث خطأ", ex.Message, "رجوع");
            }
        }

        private async void AddQ_Clicked(object sender, EventArgs e)
        {
            try
            {
                string Q = Columns.GetWhereStatment();
                if (AvaliableTables.SelectedIndex == 0)
                {
                    string Query = "Select * From Persons Where " + Q;
                    string Name = (await Acr.UserDialogs.UserDialogs.Instance.PromptAsync("أدخل تسمية:")).Text;
                    if(string.IsNullOrEmpty(Name))
                        App.Data.Insert(new SQLSavedQuery() { Command = Query, FriendlyUserName = Name, IsFromPersons = true });
                }
                else
                {
                    string Query;
                    if (Columns.GetType2 == "_A")
                    {
                        Query = "Select Id, NameCheck, time(Time), Bonus From '" + AvaliableDs[AvaliableTables.SelectedIndex].TableName + Columns.GetType2 + "' Where " + Q;
                    }
                    else if (Columns.GetType2 == "_B")
                    {
                        Query = "Select * From '" + AvaliableDs[AvaliableTables.SelectedIndex].TableName + Columns.GetType2 + "' Where " + Q;
                    }
                    else
                    {
                        Query = "Select * From '" + AvaliableDs[AvaliableTables.SelectedIndex].TableName + Columns.GetType2 + "' Where " + Q;
                    }
                    Debug.WriteLine(Query);
                    string Name = (await Acr.UserDialogs.UserDialogs.Instance.PromptAsync("أدخل تسمية:")).Text;
                    if(string.IsNullOrEmpty(Name))
                        App.Data.Insert(new SQLSavedQuery() { Command = Query, FriendlyUserName = Name, IsFromPersons = false });
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("حدث خطأ غير متوقع", ex.ToString(), "حسنًا");
            }
        }

        private async void DeleteQ_Clicked(object sender, EventArgs e)
        {
            string Chosen = await DisplayActionSheet("يرجى إختيار أمر لحذفه:", "إلغاء الأمر", null, App.Data.Table<SQLSavedQuery>().ToList().Select(x => x.FriendlyUserName).ToArray());
            if(string.IsNullOrEmpty(Chosen))
            {
                App.Data.Delete(new SQLSavedQuery() { FriendlyUserName = Chosen });
            }
        }

        private async void AddQAdv_Clicked(object sender, EventArgs e)
        {
            try
            {
                string Name = (await Acr.UserDialogs.UserDialogs.Instance.PromptAsync("أدخل تسمية:")).Text;
                if (string.IsNullOrEmpty(Name))
                    App.Data.Insert(new SQLSavedQuery() { Command = SQLCommand.Text, FriendlyUserName = Name, IsFromPersons = false });
            }
            catch (Exception ex)
            {
                await DisplayAlert("حدث خطأ", ex.Message, "رجوع");
            }
        }
    }
}