using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace ProOdezhda.Windows
{
    public partial class WinCOUNTERPARTY : Window
    {
        ProOdezhdaEntities db = new ProOdezhdaEntities();
        
        public WinCOUNTERPARTY() { InitializeComponent(); Load(); }

        private void BC_Save(object sender, RoutedEventArgs e) { Hide(); }
        private void BC_Cancel(object sender, RoutedEventArgs e) { if (Save()) { Hide(); } }

        public void Load()
        {
            TYPE.ItemsSource = db.TYPEs.ToList();

            TYPE.DisplayMemberPath = "NAME";
            TYPE.SelectedValuePath = "TYPE_ID";

            CATEGORY_C.ItemsSource = db.CATEGORY_C.ToList();

            CATEGORY_C.DisplayMemberPath = "NAME";
            CATEGORY_C.SelectedValuePath = "CATEGORY_C_ID";
        }

        public bool Save(bool test = false)
        {
            if (Chack(test))
            {
                COUNTERPARTY c = new COUNTERPARTY()
                {
                    NAME = NAME.Text,
                    FK_TYPE_ID = int.Parse(TYPE.SelectedValue.ToString()),
                    ADDRESS = ADDRESS.Text,
                    INN = INN.Text,
                    KPP = KPP.Text,
                    OGRN = OGRN.Text,
                    DIRECTOR = DIRECTOR.Text,
                    TELEPHONE = TELEPHONE.Text,
                    EMAIL = EMAIL.Text                    
                };
                if (CATEGORY_C.SelectedValue != null) { c.FK_CATEGORY_C_ID = int.Parse(CATEGORY_C.SelectedValue.ToString()); }
                db.COUNTERPARTies.Add(c); db.SaveChanges();
                return true;
            }
            return false;
        }

        public bool Chack(bool test = false)
        {
            if (NAME.Text == string.Empty)
            {
                if (test) { return false; }
                MessageBox.Show("Название не введено!"); return false; 
            }
           
            if (TYPE.SelectedValue == null)
            {
                if (test) { return false; }
                MessageBox.Show("Тип не выбранн!"); return false; 
            }
           
            if (ADDRESS.Text == string.Empty)
            {
                if (test) { return false; }
                MessageBox.Show("Адрес не введён!"); return false; 
            }
            
            if (INN.Text.Trim('_').Length < 12 && !Regex.IsMatch(INN.Text.Trim('_'), @"([0-9])+")) 
            {
                if (test) { return false; }
                MessageBox.Show("ИНН введён неправильно!"); return false; 
            }
            
            if (KPP.Text.Trim('_').Length < 9 && !Regex.IsMatch(KPP.Text.Trim('_'), @"([0-9])+")) 
            {
                if (test) { return false; }
                MessageBox.Show("КПП введён неправильно!"); return false; 
            }
            
            if (OGRN.Text.Trim('_').Length < 13 && !Regex.IsMatch(OGRN.Text.Trim('_'), @"([0-9])+")) 
            {
                if (test) { return false; }
                MessageBox.Show("ОГРН введён неправильно!"); return false; 
            }
            
            if (DIRECTOR.Text == string.Empty)
            {
                if (test) { return false; }
                MessageBox.Show("ФИО директора не введено!"); return false; 
            }
            else if (!Regex.IsMatch(DIRECTOR.Text, @"^\S+ \S+$") && !Regex.IsMatch(DIRECTOR.Text, @"^\S+ \S+ \S+$"))
            {
                if (test) { return false; }
                MessageBox.Show("ФИО директора введено неправильно!"); return false; 
            }
            
            if (TELEPHONE.Text.Replace("+8 (", "").Replace(") ", "").Replace("-", "").Replace("_", "").Length < 10 &&
                !Regex.IsMatch(TELEPHONE.Text.Replace("+8 (", "").Replace(") ", "").Replace("-", "").Replace("_", ""), @"([0-9])+"))
            {
                if (test) { return false; }
                MessageBox.Show("Телефон введён неправильно!"); return false; 
            }
            
            if (EMAIL.Text == string.Empty)
            {
                if (test) { return false; }
                MessageBox.Show("Email не введён!"); return false; 
            }
            else if (Regex.IsMatch(EMAIL.Text, @"([а-я])+"))
            {
                if (test) { return false; }
                MessageBox.Show("Email не должен содержать в себе кириллицу!"); return false; 
            }
            else if (!Regex.IsMatch(EMAIL.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                if (test) { return false; }
                MessageBox.Show("Email введён неправильно!"); return false; 
            }

            return true;
        }

        public void Filling(string name, int type, string address, string inn, string kpp, string ogrn, string director, string telephone, string email, int? category = null)
        {
            Load();
            NAME.Text = name;
            TYPE.SelectedIndex = type;
            ADDRESS.Text = address;
            INN.Text = inn;
            KPP.Text = kpp;
            OGRN.Text = ogrn;
            DIRECTOR.Text = director;
            TELEPHONE.Text = telephone;
            EMAIL.Text = email;
            if (category != null) { CATEGORY_C.SelectedIndex = (int)category; }
        }

        private void MouseDoubleClick_CursorToStart(object sender, MouseButtonEventArgs e) { (sender as TextBox).Select(0, 0); }        
    }
}
