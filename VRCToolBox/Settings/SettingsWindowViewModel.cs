﻿using System;
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
using System.Security.Cryptography;

namespace VRCToolBox.Settings
{
    internal class SettingsWindowViewModel : ViewModelBase
    {
        public ProgramSettings ProgramSettings => ProgramSettings.Settings;
        public ObservableCollectionEX<AvatarData> AvatarDatas { get; set; } = new ObservableCollectionEX<AvatarData>();
        public ObservableCollectionEX<WorldData> WorldDatas { get; set; } = new ObservableCollectionEX<WorldData>();
        public ObservableCollectionEX<PhotoTag> Tags { get; set; } = new ObservableCollectionEX<PhotoTag>();
        private AvatarData _selectAvatar = new AvatarData();
        public AvatarData SelectAvatar 
        {
            get => _selectAvatar;
            set
            {
                if (value == null) return;
                _selectAvatar = value;
                AvatarAuthor  = _selectAvatar.Author ?? new UserData();
                RaisePropertyChanged();
            }
        }
        private WorldData _selectWorld = new WorldData();
        public WorldData SelectWorld
        {
            get => _selectWorld;
            set
            {
                if (value == null) return;
                _selectWorld = value;
                WorldAuthor  = _selectWorld.Author ?? new UserData();
                RaisePropertyChanged();
            }
        }
        private UserData _worldAuthor = new UserData();
        public UserData WorldAuthor
        {
            get => _worldAuthor;
            set
            {
                _worldAuthor = value;
                RaisePropertyChanged();
            }
        }
        private UserData _avatarAuthor = new UserData();
        public UserData AvatarAuthor
        {
            get => _avatarAuthor;
            set
            {
                _avatarAuthor = value;
                RaisePropertyChanged();
            }
        }
        private PhotoTag _selectPhotoTag = new PhotoTag();
        public PhotoTag SelectPhotoTag
        {
            get => _selectPhotoTag;
            set
            {
                _selectPhotoTag = value;
                RaisePropertyChanged();
            }
        }
        private string _rawPassword = string.Empty;
        public string RawPassword
        { 
        get => _rawPassword;
            set
            {
                _rawPassword = value;
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
        private RelayCommand? _addAvatarDataCommand;
        public RelayCommand AddAvatarDataCommand => _addAvatarDataCommand ??= new RelayCommand(AddAvatarData);
        private RelayCommand? _clearSelectTagCommand;
        public RelayCommand ClearSelectTagCommand => _clearSelectTagCommand ??= new RelayCommand(ClearSelectTag);
        private RelayCommand? _deleteTagAsyncCommand;
        public RelayCommand DeleteTagAsyncCommand => _deleteTagAsyncCommand ??= new RelayCommand(async () => await DeleteTagAsync());
        private RelayCommand? _saveTagAsyncCommand;
        public RelayCommand SaveTagAsyncCommand => _saveTagAsyncCommand ??= new RelayCommand(async () => await SaveTagAsync());
        private RelayCommand? _twitterAsyncCommand;
        public RelayCommand TwitterAsyncCommand => _twitterAsyncCommand ??= new RelayCommand(async () => await TwitterAuthAsync());
        private RelayCommand? _twitterLogoutAsyncCommand;
        public RelayCommand TwitterLogoutAsyncCommand => _twitterLogoutAsyncCommand ??= new RelayCommand(async () => await TwitterLogoutAsync());

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
            //BindingOperations.EnableCollectionSynchronization(AvatarDatas, new object());
            //BindingOperations.EnableCollectionSynchronization(WorldDatas, new object());
            using (PhotoContext photoContext = new PhotoContext())
            {
                Tags.AddRange(photoContext.PhotoTags);
            }
            AvatarDatas.AddRange(data.avatars);
            WorldDatas.AddRange(data.worlds);
        }
        private (List<AvatarData> avatarDatas, List<WorldData> worldDatas) GetAvatarAndWorldData()
        {
            List<AvatarData> avatars = new List<AvatarData>();
            List<WorldData> worlds = new List<WorldData>();
            using(PhotoContext photoContext = new PhotoContext())
            {
                avatars.AddRange(photoContext.Avatars.Include(a => a.Author).ToList());
                worlds.AddRange(photoContext.Worlds.Include(w => w.Author).ToList());
            }
            return (avatars, worlds);
        }
        private void AddAvatarData()
        {
            SelectAvatar = new AvatarData();
        }
        private async Task SaveAvatarData()
        {
            try
            {
                if (SelectAvatar is null || string.IsNullOrWhiteSpace(SelectAvatar.AvatarName)) return;
                using (PhotoContext photoContext = new PhotoContext())
                {
                    if (AvatarAuthor.UserId == Ulid.Empty) 
                    {
                        if (!string.IsNullOrWhiteSpace(AvatarAuthor.VRChatName))
                        {
                            AvatarAuthor.UserId = Ulid.NewUlid();
                            SelectAvatar.AuthorId = AvatarAuthor.UserId;
                            SelectAvatar.Author = AvatarAuthor;
                            photoContext.Users.Add(AvatarAuthor);
                            photoContext.SaveChanges();
                            //photoContext.ChangeTracker.Clear();
                        }
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(AvatarAuthor.VRChatName))
                        {
                            SelectAvatar.AuthorId = Ulid.Empty;
                            SelectAvatar.Author = null;
                        }
                    }
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
                if (!AvatarDatas.Contains(SelectAvatar)) AvatarDatas.Add(SelectAvatar);
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
                    if (WorldAuthor.UserId == Ulid.Empty) 
                    {
                        if (!string.IsNullOrWhiteSpace(WorldAuthor.VRChatName))
                        {
                            WorldAuthor.UserId = Ulid.NewUlid();
                            SelectWorld.AuthorId = WorldAuthor.UserId;
                            SelectWorld.Author = WorldAuthor;
                            photoContext.Users.Add(WorldAuthor);
                            await photoContext.SaveChangesAsync().ConfigureAwait(false);
                            photoContext.ChangeTracker.Clear();
                        }
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(WorldAuthor.VRChatName))
                        {
                            SelectWorld.AuthorId = Ulid.Empty;
                            SelectWorld.Author = null;
                        }
                    }

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
                if (!WorldDatas.Contains(SelectWorld)) WorldDatas.Add(SelectWorld);
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
        private void ClearSelectTag()
        {
            SelectPhotoTag = new PhotoTag();
        }
        private async Task DeleteTagAsync()
        {
            if (SelectPhotoTag is null || SelectPhotoTag.TagId == Ulid.Empty) return;
            using(PhotoContext photoContext = new PhotoContext())
            {
                photoContext.PhotoTags.Remove(SelectPhotoTag);
                await photoContext.SaveChangesAsync().ConfigureAwait(false);
                Tags.Remove(SelectPhotoTag);
            }
        }
        private async Task SaveTagAsync()
        {
            if (SelectPhotoTag is null) return;
            if(Tags.Any(t=>t.TagName == SelectPhotoTag.TagName))
            {
                System.Windows.MessageBox.Show("登録済みです。");
                return;
            }
            using (PhotoContext photoContext = new PhotoContext())
            {
                if(SelectPhotoTag.TagId == Ulid.Empty)
                {
                    SelectPhotoTag.TagId = Ulid.NewUlid();
                    await photoContext.PhotoTags.AddAsync(SelectPhotoTag).ConfigureAwait(false);
                }
                else if(string.IsNullOrWhiteSpace(SelectPhotoTag.TagName))
                {
                    photoContext.PhotoTags.Remove(SelectPhotoTag);
                }
                else
                {
                    photoContext.PhotoTags.Update(SelectPhotoTag);
                }
                await photoContext.SaveChangesAsync().ConfigureAwait(false);
                if(Tags.FirstOrDefault(t => t.TagId == SelectPhotoTag.TagId) is PhotoTag tag)
                {
                    tag = SelectPhotoTag;
                }
                else
                {
                    Tags.Add(SelectPhotoTag);
                }
                ClearSelectTag();
            }
        }
        private async Task TwitterAuthAsync()
        {
            try
            {
                await Twitter.Twitter.AuthenticateAsync();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
            finally
            {
                RawPassword = string.Empty;
            }
        }
        private async Task TwitterLogoutAsync()
        {
            try
            {
                await new Twitter.Twitter().LogoutAsync(RawPassword).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
            finally
            {
                RawPassword = string.Empty;
            }
        }
    }
}
