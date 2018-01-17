using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Client
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Socket Server { get; set; } = null;
        public ObservableCollection<string> InfosList { get; set; } = new ObservableCollection<string>();

        public string Address { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            this.textboxAdd.Focus();
        }

        private bool Connect()
        {
            try
            {
                // Establish the remote endpoint for the socket.
                // This example uses port 11000 on the local computer.
                IPHostEntry ipHostInfo = Dns.Resolve(this.Address);
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);

                Server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                Server.Connect(remoteEP);

                this.InfosList.Add($"Socket connected to {Server.RemoteEndPoint.ToString()}");

                return true;
            }
            catch (ArgumentNullException ane)
            {
                System.Windows.MessageBox.Show($"ArgumentNullException: {ane.ToString()}");
            }
            catch (SocketException se)
            {
                if (se.SocketErrorCode == SocketError.TimedOut)
                    MessageBox.Show($"Le serveur n'est pas à l'écoute", "Problème", MessageBoxButton.OK, MessageBoxImage.Error);
                else if (se.SocketErrorCode == SocketError.HostNotFound)
                    MessageBox.Show($"L'adresse est inateignable", "Problème", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Exception : {ex.ToString()}");
            }

            return false;
        }

        private void SendMessage(string message)
        {
            if (!message.Equals("quit"))
            {
                byte[] bytes = new byte[1024];
                byte[] msg = Encoding.UTF8.GetBytes(message);
                
                int bytesSent = Server.Send(msg);
                this.InfosList.Add($"Sent : {message}");
                
                int bytesRec = Server.Receive(bytes);
                this.InfosList.Add($"Received : {Encoding.UTF8.GetString(bytes, 0, bytesRec)}");
            }
            else
            {
                this.InfosList.Add($"Disconnecting...");

                Server.Shutdown(SocketShutdown.Both);
                Server.Close();

                this.InfosList.Add($"Disconnected.");
            }
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string message = ((TextBox)sender).Text;
                if (!string.IsNullOrEmpty(message))
                {
                    if (Server == null || !Server.Connected)
                        Connect();

                    if (Server?.Connected ?? false)
                    {
                        SendMessage(message);
                        ((TextBox)sender).Text = "";
                    }
                }
            }
        }
    }
}
