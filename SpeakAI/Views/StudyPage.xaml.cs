using SpeakAI.Services.Interfaces;
using SpeakAI.Services.Models;
using SpeakAI.ViewModels;
using Microsoft.Maui.Graphics;
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
            if (sender is Frame frame && frame.BindingContext is EnrolledCourseModel selectedCourse)
            {
                var navigationParameter = new Dictionary<string, object>
                    {
                        { "courseId", selectedCourse.CourseId },
                        {"enrolledCourseId", selectedCourse.EnrolledCourseId }
                    };
                await Shell.Current.GoToAsync("exercise", navigationParameter);
            }
        }
    }
    private void OnOngoingClicked(object sender, EventArgs e)
    {
        OngoingLine.BackgroundColor = Colors.Blue;
        CompletedLine.BackgroundColor = Colors.LightGray;

        // Change text color
        ((Button)sender).TextColor = Colors.Blue;
        CompletedButton.TextColor = Colors.Gray;
    }

    private void OnCompletedClicked(object sender, EventArgs e)
    {
        OngoingLine.BackgroundColor = Colors.LightGray;
        CompletedLine.BackgroundColor = Colors.Blue;

        // Change text color
        OngoingButton.TextColor = Colors.Gray;
        ((Button)sender).TextColor = Colors.Blue;
    }

}