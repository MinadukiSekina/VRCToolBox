using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using VRCToolBox.Common;
using VRCToolBox.Data;

namespace VRCToolBox.VRCLog
{
    public class LogViewerViewModel : ViewModelBase
    {
        public ObservableCollectionEX<Data.WorldVisit> worldVisitsList { get; set; } = new ObservableCollectionEX<Data.WorldVisit>();
        public ObservableCollectionEX<Data.UserActivity> UserList { get; set; } = new ObservableCollectionEX<Data.UserActivity>();
        public LogViewerViewModel()
        {
            using (UserActivityContext userActivityContext = new UserActivityContext())
            {
                //worldVisitsList = new ObservableCollection<WorldVisit>(userActivityContext.WorldVisits.ToList());
            }
        }
        //private async Task SetWorldVisitList()
        //{
        //    bool beginParseResult = DateTime.TryParse(BeginDate.Text, out DateTime beginDateTime);
        //    bool endParseResult = DateTime.TryParse(EndDate.Text, out DateTime endDateTime);
        //    UserList.Clear();
        //    worldVisitsList.Clear();
        //    using (Data.UserActivityContext userActivityContext = new Data.UserActivityContext())
        //    {
        //        List<Data.WorldVisit> worldVisits = await userActivityContext.WorldVisits.WhereIf(w => w.VisitTime >= beginDateTime, beginParseResult).
        //                                                                                  WhereIf(w => w.VisitTime <= endDateTime.AddHours(24), endParseResult).
        //                                                                                  ToListAsync();
        //        worldVisitsList.AddRange(worldVisits);
        //    }
        //}
        //private async Task SetUserList(Ulid worldVisitId)
        //{
        //    UserList.Clear();
        //    using (Data.UserActivityContext userActivityContext = new Data.UserActivityContext())
        //    {
        //        List<Data.UserActivity> userActivities = await userActivityContext.UserActivities.Where(u => u.WorldVisitId == worldVisitId).ToListAsync();
        //        UserList.AddRange(userActivities);
        //    }
        //}

        //private async void Window_Loaded(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        await SetWorldVisitList();
        //    }
        //    catch (Exception ex)
        //    {
        //        //MessageBox.Show(ex.Message);
        //    }
        //}

        //private async void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        await SetWorldVisitList();
        //    }
        //    catch (Exception ex)
        //    {
        //        //MessageBox.Show(ex.Message);
        //    }
        //}

        //private async void WorldVisitList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    try
        //    {
        //        if (e.AddedItems.Count > 0 && e.AddedItems[0] is Data.WorldVisit visitWorld)
        //        {
        //            await SetUserList(visitWorld.WorldVisitId);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //MessageBox.Show(ex.Message);
        //    }
        //}
    }
}
