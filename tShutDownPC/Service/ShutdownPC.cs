using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using tShutDownPC.Service.Enums;

namespace tShutDownPC.Support
{
    public static class ShutdownPC
    {
        #region system32 dll import
        [DllImport("PowrProf.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool SetSuspendState(bool hiberate, bool forceCritical, bool disableWakeEvent);

        [DllImport("user32")]
        public static extern bool ExitWindowsEx(uint uFlags, uint dwReason);
        #endregion system32 dll import

        #region methods

        #region public methods

        /// <summary>
        /// Perform "shutdown" base on provided type
        /// </summary>
        public static void PerformShutdown(ShutdownType shutdownType)
        {
            switch (shutdownType)
            {
                case ShutdownType.Shutdown: //perform pc shutdown
                    ShutDownPC();
                    break;

                case ShutdownType.Reboot: //perform pc reboot
                    RebootPC();
                    break;

                case ShutdownType.Sleep: //put pc in sleep
                    SleepPC();
                    break;

                case ShutdownType.Logout: //logout current user
                    LogoutUser();
                    break;

                case ShutdownType.DisableInternet: //disable internet connection
                    DisableInternet();
                    break;
            }
        }

        #endregion public methods

        #region private methods

        /// <summary>
        /// Shutdown pc with cmd without showing cmd window
        /// </summary>
        private static void ShutDownPC()
        {
            var psi = new ProcessStartInfo("shutdown", "/s /t 0");

            psi.CreateNoWindow = true;
            psi.UseShellExecute = false;

            Process.Start(psi);
        }

        /// <summary>
        /// Reboot pc with cmd without showing cmd window
        /// </summary>
        private static void RebootPC()
        {
            //Process.Start("shutdown", "/r /t 0");

            var psi = new ProcessStartInfo("shutdown", "/r /t 0");

            psi.CreateNoWindow = true;
            psi.UseShellExecute = false;

            Process.Start(psi);
        }

        /// <summary>
        /// Put pc in sleep mode
        /// </summary>
        private static void SleepPC()
        {
            SetSuspendState(false, true, true);
        }

        /// <summary>
        /// Logout current user
        /// </summary>
        private static void LogoutUser()
        {
            ExitWindowsEx(0, 0);
        }

        /// <summary>
        /// Disable user current internet connection
        /// </summary>
        private static void DisableInternet()
        {
            var internet = new ProcessStartInfo()
            {
                FileName = "cmd.exe",
                Arguments = "/C ipconfig /" + "release",
                WindowStyle = ProcessWindowStyle.Hidden
            };
            Process.Start(internet);
        }

        #endregion private methods

        #endregion methods
    }
}
