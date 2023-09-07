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
            // await 後にスレッドが変わってしまうとエラーになるようなので、念のため Dispatcher 経由にする・一応最前面の Window を Owner として出す
            MessageBroker.Default.Subscribe<MessageContent>(
                m => Dispatcher.Invoke(new Action<MessageContent>(m => {
                    var activeWindow = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
                    if (activeWindow is null) {
                        
                        AssociatedObject.Activate();
                        activeWindow = AssociatedObject;
                    }
                    ModernWpf.MessageBox.Show(activeWindow, m.Text, nameof(VRCToolBox), (MessageBoxButton)m.Button,
                                             (MessageBoxImage)m.Icon, (MessageBoxResult)m.DefaultResult);
                }), 
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
