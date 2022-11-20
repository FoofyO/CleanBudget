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
    public class EditCategoryViewModel : BaseViewModel
    {
        #region Variables
        //Services
        private IMessenger _messenger;
        private CategoryRepository _categoryRepository;
        private CurrencyRepository _currencyRepository;

        //Checker
        public bool Checker { get; set; } = true;

        //Models
        public Category CurrentCategory { get; set; } = null;
        public Guid CurrentCategoryId { get; set; } = Guid.Empty;
        public List<string> Icons { get; set; } = new List<string>();
        public List<Currency> Currencies { get; set; } = new List<Currency>();

        //Form
        public Currency CategoryCurrency { get; set; } = null;
        public string CategoryIcon { get; set; } = string.Empty;
        public string CategoryColor { get; set; } = string.Empty;
        public string CategoryTitle { get; set; } = string.Empty;
        public string TitleValidation { get; set; } = string.Empty;

        //Commands
        public RelayCommand UpdateCommand { get; set; }
        public RelayCommand BackCommand { get; set; }
        public RelayCommand LoadCommand { get; set; }
        #endregion

        public EditCategoryViewModel(IMessenger messenger, CategoryRepository categoryRepository, CurrencyRepository currencyRepository)
        {
            _messenger = messenger;
            _categoryRepository = categoryRepository;
            _currencyRepository = currencyRepository;
            BackCommand = new RelayCommand(Back);
            LoadCommand = new RelayCommand(Loaded);
            UpdateCommand = new RelayCommand(Update);
            _messenger.Register<SendCategory>(this, ReceiveCategory, true);
        }

        private void Update()
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
                    CurrentCategory.Icon = CategoryIcon;
                    CurrentCategory.Color = CategoryColor;
                    CurrentCategory.Title = CategoryTitle;
                    CurrentCategory.CurrencyId = CategoryCurrency.Id;
                    _categoryRepository.Update(CurrentCategory);
                    _messenger.Send(new RefreshCategories(true));
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
            }

            CurrentCategory = _categoryRepository.GetById(CurrentCategoryId);
            CategoryIcon = CurrentCategory.Icon;
            CategoryColor = CurrentCategory.Color;
            CategoryTitle = CurrentCategory.Title;
            CategoryCurrency = Currencies[Currencies.FindIndex(c => c.Id == CurrentCategory.CurrencyId)];
        });

        private void Back()
        {
            CategoryTitle = TitleValidation = string.Empty;
            _messenger.Send(new NavBar("Categories"));
        }

        private void ReceiveCategory(SendCategory obj) => CurrentCategoryId = obj.CategoryId;
    }
}
