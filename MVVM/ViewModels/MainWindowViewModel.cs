using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using MVVM.Models;
using MVVM.Services;
using MVVM.Tools;
using MVVM.Views;
using RelayCommand = MVVM.Tools.RelayCommand;

namespace MVVM.ViewModels;

public class MainWindowViewModel : BaseVM
{
    private readonly DataService _dataService = new DataService();
    private Order _selectedOrder;
    private string _filterStatus = "All";
    private string _searchText = "";
    private int _nextId = 1;

    public ObservableCollection<string> Statuses { get; set; } = new ObservableCollection<string>
    {
        "All", "New", "Processing", "Delivering", "Completed"
    };
    
    public ObservableCollection<Order> Orders { get; set; } = new ObservableCollection<Order>();
    public ObservableCollection<Order> FilteredOrders { get; set; } = new ObservableCollection<Order>();

    public Order SelectedOrder
    {
        get => _selectedOrder;
        set
        {
            _selectedOrder = value;
            OnPropertyChanged();
            ((RelayCommand)NextStatusCommand).RaiseCanExecuteChanged();
            ((RelayCommand)PreviousStatusCommand).RaiseCanExecuteChanged();
            ((RelayCommand)ShowReceiptCommand).RaiseCanExecuteChanged();
        }
    }

    public string FilterStatus
    {
        get => _filterStatus;
        set
        {
            _filterStatus = value;
            OnPropertyChanged();
            ApplyFilter();
        }
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            _searchText = value;
            OnPropertyChanged();
            ApplyFilter();
        }
    }

    public ICommand AddOrderCommand { get; }
    public ICommand NextStatusCommand { get; }
    public ICommand PreviousStatusCommand { get; }
    public ICommand ShowReceiptCommand { get; }

    public MainWindowViewModel()
    {
        AddOrderCommand = new RelayCommand(AddOrder);
        NextStatusCommand = new RelayCommand(NextStatus, () => SelectedOrder != null);
        PreviousStatusCommand = new RelayCommand(PreviousStatus, () => SelectedOrder != null);
        ShowReceiptCommand = new RelayCommand(ShowReceipt, () => SelectedOrder != null);

        LoadOrders();
    }

    private void LoadOrders()
    {
        var loaded = _dataService.LoadOrders();
        foreach (var order in loaded)
            Orders.Add(order);

        _nextId = _dataService.GetNextId(loaded);
        ApplyFilter();
    }

    private void AddOrder()
    {
        var order = new Order
        {
            Id = _nextId++,
            ClientName = "Иван Иванов",
            Address = "ул. Пример, д. 1",
            Items = "Товар1, Товар2"
        };
        Orders.Add(order);
        _dataService.SaveOrders(Orders.ToList());
        ApplyFilter();
    }

    private void NextStatus()
    {
        if (SelectedOrder != null)
        {
            SelectedOrder.NextStatus();
            _dataService.SaveOrders(Orders.ToList());
            ApplyFilter();
        }
    }

    private void PreviousStatus()
    {
        if (SelectedOrder != null)
        {
            SelectedOrder.PreviousStatus();
            _dataService.SaveOrders(Orders.ToList());
            ApplyFilter();
        }
    }

    private void ShowReceipt()
    {
        if (SelectedOrder != null)
        {
            var receipt = SelectedOrder.GenerateReceipt();
            var window = new Views.ReceiptWindow(receipt);
            window.Show();
        }
    }

    private void ApplyFilter()
    {
        FilteredOrders.Clear();

        var filtered = Orders.Where(o =>
            (FilterStatus == "All" || o.Status.ToString() == FilterStatus) &&
            (string.IsNullOrEmpty(SearchText) ||
             o.ClientName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
             o.Address.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
        );

        foreach (var order in filtered)
            FilteredOrders.Add(order);

        OnPropertyChanged(nameof(FilteredOrders));

        if (SelectedOrder != null && !FilteredOrders.Contains(SelectedOrder))
            SelectedOrder = FilteredOrders.FirstOrDefault();
    }
}
