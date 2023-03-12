using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Pictures.Interface;

namespace VRCToolBox.Pictures.ViewModel
{
    public class WorldVisitViewModel : ViewModelBase, IWorldVisitViewModel
    {
        private IWorldVisit _worldVisit;
        public DateTime VisitTime { get; }

        public string WorldName { get; } = String.Empty;

        public WorldVisitViewModel(IWorldVisit worldVisit)
        {
            _worldVisit = worldVisit;
            var disposable = _worldVisit as IDisposable;
            disposable?.AddTo(_compositeDisposable);

            VisitTime = _worldVisit.VisitTime;
            WorldName = _worldVisit.WorldName;
        }
    }
}
