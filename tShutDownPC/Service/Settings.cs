using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using tShutDownPC.Service.Enums;

namespace tShutDownPC.Service
{
    public class Settings : INotifyPropertyChanged
    {
        #region fields

        private const string SETTING_NAME = "settings.cfg";

        #endregion fields

        #region properties

        /// <summary>
        /// What method of "shutdown" should we perform
        /// </summary>
        private ShutdownType m_ShutdownType;
        public ShutdownType ShutdownType
        {
            get => m_ShutdownType;
            set
            {
                m_ShutdownType = value;
                OnPropertyChanged();
            }
        }

        #region timer

        /// <summary>
        /// Indicate is shutdown by timer is enabled
        /// </summary>
        private bool m_IsByTimerEnabled;
        public bool IsByTimerEnabled
        {
            get => m_IsByTimerEnabled;
            set
            {
                m_IsByTimerEnabled = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Shutdown time for bytimer
        /// </summary>
        private int m_ShutdownPCTimeByTimer = 30;
        public int ShutdownPCTimeByTimer
        {
            get
            {
                return m_ShutdownPCTimeByTimer;
            }
            set
            {
                ShutdownCounter = value;

                m_ShutdownPCTimeByTimer = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Seconds to shutdown pc
        /// </summary>
        public int ShutdownCounter;

        #endregion timer

        #region CPU

        /// <summary>
        /// Indicate is shutdown by cpu load is enabled
        /// </summary>
        private bool m_IsByCpuLoadEnabled;
        public bool IsByCpuLoadEnabled
        {
            get => m_IsByCpuLoadEnabled;
            set
            {
                m_IsByCpuLoadEnabled = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Maximum CPU threshold to perform "shutdown"
        /// </summary>
        private float m_maximumThreshold = 80;
        public float MaximumThreshold
        {
            get => m_maximumThreshold;
            set
            {
                m_maximumThreshold = value;
                OnPropertyChanged();
            }
        }

        #endregion audio

        #region mouse

        /// <summary>
        /// Indicate is shutdown by timer is enabled
        /// </summary>
        private bool m_IsByMouseEnabled;
        public bool IsByMouseEnabled
        {
            get => m_IsByMouseEnabled;
            set
            {
                m_IsByMouseEnabled = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Shutdown time for bytimer
        /// </summary>
        private int m_ShutdownPCTimeByMouse = 30;
        public int ShutdownPCTimeByMouse
        {
            get
            {
                return m_ShutdownPCTimeByMouse;
            }
            set
            {
                ShutdownCounterMouse = value;

                m_ShutdownPCTimeByMouse = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Seconds to shutdown pc
        /// </summary>
        public int ShutdownCounterMouse;

        #endregion mouse

        #region audio

        /// <summary>
        /// Indicate is shutdown by timer is enabled
        /// </summary>
        private bool m_IsByAudioEnabled;
        public bool IsByAudioEnabled
        {
            get => m_IsByAudioEnabled;
            set
            {
                m_IsByAudioEnabled = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Shutdown time for bytimer
        /// </summary>
        private int m_ShutdownPCTimeByAudio = 30;
        public int ShutdownPCTimeByAudio
        {
            get
            {
                return m_ShutdownPCTimeByAudio;
            }
            set
            {
                ShutdownCounterAudio = value;

                m_ShutdownPCTimeByAudio = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Seconds to shutdown pc
        /// </summary>
        public int ShutdownCounterAudio;

        #endregion audio

        #region microphone

        /// <summary>
        /// Indicate is shutdown by timer is enabled
        /// </summary>
        private bool m_IsByMicrophoneEnabled;
        public bool IsByMicrophoneEnabled
        {
            get => m_IsByMicrophoneEnabled;
            set
            {
                m_IsByMicrophoneEnabled = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Shutdown time for bytimer
        /// </summary>
        private int m_ShutdownPCTimeByMicrophone = 30;
        public int ShutdownPCTimeByMicrophone
        {
            get
            {
                return m_ShutdownPCTimeByMicrophone;
            }
            set
            {
                ShutdownCounterMicrophone = value;

                m_ShutdownPCTimeByMicrophone = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Seconds to shutdown pc
        /// </summary>
        public int ShutdownCounterMicrophone;

        #endregion microphone

        /// <summary>
        /// Current application language
        /// </summary>
        public LanguageSettings ApplicationLanguage { set; get; }

        #endregion properties

        #region load methods

        public static Settings GetSettings()
        {
            if (!File.Exists(SETTING_NAME)) //if file is not exists
            {
                return new Settings(); //return empty settings
            }

            //open settings file to save settings
            using (var textWriter = new StreamReader(SETTING_NAME))
            {
                var savedObject = textWriter.ReadLine(); //write serialized settings into file
                return JsonConvert.DeserializeObject<Settings>(savedObject); //get serialized Settings 
            }
        }

        /// <summary>
        /// Save current settings into file
        /// </summary>
        /// <param name="settingsToSave">settings to save</param>
        public static void SaveSettings(Settings settingsToSave)
        {
            if (!File.Exists(SETTING_NAME)) //if file is not exists
            {
                var createdFile = File.Create(SETTING_NAME); //create it
                createdFile.Close(); //close created file to free process
            }

            //open settings file to save settings
            using (var textWriter = new StreamWriter(SETTING_NAME))
            {
                var objectToSave = JsonConvert.SerializeObject(settingsToSave); //get serialized Settings 
                textWriter.WriteLine(objectToSave); //write serialized settings into file
            }
        }
        
        #endregion load methods

        #region property changed

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        #endregion property changed
    }
}
