using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace VRCToolBox.Settings
{
    internal class SettingsWindowViewModel : INotifyPropertyChanged
    {
        private ProgramSettings _settings = ProgramSettings.Settings;

        public string VRChatLogPath
        {
            get { return _settings.VRChatLogPath; }
            set
            {
                _settings.VRChatLogPath = value;
                OnPropertyChanged(nameof(VRChatLogPath));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler? handler = PropertyChanged;
            if (handler is null) return;
            handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
