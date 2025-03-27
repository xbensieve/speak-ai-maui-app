using CommunityToolkit.Maui.Alerts;
using SpeakAI.Services.Interfaces;
using SpeakAI.Services.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
namespace SpeakAI.ViewModels
{
    public partial class SignInViewModel : INotifyPropertyChanged
    {
        private readonly ILoginService _loginService;
        private readonly IUserService _userService;
        private string _username;
        private string _password;
        private bool _isProcessing;
        private string _notificationMessage;
        public string Username
        {
            get => _username;
            set {  _username = value; OnPropertyChanged(nameof(Username)); }
        }
        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(nameof(Password));}
        }
        public bool IsProcessing
        {
            get => _isProcessing;
            set { _isProcessing = value; OnPropertyChanged(nameof(IsProcessing)); }
        }
        public string NotificationMessage
        {
            get => _notificationMessage;
            set { _notificationMessage = value; OnPropertyChanged(nameof(NotificationMessage)); ShowToast(_notificationMessage); }
        }
        private async void ShowToast(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                await Toast.Make(message, CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
            }
        }
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        public ICommand SignInCommand { get; }
        public ICommand SignUpCommand { get; }
        public SignInViewModel(ILoginService loginService, IUserService userService)
        {
            _loginService = loginService ?? throw new ArgumentNullException(nameof(loginService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));

            SignInCommand = new Command(async () => await OnSignIn(), () => !IsProcessing);
            SignUpCommand = new Command(async () => await OnSignUp(userService, loginService));
        }
        private async Task OnSignUp(IUserService userService, ILoginService loginService)
        {
            Application.Current.MainPage = new NavigationPage(new SignUpPage(userService, loginService));
        }
        private async Task OnSignIn()
        {
            if (IsProcessing) return;
            IsProcessing = true;
            ((Command)SignInCommand).ChangeCanExecute();

            try
            {
                if (!ValidateInputs()) return;

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Application.Current.MainPage = new NavigationPage(new SpeakAI.Views.LoadingPage());
                });

                var response = await _loginService.Login(new LoginRequestModel
                {
                    userName = Username,
                    password = Password
                });

                if (response?.IsSuccess == true)
                {
                    await HandleSuccessfulLogin(response.Message);
                }
                else
                {
                    ShowNotification(response?.Message ?? "Error Network!");
                    Password = string.Empty;
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        Application.Current.MainPage = new SpeakAI.LoginPage(_userService, _loginService);
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
                ((Command)SignInCommand).ChangeCanExecute();
            }
        }
        private void ResetFormFields()
        {
            Username = string.Empty;
            Password = string.Empty;
        }
        private async Task HandleSuccessfulLogin(string message)
        {
            ShowNotification(message);
            ResetFormFields();
            await Task.Delay(200);

            MainThread.BeginInvokeOnMainThread(() =>
            {
                Application.Current.MainPage = new AppShell();
            });
        }
        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ShowNotification("All fields are required.");
                return false;
            }
            return true;
        }
        private void ShowNotification(string message)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                NotificationMessage = message;
            });
        }
    }
}
