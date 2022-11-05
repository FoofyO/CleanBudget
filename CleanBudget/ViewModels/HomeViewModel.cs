using System;
using CleanBudget.Models;
using CleanBudget.Messages;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace CleanBudget.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        #region Properties
        //Services
        private IMessenger _messenger;
        public BaseViewModel CurrentViewModel { get; set; }

        //Commands
        public RelayCommand LoadCommand { get; set; }
        public RelayCommand<String> NavigationCommand { get; set; }
        #endregion

        public HomeViewModel(IMessenger messenger)
        {
            _messenger = messenger;
            LoadCommand = new RelayCommand(Loaded);
            NavigationCommand = new RelayCommand<String>(Navigaton);
        }

        public void Navigaton(String message)
        {
            if (message.Equals("Account")) CurrentViewModel = App.accountViewModel;
            else if (message.Equals("Exit")) _messenger.Send<Exit>(new Exit());
            //else if (message.Equals("Categories")) CurrentViewModel = App.loginViewModel;
            //else if (message.Equals("Operations")) CurrentViewModel = App.loginViewModel;
        }

        public void Loaded()
        {
            CurrentViewModel = App.accountViewModel;
            _messenger.Send<Resize>(new Resize(450, 1100, true));
        }
    }
}
