using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using TwitterProject.Shared.Models;

namespace TwitterProject.Client.Services
{
    public class SignalRService : IAsyncDisposable
    {
        private HubConnection? _hubConnection;
        private readonly NavigationManager _navigationManager;
        private readonly ILoggerProvider _loggerProvider;

        //Event for when we receive metrics
        public event Action<TweetMetricStreamModel>? TweetMetricReceived;
        //Event for when we receive a tweet
        public event Action<TweetModel>? TweetReceived;
        public SignalRService(NavigationManager navigationManager, ILoggerProvider loggerProvider)
        {
            _navigationManager = navigationManager;
            _loggerProvider = loggerProvider;
            _hubConnection = new HubConnectionBuilder()
            .WithUrl(_navigationManager.ToAbsoluteUri("/tweetStream"))
             .AddJsonProtocol(options =>
             {
                 options.PayloadSerializerOptions.IgnoreReadOnlyFields = true;
                 options.PayloadSerializerOptions.IgnoreReadOnlyProperties = true;
                 options.PayloadSerializerOptions.PropertyNameCaseInsensitive = true;
             })
                .ConfigureLogging(x => {
                    x.AddProvider(_loggerProvider);
                    x.SetMinimumLevel(LogLevel.Warning);
                })
            .Build();

            _hubConnection.On<TweetMetricStreamModel>("Metrics", (streamModel) =>
            {
                TweetMetricReceived?.Invoke(streamModel);
            });
            _hubConnection.On<TweetModel>("Tweets", (tweet) =>
            {
                TweetReceived?.Invoke(tweet);
            });

        }
        //Start the hubConnection
        public async void StartSignalRStream()
        {
            if(_hubConnection != null) await _hubConnection.StartAsync();
        }
        public async void SetLanguageFilter(string languageCode)
        {
            await _hubConnection.InvokeAsync("SetLanguage", languageCode);
        }
        //Set the state of the hubConnection
        public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

        public async ValueTask DisposeAsync()
        {
            if (_hubConnection is not null)
            {
                await _hubConnection.DisposeAsync();
            }
        }
    }
}
