using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCToolBox.Pictures.Interface
{
    public interface IRelatedViewModel
    {
        /// <summary>
        /// The state of model's relation.
        /// </summary>
        public ReactivePropertySlim<RelatedState> State { get; }

        /// <summary>
        /// The its name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Changge the state of relation.
        /// </summary>
        public ReactiveCommand ChangeStateCommand { get; }
    }
}
