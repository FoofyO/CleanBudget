using System;
using LiveCharts;
using System.Linq;
using LiveCharts.Wpf;
using System.Windows;
using CleanBudget.Models;
using LiveCharts.Defaults;
using CleanBudget.Messages;
using CleanBudget.Services;
using System.Windows.Media;
using System.Threading.Tasks;
using System.Collections.Generic;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.ObjectModel;
using CleanBudget.Services.Repositories;
using System.Reflection.PortableExecutable;

namespace CleanBudget.ViewModels
{
    public class CategoriesViewModel : BaseViewModel
    {
        #region Variables
        //Services
        private IMessenger _messenger;
        private CardRepository _cardRepository;
        private AccountRepository _accountRepository;
        private CategoryRepository _categoryRepository;
        private DeductOperationRepository _deductRepository;

        //Model
        public Category SelectedCategory { get; set; }
        public int PrevQueue { get; set; } = 0;
        public bool Checker { get; set; } = true;
        public bool Refresh { get; set; } = true;
        public Guid CurrentAccountId { get; set; }
        public Account CurrentAccount { get; set; }
        public Category ChangableCategory { get; set; } = null;

        //Form
        public bool IsSpin { get; set; } = false;
        public double TotalConsumptions { get; set; } = 0;
        public SeriesCollection CategorySeries { get; set; }
        public string IsViewVisible { get; set; } = "Hidden";
        public string IsSpinnerVisible { get; set; } = "Hidden";
        public string CurrentCurrency { get; set; } = string.Empty;
        public ObservableCollection<Category> Categories { get; set; }

        //Modals
        public bool CardsDialog { get; set; } = false;
        public bool DeductDialog { get; set; } = false;
        public Card AvailableCard { get; set; } = null;
        public bool CategoryDialog { get; set; } = false;
        public double AvailableCardMaximum { get; set; } = 0;
        public double AvailableCardBalance { get; set; } = 0;
        public List<Card> AvailableCards { get; set; } = new List<Card>();

        //Commands
        public RelayCommand LoadCommand { get; set; }
        public RelayCommand AddCategoryCommand { get; set; }
        public RelayCommand EditCategoryCommand { get; set; }
        public RelayCommand DeleteCategoryCommand { get; set; }
        public RelayCommand DeductClosedCommand { get; set; }
        public RelayCommand DeductCategoryCommand { get; set; }
        public RelayCommand DeductCardChangedCommand { get; set; }
        public RelayCommand DeductNavigationCommand { get; set; }
        public RelayCommand<string> ChangeQueueCommand { get; set; }
        #endregion

        public CategoriesViewModel(IMessenger messenger, AccountRepository accountRepository, CategoryRepository categoryRepository, CardRepository cardRepository, DeductOperationRepository deductRepository)
        {
            _messenger = messenger;
            _cardRepository = cardRepository;
            _deductRepository = deductRepository;
            _accountRepository = accountRepository;
            _categoryRepository = categoryRepository;
            LoadCommand = new RelayCommand(Loaded);
            AddCategoryCommand = new RelayCommand(AddCategory);
            EditCategoryCommand = new RelayCommand(EditCategory);
            DeleteCategoryCommand = new RelayCommand(DeleteCategory);
            DeductClosedCommand = new RelayCommand(DeductClosed);
            DeductCategoryCommand = new RelayCommand(DeductCategory);
            DeductNavigationCommand = new RelayCommand(DeductNavigation);
            DeductCardChangedCommand = new RelayCommand(DeductCardChanged);
            ChangeQueueCommand = new RelayCommand<string>(ChangeQueue);
            _messenger.Register<RefreshCategories>(this, Refreshing, true);
            _messenger.Register<SendAccount>(this, ReceiveAccount, true);
        }

        private void DeductCategory()
        {
            if (AvailableCardBalance > 0)
            {
                var deduct = Math.Round(AvailableCardBalance, 2);
                _cardRepository.UpdateBalance(AvailableCard.Id, Math.Round(AvailableCard.Balance - AvailableCardBalance, 2));
                _deductRepository.Create(new DeductOperation(deduct, AvailableCard.Id, CurrentAccountId, AvailableCard.Currency.Id, SelectedCategory.Id));
                if (!AvailableCard.Currency.ShortName.Equals(SelectedCategory.Currency.ShortName))
                {
                    deduct = CurrencyConverter.Convert(AvailableCard.Currency.ShortName, SelectedCategory.Currency.ShortName, deduct);
                }
                _categoryRepository.UpdateConsumption(SelectedCategory.Id, Math.Round(SelectedCategory.Consumption + deduct, 2));
                _messenger.Send(new RefreshOperations(true));
                Refresh = true;
                Loaded();
            }
            DeductClosed();
            DeductDialog = false;
        }
        private void DeductClosed()
        {
            AvailableCards.Clear();
            AvailableCardBalance = 0;
            AvailableCardMaximum = 0;
            AvailableCard = null;
        }
        private void DeductNavigation()
        {
            if (Categories.Count > 0)
            {
                Task.Run(() =>
                {
                    AvailableCards = (List<Card>)_cardRepository.GetAccountCards(CurrentAccountId);
                    if (AvailableCards.Count > 0)
                    {
                        AvailableCard = AvailableCards[0];
                        AvailableCardMaximum = AvailableCard.Balance;
                        DeductDialog = true;
                    }
                    else CardsDialog = true;
                });
            }
            else CategoryDialog = true;
        }
        private void DeductCardChanged()
        {
            if (AvailableCard != null) AvailableCardMaximum = AvailableCard.Balance;
        }

