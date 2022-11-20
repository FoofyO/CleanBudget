using System;
using System.Windows;
using CleanBudget.Models;
using CleanBudget.Messages;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.ObjectModel;
using CleanBudget.Services.Repositories;
using System.Collections.Generic;

namespace CleanBudget.ViewModels
{
    public class OperationsViewModel : BaseViewModel
    {
        #region Variables
        //Services
        private IMessenger _messenger;
        private DeductOperationRepository _deductRepository;
        private RefillOperationRepository _refillRepository;
        private TransferOperationRepository _transferRepository;

        //Model
        public bool Checker { get; set; } = true;
        public bool Refresh { get; set; } = true;
        public Guid CurrentAccountId { get; set; }
        public int TotalOperations { get; set; } = 0;

        //Form
        public bool IsSpin { get; set; } = false;
        public string IsViewVisible { get; set; } = "Hidden";
        public string IsSpinnerVisible { get; set; } = "Hidden";
        public ObservableCollection<DeductOperation> Deducts { get; set; }
        public ObservableCollection<RefillOperation> Refills { get; set; }
        public ObservableCollection<TransferOperation> Transfers { get; set; }

        //Commands
        public RelayCommand LoadCommand { get; set; }
        #endregion

        public OperationsViewModel(IMessenger messenger, DeductOperationRepository deductRepository, RefillOperationRepository refillRepository, TransferOperationRepository transferRepository)
        {
            _messenger = messenger;
            _deductRepository = deductRepository;
            _refillRepository = refillRepository;
            _transferRepository = transferRepository;
            LoadCommand = new RelayCommand(Loaded);
            _messenger.Register<SendAccount>(this, ReceiveAccount, true);
            _messenger.Register<RefreshOperations>(this, Refreshing, true);
        }

        private void Loaded() => Task.Run(() =>
        {
            if (Refresh)
            {
                Spinner(true);
                Refresh = false;
                TotalOperations = 0;
                var deducts = (List<DeductOperation>)_deductRepository.GetAccountOperations(CurrentAccountId);
                var transfers = (List<TransferOperation>)_transferRepository.GetAccountOperations(CurrentAccountId);
                var refills = (List<RefillOperation>)_refillRepository.GetAccountOperations(CurrentAccountId);

                Deducts = new ObservableCollection<DeductOperation>();
                Refills = new ObservableCollection<RefillOperation>();
                Transfers = new ObservableCollection<TransferOperation>();

                if (deducts.Count > 0)
                {
                    foreach (var deduct in deducts)
                    {
                        TotalOperations++;
                        Application.Current.Dispatcher.Invoke(() => Deducts.Add(deduct));
                    }
                }

                if (transfers.Count > 0)
                {
                    foreach (var transfer in transfers)
                    {
                        TotalOperations++;
                        Application.Current.Dispatcher.Invoke(() => Transfers.Add(transfer));
                    }
                }

                if (refills.Count > 0)
                {
                    foreach (var refill in refills)
                    {
                        TotalOperations++;
                        Application.Current.Dispatcher.Invoke(() => Refills.Add(refill));
                    }
                }
            }
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

        private void ReceiveAccount(SendAccount obj)
        {
            Refresh = true;
            Spinner(true);
            CurrentAccountId = obj.AccountId;
        }

        private void Refreshing(RefreshOperations obj) => Refresh = obj.Refresh;
    }
}
