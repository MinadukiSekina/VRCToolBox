using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Pictures.Interface;

namespace VRCToolBox.Pictures.Photo
{
    /// <summary>
    /// The model of content whitch is related other content.
    /// </summary>
    public class M_RelatedContent : IRelatedModel
    {
        public RelatedState State { get; private set; }

        public string Name { get; private set; } = string.Empty;

        public void ChangeState()
        {
            switch (State)
            {
                case RelatedState.NonAttached:
                    State = RelatedState.Add;
                    break;
                case RelatedState.Attached:
                    State = RelatedState.Remove;
                    break;
                case RelatedState.Add:
                    State = RelatedState.NonAttached;
                    break;
                case RelatedState.Remove:
                    State = RelatedState.Attached;
                    break;
            }
        }
    }
}
