using AnimalChessLibrary;
using Caliburn.Micro;
using SocketWrapperLibrary;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using WpfCMExperiments.Network;
using WpfCMExperiments.Network.Gameplay;
using System.Windows.Media;
using Color = System.Windows.Media.Color;

namespace WpfCMExperiments.ViewModels
{
    public class LANViewModel : SinglePlayerViewModel
    {
        private SocketHandler socketHandler; // Implement this, then use here
        private bool _connectionDebounce;
        private Visibility _gridHostMenuVisibility;
        private string _hostConnectionStatus;
        private string _hostConnectionStatusDetails;
        private BindableCollection<IPAddress> _hostIPs;
        private Visibility _gridJoinMenuVisibility;
        private string _joinConnectionStatus;
        private string _joinConnectionStatusDetails;
        private string _joinInputIPText;
        private Visibility _connectionMainMenuVisibility;
        private Visibility _preConnectionOverlayVisibility;
        private string _playerNickname;
        private string _opponentNickname;
        private bool _gameStarted;
        private bool _playerTurn;
        private bool _isPlayerOne;
        private Visibility _gameInfoVisibility;

        public string PlayerNickname
        {
            get => _playerNickname;
            set
            {
                _playerNickname = value;
                NotifyOfPropertyChange("PlayerNickname");
            }
        }
        public string OpponentNickname { 
            get => _opponentNickname;
            set
            {
                _opponentNickname = value;
                NotifyOfPropertyChange("OpponentNickname");
            }
        }
        public bool PlayerTurn {
            get => _playerTurn;
            set {
                _playerTurn = value;
                NotifyOfPropertyChange("PlayerTurnIndicatorVisibility");
                NotifyOfPropertyChange("OpponentTurnIndicatorVisibility");
            }
        }
        public Visibility PlayerTurnIndicatorVisibility => PlayerTurn ? Visibility.Visible : Visibility.Hidden;
        public Visibility OpponentTurnIndicatorVisibility => !PlayerTurn ? Visibility.Visible : Visibility.Hidden;
        public Visibility GameInfoVisibility { 
            get => _gameInfoVisibility;
            set
            {
                _gameInfoVisibility = value;
                NotifyOfPropertyChange("GameInfoVisibility");
            }
        }
        public bool IsPlayerOne
        {
            get => _isPlayerOne;
            set
            {
                _isPlayerOne = value;
                NotifyOfPropertyChange("PlayerGameInfoAlignment");
                NotifyOfPropertyChange("OpponentGameInfoAlignment");
                NotifyOfPropertyChange("PlayerGameInfoFlowDirection");
                NotifyOfPropertyChange("OpponentGameInfoFlowDirection");
                NotifyOfPropertyChange("PlayerGameInfoBorderColor");
                NotifyOfPropertyChange("OpponentGameInfoBorderColor");
                NotifyOfPropertyChange("PlayerGameInfoBackgroundColor");
                NotifyOfPropertyChange("OpponentGameInfoBackgroundColor");
                NotifyOfPropertyChange("PlayerGameInfoPlayerNumber");
                NotifyOfPropertyChange("OpponentGameInfoPlayerNumber");
            }
        }
        public HorizontalAlignment PlayerGameInfoAlignment => IsPlayerOne ? HorizontalAlignment.Left : HorizontalAlignment.Right;
        public HorizontalAlignment OpponentGameInfoAlignment => !IsPlayerOne ? HorizontalAlignment.Left : HorizontalAlignment.Right;
        public FlowDirection PlayerGameInfoFlowDirection => IsPlayerOne ? FlowDirection.LeftToRight : FlowDirection.RightToLeft;
        public FlowDirection OpponentGameInfoFlowDirection => !IsPlayerOne ? FlowDirection.LeftToRight : FlowDirection.RightToLeft;
        public Brush PlayerGameInfoBorderColor => IsPlayerOne ? Brushes.Red : Brushes.Blue;
        public Brush OpponentGameInfoBorderColor => !IsPlayerOne ? Brushes.Red : Brushes.Blue;
        public Brush PlayerGameInfoBackgroundColor => IsPlayerOne ? Brushes.Tomato : Brushes.LightBlue;
        public Brush OpponentGameInfoBackgroundColor => !IsPlayerOne ? Brushes.Tomato : Brushes.LightBlue;
        public string PlayerGameInfoPlayerNumber => IsPlayerOne ? "P1" : "P2";
        public string OpponentGameInfoPlayerNumber => !IsPlayerOne ? "P1" : "P2";

        // To avoid multiple requests of connection at the same time
        public bool ConnectionDebounce
        {
            get => _connectionDebounce;
            private set
            {
                _connectionDebounce = value;
                NotifyOfPropertyChange("ConnectionDebounce");
            }
        }

