using SpeakAI.ViewModels;

namespace SpeakAI.Views;

public partial class ExerciseDetailPage : ContentPage
{
	public ExerciseDetailPage()
	{
		InitializeComponent();
        BindingContext = new ExerciseDetailViewModel();
    }
    private void OnOptionSelected(object sender, CheckedChangedEventArgs e)
    {
        if (sender is RadioButton radioButton)
        {
            var viewModel = BindingContext as ExerciseDetailViewModel;
            if (viewModel != null)
            {
                viewModel.SelectedAnswer = radioButton.Content.ToString();
            }
        }
    }
    private async void OnNextExercise(object sender, EventArgs e)
    {
        await ExerciseFrame.TranslateTo(-this.Width, 0, 300, Easing.CubicInOut);
        await ExerciseFrame.TranslateTo(0, 0, 300, Easing.CubicInOut);
    }

    private async void OnPreviousExercise(object sender, EventArgs e)
    {
        await ExerciseFrame.TranslateTo(this.Width, 0, 300, Easing.CubicInOut);
        await ExerciseFrame.TranslateTo(0, 0, 300, Easing.CubicInOut);
    }
}