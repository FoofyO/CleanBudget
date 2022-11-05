using System;
using CleanBudget.Messages;
using GalaSoft.MvvmLight.Messaging;

namespace CleanBudget.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private IMessenger _messenger;
        public BaseViewModel CurrentViewModel { get; set; }

        public MainViewModel(IMessenger messenger)
        {
            _messenger = messenger;
            _messenger.Register<Navigation>(this, Navigation, true);
        }

        private void Navigation(Navigation obj)
        {
            Type type = obj.ViewModelType;
            
            if (type == typeof(LoginViewModel)) CurrentViewModel = App.loginViewModel;
            else if (type == typeof(RegisterViewModel)) CurrentViewModel = App.registerViewModel;
            else if (type == typeof(HomeViewModel)) CurrentViewModel = App.homeViewModel;
        }
    }
}