        public Visibility GridHostMenuVisibility
        {
            get => _gridHostMenuVisibility;
            private set
            {
                _gridHostMenuVisibility = value;
                NotifyOfPropertyChange("GridHostMenuVisibility");
            }
        }
        public string HostConnectionStatus
        {
            get => _hostConnectionStatus;
            private set
            {
                _hostConnectionStatus = value;
                NotifyOfPropertyChange("HostConnectionStatus");
            }
        }
        public string HostConnectionStatusDetails
        {
            get => _hostConnectionStatusDetails;
            private set
            {
                _hostConnectionStatusDetails = value;
                NotifyOfPropertyChange("HostConnectionStatusDetails");
            }
        }

        public BindableCollection<IPAddress> HostIPs
        {
            get => _hostIPs;
            private set
            {
                _hostIPs = value;
                NotifyOfPropertyChange("HostIPs");
            }
        }

        public Visibility GridJoinMenuVisibility
        {
            get => _gridJoinMenuVisibility;
            private set
            {
                _gridJoinMenuVisibility = value;
                NotifyOfPropertyChange("GridJoinMenuVisibility");
            }
        }
        public string JoinConnectionStatus
        {
            get => _joinConnectionStatus;
            private set
            {
                _joinConnectionStatus = value;
                NotifyOfPropertyChange("JoinConnectionStatus");
            }
        }
        public string JoinConnectionStatusDetails
        {
            get => _joinConnectionStatusDetails;
            private set
            {
                _joinConnectionStatusDetails = value;
                NotifyOfPropertyChange("JoinConnectionStatusDetails");
            }
        }
        public string JoinInputIPText
        {
            get => _joinInputIPText;
            set
            {
                _joinInputIPText = value;
                NotifyOfPropertyChange("JoinInputIPText");
            }
        }

        public Visibility ConnectionMainMenuVisibility
        {
            get => _connectionMainMenuVisibility;
            private set
            {
                _connectionMainMenuVisibility = value;
                NotifyOfPropertyChange("ConnectionMainMenuVisibility");
            }
        }
        public Visibility PreConnectionOverlayVisibility
        {
            get => _preConnectionOverlayVisibility;
            private set
            {
                _preConnectionOverlayVisibility = value;
                NotifyOfPropertyChange("PreConnectionOverlayVisibility");
            }
        }

        public LANViewModel() : base() {
            boardVM.AllowMoves = false; // Make connection first

            ShowMainConnectMenu();
            ConnectionDebounce = false;
            HostConnectionStatus = "Idle";
            JoinConnectionStatus = "Idle";
            JoinInputIPText = "127.0.0.1";

            GameInfoVisibility = Visibility.Hidden;
        }


        // Simple Panel visibility togglers
        public async Task LoadHostScreen()
        {
            //MessageBox.Show("Username: " + PlayerNickname);
            GridHostMenuVisibility = Visibility.Visible;
            GridJoinMenuVisibility = Visibility.Hidden;
            await StartHost(); // Maybe add cancellationToken in the future
        }
        public void LoadJoinScreen()
        {
            //MessageBox.Show("Username: " + PlayerNickname);
            GridHostMenuVisibility = Visibility.Hidden;
            GridJoinMenuVisibility = Visibility.Visible;
        }
        public void ShowMainConnectMenu()
        {
            GridHostMenuVisibility = Visibility.Hidden;
            GridJoinMenuVisibility = Visibility.Hidden;
            PreConnectionOverlayVisibility = Visibility.Visible;
            ConnectionMainMenuVisibility = Visibility.Visible;
        }
        public void PrepareUIForGameplay()
        {
            GridHostMenuVisibility = Visibility.Hidden;
            GridJoinMenuVisibility = Visibility.Hidden;
            PreConnectionOverlayVisibility = Visibility.Hidden;
            ConnectionMainMenuVisibility = Visibility.Hidden;
            GameInfoVisibility = Visibility.Visible;
        }


        // Action from UI (Kinda)
        public async Task StartHost()
        {
            if (ConnectionDebounce) return;
            ConnectionDebounce = true;

            HostConnectionStatus = "Creating host";
            socketHandler = SocketHandler.CreateHost(ConnectionTypes.Gameplay);
            if (!socketHandler.StartHost())
            {
                MessageBox.Show("Failed to create host.");
                ConnectionDebounce = false;
                return;
            }

            // Show user their corrent IPs
            HostConnectionStatus = "Gathering host IPs";
            HostIPs = new BindableCollection<IPAddress>(NetworkHelper.GetAllHostIPv4s());

            // Wait for opponent to send nickname
            HostConnectionStatus = "Sharing data (1/2)";
            OpponentNickname = await GetOpponentNickname();

            // Try to send nickname
            HostConnectionStatus = "Sharing data (2/2)";
            if (!(await SendOpponentNickname(PlayerNickname))) {
                JoinConnectionStatusDetails = "Failed to share nickname...";
                ConnectionDebounce = false;
                return;
            }

            // Success from here (+ give time to client to listen for move)
            HostConnectionStatus = $"Player connected: {OpponentNickname}";
            await Task.Delay(1000);
            HostConnectionStatus = "Starting...";
            await Task.Delay(1000);

            // Show board
            PrepareUIForGameplay();

            IsPlayerOne = true;
            PlayerTurn = true;
            boardVM.AllowMoves = PlayerTurn;
            ConnectionDebounce = false;
        }

