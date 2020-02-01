using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using tShutDownPC.Service.Enums;

namespace tShutDownPC.Service.Dialog
{
    public abstract class DialogViewModelBase
    {
        #region properties

        /// <summary>
        /// License dialog result
        /// </summary>
        public DialogResult ActivationDialogResult
        {
            get;
            private set;
        }

        #endregion properties

        #region methods

        /// <summary>
        /// Close License dialog view and return dialog result
        /// </summary>
        /// <param name="dialog">dialog's window</param>
        /// <param name="result">result to return</param>
        public void CloseDialogWithResult(Window dialog, DialogResult result)
        {
            this.ActivationDialogResult = result;
            if (dialog != null)
                dialog.DialogResult = true;
        }

        #endregion methods
    }
}
