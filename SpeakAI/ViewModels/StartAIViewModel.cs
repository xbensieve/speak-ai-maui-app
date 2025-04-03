using SpeakAI.Services.Interfaces;
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
    public class StartAIViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Matter> Topics { get; set; }
        private Matter _selectedTopic;
        public Matter SelectedTopic
        {
            get => _selectedTopic;
            set
            {
                if (_selectedTopic != value)
                {
                    _selectedTopic = value;
                    OnPropertyChanged();
                }
            }
        }
        public ICommand SelectTopicCommand { get; }
        public StartAIViewModel()
        {
            Topics = new ObservableCollection<Matter>
                {
                    new Matter { Id = 1, Name = "Daily Life and Routines", Description = "Discuss your daily activities, habits, and lifestyle" },
                    new Matter { Id = 2, Name = "Travel and Cultural Experiences", Description = "Share travel stories and cultural encounters" },
                    new Matter { Id = 3, Name = "Technology and Innovation", Description = "Explore modern technology trends and innovations" },
                    new Matter { Id = 4, Name = "Education and Career Development", Description = "Discuss learning experiences and career goals" }
                };
            SelectTopicCommand = new Command<Matter>(SelectTopic);
        }
        private void SelectTopic(Matter topic)
        {
            if (topic != null)
            {
                SelectedTopic = topic;
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class Matter
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
