using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VRCToolBox.Common
{
    // reference:https://blog.okazuki.jp/entry/20101215/1292384080
    internal class WindowManager
    {
        public static void ShowOrActivate<TWindow>()
                where TWindow : Window, new()
        {
            // 対象Windowが開かれているか探す
            var window = Application.Current.Windows.OfType<TWindow>().FirstOrDefault();
            if (window == null)
            {
                // 開かれてなかったら開く
                window = new TWindow();
                window.Show();
            }
            else
            {
                // 既に開かれていたらアクティブにする
                window.Activate();
                if (window.WindowState == WindowState.Minimized) window.WindowState = WindowState.Normal;
            }
        }

        // newでインスタンスが作れない時用
        public static void ShowOrActivate<TWindow>(Func<TWindow> factory)
            where TWindow : Window
        {
            // 対象Windowが開かれているか探す
            var window = Application.Current.Windows.OfType<TWindow>().FirstOrDefault();
            if (window == null)
            {
                // 開かれてなかったら開く
                window = factory();
                window.Show();
            }
            else
            {
                // 既に開かれていたらアクティブにする
                window.Activate();
                if (window.WindowState == WindowState.Minimized) window.WindowState = WindowState.Normal;
            }
        }
        public static void ShowOrActivate<TWindow>(Window owner)
                where TWindow : Window, new()
        {
            // 対象Windowが開かれているか探す
            var window = Application.Current.Windows.OfType<TWindow>().FirstOrDefault();
            if (window == null)
            {
                // 開かれてなかったら開く
                window = new TWindow();
                window.Owner = owner;
                window.Show();
            }
            else
            {
                window.Owner = owner;
                // 既に開かれていたらアクティブにする
                window.Activate();
                if (window.WindowState == WindowState.Minimized) window.WindowState = WindowState.Normal;
            }
        }

        // newでインスタンスが作れない時用
        public static void ShowOrActivate<TWindow>(Func<TWindow> factory, Window owner)
            where TWindow : Window
        {
            // 対象Windowが開かれているか探す
            var window = Application.Current.Windows.OfType<TWindow>().FirstOrDefault();
            if (window == null)
            {
                // 開かれてなかったら開く
                window = factory();
                window.Owner = owner;
                window.Show();
            }
            else
            {
                window.Owner = owner;
                // 既に開かれていたらアクティブにする
                window.Activate();
                if (window.WindowState == WindowState.Minimized) window.WindowState = WindowState.Normal;
            }
        }
    }
}