        public bool CanStartHost() => !ConnectionDebounce;

        // Action from UI
        public async Task ConnectToHost()
        {
            if (ConnectionDebounce) return;
            ConnectionDebounce = true;

            JoinConnectionStatus = "Creating connection";
            socketHandler = SocketHandler.CreateClient(ConnectionTypes.Gameplay);

            // Try connecting
            JoinConnectionStatus = "Establishing connection...";
            if (!socketHandler.ConnectClient(JoinInputIPText))
            {
                JoinConnectionStatus = "Failed to connect.";
                ConnectionDebounce = false;
                return;
            }

            // Try to send nickname
            JoinConnectionStatus = "Sharing data (1/2)";
            if (!(await SendOpponentNickname(PlayerNickname))) {
                JoinConnectionStatusDetails = "Failed to share nickname...";
                ConnectionDebounce = false;
                return;
            }

            // Wait for their nickname
            JoinConnectionStatus = "Sharing data (2/2)";
            OpponentNickname = await GetOpponentNickname();

            // Success from here
            JoinConnectionStatus = "Joined!";
            JoinConnectionStatusDetails = "Playing against: " + OpponentNickname;
            await Task.Delay(500);

            // Show board
            PrepareUIForGameplay();

            IsPlayerOne = false;
            PlayerTurn = false; // After connection, wait for first move
            boardVM.AllowMoves = PlayerTurn;
            await WaitOpponentMove();

            ConnectionDebounce = false;
        }

        public bool CanConnectToHost() => !ConnectionDebounce;

        // Logic
        public async Task<bool> SendOpponentNickname(string nickname, int tryLimit = 5)
        {
            // Try share nickname multiple times
            int tryCounter = 0;
            do
            {
                await Task.Delay(250 * tryCounter);
                if (socketHandler.SendData(new SocketMessage(nickname, (byte)NetworkDataTypes.ConnectionNicknameSharing))) return true;

            } while (tryCounter++ < tryLimit);

            return false;            
        }

        public async Task<string> GetOpponentNickname()
        {
            SocketMessage? message = null;

            do
            {
                HostConnectionStatusDetails = "Getting nickname (1/2)"; // maybe add a counter so the user can track the amount of tries?
                JoinConnectionStatusDetails = "Getting nickname (1/2)";

                ISocketMessage? iMessage = null;
                Task<bool> receiveTask = Task.Run(() => socketHandler.ReceiveData(out iMessage!));

                if (!await receiveTask) continue; // Receive the player nickname
                message = (SocketMessage)iMessage!; // (probably won't be null, hence the ! )

                HostConnectionStatusDetails = "Getting nickname (2/2)";
                JoinConnectionStatusDetails = "Getting nickname (2/2)";

            } while (message == null || message.ClassId != (byte)NetworkDataTypes.ConnectionNicknameSharing);

            //SocketMessage message = SocketMessage.UnformatByteArrayToClass(((SocketMessage)iMessage).Data);
            List<object> data = SocketMessageProtocol.GetUnformattedBytes(message.Data);

            // CHECK THE DATA VALUES (TRUE, NICK)

            return (string)data[0];
        }

        public async Task WaitOpponentMove(int retryLimit = 3)
        {
            boardVM.AllowMoves = false;
            PlayerTurn = false;

            ISocketMessage? iMessage = null;

            int retryCount = 0;
            //while (retryCount++ < retryLimit && !(await new Task<bool>(() => socketHandler.ReceiveData(out message)))) 
            //{
            //    if (retryCount == retryLimit) {
            //        MessageBox.Show("Failed too many times, stopping task.");
            //        return;
            //    }
            //    else MessageBox.Show("There was an error while trying to get the opponent's move. This may break the gaming experience. Retrying... (Press OK)");
            //}

            Task<bool> receiveTask = Task.Run(() => socketHandler.ReceiveData(out iMessage!));
            if (!await receiveTask)
            {
                MessageBox.Show("Failed (12521312)");
            }

            MoveSocketMessage moveMessage = (MoveSocketMessage)iMessage!; // I think it is guaranteed to not be null here (hence the ! to remove the warning)
            boardVM.PlayMove(moveMessage.OriginTile, moveMessage.TargetTile);

            boardVM.AllowMoves = true;
            PlayerTurn = true;
        }

        // Only case where async void is valid supposedly?
        public override async void OnMovePlayed(object? sender, MoveResult result)
        {
            if (!result.Success) return;

            // If registering opponent move, just throw it to base implementation (avoiding infinite recursion)
            if (!PlayerTurn)
            {
                base.OnMovePlayed(sender, result);
                return;
            }

            // Send move
            socketHandler.SendData(new MoveSocketMessage(((byte) result.OriginPosition.Value.Item1, (byte)result.OriginPosition.Value.Item2), 
                                                         ((byte) result.TargetPosition.Value.Item1, (byte)result.TargetPosition.Value.Item2)));
            base.OnMovePlayed(sender, result);
            if (boardVM.GameFinished) return;

            // Receive Move
            await WaitOpponentMove();
            if (boardVM.GameFinished) return;
        }
    }
}
