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
        private bool _isProcessing;
        public bool IsProcessing
        {
            get => _isProcessing;
            set
            {
                _isProcessing = value;
                OnPropertyChanged(nameof(IsProcessing));
            }
        }
        public ICommand LoadCoursesCommand { get; }
        public CourseViewModel(ICourseService courseService)
        {
            _courseService = courseService;
            LoadCoursesCommand = new Command(async () => await LoadCoursesAsync());
            _ = LoadCoursesAsync();
        }
        private async Task LoadCoursesAsync()
        {
            try
            {
                IsProcessing = true;

                var courses = await Task.Run(() => _courseService.GetAllCourses());

                if (courses != null)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        Courses.Clear();
                        foreach (var course in courses)
                        {
                            Courses.Add(course);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading courses: {ex.Message}");
            }
            finally
            {
                IsProcessing = false;
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) =>
       PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
