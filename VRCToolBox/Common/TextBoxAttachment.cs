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
    // reference:https://threeshark3.com/updatesource-with-enter/
    public enum EnterBehaviorMode
    {
        None,
        UpdateSource,
        UpdateSourceAndSelectAll
    }
    public static class TextBoxAttachment
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
            DependencyProperty.RegisterAttached("EnterDownBehavior", typeof(EnterBehaviorMode), typeof(TextBoxAttachment), new PropertyMetadata(EnterBehaviorMode.None, (d, e) =>
            {
                if (!(d is TextBox tb)) { return; }
                if (!(e.NewValue is EnterBehaviorMode mode)) { return; }

                // 3.添付プロパティが設定されたときにイベント購読
                if (mode != EnterBehaviorMode.None)
                {
                    tb.PreviewKeyDown += OnTextBoxKeyDown;
                }
                else
                {
                    tb.PreviewKeyDown -= OnTextBoxKeyDown;
                }
            }));

        private static void OnTextBoxKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // 4.Enterキーが押されたときに
            if (e.Key != System.Windows.Input.Key.Enter)
            {
                return;
            }
            if (!(sender is TextBox tb)) { return; }

            var mode = GetEnterDownBehavior(tb);

            if (mode == EnterBehaviorMode.None) { return; }

            // Bindingを取得してUpdateSource呼出し
            var be = BindingOperations.GetBindingExpression(tb, TextBox.TextProperty);
            be?.UpdateSource();

            // おまけ。入力中の文字を全選択するモード
            if (mode == EnterBehaviorMode.UpdateSourceAndSelectAll)
            {
                tb.SelectAll();
            }

            //e.Handled = true;
        }
    }
}
