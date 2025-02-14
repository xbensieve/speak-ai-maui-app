using SpeakAI.Services.Interfaces;
using SpeakAI.Services.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SpeakAI.ViewModels
{
    public class CourseViewModel : INotifyPropertyChanged
    {
        private readonly ICourseService _courseService;
        public ObservableCollection<CourseModel> Courses { get; set; } = new();
        public ICommand LoadCoursesCommand { get; }
        public CourseViewModel(ICourseService courseService)
        {
            _courseService = courseService;
            LoadCoursesCommand = new Command(async () => await LoadCoursesAsync());
            _ = LoadCoursesAsync();
        }
        private async Task LoadCoursesAsync()
        {
            Courses.Clear();
            var res = await _courseService.GetAllCourses();
            if (res != null)
            {
                foreach (var course in res)
                {
                    Courses.Add(course);
                }
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
