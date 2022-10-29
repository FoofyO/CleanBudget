using System;
using System.Windows;
using CleanBudget.Views;
using CleanBudget.Messages;
using CleanBudget.ViewModels;
using GalaSoft.MvvmLight.Messaging;
using CleanBudget.Services.Repositories;

namespace CleanBudget
{
    public partial class App : Application
    {
        public static MainWindow window;
        public static IMessenger messenger;
        public static HomeViewModel homeViewModel;
        public static MainViewModel mainViewModel;
        public static LoginViewModel loginViewModel;
        public static UserRepository userRepository;
        public static AccountRepository accountRepository;
        public static RegisterViewModel registerViewModel;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            messenger = new Messenger();
            messenger.Register<Resize>(this, Resize, true);
            userRepository = new UserRepository();
            accountRepository = new AccountRepository();
            mainViewModel = new MainViewModel(messenger);
            homeViewModel = new HomeViewModel(messenger);
            loginViewModel = new LoginViewModel(messenger, userRepository);
            registerViewModel = new RegisterViewModel(messenger, userRepository, accountRepository);

            mainViewModel.CurrentViewModel = loginViewModel;

            window = new MainWindow();
            window.DataContext = mainViewModel;
            window.ShowDialog();
        }

        public void Resize(Object obj)
        {
            var message = obj as Resize;
            window.Width = message.Width;
            window.Height = message.Height;
            window.MaxHeight = SystemParameters.PrimaryScreenHeight;
            window.MaxWidth = SystemParameters.PrimaryScreenHeight; ;
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            if (message.CanResize)
            {
                window.ResizeMode = ResizeMode.CanResizeWithGrip;
                window.Maximize.IsEnabled = true;
            }
            else
            {
                window.ResizeMode = ResizeMode.CanMinimize;
                window.Maximize.IsEnabled = false;
            }
        }
    }
}
