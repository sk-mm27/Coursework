using System.Windows;

namespace ProOdezhda.Windows.Test
{
    public partial class TestWin : Window
    {
        public TestWin()
        {
            InitializeComponent();
        }
        public void Clear()
        {
            ComboBox1.ItemsSource = null; 
            ComboBox1.SelectedValuePath = null;
            ComboBox1.Items.Clear();
            ComboBox2.ItemsSource = null;
            ComboBox2.SelectedValuePath = null;
            ComboBox2.Items.Clear();
            ComboBox3.ItemsSource = null;
            ComboBox3.SelectedValuePath = null;
            ComboBox3.Items.Clear();
            DataGrid.ItemsSource = null; 
            DataGrid.Items.Clear();
            TextBox1.Clear();
            TextBox2.Clear();
        }
    }
}
