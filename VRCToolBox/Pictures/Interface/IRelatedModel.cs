using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCToolBox.Pictures.Interface
{
    /// <summary>
    /// State of relation. No-relation, new related, already related, remove related.
    /// </summary>
    public enum RelatedState
    {
        /// <summary>
        /// 紐づいていない。
        /// </summary>
        NonAttached,
        /// <summary>
        /// 関係を新たに紐づけする予定。
        /// </summary>
        Add,
        /// <summary>
        /// 紐づけられている。
        /// </summary>
        Attached,
        /// <summary>
        /// 紐づけを解除する予定。
        /// </summary>
        Remove
    }

    /// <summary>
    /// For content whitch is related other content.
    /// </summary>
    public interface IRelatedModel
    {
        /// <summary>
        /// The state of model's relation.
        /// </summary>
        public RelatedState State { get; }

        /// <summary>
        /// The its name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Changge the state of relation.
        /// </summary>
        public void ChangeState();
    }
}
