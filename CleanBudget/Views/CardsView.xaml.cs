﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CleanBudget.Views
{
    public partial class CardsView : UserControl
    {
        public CardsView()
        {
            InitializeComponent();
        }

        private void ListBox_Click(object sender, MouseEventArgs e) => listbox.SelectedItem = null;
    }
}
