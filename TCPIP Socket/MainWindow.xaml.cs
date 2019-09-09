using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
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

namespace TCPIP_Socket
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TcpClient client;
        NetworkStream stream;
        MSeriesUC mUC = new MSeriesUC();
        AxSeriesUC axUC = new AxSeriesUC();

        public MainWindow()
        {
            InitializeComponent();
            Window main = Application.Current.MainWindow = this;
        }

        private void ListViewItem_Selected(object sender, RoutedEventArgs e)
        {
            contentControl.Content = mUC;
            sendButton.Visibility = Visibility.Visible;
            currentSeries.Text = "M-Series";
        }

        private void ListViewItem_Selected_1(object sender, RoutedEventArgs e)
        {
            contentControl.Content = axUC;
            sendButton.Visibility = Visibility.Visible;
            currentSeries.Text = "Ax-Series";
        }

        
        private async void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            client = new TcpClient();
            string server = IP.Text;
            string port = Port.Text;
            try
            {
                Task connect = Task.Run(() =>
                client.Connect(server, int.Parse(port)));
                Response.Text = connect.Status.ToString();
                await connect;
            }
            catch(Exception ex)
            {
                Response.Text = "Unable to connect. " + ex.Message;
            }
            try
            {
                stream = client.GetStream();
                Response.Text = stream.ToString();
                IsConnected();
            }
            catch (Exception ex)
            {
                Response.Text = "Unable to connect. " + ex.Message;
            }

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

        public bool IsConnected()
        {
            if (!client.Connected)
            {
                Response.Text = "Disconnected";
                ConnectStatus.Fill = new SolidColorBrush(Colors.Red);
                IP.IsEnabled = true;
                Port.IsEnabled = true;
                ConnectButton.IsEnabled = true;
                return false;
            }
            else
            {
                ConnectStatus.Fill = new SolidColorBrush(Colors.LightGreen);
                Response.Text = "Connected";
                IP.IsEnabled = false;
                Port.IsEnabled = false;
                ConnectButton.IsEnabled = false;
                return true;
            }

        }

        public void Send_Click(object sender, RoutedEventArgs e)
        {
            string sendMessage;
            if(contentControl.Content == mUC)
            {
                sendMessage = mUC.Message.Text;

                if (mUC.CommandList.SelectedItem == null)
                {
                    sendMessage = mUC.Message.Text;
                }
                else if (mUC.CommandList.SelectedItem.ToString().Equals("C00 - Check Status"))
                {
                    sendMessage = (char)2 + "000??" + (char)13;
                }
                else if(mUC.CommandList.SelectedItem.ToString().Equals("C41 - Send dynamic string"))
                {
                    string newMess = sendMessage.Replace((char)46, (char)10);
                    sendMessage = (char)2 + "041C1" + "E" + mUC.LayoutSelectBox.Text + "Q1" + (char)23 + "D" + newMess + "??" + (char)13;
                }
                Send(sendMessage);
            }
            else if(contentControl.Content == axUC)
            {
                try
                {
                    StringBuilder message = new StringBuilder((char)27 + "S" + axUC.MessageName.Text + axUC.MessageInput.Text + (char)4);

                    sendMessage = message.ToString();

                    Send(sendMessage);
                }
                catch (Exception err)
                {
                    Response.Text = err.Message;
                }
            }  
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
        public async void Send(string message)
        {
            string sendMessage;
            try
            {
                sendMessage = message;

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
        }

        private void HidePrinterSettings_Click(object sender, RoutedEventArgs e)
        {
            if(hidePrinterSettings.Content.ToString() == "Hide Printer Settings")
            {
                printerRow1.Height = new GridLength(0);
                printerRow2.Height = new GridLength(0);
                printerRow3.Height = new GridLength(0);
                hidePrinterSettings.Content = "Show Printer Settings";
            }
            else if(hidePrinterSettings.Content.ToString() == "Show Printer Settings")
            {
                printerRow1.Height = new GridLength(25);
                printerRow2.Height = new GridLength(40);
                printerRow3.Height = new GridLength(30);
                hidePrinterSettings.Content = "Hide Printer Settings";
            }
            
            
        }
    }
}