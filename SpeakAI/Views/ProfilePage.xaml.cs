using SpeakAI.Services.Interfaces;

namespace SpeakAI.Views;

public partial class ProfilePage : ContentPage
{
	private readonly IUserService _userService;
	private readonly ILoginService _loginService;
	public ProfilePage(IUserService userService, ILoginService loginService)
	{
		InitializeComponent();
		_userService = userService;
		_loginService = loginService;
	}
	private async void LogoutClicked(object sender, EventArgs e)
	{
        bool answer = await DisplayAlert("Question?", "Are you sure you want to logout?", "No", "Yes");
        if (!answer)
        {
            Xamarin.Essentials.SecureStorage.RemoveAll();
            Application.Current.MainPage = new NavigationPage(new LoginPage(_userService, _loginService));
        } else
		{
			return;
		}
        
	}
}