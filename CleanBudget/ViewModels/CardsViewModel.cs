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
        private CardRepository _cardRepository;
        private AccountRepository _accountRepository;
        private RefillOperationRepository _refillRepository;
        private TransferOperationRepository _transferRepository;

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

        //Modals
        public bool RefillDialog { get; set; } = false;
        public bool TransferDialog { get; set; } = false;
        public Card TransfableCard { get; set; } = null;
        public double RefillCardBalance { get; set; } = 0;
        public double TransferCardMaximum { get; set; } = 0;
        public double TransferCardBalance { get; set; } = 0;
        public bool NotEnoughCardsDialog { get; set; } = false;
        public List<Card> TransfableCards { get; set; } = new List<Card>();


        //Commands
        public RelayCommand LoadCommand { get; set; }
        public RelayCommand AddCardCommand { get; set; }
        public RelayCommand EditCardCommand { get; set; }
        public RelayCommand DeleteCardCommand { get; set; }
        public RelayCommand RefillClosedCommand { get; set; }
        public RelayCommand RefillBalanceCommand { get; set; }
        public RelayCommand RefillNavigationCommand { get; set; }
        public RelayCommand TransferClosedCommand { get; set; }
        public RelayCommand TransferBalanceCommand { get; set; }
        public RelayCommand TransferNavigationCommand { get; set; }
        public RelayCommand<string> ChangeQueueCommand { get; set; }
        #endregion

        public CardsViewModel(IMessenger messenger, AccountRepository accountRepository, CardRepository cardRepository, RefillOperationRepository refillRepository, TransferOperationRepository transferRepository)
        {
            _messenger = messenger;
            _cardRepository = cardRepository;
            _refillRepository = refillRepository;
            _accountRepository = accountRepository;
            _transferRepository = transferRepository;
            LoadCommand = new RelayCommand(Loaded);
            AddCardCommand = new RelayCommand(AddCard);
            EditCardCommand = new RelayCommand(EditCard);
            DeleteCardCommand = new RelayCommand(DeleteCard);
            
            RefillClosedCommand = new RelayCommand(RefillClosed);
            RefillBalanceCommand = new RelayCommand(RefillBalance);
            RefillNavigationCommand = new RelayCommand(RefillNavigation);
            
            TransferClosedCommand = new RelayCommand(TransferClosed);
            TransferBalanceCommand = new RelayCommand(TransferBalance);
            TransferNavigationCommand = new RelayCommand(TransferNavigation);
            ChangeQueueCommand = new RelayCommand<string>(ChangeQueue);
            
            _messenger.Register<RefreshCards>(this, Refreshing, true);
            _messenger.Register<SendAccount>(this, ReceiveAccount, true);
        }

        private void TransferClosed() => Task.Run(() =>
        {
            TransfableCards.Clear();
            TransferCardBalance = 0;
            TransferCardMaximum = 0;
            TransfableCard = null;
        });
        private void TransferNavigation()
        {
            if (Cards.Count > 1)
            {
                if (SelectedCard != null)
                {
                    TransferCardMaximum = SelectedCard.Balance;
                    TransfableCards = Cards.ToList();
                    TransfableCards.Remove(SelectedCard);
                    TransfableCard = TransfableCards[0];
                    TransferDialog = true;
                }
            } else NotEnoughCardsDialog = true;
        }
        private void TransferBalance() => Task.Run(() =>
        {   
            if (TransferCardBalance > 0)
            {           
                var transfer = Math.Round(TransferCardBalance, 2);
                _cardRepository.UpdateBalance(SelectedCard.Id, Math.Round(SelectedCard.Balance - transfer, 2));
                _transferRepository.Create(new TransferOperation(transfer, SelectedCard.Id, CurrentAccountId, SelectedCard.Currency.Id, TransfableCard.Id));
                if (!TransfableCard.Currency.ShortName.Equals(SelectedCard.Currency.ShortName))
                {
                    transfer = CurrencyConverter.Convert(SelectedCard.Currency.ShortName, TransfableCard.Currency.ShortName, transfer);
                }
                _cardRepository.UpdateBalance(TransfableCard.Id, Math.Round(TransfableCard.Balance + transfer, 2));
                _messenger.Send(new RefreshOperations(true));
                Refresh = true;
                Loaded();
            }
            TransferClosed();
            TransferDialog = false;
        });

        private void RefillBalance() => Task.Run(() =>
        {
            RefillDialog = false;
            if (RefillCardBalance != 0)
            {
                var refill = Math.Round(RefillCardBalance, 2);
                _cardRepository.UpdateBalance(SelectedCard.Id, SelectedCard.Balance + refill);
                _refillRepository.Create(new RefillOperation(refill, SelectedCard.Id, CurrentAccountId, SelectedCard.Currency.Id));
                _messenger.Send(new RefreshOperations(true));
                RefillCardBalance = 0;
                Refresh = true;
                Loaded();
            }
        });
        private void RefillClosed() => RefillCardBalance = 0;
        private void RefillNavigation()
        {
            if (Cards.Count > 0)
            {
                if (SelectedCard != null) RefillDialog = true;
            }
            else NotEnoughCardsDialog = true;
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
                        TotalBalance += CurrencyConverter.Convert(card.Currency.ShortName, CurrentCurrency, card.Balance);
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
            if (Cards.Count > 0)
            {
                if (SelectedCard != null)
                {
                    IsSpin = true;
                    IsViewVisible = "Hidden";
                    IsSpinnerVisible = "Visible";
                    _messenger.Send(new SendCard(SelectedCard.Id));
                    _messenger.Send(new NavBar("EditCard"));
                }
            }
            else NotEnoughCardsDialog = true;
        }
        
        private void DeleteCard() => Task.Run(() =>
        {
            if (Cards.Count > 0)
            {

                if (SelectedCard != null)
                {
                    Spinner(true);

                    foreach (var card in Cards)
                    {
                        if (card.Queue > SelectedCard.Queue)
                        {
                            _cardRepository.UpdateQueue(card.Id, --card.Queue);
                        }
                    }

                    _cardRepository.Delete(SelectedCard);

                    if (Cards.Count > 1)
                    {
                        if (!CurrentCurrency.Equals(SelectedCard.Currency.ShortName))
                        {
                            TotalBalance -= CurrencyConverter.Convert(SelectedCard.Currency.ShortName.ToLower(), CurrentCurrency.ToLower(), SelectedCard.Balance);
                        }
                        else TotalBalance -= SelectedCard.Balance;

                        TotalBalance = Math.Round(TotalBalance, 2);
                    }
                    else TotalBalance = 0;
                    
                    Application.Current.Dispatcher.Invoke(() => Cards.Remove(SelectedCard));

                    Spinner(false);
                }
            }
            else NotEnoughCardsDialog = true;
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
            if (Cards.Count > 0)
            {
                if (SelectedCard != null)
                {
                    if (Cards.Count > 1)
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
                }
            }
            else NotEnoughCardsDialog = true;
        }
        
        private void ReceiveAccount(SendAccount obj)
        {
            Refresh = true;
            Spinner(true);
            CurrentAccountId = obj.AccountId;
        }
        
        private void Refreshing(RefreshCards obj) => Refresh = obj.Refresh;
    }
}