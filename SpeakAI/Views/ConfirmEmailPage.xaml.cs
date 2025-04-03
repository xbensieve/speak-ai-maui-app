using SpeakAI.Services.Interfaces;
using SpeakAI.ViewModels;

namespace SpeakAI.Views;

public partial class ConfirmEmailPage : ContentPage
{
    private readonly IUserService _userService;
    public ConfirmEmailPage(IUserService userService)
    {
        _userService = userService;
        InitializeComponent();
        BindingContext = new ConfirmEmailViewModel(userService);
    }
}
