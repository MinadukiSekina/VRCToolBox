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
    public class AutoSuggestBoxTextChangedConverter : ReactiveConverter<AutoSuggestBoxTextChangedEventArgs, (bool, string)>
    {
        protected override IObservable<(bool, string)> OnConvert(IObservable<AutoSuggestBoxTextChangedEventArgs?> source)
        {
            var autoSuggestBox = AssociateObject as AutoSuggestBox;
            return source.Select(x => (x is not null && x.Reason == AutoSuggestionBoxTextChangeReason.UserInput && autoSuggestBox is not null, autoSuggestBox?.Text ?? string.Empty));
        }
    }
    public class AutoSuggestBoxQuerySubmittedConverter : ReactiveConverter<AutoSuggestBoxQuerySubmittedEventArgs, string>
    {
        protected override IObservable<string> OnConvert(IObservable<AutoSuggestBoxQuerySubmittedEventArgs?> source)
        {
            return source.Select(x => x?.QueryText ?? string.Empty);
        }
    }
}
