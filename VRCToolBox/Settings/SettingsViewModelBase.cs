using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace VRCToolBox.Settings
{
    public class SettingsViewModelBase : ViewModelBase
    {
        protected M_Settings _settings;

        public SettingsViewModelBase() : this(new M_Settings()) { }
        public SettingsViewModelBase(M_Settings m_Settings)
        {
            _settings = m_Settings;
            _settings.AddTo(_compositeDisposable);
        }

        
    }
}
