using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Common;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using ModernWpf.Controls;

namespace VRCToolBox.Settings
{
    public class VM_SettingsBase : ViewModelBase
    {
        public ReactivePropertySlim<ViewModelBase> Content { get; } = new ReactivePropertySlim<ViewModelBase>();
        public ReactiveCommand<NavigationViewItemBase> ChangeContentCommand { get; } = new ReactiveCommand<NavigationViewItemBase>();

        public VM_SettingsBase()
        {
            //Content = new ReactivePropertySlim<ViewModelBase>();
            ChangeContentCommand.Subscribe(n => ChangeContent(n)).AddTo(_compositeDisposable);
        }
        private void ChangeContent(NavigationViewItemBase item)
        {
            try
            {
                if (item is null) return;
                var vm = Activator.CreateInstance((Type)item.Tag);
                if (vm is null) return;
                Content.Value.Dispose();
                Content.Value = (ViewModelBase)vm;
            }
            catch (Exception ex)
            {
                // TODO : Do something.
            }
        }
    }
}
