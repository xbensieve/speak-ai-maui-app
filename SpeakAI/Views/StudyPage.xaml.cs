using SpeakAI.Services.Interfaces;
using SpeakAI.Services.Models;
using SpeakAI.ViewModels;

namespace SpeakAI.Views;

public partial class StudyPage : ContentPage
{
	public StudyPage(ICourseService courseService)
	{
		InitializeComponent();
		BindingContext = new StudyViewModel(courseService);
	}
    private async void OnCourseTapped(object sender, EventArgs e)
    {
        if (Shell.Current == null)
        {
            await DisplayAlert("Error", "Shell is not initialized", "OK");
        }
        else
        {
                await Shell.Current.GoToAsync("exercise");
        }
    }
}