using SpeakAI.Services.Interfaces;
using SpeakAI.Services.Models;

namespace SpeakAI.Views;

public partial class PaymentWebViewPage : ContentPage
{
    private readonly string _orderId;
    private readonly string _userId;
    private readonly IUserService _userService;
    private readonly ICourseService _courseService;
    private bool _isLoading;

    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            _isLoading = value;
            OnPropertyChanged(nameof(IsLoading));
        }
    }

    public PaymentWebViewPage(string paymentUrl, string orderId, IUserService userService, string userId, ICourseService courseService)
    {
        InitializeComponent();
        BindingContext = this; // Set BindingContext for IsLoading to work
        _orderId = orderId;
        _userId = userId;
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _courseService = courseService ?? throw new ArgumentNullException(nameof(courseService));
        PaymentWebView.Source = paymentUrl;
        PaymentWebView.Navigated += OnWebNavigated;
    }

    private async void OnWebNavigated(object sender, WebNavigatedEventArgs e)
    {
        try
        {
            if (!e.Url.Contains("waiting-checkout"))
                return;

            await DisplayAlertSafe("Payment", "Payment processing...", "OK");

            // Parse query parameters
            var queryParams = new Uri(e.Url).Query.TrimStart('?')
                .Split('&')
                .Select(param => param.Split('='))
                .ToDictionary(kv => kv[0], kv => kv.Length > 1 ? kv[1] : "");

            string orderInfo = queryParams.GetValueOrDefault("vnp_OrderInfo", "");
            string transactionNo = queryParams.GetValueOrDefault("vnp_TransactionNo", "");
            string transactionStatus = queryParams.GetValueOrDefault("vnp_TransactionStatus", "");

            bool isSuccess = transactionStatus == "00";
            var transactionModel = new TransactionModel
            {
                userId = _userId,
                transactionInfo = orderInfo,
                transactionNumber = transactionNo,
                isSuccess = isSuccess
            };

            // Process payment response with loading
            IsLoading = true;
            var responsePayment = await _userService.ResponsePayment(transactionModel);
            IsLoading = false;

            if (!responsePayment.IsSuccess)
            {
                await DisplayAlertSafe("Payment Failed", "Transaction was not successful.", "OK");
                await PopAsyncSafe();
                return;
            }

            IsLoading = true;
            var responseUpgrade = await _userService.ConfirmUpgrade(_orderId);
            IsLoading = false;

            if (responseUpgrade.IsSuccess)
            {
                await DisplayAlertSafe("Upgrade Successful", "You are now a premium user!", "OK");

                try
                {
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        Application.Current.MainPage = new AppShell();
                    });
                }
                catch (Exception ex)
                {
                    await DisplayAlertSafe("Navigation Error", $"Failed to load CoursePage: {ex.Message}", "OK");
                }
            }
            else
            {
                await DisplayAlertSafe("Upgrade Failed", "Something went wrong. Please contact support.", "OK");
            }

            await PopAsyncSafe();
        }
        catch (Exception ex)
        {
            IsLoading = false; 
            await DisplayAlertSafe("Error", $"An unexpected error occurred: {ex.Message}", "OK");
            await PopAsyncSafe();
        }
    }

    private async Task DisplayAlertSafe(string title, string message, string cancel)
    {
        if (Application.Current?.MainPage != null)
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
                Application.Current.MainPage.DisplayAlert(title, message, cancel));
        }
    }

    private async Task PopAsyncSafe()
    {
        if (Application.Current?.MainPage?.Navigation != null)
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
                Application.Current.MainPage.Navigation.PopAsync());
        }
    }
}