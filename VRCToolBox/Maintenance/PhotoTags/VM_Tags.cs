using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCToolBox.Maintenance.PhotoTags
{
    public class VM_Tags : ViewModelBase
    {
        private M_DataAccessorPhotoTag _mdaTag;
        public ReadOnlyReactiveCollection<VM_PhotoTag> Tags { get; }

        public VM_Tags() : this(new M_DataAccessorPhotoTag()) { }
        public VM_Tags(M_DataAccessorPhotoTag accessorPhotoTag)
        {
            _mdaTag = accessorPhotoTag;
            Tags = _mdaTag.PhotoTags.ToReadOnlyReactiveCollection(t => new VM_PhotoTag(t)).AddTo(_compositeDisposable);
        }
    }
}
