﻿using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tShutDownPC.Service
{
    public static class SpeakerHelper
    {
        public static int Time { get; set; } = 10;
        private static int Counter { get; set; } = 0;

        private const float eps = 0.0001f;

        private static float PrevVolume { get; set; } = 0;
        private static float CurrVolume { get; set; } = 0;

        private static MMDeviceCollection devicesSpeaker = new MMDeviceEnumerator().EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);


        public static bool CompareMicrophone()
        {
            foreach (var item in devicesSpeaker)
            {
                Console.WriteLine($": {item.FriendlyName} \r\n: {item.AudioMeterInformation.MasterPeakValue}");
                CurrVolume += item.AudioMeterInformation.MasterPeakValue;
            }

            Console.WriteLine($"PrevVolume: {PrevVolume} ==> CurrVolume:{CurrVolume} Counter:{Counter}");

            if (Math.Abs(PrevVolume - CurrVolume) < eps)
            {
                CurrVolume = 0;
                Counter++;
            }
            else
            {
                PrevVolume = CurrVolume;
                CurrVolume = 0;
                Counter = 0;
            }

            if (Counter >= Time)
            {
                Counter = 0;
                return true;
            }

            return false;
        }

    }
}