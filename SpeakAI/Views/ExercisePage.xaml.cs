using SpeakAI.Services.Interfaces;
using SpeakAI.ViewModels;

namespace SpeakAI.Views;

public partial class ExercisePage : ContentPage
{
	public ExercisePage(ICourseService courseService)
	{
		InitializeComponent();
		BindingContext = new ExerciseViewModel(courseService);
	}
}