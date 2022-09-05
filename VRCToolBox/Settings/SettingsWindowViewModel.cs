using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Data;
using Microsoft.EntityFrameworkCore;
using VRCToolBox.Common;
using VRCToolBox.Data;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace VRCToolBox.Settings
{
    internal class SettingsWindowViewModel : ViewModelBase
    {
        public ProgramSettings ProgramSettings => ProgramSettings.Settings;
        public ObservableCollectionEX<AvatarData> AvatarDatas { get; set; } = new ObservableCollectionEX<AvatarData>();
        public ObservableCollectionEX<WorldData> WorldDatas { get; set; } = new ObservableCollectionEX<WorldData>();
        private AvatarData _selectAvatar = new AvatarData();
        public AvatarData SelectAvatar 
        {
            get => _selectAvatar;
            set
            {
                _selectAvatar = value;
                RaisePropertyChanged();
            }
        }
        private WorldData _selectWorld = new WorldData();
        public WorldData SelectWorld
        {
            get => _selectWorld;
            set
            {
                _selectWorld = value;
                RaisePropertyChanged();
            }
        }
        private RelayCommand? _saveAvatarDataCommand;
        public RelayCommand SaveAvatarDataCommand => _saveAvatarDataCommand ??= new RelayCommand(async() => await SaveAvatarData());
        private RelayCommand? _deleteAvatarDataCommand;
        public RelayCommand DeleteAvatarDataCommand => _deleteAvatarDataCommand ??= new RelayCommand(async() => await DeleteAvatarData());
        private RelayCommand? _saveWorldrDataCommand;
        public RelayCommand SaveWorldrDataCommand => _saveWorldrDataCommand ??= new RelayCommand(async () => await SaveWorldData());
        private RelayCommand? _initializeAsyncCommand;
        public RelayCommand InitializeAsyncCommand => _initializeAsyncCommand ??= new RelayCommand(async() => await InitializeAsync());
        private RelayCommand? _copyWorldNameCommand;
        public RelayCommand CopyWorldNameCommand => _copyWorldNameCommand ??= new RelayCommand(CopyWorldName);

        public SettingsWindowViewModel()
        {
            //using(PhotoContext photoContext = new PhotoContext())
            //{
            //    AvatarDatas.AddRange(photoContext.Avatars);
            //}
        }
        private async Task InitializeAsync()
        {
            (List<AvatarData> avatars, List<WorldData> worlds) data = await Task.Run(() => GetAvatarAndWorldData());
            BindingOperations.EnableCollectionSynchronization(AvatarDatas, new object());
            BindingOperations.EnableCollectionSynchronization(WorldDatas, new object());
            AvatarDatas.AddRange(data.avatars);
            WorldDatas.AddRange(data.worlds);
        }
        private (List<AvatarData> avatarDatas, List<WorldData> worldDatas) GetAvatarAndWorldData()
        {
            List<AvatarData> avatars = new List<AvatarData>();
            List<WorldData> worlds = new List<WorldData>();
            using(PhotoContext photoContext = new PhotoContext())
            {
                avatars.AddRange(photoContext.Avatars);
                worlds.AddRange(photoContext.Worlds);
            }
            return (avatars, worlds);
        }
        private async Task SaveAvatarData()
        {
            try
            {
                if (SelectAvatar is null || string.IsNullOrWhiteSpace(SelectAvatar.AvatarName)) return;
                using (PhotoContext photoContext = new PhotoContext())
                {
                    if (SelectAvatar.AvatarId == Ulid.Empty)
                    {
                        SelectAvatar.AvatarId = Ulid.NewUlid();
                        photoContext.Avatars.Add(SelectAvatar);
                    }
                    else
                    {
                        photoContext.Avatars.Update(SelectAvatar);
                    }
                    await photoContext.SaveChangesAsync();
                }
                AvatarDatas.Add(SelectAvatar);
                System.Windows.MessageBox.Show("保存しました。");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }
        private async Task DeleteAvatarData()
        {
            try
            {
                if (SelectAvatar is null || string.IsNullOrWhiteSpace(SelectAvatar.AvatarName) || SelectAvatar.AvatarId == Ulid.Empty) return;
                using (PhotoContext photoContext = new PhotoContext())
                {
                    photoContext.Avatars.Remove(SelectAvatar);
                    await photoContext.SaveChangesAsync();
                }
                AvatarDatas.Remove(SelectAvatar);
                System.Windows.MessageBox.Show("削除しました。");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }
        private async Task SaveWorldData()
        {
            try
            {
                if (SelectWorld is null || string.IsNullOrWhiteSpace(SelectWorld.WorldName)) return;
                using (PhotoContext photoContext = new PhotoContext())
                {
                    if (SelectWorld.WorldId == Ulid.Empty)
                    {
                        SelectWorld.WorldId = Ulid.NewUlid();
                        photoContext.Worlds.Add(SelectWorld);
                    }
                    else
                    {
                        photoContext.Worlds.Update(SelectWorld);
                    }
                    await photoContext.SaveChangesAsync();
                }
                WorldDatas.Add(SelectWorld);
                System.Windows.MessageBox.Show("保存しました。");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }
        private void CopyWorldName()
        {
            try
            {
                if (SelectWorld is null || string.IsNullOrWhiteSpace(SelectWorld.WorldName)) return;
                System.Windows.Clipboard.SetText(SelectWorld.WorldName);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }
    }
}
