using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Common;
using VRCToolBox.Pictures.Shared;
using VRCToolBox.Pictures.Interface;

namespace VRCToolBox.Pictures.Model
{
    internal class MessageNotifierBase : DisposeBase,  IMessageNotifier
    {
        private bool _disposed;
        private CompositeDisposable _disposables = new();

        private string _additionalMessage;
        private ReactivePropertySlim<MessageContent> Message { get; }

        ReactivePropertySlim<MessageContent> IMessageNotifier.MessageContent => Message;

        string IMessageNotifier.AdditionalMessage => _additionalMessage;

        void IMessageNotifier.RaiseErrorMessage(Exception? ex) => RaiseErrorMessage(ex);


        internal MessageNotifierBase(string additionalMessage)
        {
            _additionalMessage = additionalMessage;

            Message = new ReactivePropertySlim<MessageContent>().AddTo(_disposables);
        }

        protected void RaiseErrorMessage(Exception? ex)
        {
            if (ex is null) return;
            Message.Value = new MessageContent(ex, _additionalMessage);
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _disposables.Dispose();
                }
                _disposed = true;
            }
            base.Dispose(disposing);
        }

    }
}
