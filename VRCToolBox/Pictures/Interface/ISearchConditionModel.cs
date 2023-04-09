using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCToolBox.Pictures.Interface
{
    public interface ISearchConditionModel
    {
        public ReactivePropertySlim<string> Condition { get; }

        public List<Ulid> SelectTags { get; }

        public ObservableCollectionEX<IRelatedModel> SearchTags { get; }

        public void Initialize();

        public void Update();

        public void SetCondition();
    }
}
