using System;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace CleanBudget.Views
{
    public partial class OperationsView : UserControl
    {
        public OperationsView()
        {
            InitializeComponent();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => (sender as ListBox).SelectedItem = null;
    }
}
