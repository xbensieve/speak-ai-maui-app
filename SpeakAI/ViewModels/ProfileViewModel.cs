using SpeakAI.Services.Interfaces;
using SpeakAI.Services.Models;
using System.IdentityModel.Tokens.Jwt;
using System;
using System.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using CommunityToolkit.Mvvm.Input;
using SpeakAI.Views;

namespace SpeakAI.ViewModels
{
    public class ProfileViewModel : INotifyPropertyChanged
    {
        private readonly IUserService _userService;
        private bool _isLoading;
        private ProfileModel _profile;
        private OTPModel _otpModel;

        public event PropertyChangedEventHandler PropertyChanged;

        public ProfileModel Profile
        {
            get => _profile;
            set
            {
                _profile = value;
                OnPropertyChanged();
            }
        }

        public OTPModel OTP
        {
            get => _otpModel;
            set
            {
                _otpModel = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
                System.Diagnostics.Debug.WriteLine($"IsLoading set to: {_isLoading}");
            }
        }

        public ICommand LoadProfileCommand { get; }
        public ICommand RefreshProfileCommand { get; }
        public ICommand ConfirmEmailCommand { get; }

        public ProfileViewModel(IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            LoadProfileCommand = new AsyncRelayCommand(LoadProfile);
            RefreshProfileCommand = new AsyncRelayCommand(RefreshProfileAsync);
            ConfirmEmailCommand = new AsyncRelayCommand(ConfirmEmailAsync);

            // Load profile initially
            LoadProfileCommand.Execute(null);
        }

        private async Task RefreshProfileAsync()
        {
            System.Diagnostics.Debug.WriteLine("RefreshProfileAsync started");
            await LoadProfile();
            System.Diagnostics.Debug.WriteLine("RefreshProfileAsync completed");
        }

        private async Task<string> GetUserIdFromTokenAsync()
        {
            try
            {
                string token = await Xamarin.Essentials.SecureStorage.GetAsync("AccessToken");
                if (string.IsNullOrEmpty(token))
                {
                    System.Diagnostics.Debug.WriteLine("No access token found");
                    return "Unknown";
                }

                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "Id")?.Value ?? "Unknown";
                System.Diagnostics.Debug.WriteLine($"User ID from token: {userId}");
                return userId;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Token parsing error: {ex.Message}");
                return "Unknown";
            }
        }

        private async Task LoadProfile()
        {
            System.Diagnostics.Debug.WriteLine("LoadProfile started");
            try
            {
                IsLoading = true;
                string userId = await GetUserIdFromTokenAsync();

                if (string.IsNullOrEmpty(userId) || userId == "Unknown")
                {
                    System.Diagnostics.Debug.WriteLine("Invalid user ID, showing alert");
                    await Application.Current.MainPage.DisplayAlert("Error", "User ID not found. Please log in again.", "OK");
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"Fetching profile for user ID: {userId}");
                var response = await _userService.GetProfile(userId);

                if (response?.IsSuccess == true)
                {
                    Profile = response.Result;
                    System.Diagnostics.Debug.WriteLine("Profile loaded successfully");

                    if (DateTime.TryParse(Profile?.Birthday?.ToString(), out DateTime birthDate))
                    {
                        Profile.Birthday = birthDate;
                    }

                    if (DateTime.TryParse(Profile?.PremiumExpiredTime?.ToString(), out DateTime premiumExpiry))
                    {
                        Profile.PremiumExpiredTime = premiumExpiry;
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to load profile: {response?.Message}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in LoadProfile: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to load profile. Please try again.", "OK");
            }
            finally
            {
                IsLoading = false;
                System.Diagnostics.Debug.WriteLine("LoadProfile completed, IsLoading set to false");
            }
        }

        private async Task ConfirmEmailAsync()
        {
            try
            {
                IsLoading = true;
                string userId = await GetUserIdFromTokenAsync();

                if (string.IsNullOrEmpty(userId) || userId == "Unknown")
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "User ID not found. Please log in again.", "OK");
                    return;
                }

                var response = await _userService.ConfirmEmail(userId);
                if (response?.IsSuccess == true)
                {
                    OTP = response.Result;

                    await Xamarin.Essentials.MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        await Shell.Current.GoToAsync(nameof(ConfirmEmailPage), new Dictionary<string, object>
                        {
                            { "OtpCode", OTP }
                        });
                    });
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Failed to fetch OTP. Try again.", "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in ConfirmEmailAsync: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}