using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ProOdezhda;
using ProOdezhda.Windows;
using System.Linq;
using ProOdezhda.Windows.Test;
using System.Windows.Controls;

namespace ProOdezhdaTest
{
    [TestClass]
    public class ProOdezhdaTest
    {
        [TestClass]
        public class Working_With_Forms_Test
        {
            ProOdezhdaEntities db = new ProOdezhdaEntities();
            TestWin tw = new TestWin();
            Working_With_Forms wwf = new Working_With_Forms();

            [TestMethod]
            public void Test1_Load_COUNTERPARTY_and_NOMENCLATURE()
            {
                // Тест Load_COUNTERPARTY_and_NOMENCLATURE: Load_NOMENCLATURE, Load_COUNTERPARTY
                wwf.Load_COUNTERPARTY_and_NOMENCLATURE(tw.ComboBox1, "Агент", tw.ComboBox2, "Продукция");
                Assert.AreEqual("NAME", tw.ComboBox1.DisplayMemberPath); Assert.AreEqual("COUNTERPARTY_ID", tw.ComboBox1.SelectedValuePath);
                Assert.AreEqual("NAME", tw.ComboBox2.DisplayMemberPath); Assert.AreEqual("NOMENCLATURE_ID", tw.ComboBox2.SelectedValuePath);

                foreach (COUNTERPARTY c in tw.ComboBox1.Items) { Assert.AreEqual("Агент", c.CATEGORY_C.NAME); }
                foreach (NOMENCLATURE c in tw.ComboBox2.Items) { Assert.AreEqual("Продукция", c.CATEGORY_N.NAME); }

                wwf.Load_COUNTERPARTY_and_NOMENCLATURE(tw.ComboBox1, "Поставщик", tw.ComboBox2, "Материал");

                foreach (COUNTERPARTY c in tw.ComboBox1.Items) { Assert.AreEqual("Поставщик", c.CATEGORY_C.NAME); }
                foreach (NOMENCLATURE c in tw.ComboBox2.Items) { Assert.AreEqual("Материал", c.CATEGORY_N.NAME); }
            }

            [TestMethod]
            public void Test2_Load_ORDER()
            {
                // Тест Load_ORDER
                tw.Clear();

                foreach (ORDER o in db.ORDERs)
                {
                    if (!db.PURCHASE_INVOICE.Any(p_i => p_i.FK_ORDER_ID == o.ORDER_ID))
                    {
                        wwf.Load_ORDER(tw.ComboBox1, o.FK_COUNTERPARTY_ID);
                        Assert.AreEqual("ORDER_ID", tw.ComboBox1.SelectedValuePath);
                        foreach (ORDER c in tw.ComboBox1.Items) { Assert.AreEqual(o.FK_COUNTERPARTY_ID, c.FK_COUNTERPARTY_ID); }
                        goto Load_ORDER;
                    }
                }
                Assert.Fail("Тест Load_ORDER невозможен, т.к. отсутствуют заказы или на каждый из них оформлена накладная");
            Load_ORDER:;
            }

            [TestMethod]
            public void Test3_Load_REQUEST()
            {
                // Тест Load_REQUEST
                tw.Clear();

                foreach (REQUEST r in db.REQUESTs)
                {
                    if (!db.DISPATCHes.Any(d => d.FK_REQUEST_ID == r.REQUEST_ID))
                    {
                        wwf.Load_REQUEST(tw.ComboBox1, r.FK_COUNTERPARTY_ID);
                        Assert.AreEqual("REQUEST_ID", tw.ComboBox1.SelectedValuePath);
                        foreach (REQUEST c in tw.ComboBox1.Items) { Assert.AreEqual(r.FK_COUNTERPARTY_ID, c.FK_COUNTERPARTY_ID); }
                        goto Load_REQUEST;
                    }
                }
                Assert.Fail("Тест Load_REQUEST невозможен, т.к. отсутствуют запросы или на каждый из них оформлена поставка");
            Load_REQUEST:;
            }

