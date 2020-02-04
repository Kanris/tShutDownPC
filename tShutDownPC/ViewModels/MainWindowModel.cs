using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Timers;
using System.Windows;
using tShutDownPC.Service;
using tShutDownPC.Service.Enums;

namespace tShutDownPC.ViewModels
{
    public class MainWindowModel : INotifyPropertyChanged
    {
        #region fields

        private Timer m_GlobalTimer; //global timer that check is need to perform "shutdown"

        private PerformanceCounter cpuCounter; //CPU statistic

        private TrayService m_TrayService;

        #endregion fields

        #region properties

        /// <summary>
        /// Application settings
        /// </summary>
        private Settings m_ApplicationSettings;
        public Settings ApplicationSettings
        {
            get => m_ApplicationSettings;
            set
            {
                m_ApplicationSettings = value;
                OnPropertyChanged();
            }
        }

        #endregion properties

        #region commands

        /// <summary>
        /// Command to change application language to english
        /// </summary>
        private RelayCommand m_ChangeLanguageToEnCommand;
        public RelayCommand ChangeLanguageToEnCommand => m_ChangeLanguageToEnCommand ?? (m_ChangeLanguageToEnCommand = new RelayCommand(ChangeLocalizationToEn));

        /// <summary>
        /// Command to change application language to russian
        /// </summary>
        private RelayCommand m_ChangeLanguageToRuCommand;
        public RelayCommand ChangeLanguageToRuCommand => m_ChangeLanguageToRuCommand ?? (m_ChangeLanguageToRuCommand = new RelayCommand(ChangeLocalizationToRu));

        /// <summary>
        /// Command to open license window
        /// </summary>
        private RelayCommand m_OpenLicenseWindowCommand;
        public RelayCommand OpenLicenseWindowCommand => m_OpenLicenseWindowCommand ?? (m_OpenLicenseWindowCommand = new RelayCommand(OpenLicenseDialog));

        /// <summary>
        /// Command to check license when window loaded
        /// </summary>
        private RelayCommand m_LoadedCommand;
        public RelayCommand LoadedCommand => m_LoadedCommand ?? (m_LoadedCommand = new RelayCommand(CheckLicense));

        /// <summary>
        /// Command to save settings into settings file
        /// </summary>
        private RelayCommand m_SaveSettingsCommand;
        public RelayCommand SaveSettingsCommand => m_SaveSettingsCommand ?? (m_SaveSettingsCommand = new RelayCommand(SaveSettings));

        #endregion commands

