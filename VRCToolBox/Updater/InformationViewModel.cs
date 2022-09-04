using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VRCToolBox.Common;

namespace VRCToolBox.Updater
{
    public class InformationViewModel : ViewModelBase
    {
        public NotifyTaskCompletion<bool> CheckUpdateExists { get; private set; } = new NotifyTaskCompletion<bool>(Updater.CheckUpdateAsync(new CancellationToken()));
        public string VersionText { get; private set; } = $@"バージョン：{Updater.CurrentVersion}";

        private RelayCommand? _updateCommand;
        public RelayCommand UpdateCommand => _updateCommand ??= new RelayCommand(async () => await UpdateProgram());

        private async Task UpdateProgram()
        {
            bool isUpdateSuccess = await Updater.UpdateProgramAsync(new System.Threading.CancellationToken());
            if (isUpdateSuccess) System.Windows.Application.Current.Shutdown();
        }
    }
}
