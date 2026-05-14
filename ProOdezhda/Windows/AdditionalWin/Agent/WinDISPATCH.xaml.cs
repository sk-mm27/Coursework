using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ProOdezhda.Windows
{
    public partial class WinDISPATCH : Window
    {
        ProOdezhdaEntities db = new ProOdezhdaEntities();
        Working_With_Forms af = new Working_With_Forms();
        MainFunctionality mf = new MainFunctionality();

        public WinDISPATCH() { InitializeComponent(); mf.Load_COUNTERPARTY(COUNTERPARTYs, "Агент"); }

        private void BC_Cancel(object sender, RoutedEventArgs e) { Hide(); }
        private void BC_Save(object sender, RoutedEventArgs e) { if (Save()) { Hide(); } }

        private void COUNTERPARTYs_SelectionChanged(object sender, SelectionChangedEventArgs e) 
        { af.COUNTERPARTY_Selection("REQUEST", REQUESTs, LIST_Grid, N_REQUEST, COUNTERPARTYs); }        

        private void REQUESTs_SelectionChanged(object sender, SelectionChangedEventArgs e) 
        { af.REQUEST_or_ORDER_Selection("REQUEST", REQUESTs, LIST_Grid); }   
        
        public bool Save(int user = 0, bool test = false)
        {
            if (Check(test))
            {
                int u = test ? user : int.Parse(Application.Current.Properties["USER_ID"].ToString());

                DISPATCH dispatch = new DISPATCH
                {
                    DATE = DateTime.Parse(DATE.SelectedDate.ToString()),
                    FK_REQUEST_ID = int.Parse(REQUESTs.SelectedValue.ToString()),
                   
                    FK_USER_ID = u
                };

                db.DISPATCHes.Add(dispatch);

                db.SaveChanges();

                return true;
            }

            return false;
        }

        public bool Check(bool test = false)
        {
            if (REQUESTs.SelectedValue == null) { return false; }
            int id = int.Parse(REQUESTs.SelectedValue.ToString());
            if (DATE.SelectedDate == null)
            { 
                if (test) { return false; } 
                MessageBox.Show("Дата оформления не введена!"); return false; 
            }
            else if (DATE.SelectedDate < db.REQUESTs.Where(r_id => r_id.REQUEST_ID == id).FirstOrDefault().DATE)
            { 
                if (test) { return false; } 
                MessageBox.Show("Дата оформления поставки раньше чем у запроса!"); return false; 
            }

            if (COUNTERPARTYs.SelectedValue == null)
            { 
                if (test) { return false; } 
                MessageBox.Show("Агент не выбран!"); return false; 
            }

            if (REQUESTs.SelectedValue == null)
            { 
                if (test) { return false; } 
                MessageBox.Show("Запрос не выбран!"); return false; 
            }

            return true;
        }

        public void Filling(DateTime date, int counterparty, int requests)
        {
            DATE.SelectedDate = date;
            COUNTERPARTYs.SelectedIndex = counterparty;
            REQUESTs.SelectedIndex = requests;
        }
    }
}
