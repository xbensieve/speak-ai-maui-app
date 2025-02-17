using SpeakAI.Services.Interfaces;
using SpeakAI.Services.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


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
                    LoadCourseProgress();
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
                    LoadCourseDetails();
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
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        public ExerciseViewModel(ICourseService courseService)
        {
            _courseService = courseService;
        }
        private async void LoadCourseDetails()
        {
            if (string.IsNullOrEmpty(CourseId)) return;

            var response = await _courseService.GetCourseDetails(CourseId);
            if (response != null && response.IsSuccess)
            {
                Course = response.Result;
                Topics = Course.Topics ?? new List<Topic>();
            }
        }
        private async void LoadCourseProgress()
        {
            if (string.IsNullOrEmpty(EnrolledCourseId)) return;

            var response = await _courseService.GetCourseProgress(EnrolledCourseId);
            if (response != null && response.IsSuccess)
            {
                EnrolledCourse = response.Result;
                TopicProgresses = EnrolledCourse.TopicProgresses ?? new List<TopicProgress>();

                // Merge TopicProgress with Topics
                foreach (var topic in Topics)
                {
                    var topicProgress = TopicProgresses.FirstOrDefault(tp => tp.TopicId == topic.TopicId);
                    if (topicProgress != null)
                    {
                        topic.ProgressPoints = topicProgress.ProgressPoints;
                    }
                }
            }
        }
    }
}
