using SpeakAI.Services.Interfaces;
using SpeakAI.ViewModels;

namespace SpeakAI;

public partial class SignUpPage : ContentPage
{
	public SignUpPage(IUserService userService)
	{
		InitializeComponent();
        BindingContext = new SignUpViewModel(userService);
    }
}