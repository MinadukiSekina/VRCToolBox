using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCToolBox.Maintenance.PhotoTags
{
    public class VM_PhotoTag : ViewModelBase
    {
        private M_PhotoTag _photoTag;

        public ReactivePropertySlim<string> TagName { get; }

        public VM_PhotoTag() : this(new M_PhotoTag()) { }
        public VM_PhotoTag(M_PhotoTag m_PhotoTag)
        {
            _photoTag = m_PhotoTag;
            _photoTag.AddTo(_compositeDisposable);
            TagName = _photoTag.TagName.ToReactivePropertySlimAsSynchronized(n => n.Value).AddTo(_compositeDisposable);
        }

    }
}
