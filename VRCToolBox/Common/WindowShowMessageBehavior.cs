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
            MessageBroker.Default.Subscribe<MessageContent>(m => ModernWpf.MessageBox.Show(m.Text, nameof(VRCToolBox), (MessageBoxButton)m.Button, 
                                                                                           (MessageBoxImage)m.Icon, (MessageBoxResult)m.DefaultResult));
        }


        protected override void OnDetaching()
        {
            base.OnDetaching();
        }
    }
}
