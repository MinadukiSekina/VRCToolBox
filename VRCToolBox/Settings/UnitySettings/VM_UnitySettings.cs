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
        public VM_UnitySettings() : this(new M_Settings()) { }
        public VM_UnitySettings(M_Settings m_Settings) : base(m_Settings)
        {
            UnityProjectDirectory   = _settings.UnityProjectDirectory.ToReactivePropertyAsSynchronized(d => d.Value).AddTo(_compositeDisposable);
            UseVCCUserProjects      = _settings.UseVCCUserProjects.ToReactivePropertyAsSynchronized(d => d.Value).AddTo(_compositeDisposable);
            ProjectBackupsDirectory = _settings.ProjectBackupsDirectory.ToReactivePropertyAsSynchronized(d => d.Value).AddTo(_compositeDisposable);
            UseVCCProjectBackupPath = _settings.UseVCCProjectBackupPath.ToReactivePropertyAsSynchronized(d => d.Value).AddTo(_compositeDisposable);
        }
    }
}
