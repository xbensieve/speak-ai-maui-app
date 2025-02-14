using SpeakAI.Services.Interfaces;
using SpeakAI.ViewModels;

namespace SpeakAI;

public partial class LoginPage : ContentPage
{
	public LoginPage(IUserService userService, ILoginService loginService)
	{
		InitializeComponent();
        BindingContext = new SignInViewModel(loginService, userService);
    }
}