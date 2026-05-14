using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ProOdezhda.Windows
{
    public partial class WorkWindow : Window
    {
        ProOdezhdaEntities db = new ProOdezhdaEntities();
        Insert_and_Delete af_oar = new Insert_and_Delete();
        MainFunctionality mf = new MainFunctionality();

        string dateformat = "dd.MM.yyyy";

        public WorkWindow()
        {
            InitializeComponent();
            mf.Load("COUNTERPARTY", COUNTERPARTY_Grid);
            mf.Load("ORDER", ORDER_Grid);
            mf.Load("PURCHASE_INVOICE", PURCHASE_INVOICE_Grid);
            mf.Load("REQUEST", REQUEST_Grid);
            mf.Load("DISPATCH", DISPATCH_Grid);
            mf.Load("RETAIL_REPORT", RETAIL_REPORT_Grid);
            mf.Load("RATING", RATING_Grid);
        }        

        //---------------------------------------------Контрагенты---------------------------------------------------\\
        //---------------------------------------------\\\-----///---------------------------------------------------\\

        private void NEW_COUNTERPARTY(object sender, RoutedEventArgs e) { af_oar.NEW("COUNTERPARTY", COUNTERPARTY_Grid); }
        private void DELETE_COUNTERPARTY(object sender, RoutedEventArgs e) { af_oar.DELETE("COUNTERPARTY", COUNTERPARTY_Grid); }

        //---------------------------------------------///-----\\\---------------------------------------------------\\
        //---------------------------------------------Контрагенты---------------------------------------------------\\


        //-----------------------------------------------------------------------------------------------------------\\


        //------------------------------------------Заказы материалов------------------------------------------------\\
        //---------------------------------------------\\\-----///---------------------------------------------------\\

        private void NEW_ORDER(object sender, RoutedEventArgs e) { af_oar.NEW("ORDER", ORDER_Grid); }

        private void DELETE_ORDER(object sender, RoutedEventArgs e) { DO(); }
        public void DO(bool test = false) { af_oar.DELETE("ORDER", ORDER_Grid, O_ORDER_ID, O_DATE, O_CUTOFF_DATE, O_FK_COUNTERPARTY_ID, ORDER_LIST_Grid, test); }
        
        private void ORDER_Grid_SelectionChanged(object sender, SelectionChangedEventArgs e) 
        { mf.Grid_SelectionChanged("ORDER", ORDER_Grid, O_ORDER_ID, O_DATE, O_CUTOFF_DATE, O_FK_COUNTERPARTY_ID, ORDER_LIST_Grid, dateformat); }

        //---------------------------------------------///-----\\\---------------------------------------------------\\
        //------------------------------------------Заказы материалов------------------------------------------------\\


        //-----------------------------------------------------------------------------------------------------------\\


        //-----------------------------------------Приходные накладные-----------------------------------------------\\
        //---------------------------------------------\\\-----///---------------------------------------------------\\

        private void NEW_PURCHASE_INVOICE(object sender, RoutedEventArgs e) { af_oar.NEW("PURCHASE_INVOICE", PURCHASE_INVOICE_Grid); }
        private void DELETE_PURCHASE_INVOICE(object sender, RoutedEventArgs e) { DPI(); }
        public void DPI(bool test = false) { af_oar.DELETE("PURCHASE_INVOICE", PURCHASE_INVOICE_Grid, P_I_ORDER_ID, P_I_DATE, P_I_FK_ORDER_ID, P_I_FK_COUNTERPARTY_ID, PURCHASE_INVOICE_LIST_Grid, test); }
        private void PURCHASE_INVOICE_Grid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        { mf.Grid_SelectionChanged("PURCHASE_INVOICE", PURCHASE_INVOICE_Grid, P_I_ORDER_ID, P_I_DATE, P_I_FK_ORDER_ID, P_I_FK_COUNTERPARTY_ID, PURCHASE_INVOICE_LIST_Grid, dateformat); }            


        //---------------------------------------------///-----\\\---------------------------------------------------\\
        //-----------------------------------------Приходные накладные-----------------------------------------------\\


        //-----------------------------------------------------------------------------------------------------------\\


        //-------------------------------------------Запросы агентов-------------------------------------------------\\
        //---------------------------------------------\\\-----///---------------------------------------------------\\

        private void NEW_REQUEST(object sender, RoutedEventArgs e) { af_oar.NEW("REQUEST", REQUEST_Grid); }
        private void DELETE_REQUEST(object sender, RoutedEventArgs e) { DR(); }
        public void DR(bool test = false) { af_oar.DELETE("REQUEST", REQUEST_Grid, R_REQUEST_ID, R_DATE, R_CUTOFF_DATE, R_FK_COUNTERPARTY_ID, REQUEST_LIST_Grid, test); }
        private void REQUEST_Grid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        { mf.Grid_SelectionChanged("REQUEST", REQUEST_Grid, R_REQUEST_ID, R_DATE, R_CUTOFF_DATE, R_FK_COUNTERPARTY_ID, REQUEST_LIST_Grid, dateformat); }


        //---------------------------------------------///-----\\\---------------------------------------------------\\
        //-------------------------------------------Запросы агентов-------------------------------------------------\\


        //-----------------------------------------------------------------------------------------------------------\\


        //------------------------------------------Поставки продукции-----------------------------------------------\\
        //---------------------------------------------\\\-----///---------------------------------------------------\\

        private void NEW_DISPATCH(object sender, RoutedEventArgs e) { af_oar.NEW("DISPATCH", DISPATCH_Grid); }
        private void DELETE_DISPATCH(object sender, RoutedEventArgs e) { DD(); }
        public void DD(bool test = false) { af_oar.DELETE("DISPATCH", DISPATCH_Grid, D_REQUEST_ID, D_DATE, D_FK_REQUEST_ID, D_FK_COUNTERPARTY_ID, DISPATCH_LIST_Grid, test); }
        private void DISPATCH_Grid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        { mf.Grid_SelectionChanged("DISPATCH", DISPATCH_Grid, D_REQUEST_ID, D_DATE, D_FK_REQUEST_ID, D_FK_COUNTERPARTY_ID, DISPATCH_LIST_Grid, dateformat); }

        //---------------------------------------------///-----\\\---------------------------------------------------\\
        //------------------------------------------Поставки продукции-----------------------------------------------\\


        //-----------------------------------------------------------------------------------------------------------\\


        //------------------------------------------Розничные продажи------------------------------------------------\\
        //---------------------------------------------\\\-----///---------------------------------------------------\\

        private void NEW_RETAIL_REPORT(object sender, RoutedEventArgs e) { af_oar.NEW("RETAIL_REPORT", RETAIL_REPORT_Grid); }
        private void DELETE_RETAIL_REPORT(object sender, RoutedEventArgs e) { DRR(); }
        public void DRR(bool test = false) { af_oar.DELETE("RETAIL_REPORT", RETAIL_REPORT_Grid, R_R_RETAIL_REPORT_ID, R_R_DATE, null, R_R_FK_COUNTERPARTY_ID, RETAIL_REPORT_LIST_Grid, test); }
        private void RETAIL_REPORT_Grid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        { mf.Grid_SelectionChanged("RETAIL_REPORT", RETAIL_REPORT_Grid, R_R_RETAIL_REPORT_ID, R_R_DATE, null, R_R_FK_COUNTERPARTY_ID, RETAIL_REPORT_LIST_Grid, dateformat); }

        //---------------------------------------------///-----\\\---------------------------------------------------\\
        //------------------------------------------Розничные продажи------------------------------------------------\\


        //-----------------------------------------------------------------------------------------------------------\\


        //-----------------------------------------Рейтинг контрагентов----------------------------------------------\\
        //---------------------------------------------\\\-----///---------------------------------------------------\\

        private void NEW_RATING(object sender, RoutedEventArgs e) { N_R(); }
        public bool N_R(bool test = false)
        {
            if (Check(test))
            {
                RATING rating = new RATING
                {
                    DATE = DateTime.Parse(Ra_DATE.SelectedDate.ToString()),
                    FK_COUNTERPARTY_ID = int.Parse(Ra_FK_COUNTERPARTY_ID.SelectedValue.ToString()),
                    RATING1 = double.Parse(Ra_RATING.Text)
                };
                db.RATINGs.Add(rating);
                db.SaveChanges();
                mf.Load("RATING", RATING_Grid);

                return true;
            }
            return false;
        }
        private bool Check(bool test = false)
        {
            if (Ra_FK_COUNTERPARTY_ID.SelectedValue == null)
            {
                if (test) { return false; }
                MessageBox.Show("Контрагент не выбран!"); return false; 
            }
            
            if (!double.TryParse(Ra_RATING.Text, out double rating))
            {
                if (test) { return false; }
                MessageBox.Show("Рейтинг введён неправильно!"); return false; 
            }

            if (Ra_DATE.SelectedDate == null)
            {
                if (test) { return false; }
                MessageBox.Show("Дата назначения не введена!"); return false; 
            }

            return true;
        }

        private void DELETE_RATING(object sender, RoutedEventArgs e) { D_R(); }
        public bool D_R()
        {
            if(RATING_Grid.SelectedItem is RATING rating)
            {
                db.RATINGs.Remove(db.RATINGs.Where(r => r.RATING_ID == rating.RATING_ID).FirstOrDefault());
                db.SaveChanges();
                mf.Load("RATING", RATING_Grid);

                Ra_DATE.SelectedDate = null;
                Ra_FK_COUNTERPARTY_ID.SelectedValue = null;
                Ra_RATING.Text = "";
                
                return true;
            }
            return false;
        }

        private void UPDATE_RATING(object sender, RoutedEventArgs e) { U_R(); }
        public bool U_R(bool test = false)
        {
            if (Check(test))
            {
                if (RATING_Grid.SelectedItem is RATING rating)
                {
                    rating.DATE = DateTime.Parse(Ra_DATE.SelectedDate.ToString());
                    rating.FK_COUNTERPARTY_ID = int.Parse(Ra_FK_COUNTERPARTY_ID.SelectedValue.ToString());
                    rating.RATING1 = double.Parse(Ra_RATING.Text);

                    db.SaveChanges();
                    mf.Load("RATING", RATING_Grid);
                }
                return true;
            }
            return false;
        }

        private void RATING_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) { RMLBU(); }
        public void RMLBU() { mf.Load_COUNTERPARTY(Ra_FK_COUNTERPARTY_ID); }
        
        private void RATING_Grid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RATING_Grid.SelectedItem is RATING rating)
            {
                Ra_DATE.SelectedDate = rating.DATE;
                Ra_FK_COUNTERPARTY_ID.SelectedValue = rating.FK_COUNTERPARTY_ID;
                Ra_RATING.Text = rating.RATING1.ToString();
            }
        }

        public void RATING_Filling(DateTime date, int countrparty, string rating)
        {
            Ra_DATE.SelectedDate = date;
            Ra_FK_COUNTERPARTY_ID.SelectedIndex = countrparty;
            Ra_RATING.Text = rating;
        }

        //---------------------------------------------///-----\\\---------------------------------------------------\\
        //-----------------------------------------Рейтинг контрагентов----------------------------------------------\\


        //-----------------------------------------------------------------------------------------------------------\\


        //-----------------------------------------------Отчёты------------------------------------------------------\\
        //---------------------------------------------\\\-----///---------------------------------------------------\\

        private void BC_Arrived_Orders(object sender, RoutedEventArgs e) { mf.BC_Arrived_Orders(Arrived_Orders); }
        private void BC_Completed_Requests(object sender, RoutedEventArgs e) { mf.BC_Completed_Requests(Completed_Requests); }
        private void BC_Products_Sold(object sender, RoutedEventArgs e) { mf.BC_Products_Sold(Products_Sold); }
        private void BC_Counterparty_Rating(object sender, RoutedEventArgs e) { mf.BC_Counterparty_Rating(Counterparty_Rating); }

        //---------------------------------------------///-----\\\---------------------------------------------------\\
        //-----------------------------------------------Отчёты------------------------------------------------------\\

        private void Window_Closed(object sender, EventArgs e) 
        {
            USER_ACTIVITY ACTIVITY = new USER_ACTIVITY
            {
                FK_USER_ID = int.Parse(Application.Current.Properties["USER_ID"].ToString()),
                ENTRY_TIME = DateTime.Parse(Application.Current.Properties["ENTRY_TIME"].ToString()),
                EXIT_TIME = DateTime.Now
            };

            db.USER_ACTIVITY.Add(ACTIVITY);
            db.SaveChanges();

            Application.Current.Shutdown(); 
        }        
    }
}