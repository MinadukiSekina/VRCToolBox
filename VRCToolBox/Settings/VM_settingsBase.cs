using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Common;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using ModernWpf.Controls;
using VRCToolBox.Settings.NotifySettings;

namespace VRCToolBox.Settings
{
    public class VM_SettingsBase : SettingsViewModelBase
    {
        public ReactivePropertySlim<SettingsViewModelBase> Content { get; } = new ReactivePropertySlim<SettingsViewModelBase>();
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
