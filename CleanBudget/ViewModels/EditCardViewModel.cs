using System;
using CleanBudget.Models;
using CleanBudget.Messages;
using CleanBudget.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using CleanBudget.Services.Repositories;

namespace CleanBudget.ViewModels
{
    public class EditCardViewModel : BaseViewModel
    {
        #region Variables
        //Services
        private IMessenger _messenger;
        private CardRepository _cardRepository;
        private CurrencyRepository _currencyRepository;

        //Checker
        public bool Checker { get; set; } = true;

        //Models
        public Card CurrentCard { get; set; } = null;
        public Guid CurrentCardId { get; set; } = Guid.Empty;
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
        public RelayCommand UpdateCommand { get; set; }
        public RelayCommand BackCommand { get; set; }
        public RelayCommand LoadCommand { get; set; }
        #endregion

        public EditCardViewModel(IMessenger messenger, CardRepository cardRepository, CurrencyRepository currencyRepository)
        {
            _messenger = messenger;
            _cardRepository = cardRepository;
            _currencyRepository = currencyRepository;
            BackCommand = new RelayCommand(Back);
            LoadCommand = new RelayCommand(Loaded);
            UpdateCommand = new RelayCommand(Update);
            _messenger.Register<SendCard>(this, ReceiveCard, true);
        }

        private void Update()
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
                    CurrentCard.Icon = CardIcon;
                    CurrentCard.Color = CardColor;
                    CurrentCard.Title = CardTitle;
                    CurrentCard.Balance = CardBalance;
                    CurrentCard.CurrencyId = CardCurrency.Id;
                    CurrentCard.Description = CardDescription;
                    _cardRepository.Update(CurrentCard);
                    _messenger.Send(new RefreshCards(true));
                    Back();
                }
                catch (Exception) { }
            });
        }

        private void Loaded() => Task.Run(() =>
        {
            if (Currencies.Count == 0 && Icons.Count == 0)
            {
                Currencies = (List<Currency>)_currencyRepository.GetAll();
                Icons.AddRange(new List<string>() { "CreditCardOutline", "PiggyBankOutline", "WalletOutline", "WalletTravel", "ReceiptTextOutline", "SafeSquareOutline", "Cash", "CurrencyUsd", "CurrencyEur", "CurrencyGbp", "CurrencyBtc" });
            }

            CurrentCard = _cardRepository.GetById(CurrentCardId);
            CardIcon = CurrentCard.Icon;
            CardColor = CurrentCard.Color;
            CardTitle = CurrentCard.Title;
            CardBalance = CurrentCard.Balance;
            CardDescription = CurrentCard.Description;
            CardCurrency = Currencies[Currencies.FindIndex(c => c.Id == CurrentCard.CurrencyId)];
        });

        private void Back()
        {
            CardTitle = TitleValidation = CardDescription = string.Empty;
            _messenger.Send(new NavBar("Cards"));
        }
        
        private void ReceiveCard(SendCard obj) => CurrentCardId = obj.CardId;
    }
}
