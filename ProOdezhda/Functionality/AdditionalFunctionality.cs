using ProOdezhda.Windows;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ProOdezhda
{
    public partial class Nomenclature
    {
        public Nomenclature(int id, string n, double q, decimal p)
        {
            this.NOMENCLATURE_ID = id;
            this.NOMENCLATURE = n;
            this.QUANTITY = q;
            this.PRICE = p;
            this.SUM = decimal.Parse(q.ToString()) * p;
        }
        public int NOMENCLATURE_ID { get; set; }
        public string NOMENCLATURE { get; set; }
        public double QUANTITY { get; set; }
        public decimal PRICE { get; set; }
        public decimal SUM { get; set; }
    }

    // Класс содержит методы предназначенные для заполнения форм
    public class Working_With_Forms
    {
        MainFunctionality mf = new MainFunctionality();
        ProOdezhdaEntities db = new ProOdezhdaEntities();

        // Загрузить списки агентов/поставщиков и продукции/материалов
        public void Load_COUNTERPARTY_and_NOMENCLATURE(ComboBox COUNTERPARTY, string CC, ComboBox NOMENCLATURE, string N)
        { mf.Load_COUNTERPARTY(COUNTERPARTY, CC); Load_NOMENCLATURE(NOMENCLATURE, N); }

        // Загрузить список продукции/материалов
        public void Load_NOMENCLATURE(ComboBox NOMENCLATURE, string N)
        {
            NOMENCLATURE.ItemsSource = db.NOMENCLATUREs.Where(n => n.CATEGORY_N.NAME == N).ToList();

            NOMENCLATURE.DisplayMemberPath = "NAME";
            NOMENCLATURE.SelectedValuePath = "NOMENCLATURE_ID";
        }

        // Загрузить список заказов поставщику
        public void Load_ORDER(ComboBox ORDER, int ID)
        {
            foreach (ORDER order in db.ORDERs.Where(c => c.FK_COUNTERPARTY_ID == ID))
            { if (!db.PURCHASE_INVOICE.Any(o => o.FK_ORDER_ID == order.ORDER_ID)) { ORDER.Items.Add(order); } }
            ORDER.SelectedValuePath = "ORDER_ID";
        }

        // Загрузить список запросов агента
        public void Load_REQUEST(ComboBox REQUEST, int ID)
        {
            foreach(REQUEST request in db.REQUESTs.Where(c => c.FK_COUNTERPARTY_ID == ID))
            { if (!db.DISPATCHes.Any(o => o.FK_REQUEST_ID == request.REQUEST_ID)) { REQUEST.Items.Add(request); }}
            REQUEST.SelectedValuePath = "REQUEST_ID";
        }

        // Загрузить состав запроса/заказа
        public void REQUEST_or_ORDER_Selection(string LR_LO, ComboBox RO, DataGrid LIST)
        {
            if (RO.SelectedItem != null)
            {
                int id = int.Parse(RO.SelectedValue.ToString());
                if (LR_LO == "REQUEST")
                { LIST.ItemsSource = db.REQUEST_LIST.Where(id_o => id_o.FK_REQUEST_ID == id).ToList(); }
                else
                { LIST.ItemsSource = db.ORDER_LIST.Where(id_o => id_o.FK_ORDER_ID == id).ToList(); }                    
            }
        }

        // Загрузить список запросов/заказов при выборе контрагента
        public void COUNTERPARTY_Selection(string LR_LO, ComboBox RO, DataGrid LIST, TextBlock Name, ComboBox COUNTERPARTYs)
        {
            RO.SelectedItem = null;
            RO.Items.Clear();
            LIST.ItemsSource = null;
            Name.Foreground = new SolidColorBrush(Color.FromArgb(255, 81, 15, 173));
            RO.IsEnabled = true;
            if (LR_LO == "REQUEST")
            { Load_REQUEST(RO, int.Parse(COUNTERPARTYs.SelectedValue.ToString())); }
            else
            { Load_ORDER(RO, int.Parse(COUNTERPARTYs.SelectedValue.ToString())); }
        }

        // Добавить запись в лист
        public void Insert_List(DataGrid Grid, ComboBox NOMENCLATURE, TextBox QUANTITY, TextBox PRICE)
        {
            if (NOMENCLATURE.SelectedValue == null) { return; }
            if (int.TryParse(NOMENCLATURE.SelectedValue.ToString(), out int id) &&
                double.TryParse(QUANTITY.Text, out double q) &&
                decimal.TryParse(PRICE.Text, out decimal p)                
            )
            { Grid.Items.Add(new Nomenclature(id, NOMENCLATURE.Text, q, p)); }
        }
        
        // Изменить запись в листе
        public void Update_List(DataGrid Grid, ComboBox NOMENCLATURE, TextBox QUANTITY, TextBox PRICE)
        {
            if (NOMENCLATURE.SelectedValue == null) { return; }
            if (int.TryParse(NOMENCLATURE.SelectedValue.ToString(), out int id) &&
                double.TryParse(QUANTITY.Text, out double q) &&
                decimal.TryParse(PRICE.Text, out decimal p))
            {
                int index = Grid.SelectedIndex;
                Delete_List(Grid);
                Grid.Items.Insert(index, new Nomenclature(id, NOMENCLATURE.Text, q, p));
            }
        }

        // Удалить запись из листа
        public void Delete_List(DataGrid Grid) { Grid.Items.Remove(Grid.SelectedItem); }

        // Заполнить строки
        public void LIST_Selection_Changed(DataGrid Grid, ComboBox NOMENCLATURE, TextBox QUANTITY, TextBox PRICE)
        {
            if (Grid.SelectedItem is Nomenclature nom)
            {
                NOMENCLATURE.SelectedValue = nom.NOMENCLATURE_ID;
                QUANTITY.Text = nom.QUANTITY.ToString();
                PRICE.Text = nom.PRICE.ToString();
            }
        }        
    }


    // Класс содержит методы предназначенные для добавления и удаления записей базы данных
    public class Insert_and_Delete
    {

        ProOdezhdaEntities db = new ProOdezhdaEntities();
        MainFunctionality mf = new MainFunctionality();

        public void NEW(string Choice, DataGrid Grid)
        {
            switch (Choice)
            {
                case "COUNTERPARTY":
                    WinCOUNTERPARTY c = new WinCOUNTERPARTY(); c.ShowDialog(); 
                    break;

                case "ORDER":
                    WinORDER o = new WinORDER(); o.ShowDialog();
                    break;

                case "PURCHASE_INVOICE":
                    WinPURCHASE_INVOICE p_i = new WinPURCHASE_INVOICE(); p_i.ShowDialog();
                    break;

                case "REQUEST":
                    WinREQUEST r = new WinREQUEST(); r.ShowDialog();
                    break;

                case "DISPATCH":
                    WinDISPATCH d = new WinDISPATCH(); d.ShowDialog();
                    break;

                case "RETAIL_REPORT":
                    WinRETAIL_REPORT r_r = new WinRETAIL_REPORT(); r_r.ShowDialog();
                    break;

                default:
                    return;
            }

            mf.Load(Choice, Grid);
        }

        public void DELETE(string Choice, DataGrid Grid, TextBlock ID = null, TextBlock DATE = null, TextBlock CD_O_R = null, TextBlock COUNTERPARTY = null, DataGrid LIST = null, bool test = false)
        {
            if (Grid.SelectedItem == null) { return; }
            MessageBoxResult yes;
            if (!test) { yes = MessageBox.Show("Вы уверены?", "Удаление записи", MessageBoxButton.YesNo, MessageBoxImage.Question); }
            else { yes = MessageBoxResult.Yes; }

            if (yes == MessageBoxResult.Yes)
            {            
                int id = ID == null ? 0 : int.Parse(ID.Text);

                switch (Choice)
                {
                    case "COUNTERPARTY": // Удаление записей о контрагентах
                        if (Grid.SelectedItem is COUNTERPARTY selectedCOUNTERPARTY)
                        {
                            // Проверка наличия зависимых записей
                            if (!db.REQUESTs.Any(r => r.FK_COUNTERPARTY_ID == selectedCOUNTERPARTY.COUNTERPARTY_ID) &&
                                !db.RETAIL_REPORT.Any(r => r.FK_COUNTERPARTY_ID == selectedCOUNTERPARTY.COUNTERPARTY_ID) &&
                                !db.ORDERs.Any(r => r.FK_COUNTERPARTY_ID == selectedCOUNTERPARTY.COUNTERPARTY_ID))

                            { db.COUNTERPARTies.Remove(db.COUNTERPARTies.Where(c => c.COUNTERPARTY_ID == selectedCOUNTERPARTY.COUNTERPARTY_ID).FirstOrDefault()); }
                            
                            else { MessageBox.Show("Невозможно удалить контрагента, т.к. есть связанные с ним документы!"); return; }
                        }
                        
                        break;

                    case "ORDER": // Удаление записей о заказах материалов
                        foreach (ORDER_LIST order_list in db.ORDER_LIST.Where(o => o.FK_ORDER_ID == id)) 
                        { db.ORDER_LIST.Remove(order_list); }

                        if (db.PURCHASE_INVOICE.Any(p_i => p_i.FK_ORDER_ID == id)) // Удаляет связанные с заказом приходные наклодные, если такие имеются
                        {
                            foreach (PURCHASE_INVOICE purchase_invoice in db.PURCHASE_INVOICE.Where(i => i.FK_ORDER_ID == id))
                            { db.PURCHASE_INVOICE.Remove(purchase_invoice); }
                        }

                        db.ORDERs.Remove(db.ORDERs.Where(o => o.ORDER_ID == id).FirstOrDefault());

                        break;

                    case "PURCHASE_INVOICE": // Удаление записей о приходных накладных
                        if (Grid.SelectedItem is PURCHASE_INVOICE selectedPURCHASE_INVOICE)
                        { db.PURCHASE_INVOICE.Remove(db.PURCHASE_INVOICE.Where(p_i => p_i.PURCHASE_INVOICE_ID == selectedPURCHASE_INVOICE.PURCHASE_INVOICE_ID).FirstOrDefault()); }

                        break;

                    case "REQUEST": // Удаление записей о запросах агентов
                        foreach (REQUEST_LIST request_list in db.REQUEST_LIST.Where(o => o.FK_REQUEST_ID == id)) 
                        { db.REQUEST_LIST.Remove(request_list); }

                        if (db.DISPATCHes.Any(d => d.FK_REQUEST_ID == id)) // Удаляет связанные с запросом поставки, если такие имеются
                        {
                            foreach (DISPATCH dispatch in db.DISPATCHes.Where(i => i.FK_REQUEST_ID == id))
                            { db.DISPATCHes.Remove(dispatch); }
                        }

                        db.REQUESTs.Remove(db.REQUESTs.Where(r => r.REQUEST_ID == id).FirstOrDefault());

                        break;

                    case "DISPATCH": // Удаление записей о поставках продукции
                        if (Grid.SelectedItem is DISPATCH selectedDISPATCHes)
                        { db.DISPATCHes.Remove(db.DISPATCHes.Where(d => d.DISPATCH_ID == selectedDISPATCHes.DISPATCH_ID).FirstOrDefault()); }
                        
                        break;

                    case "RETAIL_REPORT": // Удаление записей о розничных отчётах
                        foreach (RETAIL_REPORT_LIST retail_report in db.RETAIL_REPORT_LIST.Where(o => o.FK_RETAIL_REPORT_ID == id))
                        { db.RETAIL_REPORT_LIST.Remove(retail_report); }

                        db.RETAIL_REPORT.Remove(db.RETAIL_REPORT.Where(r_r => r_r.RETAIL_REPORT_ID == id).FirstOrDefault());

                        break;

                    default:
                        return;
                }                

                db.SaveChanges();

                if (ID != null) { CLEANING(ID, DATE, CD_O_R, COUNTERPARTY, LIST); }

                mf.Load(Choice, Grid);               
            }
        }

        public void CLEANING(TextBlock ID, TextBlock DATE, TextBlock CD_O_R, TextBlock COUNTERPARTY, DataGrid LIST)
        {
            ID.Text = string.Empty;
            DATE.Text = string.Empty;
            if (CD_O_R != null) { CD_O_R.Text = string.Empty; }
            COUNTERPARTY.Text = string.Empty;
            LIST.ItemsSource = null;
            LIST.Items.Clear();
        }
    }
}

