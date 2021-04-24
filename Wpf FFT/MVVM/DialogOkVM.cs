using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Wpf_FFT.Commands;
using Wpf_FFT.MVVM.Base;

namespace Wpf_FFT.MVVM
{
    public class DialogOkVM : IDialogVM
    {
        IDialog dialog;

        public string Message { get; set; }
        public DialogOkVM(IDialog dialog, string message)
        {
            this.dialog = dialog;
            Message = message;
        }


        public ActionCommand OKCommand
        {
            get
            {
                var t = new ActionCommand(p =>
                {
                    Results = "OK";
                    // dialog.Hide();
                    dialog.DialogResult = true;
                });

                return t;
            }
        }

        public ActionCommand CancelCommand
        {
            get
            {
                var t = new ActionCommand(p =>
                {
                    Results = "Cancel";
                    //dialog.Hide();
                    dialog.DialogResult = true;

                });
                return t;
            }
        }

        public string Results { get; set; } = "";

        public void Show()
        {
            dialog.ShowDialog();
        }

        public void Close()
        {
            dialog.Close();
        }
    }
}
