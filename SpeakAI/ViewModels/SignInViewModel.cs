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
            set { _notificationMessage = value; OnPropertyChanged(nameof(NotificationMessage)); }
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
            SignUpCommand = new Command(async () => await OnSignUp(userService));
        }
        private async Task OnSignUp(IUserService userService)
        {
            await Application.Current.MainPage.Navigation.PushAsync(new SignUpPage(userService));
        }
        private async Task OnSignIn()
        {
            if (IsProcessing) return;
            IsProcessing = true;
            ((Command)SignInCommand).ChangeCanExecute();

            try
            {
                if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
                {
                    NotificationMessage = "All fields are required.";
                    return;
                }
                var user = new LoginRequestModel
                {
                    userName = Username,
                    password = Password,
                };

                var response = await _loginService.Login(user);
                if (response == null)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Login service returned null response.", "OK");
                    return;
                }
                if (response.IsSuccess)
                {
                    await Application.Current.MainPage.DisplayAlert("Success", "Signed in successfully!", "OK");

                    ResetFormFields();

                    await Task.Delay(1500);

                    Application.Current.MainPage = new AppShell();
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", $"Signup failed: {response.Message}", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
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
    }
}
