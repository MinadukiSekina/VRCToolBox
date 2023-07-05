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
        private static Dictionary<Type, Type> _typeRelation = new Dictionary<Type, Type>() { 
            { typeof(Pictures.ViewModel.SearchConditionViewModel), typeof(Pictures.SearchConditionWindow) },
            { typeof(Pictures.ViewModel.ImageConverterViewmodel ), typeof(Pictures.ImageConverterView   ) }
        };
        private static Dictionary<Type, Type> _ownedRelation = new Dictionary<Type, Type>()
        {
            { typeof(Pictures.ViewModel.SearchConditionViewModel), typeof(MainWindow) },
            { typeof(Pictures.ViewModel.ImageConverterViewmodel ), typeof(MainWindow) }
        };

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

        // reference:https://qiita.com/tricogimmick/items/6e4fcc771bcfffaf66e8
        private static Window? CreateWindow<T>(T viewModel)
        {
            if (!_typeRelation.ContainsKey(viewModel!.GetType())) return null;
            var viewType = _typeRelation[viewModel.GetType()];
            var wnd = Activator.CreateInstance(viewType) as Window;
            if (wnd is not null) wnd.DataContext = viewModel;
            return wnd;
        }
        public static bool ShowDialog<T>(T viewModel)
        {
            var view = CreateWindow(viewModel);
            return view is not null && view.ShowDialog() == true;
        }
        public static bool ShowDialogWithOwner<T>(T viewModel)
        {
            var view = CreateWindow(viewModel);
            if (view is not null && _ownedRelation.ContainsKey(viewModel!.GetType()))
            {
                var window = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.GetType() == _ownedRelation[viewModel!.GetType()]);
                if (window is not null) view.Owner = window;
            }
            return view is not null && view.ShowDialog() == true;
        }
    }
}
