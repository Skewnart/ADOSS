using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace Client
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private string serverPath = "127.0.0.1";
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
        private string key;
        public string Key
        {
            get { return key; }
            set
            {
                key = value;
                OnPropertyChanged("Key");
            }
        }
        private string val;
        public string Val
        {
            get { return val; }
            set
            {
                val = value;
                OnPropertyChanged("Val");
            }
        }
        public string Token { get; set; } = null;


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
                IPAddress ipAddress = null;
                if (Regex.IsMatch(this.ServerPath, @"^([0-2]?([0-9]{1,2})\.){3}([0-2]?([0-9]{1,2}))$"))
                    ipAddress = new IPAddress(this.ServerPath.Split(new string[] { "." }, StringSplitOptions.None).Select(x => byte.Parse(x)).ToArray());
                else
                {

                    IPHostEntry ipHostInfo = Dns.Resolve(this.ServerPath);
                    ipAddress = ipHostInfo.AddressList[0];
                }

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
                    MessageBox.Show($"Server is not listening", "Problem", MessageBoxButton.OK, MessageBoxImage.Error);
                else if (se.SocketErrorCode == SocketError.HostNotFound)
                    MessageBox.Show($"Given address is unreachable", "Problem", MessageBoxButton.OK, MessageBoxImage.Error);
                else if (se.SocketErrorCode == SocketError.ConnectionRefused)
                    MessageBox.Show($"Server refused the connection", "Problem", MessageBoxButton.OK, MessageBoxImage.Error);
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
                this.Result = this.ErrorCodes.First(x => x.Code.Equals(result[0])).Description;

                if (request.StartsWith("user connect") && result[0].Equals("608") && result.Length == 2)
                    this.Token = result[1];
                else if (request.StartsWith("get") && result[0].Equals("804") && result.Length == 2)
                    this.Val = Encoding.UTF8.GetString(Convert.FromBase64String(result[1]));
            }
        }

        private void Change_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(this.NewPassword))
                this.DoAction($"user changepassword {this.Access} {this.Username} {this.Password} {this.NewPassword}");
        }

        private void GET_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(this.Token)) this.Result = "You must be connected before.";
            else if (String.IsNullOrEmpty(this.Key)) Result = "You need to set a key for GET command";
            else
            {
                this.DoAction($"get \"{this.Key}\" \"{this.Token}\"");
            }
        }

        private void SET_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(this.Token)) this.Result = "You must be connected before.";
            else if (String.IsNullOrEmpty(this.Key)) Result = "You need to set a key for SET command";
            else if (String.IsNullOrEmpty(this.Val)) Result = "You need to set a value for SET command";
            else
            {
                this.DoAction($"set \"{this.Key}\" \"{Convert.ToBase64String(Encoding.UTF8.GetBytes(this.Val))}\" \"{this.Token}\"");
            }
        }

        private void DEL_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(this.Token)) this.Result = "You must be connected before.";
            else if (String.IsNullOrEmpty(this.Key)) Result = "You need to set a key for DEL command";
            else
            {
                this.DoAction($"del \"{this.Key}\" \"{this.Token}\"");
            }
        }

        private void DELALL_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(this.Token)) this.Result = "You must be connected before.";
            else
            {
                this.DoAction($"delall \"{this.Token}\"");
            }
        }
    }
}