        #region initialize
        /// <summary>
        /// Initialzie Global values
        /// </summary>
        private void InitVariables()
        {
            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
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

        /// <summary>
        /// Init and show tray icon
        /// </summary>
        private void InitTray()
        {
            m_TrayService = new TrayService(); //initialize tray icon

            Application.Current.MainWindow.Closing += new CancelEventHandler(OnMainWindowClosing); //subscribe to main window closing
        }

        /// <summary>
        /// Init settings
        /// </summary>
        private void InitiSettings()
        {
            ApplicationSettings = Settings.GetSettings(); //get saved settings

            ChangeLanguage.ChangeLanguageTo(ApplicationSettings.ApplicationLanguage); //change application settings
        }

        #endregion initialize

        #region methods

        /// <summary>
        /// When user tries to close main window
        /// </summary>
        private void OnMainWindowClosing(object sender, CancelEventArgs e)
        {
            //if application should hide in tray
            if (ApplicationSettings.IsHideInTray)
            {
                e.Cancel = true;
                App.Current.MainWindow.Visibility = Visibility.Hidden;

                m_TrayService?.ShowTrayNotification("Application is in tray");
            }
        }

        /// <summary>
        /// Change application language to en
        /// </summary>
        private void ChangeLocalizationToEn(object obj)
        {
            ApplicationSettings.ApplicationLanguage = Service.Enums.LanguageSettings.EN;
            ChangeLanguage.ChangeLanguageTo(ApplicationSettings.ApplicationLanguage);
        }

        /// <summary>
        /// Change application language to ru
        /// </summary>
        private void ChangeLocalizationToRu(object obj)
        {
            ApplicationSettings.ApplicationLanguage = Service.Enums.LanguageSettings.RU;
            ChangeLanguage.ChangeLanguageTo(ApplicationSettings.ApplicationLanguage);
        }

        /// <summary>
        /// Opens license dialog
        /// </summary>
        private void OpenLicenseDialog(object obj)
        {
            var dialogViewModel = new DialogWindowModel(); //create viewmodel for dialog window

            var dialogResult = DialogService.OpenDialog(dialogViewModel);

            //if user haven't activate product in dialog window AND he has not available license
            if (dialogResult != DialogResult.Yes && !LicenseCheck.IsLicenseAvailable())
            {
                Application.Current.Shutdown(); //close application
            }
        }

        /// <summary>
        /// Check is license expired; and if it's expired open license activation dialog
        /// </summary>
        private void CheckLicense(object obj)
        {
            //check is license is license expired
            if (!LicenseCheck.IsLicenseAvailable())
            {
                OpenLicenseDialog(obj); //open license activation dialog
            }

            InitiSettings(); //initialize global settings            
            InitVariables(); //initialize global valuse
            InitTimer(); //initialize and start global timer for shutdown
            InitTray(); //init tray icon
        }

        /// <summary>
        /// Save settings into file
        /// </summary>
        private void SaveSettings(object obj)
        {
            Settings.SaveSettings(ApplicationSettings); //save current settings
        }

        /// <summary>
        /// Write log and perform choosen shutdown
        /// </summary>
        /// <param name="shutdownOptions">when shutdown occures</param>
        private void PerformShutdown(ShutdownOptions shutdownOptions)
        {
            Logger.WriteLog(ApplicationSettings.ShutdownType, shutdownOptions); //write log about shutdown
            ShutdownPC.PerformShutdown(ApplicationSettings.ShutdownType); //perform shutdown base on type

            m_GlobalTimer.Stop(); //stop timer

            ApplicationSettings.IsUserNotified = false;
        }

        /// <summary>
        /// Check is pc will be shutdown soon
        /// </summary>
        /// <param name="secondsToShutdownLeft">time left</param>
        /// <returns>is pc will be shutdown soon</returns>
        private bool IsCurrentShutdown(int secondsToShutdownLeft)
        {
            return ApplicationSettings.IsUserNotified = secondsToShutdownLeft == ApplicationSettings.NotificationTime;
        }

        private void NotifyUserAboutShutdown(bool isShutdownSoon)
        {
            if (isShutdownSoon)
            {
                ApplicationSettings.IsUserNotified = true;
                m_TrayService.ShowTrayNotification($"{ApplicationSettings.ShutdownType} in {ApplicationSettings.NotificationTime} seconds");
            }
        }

        /// <summary>
        /// Global timer that can trigger shutdown
        /// </summary>
        private void M_GlobalTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var isShutdownSoon = false;

            //if shutdown by timer is enabled
            if (ApplicationSettings.IsByTimerEnabled)
            {
                //if timer is expired
                if (ApplicationSettings.ShutdownCounter >= ApplicationSettings.ShutdownPCTimeByTimer)
                {
                    PerformShutdown(ShutdownOptions.Timer); //write log about shutdown and perform it
                }
                else //timer is not expired
                    ApplicationSettings.ShutdownCounter++; //indicate one tick

                isShutdownSoon = IsCurrentShutdown(ApplicationSettings.ShutdownPCTimeByTimer - ApplicationSettings.ShutdownCounter); //check is shutdown will occure soon
            }

            //if shutdown by CPU load is enabled
            if (ApplicationSettings.IsByCpuLoadEnabled)
            {
                //if cpu laod is greater than value
                if (cpuCounter.NextValue() > ApplicationSettings.MaximumThreshold)
                {
                    PerformShutdown(ShutdownOptions.Load); //write log about shutdown and perform it
                }
            }

            //is shutdown by mouse inactivity enabled
            if (ApplicationSettings.IsByMouseEnabled)
            {
                //check is mouse still inactive
                if (MouseHelper.ComparePoints(ref ApplicationSettings.ShutdownCounterMouse, ApplicationSettings.ShutdownPCTimeByMouse))
                {
                    PerformShutdown(ShutdownOptions.Mouse); //write log about shutdown and perform it
                }

                isShutdownSoon = IsCurrentShutdown(ApplicationSettings.ShutdownPCTimeByMouse - ApplicationSettings.ShutdownCounterMouse); //check is shutdown will occure soon
            }

            //is shutdown by audio enabled
            if (ApplicationSettings.IsByAudioEnabled)
            {
                //check is audio output is inactive
                if (SpeakerHelper.ComapreAudioNoise(ref ApplicationSettings.ShutdownCounterAudio, ApplicationSettings.ShutdownPCTimeByAudio))
                {
                    PerformShutdown(ShutdownOptions.Audio); //write log about shutdown and perform it
                }

                isShutdownSoon = IsCurrentShutdown(ApplicationSettings.ShutdownPCTimeByAudio - ApplicationSettings.ShutdownCounterAudio); //check is shutdown will occure soon
            }

            //is shutdown by microphone enabled
            if (ApplicationSettings.IsByMicrophoneEnabled)
            {
                //check is microphone input is inactive
                if (MicrophoneHelper.CompareMicrophone(ref ApplicationSettings.ShutdownCounterMicrophone, ApplicationSettings.ShutdownPCTimeByMicrophone))
                {
                    PerformShutdown(ShutdownOptions.Microphone); //write log about shutdown and perform it
                }

                isShutdownSoon = IsCurrentShutdown(ApplicationSettings.ShutdownPCTimeByMicrophone - ApplicationSettings.ShutdownCounterMicrophone); //check is shutdown will occure soon
            }

            //if pc shutdown by day of the week is enabled
            if (ApplicationSettings.IsByDayOfTheWeekEnabled)
            {
                isShutdownSoon = IsCurrentShutdown(PerformDayOfTheWeekCheck());
            }

            if (ApplicationSettings.IsUserNotified)
                NotifyUserAboutShutdown(isShutdownSoon);
        }

