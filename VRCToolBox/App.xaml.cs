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

namespace VRCToolBox
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                if(CheckIsSecondProcess())
                {
                    Current.Shutdown();
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
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            ProgramConst.CancellationTokenSource.Cancel();
            ProgramConst.CancellationTokenSource.Dispose();
        }

        private bool CheckIsSecondProcess()
        {
            System.Diagnostics.Process[] ps = System.Diagnostics.Process.GetProcessesByName(nameof(VRCToolBox));
            if (ps.Length > 1)
            {
                MessageBox.Show("既に起動しています。", nameof(VRCToolBox), MessageBoxButton.OK, MessageBoxImage.Information);
                return true;
            }
            return false;
        }
        private async Task RemoveTempDirectoriesAsync()
        {
            await Task.Run(() => {
                string tempPah = $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\{nameof(VRCToolBox)}\Temp";
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
            style.Setters.Add(new Setter(Window.FontFamilyProperty, fontFamily));
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
