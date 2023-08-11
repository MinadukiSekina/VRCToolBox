using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;
using Reactive.Bindings.Notifiers;

namespace VRCToolBox.Common
{
    internal class WindowShowMessageBehavior : Behavior<Window>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            // await 後にスレッドが変わってしまうとエラーになるようなので、念のため Dispatcher 軽油にする
            MessageBroker.Default.Subscribe<MessageContent>(
                m => Dispatcher.Invoke(new Action<MessageContent>(m => ModernWpf.MessageBox.Show(AssociatedObject, m.Text, nameof(VRCToolBox), (MessageBoxButton)m.Button, 
                                                                                                 (MessageBoxImage)m.Icon, (MessageBoxResult)m.DefaultResult)), 
                                                                  System.Windows.Threading.DispatcherPriority.Send, 
                                                                  new object[] {m})
            );
        }


        protected override void OnDetaching()
        {
            base.OnDetaching();
        }
    }
}
