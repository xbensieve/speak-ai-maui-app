using SpeakAI.Services.Interfaces;
using SpeakAI.ViewModels;

namespace SpeakAI.Views;

public partial class StartAIPage : ContentPage
{
    private Matter _selectedTopic;

    public StartAIPage()
	{
		InitializeComponent();
		BindingContext = new StartAIViewModel();
    }
    private void OnTopicTapped(object sender, TappedEventArgs e) 
    {
        if (e.Parameter is Matter selectedTopic)
        {
            _selectedTopic = selectedTopic;
            SelectedTopicLabel.Text = $"Selected: {selectedTopic.Name}\n{selectedTopic.Description}";
            StartButton.IsVisible = true;
        }
    }
    private async void OnStartButtonClicked(object sender, EventArgs e)
    {
        if (_selectedTopic != null)
        {
            var navigationParameter = new Dictionary<string, object>
        {
            { "topicId", _selectedTopic.Id }
        };

            await Shell.Current.GoToAsync("aitutor", navigationParameter);
        }
    }
}