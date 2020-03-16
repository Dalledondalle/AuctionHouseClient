using AuctionHouseClient.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Security;
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

namespace AuctionHouseClient.Views
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : UserControl, INotifyPropertyChanged
    {
        private string loginInfo;
        public string LoginInfo
        {
            get
            {
                return loginInfo;
            }
            set
            {
                loginInfo = value;
                INotifyPropertyChanged("loginInfo");
            }
        }
        public Visibility VisLogin {
            get
            {
                return visLogin;
            }
            set
            {
                visLogin = value;
                INotifyPropertyChanged("VisLogin");
            }
        }
        private Visibility visLogin;

        public Visibility VisSucces {
            get
            {
                return visSucces;
            }
            set
            {
                visSucces = value;
                INotifyPropertyChanged("VisSucces");
            }
        }
        private Visibility visSucces;
        
        public Visibility Vis {
            get
            {
                return vis;
            }
            set
            {
                vis = value;
                INotifyPropertyChanged("Vis");
            }
        }
        private Visibility vis;

        public Visibility VisSignUp 
        {
            get
            {
                return visSignUp;
            }
            set
            {
                visSignUp = value;
                INotifyPropertyChanged("VisSignUp");
            }
        }
        private Visibility visSignUp;

        public Visibility VisAccountCreationSucces 
        { 
            get
            {
                return visAccountCreationSucces;
            }
            set
            {
                visAccountCreationSucces = value;
                INotifyPropertyChanged("VisAccountCreationSucces");
            }
        }
        private Visibility visAccountCreationSucces;

        private void INotifyPropertyChanged(string v)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(v));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public string NewUsername { 
            get 
            { 
                return newUsername; 
            } 
            set 
            { 
                newUsername = value; 
            } 
        }
        private string newUsername;

        public string NewPassword
        {
            get
            {
                return newPassword;
            } 
            set
            {
                newPassword = value;
            }
        }
        private string newPassword;

        public string NewPasswordConfirm 
        {
            get
            {
                return newPasswordConfirm;
            }
            set
            {
                newPasswordConfirm = value;
            }
        }
        private string newPasswordConfirm;
        public string Username {
            get
            {
                return username;
            }
            set
            {
                username = value;
            }
        }
        string username;

        public string Password {
            get
            {
                return password;
            }
            set
            {
                password = value;
            }
        }
        string password;

        private DBConn db;        

        public bool LoggedIn { get; set; }

        public LoginView(DBConn Db)
        {
            VisAccountCreationSucces = Visibility.Collapsed;
            VisSignUp = Visibility.Collapsed;
            VisSucces = Visibility.Collapsed;
            db = Db;
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            Login();
        }

        private void Login()
        {
            LoggedIn = db.LoginWith(Username, PasswordBox.Password);
            if (LoggedIn) LoginSucces();
        }

        private void LoginSucces()
        {
            Username = db.Username;
            LoginInfo = $"You are logged in as {Username}";
            Vis = Visibility.Collapsed;
            VisSucces = Visibility.Visible;
        }

        private void SignUp_Click(object sender, RoutedEventArgs e)
        {
            Vis = Visibility.Collapsed;
            VisSignUp = Visibility.Visible;
        }

        private void Password_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) Login();
        }

        private void Username_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) Login();
        }

        private void BackSignUp_Click(object sender, RoutedEventArgs e)
        {
            VisSignUp = Visibility.Collapsed;
            Vis = Visibility.Visible;
        }

        private void CreateUser()
        {
            if (NewPasswordBox.Password == NewPasswordBoxConfirm.Password && NewPasswordBox.Password.Length > 5)
            {
                if (!db.DoesUserExist(newUsername))
                {
                    db.CreateNewUser(newUsername, NewPasswordBox.Password);
                    VisSignUp = Visibility.Collapsed;
                    VisAccountCreationSucces = Visibility.Visible;
                    Vis = Visibility.Visible;
                }
            }
        }

        private void CreateSignUp_Click(object sender, RoutedEventArgs e)
        {
            CreateUser();
        }

        private void SignUpKeyDown_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) CreateUser(); 
        }

        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            IconTemp t = new IconTemp();
            t.Show();
        }
    }
}
