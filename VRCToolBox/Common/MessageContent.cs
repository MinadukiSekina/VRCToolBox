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

        // コンストラクタ
        internal MessageContent() : this(string.Empty, MessageButton.OK, MessageIcon.None, MessageResult.None)
        {
        }
        
        internal MessageContent(Exception ex) : this(ex, string.Empty) { }
        
        internal MessageContent(Exception ex, string additionalMessage) : this($"{additionalMessage}{Environment.NewLine}{ex.Message}", MessageButton.OK, MessageIcon.Error, MessageResult.OK)
        {
        }

        internal MessageContent(string text, MessageButton button, MessageIcon icon, MessageResult defaultResult)
        {
            Text          = text;
            Button        = button;
            Icon          = icon;
            DefaultResult = defaultResult;
        }

    }
    internal static class MessageContentExtension 
    { 
        internal static void ShowMessage(this MessageContent message, Reactive.Bindings.Notifiers.IMessageBroker broker)
        {
            broker.Publish(message);
        }
        internal static void ShowMessage(this MessageContent message)
        {
            Reactive.Bindings.Notifiers.MessageBroker.Default.Publish(message);         
        }
        internal static MessageResult ShowDialog(this MessageContent message)
        {
            return (MessageResult)ModernWpf.MessageBox.Show(message.Text, nameof(VRCToolBox), (MessageBoxButton)message.Button, (MessageBoxImage)message.Icon, (MessageBoxResult)message.DefaultResult);
        }
    }

}
