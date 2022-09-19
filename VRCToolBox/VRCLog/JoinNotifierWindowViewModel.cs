using System;
using System.IO;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VRCToolBox.Common;
using VRCToolBox.Settings;
using VRCToolBox.Data;
using XSNotifications;
using XSNotifications.Enum;

namespace VRCToolBox.VRCLog
{
    public class JoinNotifierWindowViewModel : ViewModelBase
    {
        private bool _logWatching = true;
        private CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly CancellationToken _ct;
        private bool _doSomething;
        private string _base64Icon;
        private float _interval = 1.5f;
        private ConcurrentQueue<XSNotification> _notificationQueue = new ConcurrentQueue<XSNotification>();
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
        private RelayCommand? _checkVRCLogCommand;
        public RelayCommand CheckVRCLogCommand => _checkVRCLogCommand ??= new RelayCommand(async () => await CheckVRCLog());
        private RelayCommand? _windowClosedCommand;
        public RelayCommand WindowClosedCommand => _windowClosedCommand ??= new RelayCommand(async () => await WindowClosed());
        private RelayCommand? _startLogWatchingCommand;
        public RelayCommand StartLogWatchingCommand => _startLogWatchingCommand ??= new RelayCommand(StartLogWatching, () => !_logWatching);
        private RelayCommand? _stopLogWatchingCommand;
        public RelayCommand StopLogWatchingCommand => _stopLogWatchingCommand ??= new RelayCommand(StopLogWatching, () => _logWatching);

        public JoinNotifierWindowViewModel()
        {
            _ct = _cts.Token;
            try
            {
                _base64Icon = Pictures.ImageFileOperator.GetBase64Image($@"/Images/icon_128x128.png");
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
            _ = SendNotificationAsync();
            _doSomething = true;
            (System.Diagnostics.Process[] VRCExes, FileInfo? file) targets = (Array.Empty<System.Diagnostics.Process>(), null);
            FileInfo? file = null;
            long count = 0;
            while (_logWatching && !_cts.IsCancellationRequested)
            {
                if (count >= 12) 
                {
                    count = 0;
                }
                else
                {
                    targets = CheckProcessAndLog();
                }

                if (targets.VRCExes.Length == 0 || targets.file is null || (file is not null && targets.file.Name == file.Name)) 
                {
                    await Task.Delay(60000);
                    continue;
                }
                file = targets.file;
                count = 0;
                using (FileStream fileStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (StreamReader sr = new StreamReader(fileStream))
                {
                    bool isFirstWorldEnter = false;
                    bool isSkipNotification = false;
                    string localUserName   = string.Empty;

                    while (_logWatching && !_ct.IsCancellationRequested)
                    {
                        string line = await sr.ReadLineAsync() ?? string.Empty;                        
                        // when line is null, read again after.
                        if (sr.EndOfStream)
                        {
                            await Task.Delay(5000);
                            count++;
                            if (count >= 12)
                            {
                                targets = CheckProcessAndLog();
                                if (targets.VRCExes.Length == 0 || targets.file is null || targets.file.Name != file.Name) break;
                                count = 0;
                            }
                            continue;
                        }
                        // parse line.
                        (WorldVisit? world, UserActivity? activity) = VRCLog.ParseLogLine(line);
                        if (world is not null)
                        {
                            isFirstWorldEnter = true;
                            isSkipNotification = false;
                            UserList.Clear();
                            WorldName = world.WorldName;
                            UserCount = 0;
                            continue;
                        }
                        if (activity is not null)
                        {
                            if (isFirstWorldEnter)
                            {
                                localUserName = activity.UserName;
                                isFirstWorldEnter = false;
                            }
                            if (activity.ActivityType == "Join")
                            {
                                UserCount++;
                            }
                            else
                            {
                                if (activity.UserName == localUserName)
                                {
                                    isSkipNotification = true;
                                }
                                else
                                {
                                    UserCount--;
                                }
                            }
                            UserActivityInfo activityInfo = new UserActivityInfo() { ActivityTime = activity.ActivityTime, ActivityType = activity.ActivityType, UserName = activity.UserName };
                            if(activity.ActivityType == "Join")
                            {
                                using (UserActivityContext context = new UserActivityContext())
                                {
                                    UserActivity? latestActivity = context.UserActivities.Where(u => u.UserName == activity.UserName && u.ActivityType == "Join").
                                                                                        Include(u => u.world).
                                                                                        OrderByDescending(u => u.ActivityTime).
                                                                                        FirstOrDefault();
                                    if (latestActivity is not null)
                                    {
                                        activityInfo.LastMetWorld = latestActivity.world.WorldName;
                                        activityInfo.LastMetTime  = latestActivity.ActivityTime;
                                        TimeSpan intervalTime = activityInfo.ActivityTime - activityInfo.LastMetTime;
                                        activityInfo.LastMetDateInfo = activityInfo.LastMetTime == DateTime.MinValue ? "記録なし" : $"{activityInfo.LastMetTime:yyyy年MM月dd日} ({intervalTime.Days}日前)";
                                    }
                                }
                            }
                            if (!isSkipNotification && activity.UserName != localUserName) 
                            {
                                XSNotification notification = new XSNotification()
                                {
                                    UseBase64Icon = true,
                                    Icon = _base64Icon,
                                    Title = $@"{nameof(VRCToolBox)}",
                                    MessageType = XSMessageType.Notification,
                                    Content = $@"{activityInfo.ActivityType}：{activityInfo.UserName}{Environment.NewLine}前回：{activityInfo.LastMetDateInfo}{Environment.NewLine}場所：{activityInfo.LastMetWorld}",
                                    Timeout = _interval,
                                    SourceApp = $@"{nameof(VRCToolBox)}"
                                };
                                _notificationQueue.Enqueue(notification);
                            }
                            UserList.Add(activityInfo);
                            continue;
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
                    while (!_logWatching && !_ct.IsCancellationRequested) 
                    {
                        if (_notificationQueue.IsEmpty)
                        {
                            await Task.Delay(5000, _ct);
                            continue;
                        }
                        bool result = _notificationQueue.TryDequeue(out XSNotification? notification);
                        if (result && notification is not null)
                        {
                            notifier.SendNotification(notification);
                            await Task.Delay((int)(_interval * 1000), _ct);
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                // TODO : do something.
            }
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
        private async Task WindowClosed()
        {
            _cts.Cancel();
            while (_doSomething)
            {
                await Task.Delay(5000);
                continue;
            }
            _cts.Dispose();
        }
    }
}
