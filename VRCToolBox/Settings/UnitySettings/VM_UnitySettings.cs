using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace VRCToolBox.Settings.UnitySettings
{
    public class VM_UnitySettings : SettingsViewModelBase
    {
        public ReactiveProperty<string> UnityProjectDirectory { get; } = new ReactiveProperty<string>();
        public ReactiveProperty<bool> UseVCCUserProjects { get; } = new ReactiveProperty<bool>();
        public ReactiveProperty<string> ProjectBackupsDirectory { get; } = new ReactiveProperty<string>();
        public ReactiveProperty<bool> UseVCCProjectBackupPath { get; } = new ReactiveProperty<bool>();
        public AsyncReactiveCommand WriteListToVCCAsyncCommand { get; } = new AsyncReactiveCommand();
        public VM_UnitySettings() : this(new M_Settings()) { }
        public VM_UnitySettings(M_Settings m_Settings) : base(m_Settings)
        {
            UnityProjectDirectory   = _settings.UnityProjectDirectory.ToReactivePropertyAsSynchronized(d => d.Value).AddTo(_compositeDisposable);
            UseVCCUserProjects      = _settings.UseVCCUserProjects.ToReactivePropertyAsSynchronized(d => d.Value).AddTo(_compositeDisposable);
            ProjectBackupsDirectory = _settings.ProjectBackupsDirectory.ToReactivePropertyAsSynchronized(d => d.Value).AddTo(_compositeDisposable);
            UseVCCProjectBackupPath = _settings.UseVCCProjectBackupPath.ToReactivePropertyAsSynchronized(d => d.Value).AddTo(_compositeDisposable);
            WriteListToVCCAsyncCommand.Subscribe(_ => WriteListToVCCAsync()).AddTo(_compositeDisposable);
        }
        private async Task WriteListToVCCAsync()
        {
            try
            {
                await M_UnityOperator.WriteListToVCCAsync();
                var message = new MessageContent()
                {
                    Button = MessageButton.OK,
                    DefaultResult = MessageResult.OK,
                    Icon = MessageIcon.Information,
                    Text = $@"VCCの設定ファイルへリストを書き込みました。"
                };
                message.ShowMessage();
            }
            catch (Exception ex)
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
