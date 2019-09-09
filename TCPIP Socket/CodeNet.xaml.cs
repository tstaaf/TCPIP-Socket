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
using System.Windows.Shapes;

namespace TCPIP_Socket
{
    /// <summary>
    /// Interaction logic for CodeNet.xaml
    /// </summary>
    public partial class CodeNet : Window
    {
        public CodeNet()
        {
            InitializeComponent();
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StringBuilder message = new StringBuilder((char)27 + "S" + MessageName.Text + MessageInput.Text + (char)4);

                var sendMessage = message.ToString();

            }
            catch (Exception err)
            {
                MessageInput.Text = err.Message;
            }
        }
    }
}
