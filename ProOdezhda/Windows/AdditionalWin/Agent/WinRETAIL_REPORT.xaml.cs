using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ProOdezhda.Windows
{
    public partial class WinRETAIL_REPORT : Window
    {
        ProOdezhdaEntities db = new ProOdezhdaEntities();
        Working_With_Forms af = new Working_With_Forms();

        public WinRETAIL_REPORT()
        {
            InitializeComponent();
            af.Load_COUNTERPARTY_and_NOMENCLATURE(FK_COUNTERPARTY_ID, "Агент", FK_NOMENCLATURE_ID, "Продукция");
        }

        private void Button_Click(object sender, RoutedEventArgs e) { Hide(); }
        private void Button_Click_1(object sender, RoutedEventArgs e) { if (Save()) { Hide(); } }

        public bool Save(bool test = false)
        {
            if (Check(test))
            {
                int id = db.RETAIL_REPORT.OrderByDescending(o => o.RETAIL_REPORT_ID).FirstOrDefault() == null ?
                    1 : int.Parse(db.RETAIL_REPORT.OrderByDescending(o => o.RETAIL_REPORT_ID).FirstOrDefault().RETAIL_REPORT_ID.ToString()) + 1;

                RETAIL_REPORT order = new RETAIL_REPORT
                {
                    RETAIL_REPORT_ID = id,
                    DATE = DateTime.Parse(DATE.SelectedDate.ToString()),
                    FK_COUNTERPARTY_ID = int.Parse(FK_COUNTERPARTY_ID.SelectedValue.ToString()),
                };

                db.RETAIL_REPORT.Add(order);

                foreach (Nomenclature o_L in RETAIL_REPORT_LIST_Grid.Items)
                {
                    RETAIL_REPORT_LIST order_list = new RETAIL_REPORT_LIST
                    {
                        FK_RETAIL_REPORT_ID = id,
                        FK_NOMENCLATURE_ID = o_L.NOMENCLATURE_ID,
                        QUANTITY = o_L.QUANTITY,
                        PRICE = decimal.Parse(o_L.PRICE.ToString()),
                        SUM = decimal.Parse(o_L.SUM.ToString())
                    };

                    db.RETAIL_REPORT_LIST.Add(order_list);
                }

                db.SaveChanges();

                return true;
            }

            return false;
        }

        private bool Check(bool test = false)
        {
            if (DATE.SelectedDate == null)
            {
                if (test) { return false; }
                MessageBox.Show("Дата оформления не введена!"); return false; 
            }

            if (FK_COUNTERPARTY_ID.SelectedValue == null)
            {
                if (test) { return false; }
                MessageBox.Show("Агент не выбран!"); return false; 
            }

            if (RETAIL_REPORT_LIST_Grid.Items.Count == 0)
            {
                if (test) { return false; }
                MessageBox.Show("Состав заказа пуст!"); return false; 
            }

            return true;
        }

        private void Insert_N_Click(object sender, RoutedEventArgs e) { INC(); }
        public void INC() { af.Insert_List(RETAIL_REPORT_LIST_Grid, FK_NOMENCLATURE_ID, QUANTITY, PRICE); }

        private void Update_N_Click(object sender, RoutedEventArgs e) 
        { af.Update_List(RETAIL_REPORT_LIST_Grid, FK_NOMENCLATURE_ID, QUANTITY, PRICE); }

        private void Delete_N_Click(object sender, RoutedEventArgs e) 
        { af.Delete_List(RETAIL_REPORT_LIST_Grid); }

        private void RETAIL_REPORT_LIST_Grid_SelectionChanged(object sender, SelectionChangedEventArgs e) 
        { af.LIST_Selection_Changed(RETAIL_REPORT_LIST_Grid, FK_NOMENCLATURE_ID, QUANTITY, PRICE); }

        public void Filling(DateTime date, int countrparty)
        {
            DATE.SelectedDate = date;
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
