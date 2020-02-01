using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tShutDownPC.Service
{
    public static class LicenseCheck
    {
        /// <summary>
        /// Key to activate product
        /// </summary>
        private const string ACTIVATION_KEY = "HelloWorld";

        /// <summary>
        /// Path to license in registry
        /// </summary>
        private const string PATH_TO_LICENSE = @"Software\PCShutdown\License";

        /// <summary>
        /// Check is license on PC valid
        /// </summary>
        /// <returns>true - license is valid; false - otherwise</returns>
        public static bool IsLicenseAvailable()
        {
            var activationTime = CreateIfNotExists(DateTime.Now); //create registry entry if not exists and write activation value

            //if product is activated or trial time is not ended
            return activationTime == DateTime.MinValue || activationTime.AddDays(14) > DateTime.Now;
        }

        /// <summary>
        /// Activate product with key
        /// </summary>
        /// <param name="input">key to activate product</param>
        /// <returns>true - is key valid; false - otherwise</returns>
        public static bool ActivateLicense(string input)
        {
            //if input is a valid activation key
            if (input == ACTIVATION_KEY)
            {
                var activationTime = CreateIfNotExists(DateTime.MinValue);
                return true;
            }
            else
                return false;
        }

        private static DateTime CreateIfNotExists(DateTime timeToApply)
        {
            var localMachine = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.CurrentUser, RegistryView.Default); //here you specify where exactly you want your entry

            var reg = localMachine.OpenSubKey(PATH_TO_LICENSE, true); //get path to the license key in registry

            //if node in registry is not exists
            if (ReferenceEquals(reg, null))
            {
                reg = localMachine.CreateSubKey(PATH_TO_LICENSE); //create entry in registry
            }

            //if there is no value in registry
            if (reg.GetValue("LicenseKey") == null)
            {
                reg.SetValue("LicenseKey", DateTime.Now); //write current activation time
            }

            DateTime.TryParse(reg.GetValue("LicenseKey").ToString(), out var licenseTime);

            return licenseTime; //return license value in datetime
        }

        private static void ApplyLicenseKey(DateTime timeToWrite)
        {

        }
    }
}
