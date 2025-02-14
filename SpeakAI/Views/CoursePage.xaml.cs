using SpeakAI.Services.Interfaces;
using SpeakAI.Services.Service;
using SpeakAI.ViewModels;

namespace SpeakAI.Views;

public partial class CoursePage : ContentPage
{
	public CoursePage(ICourseService courseService)
    {
		InitializeComponent();
        BindingContext = new CourseViewModel(courseService);
    }
}