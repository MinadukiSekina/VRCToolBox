using Reactive.Bindings.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.Storage.Pickers;
using WinRT.Interop;
using System.Reactive.Threading.Tasks;

namespace VRCToolBox.Settings.SettingsCommon
{
    // reference : https://blog.okazuki.jp/entry/2015/02/22/142823
    internal class SelectFolderConverter : ReactiveConverter<RoutedEventArgs, string>
    {
        protected override IObservable<string?> OnConvert(IObservable<RoutedEventArgs?> source)
        {
            return source.Select(_ => new FolderPicker()).
                          Do(p => InitializeWithWindow.Initialize(p, new System.Windows.Interop.WindowInteropHelper(Application.Current.MainWindow).Handle)).
                          Do(p => p.SuggestedStartLocation = PickerLocationId.Desktop).
                          Do(p => p.FileTypeFilter.Add("*")).
                          SelectMany(p => p.PickSingleFolderAsync().AsTask().ToObservable()).
                          Where(f => f != null).
                          Select(f => f.Path);
        }
    }
}