        private void Loaded() => Task.Run(() =>
        {
            Spinner(true);
            var categories = (List<Category>)_categoryRepository.GetAccountCategories(CurrentAccountId);

            if (Refresh)
            {
                Refresh = false;
                CurrentAccount = _accountRepository.GetById(CurrentAccountId);
                CurrentCurrency = CurrentAccount.Currency.ShortName;

                Categories = new ObservableCollection<Category>();
                if (categories.Count > 0)
                {
                    foreach (var category in categories)
                    {
                        Application.Current.Dispatcher.Invoke(() => Categories.Add(category));
                    }
                }

                TotalConsumptions = 0;
                if (categories.Count > 0)
                {
                    foreach (var category in categories)
                    {
                        if (!CurrentCurrency.Equals(category.Currency.ShortName))
                        {
                            TotalConsumptions += CurrencyConverter.Convert(category.Currency.ShortName, CurrentCurrency, category.Consumption);
                        }
                        else TotalConsumptions += category.Consumption;
                    }
                    TotalConsumptions = Math.Round(TotalConsumptions, 2);
                }
            }
            
            CategorySeries = new SeriesCollection();
            if (categories.Count > 0)
            {
                foreach (var category in categories)
                {
                    Application.Current.Dispatcher.Invoke(() => CategorySeries.Add(new PieSeries()
                    {
                        Title = category.Title,
                        Values = new ChartValues<ObservableValue> { new ObservableValue(category.Consumption) },
                        Fill = new BrushConverter().ConvertFromString(category.Color) as Brush
                    }));
                }
            }

            Spinner(false);
        });

        private void AddCategory()
        {
            IsSpin = true;
            IsViewVisible = "Hidden";
            IsSpinnerVisible = "Visible";
            _messenger.Send(new NavBar("AddCategory"));
        }

        private void EditCategory()
        {
            if (Categories.Count > 0)
            {
                if (SelectedCategory != null)
                {
                    IsSpin = true;
                    IsViewVisible = "Hidden";
                    IsSpinnerVisible = "Visible";
                    _messenger.Send(new SendCategory(SelectedCategory.Id));
                    _messenger.Send(new NavBar("EditCategory"));
                }
            }
            else CategoryDialog = true;
        }

        private void DeleteCategory() => Task.Run(() =>
        {
            if (Categories.Count > 0) 
            { 
                if (SelectedCategory != null)
                {
                    Spinner(true);

                    foreach (var category in Categories)
                    {
                        if (category.Queue > SelectedCategory.Queue)
                        {
                            _categoryRepository.UpdateQueue(category.Id, --category.Queue);
                        }
                    }

                    _categoryRepository.Delete(SelectedCategory);

                    if (Categories.Count > 1)
                    {
                        if (!CurrentCurrency.Equals(SelectedCategory.Currency.ShortName))
                        {
                            TotalConsumptions -= CurrencyConverter.Convert(SelectedCategory.Currency.ShortName, CurrentCurrency, SelectedCategory.Consumption);
                        }
                        else TotalConsumptions -= SelectedCategory.Consumption;

                        TotalConsumptions = Math.Round(TotalConsumptions, 2);
                    }
                    else TotalConsumptions = 0;
                    
                    Application.Current.Dispatcher.Invoke(() => Categories.Remove(SelectedCategory));

                    Spinner(false);
                }
            }
            else CategoryDialog = true;
            
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
                if (SelectedCategory.Queue != 0) PrevQueue = SelectedCategory.Queue--;
                else Checker = false;
            }
            else
            {
                if (SelectedCategory.Queue != Categories.Count) PrevQueue = SelectedCategory.Queue++;
                else Checker = false;
            }

            if (Checker)
            {
                ChangableCategory = Categories.ElementAt(SelectedCategory.Queue - 1);
                ChangableCategory.Queue = PrevQueue;
                Categories.Move(PrevQueue - 1, SelectedCategory.Queue - 1);
                Task.Run(() =>
                {
                    _categoryRepository.UpdateQueue(SelectedCategory.Id, SelectedCategory.Queue);
                    _categoryRepository.UpdateQueue(ChangableCategory.Id, ChangableCategory.Queue);
                });
            }
        }

        private void ReceiveAccount(SendAccount obj)
        {
            Refresh = true;
            Spinner(true);
            CurrentAccountId = obj.AccountId;
        }
        
        private void Refreshing(RefreshCategories obj) => Refresh = obj.Refresh;
    }
}
