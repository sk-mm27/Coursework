using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ProOdezhda.Windows
{
    public partial class WinORDER : Window
    {
        ProOdezhdaEntities db = new ProOdezhdaEntities();
        Working_With_Forms af = new Working_With_Forms();

        public WinORDER()
        {
            InitializeComponent();
            af.Load_COUNTERPARTY_and_NOMENCLATURE (FK_COUNTERPARTY_ID, "Поставщик", FK_NOMENCLATURE_ID, "Материал");
        }

        private void BC_Cancel(object sender, RoutedEventArgs e) { Hide(); }

        private void BC_Save(object sender, RoutedEventArgs e) { if (Save()) { Hide(); } }

        public bool Save(int user = 0, bool test = false)
        {
            if (Check(test))
            {
                int id = db.ORDERs.OrderByDescending(o => o.ORDER_ID).FirstOrDefault() == null ?
                    1 : int.Parse(db.ORDERs.OrderByDescending(o => o.ORDER_ID).FirstOrDefault().ORDER_ID.ToString()) + 1;

                int u = test ? user : int.Parse(Application.Current.Properties["USER_ID"].ToString());

                ORDER order = new ORDER
                {
                    ORDER_ID = id,
                    DATE = DateTime.Parse(DATE.SelectedDate.ToString()),
                    CUTOFF_DATE = DateTime.Parse(CUTOFF_DATE.SelectedDate.ToString()),
                    FK_COUNTERPARTY_ID = int.Parse(FK_COUNTERPARTY_ID.SelectedValue.ToString()),

                    FK_USER_ID = u
                };

                db.ORDERs.Add(order);

                foreach (Nomenclature o_L in ORDER_LIST_Grid.Items)
                {
                    ORDER_LIST order_list = new ORDER_LIST
                    {
                        FK_ORDER_ID = id,
                        FK_NOMENCLATURE_ID = o_L.NOMENCLATURE_ID,
                        QUANTITY = o_L.QUANTITY,
                        PRICE = decimal.Parse(o_L.PRICE.ToString()),
                        SUM = decimal.Parse(o_L.SUM.ToString())
                    };

                    db.ORDER_LIST.Add(order_list);
                }

                db.SaveChanges();

                return true;
            }

            return false;
        }

        private bool Check(bool test = false)
        {
            if(DATE.SelectedDate == null)
            {
                if (test) { return false; }
                MessageBox.Show("Дата оформления не введена!"); return false;  
            }
            
            if (CUTOFF_DATE.SelectedDate == null)
            {
                if (test) { return false; }
                MessageBox.Show("Дата крайнего срока не введена!"); return false; 
            }
            else if (DATE.SelectedDate > CUTOFF_DATE.SelectedDate)
            {
                if (test) { return false; }
                MessageBox.Show("Дата крайнего срока меньше даты оформления!"); return false; 
            }

            if (FK_COUNTERPARTY_ID.SelectedValue == null)
            {
                if (test) { return false; }
                MessageBox.Show("Поставщик не выбран!"); return false; 
            }

            if (ORDER_LIST_Grid.Items.Count == 0)
            {
                if (test) { return false; }
                MessageBox.Show("Состав заказа пуст!"); return false; 
            }
            
            return true;
        }

        private void Insert_N_Click(object sender, RoutedEventArgs e) { INC(); }   
        public void INC() { af.Insert_List(ORDER_LIST_Grid, FK_NOMENCLATURE_ID, QUANTITY, PRICE); }

        private void Update_N_Click(object sender, RoutedEventArgs e) { UNC(); }
        public void UNC() { af.Update_List(ORDER_LIST_Grid, FK_NOMENCLATURE_ID, QUANTITY, PRICE); }    
        
        private void Delete_N_Click(object sender, RoutedEventArgs e) { DNC(); }
        public void DNC() { af.Delete_List(ORDER_LIST_Grid); }

        private void ORDER_LIST_Grid_SelectionChanged(object sender, SelectionChangedEventArgs e) { OLGSC(); }
        public void OLGSC() { af.LIST_Selection_Changed(ORDER_LIST_Grid, FK_NOMENCLATURE_ID, QUANTITY, PRICE); }

        public void Filling(DateTime date, DateTime cutoff_date, int countrparty)
        {
            DATE.SelectedDate = date;
            CUTOFF_DATE.SelectedDate = cutoff_date;
            FK_COUNTERPARTY_ID.SelectedIndex = countrparty;
        }
        public void Filling_LIST(int nomenclature, string quantity, string price)
        {
            FK_NOMENCLATURE_ID.SelectedIndex = nomenclature;
            QUANTITY.Text = quantity;
            PRICE.Text = price;
        }
    }
}
