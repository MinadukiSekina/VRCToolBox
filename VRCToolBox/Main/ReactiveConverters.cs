using Reactive.Bindings.Interactivity;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Input;
using ModernWpf.Controls;

namespace VRCToolBox.Main
{
    class ReactiveConverters
    {
    }
    public class NavigationViewSelectionToIndexConverter : ReactiveConverter<NavigationViewSelectionChangedEventArgs, NavigationViewItemBase>
    {
        protected override IObservable<NavigationViewItemBase?> OnConvert(IObservable<NavigationViewSelectionChangedEventArgs?> source) => source
            .Select(x => x?.SelectedItemContainer);
    }
}