            [TestMethod]
            public void Test4_REQUEST_or_ORDER_Selection()
            {
                // Тест REQUEST_or_ORDER_Selection
                tw.Clear();

                foreach (ORDER o in db.ORDERs)
                {
                    if (!db.PURCHASE_INVOICE.Any(i => i.FK_ORDER_ID == o.ORDER_ID))
                    {
                        wwf.Load_ORDER(tw.ComboBox1, o.COUNTERPARTY.COUNTERPARTY_ID);
                        tw.ComboBox1.SelectedValue = o.ORDER_ID;
                        wwf.REQUEST_or_ORDER_Selection("ORDER", tw.ComboBox1, tw.DataGrid);
                        Assert.IsFalse(tw.DataGrid.Items.Count == 0, "DaraGrid пуст");
                        goto ORDER_Selection;
                    }
                }
                Assert.Fail("Тест REQUEST_or_ORDER_Selection невозможен, т.к. отсутствуют заказы");
            ORDER_Selection:;

                tw.Clear();

                foreach (REQUEST r in db.REQUESTs)
                {
                    if (!db.DISPATCHes.Any(i => i.FK_REQUEST_ID == r.REQUEST_ID))
                    {
                        wwf.Load_REQUEST(tw.ComboBox1, r.COUNTERPARTY.COUNTERPARTY_ID);
                        tw.ComboBox1.SelectedValue = r.REQUEST_ID;
                        wwf.REQUEST_or_ORDER_Selection("REQUEST", tw.ComboBox1, tw.DataGrid);
                        Assert.IsFalse(tw.DataGrid.Items.Count == 0, "DaraGrid пуст");
                        goto REQUEST_Selection;
                    }
                }
                Assert.Fail("Тест REQUEST_or_ORDER_Selection невозможен, т.к. отсутствуют запросы");
            REQUEST_Selection:;
            }

            [TestMethod]
            public void Test5_IUDS()
            {
                // Тест Insert_List, Update_List, Delete_List и LIST_Selection_Changed
                tw.Clear();
                wwf.Load_NOMENCLATURE(tw.ComboBox3, "Продукция");

                tw.ComboBox3.SelectedIndex = 0;
                tw.TextBox1.Text = "100";
                tw.TextBox2.Text = "10";

                wwf.Insert_List(tw.DataGrid, tw.ComboBox3, tw.TextBox1, tw.TextBox2);
                Assert.IsTrue(tw.DataGrid.Items.Count == 1, "Запись не добавленна");

                tw.ComboBox3.SelectedIndex = 1;
                tw.TextBox1.Text = "10";
                tw.TextBox2.Text = "100";
                wwf.Insert_List(tw.DataGrid, tw.ComboBox3, tw.TextBox1, tw.TextBox2);
                Assert.IsTrue(tw.DataGrid.Items.Count == 2, "Запись не добавленна");

                tw.DataGrid.SelectedIndex = 0;

                wwf.LIST_Selection_Changed(tw.DataGrid, tw.ComboBox3, tw.TextBox1, tw.TextBox2);
                Assert.AreEqual(0, tw.ComboBox3.SelectedIndex);
                Assert.AreEqual("100", tw.TextBox1.Text);
                Assert.AreEqual("10", tw.TextBox2.Text);

                tw.DataGrid.SelectedIndex = 1;
                wwf.Delete_List(tw.DataGrid);
                Assert.IsTrue(tw.DataGrid.Items.Count == 1, "Запись не удалена");
            }
        }

        [TestClass]
        public class WindowTest
        {
            [TestClass]
            public class MainWindowTest
            {
                ProOdezhdaEntities db = new ProOdezhdaEntities();
                MainWindow mw = new MainWindow();

                [TestMethod]
                public void Test_Authorization()
                {
                    Assert.IsTrue(mw.Authorization("Менеджер", "692", true));
                    Assert.IsFalse(mw.Authorization("Дизайнер", "123", true));
                    Assert.IsFalse(mw.Authorization("", "", true));
                    Assert.IsFalse(mw.Authorization("Дизайнер", "", true));
                }
            }

            [TestClass]
            public class WinRegistrationTest
            {
                ProOdezhdaEntities db = new ProOdezhdaEntities();
                WinRegistration wreg = new WinRegistration();

