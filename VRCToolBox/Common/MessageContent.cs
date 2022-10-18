using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VRCToolBox.Common
{
    internal enum MessageButton
    {
        OK          = MessageBoxButton.OK,
        OKCancel    = MessageBoxButton.OKCancel,
        YesNoCancel = MessageBoxButton.YesNoCancel,
        YesNo       = MessageBoxButton.YesNo
    }
    internal enum MessageIcon
    {
        None        = MessageBoxImage.None,
        Error       = MessageBoxImage.Error,
        Stop        = MessageBoxImage.Stop,
        Question    = MessageBoxImage.Question,
        Exclamation = MessageBoxImage.Exclamation,
        Warning     = MessageBoxImage.Warning,
        Information = MessageBoxImage.Information,
    }
    internal enum MessageResult
    {
        None   = MessageBoxResult.None,
        OK     = MessageBoxResult.OK,
        Cancel = MessageBoxResult.Cancel,
        Yes    = MessageBoxResult.Yes,
        No     = MessageBoxResult.No
    }
    internal class MessageContent
    {
        internal string Text { get; set; } = string.Empty;
        internal MessageButton Button { get; set; }
        internal MessageIcon Icon { get; set; }
        internal MessageResult DefaultResult { get; set; }
    }
    internal static class MessageContentExtension 
    { 
        internal static void ShowMessage(this MessageContent message, Reactive.Bindings.Notifiers.IMessageBroker broker)
        {
            broker.Publish(message);
        }
    }

}
