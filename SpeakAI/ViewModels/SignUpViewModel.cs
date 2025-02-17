using SpeakAI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SpeakAI.Services.Models;

namespace SpeakAI.ViewModels
{
    public partial class SignUpViewModel : INotifyPropertyChanged
    {
        private readonly IUserService _userService;
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
            set { _notificationMessage = value; OnPropertyChanged(nameof(NotificationMessage)); }
        }
        public bool IsProcessing
        {
            get => _isProcessing;
            set { _isProcessing = value; OnPropertyChanged(nameof(IsProcessing)); }
        }
        public ICommand SignUpCommand { get; }
        public ICommand SignInCommand { get; }
        public SignUpViewModel(IUserService userService)
        {
            _userService = userService;
            SignUpCommand = new Command(async () => await OnSignUp(), () => !IsProcessing);
            SignInCommand = new Command(async () => await OnSignIn());
        }
        private async Task OnSignIn()
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }
        private async Task OnSignUp()
        {
            if (IsProcessing) return;
            IsProcessing = true;
            ((Command)SignUpCommand).ChangeCanExecute();

            try
            {
                if (string.IsNullOrWhiteSpace(Username) ||
                    string.IsNullOrWhiteSpace(Email) ||
                    string.IsNullOrWhiteSpace(FullName) ||
                    string.IsNullOrWhiteSpace(Gender) ||
                    string.IsNullOrWhiteSpace(Password) ||
                    string.IsNullOrWhiteSpace(ConfirmPassword))
                {
                    NotificationMessage = "All fields are required.";
                    return;
                }

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

                if (response.IsSuccess)
                {
                    await Application.Current.MainPage.DisplayAlert("Success", "Signed up successfully!", "OK");

                    ResetFormFields();

                    await Task.Delay(1500);

                    await Application.Current.MainPage.Navigation.PopAsync();
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
                ((Command)SignUpCommand).ChangeCanExecute();
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
