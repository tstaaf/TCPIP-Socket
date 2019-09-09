using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
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
    /// Interaction logic for M_Series.xaml
    /// </summary>
    public partial class M_Series : Window
    {
        TcpClient client;
        NetworkStream stream;
        List<string> commandList = new List<string>();

        public M_Series()
        {
            InitializeComponent();

            commandList.Add("C00 - Check Status");
            commandList.Add("C41 - Send dynamic string");

            CommandList.ItemsSource = commandList;
        }

        public async void Send(string server, string message)
        {

            string sendMessage;
            try
            {
                if (CommandList.SelectedIndex == 0)
                {
                    sendMessage = (char)2 + "000??" + (char)13;
                }
                else if (CommandList.SelectedIndex == 1)
                {
                    string newMess = message.Replace((char)46, (char)10);
                    sendMessage = (char)2 + "041C1" + "E" + LayoutSelectBox.Text + "Q1" + (char)23 + "D" + newMess + "??" + (char)13;
                }
                else
                {
                    sendMessage = message;
                }

                byte[] data = Encoding.ASCII.GetBytes(sendMessage);
                await stream.WriteAsync(data, 0, data.Length);

                data = new byte[256];

                string responseData = string.Empty;
                int bytes = await stream.ReadAsync(data, 0, data.Length);
                stream.ReadTimeout = 7000;
                responseData = Encoding.ASCII.GetString(data, 0, bytes);
                await GetResponse(sendMessage, responseData);

            }
            catch (System.IO.IOException ex)
            {
                Response.Text = "Unable to transmit to/read from printer. " + ex.Message;
            }
            catch (Exception ex)
            {
                Response.Text = "Error: " + ex.Message;
            }
            //catch (ArgumentNullException e)
            //{
            //    Console.WriteLine("ArgumentNullException: {0}", e);
            //}
            //catch (SocketException e)
            //{
            //    Console.WriteLine("SocketException: {0}", e);
            //}

        }

        private async Task GetResponse(string sent, string response)
        {
            await Task.Run(() =>
            {
                Dispatcher.Invoke(() =>
                {
                    Response.Text = "Sent: " + sent + Environment.NewLine + "Received: " + response;
                });
            });

        }

        public void Send_Click(object sender, RoutedEventArgs e)
        {
            string server = IP.Text;
            string message = Message.Text;

            Send(server, message);
        }

        private async void Connect_Click(object sender, RoutedEventArgs e)
        {
            client = new TcpClient();
            try
            {
                string server = IP.Text;
                string port = Port.Text;

                Task connect = Task.Run(() =>
                client.Connect(server, int.Parse(port)));
                await connect;

                stream = client.GetStream();

                Response.Text = stream.ToString();
                IsConnected();
            }
            catch (SocketException ex)
            {
                Response.Text = "Unable to connect. " + ex.Message;
            }
            catch (Exception ex)
            {
                Response.Text = "Unable to connect. " + ex.Message;
            }

        }

        public bool IsConnected()
        {
            if (!client.Connected)
            {
                Response.Text = "Disconnected";
                ConnectStatus.Fill = new SolidColorBrush(Colors.Red);
                IP.IsEnabled = true;
                Port.IsEnabled = true;
                ConnectButton.IsEnabled = true;
            }
            else
            {
                ConnectStatus.Fill = new SolidColorBrush(Colors.LightGreen);
                Response.Text = "Connected";
                IP.IsEnabled = false;
                Port.IsEnabled = false;
                ConnectButton.IsEnabled = false;
            }

            return true;
        }

        private void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                client.Close();
                stream.Close();
                IsConnected();
            }
            catch (NullReferenceException)
            {
                Response.Text = "No printer connected.";
            }

        }

        private void C41_Click(object sender, RoutedEventArgs e)
        {
            C41Window c41win = new C41Window();
            c41win.Owner = this;
            c41win.Show();
        }

        private void CommandList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CommandList.SelectedIndex == 1)
            {
                LayoutSelectLabel.Visibility = Visibility.Visible;
                LayoutSelectBox.Visibility = Visibility.Visible;
            }
            else if (CommandList.SelectedIndex != 1)
            {
                LayoutSelectLabel.Visibility = Visibility.Collapsed;
                LayoutSelectBox.Visibility = Visibility.Collapsed;
            }
        }

        private void CodeNetButton_Click(object sender, RoutedEventArgs e)
        {
            CodeNet codenetWin = new CodeNet();
            codenetWin.Owner = this;
            codenetWin.Show();
        }
    }
}
