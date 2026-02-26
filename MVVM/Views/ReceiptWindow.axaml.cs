using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MVVM.ViewModels;

namespace MVVM.Views;

public partial class ReceiptWindow : Window
{
    public ReceiptWindow(string receiptText)
    {
        InitializeComponent();
        DataContext = new ReceiptWindowViewModel(receiptText);
    }

    private void CloseButton_Click(object? sender, RoutedEventArgs e)
    {
        this.Close();
    }
}