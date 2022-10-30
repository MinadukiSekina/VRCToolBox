using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Data;

namespace VRCToolBox.Maintenance.PhotoTags
{
    public class VM_PhotoData : ViewModelBase
    {
        private PhotoData _photo;

        public ReactivePropertySlim<string> FullName { get; } = new ReactivePropertySlim<string>();
        public VM_PhotoData() : this(new PhotoData()) { }
        public VM_PhotoData(PhotoData photo)
        {
            _photo = photo;
            FullName.Value = _photo.FullName;
            FullName.AddTo(_compositeDisposable);
        }
    }
}
