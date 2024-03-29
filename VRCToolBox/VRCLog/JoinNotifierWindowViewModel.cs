﻿using System;
using System.IO;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Toolkit.Uwp.Notifications;
using VRCToolBox.Common;
using VRCToolBox.Settings;
using VRCToolBox.Data;
using XSNotifications;
using XSNotifications.Enum;
using VRCToolBox.VRCLog.Analyse.Model;

namespace VRCToolBox.VRCLog
{
    public class JoinNotifierWindowViewModel : ViewModelBase
    {
        private bool _logWatching = true;
        private CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly CancellationToken _ct;
        private bool _doSomething;
        private string _base64Icon;
        private ConcurrentQueue<UserActivityInfo> _notificationQueue = new ConcurrentQueue<UserActivityInfo>();
        public ObservableCollectionEX<UserActivityInfo> UserList { get; } = new ObservableCollectionEX<UserActivityInfo>();
        private long _userCount;
        public long UserCount
        {
            get => _userCount;
            set
            {
                _userCount = value;
                RaisePropertyChanged();
            }
        }
        private string _worldName = string.Empty;
        public string WorldName
        {
            get => _worldName;
            set
            {
                _worldName = value;
                RaisePropertyChanged();
            }
        }
        private bool _isStoppedNotification;
        public bool IsStoppedNotification
        {
            get => _isStoppedNotification;
            set
            {
                _isStoppedNotification = value;
                if (_isStoppedNotification)
                {
                    _notificationQueue.Clear();
                }
                else
                {
                    _ = SendNotificationAsync();
                }
                RaisePropertyChanged();
            }
        }
        private RelayCommand? _checkVRCLogCommand;
        public RelayCommand CheckVRCLogCommand => _checkVRCLogCommand ??= new RelayCommand(async () => await CheckVRCLog());
        private RelayCommand? _windowClosedCommand;
        public RelayCommand WindowClosedCommand => _windowClosedCommand ??= new RelayCommand(WindowClosed);
        private RelayCommand? _startLogWatchingCommand;
        public RelayCommand StartLogWatchingCommand => _startLogWatchingCommand ??= new RelayCommand(StartLogWatching, () => !_logWatching);
        private RelayCommand? _stopLogWatchingCommand;
        public RelayCommand StopLogWatchingCommand => _stopLogWatchingCommand ??= new RelayCommand(StopLogWatching, () => _logWatching);

