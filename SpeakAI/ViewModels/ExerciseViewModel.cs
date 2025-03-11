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

            var response = await _courseService.GetCourseDetails(CourseId);
            if (response != null && response.IsSuccess)
            {
                Course = response.Result;
                Topics = Course.Topics ?? new List<Topic>();
            }
        }
        private async Task LoadCourseProgressAsync()
        {
            if (string.IsNullOrEmpty(EnrolledCourseId)) return;

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
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
