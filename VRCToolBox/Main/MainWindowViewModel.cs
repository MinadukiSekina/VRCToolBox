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
        public RelayCommand MoveAndEditLogAsyncCommand => _moveAndEditLogAsyncCommand ??= new RelayCommand(async () => { _moveAndEditLogAsyncCanExecute = false; await MoveAndEditLogAsync(); _moveAndEditLogAsyncCanExecute = true; }, () => _moveAndEditLogAsyncCanExecute);
        private RelayCommand? _movePhotoAsyncCommand;
        public RelayCommand MovePhotoAsyncCommand => _movePhotoAsyncCommand ??= new RelayCommand(async () => { _movePhotoAsyncCanExecute = false; await MovePhotoAsync(); _movePhotoAsyncCanExecute = true; }, () => _moveAndEditLogAsyncCanExecute);

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
        private bool _updateIsExist;
        public bool UpdateIsExist
        {
            get => _updateIsExist;
            set
            {
                _updateIsExist = value;
                RaisePropertyChanged();
            }
        }
        public MainWindowViewModel()
        {

        }

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
