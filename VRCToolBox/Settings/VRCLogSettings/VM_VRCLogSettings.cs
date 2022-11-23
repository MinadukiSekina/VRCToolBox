using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using VRCToolBox.Common;

namespace VRCToolBox.Settings.VRCLogSettings
{
    public class VM_VRCLogSettings : SettingsViewModelBase
    {
        public ReactiveProperty<string> VRChatLogPath { get; } = new ReactiveProperty<string>();
        public ReactiveProperty<string> VRChatLogMovedPath { get; } = new ReactiveProperty<string>();
        public ReactiveProperty<float> NotificationInterval { get; } = new ReactiveProperty<float>();
        public ReactiveProperty<bool> MakeVRCLogYearFolder { get; } = new ReactiveProperty<bool>();
        public ReactiveProperty<bool> MakeVRCLogMonthFolder { get; } = new ReactiveProperty<bool>();
        public ReactiveProperty<bool> SendToastNotification { get; } = new ReactiveProperty<bool>();
        public VM_VRCLogSettings() : this(new M_Settings()) { }
        public VM_VRCLogSettings(M_Settings m_Settings) : base(m_Settings)
        {
            VRChatLogPath         = _settings.VRChatLogPath.ToReactivePropertyAsSynchronized(p => p.Value).AddTo(_compositeDisposable);
            VRChatLogMovedPath    = _settings.MovedPath.ToReactivePropertyAsSynchronized(p => p.Value).AddTo(_compositeDisposable);
            NotificationInterval  = _settings.NotificationInterval.ToReactivePropertyAsSynchronized(p => p.Value).AddTo(_compositeDisposable);
            MakeVRCLogYearFolder  = _settings.MakeVRCLogYearFolder.ToReactivePropertyAsSynchronized(p => p.Value).AddTo(_compositeDisposable);
            MakeVRCLogMonthFolder = _settings.MakeVRCLogMonthFolder.ToReactivePropertyAsSynchronized(p => p.Value).AddTo(_compositeDisposable);
            SendToastNotification = _settings.SendToastNotification.ToReactivePropertyAsSynchronized(p => p.Value).AddTo(_compositeDisposable);
        }
    }
}
