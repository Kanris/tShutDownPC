using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tShutDownPC.Service
{
    public static class MicrophoneHelper
    {
        //public static int Time { get; set; } = 10;
        //private static int Counter { get; set; } = 0;

        private const float eps = 0.01f;

        private static float PrevVolume { get; set; } = 0;
        public static float CurrVolume { get; set; } = 0;

        private static MMDeviceCollection devicesMicrophone = new MMDeviceEnumerator().EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);


        public static bool CompareMicrophone(ref int counter, int time)
        {
            foreach (var item in devicesMicrophone)
            {
                //Console.WriteLine($": {item.FriendlyName} \r\n: {item.AudioMeterInformation.MasterPeakValue}");
                CurrVolume += item.AudioMeterInformation.MasterPeakValue;
            }

            // Console.WriteLine($"PrevVolume: {PrevVolume} ==> CurrVolume:{CurrVolume} Counter:{Counter}");

            if (Math.Abs(PrevVolume - CurrVolume) < eps)
            {
                CurrVolume = 0;
                counter++;
            }
            else
            {
                PrevVolume = CurrVolume;
                CurrVolume = 0;
                counter = 0;
            }

            if (counter >= time)
            {
                counter = 0;
                return true;
            }

            return false;
        }

        public static double GetVolume()
        {
            double tmp = 0;

            foreach (var item in devicesMicrophone)
            {
                tmp += item.AudioMeterInformation.MasterPeakValue;
            }
            Console.WriteLine("::" + tmp);
            return tmp;
        }



    }

}
