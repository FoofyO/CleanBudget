using CleanBudget.Messages;
using CleanBudget.Models;
using CleanBudget.Services;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using CleanBudget.Services.Repositories;
using System.Windows;

namespace CleanBudget.ViewModels
{
    public class AddCardViewModel : BaseViewModel
    {
        #region Variables
        //Services
        private IMessenger _messenger;
        private CardRepository _cardRepository;
        private AccountRepository _accountRepository;
        private CurrencyRepository _currencyRepository;

        //Checker
        public bool Checker { get; set; } = true;

        //Models
        public Guid CurrentAccountId { get; set; } = Guid.Empty;
        public List<string> Icons { get; set; } = new List<string>();
        public List<Currency> Currencies { get; set; } = new List<Currency>();

        //Form
        public double CardBalance { get; set; } = 0;
        public Currency CardCurrency { get; set; } = null;
        public string CardIcon { get; set; } = string.Empty;
        public string CardColor { get; set; } = string.Empty;
        public string CardTitle { get; set; } = string.Empty;
        public string TitleValidation { get; set; } = string.Empty;
        public string CardDescription { get; set; } = string.Empty;

        //Commands
        public RelayCommand AddCommand { get; set; }
        public RelayCommand BackCommand { get; set; }
        public RelayCommand LoadCommand { get; set; }
        #endregion

        public AddCardViewModel(IMessenger messenger, CardRepository cardRepository, AccountRepository accountRepository, CurrencyRepository currencyRepository)
        {
            _messenger = messenger;
            _cardRepository = cardRepository;
            _accountRepository = accountRepository;
            _currencyRepository = currencyRepository;
            AddCommand = new RelayCommand(Add);
            BackCommand = new RelayCommand(Back);
            LoadCommand = new RelayCommand(Loaded);
            _messenger.Register<SendAccount>(this, ReceiveAccount, true);
        }

        private void Add()
        {
            Checker = true;

            //Title
            if (CardTitle == string.Empty)
            {
                Checker = false;
                TitleValidation = "* Fill Title Field";
            }
            else if (!ValidatorExtensions.IsTitleValid(CardTitle))
            {
                Checker = false;
                TitleValidation = "* Invalid Title";
            }
            else TitleValidation = string.Empty;

            if (Checker) Task.Run(() =>
            {
                try
                {
                    int cardCount = _accountRepository.GetCardsCount(CurrentAccountId) + 1;
                    _cardRepository.Create(new Card(CardTitle, CardDescription, CardBalance, CardCurrency.Id, cardCount, CardIcon, CardColor, CurrentAccountId));
                    _messenger.Send(new RefreshCards(true));
                    Back();
                }
                catch (Exception) { }
            });
        }

        private void Loaded() => Task.Run(() =>
        {
            if (Currencies.Count == 0)
            {
                CardBalance = 0;
                CardColor = "#FF000000";
                Icons.AddRange(new List<string>() { "CreditCardOutline", "PiggyBankOutline", "WalletOutline", "WalletTravel", "ReceiptTextOutline", "SafeSquareOutline", "Cash", "CurrencyUsd", "CurrencyEur", "CurrencyGbp", "CurrencyBtc" });
                Currencies = (List<Currency>)_currencyRepository.GetAll();
                CardIcon = Icons[0];
                CardCurrency = Currencies[0];
            }
        });

        private void Back()
        {
            CardBalance = 0;
            CardColor = "#FF000000";
            CardIcon = Icons[0];
            CardCurrency = Currencies[0];
            CardTitle = TitleValidation = CardDescription = string.Empty;
            _messenger.Send(new NavBar() { View = "Cards" });
        }
        
        private void ReceiveAccount(SendAccount obj) => CurrentAccountId = obj.AccountId;
    }
}
