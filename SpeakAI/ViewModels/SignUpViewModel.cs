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

namespace SpeakAI.ViewModels
{
    public partial class SignUpViewModel : INotifyPropertyChanged
    {
        private readonly IUserService _userService;
        private readonly ILoginService _loginService;
        private string _username;
        private string _email;
        private string _fullName;
        private DateTime _birthday = DateTime.Today;
        private string _gender;
        private string _password;
        private string _confirmPassword;
        private string _notificationMessage;
        private bool _isProcessing;
        public event PropertyChangedEventHandler PropertyChanged;
        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(nameof(Username)); }
        }

        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(nameof(Email)); }
        }

        public string FullName
        {
            get => _fullName;
            set { _fullName = value; OnPropertyChanged(nameof(FullName)); }
        }

        public DateTime Birthday
        {
            get => _birthday;
            set { _birthday = value; OnPropertyChanged(nameof(Birthday)); }
        }

        public string Gender
        {
            get => _gender;
            set { _gender = value; OnPropertyChanged(nameof(Gender)); }
        }

        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(nameof(Password)); }
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set { _confirmPassword = value; OnPropertyChanged(nameof(ConfirmPassword)); }
        }
        public string NotificationMessage
        {
            get => _notificationMessage;
            set { _notificationMessage = value; OnPropertyChanged(nameof(NotificationMessage)); ShowToast(_notificationMessage); }
        }
        public bool IsProcessing
        {
            get => _isProcessing;
            set { _isProcessing = value; OnPropertyChanged(nameof(IsProcessing)); }
        }
        public ICommand SignUpCommand { get; }
        public ICommand SignInCommand { get; }
        public SignUpViewModel(IUserService userService, ILoginService loginService)
        {
            _userService = userService;
            _loginService = loginService;
            SignUpCommand = new Command(async () => await OnSignUp(), () => !IsProcessing);
            SignInCommand = new Command(async () => await OnSignIn());
        }
        private async void ShowToast(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                await Toast.Make(message, CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
            }
        }
        private async Task OnSignIn()
        {
            Application.Current.MainPage = new NavigationPage(new LoginPage(_userService, _loginService));
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
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Application.Current.MainPage = new NavigationPage(new SpeakAI.Views.LoadingPage());
                });

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
               
                var response = await Task.Run(async () => await _userService.SignUpCustomer(newUser)).ConfigureAwait(false);

                if (response?.IsSuccess == true)
                {
                    await HandleSuccessfulSignIn(response.Message);
                }
                else
                {
                    ShowNotification(response?.Message ?? "Error Network!");
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        Application.Current.MainPage = new SpeakAI.SignUpPage(_userService, _loginService);
                    });
                }
            }
            catch (Exception ex)
            {
                ShowNotification($"Error: {ex.Message}");
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Application.Current.MainPage = new SpeakAI.LoginPage(_userService, _loginService);
                });
            }
            finally
            {
                IsProcessing = false;
                ((Command)SignUpCommand).ChangeCanExecute();
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(Username) ||
                 string.IsNullOrWhiteSpace(Email) ||
                 string.IsNullOrWhiteSpace(FullName) ||
                 string.IsNullOrWhiteSpace(Gender) ||
                 string.IsNullOrWhiteSpace(Password) ||
                 string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                ShowNotification("All fields are required.");
                return false;
            }
            return true;
        }
        private async Task HandleSuccessfulSignIn(string message)
        {
            ShowNotification(message);
            ResetFormFields();
            await Application.Current.MainPage.Navigation.PushModalAsync(new SuccessPopup());

            await Task.Delay(2000);

            await Application.Current.MainPage.Navigation.PopModalAsync();

            await Application.Current.MainPage.Navigation.PushAsync(new LoginPage(_userService, _loginService));
        }
        private void ShowNotification(string message)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                NotificationMessage = message;
            });
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
