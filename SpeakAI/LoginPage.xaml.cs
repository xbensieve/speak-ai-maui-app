using SpeakAI.Services.Interfaces;
using SpeakAI.ViewModels;

namespace SpeakAI;

public partial class LoginPage : ContentPage
{
	public LoginPage(IUserService userService, ILoginService loginService)
	{
		InitializeComponent();
        var loginButton = this.FindByName<Button>("LoginButton");
        loginButton.Pressed += async (s, e) => await loginButton.ScaleTo(0.98, 100);
        loginButton.Released += async (s, e) => await loginButton.ScaleTo(1, 100);
        BindingContext = new SignInViewModel(loginService, userService);
    }
}