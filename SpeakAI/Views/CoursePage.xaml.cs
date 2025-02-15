using SpeakAI.Services.Interfaces;
using SpeakAI.Services.Models;
using SpeakAI.Services.Service;
using SpeakAI.ViewModels;

namespace SpeakAI.Views;

public partial class CoursePage : ContentPage
{
	public CoursePage(ICourseService courseService)
    {
		InitializeComponent();
        BindingContext = new CourseViewModel(courseService);
        AnimatePage();
    }

    private async void OnCourseTapped(object sender, EventArgs e)
    {
        if (Shell.Current == null)
        {
            await DisplayAlert("Error", "Shell is not initialized", "OK");
        }
        else
        {
            if (sender is Frame frame && frame.BindingContext is CourseModel selectedCourse)
            {
                var navigationParameter = new Dictionary<string, object>
                    {
                        { "course", selectedCourse }
                    };
                await Shell.Current.GoToAsync("coursedetail", navigationParameter);
            }
        }

    }

    private async void AnimatePage()
    {
        await TitleLabel.FadeTo(1, 1000, Easing.CubicInOut);
    }
}