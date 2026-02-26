using MVVM.Tools;

namespace MVVM.ViewModels;

public class ReceiptWindowViewModel: BaseVM
{
    private string _receiptText;

    public string ReceiptText
    {
        get => _receiptText;
        set
        {
            _receiptText = value;
            OnPropertyChanged();
        }
    }

    public ReceiptWindowViewModel(string receiptText)
    {
        _receiptText = receiptText;
    }
}