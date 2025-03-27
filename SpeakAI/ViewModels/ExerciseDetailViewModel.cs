using SpeakAI.Services.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using SpeakAI.Services.Interfaces;
using System.Diagnostics;

namespace SpeakAI.ViewModels
{
    [QueryProperty(nameof(Topic), "topic")]
    public class ExerciseDetailViewModel : INotifyPropertyChanged
    {
        private readonly ICourseService _courseService;
        private Topic _topic;
        private int _currentIndex;
        private string _selectedAnswer;
        private bool _isAnswerVisible;

        public bool IsAnswerVisible
        {
            get => _isAnswerVisible;
            set
            {
                if (_isAnswerVisible != value)
                {
                    _isAnswerVisible = value;
                    OnPropertyChanged(nameof(SelectedAnswer));
                }
            }
        }

        public void ToggleAnswerVisibility()
        {
            IsAnswerVisible = SelectedAnswer == CurrentExercise?.ContentExercises.Answer;
            OnPropertyChanged(nameof(IsAnswerVisible));
        }

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
        public Exercise CurrentExercise => Exercises != null && Exercises.Count > 0 && _currentIndex >= 0 && _currentIndex < Exercises.Count ? Exercises[_currentIndex] : null;
        public string SelectedAnswer
        {
            get => _selectedAnswer;
            set
            {
                if (_selectedAnswer != value)
                {
                    _selectedAnswer = value;
                    Console.WriteLine($"[DOTNET] SelectedAnswer set to: {value}");
                    OnPropertyChanged(nameof(SelectedAnswer));
                }
            }
        }

        public bool CanGoBack => _currentIndex > 0;
        public bool CanGoNext => _currentIndex < Exercises.Count - 1;

        public ICommand NextExerciseCommand { get; }
        public ICommand PreviousExerciseCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand SubmitAnswerCommand { get; }

        public ExerciseDetailViewModel(ICourseService courseService)
        {
            _courseService = courseService;
            NextExerciseCommand = new Command(() =>
            {
                try
                {
                    if (Exercises == null || Exercises.Count == 0)
                    {
                        Console.WriteLine("Exercise list is empty or null.");
                        Application.Current.MainPage.DisplayAlert("Error", "No exercises available.", "OK");
                        return;
                    }

                    if (_currentIndex < Exercises.Count - 1)
                    {
                        _currentIndex++;
                        IsAnswerVisible = false;
                        OnPropertyChanged(nameof(IsAnswerVisible));
                        OnPropertyChanged(nameof(CurrentExercise));
                        OnPropertyChanged(nameof(CanGoBack));
                        OnPropertyChanged(nameof(CanGoNext));
                    }
                    else
                    {
                        Application.Current.MainPage.DisplayAlert("End", "This is the last exercise.", "OK");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in NextExerciseCommand: {ex.Message}");
                    Application.Current.MainPage.DisplayAlert("Error", $"Something went wrong: {ex.Message}", "OK");
                }
            });



            PreviousExerciseCommand = new Command(() =>
            {
                if (_currentIndex > 0)
                {
                    _currentIndex--;
                    IsAnswerVisible = false;
                    OnPropertyChanged(nameof(IsAnswerVisible));
                    OnPropertyChanged(nameof(CurrentExercise));
                    OnPropertyChanged(nameof(CanGoBack));
                    OnPropertyChanged(nameof(CanGoNext));
                }
                else
                {
                    Console.WriteLine("No previous exercises.");
                }
            });

            ExitCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
            SubmitAnswerCommand = new Command(OnSubmitAnswer);
        }

        private async void OnSubmitAnswer()
        {
            try
            {
                if (string.IsNullOrEmpty(SelectedAnswer) || CurrentExercise == null)
                {
                    Console.WriteLine("No answer selected or no current exercise.");
                    IsAnswerVisible = false;
                }
                else
                {
                    Console.WriteLine($"Answer submitted: {SelectedAnswer}");
                    if (SelectedAnswer == CurrentExercise?.ContentExercises.Answer)
                    {
                        IsAnswerVisible = true;
                        SelectedAnswer = string.Empty;
                        var exerciseId = CurrentExercise?.ExerciseId;
                        var earnedPoints = CurrentExercise?.MaxPoint;
                        _ = Task.Run(async () =>
                        {
                            try
                            {
                                var response = await _courseService.SubmitExerciseResult(exerciseId, (decimal)earnedPoints);
                                if (response.IsSuccess)
                                {
                                    SelectedAnswer = string.Empty;
                                    Console.WriteLine("Submit success");
                                }
                                SelectedAnswer = string.Empty;
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine($"Error submitting exercise: {ex.Message}");
                            }
                        });
                    }
                    else
                    {
                        Application.Current.MainPage.DisplayAlert("Wrong", "Answer not correct", "OK");
                    }
                }

                OnPropertyChanged(nameof(IsAnswerVisible));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error submitting answer: {ex.Message}");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
