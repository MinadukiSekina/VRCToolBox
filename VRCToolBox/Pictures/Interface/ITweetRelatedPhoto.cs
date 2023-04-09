using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCToolBox.Pictures.Interface
{
    public interface ITweetRelatedPhoto
    {
        public string FullName { get; }

        public int Order { get; }

        public ReactivePropertySlim<RelatedState> State { get; }

        public void ChangeState();
    }
}
