using SpeakAI.Services.Interfaces;
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
            _userService = userService;
            _loginService = loginService;
            LoadUserProfile();
        }

        private async void LoadUserProfile()
        {
            try
            {
                string token = await Xamarin.Essentials.SecureStorage.GetAsync("AccessToken");
                if (!string.IsNullOrEmpty(token))
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadJwtToken(token);
                    var name = jwtToken.Claims.FirstOrDefault(c => c.Type == "name")?.Value ?? "Unknown";
                    var email = jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value ?? "No email";
                    var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "Id")?.Value ?? "Unknown";
                    NameLabel.Text = name;
                    EmailLabel.Text = email;
                    UserIdLabel.Text = $"User ID: {userId}";
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load profile: {ex.Message}", "OK");
            }
        }
        private async void LogoutClicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Logout", "Are you sure you want to logout?", "No", "Yes");
            if (!answer)
            {
                Xamarin.Essentials.SecureStorage.RemoveAll();
                Application.Current.MainPage = new NavigationPage(new LoginPage(_userService, _loginService));
            }
        }
    }
}
