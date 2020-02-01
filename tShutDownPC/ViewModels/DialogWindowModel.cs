using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tShutDownPC.Service.Dialog;
using tShutDownPC.Service;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Windows;

namespace tShutDownPC.ViewModels
{
    class DialogWindowModel : DialogViewModelBase, INotifyPropertyChanged
    {
        #region properties

        /// <summary>
        /// Activation key in input window
        /// </summary>
        private string m_ActivationKey;
        [RegularExpression(@"[a-zA-Z1-9]{10}", 
            ErrorMessage = "Provided key is incorrect")]
        public string ActivationKey
        {
            get => m_ActivationKey;
            set
            {
                m_ActivationKey = value;
                OnPropertyChanged();
            }
        }

        #endregion properties


        #region commands

        /// <summary>
        /// Activate program command
        /// </summary>
        private RelayCommand m_ActivateCommand;
        public RelayCommand ActivateCommand => m_ActivateCommand ?? (m_ActivateCommand = new RelayCommand(ActivateLicenseCommand));

        /// <summary>
        /// Close dialog command (without attempt to activate)
        /// </summary>
        private RelayCommand m_CloseActivationWindow;
        public RelayCommand CloseActivationWindow => m_CloseActivationWindow ?? (m_CloseActivationWindow = new RelayCommand(CloseLicenseWindowCommand));

        #endregion commands

        #region methods

        /// <summary>
        /// Try to activate program with provided key
        /// </summary>
        private void ActivateLicenseCommand(object obj)
        {
            //try to activate inputed key
            if (LicenseCheck.ActivateLicense(ActivationKey))
                this.CloseDialogWithResult(obj as Window, Service.Enums.DialogResult.Yes);
            else //provided key is not valid
                MessageBox.Show("License key is not value"); //write error about it
        }

        /// <summary>
        /// Close license window without activation
        /// </summary>
        private void CloseLicenseWindowCommand(object obj)
        {
            this.CloseDialogWithResult(obj as Window, Service.Enums.DialogResult.No);
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
