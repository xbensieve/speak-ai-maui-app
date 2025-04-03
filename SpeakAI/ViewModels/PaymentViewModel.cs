using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpeakAI.Services.Interfaces;
using SpeakAI.Services.Models;
using SpeakAI.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SpeakAI.ViewModels
{
    public partial class PaymentViewModel : ObservableObject
    {
        private readonly IUserService _userService;
        private readonly ICourseService _courseService;
        [ObservableProperty]
        private bool isPremium;

        [ObservableProperty]
        private bool isVerified;

        [ObservableProperty]
        private string voucherCode;

        public ICommand PayNowCommand { get; }

        public PaymentViewModel(IUserService userService, ICourseService courseService)
        {
            _userService = userService;
            LoadUserStatus();
            PayNowCommand = new AsyncRelayCommand(PayNowAsync);
            _courseService = courseService;
        }
        private async Task<string> GetUserIdFromTokenAsync()
        {
            try
            {
                string token = await Xamarin.Essentials.SecureStorage.GetAsync("AccessToken");

                if (!string.IsNullOrEmpty(token))
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadJwtToken(token);
                    return jwtToken.Claims.FirstOrDefault(c => c.Type == "Id")?.Value ?? "Unknown";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Token parsing error: {ex.Message}");
            }
            return "Unknown";
        }
        private async void LoadUserStatus()
        {
            string userId = await GetUserIdFromTokenAsync();
            var user = await _userService.GetProfile(userId);
            if (user != null)
            {
                IsPremium = user.Result.IsPremium;
                IsVerified = user.Result.IsVerified;
            }
        }

        private async Task PayNowAsync()
        {
            if (!IsVerified)
            {
                await Application.Current.MainPage.DisplayAlert("Email Not Verified", "Please verify your email before upgrading to premium.", "OK");
                LoadUserStatus();
                return;
            }
            if (IsPremium)
            {
                await Application.Current.MainPage.DisplayAlert("Already Premium", "You are already a premium user.", "OK");
                LoadUserStatus();
                return;
            }
            string userId = await GetUserIdFromTokenAsync();
            try
            {
                var orderResponse = await _userService.CreateOrder(userId);
                if (orderResponse?.Result == null)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Failed to create order.", "OK");
                    return;
                }
                string orderId = orderResponse.Result;
                var order = new OrderModel { OrderId = orderId, VoucherCode = VoucherCode };
                var paymentResponse = await _userService.RequestPayment(order);
                if (paymentResponse?.Result != null)
                {
                    string paymentUrl = paymentResponse.Result;
                    await Application.Current.MainPage.Navigation.PushAsync(new PaymentWebViewPage(paymentUrl, orderId, _userService, userId, _courseService));
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Payment Failed", "Failed to process payment.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }
    }
}
