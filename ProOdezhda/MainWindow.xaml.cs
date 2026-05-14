using System;
using System.Linq;
using System.Windows;
using ProOdezhda.Windows;

namespace ProOdezhda
{
    public partial class MainWindow : Window
    {
        ProOdezhdaEntities db = new ProOdezhdaEntities();

        public MainWindow() { InitializeComponent(); }

        private void Login(object sender, RoutedEventArgs e)
        {
            if (Authorization(LOGIN.Text, PASSWORD.Password)) { WorkWindow w = new WorkWindow(); Hide(); w.Show(); Close(); }
            else { MessageBox.Show("Неправильный логин или пароль!"); }
        }

        public bool Authorization(string login, string Password, bool test = false)
        {
            var id = db.USERs.Where(po => po.POSITION.NAME == "Менеджер")
                             .Where(l => l.LOGIN == login)
                             .Where(p => p.PASSWORD == Password
                             .ToString()).FirstOrDefault();
            if (id == null) { return false; }
            if (!test)
            {
                Application.Current.Properties["USER_ID"] = id.USER_ID;
                Application.Current.Properties["ENTRY_TIME"] = DateTime.Now;
            }            
            return true;
        }

        private void Registration_Click(object sender, RoutedEventArgs e)
        { WinRegistration r = new WinRegistration(); r.ShowDialog(); }

        private void Exit(object sender, RoutedEventArgs e)
        { Application.Current.Shutdown(); }
    }
}
