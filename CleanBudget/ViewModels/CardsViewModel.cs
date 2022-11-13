using System;
using System.Linq;
using System.Windows;
using CleanBudget.Models;
using CleanBudget.Messages;
using CleanBudget.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.ObjectModel;
using CleanBudget.Services.Repositories;

namespace CleanBudget.ViewModels
{
    public class CardsViewModel : BaseViewModel
    {
        #region Variables
        //Services
        private IMessenger _messenger;
        private AccountRepository _accountRepository;
        private CardRepository _cardRepository;

        //Model
        public Card SelectedCard { get; set; }
        public int PrevQueue { get; set; } = 0;
        public bool Checker { get; set; } = true;
        public bool Refresh { get; set; } = true;
        public Guid CurrentAccountId { get; set; }
        public Account CurrentAccount { get; set; }
        public Card ChangableCard { get; set; } = null;

        //Form
        public bool IsSpin { get; set; } = false;
        public double TotalBalance { get; set; } = 0;
        public ObservableCollection<Card> Cards { get; set; }
        public string IsViewVisible { get; set; } = "Hidden";
        public string IsSpinnerVisible { get; set; } = "Hidden";
        public string CurrentCurrency { get; set; } = string.Empty;

        //Commands
        public RelayCommand LoadCommand { get; set; }
        public RelayCommand AddCardCommand { get; set; }
        public RelayCommand EditCardCommand { get; set; }
        public RelayCommand DeleteCardCommand { get; set; }
        public RelayCommand<string> ChangeQueueCommand { get; set; }
        #endregion

        public CardsViewModel(IMessenger messenger, AccountRepository accountRepository, CardRepository cardRepository)
        {
            _messenger = messenger;
            _accountRepository = accountRepository;
            _cardRepository = cardRepository;
            LoadCommand = new RelayCommand(Loaded);
            AddCardCommand = new RelayCommand(AddCard);
            EditCardCommand = new RelayCommand(EditCard);
            DeleteCardCommand = new RelayCommand(DeleteCard);
            ChangeQueueCommand = new RelayCommand<string>(ChangeQueue);
            _messenger.Register<RefreshCards>(this, Refreshing, true);
            _messenger.Register<SendAccount>(this, ReceiveAccount, true);
        }

        private void Loaded() => Task.Run(() =>
        {
            if (Refresh)
            {
                Spinner(true);
                Refresh = false;
                CurrentAccount = _accountRepository.GetById(CurrentAccountId);
                CurrentCurrency = CurrentAccount.Currency.ShortName;

                Cards = new ObservableCollection<Card>();
                foreach (var card in _cardRepository.GetAccountCards(CurrentAccountId))
                {
                    Application.Current.Dispatcher.Invoke(() => Cards.Add(card));
                }

                TotalBalance = 0;
                foreach (var card in Cards)
                {
                    if (!CurrentCurrency.Equals(card.Currency.ShortName))
                    {
                        TotalBalance += CurrencyConverter.Convert(card.Currency.ShortName.ToLower(), CurrentCurrency.ToLower(), card.Balance);
                    }
                    else TotalBalance += card.Balance;
                }
                TotalBalance = Math.Round(TotalBalance, 2);
            }
            Spinner(false);
        });
        
        private void AddCard()
        {
            IsSpin = true;
            IsViewVisible = "Hidden";
            IsSpinnerVisible = "Visible";
            _messenger.Send(new NavBar("AddCard"));
        }
        
        private void EditCard()
        {
            IsSpin = true;
            IsViewVisible = "Hidden";
            IsSpinnerVisible = "Visible";
            _messenger.Send(new SendCard(SelectedCard.Id));
            _messenger.Send(new NavBar("EditCard"));
        }
        
        private void DeleteCard() => Task.Run(() =>
        {
            Spinner(true);

            foreach (var card in Cards)
            {
                if (card.Queue > SelectedCard.Queue)
                {
                    card.Queue--;
                    _cardRepository.UpdateQueue(card.Id, card.Queue);
                }
            }

            _cardRepository.Delete(SelectedCard);

            if (!CurrentCurrency.Equals(SelectedCard.Currency.ShortName))
            {
                TotalBalance -= CurrencyConverter.Convert(SelectedCard.Currency.ShortName.ToLower(), CurrentCurrency.ToLower(), SelectedCard.Balance);
            }
            else TotalBalance -= SelectedCard.Balance;

            TotalBalance = Math.Round(TotalBalance, 2);
            Application.Current.Dispatcher.Invoke(() => Cards.Remove(SelectedCard));

            Spinner(false);
        });
        
        private void Spinner(bool flag)
        {
            if (flag)
            {
                IsSpin = true;
                IsViewVisible = "Hidden";
                IsSpinnerVisible = "Visible";
            }
            else
            {
                IsSpin = false;
                IsViewVisible = "Visible";
                IsSpinnerVisible = "Hidden";
            }
        }
        
        private void ChangeQueue(string type)
        {
            Checker = true;
            if (type.Equals("Up"))
            {
                if (SelectedCard.Queue != 0) PrevQueue = SelectedCard.Queue--;
                else Checker = false;
            }
            else
            {
                if (SelectedCard.Queue != Cards.Count) PrevQueue = SelectedCard.Queue++;
                else Checker = false;
            }

            if (Checker)
            {
                ChangableCard = Cards.ElementAt(SelectedCard.Queue - 1);
                ChangableCard.Queue = PrevQueue;
                Cards.Move(PrevQueue - 1, SelectedCard.Queue - 1);
                Task.Run(() =>
                {
                    _cardRepository.UpdateQueue(SelectedCard.Id, SelectedCard.Queue);
                    _cardRepository.UpdateQueue(ChangableCard.Id, ChangableCard.Queue);
                });
            }
        }
        
        private void Refreshing(RefreshCards obj) => Refresh = obj.Refresh;
        
        private void ReceiveAccount(SendAccount obj) => CurrentAccountId = obj.AccountId;
    }
}