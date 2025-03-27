using SpeakAI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SpeakAI.Services.Models;
using Android.SE.Omapi;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Text.RegularExpressions;

namespace SpeakAI.ViewModels
{
    public partial class SignUpViewModel : INotifyPropertyChanged
    {
        private readonly IUserService _userService;
        private readonly ILoginService _loginService;
        private string _username = string.Empty;
        private string _email = string.Empty;
        private string _fullName = string.Empty;
        private DateTime _birthday = DateTime.Today;
        private string _gender = "Prefer not to say";
        private string _password = string.Empty;
        private string _confirmPassword = string.Empty;
        private string _notificationMessage = string.Empty;
        private bool _isProcessing;
        private List<string> _genderOptions;
        public event PropertyChangedEventHandler? PropertyChanged;
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        public string FullName
        {
            get => _fullName;
            set
            {
                _fullName = value;
                OnPropertyChanged(nameof(FullName));
            }
        }

        public DateTime Birthday
        {
            get => _birthday;
            set
            {
                _birthday = value;
                OnPropertyChanged(nameof(Birthday));
            }
        }

        public string Gender
        {
            get => _gender;
            set
            {
                _gender = value;
                OnPropertyChanged(nameof(Gender));
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                _confirmPassword = value;
                OnPropertyChanged(nameof(ConfirmPassword));
            }
        }
        public string NotificationMessage
        {
            get => _notificationMessage;
            set
            {
                _notificationMessage = value;
                OnPropertyChanged(nameof(NotificationMessage));
            }
        }
        public bool IsProcessing
        {
            get => _isProcessing;
            set
            {
                _isProcessing = value;
                OnPropertyChanged(nameof(IsProcessing));
            }
        }
        public List<string> GenderOptions
        {
            get => _genderOptions;
            set
            {
                _genderOptions = value;
                OnPropertyChanged(nameof(GenderOptions));
            }
        }
        public ICommand SignUpCommand
        {
            get;
        }
        public ICommand SignInCommand
        {
            get;
        }
        public SignUpViewModel(IUserService userService, ILoginService loginService)
        {
            _userService = userService;
            _loginService = loginService;
            SignUpCommand = new Command(async () => await OnSignUp(), () => !IsProcessing);
            SignInCommand = new Command(async () => await OnSignIn());
            GenderOptions = new List<string> {
            "Male",
            "Female",
            "Non-binary",
            "Prefer not to say"
         };
        }
        private async Task OnSignIn()
        {
            if (Application.Current?.MainPage?.Navigation == null)
            {
                ShowNotification("Navigation is not available.");
                return;
            }
            await Application.Current.MainPage.Navigation.PushAsync(new LoginPage(_userService, _loginService));
        }
        private async Task OnSignUp()
        {
            if (IsProcessing) return;
            IsProcessing = true;
            ((Command)SignUpCommand).ChangeCanExecute();

            try
            {
                if (!ValidateInputs()) return;

                if (Password != ConfirmPassword)
                {
                    NotificationMessage = "Passwords do not match.";
                    return;
                }

                var newUser = new UserModel
                {
                    userName = Username.Trim(),
                    password = Password,
                    confirmedPassword = ConfirmPassword,
                    email = Email.Trim(),
                    fullName = FullName.Trim(),
                    birthday = Birthday.ToString("yyyy-MM-dd"),
                    gender = Gender
                };

                var response = await _userService.SignUpCustomer(newUser);

                if (response == null)
                {
                    Console.WriteLine("API response is null.");
                    ShowNotification("Network error occurred! Please try again.");
                    return;
                }

                if (response?.IsSuccess == true)
                {
                    await HandleSuccessfulSignIn(response.Message);
                }
                else
                {
                    ShowNotification(response?.Message ?? "Network error occurred!");
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                    await Application.Current.MainPage.DisplayAlert("Error", ex.InnerException.Message, "OK");
                }
                else
                {
                    Console.WriteLine($"SignUp Error: {ex.Message}");
                    await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
                }
            }
            finally
            {
                IsProcessing = false;
                ((Command)SignUpCommand).ChangeCanExecute();
            }
        }
        private async Task ShowToastAsync(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await Toast.Make(message, CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
                });
            }
        }
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(Username) || Username.Length < 3)
            {
                ShowNotification("Username must be at least 3 characters.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(Email) || !Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                ShowNotification("Invalid email format.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(FullName) || FullName.Trim().Split(' ').Length < 2)
            {
                ShowNotification("Please enter your full name (first and last).");
                return false;
            }

            //if (Birthday > DateTime.Today.AddYears(-13))
            //{
            //    ShowNotification("You must be at least 13 years old to sign up.");
            //    return false;
            //}

            if (!GenderOptions.Contains(Gender))
            {
                ShowNotification("Please select a valid gender.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(Password) ||
                Password.Length < 8 ||
                !Regex.IsMatch(Password, @"[A-Z]") ||
                !Regex.IsMatch(Password, @"[a-z]") ||
                !Regex.IsMatch(Password, @"\d") ||
                !Regex.IsMatch(Password, @"[\W_]"))
            {
                ShowNotification("Password must be at least 8 characters, including an uppercase letter, a lowercase letter, a number, and a special character.");
                return false;
            }

            if (Password != ConfirmPassword)
            {
                ShowNotification("Passwords do not match.");
                return false;
            }

            return true;
        }
        private async Task HandleSuccessfulSignIn(string message)
        {
            await ShowToastAsync(message);
            ResetFormFields();
            await Task.Delay(2000);
            if (Application.Current?.MainPage?.Navigation != null)
            {
                await Application.Current.MainPage.Navigation.PushAsync(new LoginPage(_userService, _loginService));
            }
            else
            {
                ShowNotification("Navigation is not available.");
            }
        }
        private void ShowNotification(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;
            NotificationMessage = message;
        }
        private void ResetFormFields()
        {
            Username = string.Empty;
            Email = string.Empty;
            FullName = string.Empty;
            Birthday = DateTime.Today;
            Gender = string.Empty;
            Password = string.Empty;
            ConfirmPassword = string.Empty;
        }
    }
}