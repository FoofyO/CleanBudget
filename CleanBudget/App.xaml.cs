using System;
using System.Windows;
using CleanBudget.Views;
using CleanBudget.Messages;
using CleanBudget.ViewModels;
using GalaSoft.MvvmLight.Messaging;
using CleanBudget.Services.Repositories;

namespace CleanBudget
{
    public partial class App : Application
    {
        #region Variables
        //Application
        public static MainWindow window;
        public static IMessenger messenger;
        
        //View Models
        public static HomeViewModel homeViewModel;
        public static MainViewModel mainViewModel;
        public static CardsViewModel cardsViewModel;
        public static LoginViewModel loginViewModel;
        public static AccountViewModel accountViewModel;
        public static AddCardViewModel addCardViewModel;
        public static EditCardViewModel editCardViewModel;
        public static RegisterViewModel registerViewModel;
        public static OperationsViewModel operationsViewModel;
        public static CategoriesViewModel categoriesViewModel;
        public static AddCategoryViewModel addCategoryViewModel;
        public static EditCategoryViewModel editCategoryViewModel;
        
        //Service
        public static CardRepository cardRepository;
        public static UserRepository userRepository;
        public static AccountRepository accountRepository;
        public static CategoryRepository categoryRepository;
        public static CurrencyRepository currencyRepository;
        public static DeductOperationRepository deductRepository;
        public static RefillOperationRepository refillRepository;
        public static TransferOperationRepository transferRepository;
        #endregion

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            messenger = new Messenger();
            messenger.Register<Resize>(this, Resize, true);
            messenger.Register<Exit>(this, Exit, true);

            InitializeService();
            InitializeViewModels();

            mainViewModel.CurrentViewModel = loginViewModel;

            window = new MainWindow();
            window.DataContext = mainViewModel;
            window.ShowDialog();
        }

        private void InitializeService()
        {
            cardRepository = new CardRepository();
            userRepository = new UserRepository();
            accountRepository = new AccountRepository();
            categoryRepository = new CategoryRepository();
            currencyRepository = new CurrencyRepository();
            deductRepository = new DeductOperationRepository();
            refillRepository = new RefillOperationRepository();
            transferRepository = new TransferOperationRepository();
        }

        private void InitializeViewModels()
        {
            mainViewModel = new MainViewModel(messenger);
            homeViewModel = new HomeViewModel(messenger);
            loginViewModel = new LoginViewModel(messenger, userRepository);
            editCardViewModel = new EditCardViewModel(messenger, cardRepository, currencyRepository);
            editCategoryViewModel = new EditCategoryViewModel(messenger, categoryRepository, currencyRepository);
            addCardViewModel = new AddCardViewModel(messenger, cardRepository, accountRepository, currencyRepository);
            accountViewModel = new AccountViewModel(messenger, userRepository, accountRepository, currencyRepository);
            registerViewModel = new RegisterViewModel(messenger, userRepository, accountRepository, currencyRepository);
            operationsViewModel = new OperationsViewModel(messenger, deductRepository, refillRepository, transferRepository);
            addCategoryViewModel = new AddCategoryViewModel(messenger, categoryRepository, accountRepository, currencyRepository);
            cardsViewModel = new CardsViewModel(messenger, accountRepository, cardRepository, refillRepository, transferRepository);
            categoriesViewModel = new CategoriesViewModel(messenger, accountRepository, categoryRepository, cardRepository, deductRepository);
        }

        private void Resize(object obj)
        {
            var message = obj as Resize;
            window.Width = message.Width;
            window.Height = message.Height;

            if (message.CanResize) window.ResizeMode = ResizeMode.CanResizeWithGrip;
            else window.ResizeMode = ResizeMode.CanMinimize;

            if (message.IsMaximized)
            {
                window.WindowState = WindowState.Maximized;
                window.ResizeMode = ResizeMode.NoResize;
            }
            else window.WindowState = WindowState.Normal;
        }

        private void Exit(object obj) => Current.Shutdown();
    }
}
