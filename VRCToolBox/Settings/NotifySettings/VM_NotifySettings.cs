using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCToolBox.Settings.NotifySettings
{
    public class VM_NotifySettings : SettingsViewModelBase
    {
        public ReactiveProperty<float> NotificationInterval { get; } = new ReactiveProperty<float>();
        public ReactiveProperty<bool> SendToastNotification { get; } = new ReactiveProperty<bool>();
        public VM_NotifySettings() : this(new M_Settings()) { }
        public VM_NotifySettings(M_Settings m_Settings) : base(m_Settings)
        {
            NotificationInterval = _settings.NotificationInterval.ToReactivePropertyAsSynchronized(p => p.Value).AddTo(_compositeDisposable);
            SendToastNotification = _settings.SendToastNotification.ToReactivePropertyAsSynchronized(p => p.Value).AddTo(_compositeDisposable);
        }
    }
}
