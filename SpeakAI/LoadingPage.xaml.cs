using SpeakAI.Services.Interfaces;

namespace SpeakAI;

public partial class LoadingPage : ContentPage
{
	private readonly IUserService _userService;
    private readonly ILoginService _loginService;

    public LoadingPage(IUserService userService, ILoginService loginService)
	{
		InitializeComponent();
		_userService = userService;
		_loginService = loginService;
        _ = CheckLoginStatusAsync();
	}
	private async Task CheckLoginStatusAsync()
	{
		await Task.Delay(1000);
		string accessToken = await Xamarin.Essentials.SecureStorage.GetAsync("AccessToken");
		if (accessToken != null)
		{
            Application.Current.MainPage = new AppShell();
        } else
		{
            Application.Current.MainPage = new AppShell();
        }
	}
}