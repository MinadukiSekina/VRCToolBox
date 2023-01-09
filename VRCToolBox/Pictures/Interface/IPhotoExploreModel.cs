using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCToolBox.Pictures.Interface
{
    public interface IPhotoExploreModel
    {
        public IPhotoDataModel PhotoDataModel { get; }

        public ObservableCollectionEX<string> Directories { get; }

        public ObservableCollectionEX<string> HoldPhotos { get; }
    }
}
