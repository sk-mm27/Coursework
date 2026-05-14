using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace ProOdezhda.Windows
{
    public partial class WinRegistration : Window
    {
        ProOdezhdaEntities db = new ProOdezhdaEntities();

        public WinRegistration()
        {
            InitializeComponent();
            POSITION.ItemsSource = db.POSITIONs.ToList();

            POSITION.DisplayMemberPath = "NAME";
            POSITION.SelectedValuePath = "POSITION_ID";
        }

        private void BC_Cancel(object sender, RoutedEventArgs e) { Hide(); }
        private void BC_Save(object sender, RoutedEventArgs e) 
        { if (Save(LOGIN.Text, PASSWORD.Text, POSITION)) { Hide(); } }

        public bool Save(string login, string password, ComboBox position, bool test = false)
        {
            if (Check(login, password, position, test))
            {
                USER u = new USER
                {
                    LOGIN = login,
                    PASSWORD = password,
                    FK_POSITION_ID = int.Parse(position.SelectedValue.ToString())
                };
                db.USERs.Add(u); db.SaveChanges();

                return true;
            }
            return false;
        }

        public bool Check(string login, string password, ComboBox position, bool test = false)
        {
            if (login == string.Empty)
            {
                if (!test) { MessageBox.Show("Логин не введён!"); }
                return false; 
            }
            else if (login.Contains(" "))
            {
                if (!test) { MessageBox.Show("Логин введён неправильно!"); }
                return false; 
            }
            else if (db.USERs.Where(l => l.LOGIN == login).FirstOrDefault() != null)
            {
                if (!test) { MessageBox.Show("Логин уже занят!"); }
                return false; 
            }

            if (password == string.Empty)
            {
                if (!test) { MessageBox.Show("Пароль не введён!"); }
                return false; 
            }
            else if (password.Contains(" ") || Regex.IsMatch(password, @"([а-я])+"))
            {
                if (!test) { MessageBox.Show("Пароль введён неправильно!"); }
                return false; 
            }

            if (position.SelectedValue == null)
            {
                if (!test) { MessageBox.Show("Должность не выбранна!"); } 
                return false; 
            }

            return true;
        }        
    }
}
