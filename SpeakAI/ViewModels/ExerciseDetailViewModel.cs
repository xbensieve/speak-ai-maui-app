using SpeakAI.Services.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace SpeakAI.ViewModels
{
    [QueryProperty(nameof(Topic), "topic")]
    public class ExerciseDetailViewModel : INotifyPropertyChanged
    {
        private Topic _topic;
        private int _currentIndex;
        private string _selectedAnswer;

        public Topic Topic
        {
            get { return _topic; }
            set
            {
                if (_topic != value)
                {
                    _topic = value;
                    _currentIndex = 0;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Exercises));
                    OnPropertyChanged(nameof(CurrentExercise));
                    OnPropertyChanged(nameof(CanGoBack));
                    OnPropertyChanged(nameof(CanGoNext));
                }
            }
        }

        public List<Exercise> Exercises => Topic?.Exercises ?? new List<Exercise>();

        public Exercise CurrentExercise => Exercises.Count > 0 ? Exercises[_currentIndex] : null;

        public string SelectedAnswer
        {
            get => _selectedAnswer;
            set
            {
                if (_selectedAnswer != value)
                {
                    _selectedAnswer = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool CanGoBack => _currentIndex > 0;
        public bool CanGoNext => _currentIndex < Exercises.Count - 1;

        public ICommand NextExerciseCommand { get; }
        public ICommand PreviousExerciseCommand { get; }
        public ICommand ExitCommand { get; }

        public ExerciseDetailViewModel()
        {
            NextExerciseCommand = new Command(() =>
            {
                if (_currentIndex < Exercises.Count - 1)
                {
                    _currentIndex++;
                    OnPropertyChanged(nameof(CurrentExercise));
                    OnPropertyChanged(nameof(CanGoBack));
                    OnPropertyChanged(nameof(CanGoNext));
                }
            });

            PreviousExerciseCommand = new Command(() =>
            {
                if (_currentIndex > 0)
                {
                    _currentIndex--;
                    OnPropertyChanged(nameof(CurrentExercise));
                    OnPropertyChanged(nameof(CanGoBack));
                    OnPropertyChanged(nameof(CanGoNext));
                }
            });

            ExitCommand = new Command(async () =>
            {
                await Shell.Current.GoToAsync("..");
            });
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
