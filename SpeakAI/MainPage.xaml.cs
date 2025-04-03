using Plugin.Firebase.CloudMessaging;
using System.IdentityModel.Tokens.Jwt;
using System.Xml;

namespace SpeakAI
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
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
                    greetingLabel.Text = $"Hi, {name}👋";
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load profile: {ex.Message}", "OK");
            }
        }
        private async void OnCounterClicked(object sender, EventArgs e)
        {
            await CrossFirebaseCloudMessaging.Current.CheckIfValidAsync();
            var token = await CrossFirebaseCloudMessaging.Current.GetTokenAsync();
            Console.WriteLine($"FCM token: {token}");
        }
    }
}
