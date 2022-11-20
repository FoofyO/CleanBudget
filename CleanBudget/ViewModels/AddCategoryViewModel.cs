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
    public class AddCategoryViewModel : BaseViewModel
    {
        #region Variables
        //Services
        private IMessenger _messenger;
        private CategoryRepository _categoryRepository;
        private AccountRepository _accountRepository;
        private CurrencyRepository _currencyRepository;

        //Checker
        public bool Checker { get; set; } = true;

        //Models
        public Guid CurrentAccountId { get; set; } = Guid.Empty;
        public List<string> Icons { get; set; } = new List<string>();
        public List<Currency> Currencies { get; set; } = new List<Currency>();

        //Form
        public Currency CategoryCurrency { get; set; } = null;
        public string CategoryIcon { get; set; } = string.Empty;
        public string CategoryColor { get; set; } = string.Empty;
        public string CategoryTitle { get; set; } = string.Empty;
        public string TitleValidation { get; set; } = string.Empty;

        //Commands
        public RelayCommand AddCommand { get; set; }
        public RelayCommand BackCommand { get; set; }
        public RelayCommand LoadCommand { get; set; }
        #endregion

        public AddCategoryViewModel(IMessenger messenger, CategoryRepository categoryRepository, AccountRepository accountRepository, CurrencyRepository currencyRepository)
        {
            _messenger = messenger;
            _categoryRepository = categoryRepository;
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
            if (CategoryTitle == string.Empty)
            {
                Checker = false;
                TitleValidation = "* Fill Title Field";
            }
            else if (!ValidatorExtensions.IsTitleValid(CategoryTitle))
            {
                Checker = false;
                TitleValidation = "* Invalid Title";
            }
            else TitleValidation = string.Empty;

            if (Checker) Task.Run(() =>
            {
                try
                {
                    int categoryCount = _accountRepository.GetCategoryCount(CurrentAccountId) + 1;
                    _categoryRepository.Create(new Category(CategoryTitle, CategoryCurrency.Id, categoryCount, CategoryIcon, CategoryColor, CurrentAccountId));
                    _messenger.Send(new RefreshCategories(true));
                    Back();
                }
                catch (Exception) { }
            });
        }

        private void Loaded() => Task.Run(() =>
        {
            if (Currencies.Count == 0)
            {
                CategoryColor = "#FF000000";
                Icons.AddRange(new List<string>() { "BasketOutline", "CartOutline", "StoreOutline", 
                    "SilverwareForkKnife", "CoffeeOutline", "BeerOutline", 
                    "Hamburger", "FoodDrumstickOutline", "Pizza", 
                    "GlassCocktail", "GlassMugVariant", "BottleTonicOutline", 
                    "Smoking", "Leaf", "CandyOutline", "Candycane", 
                    "CakeVariantOutline", "Filmstrip", "Cellphone", 
                    "TelevisionPlay", "Laptop", "Domain", "Home", 
                    "FlashOutline", "WaterOutline", "Fire", "PhoneOutline", 
                    "FridgeOutline", "WashingMachine", "SeatOutline", 
                    "SofaOutline", "HardHat", "Broom", "ShoppingOutline", 
                    "SaleOutline", "Hanger", "TshirtCrewOutline", "ShoeSneaker", 
                    "FaceMan", "BabyCarriage", "GenderMale", "GenderFemale", "Web", 
                    "Translate", "BookOpenPageVariant", "SchoolOutline", "MedicalBag", 
                    "Bandage", "Basketball", "Dumbbell", "Paw", "Bone", "MusicNoteOutline", 
                    "Speaker", "PaletteOutline", "GiftOutline", "AirplaneTakeoff", "Earth", 
                    "Bed", "ImageFilterHdr", "Snowflake", "WhiteBalanceSunny", 
                    "UmbrellaBeachOutline", "Sunglasses", "Ferry", "Car", "GasStationOutline", 
                    "Parking", "Taxi", "Bus", "SubwayVariant", "TruckOutline", "Bike", "RacingHelmet", 
                    "WrenchOutline", "Dice5", "ControllerClassic", "RobotOutline", "PacMan", "HandCoinOutline", 
                    "Gavel", "Pin", "DotsHorizontal" });
                Currencies = (List<Currency>)_currencyRepository.GetAll();
                CategoryIcon = Icons[0];
                CategoryCurrency = Currencies[0];
            }
        });

        private void Back()
        {
            CategoryColor = "#FF000000";
            CategoryIcon = Icons[0];
            CategoryCurrency = Currencies[0];
            CategoryTitle = TitleValidation = string.Empty;
            _messenger.Send(new NavBar("Categories"));
        }

        private void ReceiveAccount(SendAccount obj) => CurrentAccountId = obj.AccountId;
    }
}
