using SpeakAI.Services.Interfaces;
using SpeakAI.Services.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;


namespace SpeakAI.ViewModels
{
    [QueryProperty(nameof(CourseId), "courseId")]
    [QueryProperty(nameof(EnrolledCourseId), "enrolledCourseId")]
    public class ExerciseViewModel : INotifyPropertyChanged
    {
        private readonly ICourseService _courseService;
        private string _courseId;
        private string _enrolledCourseId;
        private bool _isLoading;
        public string EnrolledCourseId
        {
            get => _enrolledCourseId;
            set
            {
               if ( _enrolledCourseId != value)
                {
                    _enrolledCourseId = value;
                    OnPropertyChanged();
                    LoadCourseProgressAsync();
                }
            }
        }
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
        private EnrolledCourseProgressModel _progressModel;
        public EnrolledCourseProgressModel EnrolledCourse
        {
            get => _progressModel;
            set
            {
                _progressModel = value;
                OnPropertyChanged();
            }
        }
        private List<TopicProgress> _topicProgresses;
        public List<TopicProgress> TopicProgresses
        {
            get => _topicProgresses;
            set
            {
                if ( _topicProgresses != value)
                {
                    _topicProgresses = value;
                    OnPropertyChanged();
                }
            }
        }
        public string CourseId
        {
            get => _courseId;
            set
            {
                if (_courseId != value)
                {
                    _courseId = value;
                    OnPropertyChanged();
                    LoadCourseDetailsAsync();
                }
            }
        }
        private CourseDetailModel _course;
        public CourseDetailModel Course
        {
            get => _course;
            set
            {
                _course = value;
                OnPropertyChanged();
            }
        }
        private List<Topic> _topics = new();
        public List<Topic> Topics
        {
            get => _topics;
            set
            {
                _topics = value;
                OnPropertyChanged();
            }
        }
        private bool _isRefreshing;
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set
            {
                _isRefreshing = value;
                OnPropertyChanged();
            }
        }

        public ICommand RefreshCommand { get; }
        private async Task RefreshData()
        {
            IsRefreshing = true;
            await LoadCourseDetailsAsync();
            await LoadCourseProgressAsync();
            IsRefreshing = false;
        }

        public ExerciseViewModel(ICourseService courseService)
        {
            _courseService = courseService;
            RefreshCommand = new Command(async () => await RefreshData());
        }
        private async Task LoadCourseDetailsAsync()
        {

            if (string.IsNullOrEmpty(CourseId)) return;

            IsLoading = true; // Start loading
            try
            {
                var response = await _courseService.GetCourseDetails(CourseId);
                if (response != null && response.IsSuccess)
                {
                    Course = response.Result;
                    Topics = Course.Topics ?? new List<Topic>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading course details: {ex.Message}");
            }
            finally
            {
                IsLoading = false; // Stop loading
            }
        }
        private async Task LoadCourseProgressAsync()
        {
            if (string.IsNullOrEmpty(EnrolledCourseId)) return;

            IsLoading = true; // Start loading
            try
            {
                var response = await _courseService.GetCourseProgress(EnrolledCourseId);
                if (response != null && response.IsSuccess)
                {
                    EnrolledCourse = response.Result;
                    TopicProgresses = EnrolledCourse.TopicProgresses ?? new List<TopicProgress>();

                    foreach (var topic in Topics)
                    {
                        var topicProgress = TopicProgresses.SingleOrDefault(tp => tp.TopicId == topic.TopicId);
                        if (topicProgress != null)
                        {
                            Console.WriteLine($"Topic: {topic.TopicName}, Progress: {topicProgress.ProgressPoints}");
                            topic.ProgressPoints = topicProgress.ProgressPoints;
                        }
                        else
                        {
                            Console.WriteLine($"No progress found for Topic: {topic.TopicName}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading course progress: {ex.Message}");
            }
            finally
            {
                IsLoading = false; // Stop loading
            }
        }
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
