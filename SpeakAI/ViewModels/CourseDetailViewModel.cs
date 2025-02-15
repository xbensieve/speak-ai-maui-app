using SpeakAI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeakAI.ViewModels
{
    public class CourseDetailViewModel : INotifyPropertyChanged
    {
        private readonly ICourseService _courseService;
        public CourseDetailViewModel(ICourseService courseService)
        {
            _courseService = courseService;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