        public JoinNotifierWindowViewModel()
        {
            _ct = _cts.Token;
            try
            {
                _base64Icon = Pictures.ImageFileOperator.GetBase64Image(ProgramConst.IconImage);
            }
            catch (Exception ex)
            {
                _base64Icon = string.Empty;
            }
        }
        private void StopLogWatching()
        {
            _logWatching = false;
            _doSomething = false;
        }
        private void StartLogWatching()
        {
            _logWatching = true;
            _doSomething = true;
            _ = CheckVRCLog();
        }
        private async Task CheckVRCLog()
        {
            Task task = SendNotificationAsync();
            _doSomething = true;
            (System.Diagnostics.Process[] VRCExes, FileInfo? file) targets = (Array.Empty<System.Diagnostics.Process>(), null);
            FileInfo? file = null;
            long count = 0;
            
            int milisecondsDelay = 3000;
            int oneMinuteMiliseconds = 60000;
            int maxCount = oneMinuteMiliseconds / milisecondsDelay;

            while (_logWatching && !_cts.IsCancellationRequested)
            {
                if (count >= maxCount) 
                {
                    count = 0;
                }
                else
                {
                    targets = CheckProcessAndLog();
                }

                if (targets.VRCExes.Length == 0 || targets.file is null || (file is not null && targets.file.Name == file.Name)) 
                {
                    await Task.Delay(oneMinuteMiliseconds);
                    continue;
                }
                file = targets.file;
                count = 0;
                using (FileStream fileStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (StreamReader sr = new StreamReader(fileStream))
                {
                    bool isSkipNotification = false;
                    string localUserName    = string.Empty;

                    while (_logWatching && !_ct.IsCancellationRequested)
                    {
                        string line = await sr.ReadLineAsync() ?? string.Empty;                        
                        // when line is null, read again after.
                        if (sr.EndOfStream)
                        {
                            await Task.Delay(milisecondsDelay);
                            count++;
                            if (count >= maxCount)
                            {
                                targets = CheckProcessAndLog();
                                if (targets.VRCExes.Length == 0 || targets.file is null || targets.file.Name != file.Name) break;
                                count = 0;
                            }
                            continue;
                        }

                        // parse line.
                        var result = VRCLogParser.ParseLogLine(line);
                        if (result == null) continue;

                        // ワールドに入った時
                        if (!string.IsNullOrEmpty(result.WorldName))
                        {
                            _notificationQueue.Clear();
                            UserList.Clear();
                            WorldName = result.WorldName;
                            UserCount = 0;
                            isSkipNotification = false;
                            continue;
                        }
                        // プレイヤーのJoinもしくはLeft時
                        if (!string.IsNullOrEmpty(result.PlayerName))
                        {
                            // 行動情報の取得と設定
                            var activityInfo = GetAndSetUserActivity(result);
                            UserList.Add(activityInfo);

                            switch (result.Action)
                            {
                                case E_ActivityType.Join:
                                    UserCount++;
                                    if (result.IsLocal)
                                    {
                                        // ローカルプレイヤーのJoin前は通知しない
                                        _notificationQueue.Clear();
                                        // ローカルプレイヤーの名前を保持
                                        localUserName = result.PlayerName;
                                        continue;
                                    }
                                    if (!IsStoppedNotification && !isSkipNotification) _notificationQueue.Enqueue(activityInfo);
                                    break;

                                case E_ActivityType.Left:
                                    UserCount--;
                                    if (result.PlayerName == localUserName)
                                    {
                                        // ローカルプレイヤーのLeft後は通知しない
                                        _notificationQueue.Clear();
                                        isSkipNotification = true;
                                        continue;
                                    }
                                    if (!IsStoppedNotification && !isSkipNotification) _notificationQueue.Enqueue(activityInfo);
                                    break;

                                default: break;

                            }
                        }
                    }
                }
            }
            _doSomething = false;
        }
        private async Task SendNotificationAsync()
        {
            try
            {
                using (XSNotifier notifier = new XSNotifier())
                {
                    while (_logWatching && !_ct.IsCancellationRequested) 
                    {
                        if (IsStoppedNotification) break;
                        if (_notificationQueue.IsEmpty)
                        {
                            await Task.Delay(1000, _ct);
                            continue;
                        }
                        bool result = _notificationQueue.TryDequeue(out UserActivityInfo? activityInfo);
                        if (result && activityInfo is not null)
                        {
                            string content = $@"{activityInfo.ActivityType}：{activityInfo.UserName}";
                            if (activityInfo.ActivityType == "Join") content += $@"{Environment.NewLine}前回：{activityInfo.LastMetDateInfo}{Environment.NewLine}場所：{activityInfo.LastMetWorld}";
                            XSNotification notification = new XSNotification()
                            {
                                UseBase64Icon = true,
                                Icon = _base64Icon,
                                Title = $@"{nameof(VRCToolBox)}",
                                MessageType = XSMessageType.Notification,
                                Content = content,
                                Timeout = ProgramSettings.Settings.NotificationInterval,
                                SourceApp = $@"{nameof(VRCToolBox)}"
                            };
                            // Send notification to XSOverlay.
                            notifier.SendNotification(notification);
                            // Send notification to Windows.
                            if (ProgramSettings.Settings.SendToastNotification)
                            {
                                new ToastContentBuilder().AddText($@"{activityInfo.UserName}  {activityInfo.ActivityType}!")
                                                         .AddText($@"{activityInfo.ActivityTime: H:m:s}  {activityInfo.ActivityType}  {activityInfo.UserName}")
                                                         .AddCustomTimeStamp(activityInfo.ActivityTime)
                                                         .Show();
                            }
                            await Task.Delay((int)(ProgramSettings.Settings.NotificationInterval * 1000), _ct);
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                // TODO : do something.
            }
        }

        private UserActivityInfo GetAndSetUserActivity(IParseLogResult result)
        {
            var activityInfo = new UserActivityInfo() { ActivityTime = result.Timestamp, ActivityType = result.Action.ToString(), UserName = result.PlayerName ?? string.Empty };
            if (result.Action != E_ActivityType.Join || result.IsLocal) return activityInfo;
            
            using (UserActivityContext context = new UserActivityContext())
            {
                UserActivity? latestActivity = context.UserActivities.Where(u => u.UserName == activityInfo.UserName && u.ActivityType == "Join")
                                                                     .Include(u => u.world)
                                                                     .OrderByDescending(u => u.ActivityTime)
                                                                     .FirstOrDefault();
                if (latestActivity is null) return activityInfo;
                
                activityInfo.LastMetWorld = latestActivity.world.WorldName;
                activityInfo.LastMetTime  = latestActivity.ActivityTime;

                var intervalTime = activityInfo.ActivityTime - activityInfo.LastMetTime;
                activityInfo.LastMetDateInfo = activityInfo.LastMetTime <= ProgramConst.MinimumDate ? "記録なし" : $"{activityInfo.LastMetTime:yyyy年MM月dd日} ({intervalTime.Days}日前)";
            }
            return activityInfo;
        }

        private (System.Diagnostics.Process[] VRCprocesses, FileInfo? logFile) CheckProcessAndLog()
        {
            System.Diagnostics.Process[] VRCExes = System.Diagnostics.Process.GetProcessesByName("VRChat");
            FileInfo? file = new DirectoryInfo(ProgramSettings.Settings.VRChatLogPath).EnumerateFiles("*Log*.txt", SearchOption.AllDirectories).
                                                                                                    Where(f => !string.IsNullOrWhiteSpace(f.DirectoryName) && !f.DirectoryName.Contains(ProgramSettings.Settings.MovedPath)).
                                                                                                    OrderByDescending(f => f.LastWriteTime).
                                                                                                    FirstOrDefault();
            return (VRCExes, file);
        }
        private void WindowClosed()
        {
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}
