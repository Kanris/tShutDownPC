using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using tShutDownPC.Enums;
using tShutDownPC.Support;

namespace tShutDownPC.Models
{
    public class MainWindowModel : INotifyPropertyChanged
    {
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
        private DateTime m_ShutdownPCTimerByTimer;
        public DateTime ShutdownPCTimerByTimer
        {
            get
            {
                return m_ShutdownPCTimerByTimer;
            }
            set
            {
                m_ShutdownPCTimerByTimer = value;
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
