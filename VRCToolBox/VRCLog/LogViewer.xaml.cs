using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using VRCToolBox.Common;

namespace VRCToolBox.VRCLog
{
    /// <summary>
    /// LogViewer.xaml の相互作用ロジック
    /// </summary>
    public partial class LogViewer : Window
    {
        public ObservableCollectionEX<Data.WorldVisit> worldVisitsList { get; set;} = new ObservableCollectionEX<Data.WorldVisit>();
        public ObservableCollectionEX<Data.UserActivity> UserList { get; set;} = new ObservableCollectionEX<Data.UserActivity>();
        public LogViewer()
        {
            InitializeComponent();
            DataContext = this;
        }
        private async Task SetWorldVisitList()
        {
            bool beginParseResult = DateTime.TryParse(BeginDate.Text, out DateTime beginDateTime);
            bool endParseResult   = DateTime.TryParse(EndDate.Text  , out DateTime endDateTime);
            UserList.Clear();
            worldVisitsList.Clear();
            using (Data.UserActivityContext userActivityContext = new Data.UserActivityContext())
            {
                List<Data.WorldVisit> worldVisits = await userActivityContext.WorldVisits.WhereIf(w => w.VisitTime >= beginDateTime, beginParseResult).
                                                                                          WhereIf(w => w.VisitTime <= endDateTime.AddHours(24), endParseResult).
                                                                                          ToListAsync();
                worldVisitsList.AddRange(worldVisits);
            }
        }
        private async Task SetUserList(Ulid worldVisitId)
        {
            UserList.Clear();
            using(Data.UserActivityContext userActivityContext = new Data.UserActivityContext())
            {
                List<Data.UserActivity> userActivities = await userActivityContext.UserActivities.Where(u => u.WorldVisitId == worldVisitId).ToListAsync();
                UserList.AddRange(userActivities);
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await SetWorldVisitList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await SetWorldVisitList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void WorldVisitList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if(e.AddedItems.Count > 0 && e.AddedItems[0] is Data.WorldVisit visitWorld)
                {
                    await SetUserList(visitWorld.WorldVisitId);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