                [TestMethod]
                public void Test_Save()
                {
                    int users = db.USERs.Count() + 1;
                    ComboBox a = new ComboBox();
                    a.ItemsSource = db.POSITIONs.ToList();
                    a.DisplayMemberPath = "NAME";
                    a.SelectedValuePath = "POSITION_ID";

                    a.SelectedIndex = 1;
                    Assert.IsTrue(wreg.Save("Матвей", "123", a, true)); Assert.AreEqual(users, db.USERs.Count());

                    Assert.IsFalse(wreg.Save("", "123", a, true)); Assert.AreEqual(users, db.USERs.Count());
                    Assert.IsFalse(wreg.Save("Матвей Скрипов", "123", a, true)); Assert.AreEqual(users, db.USERs.Count());
                    Assert.IsFalse(wreg.Save(" ", "123", a, true)); Assert.AreEqual(users, db.USERs.Count());
                    Assert.IsFalse(wreg.Save("Менеджер", "123", a, true)); Assert.AreEqual(users, db.USERs.Count());


                    Assert.IsFalse(wreg.Save("Матвей", "", a, true)); Assert.AreEqual(users, db.USERs.Count());
                    Assert.IsFalse(wreg.Save("Матвей", " ", a, true)); Assert.AreEqual(users, db.USERs.Count());
                    Assert.IsFalse(wreg.Save("Матвей", "Lord #1", a, true)); Assert.AreEqual(users, db.USERs.Count());
                    Assert.IsFalse(wreg.Save("Матвей", "пароль", a, true)); Assert.AreEqual(users, db.USERs.Count());
                    
                    a.SelectedValue = null;
                    Assert.IsFalse(wreg.Save("Матвей", "123", a, true)); Assert.AreEqual(users, db.USERs.Count());

                    db.USERs.Remove(db.USERs.Where(l => l.LOGIN == "Матвей").Where(p => p.PASSWORD == "123").FirstOrDefault());
                    db.SaveChanges();
                }
            }

            [TestClass]
            public class WinCOUNTERPARTYTest
            {
                ProOdezhdaEntities db = new ProOdezhdaEntities();
                WinCOUNTERPARTY wcoun = new WinCOUNTERPARTY();
                
