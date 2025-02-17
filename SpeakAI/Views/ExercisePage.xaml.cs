using SpeakAI.Services.Interfaces;
using SpeakAI.Services.Models;
using SpeakAI.ViewModels;

namespace SpeakAI.Views;

public partial class ExercisePage : ContentPage
{
	public ExercisePage(ICourseService courseService)
	{
		InitializeComponent();
		BindingContext = new ExerciseViewModel(courseService);
	}
    private async void OnExerciseTapped(object sender, EventArgs e)
    {
        if (Shell.Current == null)
        {
            await DisplayAlert("Error", "Shell is not initialized", "OK");
        }
        else
        {
            if (sender is Frame frame && frame.BindingContext is Topic selectedTopic)
            {
                var navigationParameter = new Dictionary<string, object>
                    {
                        { "topic", selectedTopic }
                    };
                await Shell.Current.GoToAsync("exercisedetail", navigationParameter);
            }
        }

    }
}