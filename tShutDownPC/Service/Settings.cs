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
using WPF.Themes;

namespace tShutDownPC.Service
{
    public class Settings : INotifyPropertyChanged
    {
        int ignore_count = 2; 
        #region fields

        private const string SETTING_NAME = "settings.cfg";

        public bool IsUserNotified = false; //indicate is user notified before "shutdown"

        #endregion fields

        #region properties

        /// <summary>
        /// time left before shutdown
        /// </summary>
        private int m_NotificationTime = 10; 
        public int NotificationTime
        {
            get => m_NotificationTime;
            set
            {
                m_NotificationTime = value;
                ShutdownPCTimeByCPU = m_NotificationTime;
                OnPropertyChanged();
            }
        }

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

        #region settings

        /// <summary>
        /// Indicate that when users press close mainwindow it will hide or not in tray
        /// </summary>
        private bool m_IsHideInTray;
        public bool IsHideInTray
        {
            get => m_IsHideInTray;
            set
            {
                m_IsHideInTray = value;
                OnPropertyChanged();
            }
        }

        #endregion settings

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
                ShutdownCounter = 0;

                m_ShutdownPCTimeByTimer = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Seconds to shutdown pc
        /// </summary>
        public int ShutdownCounter;
        public int ShutdownCounterCPU;

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

        private int m_ShutdownPCTimeByCPU = 1;
        public int ShutdownPCTimeByCPU
        {
            get
            {
                return m_ShutdownPCTimeByCPU;
            }
            set
            {
                ShutdownCounterCPU = 0;

                m_ShutdownPCTimeByCPU = value;
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

        #region by day of the week

        /// <summary>
        /// Indicate is shutdown by timer is enabled
        /// </summary>
        private bool m_IsByDayOfTheWeekEnabled;
        public bool IsByDayOfTheWeekEnabled
        {
            get => m_IsByDayOfTheWeekEnabled;
            set
            {
                m_IsByDayOfTheWeekEnabled = value;
                OnPropertyChanged();
            }
        }

        private DateTime m_MondayShutdownTime;
        public DateTime MondayShutdownTime
        {
            get => m_MondayShutdownTime;
            set
            {
                m_MondayShutdownTime = value;
                OnPropertyChanged();
            }
        }

        private DateTime m_TuesdayShutdownTime;
        public DateTime TuesdayShutdownTime
        {
            get => m_TuesdayShutdownTime;
            set
            {
                m_TuesdayShutdownTime = value;
                OnPropertyChanged();
            }
        }

        private DateTime m_WednesdayShutdownTime;
        public DateTime WednesdayShutdownTime
        {
            get => m_WednesdayShutdownTime;
            set
            {
                m_WednesdayShutdownTime = value;
                OnPropertyChanged();
            }
        }
        
        private DateTime m_ThursdayShutdownTime;
        public DateTime ThursdayShutdownTime
        {
            get => m_ThursdayShutdownTime;
            set
            {
                m_ThursdayShutdownTime = value;
                OnPropertyChanged();
            }
        }
        
        private DateTime m_FridayhutdownTime;
        public DateTime FridayhutdownTime
        {
            get => m_FridayhutdownTime;
            set
            {
                m_FridayhutdownTime = value;
                OnPropertyChanged();
            }
        }

        private DateTime m_SaturdayhutdownTime;
        public DateTime SaturdayhutdownTime
        {
            get => m_SaturdayhutdownTime;
            set
            {
                m_SaturdayhutdownTime = value;
                OnPropertyChanged();
            }
        }
        
        private DateTime m_SundayShutdownTime;
        public DateTime SundayShutdownTime
        {
            get => m_SundayShutdownTime;
            set
            {
                m_SundayShutdownTime = value;
                OnPropertyChanged();
            }
        }

        #endregion by day of the week


        #region Themes

        private string[] _Themes ;
        public string[] Themes
        {
            get => _Themes;
            set
            {

                _Themes = value;
              

            }
        }



        private string _SelectedThemes;
        public string SelectedThemes
        {
            get
            {
                return _SelectedThemes;
            }
            set
            {
                if (value== "ExpressionDark" && ignore_count!=0)
                {
                    ignore_count--;
                    return;
                }

                _SelectedThemes = value;
                OnPropertyChanged();
            }
        }
        #endregion
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
