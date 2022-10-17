using System;
using System.Windows;
using System.Windows.Input;

namespace CleanBudget.Views
{
    public partial class MainWindow : Window
    {
        private bool mouseDown;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized) this.WindowState = WindowState.Normal;
            else this.WindowState = WindowState.Maximized;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Titlebar_Move(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if ((ResizeMode == ResizeMode.CanResize) || (ResizeMode == ResizeMode.CanResizeWithGrip))
                {
                    if (this.WindowState == WindowState.Maximized) this.WindowState = WindowState.Normal;
                    else this.WindowState = WindowState.Maximized;
                }
            }
            else
            {
                if (this.WindowState == WindowState.Maximized) mouseDown = true;
                DragMove();
            }
        }

        private void Titlebar_Mouse_Up(object sender, MouseButtonEventArgs e)
        {
            mouseDown = false;
        }

        private void Titlebar_Mouse_Move(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                var mouseX = e.GetPosition(this).X;
                var width = RestoreBounds.Width;
                var x = mouseX - width / 2;

                if (x < 0) x = 0;
                else
                {
                    if (x + width > SystemParameters.PrimaryScreenWidth) x = SystemParameters.PrimaryScreenWidth - width;

                    WindowState = WindowState.Normal;
                    Left = x;
                    Top = 0;
                    DragMove();
                }
            }
        }
    }
}
