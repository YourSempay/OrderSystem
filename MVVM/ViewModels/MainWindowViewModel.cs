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

public partial class MainWindowViewModel : BaseVM
{
private readonly DataService _dataService = new DataService();
    private Order _selectedOrder;
    private string _filterStatus = "All";

    public ObservableCollection<Order> Orders { get; set; } = new ObservableCollection<Order>();
    public ObservableCollection<Order> FilteredOrders { get; set; } = new ObservableCollection<Order>();

    public Order SelectedOrder
    {
        get => _selectedOrder;
        set { _selectedOrder = value; OnPropertyChanged(); }
    }

    public string FilterStatus
    {
        get => _filterStatus;
        set { _filterStatus = value; OnPropertyChanged(); ApplyFilter(); }
    }

    public ICommand AddOrderCommand { get; }
    public ICommand NextStatusCommand { get; }
    public ICommand PreviousStatusCommand { get; }
    public ICommand ShowReceiptCommand { get; }

    public MainWindowViewModel()
    {
        LoadOrders();

        AddOrderCommand = new RelayCommand(AddOrder);
        NextStatusCommand = new RelayCommand(NextStatus, () => SelectedOrder != null);
        PreviousStatusCommand = new RelayCommand(PreviousStatus, () => SelectedOrder != null);
        ShowReceiptCommand = new RelayCommand(ShowReceipt, () => SelectedOrder != null);
    }

    private void LoadOrders()
    {
        var loaded = _dataService.LoadOrders();
        foreach (var order in loaded)
            Orders.Add(order);

        _nextId = _dataService.GetNextId(loaded);
        ApplyFilter();
    }

    private int _nextId = 1;

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
        SelectedOrder?.NextStatus();
        _dataService.SaveOrders(Orders.ToList());
        ApplyFilter();
    }

    private void PreviousStatus()
    {
        SelectedOrder?.PreviousStatus();
        _dataService.SaveOrders(Orders.ToList());
        ApplyFilter();
    }

    private void ShowReceipt()
    {
        if (SelectedOrder != null)
        {
            var receipt = SelectedOrder.GenerateReceipt();
            var receiptWindow = new Views.ReceiptWindow(receipt);
            receiptWindow.Show();
        }
    }

    private void ApplyFilter()
    {
        FilteredOrders.Clear();
        foreach (var order in Orders)
        {
            if (FilterStatus == "All" || order.Status.ToString() == FilterStatus)
                FilteredOrders.Add(order);
        }
        OnPropertyChanged(nameof(FilteredOrders));
    }
}
