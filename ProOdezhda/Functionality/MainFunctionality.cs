using System.Linq;
using System.Windows.Controls;

namespace ProOdezhda
{
    public class MainFunctionality
    {
        ProOdezhdaEntities db = new ProOdezhdaEntities();

        public void Load_COUNTERPARTY(ComboBox COUNTERPARTY, string CC = "")
        {
            if (CC == "")
            { COUNTERPARTY.ItemsSource = db.COUNTERPARTies.ToList(); }
            else
            {
                COUNTERPARTY.ItemsSource = db.COUNTERPARTies.Where(c => c.CATEGORY_C.NAME == CC).ToList();
                COUNTERPARTY.DisplayMemberPath = "NAME";
            }

            COUNTERPARTY.SelectedValuePath = "COUNTERPARTY_ID";
        }

        public void Load(string Choice, DataGrid Grid)
        {
            switch (Choice)
            {
                case "COUNTERPARTY":
                    Grid.ItemsSource = db.COUNTERPARTies.ToList();
                    break;

                case "ORDER":
                    Grid.ItemsSource = db.ORDERs.ToList();
                    break;

                case "PURCHASE_INVOICE":
                    Grid.ItemsSource = db.PURCHASE_INVOICE.ToList();
                    break;

                case "REQUEST":
                    Grid.ItemsSource = db.REQUESTs.ToList();
                    break;

                case "DISPATCH":
                    Grid.ItemsSource = db.DISPATCHes.ToList();
                    break;

                case "RETAIL_REPORT":
                    Grid.ItemsSource = db.RETAIL_REPORT.ToList();
                    break;

                case "RATING":
                    Grid.ItemsSource = db.RATINGs.ToList();
                    break;

                default:
                    return;
            }            
        }

        public void Grid_SelectionChanged(string Choice, DataGrid Grid, TextBlock ID, TextBlock DATE, TextBlock CD_O_R, TextBlock COUNTERPARTY, DataGrid LIST, string dateformat)
        {
            switch (Choice)
            {
                case "ORDER":
                    if (Grid.SelectedItem is ORDER o)
                    {
                        ID.Text = o.ORDER_ID.ToString();
                        DATE.Text = o.DATE.ToString(dateformat);
                        CD_O_R.Text = o.CUTOFF_DATE.ToString(dateformat);
                        COUNTERPARTY.Text = o.COUNTERPARTY.NAME;
                    }
                    break;

                case "PURCHASE_INVOICE":
                    if (Grid.SelectedItem is PURCHASE_INVOICE p_i)
                    {
                        ID.Text = p_i.ORDER.ORDER_ID.ToString();
                        DATE.Text = p_i.DATE.ToString(dateformat);
                        CD_O_R.Text = "№" + p_i.FK_ORDER_ID.ToString() + " от " + p_i.ORDER.DATE.ToString(dateformat);
                        COUNTERPARTY.Text = p_i.ORDER.COUNTERPARTY.NAME;
                    }
                    break;

                case "REQUEST":
                    if (Grid.SelectedItem is REQUEST r)
                    {
                        ID.Text = r.REQUEST_ID.ToString();
                        DATE.Text = r.DATE.ToString(dateformat);
                        CD_O_R.Text = r.CUTOFF_DATE.ToString(dateformat);
                        COUNTERPARTY.Text = r.COUNTERPARTY.NAME;
                    }
                    break;

                case "DISPATCH":
                    if (Grid.SelectedItem is DISPATCH d)
                    {
                        ID.Text = d.REQUEST.REQUEST_ID.ToString();
                        DATE.Text = d.DATE.ToString(dateformat);
                        CD_O_R.Text = "№" + d.FK_REQUEST_ID.ToString() + " от " + d.REQUEST.DATE.ToString(dateformat);
                        COUNTERPARTY.Text = d.REQUEST.COUNTERPARTY.NAME;
                    }
                    break;

                case "RETAIL_REPORT":
                    if (Grid.SelectedItem is RETAIL_REPORT r_r)
                    {
                        ID.Text = r_r.RETAIL_REPORT_ID.ToString();
                        DATE.Text = r_r.DATE.ToString(dateformat);
                        COUNTERPARTY.Text = r_r.COUNTERPARTY.NAME;
                    }
                    break;

                default:
                    return;
            }
            
            Load_LIST(Choice, LIST, ID);
        }

