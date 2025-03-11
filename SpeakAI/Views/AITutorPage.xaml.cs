using CommunityToolkit.Maui.Media;
using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;

namespace SpeakAI.Views
{
    public partial class AITutorPage : ContentPage
    {
        private HubConnection _hubConnection;
        private readonly ISpeechToText speechToText;
        private CancellationTokenSource tokenSource;
        public CultureInfo culture { get; set; } = CultureInfo.CurrentCulture;
        public ObservableCollection<string> Messages { get; set; } = new();
        public AITutorPage() : this(SpeechToText.Default) { }
        public AITutorPage(ISpeechToText speechToText)
        {
            InitializeComponent();
            this.speechToText = speechToText;
            BindingContext = this;
            MessagesListView.ItemsSource = Messages;
            ConnectToChatHub();
        }

        private async void ConnectToChatHub()
        {
            try
            {
                _hubConnection = new HubConnectionBuilder()
                    .WithUrl("http://10.87.46.36:5232/chatHub", options => {
                        options.HttpMessageHandlerFactory = _ => new HttpClientHandler { ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true };
                    })
            .WithAutomaticReconnect()
            .Build();

                await _hubConnection.StartAsync();
                System.Diagnostics.Debug.WriteLine("Connected to ChatHub successfully.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Connection Error: {ex.Message}");
                await DisplayAlert("Connection Error", $"Failed to connect: {ex.Message}", "OK");
            }
        }
        public async void Listen(object sender, EventArgs args)
        {
            tokenSource = new CancellationTokenSource();
            try
            {
                var isAuthorized = await speechToText.RequestPermissions(tokenSource.Token);
                if (!isAuthorized)
                {
                    await DisplayAlert("Permission Error", "Microphone access denied.", "OK");
                    return;
                }

                var result = await speechToText.ListenAsync(CultureInfo.CurrentCulture, null, tokenSource.Token);

                if (result.IsSuccessful && !string.IsNullOrWhiteSpace(result.Text))
                {
                    Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() =>
                    {
                        Messages.Add($"You: {result.Text}");
                        Messages.Add("AI is typing..."); // Show "Typing..." status
                        ScrollToLastMessage(); // Scroll down
                    });

                    string token = await Xamarin.Essentials.SecureStorage.GetAsync("AccessToken");
                    string userId = Guid.NewGuid().ToString();

                    if (!string.IsNullOrEmpty(token))
                    {
                        try
                        {
                            var handler = new JwtSecurityTokenHandler();
                            var jwtToken = handler.ReadJwtToken(token);
                            string userIdStr = jwtToken.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;

                            if (!string.IsNullOrEmpty(userIdStr))
                            {
                                userId = userIdStr;
                            }
                        }
                        catch (Exception ex)
                        {
                            await DisplayAlert("Error", $"Invalid token: {ex.Message}", "OK");
                            MessageEntry.IsEnabled = true;
                            return;
                        }
                    }

                    var chatMessage = new ChatHubDTO
                    {
                        UserId = userId,
                        Message = result.Text,
                        TopicId = 1
                    };

                    if (_hubConnection.State != HubConnectionState.Connected)
                    {
                        await DisplayAlert("Error", "Not connected to chat server.", "OK");
                        MessageEntry.IsEnabled = true;
                        return;
                    }

                    try
                    {
                        System.Diagnostics.Debug.WriteLine($"Sending Message: {chatMessage.Message}");

                        // Send message and wait for AI response
                        string responseText = await _hubConnection.InvokeAsync<string>("SendMessage", chatMessage);

                        System.Diagnostics.Debug.WriteLine($"Bot Response: {responseText}");

                        Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(async() =>
                        {
                            Messages.Remove("AI is typing...");
                            var locales = await Microsoft.Maui.Media.TextToSpeech.GetLocalesAsync();
                            var selectedLocale = locales.FirstOrDefault(l => l.Language.StartsWith("en"));
                            var speechOptions = new SpeechOptions()
                            {
                                Locale = selectedLocale ?? locales.FirstOrDefault()
                            };
                            await Task.WhenAll(
                                    TextToSpeech.SpeakAsync(responseText, speechOptions),
                                    Task.Run(() => Messages.Add($"AI: {responseText}")));
                            MessageEntry.IsEnabled = true; // Re-enable input
                            ScrollToLastMessage(); // Scroll down
                        });
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Send Message Error: {ex.Message}");
                        await DisplayAlert("Error", $"Failed to send message: {ex.Message}", "OK");
                        MessageEntry.IsEnabled = true;
                    }
                }
                else
                {
                    Messages.Add("No speech detected.");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
        private async void SendMessage_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(MessageEntry.Text)) return;

            string userMessage = MessageEntry.Text;
            MessageEntry.Text = ""; // Clear input after sending
            MessageEntry.IsEnabled = false; // Disable input while AI is typing

            // Add user message to chat list
            Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() =>
            {
                Messages.Add($"You: {userMessage}");
                Messages.Add("AI is typing..."); // Show "Typing..." status
                ScrollToLastMessage(); // Scroll down
            });

            string token = await Xamarin.Essentials.SecureStorage.GetAsync("AccessToken");
            string userId = Guid.NewGuid().ToString();

            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadJwtToken(token);
                    string userIdStr = jwtToken.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;

                    if (!string.IsNullOrEmpty(userIdStr))
                    {
                        userId = userIdStr;
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Invalid token: {ex.Message}", "OK");
                    MessageEntry.IsEnabled = true;
                    return;
                }
            }

            var chatMessage = new ChatHubDTO
            {
                UserId = userId,
                Message = userMessage,
                TopicId = 1
            };

            if (_hubConnection.State != HubConnectionState.Connected)
            {
                await DisplayAlert("Error", "Not connected to chat server.", "OK");
                MessageEntry.IsEnabled = true;
                return;
            }

            try
            {
                System.Diagnostics.Debug.WriteLine($"Sending Message: {chatMessage.Message}");

                // Send message and wait for AI response
                string responseText = await _hubConnection.InvokeAsync<string>("SendMessage", chatMessage);

                System.Diagnostics.Debug.WriteLine($"Bot Response: {responseText}");

                Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(async () =>
                {
                    Messages.Remove("AI is typing...");
                    var locales = await Microsoft.Maui.Media.TextToSpeech.GetLocalesAsync();
                    var selectedLocale = locales.FirstOrDefault(l => l.Language.StartsWith("en"));
                    var speechOptions = new SpeechOptions()
                    {
                        Locale = selectedLocale ?? locales.FirstOrDefault()
                    };
                    await Task.WhenAll(
                            TextToSpeech.SpeakAsync(responseText, speechOptions),
                            Task.Run(() => Messages.Add($"AI: {responseText}")));
                    MessageEntry.IsEnabled = true; // Re-enable input
                    ScrollToLastMessage(); // Scroll down
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Send Message Error: {ex.Message}");
                await DisplayAlert("Error", $"Failed to send message: {ex.Message}", "OK");
                MessageEntry.IsEnabled = true;
            }
        }

        // Auto-scroll to the last message
        private void ScrollToLastMessage()
        {
            if (Messages.Count > 0)
            {
                MessagesListView.ScrollTo(Messages.Last(), ScrollToPosition.End, true);
            }
        }


    }

    public class ChatHubDTO
    {
        public string UserId { get; set; }
        public string Message { get; set; }
        public int TopicId { get; set; }
    }
}
