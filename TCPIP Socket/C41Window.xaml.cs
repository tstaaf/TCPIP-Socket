using System;
using System.Collections;
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
    /// Interaction logic for C41Windows.xaml
    /// </summary>
    public partial class C41Window : Window
    {

        public C41Window()
        {
            InitializeComponent();

        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            dynList.Items.Add(dynValue.Text);
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            try { 
            StringBuilder message = new StringBuilder((char)2 + "041C1" + "E" + LayoutSelectBox.Text + "Q1" + (char)23 + "D" + dynList.Items[0]);

            IEnumerable list = dynList.Items;

            foreach (string val in list.Cast<string>().Skip(1))
            {
                message.Append((char)10 + val);
            }

            var sendMessage = message + "??" + (char)13;

            response.Text = sendMessage;

            }
            catch (Exception err)
            {
                response.Text = "Something went wrong. " + err.Message;
            }

        }
        private void dynValue_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                //execute go button method
                AddButton_Click(sender, e);
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dynList.Items.RemoveAt(dynList.SelectedIndex);
            }
            catch(Exception ex)
            {
                response.Text = "Something went wrong. " + ex.Message;
            }
        }
    }
}