                [TestMethod]
                public void Test1_Insert()
                {
                    int counterparty = db.COUNTERPARTies.Count() + 1;
                    wcoun.Filling("Рога и копыта", 0, "г. Томск", "102938746573", "927318456", "2348569374234", "Марченко Н.Н.", "9039563456", "cow@gmail.com", 0);
                    Assert.IsTrue(wcoun.Save(true)); Assert.AreEqual(counterparty, db.COUNTERPARTies.Count());

                    wcoun.Filling("", 2, "", "", "", "", "", "", "");
                    Assert.IsFalse(wcoun.Save(true)); Assert.AreEqual(counterparty, db.COUNTERPARTies.Count());

                    wcoun.Filling("Рога и копыта", 0, "", "", "", "", "", "", "");
                    Assert.IsFalse(wcoun.Save(true)); Assert.AreEqual(counterparty, db.COUNTERPARTies.Count());

                    wcoun.Filling("Рога и копыта", 0, "г. Томск", "324", "", "", "", "", "");
                    Assert.IsFalse(wcoun.Save(true)); Assert.AreEqual(counterparty, db.COUNTERPARTies.Count());

                    wcoun.Filling("Рога и копыта", 0, "г. Томск", "asffdgsfdhdgh", "", "", "", "", "");
                    Assert.IsFalse(wcoun.Save(true)); Assert.AreEqual(counterparty, db.COUNTERPARTies.Count());

                    wcoun.Filling("Рога и копыта", 0, "г. Томск", "102938746573", "234", "", "", "", "");
                    Assert.IsFalse(wcoun.Save(true)); Assert.AreEqual(counterparty, db.COUNTERPARTies.Count());

                    wcoun.Filling("Рога и копыта", 0, "г. Томск", "102938746573", "asfdgfdhh", "", "", "", "");
                    Assert.IsFalse(wcoun.Save(true)); Assert.AreEqual(counterparty, db.COUNTERPARTies.Count());

                    wcoun.Filling("Рога и копыта", 0, "г. Томск", "102938746573", "927318456", "324", "", "", "");
                    Assert.IsFalse(wcoun.Save(true)); Assert.AreEqual(counterparty, db.COUNTERPARTies.Count());

                    wcoun.Filling("Рога и копыта", 0, "г. Томск", "102938746573", "927318456", "asfdfgfhggfhg", "", "", "");
                    Assert.IsFalse(wcoun.Save(true)); Assert.AreEqual(counterparty, db.COUNTERPARTies.Count());

                    wcoun.Filling("Рога и копыта", 0, "г. Томск", "102938746573", "927318456", "2348569374234", "Марченко", "", "");
                    Assert.IsFalse(wcoun.Save(true)); Assert.AreEqual(counterparty, db.COUNTERPARTies.Count());

                    wcoun.Filling("Рога и копыта", 0, "г. Томск", "102938746573", "927318456", "2348569374234", "Марченко Н.Н.", "123", "");
                    Assert.IsFalse(wcoun.Save(true)); Assert.AreEqual(counterparty, db.COUNTERPARTies.Count());

                    wcoun.Filling("Рога и копыта", 0, "г. Томск", "102938746573", "927318456", "2348569374234", "Марченко Н.Н.", "asd", "");
                    Assert.IsFalse(wcoun.Save(true)); Assert.AreEqual(counterparty, db.COUNTERPARTies.Count());
                   
                    wcoun.Filling("Рога и копыта", 0, "г. Томск", "102938746573", "927318456", "2348569374234", "Марченко Н.Н.", "9039563456", "Привет!");
                    Assert.IsFalse(wcoun.Save(true)); Assert.AreEqual(counterparty, db.COUNTERPARTies.Count());
                    
                    wcoun.Filling("Рога и копыта", 0, "г. Томск", "102938746573", "927318456", "2348569374234", "Марченко Н.Н.", "9039563456", "cow @gmail.com");
                    Assert.IsFalse(wcoun.Save(true)); Assert.AreEqual(counterparty, db.COUNTERPARTies.Count());
                }

                Insert_and_Delete iad = new Insert_and_Delete();
                WorkWindow ww = new WorkWindow();
                
                [TestMethod]
                public void Test2_Delete()
                {
                    int counterparty = db.COUNTERPARTies.Count() - 1;
                    ww.COUNTERPARTY_Grid.SelectedIndex = ww.COUNTERPARTY_Grid.Items.Count - 1;
                    iad.DELETE("COUNTERPARTY", ww.COUNTERPARTY_Grid, null, null, null, null, null, true);
                    Assert.AreEqual(counterparty, db.COUNTERPARTies.Count());
                }
            }

            [TestClass]
            public class WinORDERTest
            {
                ProOdezhdaEntities db = new ProOdezhdaEntities();
                WinORDER wo = new WinORDER();

                [TestMethod]
                public void Test1_Insert()
                {
                    int orders = db.ORDERs.Count();
                    int list = db.ORDER_LIST.Count();
                    Assert.IsFalse(wo.Save(1,true));

                    wo.Filling(DateTime.Now, DateTime.Now.AddDays(-1), 0);
                    Assert.IsFalse(wo.Save(1, true));

                    wo.Filling(DateTime.Now, DateTime.Now.AddDays(1), 0);
                    Assert.IsFalse(wo.Save(1, true));

                    wo.Filling_LIST(0,"10","10");
                    wo.INC(); wo.INC(); wo.INC();
                    Assert.IsTrue(wo.Save(1, true)); 
                    Assert.AreEqual(orders + 1, db.ORDERs.Count());
                    Assert.AreEqual(list + 3, db.ORDER_LIST.Count());
                }

                WorkWindow ww = new WorkWindow();

                [TestMethod]
                public void Test2_Delete()
                {
                    int orders = db.ORDERs.Count() - 1;
                    ww.ORDER_Grid.SelectedIndex = ww.ORDER_Grid.Items.Count - 1;
                    ww.DO(true); Assert.AreEqual(orders, db.ORDERs.Count());
                }
            }

