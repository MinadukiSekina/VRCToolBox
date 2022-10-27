using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using ModernWpf.Controls;

namespace VRCToolBox.Settings.DataSettings
{
    public class VM_DataSettings : SettingsViewModelBase
    {
        public ReactiveProperty<string> DBDirectoryPath { get; } = new ReactiveProperty<string>();
        public IReadOnlyList<NavigationViewItem> MenuItems { get; private set; } =
         new List<NavigationViewItem>() { new NavigationViewItem() { Icon = new SymbolIcon(Symbol.Contact), Content = "アバター",  IsSelected = true},
                                          new NavigationViewItem() { Icon = new SymbolIcon(Symbol.World), Content = "ワールド" },
                                          new NavigationViewItem() { Icon = new SymbolIcon(Symbol.Tag), Content = "タグ"   },
                                          new NavigationViewItem() { Icon = new SymbolIcon(Symbol.People), Content = "ユーザー"   },
                                         };
        public VM_DataSettings() : this(new M_Settings()) { }
        public VM_DataSettings(M_Settings m_Settings) : base(m_Settings)
        {
            DBDirectoryPath = _settings.DBDirectoryPath.ToReactivePropertyAsSynchronized(p => p.Value).AddTo(_compositeDisposable);
        }
    }
}
