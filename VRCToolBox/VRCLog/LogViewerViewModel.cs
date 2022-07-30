using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using VRCToolBox.Data;

namespace VRCToolBox.VRCLog
{
    public class LogViewerViewModel //: INotifyPropertyChanged
    {
        public ObservableCollection<WorldVisit> worldVisitsList { get; set; }
        //public event PropertyChangedEventHandler? PropertyChanged;
        //private void NotifyPropertyChanged(string propertyName)
        //{
        //    PropertyChangedEventHandler? handler = PropertyChanged;
        //    if (handler is not null) handler(this, new PropertyChangedEventArgs(propertyName));
        //}
        public LogViewerViewModel()
        {
            using (UserActivityContext userActivityContext = new UserActivityContext())
            {
                worldVisitsList = new ObservableCollection<WorldVisit>(userActivityContext.WorldVisits.ToList());
            }
        }
    }
}