            [TestClass]
            public class WinPURCHASE_INVOICETest
            {
                ProOdezhdaEntities db = new ProOdezhdaEntities();
                WinPURCHASE_INVOICE wpi = new WinPURCHASE_INVOICE();

                [TestMethod]
                public void Test1_Insert()
                {
                    int purchase_invoice = db.PURCHASE_INVOICE.Count();
                    
                    Assert.IsFalse(wpi.Save(true));

                    wpi.Filling(DateTime.Now.AddYears(-100), 0, 0);
                    Assert.IsFalse(wpi.Save(true));

                    wpi.Filling(DateTime.Now, 0, 0);
                    Assert.IsTrue(wpi.Save(true));
                    Assert.AreEqual(purchase_invoice + 1, db.PURCHASE_INVOICE.Count());
                }

                WorkWindow ww = new WorkWindow();

                [TestMethod]
                public void Test2_Delete()
                {
                    int purchase_invoice = db.PURCHASE_INVOICE.Count() - 1;
                    ww.PURCHASE_INVOICE_Grid.SelectedIndex = ww.PURCHASE_INVOICE_Grid.Items.Count - 1;
                    ww.DPI(true); Assert.AreEqual(purchase_invoice, db.PURCHASE_INVOICE.Count());
                }
            }

            [TestClass]
            public class WinREQUESTTest
            {
                ProOdezhdaEntities db = new ProOdezhdaEntities();
                WinREQUEST wreq = new WinREQUEST();

                [TestMethod]
                public void Test1_Insert()
                {
                    int requests = db.REQUESTs.Count();
                    int list = db.REQUEST_LIST.Count();
                    Assert.IsFalse(wreq.Save( true));

                    wreq.Filling(DateTime.Now, DateTime.Now.AddDays(-1), 0);
                    Assert.IsFalse(wreq.Save(true));

                    wreq.Filling(DateTime.Now, DateTime.Now.AddDays(1), 0);
                    Assert.IsFalse(wreq.Save( true));

                    wreq.Filling_LIST(0, "10", "10");
                    wreq.INC(); wreq.INC(); wreq.INC();
                    Assert.IsTrue(wreq.Save(true));
                    Assert.AreEqual(requests + 1, db.REQUESTs.Count());
                    Assert.AreEqual(list + 3, db.REQUEST_LIST.Count());
                }

                WorkWindow ww = new WorkWindow();

                [TestMethod]
                public void Test2_Delete()
                {
                    int requests = db.REQUESTs.Count() - 1;
                    ww.REQUEST_Grid.SelectedIndex = ww.REQUEST_Grid.Items.Count - 1;
                    ww.DR(true); Assert.AreEqual(requests, db.REQUESTs.Count());
                }
            }

            [TestClass]
            public class WinDISPATCHTest
            {
                ProOdezhdaEntities db = new ProOdezhdaEntities();
                WinDISPATCH wd = new WinDISPATCH();

                [TestMethod]
                public void Test1_Insert()
                {
                    int dispatch = db.DISPATCHes.Count();

                    Assert.IsFalse(wd.Save(1, true));

                    wd.Filling(DateTime.Now.AddYears(-100), 0, 0);
                    Assert.IsFalse(wd.Save(1, true));

                    wd.Filling(DateTime.Now, 0, 0);
                    Assert.IsTrue(wd.Save(1, true), "Все запросы выполнены");
                    Assert.AreEqual(dispatch + 1, db.DISPATCHes.Count());
                }

                WorkWindow ww = new WorkWindow();

                [TestMethod]
                public void Test2_Delete()
                {
                    int dispatch = db.DISPATCHes.Count() - 1;
                    ww.DISPATCH_Grid.SelectedIndex = ww.DISPATCH_Grid.Items.Count - 1;
                    ww.DD(true); Assert.AreEqual(dispatch, db.DISPATCHes.Count());
                }
            }

            [TestClass]
            public class WinRETAIL_REPORTTest
            {
                ProOdezhdaEntities db = new ProOdezhdaEntities();
                WinRETAIL_REPORT wrr = new WinRETAIL_REPORT();

