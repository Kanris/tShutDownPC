using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tShutDownPC.Service.Dialog;
using tShutDownPC.Service.Enums;
using tShutDownPC.Views;

namespace tShutDownPC.Service
{
    public static class DialogService
    {
        /// <summary>
        /// Opens Dialog window and return open result
        /// </summary>
        /// <returns></returns>
        public static DialogResult OpenDialog(DialogViewModelBase viewModel)
        {
            var dialogWindow = new DialogWindow(); //create dialog window

            dialogWindow.DataContext = viewModel; //assign datacontext to dialog
            dialogWindow.ShowDialog(); //show dialog window

            var dialogResult = (dialogWindow.DataContext as DialogViewModelBase).ActivationDialogResult; //get dialog result

            return dialogResult; //return dialog result
        }
    }
}
