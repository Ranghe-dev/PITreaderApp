using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using PITreaderApp.Helpers;
using PITreaderApp.Models;
using PITreaderApp.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PITreaderApp
{
    public partial class MainWindow : Window
    {
        #region Services

        private PitReaderClient _pitReaderClient;
        private SettingsService _settingsService;

        #endregion

        #region UI Controls

        private TextBox _ipTextBox;
        private TextBox _tokenTextBox;

        private TextBlock _connectionStatusText;
        private TextBlock _connectionInfoText;

        private TextBlock _firmwareText;
        private TextBlock _authenticatedText;
        private TextBlock _securityIdText;
        private TextBlock _uidText;
        private TextBlock _permissionText;

        private Border _ledStatusCard;
        private TextBlock _ledTitleText;
        private TextBlock _ledModeText;

        private StackPanel _logPanel;

        private TextBlock _dashboardAuthText;
        private TextBlock _dashboardSecurityText;

        #endregion

        #region State and Timers

        private DispatcherTimer _timer;
        private DispatcherTimer _ledBlinkTimer;

        private string _currentIp = string.Empty;

        private bool _ledVisible = true;
        private IBrush _currentLedBrush = Brushes.Gray;

        #endregion

        public MainWindow()
        {
            InitializeComponent();

            InitializeControls();
            InitializeServices();
            InitializeTimers();
            InitializeEvents();
        }

        #region Initialization

        private void InitializeControls()
        {
            _ipTextBox = this.FindControl<TextBox>("IpTextBox")!;
            _tokenTextBox = this.FindControl<TextBox>("TokenTextBox")!;

            _connectionStatusText =
                this.FindControl<TextBlock>("ConnectionStatusText")!;

            _connectionInfoText =
                this.FindControl<TextBlock>("ConnectionInfoText")!;

            _firmwareText =
                this.FindControl<TextBlock>("FirmwareText")!;

            _authenticatedText =
                this.FindControl<TextBlock>("AuthenticatedText")!;

            _securityIdText =
                this.FindControl<TextBlock>("SecurityIdText")!;

            _uidText =
                this.FindControl<TextBlock>("UidText")!;

            _permissionText =
                this.FindControl<TextBlock>("PermissionText")!;

            _ledStatusCard =
                this.FindControl<Border>("LedStatusCard")!;

            _ledTitleText =
                this.FindControl<TextBlock>("LedTitleText")!;

            _ledModeText =
                this.FindControl<TextBlock>("LedModeText")!;

            _logPanel =
                this.FindControl<StackPanel>("LogPanel")!;

            _dashboardAuthText =
                this.FindControl<TextBlock>("DashboardAuthText")!;

            _dashboardSecurityText =
                this.FindControl<TextBlock>("DashboardSecurityText")!;
        }

        private void InitializeServices()
        {
            _settingsService = new SettingsService();

            var settings = _settingsService.Load();

            _ipTextBox.Text = settings.IpAddress;
            _tokenTextBox.Text = settings.ApiToken;

            _pitReaderClient = new PitReaderClient();
        }

        private void InitializeTimers()
        {
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };

            _timer.Tick += async (_, _) => await UpdateStatus();

            _ledBlinkTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(500)
            };

            _ledBlinkTimer.Tick += (_, _) =>
            {
                _ledVisible = !_ledVisible;

                _ledStatusCard.BorderBrush =
                    _ledVisible
                        ? _currentLedBrush
                        : Brushes.Transparent;
            };
        }

        private void InitializeEvents()
        {
            var connectButton =
                this.FindControl<Button>("ConnectButton");

            connectButton.Click += async (_, _) =>
                await ConnectButton_Click();
        }

        #endregion

        #region Connection

        private async Task ConnectButton_Click()
        {
            string ip = _ipTextBox.Text ?? string.Empty;
            _currentIp = ip;

            string token = _tokenTextBox.Text ?? string.Empty;
            _pitReaderClient.SetToken(token);

            try
            {
                _settingsService.Save(new AppSettings
                {
                    IpAddress = ip,
                    ApiToken = token
                });

                var status =
                    await _pitReaderClient.GetStatusAsync(ip);

                if (status == null)
                {
                    SetConnectionState(false);
                    return;
                }

                UpdateUi(status);

                // Caricamento registro
                await LoadDiagnosticLog();

                if (!_timer.IsEnabled)
                {
                    _timer.Start();
                }

                SetConnectionState(true);
            }
            catch (Exception)
            {
                _firmwareText.Text = "Errore di connessione";
                _authenticatedText.Text = string.Empty;
                _securityIdText.Text = string.Empty;
                _uidText.Text = string.Empty;
                _permissionText.Text = string.Empty;
            }

            // Manteniamo il comportamento attuale:
            // il pulsante Salva viene collegato quando si effettua la connessione.
            var saveButton =
                this.FindControl<Button>("SaveSettingsButton");

            saveButton.Click += (_, _) =>
            {
                _settingsService.Save(new AppSettings
                {
                    IpAddress = _ipTextBox.Text ?? string.Empty,
                    ApiToken = _tokenTextBox.Text ?? string.Empty
                });
            };
        }

        private async Task UpdateStatus()
        {
            try
            {
                var status =
                    await _pitReaderClient.GetStatusAsync(_currentIp);

                if (status == null)
                {
                    return;
                }

                UpdateUi(status);
                SetConnectionState(true);
            }
            catch (Exception ex)
            {
                SetConnectionState(false);
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        private void SetConnectionState(bool connected)
        {
            _connectionStatusText.Text = connected
                ? "🟢 Stato: Connesso"
                : "🔴 Stato: Disconnesso";

            _connectionInfoText.Text = connected
                ? $"Connesso a {_currentIp} - Ultimo aggiornamento {DateTime.Now:HH:mm:ss}"
                : "In attesa di connessione...";
        }

        #endregion

        #region Device Status

        private void UpdateUi(StatusResponse status)
        {
            _firmwareText.Text =
                $"Firmware: {status.Status.FwVersion}";

            _authenticatedText.Text =
                $"Autenticato: {status.Authentication.Authenticated}";

            _securityIdText.Text =
                $"Security ID: {status.Authentication.SecurityId}";

            _uidText.Text =
                $"UID: {status.Authentication.TransponderUid}";

            _permissionText.Text =
                $"Permesso: {status.Authentication.Permission}";

            _dashboardAuthText.Text =
                status.Authentication.Authenticated
                    ? "Autenticazione: OK"
                    : "Autenticazione: NO";

            _dashboardSecurityText.Text =
                $"Security ID: {status.Authentication.SecurityId}";

            UpdateLedCard(
                status.Led.Colour,
                status.Led.FlashMode == 1);

            SetConnectionState(true);
        }

        #endregion

        #region LED

        private void UpdateLedCard(int colour, bool isBlinking)
        {
            var (brush, name) = colour switch
            {
                0 => (Brushes.Gray, "Spento"),
                1 => (Brushes.DodgerBlue, "Blu"),
                2 => (Brushes.Gold, "Giallo"),
                3 => (Brushes.Red, "Rosso"),
                4 => (Brushes.LimeGreen, "Verde"),
                _ => (Brushes.DimGray, "Sconosciuto")
            };

            _currentLedBrush = brush;

            _ledTitleText.Text = $"LED {name}";
            _ledModeText.Text =
                isBlinking
                    ? "Lampeggiante [1Hz]"
                    : "Fisso";

            if (isBlinking)
            {
                if (!_ledBlinkTimer.IsEnabled)
                {
                    _ledBlinkTimer.Start();
                }

                return;
            }

            _ledBlinkTimer.Stop();

            _ledStatusCard.BorderBrush =
                _ledVisible
                    ? _currentLedBrush
                    : Brushes.DimGray;
        }

        #endregion

        #region Diagnostic Log

        private async Task LoadDiagnosticLog()
        {
            var log =
                await _pitReaderClient.GetDiagnosticLogAsync(_currentIp);

            if (log == null)
            {
                return;
            }

            _logPanel.Children.Clear();

            foreach (var item in log.Items.OrderByDescending(x => x.Timestamp))
            {
                AddLogCard(item);
            }
        }

        private void AddLogCard(DiagnosticLogItem item)
        {
            Grid grid = new Grid
            {
                RowDefinitions = new RowDefinitions("Auto,Auto,Auto"),
                ColumnDefinitions = new ColumnDefinitions("*,Auto")
            };

            grid.Children.Add(new TextBlock
            {
                Text =
                    $"{GetEventIcon(item.Id)}  {EventTranslator.GetDescription(item.Id)}",
                FontWeight = FontWeight.Bold,
                FontSize = 17
            });

            TextBlock time = new TextBlock
            {
                Text = item.Timestamp
                    .ToLocalTime()
                    .ToString("HH:mm:ss"),

                HorizontalAlignment =
                    Avalonia.Layout.HorizontalAlignment.Right
            };

            Grid.SetColumn(time, 1);
            grid.Children.Add(time);

            TextBlock sid = new TextBlock
            {
                // Text = parameter
            };

            Grid.SetRow(sid, 1);
            Grid.SetColumnSpan(sid, 2);
            grid.Children.Add(sid);

            TextBlock index = new TextBlock
            {
                Text = $"Indice: {item.Index}",
                Opacity = 0.7
            };

            Grid.SetRow(index, 2);
            Grid.SetColumnSpan(index, 2);
            grid.Children.Add(index);

            // card.Child = grid;
        }

        private string GetEventIcon(int id)
        {
            return id switch
            {
                20604 => "🟢",
                20605 => "🔴",
                20570 => "✅",
                _ => "ℹ️"
            };
        }

        #endregion
    }
}