using System;
using CleanBudget.Messages;
using GalaSoft.MvvmLight.Messaging;

namespace CleanBudget.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private IMessenger messenger;
        public BaseViewModel CurrentViewModel { get; set; }

        public MainViewModel(IMessenger messenger)
        {
            this.messenger = messenger;
            this.messenger.Register<Navigation>(this, Navigation, true);
        }

        private void Navigation(Object obj)
        {
            var message = obj as Navigation;
            var type = message.ViewModelType;

            if (type == typeof(LoginViewModel)) CurrentViewModel = App.loginViewModel;
            else if (type == typeof(RegisterViewModel)) CurrentViewModel = App.registerViewModel;
        }
    }
}
