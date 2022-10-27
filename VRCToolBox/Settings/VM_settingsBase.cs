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
using VRCToolBox.Settings.UnitySettings;
using VRCToolBox.Settings.DataSettings;
using VRCToolBox.Settings.APISettings;
using VRCToolBox.Settings.NotifySettings;

namespace VRCToolBox.Settings
{
    public class VM_SettingsBase : SettingsViewModelBase
    {
        public ReactivePropertySlim<SettingsViewModelBase> Content { get; } = new ReactivePropertySlim<SettingsViewModelBase>();
        public IReadOnlyList<NavigationViewItem> MenuItems { get; private set; } =
         new List<NavigationViewItem>() { new NavigationViewItem() { Icon = new FontIcon() { FontFamily = ProgramConst.S_segoeMDL2Assets, Glyph = "\xEA8F" }, Content = "通知", Tag = typeof(VM_NotifySettings) , IsSelected = true},
                                          new NavigationViewItem() { Icon = new FontIcon() { FontFamily = ProgramConst.S_segoeMDL2Assets, Glyph = "\xF000" }, Content = "VRChatログ", Tag = typeof(VM_VRCLogSettings) },
                                          new NavigationViewItem() { Icon = new FontIcon() { FontFamily = ProgramConst.S_segoeMDL2Assets, Glyph = "\xEB9F" }, Content = "写真"  , Tag = typeof(VM_PicturesSettings) },
                                          new NavigationViewItem() { Icon = new FontIcon() { FontFamily = ProgramConst.S_segoeMDL2Assets, Glyph = "\xECAA" }, Content = "Unity"  , Tag = typeof(VM_UnitySettings) },
                                          new NavigationViewItem() { Icon = new FontIcon() { FontFamily = ProgramConst.S_segoeMDL2Assets, Glyph = "\xF156" }, Content = "データ"  , Tag = typeof(VM_DataSettings) },
                                          new NavigationViewItem() { Icon = new SymbolIcon(Symbol.Switch), Content = "アプリ連携"  , Tag = typeof(VM_APISettings) },
                                         };
        public ReactiveCommand<NavigationViewItemBase> ChangeContentCommand { get; } = new ReactiveCommand<NavigationViewItemBase>();
        public AsyncReactiveCommand SaveSettingsCommand { get; } = new AsyncReactiveCommand();
        public VM_SettingsBase() : this(new M_Settings(ProgramSettings.Settings))
        {
        }
        public VM_SettingsBase(M_Settings m_Settings) : base(m_Settings)
        {
            ChangeContentCommand.Subscribe(n => ChangeContent(n)).AddTo(_compositeDisposable);
            SaveSettingsCommand.Subscribe(_ => SaveSettings()).AddTo(_compositeDisposable);
            Content.Value = new VM_NotifySettings(_settings);
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
                var vm = Activator.CreateInstance((Type)item.Tag, _settings);
                if (vm is null) return;
                Content.Value.Dispose();
                Content.Value = (SettingsViewModelBase)vm;
            }
            catch (Exception ex)
            {
                // TODO : Do something.
            }
        }
        private async Task SaveSettings()
        {
            try
            {
                await _settings.SaveSettingsAsync();
                var message = new MessageContent()
                {
                    Button = MessageButton.OK,
                    DefaultResult = MessageResult.OK,
                    Icon = MessageIcon.Information,
                    Text = $@"設定を保存しました。{Environment.NewLine}データベースの保存場所を変更した場合は、アプリを再起動してください。"
                };
                message.ShowMessage();
            }
            catch(Exception ex)
            {
                var message = new MessageContent()
                {
                    Button = MessageButton.OK,
                    DefaultResult = MessageResult.OK,
                    Icon = MessageIcon.Exclamation,
                    Text = $@"申し訳ありません。エラーが発生しました。{Environment.NewLine}{ex.Message}"
                };
                message.ShowMessage();
            }
        }
    }
}
