using SpeakAI.Services.Interfaces;
using SpeakAI.Services.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SpeakAI.ViewModels
{
    public class StudyViewModel : INotifyPropertyChanged
    {
        private readonly ICourseService _courseService;
        private bool _isLoading;
        public ObservableCollection<EnrolledCourseModel> EnrolledCourses { get; set; } = new();
        public ICommand LoadCoursesCommand { get; }
        public ICommand RefreshCommand { get; }
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (_isLoading == value) return;
                _isLoading = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsNotLoading));
            }
        }

        public bool IsNotLoading => !IsLoading;
        public StudyViewModel(ICourseService courseService)
        {
            _courseService = courseService;
            LoadCoursesCommand = new Command(async () => await LoadCoursesAsync());
            _ = LoadCoursesAsync();
        }
        private async Task LoadCoursesAsync()
        {
            if (IsLoading) return;

            IsLoading = true;

            try
            {
                EnrolledCourses.Clear();
                var res = await _courseService.GetEnrolledCourses();
                if (res?.Result != null)
                {
                    foreach (var course in res.Result)
                    {
                        EnrolledCourses.Add(course);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading courses: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