                [TestMethod]
                public void Test1_Insert()
                {
                    int retail_repoet = db.RETAIL_REPORT.Count();
                    int list = db.RETAIL_REPORT_LIST.Count();
                    Assert.IsFalse(wrr.Save(true));

                    wrr.Filling(DateTime.Now, 0);
                    Assert.IsFalse(wrr.Save(true));

                    wrr.Filling(DateTime.Now, 0);
                    Assert.IsFalse(wrr.Save(true));

                    wrr.Filling_LIST(0, "10", "10");
                    wrr.INC(); wrr.INC(); wrr.INC();
                    Assert.IsTrue(wrr.Save(true));
                    Assert.AreEqual(retail_repoet + 1, db.RETAIL_REPORT.Count());
                    Assert.AreEqual(list + 3, db.RETAIL_REPORT_LIST.Count());
                }

                WorkWindow ww = new WorkWindow();

                [TestMethod]
                public void Test2_Delete()
                {
                    int retail_repoet = db.RETAIL_REPORT.Count() - 1;
                    ww.RETAIL_REPORT_Grid.SelectedIndex = ww.RETAIL_REPORT_Grid.Items.Count - 1;
                    ww.DRR(true); Assert.AreEqual(retail_repoet, db.RETAIL_REPORT.Count());
                }
            }

            [TestClass]
            public class WorkWindowTest
            {
                ProOdezhdaEntities db = new ProOdezhdaEntities();
                WorkWindow ww = new WorkWindow();

                [TestMethod]
                public void Test1_Insert()
                {
                    int rating = db.RATINGs.Count();
                    ww.RMLBU();
                    Assert.IsFalse(ww.N_R(true));

                    ww.RATING_Filling(DateTime.Now, 0, "2erdsf");
                    Assert.IsFalse(ww.N_R(true));

                    ww.RATING_Filling(DateTime.Now, 0, "13,4"); 
                    Assert.IsTrue(ww.N_R());

                    Assert.AreEqual(rating + 1, db.RATINGs.Count());
                }

                [TestMethod]
                public void Test2_Update()
                {
                    ww.RATING_Grid.SelectedIndex = ww.RATING_Grid.Items.Count - 1;
                    if (ww.RATING_Grid.SelectedItem is RATING rating1) 
                    {
                        ww.RATING_Filling(DateTime.Now.AddDays(1), 0, "asfds");
                        Assert.IsFalse(ww.U_R(true));

                        ww.RATING_Filling(DateTime.Now.AddDays(1), 0, "20,7");
                        Assert.IsTrue(ww.U_R(true));
                        if (ww.RATING_Grid.SelectedItem is RATING rating2) 
                        { Assert.AreEqual(rating1, rating2); }
                        else
                        { Assert.Fail("Ошибка конвертации (2)"); }
                    }
                    else { Assert.Fail("Ошибка конвертации (1)"); }
                }

                [TestMethod]
                public void Test3_Delete()
                {
                    int rating = db.RATINGs.Count();

                    ww.RATING_Grid.SelectedIndex = ww.RATING_Grid.Items.Count - 1;
                    Assert.IsTrue(ww.D_R());

                    Assert.AreEqual(rating - 1, db.RATINGs.Count());
                }
            }
        }
        [TestClass]
        public class ReportsTest
        {
            ProOdezhdaEntities db = new ProOdezhdaEntities();
            WorkWindow ww = new WorkWindow();
            MainFunctionality mf = new MainFunctionality();

            [TestMethod]
            public void Test_Formation()
            {
                mf.BC_Arrived_Orders(ww.Arrived_Orders);
                Assert.IsFalse(ww.Arrived_Orders.Items.Count == 0, "Нет прибывших заказов");

                mf.BC_Completed_Requests(ww.Completed_Requests);
                Assert.IsFalse(ww.Completed_Requests.Items.Count == 0, "Нет выполненных запросов");

                mf.BC_Products_Sold(ww.Products_Sold);
                Assert.IsFalse(ww.Products_Sold.Items.Count == 0, "Нет розничных отчётов");

                mf.BC_Counterparty_Rating(ww.Counterparty_Rating);
                Assert.IsFalse(ww.Counterparty_Rating.Items.Count == 0, "Нет рейтингов");
            }
        }
    }
}