        public void Load_LIST(string Choice, DataGrid LIST, TextBlock ID)
        {
            if (int.TryParse(ID.Text, out int id))
            {
                switch (Choice)
                {
                    case "ORDER":
                        LIST.ItemsSource = db.ORDER_LIST.Where(o => o.FK_ORDER_ID == id).ToList();
                        break;

                    case "PURCHASE_INVOICE":
                        LIST.ItemsSource = db.ORDER_LIST.Where(o => o.FK_ORDER_ID == id).ToList();
                        break;

                    case "REQUEST":
                        LIST.ItemsSource = db.REQUEST_LIST.Where(o => o.FK_REQUEST_ID == id).ToList();
                        break;

                    case "DISPATCH":
                        LIST.ItemsSource = db.REQUEST_LIST.Where(o => o.FK_REQUEST_ID == id).ToList();
                        break;

                    case "RETAIL_REPORT":
                        LIST.ItemsSource = db.RETAIL_REPORT_LIST.Where(o => o.FK_RETAIL_REPORT_ID == id).ToList();
                        break;

                    default:
                        return;
                }
            }
        }

        public void BC_Arrived_Orders(DataGrid Grid)
        {
            Grid.ItemsSource = (
                from ORDER in db.ORDERs
                join PURCHASE_INVOICE in db.PURCHASE_INVOICE on ORDER.ORDER_ID equals PURCHASE_INVOICE.FK_ORDER_ID
                select new
                {
                    ORDER = "№" + ORDER.ORDER_ID,
                    COUNTERPARTY = ORDER.COUNTERPARTY.NAME,
                    ORDER_DATE = ORDER.DATE,
                    ORDER.CUTOFF_DATE,
                    PURCHASE_INVOICE = "№" + PURCHASE_INVOICE.PURCHASE_INVOICE_ID,
                    PURCHASE_INVOICE_DATE = PURCHASE_INVOICE.DATE,
                    STATUS = ORDER.CUTOFF_DATE < PURCHASE_INVOICE.DATE ? "Просрочен" : ""
                }).ToList();
        }
        public void BC_Completed_Requests(DataGrid Grid)
        {
            Grid.ItemsSource = (
                from REQUEST in db.REQUESTs
                join DISPATCH in db.DISPATCHes on REQUEST.REQUEST_ID equals DISPATCH.FK_REQUEST_ID
                select new
                {
                    REQUEST = "№" + REQUEST.REQUEST_ID,
                    COUNTERPARTY = REQUEST.COUNTERPARTY.NAME,
                    REQUEST_DATE = REQUEST.DATE,
                    REQUEST.CUTOFF_DATE,
                    DISPATCH = "№" + DISPATCH.DISPATCH_ID,
                    DISPATCH_DATE = DISPATCH.DATE,
                    STATUS = REQUEST.CUTOFF_DATE < DISPATCH.DATE ? "Просрочен" : ""
                }).ToList();
        }
        public void BC_Products_Sold(DataGrid Grid)
        {
            Grid.ItemsSource = (
                from RETAIL_REPORT_LIST in db.RETAIL_REPORT_LIST
                group RETAIL_REPORT_LIST by RETAIL_REPORT_LIST.NOMENCLATURE.NAME into NOMENCLATUREGroup
                select new
                {
                    NOMENCLATURE = NOMENCLATUREGroup.Key,
                    QUANTITY = NOMENCLATUREGroup.Sum(s => s.QUANTITY),
                    SUM = NOMENCLATUREGroup.Sum(s => s.SUM)
                }).ToList();
        }
        public void BC_Counterparty_Rating(DataGrid Grid)
        {
            Grid.ItemsSource = (
                from RATING in db.RATINGs
                group RATING by RATING.COUNTERPARTY into COUNTERPARTYGroup
                select new
                {
                    COUNTERPARTY = COUNTERPARTYGroup.Key.NAME,
                    CATEGORY = COUNTERPARTYGroup.Key.CATEGORY_C.NAME,
                    RATING = COUNTERPARTYGroup.OrderByDescending(c => c.DATE).FirstOrDefault().RATING1
                }).OrderBy(c => c.CATEGORY).ThenByDescending(r => r.RATING).ToList();
        }
    }
}
