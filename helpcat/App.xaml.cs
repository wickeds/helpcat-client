using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using WickedFramework.Tools;
using System.Diagnostics;
using System.IO;

namespace helpcat
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            this.ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown;

            if (helpcat.Properties.Settings.Default.UpgradeRequired)
                helpcat.Properties.Settings.Default.Upgrade();

            if (CommandLineArgs.GetKey("waitforexit"))
            {
                try
                {
                    Process proc = Process.GetProcessById(CommandLineArgs.GetValue<int>("waitforexit"));
                    proc.WaitForExit();
                }
                catch (ArgumentException)
                {
                    // do nothing, process already died
                }
            }

            if (CommandLineArgs.GetKey("cleanupinstaller"))
            {
                try
                {
                    Directory.Delete(CommandLineArgs.GetValue<string>("cleanupinstaller"), true);
                }
                catch
                {
                    // do nothing, just leave the battlefield
                }
            }

            LoginWindow loginWindow = new LoginWindow();
            loginWindow.ShowDialog();
        }
    }
}
