using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using tShutDownPC.Enums;
using tShutDownPC.Support;

namespace tShutDownPC.Models
{
    public class MainWindowModel : INotifyPropertyChanged
    {
        #region fields

        private Timer m_GlobalTimer; //global timer that check is need to perform "shutdown"

        private PerformanceCounter cpuCounter; //CPU statistic

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
                m_ShutdownPCTimeByTimer = value;
                OnPropertyChanged();
            }
        }

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

        #endregion properties

        #region commands

        /// <summary>
        /// Command to change application language to english
        /// </summary>
        private RelayCommand m_ChangeLanguageToEn;
        public RelayCommand ChangeLanguageToEn => m_ChangeLanguageToEn ?? (m_ChangeLanguageToEn = new RelayCommand(ChangeLocalizationToEn));

        /// <summary>
        /// Command to change application language to russian
        /// </summary>
        private RelayCommand m_ChangeLanguageToRu;
        public RelayCommand ChangeLanguageToRu => m_ChangeLanguageToRu ?? (m_ChangeLanguageToRu = new RelayCommand(ChangeLocalizationToRu));

        #endregion commands

        #region initialize

        public MainWindowModel()
        {
            InitLicense();
            InitVariables();
            InitTimer();
        }

        /// <summary>
        /// Initialzie Global values
        /// </summary>
        private void InitVariables()
        {
            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        }

        /// <summary>
        /// Check/Create license entry
        /// </summary>
        private void InitLicense()
        {
            if (!LicenseCheck.IsLicenseAvailable())
            {
                //open license activation window
            }
        }

        /// <summary>
        /// Init global timer
        /// </summary>
        private void InitTimer()
        {
            m_GlobalTimer = new Timer(1 * 1000); //tick every 1 second
            m_GlobalTimer.Elapsed += M_GlobalTimer_Elapsed; //method to perform
            m_GlobalTimer.Start(); //start global timer
        }

        #endregion initialize

        #region methods

        /// <summary>
        /// Change application language to en
        /// </summary>
        private void ChangeLocalizationToEn(object obj)
        {
            ChangeLanguage.ChangeLanguageTo(Enums.LanguageSettings.EN);
        }

        /// <summary>
        /// Change application language to ru
        /// </summary>
        private void ChangeLocalizationToRu(object obj)
        {
            ChangeLanguage.ChangeLanguageTo(Enums.LanguageSettings.RU);
        }

        private void M_GlobalTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //if shutdown by timer is enabled
            if (IsByTimerEnabled)
            {
                //if timer is expired
                if (ShutdownPCTimeByTimer <= 0)
                {
                    Logger.WriteLog(ShutdownType, ShutdownOptions.Timer); //write log about it
                    ShutdownPC.PerformShutdown(ShutdownType); //perform shutdown base on type

                    m_GlobalTimer.Stop(); //stop timer
                }
                else //timer is not expired
                    ShutdownPCTimeByTimer--; //indicate one tick
            }

            //if shutdown by CPU load is enabled
            if (IsByCpuLoadEnabled)
            {
                //if cpu laod is greater than value
                if (cpuCounter.NextValue() > MaximumThreshold)
                {
                    Logger.WriteLog(ShutdownType, ShutdownOptions.Load); //write log about it
                    ShutdownPC.PerformShutdown(ShutdownType); //perform shutdown base on type

                    m_GlobalTimer.Stop(); //stop timer
                }
            }
        }

        #endregion methods

        #region property changed

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        #endregion property changed
    }
}
