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
    public class StudyViewModel : INotifyPropertyChanged
    {
        private readonly ICourseService _courseService;
        public ObservableCollection<EnrolledCourseModel> EnrolledCourses { get; set; } = new();
        public ICommand LoadCoursesCommand { get; }
        public StudyViewModel(ICourseService courseService)
        {
            _courseService = courseService;
            LoadCoursesCommand = new Command(async () => await LoadCoursesAsync());
            _ = LoadCoursesAsync();
        }
        private async Task LoadCoursesAsync()
        {
            EnrolledCourses.Clear();
            var res = await _courseService.GetEnrolledCourses();
            if (res != null)
            {
                foreach (var course in res.Result)
                {
                    EnrolledCourses.Add(course);
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
