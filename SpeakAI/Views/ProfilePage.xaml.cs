using AndroidX.Lifecycle;
using SpeakAI.Services.Interfaces;
using SpeakAI.ViewModels;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text.Json;
using System.Xml;
using Xamarin.Essentials;

namespace SpeakAI.Views
{
    public partial class ProfilePage : ContentPage
    {
        private readonly IUserService _userService;
        private readonly ILoginService _loginService;

        public ProfilePage(IUserService userService, ILoginService loginService)
        {
            InitializeComponent();
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _loginService = loginService ?? throw new ArgumentNullException(nameof(loginService));
            BindingContext = new ProfileViewModel(_userService);
            System.Diagnostics.Debug.WriteLine("ProfilePage initialized");
        }

        private async void LogoutClicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Logout", "Are you sure you want to logout?", "No", "Yes");
            if (!answer)
            {
                System.Diagnostics.Debug.WriteLine("User confirmed logout");
                Xamarin.Essentials.SecureStorage.RemoveAll();
                Application.Current.MainPage = new NavigationPage(new LoginPage(_userService, _loginService));
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Logout cancelled");
            }
        }

        private async void UpgradeClicked(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Navigating to payment page");
            await Shell.Current.GoToAsync("payment");
        }
    }
}