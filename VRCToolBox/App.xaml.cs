using System;
using System.IO;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using VRCToolBox.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Toolkit.Uwp.Notifications;

namespace VRCToolBox
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private SplashScreen? splashScreen;
        public App() : base()
        {
            splashScreen = new SplashScreen("images/SplashScreen.png");
            splashScreen.Show(false);
        }
        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                if(CheckIsSecondProcess())
                {
#if !DEBUG
                    Current.Shutdown();
#endif
                }
                else
                {
                    // reference : https://threeshark3.com/unhandled-exception-handling/
                    // UIスレッドの未処理例外で発生
                    DispatcherUnhandledException += OnDispatcherUnhandledException;
                    // UIスレッド以外の未処理例外で発生
                    TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
                    // それでも処理されない例外で発生
                    AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

                    ToastNotificationManagerCompat.OnActivated += ToastNotificationManagerCompat_OnActivated;
                    List<Task> tasks = new List<Task>();
                    tasks.Add(ProgramSettings.Initialize());
                    tasks.Add(RemoveTempDirectoriesAsync());
                    //tasks.Add(Task.Run(() => SetFontStyle()));
                    await Task.WhenAll(tasks);
                    //}
                    tasks.Clear();
                    tasks.Add(Data.SqliteAccess.InitializeAsync());
                    await Task.WhenAll(tasks);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($@"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                Current.Shutdown();
            }
            finally
            {
                splashScreen?.Close(TimeSpan.Zero);
                splashScreen = null;
                await Task.Delay(1000);
            }
        }

        private void ToastNotificationManagerCompat_OnActivated(ToastNotificationActivatedEventArgsCompat e)
        {
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            try
            {
                ToastNotificationManagerCompat.History.Clear();
                ToastNotificationManagerCompat.Uninstall();

                ProgramConst.CancellationTokenSource.Cancel();
                ProgramConst.CancellationTokenSource.Dispose();
            }
            catch (Exception ex)
            {
                // TODO : Do something.
            }
        }

        private bool CheckIsSecondProcess()
        {
#if DEBUG
            return false;
#else
            System.Diagnostics.Process[] ps = System.Diagnostics.Process.GetProcessesByName(nameof(VRCToolBox));
            if (ps.Length > 1)
            {
                MessageBox.Show("既に起動しています。", nameof(VRCToolBox), MessageBoxButton.OK, MessageBoxImage.Information);
                return true;
            }
            return false;
#endif
        }
        private async Task RemoveTempDirectoriesAsync()
        {
            await Task.Run(() => {
                string tempPah = $@"{Path.GetTempPath()}{nameof(VRCToolBox)}";
                if (!Directory.Exists(tempPah)) tempPah = $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\{nameof(VRCToolBox)}\Temp";
                if (!Directory.Exists(tempPah)) return;
                DirectoryInfo tempDirectory = new DirectoryInfo(tempPah);
                foreach (DirectoryInfo dir in tempDirectory.EnumerateDirectories("*", SearchOption.AllDirectories))
                {
                    if ((dir.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                        dir.Attributes = FileAttributes.Normal;

                    foreach (FileInfo file in dir.EnumerateFiles("*", SearchOption.AllDirectories))
                    {
                        if ((file.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                            file.Attributes = FileAttributes.Normal;
                    }
                    dir.Delete(true);
                }
            });
        }
        private void SetFontStyle()
        {
            System.Windows.Media.FontFamily fontFamily = new System.Windows.Media.FontFamily("Meiryo");
            Style style = new Style(typeof(Window));
            style.Setters.Add(new Setter(System.Windows.Controls.Control.FontFamilyProperty, fontFamily));
            FrameworkElement.StyleProperty.OverrideMetadata(typeof(Window), new PropertyMetadata(style));
        }
        private void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Exception exception = e.Exception;
            HandleException(exception);
        }

        private void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            Exception? exception = e.Exception.InnerException;
            HandleException(exception);
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception? exception = e.ExceptionObject as Exception;
            HandleException(exception);
        }

        private void HandleException(Exception? e)
        {
            // ログを送ったり、ユーザーにお知らせしたりする
            MessageBox.Show($@"申し訳ありません。エラーが発生しました。{Environment.NewLine}{e?.ToString()}");
            Environment.Exit(1);
        }
    }
}
