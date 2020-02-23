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

        private static List<string> ACTIVATION_KEY_List = new List<string>() 
        {
            "DBD0A62AD03DD90"
            ,"4610674618BD318"
            ,"BCD2C0AA2860087"
            ,"741739C208DB9D8"
            ,"B9DAB9135A75C76"
            ,"53100969474C448"
            ,"B9140391C63C272"
            ,"1875182DAC2C980"
            ,"0D74C95C455146B"
            ,"1707569D4204625"
            ,"2C94B8467D1594A"
            ,"BCD978D65AB6142"
            ,"A3730604B9604BA"
            ,"65977C754253C0A"
            ,"166D6262D1913CD"
            ,"B335A5DB15044BD"
            ,"8BCC97A93661B24"
            ,"51CB37A1BC6B5C7"
            ,"D3B41325B623010"
            ,"49C347B40585736"
        };

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
            //if (input == ACTIVATION_KEY)
            if (ACTIVATION_KEY_List.Any(x=> String.Equals(x,input)))
            {
                var activationTime = CreateIfNotExists(DateTime.MinValue);
                return true;
            }
            else
                return false;
        }


        public static DateTime GetDateLicense()
        {
            var activationTime = CreateIfNotExists(DateTime.Now);
            return activationTime;
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


            if (ReferenceEquals(reg.GetValue("LicenseKey"), null))
            {
                reg.SetValue("LicenseKey", timeToApply); //write current activation time
            }
            else if (timeToApply == DateTime.MinValue)
            {
                reg.SetValue("LicenseKey", timeToApply); //write current activation time
            }
               
            

            DateTime.TryParse(reg.GetValue("LicenseKey").ToString(), out var licenseTime);

            return licenseTime; //return license value in datetime
        }

        private static void ApplyLicenseKey(DateTime timeToWrite)
        {

        }
    }
}
