using System;
using System.Windows.Input;
using System.Windows.Controls;

namespace CleanBudget.Views
{
    public partial class CategoriesView : UserControl
    {
        public CategoriesView()
        {
            InitializeComponent();
        }

        private void ListBox_Click(object sender, MouseEventArgs e) => listbox.SelectedItem = null;
    }
}
