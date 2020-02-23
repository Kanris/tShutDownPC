using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tShutDownPC.Service
{
    public static class SpeakerHelper
    {
        //public static int Time { get; set; } = 10;
        //private static int Counter { get; set; } = 0;

        private const float eps = 0.0001f;

        private static float PrevVolume { get; set; } = 0;
        public static float CurrVolume { get; set; } = 0;

        private static MMDeviceCollection devicesSpeaker = new MMDeviceEnumerator().EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);


        public static bool ComapreAudioNoise(ref int counter, int time)
        {
            foreach (var item in devicesSpeaker)
            {
               
                CurrVolume += item.AudioMeterInformation.MasterPeakValue;
            }

         

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
                return false;
            }

            return false;
        }

        public static double GetVolume()
        {
            double tmp = 0;

            foreach (var item in devicesSpeaker)
            {
                tmp += item.AudioMeterInformation.MasterPeakValue;
            }
            return tmp;
        }
     



    }
}
