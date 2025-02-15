using SpeakAI.Services.Interfaces;
using SpeakAI.Services.Models;
using SpeakAI.ViewModels;
using System.Diagnostics;

namespace SpeakAI.Views
{
    public partial class CourseDetailPage : ContentPage
    {
        public CourseDetailPage(ICourseService courseService)
        {
            InitializeComponent();
            BindingContext = new CourseDetailViewModel(courseService);
        }
    }
}
