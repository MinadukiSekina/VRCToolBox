using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRCToolBox.Common;
using VRCToolBox.Settings;

namespace VRCToolBox.Main
{
    internal class MainWindowViewModel : ViewModelBase
    {
        private RelayCommand? _openSettingsWindowCommand;
        public RelayCommand OpenSettingsWindowCommand => _openSettingsWindowCommand ??= new RelayCommand(OpenSettingsWindow);

        private void OpenSettingsWindow()
        {
            try
            {
                //WindowManager.ShowOrActivate<SettingsWindow>(this);
            }
            catch (Exception ex)
            {

            }
        }
    }
}
