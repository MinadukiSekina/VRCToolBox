using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading.Tasks;
using VRCToolBox.Common;
using VRCToolBox.Settings;
using VRCToolBox.Pictures;

namespace VRCToolBox.Main
{
    internal class MainWindowViewModel : ViewModelBase
    {
        private RelayCommand? _openSettingsWindowCommand;
        public RelayCommand OpenSettingsWindowCommand => _openSettingsWindowCommand ??= new RelayCommand(OpenSettingsWindow);
        private RelayCommand? _moveAndEditLogAsyncCommand;
        public RelayCommand MoveAndEditLogAsyncCommand => _moveAndEditLogAsyncCommand ??= new RelayCommand(async () => { MoveAndEditLogAsyncCanExecute = false; await MoveAndEditLogAsync(); MoveAndEditLogAsyncCanExecute = true; }, () => MoveAndEditLogAsyncCanExecute);
        private RelayCommand? _movePhotoAsyncCommand;
        public RelayCommand MovePhotoAsyncCommand => _movePhotoAsyncCommand ??= new RelayCommand(async () => { MovePhotoAsyncCanExecute = false; await MovePhotoAsync(); MovePhotoAsyncCanExecute = true; }, () => MovePhotoAsyncCanExecute);

        private bool _moveAndEditLogAsyncCanExecute = true;
        public bool MoveAndEditLogAsyncCanExecute
        {
            get => _moveAndEditLogAsyncCanExecute;
            set
            {
                _moveAndEditLogAsyncCanExecute = value;
                RaisePropertyChanged();
            }
        }
        private bool _movePhotoAsyncCanExecute = true;
        public bool MovePhotoAsyncCanExecute
        {
            get => _movePhotoAsyncCanExecute;
            set
            {
                _movePhotoAsyncCanExecute = value;
                RaisePropertyChanged();
            }
        }
        public NotifyTaskCompletion<bool> CheckUpdateExists { get; private set; } = new NotifyTaskCompletion<bool>(Updater.Updater.CheckUpdateAsync(new System.Threading.CancellationToken()));

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
        private async Task MoveAndEditLogAsync()
        {
            try
            {
                await VRCLog.VRCLog.CopyAndEdit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private async Task MovePhotoAsync()
        {
            try
            {
                await Task.Run(() => PicturesOrganizer.OrganizePictures());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
