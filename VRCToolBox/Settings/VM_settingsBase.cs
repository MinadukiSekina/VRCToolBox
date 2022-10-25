using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Common;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using ModernWpf.Controls;
using VRCToolBox.Settings.VRCLogSettings;
using VRCToolBox.Settings.PicturesSettings;

namespace VRCToolBox.Settings
{
    public class VM_SettingsBase : ViewModelBase
    {
        public ViewModelBase Content { get; set; }
        public List<NavigationViewItem> MenuItems { get; private set; } =
         new List<NavigationViewItem>() { new NavigationViewItem() { Icon = new FontIcon() { FontFamily = ProgramConst.S_segoeMDL2Assets, Glyph = "\xF000" }, Content = "VRChatログ", Tag = typeof(VM_VRCLogSettings) },
                                             new NavigationViewItem() { Icon = new FontIcon() { FontFamily = ProgramConst.S_segoeMDL2Assets, Glyph = "\xEB9F" }, Content = "写真"  , Tag = typeof(VM_PicturesSettings) },
                                             };
        public ReactiveCommand<NavigationViewItemBase> ChangeContentCommand { get; } = new ReactiveCommand<NavigationViewItemBase>();
        private M_Settings Settings { get; }
        public VM_SettingsBase()
        {
            ChangeContentCommand.Subscribe(n => ChangeContent(n)).AddTo(_compositeDisposable);
            Settings = new M_Settings(ProgramSettings.Settings);
            Settings.AddTo(_compositeDisposable);
            Content = new VM_VRCLogSettings(Settings);
        }
        public override void Dispose()
        {
            base.Dispose();
            Content.Dispose();
        }

        private void ChangeContent(NavigationViewItemBase item)
        {
            try
            {
                if (item is null) return;
                var vm = Activator.CreateInstance((Type)item.Tag, Settings);
                if (vm is null) return;
                Content.Dispose();
                Content = (ViewModelBase)vm;
            }
            catch (Exception ex)
            {
                // TODO : Do something.
            }
        }
    }
}
