using SpeakAI.Services.Interfaces;
using SpeakAI.ViewModels;

namespace SpeakAI.Views;

public partial class PaymentPage : ContentPage
{
    public PaymentPage(IUserService userService, ICourseService courseService)
    {
        InitializeComponent();
        BindingContext = new PaymentViewModel(userService, courseService);
    }
}