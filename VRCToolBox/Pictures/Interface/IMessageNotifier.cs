using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCToolBox.Pictures.Interface
{
    internal interface IMessageNotifier
    {
        /// <summary>
        /// メッセージを保持します
        /// </summary>
        internal ReactivePropertySlim<MessageContent> MessageContent { get; }

        /// <summary>
        /// メッセージに追加する文言。モデル内で固定の想定
        /// </summary>
        internal string AdditionalMessage { get; }

        /// <summary>
        /// エラー時にメッセージを更新します
        /// </summary>
        /// <param name="ex"></param>
        internal void RaiseErrorMessage(Exception? ex);
    }

    internal interface IMessageReciever
    {
        /// <summary>
        /// メッセージを保持します
        /// </summary>
        internal ReadOnlyReactivePropertySlim<MessageContent?> MessageContent { get; }

    }
}
