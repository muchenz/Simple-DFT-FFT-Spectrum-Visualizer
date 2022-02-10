using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Wpf_FFT.Commands;
using Wpf_FFT.MVVM.Base;

namespace Wpf_FFT.MVVM
{
    public class AboutWindowVM:IDialogVM
    {
        IDialog dialog;

        public AboutWindowVM(IDialog dialog)
        {
            this.dialog = dialog;
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
