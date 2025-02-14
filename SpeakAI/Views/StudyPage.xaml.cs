using SpeakAI.Services.Interfaces;
using SpeakAI.ViewModels;

namespace SpeakAI.Views;

public partial class StudyPage : ContentPage
{
	public StudyPage(ICourseService courseService)
	{
		InitializeComponent();
		BindingContext = new StudyViewModel(courseService);
	}
}