using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace VRCToolBox.Common
{
    public class ComboboxAttachment
    {
        public static EnterBehaviorMode GetEnterDownBehavior(DependencyObject obj)
        {
            return (EnterBehaviorMode)obj.GetValue(EnterDownBehaviorProperty);
        }

        public static void SetEnterDownBehavior(DependencyObject obj, EnterBehaviorMode value)
        {
            obj.SetValue(EnterDownBehaviorProperty, value);
        }

        // Using a DependencyProperty as the backing store for EnterDownBehavior.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnterDownBehaviorProperty =
            DependencyProperty.RegisterAttached("EnterDownBehavior", typeof(EnterBehaviorMode), typeof(ComboboxAttachment), new PropertyMetadata(EnterBehaviorMode.None, (d, e) =>
            {
                if (d is not ComboBox cb) { return; }
                if (e.NewValue is not EnterBehaviorMode mode) { return; }

                // 3.添付プロパティが設定されたときにイベント購読
                if (mode != EnterBehaviorMode.None)
                {
                    cb.PreviewKeyDown += OnTextBoxKeyDown;
                    cb.DropDownClosed += DropDownClosed;
                }
                else
                {
                    cb.PreviewKeyDown -= OnTextBoxKeyDown;
                    cb.DropDownClosed -= DropDownClosed;
                }
            }));

        private static void OnTextBoxKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // 4.Enterキーが押されたときに
            if (e.Key != System.Windows.Input.Key.Enter) return;
            UpdateSource(sender);
            //e.Handled = true;
        }
        private static void DropDownClosed(object? sender, EventArgs e)
        {
            UpdateSource(sender);
        }

        private static void UpdateSource(object? sender)
        {
            if (sender is not ComboBox cb)  return; 

            var mode = GetEnterDownBehavior(cb);

            if (mode == EnterBehaviorMode.None) return; 

            // Bindingを取得してUpdateSource呼出し
            var be = BindingOperations.GetBindingExpression(cb, ComboBox.TextProperty);
            be?.UpdateSource();
        }
    }
}
