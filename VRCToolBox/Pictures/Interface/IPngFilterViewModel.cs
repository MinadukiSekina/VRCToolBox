using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCToolBox.Pictures.Interface
{
    public interface IPngFilterViewModel
    {
        /// <summary>
        /// 表示用の名前
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// チェックボックスへのバインド用
        /// </summary>
        public ReactivePropertySlim<bool?> IsChecked { get; }

    }
}
