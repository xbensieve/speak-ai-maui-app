using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpeakAI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;

namespace SpeakAI.ViewModels
{
    public partial class ConfirmEmailViewModel : ObservableObject, IQueryAttributable
    {
        private readonly IUserService _userService;

        [ObservableProperty]
        private string otpCode;

        [ObservableProperty]
        private string enteredOtp;

        public ICommand VerifyOtpCommand { get; }

        public ConfirmEmailViewModel(IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService)); // ✅ Ensure Dependency is Injected
            VerifyOtpCommand = new AsyncRelayCommand(VerifyOtpAsync);
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query != null && query.ContainsKey("OtpCode"))
            {
                OtpCode = query["OtpCode"] as string;
            }
        }

        private async Task<string> GetUserIdFromTokenAsync()
        {
            try
            {
                string token = await Xamarin.Essentials.SecureStorage.GetAsync("AccessToken");
                if (string.IsNullOrEmpty(token)) return "Unknown";

                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                return jwtToken.Claims.FirstOrDefault(c => c.Type == "Id")?.Value ?? "Unknown";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Token parsing error: {ex.Message}");
                return "Unknown";
            }
        }

        private async Task VerifyOtpAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(EnteredOtp))
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Please enter the OTP.", "OK");
                    return;
                }

                string userId = await GetUserIdFromTokenAsync();
                if (userId == "Unknown")
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Failed to retrieve user ID.", "OK");
                    return;
                }

                var response = await _userService.VerifyOTP(userId, EnteredOtp);
                if (response == null) throw new Exception("Null response from server.");

                if (response.IsSuccess)
                {
                    await Application.Current.MainPage.DisplayAlert("Success", "Email verified successfully!", "OK");
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Invalid OTP. Please try again.", "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Verification error: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error", "An unexpected error occurred.", "OK");
            }
        }
    }
}
