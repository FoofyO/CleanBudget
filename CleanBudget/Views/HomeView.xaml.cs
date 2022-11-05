using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;

namespace CleanBudget.Views
{
    public partial class HomeView : UserControl
    {
        public StackPanel CurrentPanel { get; set; }
        public static Brush NavBarGray { get; set; }
        public static Brush NavBarWhiteBrush { get; set; }
        public static Brush NavBarBlueBrush { get; set; }
        public static Brush NavBarLightBlueBrush { get; set; }

        public HomeView()
        {
            InitializeComponent();
            NavBarGray = (Brush)Application.Current.Resources["NavBarGray"];
            NavBarWhiteBrush = (Brush)Application.Current.Resources["NavBarWhite"];
            NavBarBlueBrush = (Brush)Application.Current.Resources["NavBarOrange"];
            NavBarLightBlueBrush = (Brush)Application.Current.Resources["NavBarDarkGray"];
            SetPanel(Account);
        }

        private void StackPanel_MouseEnter(object sender, MouseEventArgs e)
        {
            if (CurrentPanel != (StackPanel)sender)
            {
                var panel = (StackPanel)sender;
                var childrens = panel.Children;
                var icon = (MaterialDesignThemes.Wpf.PackIcon)childrens[0];
                var title = (Label)childrens[1];
                panel.Background = NavBarLightBlueBrush;
                icon.Foreground = title.Foreground = NavBarWhiteBrush;
            }
        }

        private void StackPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            if (CurrentPanel != (StackPanel)sender)
            {
                var panel = (StackPanel)sender;
                var childrens = panel.Children;
                var icon = (MaterialDesignThemes.Wpf.PackIcon)childrens[0];
                var title = (Label)childrens[1];
                panel.Background = Brushes.Transparent;
                icon.Foreground = title.Foreground = NavBarGray;
            }
        }

        private void SetPanel(object sender, MouseButtonEventArgs e)
        {
            if (sender != null)
            {
                if (CurrentPanel != (StackPanel)sender)
                {
                    DisablePanel();
                    SetPanel((StackPanel)sender);
                }
            }
        }

        private void SetPanel(StackPanel panel)
        {
            CurrentPanel = panel;
            CurrentPanel.Background = NavBarBlueBrush;
            (CurrentPanel.Children[0] as MaterialDesignThemes.Wpf.PackIcon).Foreground =
            (CurrentPanel.Children[1] as Label).Foreground = NavBarWhiteBrush;
        }

        private void DisablePanel()
        {
            if (CurrentPanel != null)
            {
                CurrentPanel.Background = Brushes.Transparent;
                (CurrentPanel.Children[0] as MaterialDesignThemes.Wpf.PackIcon).Foreground =
                (CurrentPanel.Children[1] as Label).Foreground = NavBarGray;
            }
        }
    }
}
