using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TCPIP_Socket
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class MSeriesUC : UserControl
    {
        //TcpClient client;
        //NetworkStream stream;
        List<string> commandList = new List<string>();

        public MSeriesUC()
        {
            InitializeComponent();
            commandList.Add("-");
            commandList.Add("C00 - Check Status");
            commandList.Add("C41 - Send dynamic string");

            

            CommandList.ItemsSource = commandList;
        }


        public void Send_Click(object sender, RoutedEventArgs e)
        {
            string sendMessage = Message.Text;

            if (CommandList.SelectedIndex == 0)
            {
                sendMessage = (char)2 + "000??" + (char)13;
            }
            else if (CommandList.SelectedIndex == 1)
            {
                string newMess = sendMessage.Replace((char)46, (char)10);
                sendMessage = (char)2 + "041C1" + "E" + LayoutSelectBox.Text + "Q1" + (char)23 + "D" + newMess + "??" + (char)13;
            }
            else
            {
                sendMessage = Message.Text;
            }

            //Send(sendMessage);
        }

        private void CommandList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CommandList.SelectedItem.ToString().Equals("C41 - Send dynamic string"))
            {
                LayoutSelectLabel.Visibility = Visibility.Visible;
                LayoutSelectBox.Visibility = Visibility.Visible;
            }
            else
            {
                LayoutSelectLabel.Visibility = Visibility.Collapsed;
                LayoutSelectBox.Visibility = Visibility.Collapsed;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            C41Window c41win = new C41Window();
            c41win.Show();
        }
    }
}
