using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;

namespace VRCToolBox.Common
{
    // reference : https://blog.okazuki.jp/entry/2017/02/01/123827
    internal class WhenWindowClosedBehavior : Behavior<Window>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Closed += WindowClosed;
        }

        private void WindowClosed(object? sender, EventArgs e)
        {
            if(AssociatedObject is MainWindow main)
            {
                foreach(Window window in App.Current.Windows)
                {
                    if (window is MainWindow) continue;
                    var context = window.DataContext as IDisposable;
                    window.Close();
                    context?.Dispose();
                }
                (main.DataContext as IDisposable)?.Dispose();
            }
            //(AssociatedObject.DataContext as IDisposable)?.Dispose();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Closed -= WindowClosed;
        }
    }
}
