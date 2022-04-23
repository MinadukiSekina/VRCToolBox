﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using VRCToolBox.Settings;

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
                System.Diagnostics.Process[] ps = System.Diagnostics.Process.GetProcessesByName(nameof(VRCToolBox));
                if(ps.Length > 1)
                {
                    MessageBox.Show("既に起動しています。", nameof(VRCToolBox), MessageBoxButton.OK, MessageBoxImage.Information);
                    Current.Shutdown();
                    return;
                }
               await ProgramSettings.Initialize();
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
    }
}
