using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MemoMinder
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static int openWindowCount = 0;

        //protected override void OnStartup(StartupEventArgs e)
        //{
        //    base.OnStartup(e);
        //    MainWindow mainWindow = new MainWindow();
        //    mainWindow.Closed += MainWindow_Closed;
        //    openWindowCount++;
        //}

        //private void MainWindow_Closed(object sender, EventArgs e)
        //{
        //    openWindowCount--;

        //    if (openWindowCount == 0)
        //    {
        //        Shutdown();
        //    }
        //}
    }
}
