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
        public ObservableCollectionEX<string> VisitedWorldList { get; set; } = new ObservableCollectionEX<string>();

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
        private string _searchWorld = Settings.ProgramConst.NoDesignation;
        public string SearchWorld
        {
            get => _searchWorld;
            set
            {
                _searchWorld = value;
                RaisePropertyChanged();
            }
        }
        private RelayCommand? _setWorldVisitAsyncCommand;
        public RelayCommand SetWorldVisitAsyncCommand => _setWorldVisitAsyncCommand ??= new RelayCommand(async () => await SetWorldVisitListASync());
        private RelayCommand<Ulid>? _setUserListAsyncCommand;
        public RelayCommand<Ulid> SetUserListAsyncCommand => _setUserListAsyncCommand ??= new RelayCommand<Ulid>(async (worldVisitId) => await SetUserListAsync(worldVisitId));
        public LogViewerViewModel()
        {
            VisitedWorldList.Add(Settings.ProgramConst.NoDesignation);
            using (Data.UserActivityContext context = new Data.UserActivityContext())
            {
                VisitedWorldList.AddRange(context.WorldVisits.GroupBy(w => w.WorldName).Select(w =>w.Key).ToList()); 
            }
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
                                                                                         .WhereIf(w => w.WorldName.Contains(SearchWorld), !string.IsNullOrEmpty(SearchWorld) && SearchWorld != Settings.ProgramConst.NoDesignation)
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

    internal record struct SubStruct(string Item1, string Item2)
    {
        public static implicit operator (string, string)(SubStruct value)
        {
            return (value.Item1, value.Item2);
        }

        public static implicit operator SubStruct((string, string) value)
        {
            return new SubStruct(value.Item1, value.Item2);
        }
    }
}
