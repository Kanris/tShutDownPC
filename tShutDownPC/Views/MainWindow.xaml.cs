using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace tShutDownPC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            if (!Protection.Protect.CheckAccess("100"))
            {
                MessageBox.Show("К сожалению не удалось запустить приложение возможно у вас отсутствует интернет, если это не так свяжитесь с разработчиком \r\nTelegram: @Dem0nch1k");
                Environment.Exit(0);
            } 

                  
        }

        private void CircularProgress_StylusOutOfRange(object sender, StylusEventArgs e)
        {

        }
    }
}
