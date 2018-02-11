using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;

namespace Client
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private string serverPath;
        public string ServerPath
        {
            get { return serverPath; }
            set
            {
                serverPath = value;
                OnPropertyChanged("ServerPah");
            }
        }
        private string username;
        public string Username
        {
            get { return username; }
            set
            {
                username = value;
                OnPropertyChanged("Username");
            }
        }
        private string password;
        public string Password
        {
            get { return password; }
            set
            {
                password = value;
                OnPropertyChanged("Password");
            }
        }
        private string newpassword;
        public string NewPassword
        {
            get { return newpassword; }
            set
            {
                newpassword = value;
                OnPropertyChanged("NewPassword");
            }
        }
        private string access;
        public string Access
        {
            get { return access; }
            set
            {
                access = value;
                OnPropertyChanged("Access");
            }
        }
        private string result;
        public string Result
        {
            get { return result; }
            set
            {
                result = value;
                OnPropertyChanged("Result");
            }
        }

        public List<ErrorCode> ErrorCodes { get; set; } = new List<ErrorCode>();
        public Socket Server { get; set; } = null;

        public MainWindow()
        {
            RSA.LoadKeys();

            InitializeComponent();
            this.textbox_server.Focus();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private bool Connect()
        {
            try
            {
                // Establish the remote endpoint for the socket.
                // This example uses port 11000 on the local computer.
                IPHostEntry ipHostInfo = Dns.Resolve(this.ServerPath);
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);

                Server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                Server.Connect(remoteEP);

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

        private string SendMessage(string message)
        {
            if (this.Connect())
            {
                byte[] bytesPub = new byte[1024];
                int bytesRec = Server.Receive(bytesPub);
                string pubServ = Encoding.UTF8.GetString(bytesPub, 0, bytesRec);

                byte[] msg = RSA.Encrypt(message, pubServ, true);
                int bytesSent = Server.Send(msg);

                byte[] bytes = new byte[65536];
                bytesRec = Server.Receive(bytes);
                return RSA.Decrypt(bytes, bytesRec).Message;
            }

            //Server.Shutdown(SocketShutdown.Both);
            //Server.Close();

            return null;
        }

        private void SignIn_Click(object sender, RoutedEventArgs e)
        {
            this.DoAction($"user connect \"{this.Access}\" \"{this.Username}\" \"{this.Password}\"");
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            this.DoAction($"user register \"{this.Access}\" \"{this.Username}\" \"{this.Password}\"");
        }

        private void DoAction(string request)
        {
            if (this.ErrorCodes.Count == 0)
            {
                string[] codesplitted = SendMessage("errors list").Split(new string[] { "$$" }, StringSplitOptions.None);
                foreach (string code in codesplitted)
                    this.ErrorCodes.Add(new ErrorCode(code));
            }

            if (!String.IsNullOrEmpty(this.ServerPath) && !String.IsNullOrEmpty(this.Access) && !String.IsNullOrEmpty(this.Username) && !String.IsNullOrEmpty(this.Password))
            {
                string[] result = this.SendMessage(request).Split(new string[] { ";;" }, StringSplitOptions.None);
                this.Result = this.ErrorCodes.First(x => x.Code.Equals(result[0])).FrenchDesc;
            }
        }

        private void Change_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(this.NewPassword))
                this.DoAction($"user changepassword {this.Access} {this.Username} {this.Password} {this.NewPassword}");
        }
    }
}
