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

        private Timer m_ViewTimer; //timer for update view

        private Timer m_LicenseTimer; //timer for update view

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


        private bool _isEnabled;
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                OnPropertyChanged();
            }
        }

        private double _CPULoad = 1;
        public double CPULoad
        {
            get => _CPULoad;
            set
            {

                if (_CPULoad < value)
                {
                    _CPULoad++;
                }
                else if (_CPULoad > value)
                {
                    _CPULoad--;
                }
                else
                {
                    _CPULoad = value;
                }
                OnPropertyChanged();
            }
        }

        private double _NoizeInMic = 1;
        public double NoizeInMic
        {
            get => _NoizeInMic;
            set
            {
                _NoizeInMic = value * 100;
                OnPropertyChanged();
            }
        }

        private double _NoizeInSpeaker = 1;
        public double NoizeInSpeaker
        {
            get => _NoizeInSpeaker;
            set
            {
                _NoizeInSpeaker = value * 100;
                OnPropertyChanged();
            }
        }

        private string _LicenseText;
        public string LicenseText
        {
            get => _LicenseText;
            set
            {
                _LicenseText = value;
                OnPropertyChanged();
            }
        }

        private int _tabIndex;
        public int TabIndex
        {
            get => _tabIndex;

            set
            {
                _tabIndex = value;
                ResizeForm();
                OnPropertyChanged();
            }
        }

        private int _formHeight = 800;
        public int FormHeight
        {
            get => _formHeight;

            set
            {
                _formHeight = value;
                OnPropertyChanged();
            }
        }


        private int _formWidth = 800;
        public int FormWidth
        {
            get => _formWidth;

            set
            {
                _formWidth = value;
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
            m_GlobalTimer = new Timer(1 * 1000 * 60); //tick every 1 min
            m_GlobalTimer.Elapsed += M_GlobalTimer_Elapsed; //method to perform
            m_GlobalTimer.Start(); //start global timer

            m_ViewTimer = new Timer(15);  // 60 fps
            m_ViewTimer.Elapsed += M_ViewTimer_Elapsed; //method to perform
            m_ViewTimer.Start(); //start timer for update view

            M_LicenseTimer_Elapsed(null, null);
            m_LicenseTimer = new Timer(1000 * 60 * 60);// check license every 1 hour
            m_LicenseTimer.Elapsed += M_LicenseTimer_Elapsed; //method to perform
            m_LicenseTimer.Start(); //start license timer

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
            ResizeForm();
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
            //Console.WriteLine(SpeakerHelper.CurrVolume);
            //NoizeInMic = Convert.ToInt32(MicrophoneHelper.CurrVolume * 1000);
            //NoizeInSpeaker = Convert.ToInt32(SpeakerHelper.CurrVolume * 1000);
            if (IsEnabled == false)
            {
                return;
            }


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


        //get new value (mic/speaker/cpu) for update view
        private void M_ViewTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            NoizeInSpeaker = SpeakerHelper.GetVolume();
            NoizeInMic = MicrophoneHelper.GetVolume();
            CPULoad = cpuCounter.NextValue();
        }


        //check license every 1 hour
        private void M_LicenseTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var activationTime = LicenseCheck.GetDateLicense();
            Console.WriteLine(activationTime);
            var days = (activationTime.AddDays(14) - DateTime.Now).Days;

            if (activationTime == DateTime.MinValue)
            {
                if (ApplicationSettings.ApplicationLanguage == LanguageSettings.EN)
                    LicenseText = "Program activated";
                else
                    LicenseText = "Программа активирована";

                try
                {
                    m_LicenseTimer.Stop();
                }
                catch { }
            }
            else if (days <= 14)
            {
                if (ApplicationSettings.ApplicationLanguage == LanguageSettings.EN)
                    LicenseText = $"Activate the program. You have {days} days left";
                else
                    LicenseText = $"Активируйте программу у вас осталось {days} дней";

            }
            else
            {
                if (ApplicationSettings.ApplicationLanguage == LanguageSettings.EN)
                    LicenseText = "The program is NOT activated";
                else
                    LicenseText = $"Программа НЕ активирована";
            }

            //ApplicationSettings.ApplicationLanguage


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

        private void ResizeForm()
        {
            switch (TabIndex)
            {
                case 0:
                    FormHeight = 400;
                    FormWidth = 800;
                    break;

                case 1:
                    FormHeight = 800;
                    FormWidth = 800;
                    break;

                case 2:
                    FormHeight = 400;
                    FormWidth = 800;
                    break;

                default:
                    FormHeight = 400;
                    FormWidth = 800;
                    break;
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
