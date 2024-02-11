using AnimalChessLibrary;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfCMExperiments.Models.AnimalChessModels;
using WpfCMExperiments.Utils;

namespace WpfCMExperiments.ViewModels
{
    public class AnimalChessViewModel : Screen
    {
		private AnimalChessModel animalChessModel;
		private TileModel[,] _boardTiles;
		public TileModel[,] BoardTiles // Conflicted about having another persistent copy of the board, but whatever
		{
			get { return _boardTiles; }
			set { 
                _boardTiles = value;
                //NotifyOfPropertyChange("BoardTiles");
                NotifyOfPropertyChange("BoardTilesList");
            }
		}
        public BindableCollection<TileModel> BoardTilesList => new BindableCollection<TileModel>(AnimalChessLibrary.CollectionHelper.GetListFromGameBoard(BoardTiles));
        public BindableCollection<TileModel> StructuresList => new BindableCollection<TileModel>(AnimalChessLibrary.CollectionHelper.GetListFromGameBoard(BoardTiles));

        public int BoardColumnAmount => BoardTiles.GetLength(0);
        public int BoardRowAmount => BoardTiles.GetLength(1);
        public bool GameInProgress => animalChessModel.GameInProgress;
        public bool GameFinished => animalChessModel.GameFinished;
        public bool PlayerOneWon => animalChessModel.PlayerOneWon;

        private (int x, int y)? _selectedTile;
        public (int x, int y)? SelectedTile { 
            get => _selectedTile;
            set {
                if (SelectedTile != null) BoardTiles[SelectedTile.Value.x, SelectedTile.Value.y].ToggleSelection(false);
                _selectedTile = value;
                if (value != null) BoardTiles[value.Value.x, value.Value.y].ToggleSelection(true);
                NotifyOfPropertyChange("BoardTilesList");
            }
        }
        public ICommand SelectTileCommand { get; private set; }

        public bool AllowMoves = true;
        private bool inputDebounce = false;

        //private string _debugMessage;
        //public string DebugMessage
        //{
        //    get { 
        //        return _debugMessage;
        //    }
        //    set { 
        //        _debugMessage = value;
        //        NotifyOfPropertyChange("DebugMessage");
        //    }
        //}

        private Queue<string> _logMessages;

        public Queue<string> LogMessages
        {
            get { return _logMessages; }
            set { _logMessages = value; }
        }

        // Events
        public event EventHandler<MoveResult> MovePlayed;
        public event EventHandler LogMessagesUpdated;


        public AnimalChessViewModel()
        {
            animalChessModel = new AnimalChessModel();
            BoardTiles = animalChessModel.GetTileModels();
            
            SelectedTile = null;
            SelectTileCommand = new DelegateCommand(SelectTile);

            LogMessages = new Queue<string>(10);
            AddLogMessage("Initialized.");
        }

        public void SelectTile(object pos)
        {
            if (!AllowMoves || inputDebounce || GameFinished) return; // May need to add checks for GameInProgress?
            inputDebounce = true;

            if (pos == null)
            {
                //throw new ArgumentNullException("pos");
                AddLogMessage("Invalid tile selection");
                inputDebounce = false;
                return;
            }

            // Handle tile selection logic (highlighting and stuff)
            (int x, int y) position = ((int, int))pos;

            // Check out of bounds origin pos
            if (position.x < 0 || position.x > BoardTiles.GetLength(0) ||
                position.y < 0 || position.y > BoardTiles.GetLength(1))
            {
                AddLogMessage("Invalid tile selection");
                inputDebounce = false;
                return;
            }

            // If no selection
            if (SelectedTile == null)
            {
                SelectedTile = position;
                AddLogMessage("Tile selected successfully");
            }
            // Else if selecting the already selected
            else if (SelectedTile == position)
            {
                ResetSelection();
            }
            else
            {
                PlayMove(((int, int))SelectedTile, position);
                // Get move for player 2
            }

            inputDebounce = false;
        }

        //public void SetSelectedTile((int x, int y)? pos)
        //{
        //    if (SelectedTile != null) BoardTiles[SelectedTile.Value.x, SelectedTile.Value.y].ToggleSelection(false);
        //    SelectedTile = pos;
        //    if (pos != null) BoardTiles[pos.Value.x, pos.Value.y].ToggleSelection(true);
        //    NotifyOfPropertyChange("BoardTilesList"); // Maybe unnecessary?
        //}

        public void ResetSelection()
        {
            SelectedTile = null;

            // Reset visual effects too -> DONE AT PROPERTY

            AddLogMessage("Selection reset.");
        }

        public bool PlayMove((int, int) origin, (int, int) target)
        {
            if (GameFinished) return false;

            MoveResult result = animalChessModel.PlayMove(origin, target);
            AddLogMessage(result.ToString());

            if (result.Success)
            {
                BoardTiles = animalChessModel.GetTileModels();
                OnMovePlayed(result);
            }

            ResetSelection();
            CheckGameStatus();
            return result.Success;
        }

        public void CheckGameStatus()
        {
            if (animalChessModel.GameFinished)
            {
                AddLogMessage("Game ended.");

                if (animalChessModel.PlayerOneWon) AddLogMessage("Player one wins!");
                else AddLogMessage("Player two wins!");
            }
        }

        private void AddLogMessage(string message)
        {
            LogMessages.Enqueue(message);
            OnLogMessagesUpdated();
        }


        // Events
        protected virtual void OnMovePlayed(MoveResult moveResult)
        {
            if (MovePlayed != null)
            {
                MovePlayed(this, moveResult);
            }
        }

        protected virtual void OnLogMessagesUpdated()
        {
            if (LogMessagesUpdated != null)
            {
                LogMessagesUpdated(this, EventArgs.Empty);
            }
        }
    }
}
