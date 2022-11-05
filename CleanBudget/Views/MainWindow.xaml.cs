using System;
using System.Windows;
using MahApps.Metro.Controls;

namespace CleanBudget.Views
{
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            MinWidth = 450;
            MinHeight = 530;
            MaxWidth = SystemParameters.PrimaryScreenWidth;
            MaxHeight = SystemParameters.PrimaryScreenHeight;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }
    }
}