        /// <summary>
        /// Base on current day of the week check is time to shutdown become or not
        /// </summary>
        private int PerformDayOfTheWeekCheck()
        {
            var currentDayOfTheWeek = DateTime.Now.DayOfWeek; //get current day of the week
            var timeToShutdown = DateTime.MinValue;


            switch (currentDayOfTheWeek)
            {
                case DayOfWeek.Monday:
                    timeToShutdown = ApplicationSettings.MondayShutdownTime;
                    break;
                    
                case DayOfWeek.Tuesday:
                    timeToShutdown = ApplicationSettings.TuesdayShutdownTime;
                    break;

                case DayOfWeek.Wednesday:
                    timeToShutdown = ApplicationSettings.WednesdayShutdownTime;
                    break;

                case DayOfWeek.Thursday:
                    timeToShutdown = ApplicationSettings.ThursdayShutdownTime;
                    break;

                case DayOfWeek.Friday:
                    timeToShutdown = ApplicationSettings.FridayhutdownTime;
                    break;

                case DayOfWeek.Saturday:
                    timeToShutdown = ApplicationSettings.SaturdayhutdownTime;
                    break;

                case DayOfWeek.Sunday:
                    timeToShutdown = ApplicationSettings.SundayShutdownTime;
                    break;
            }

            return CheckTime(timeToShutdown);
        }

        /// <summary>
        /// Check is time to shutdown occurs
        /// </summary>
        /// <param name="dayOfTheWeek">setted time</param>
        private int CheckTime(DateTime dayOfTheWeek)
        {
            var substractResult = Convert.ToInt32(dayOfTheWeek.TimeOfDay.Subtract(DateTime.Now.TimeOfDay).TotalSeconds);

            if (substractResult == 0)
            {
                PerformShutdown(ShutdownOptions.Schedule); //write log about shutdown and perform it
            }

            return substractResult;
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
