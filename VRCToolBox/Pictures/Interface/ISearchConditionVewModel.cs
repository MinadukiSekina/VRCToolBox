using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCToolBox.Pictures.Interface
{
    public interface ISearchConditionVewModel
    {
        public ReactivePropertySlim<string> Conditions { get; }

        public ReadOnlyReactiveCollection<IRelatedViewModel> SearchTags { get; }

        public void SetCondition();

        public void Update();
    }
}
