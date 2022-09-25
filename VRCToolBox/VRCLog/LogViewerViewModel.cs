using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using VRCToolBox.Common;
using VRCToolBox.Data;

namespace VRCToolBox.VRCLog
{
    public class LogViewerViewModel : ViewModelBase
    {
        public ObservableCollectionEX<Data.WorldVisit> WorldVisitsList { get; set; } = new ObservableCollectionEX<Data.WorldVisit>();
        public ObservableCollectionEX<Data.UserActivity> UserList { get; set; } = new ObservableCollectionEX<Data.UserActivity>();

        private DateTime _beginDate = DateTime.Now;
        public DateTime BeginDate
        {
            get => _beginDate;
            set
            {
                _beginDate = value;
                RaisePropertyChanged();
            }
        }
        private DateTime _endDate = DateTime.Now;
        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                _endDate = value;
                RaisePropertyChanged();
            }
        }
        private RelayCommand? _setWorldVisitAsyncCommand;
        public RelayCommand SetWorldVisitAsyncCommand => _setWorldVisitAsyncCommand ??= new RelayCommand(async () => await SetWorldVisitListASync());
        private RelayCommand<Ulid>? _setUserListAsyncCommand;
        public RelayCommand<Ulid> SetUserListAsyncCommand => _setUserListAsyncCommand ??= new RelayCommand<Ulid>(async (worldVisitId) => await SetUserListAsync(worldVisitId));
        public LogViewerViewModel()
        {
        }
        private async Task SetWorldVisitListASync()
        {
            //bool beginParseResult = DateTime.TryParse(BeginDate.Text, out DateTime beginDateTime);
            //bool endParseResult = DateTime.TryParse(EndDate.Text, out DateTime endDateTime);
            UserList.Clear();
            WorldVisitsList.Clear();
            using (Data.UserActivityContext userActivityContext = new Data.UserActivityContext())
            {
                List<Data.WorldVisit> worldVisits = await userActivityContext.WorldVisits.WhereIf(w => w.VisitTime >= BeginDate, BeginDate >= Settings.ProgramConst.MinimumDate)
                                                                                         .WhereIf(w => w.VisitTime <= EndDate.AddHours(24), EndDate >= Settings.ProgramConst.MinimumDate)
                                                                                         .OrderBy(w => w.VisitTime)
                                                                                         .ToListAsync();
                WorldVisitsList.AddRange(worldVisits);
            }
        }
        private async Task SetUserListAsync(Ulid worldVisitId)
        {
            UserList.Clear();
            using (Data.UserActivityContext userActivityContext = new Data.UserActivityContext())
            {
                List<Data.UserActivity> userActivities = await userActivityContext.UserActivities.Where(u => u.WorldVisitId == worldVisitId).OrderBy(u => u.ActivityTime).ToListAsync();
                UserList.AddRange(userActivities);
            }
        }
    }
}
