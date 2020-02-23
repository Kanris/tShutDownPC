using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace tShutDownPC.Service
{
    public class TrayService
    {
        private System.Windows.Forms.NotifyIcon m_TrayIcon 
            = new System.Windows.Forms.NotifyIcon();

        public TrayService()
        {
            try
            {
                var pathToIcon = $@"{Directory.GetCurrentDirectory()}\Resources\clock.ico";

                m_TrayIcon.Icon = new Icon(pathToIcon);
                m_TrayIcon.Visible = true;
                //m_TrayIcon.Click += OnTrayIconShow;

                m_TrayIcon.ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip();
                m_TrayIcon.ContextMenuStrip.Items.Add("Show", null, OnTrayIconShow);
                m_TrayIcon.ContextMenuStrip.Items.Add("Exit", null, OnTrayIconExit);

                App.Current.MainWindow.Closed += MainWindow_Closed; ;
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// Hide tray icon when application is closing
        /// </summary>
        private void MainWindow_Closed(object sender, EventArgs e)
        {
            //if tray icon was created
            if (!ReferenceEquals(m_TrayIcon, null))
                m_TrayIcon.Visible = false; //hide tray icon
        }

        /// <summary>
        /// Display tray notification
        /// </summary>
        public void ShowTrayNotification(string messageToDisplay)
        {
            m_TrayIcon.ShowBalloonTip(5000, "ShutdownPC", messageToDisplay, System.Windows.Forms.ToolTipIcon.Info);
        }

        /// <summary>
        /// Show app
        /// </summary>
        private void OnTrayIconShow(object sender, EventArgs e)
        {
            App.Current.MainWindow.Visibility = Visibility.Visible; //show main window
            App.Current.MainWindow.WindowState = WindowState.Normal; //display on screen
        }

        /// <summary>
        /// Perform app exit
        /// </summary>
        private void OnTrayIconExit(object sender, EventArgs e)
        {
            Application.Current.Shutdown(1); //shutdown application
        }
    }
}
