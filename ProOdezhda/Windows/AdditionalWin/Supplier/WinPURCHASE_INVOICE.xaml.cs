using System;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ProOdezhda.Windows
{
    public partial class WinPURCHASE_INVOICE : Window
    {
        ProOdezhdaEntities db = new ProOdezhdaEntities();
        Working_With_Forms af = new Working_With_Forms();
        MainFunctionality mf = new MainFunctionality();

        public WinPURCHASE_INVOICE()
        {
            InitializeComponent();
            mf.Load_COUNTERPARTY(COUNTERPARTYs, "Поставщик");
        }

        private void BC_Cancel(object sender, RoutedEventArgs e) { Hide(); }
        private void BC_Save(object sender, RoutedEventArgs e) { if (Save()) { Hide(); } }

        private void COUNTERPARTYs_SelectionChanged(object sender, SelectionChangedEventArgs e) 
        { af.COUNTERPARTY_Selection("ORDER", ORDERs, LIST_Grid, N_ORDER, COUNTERPARTYs); }
        
        private void ORDERs_SelectionChanged(object sender, SelectionChangedEventArgs e) 
        { af.REQUEST_or_ORDER_Selection("ORDER", ORDERs, LIST_Grid); }    

        public bool Save(bool test = false)
        {
            if (Check(test))
            {
                PURCHASE_INVOICE purchase_invoice = new PURCHASE_INVOICE
                {
                    DATE = DateTime.Parse(DATE.SelectedDate.ToString()),
                    FK_ORDER_ID = int.Parse(ORDERs.SelectedValue.ToString()),
                };

                db.PURCHASE_INVOICE.Add(purchase_invoice);                

                db.SaveChanges();

                return true;
            }

            return false;
        }

        public bool Check(bool test = false)
        {
            if(ORDERs.SelectedValue == null) { return false; }
            int id = int.Parse(ORDERs.SelectedValue.ToString());
            if (DATE.SelectedDate == null)
            {
                if (test) { return false; }
                MessageBox.Show("Дата оформления не введена!"); return false; 
            }
            else if (DATE.SelectedDate < db.ORDERs.Where(o_id => o_id.ORDER_ID == id).FirstOrDefault().DATE)
            {
                if (test) { return false; }
                MessageBox.Show("Дата оформления накладной раньше чем у заказа!"); return false; 
            }

            if (COUNTERPARTYs.SelectedValue == null)
            {
                if (test) { return false; }
                MessageBox.Show("Поставщик не выбран!"); return false; 
            }

            if (ORDERs.SelectedValue == null)
            {
                if (test) { return false; }
                MessageBox.Show("Заказ не выбран!"); return false; 
            }

            return true;
        }

        public void Filling(DateTime date, int counterparty, int order)
        {
            DATE.SelectedDate = date;
            COUNTERPARTYs.SelectedIndex = counterparty;
            ORDERs.SelectedIndex = order;
        }
    }
}
