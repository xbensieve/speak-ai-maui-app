using CommunityToolkit.Maui.Media;
using Microsoft.AspNetCore.SignalR.Client;
using SpeakAI.Services.Interfaces;
using SpeakAI.Services.Models;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SpeakAI.Views
{
    [QueryProperty(nameof(TopicId), "topicId")]
    public partial class AITutorPage : ContentPage
    {
        private readonly IAIService _aIService;
        private int _topicId;
        public int TopicId
        {
            get => _topicId;
            set
            {
                if (_topicId != value)
                {
                    _topicId = value;
                    OnPropertyChanged(nameof(TopicId));
                    _ = FetchTopicDataAsync();
                }
            }
        }
        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }
        private bool _isLoaded;
        public bool IsLoaded
        {
            get => _isLoaded;
            set
            {
                _isLoaded = value;
                OnPropertyChanged(nameof(IsLoaded));
            }
        }
        private HubConnection _hubConnection;
        private readonly ISpeechToText speechToText;
        private CancellationTokenSource tokenSource;
        public CultureInfo culture { get; set; } = CultureInfo.CurrentCulture;
        private ObservableCollection<string> _messages = new();
        public ObservableCollection<string> Messages
        {
            get => _messages;
            set
            {
                _messages = value;
                OnPropertyChanged(nameof(Messages));
            }
        }

        private readonly Dictionary<string, Frame> _messageFrames = new();

        public AITutorPage(ISpeechToText speechToText, IAIService aIService)
        {
            InitializeComponent();

            this.speechToText = speechToText ?? throw new ArgumentNullException(nameof(speechToText));
            this._aIService = aIService ?? throw new ArgumentNullException(nameof(aIService));
            BindingContext = this;
            MessagesListView.ItemsSource = Messages;
            SetupAnimations();
        }

        private void SetupAnimations()
        {
            this.PropertyChanged += async (s, e) =>
            {
                if (e.PropertyName == nameof(IsLoading) && IsLoading)
                {
                    LoadingIndicator.Scale = 0.5;
                    await LoadingIndicator.ScaleTo(1.0, 800, Easing.BounceOut);
                }
            };

            Messages.CollectionChanged += async (s, e) =>
            {
                if (e.NewItems != null && e.NewItems.Count > 0)
                {
                    foreach (string newMessage in e.NewItems)
                    {
                        await Task.Delay(100);
                        if (_messageFrames.TryGetValue(newMessage, out var frame))
                        {
                            frame.Opacity = 0;
                            frame.TranslationY = 20;
                            await Task.WhenAll(
                                frame.FadeTo(1, 600, Easing.SinOut),
                                frame.TranslateTo(0, 0, 600, Easing.SinOut)
                            );
                        }
                    }
                }
            };

            AddButtonPressEffect(SendButton);
            AddButtonPressEffect(ListenButton);

            MessagesListView.ItemTemplate = new DataTemplate(() =>
            {
                var frame = new Frame
                {
                    Padding = new Thickness(15),
                    Margin = new Thickness(5),
                    CornerRadius = 12,
                    HasShadow = true,
                    BackgroundColor = Colors.White,
                    Opacity = 0
                };

                var stackLayout = new StackLayout { Padding = new Thickness(5) };
                var label = new Label
                {
                    FontSize = 18,
                    FontFamily = "OpenSans-SemiBold",
                    TextColor = Color.FromArgb("#333"),
                    LineBreakMode = LineBreakMode.WordWrap
                };
                label.SetBinding(Label.TextProperty, ".");

                stackLayout.Children.Add(label);
                frame.Content = stackLayout;
                frame.BindingContextChanged += (s, e) =>
                {
                    if (frame.BindingContext is string message)
                    {
                        if (!_messageFrames.ContainsKey(message))
                        {
                            _messageFrames[message] = frame;
                        }
                        else
                        {
                            _messageFrames.Remove(message);
                        }
                    }
                };
                return frame;
            });
        }
        private async Task FetchTopicDataAsync()
        {
            IsLoading = true;
            IsLoaded = false;
            try
            {
                if (_aIService == null)
                {
                    throw new InvalidOperationException("AI Service is not initialized!");
                }

                var topic = new TopicModel
                {
                    topicId = _topicId
                };

                var result = await _aIService.StartTopicAsync(topic);

                if (result != null)
                {
                    IsLoaded = true;
                    IsLoading = false;
                    var locales = await Microsoft.Maui.Media.TextToSpeech.GetLocalesAsync();
                    var selectedLocale = locales.FirstOrDefault(l => l.Language.StartsWith("en"));
                    var speechOptions = new SpeechOptions()
                    {
                        Locale = selectedLocale ?? locales.FirstOrDefault()
                    };
                    MessageEntry.IsEnabled = false;
                    ListenButton.IsEnabled = false;
                    await Task.WhenAll(
                        TextToSpeech.SpeakAsync(result.BotResponse, speechOptions),
                        Task.Run(() =>
                        {
                            MainThread.BeginInvokeOnMainThread(() =>
                            {
                                Messages.Add($"AI: {result.BotResponse}");
                                ScrollToLastMessage();
                            });
                        }));
                    MessageEntry.IsEnabled = true;
                    ListenButton.IsEnabled = true;
                    ConnectToChatHub();
                }
                else
                {
                    await DisplayAlert("Error", "Failed to fetch data: No valid response.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to fetch data: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }
        private async void ConnectToChatHub()
        {
            try
            {
                _hubConnection = new HubConnectionBuilder()
                    .WithUrl("http://sai.runasp.net/chatHub", options =>
                    {
                        options.HttpMessageHandlerFactory = _ => new HttpClientHandler
                        {
                            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                        };
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
                        Messages.Add("AI is typing...");
                        ScrollToLastMessage();
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
                        TopicId = _topicId
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

                        string responseText = await _hubConnection.InvokeAsync<string>("SendMessage", chatMessage);

                        System.Diagnostics.Debug.WriteLine($"Bot Response: {responseText}");

                        Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(async () =>
                        {
                            var typingMessage = Messages.FirstOrDefault(msg => msg == "AI is typing...");
                            if (typingMessage != null)
                            {
                                Messages.Remove(typingMessage);
                            }
                            var locales = await Microsoft.Maui.Media.TextToSpeech.GetLocalesAsync();
                            var selectedLocale = locales.FirstOrDefault(l => l.Language.StartsWith("en"));
                            var speechOptions = new SpeechOptions()
                            {
                                Locale = selectedLocale ?? locales.FirstOrDefault()
                            };
                            MessageEntry.IsEnabled = false;
                            ListenButton.IsEnabled = false;
                            await Task.WhenAll(
                                 TextToSpeech.SpeakAsync(responseText, speechOptions),
                                 Task.Run(() =>
                                 {
                                     MainThread.BeginInvokeOnMainThread(() =>
                                     {
                                         Messages.Add($"AI: {responseText}");
                                         ScrollToLastMessage();
                                     });
                                 }));
                            await Task.Delay(100);
                            ScrollToLastMessage();
                            MessageEntry.IsEnabled = true;
                            ListenButton.IsEnabled = true;
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
            MessageEntry.Text = "";
            MessageEntry.IsEnabled = false;

            Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() =>
            {
                Messages.Add($"You: {userMessage}");
                Messages.Add("AI is typing...");
                ScrollToLastMessage();
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
                TopicId = _topicId
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

                string responseText = await _hubConnection.InvokeAsync<string>("SendMessage", chatMessage);

                System.Diagnostics.Debug.WriteLine($"Bot Response: {responseText}");

                Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(async () =>
                {
                    var typingMessage = Messages.FirstOrDefault(msg => msg == "AI is typing...");
                    if (typingMessage != null)
                    {
                        Messages.Remove(typingMessage);
                    }
                    var locales = await Microsoft.Maui.Media.TextToSpeech.GetLocalesAsync();
                    var selectedLocale = locales.FirstOrDefault(l => l.Language.StartsWith("en"));
                    var speechOptions = new SpeechOptions()
                    {
                        Locale = selectedLocale ?? locales.FirstOrDefault()
                    };
                    MessageEntry.IsEnabled = false;
                    ListenButton.IsEnabled = false;
                    await Task.WhenAll(
                        TextToSpeech.SpeakAsync(responseText, speechOptions),
                        Task.Run(() =>
                        {
                            MainThread.BeginInvokeOnMainThread(() =>
                            {
                                Messages.Add($"AI: {responseText}");
                                ScrollToLastMessage();
                            });
                        }));
                    await Task.Delay(100);
                    ScrollToLastMessage();
                    MessageEntry.IsEnabled = true;
                    ListenButton.IsEnabled = true;
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Send Message Error: {ex.Message}");
                await DisplayAlert("Error", $"Failed to send message: {ex.Message}", "OK");
                MessageEntry.IsEnabled = true;
            }
        }
        private void ScrollToLastMessage()
        {
            if (MessagesListView != null && Messages.Any())
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    MessagesListView.ScrollTo(Messages.Last(), position: ScrollToPosition.End, animate: true);
                });
            }
        }
        private void AddButtonPressEffect(Button button)
        {
            button.Pressed += async (s, e) =>
            {
                await button.ScaleTo(0.95, 100, Easing.Linear);
            };

            button.Released += async (s, e) =>
            {
                await button.ScaleTo(1.0, 100, Easing.SpringOut);
            };
        }
        public async void EndConversation_Clicked(object sender, EventArgs e)
        {
            try
            {
                IsLoading = true;

                string botResponse = await _hubConnection.InvokeAsync<string>("EndConversation");

                string formattedResponse = ParseBotResponse(botResponse);
                await _hubConnection.StopAsync();

                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await DisplayAlert("Conversation Ended", formattedResponse, "OK");
                    await Navigation.PopAsync();
                });
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to end conversation: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }
        private string ParseBotResponse(string botResponse)
        {
            try
            {
                var lines = botResponse.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

                string summary = "";
                string weaknesses = "";
                string suggestions = "";

                foreach (var line in lines)
                {
                    if (line.Contains("Summary:"))
                    {
                        summary = line.Replace("Summary:", "").Trim();
                    }
                    else if (line.Contains("Weaknesses:"))
                    {
                        weaknesses = line.Replace("Weaknesses:", "").Trim();
                    }
                    else if (line.Contains("Suggestions for Improvement:"))
                    {
                        suggestions = line.Replace("Suggestions for Improvement:", "").Trim();
                    }
                }

                return $"✨ Your Conversation Summary ✨\n" +
             $"═══════════════════════\n" +
             $"📝 *Summary*\n{summary}\n\n" +
             $"⚠️ *Weaknesses*\n{weaknesses}\n\n" +
             $"💡 *Suggestions*\n{suggestions}\n" +
             $"═══════════════════════\n" +
             $"Keep up the great work! 🚀";
            }
            catch
            {
                return $"⚠️ Conversation Summary ⚠️\n" +
               $"═══════════════════════\n" +
               $"{botResponse}\n" +
               $"═══════════════════════\n";
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