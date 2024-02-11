using AnimalChessLibrary;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace WpfCMExperiments.ViewModels
{
    public class SinglePlayerViewModel : Conductor<object>
    {
        internal AnimalChessViewModel boardVM;
        public BindableCollection<string> LogMessagesList => new BindableCollection<string>(boardVM.LogMessages);
        private Stack<string> moveTracker = new Stack<string>();
        public BindableCollection<string> MoveTrackerList => new BindableCollection<string>(moveTracker); // move both to animalchessviewmodel?

        public SinglePlayerViewModel()
        {
            ActivateItemAsync(IoC.Get<AnimalChessViewModel>(), new CancellationToken());
            boardVM = (AnimalChessViewModel)ActiveItem;
            boardVM.MovePlayed += OnMovePlayed;
            boardVM.LogMessagesUpdated += OnLogMessagesUpdated;

            //boardVM.Refresh(); // Makes the properties of this child element to update
            //MessageBox.Show(boardVM.BoardColumnAmount.ToString());
        }

        public virtual void OnMovePlayed(object? sender, MoveResult result)
        {
            if (!result.Success) return;

            moveTracker.Push(result.GetMoveShortDescription());
            NotifyOfPropertyChange("MoveTrackerList");

            if (boardVM.GameFinished) GameEndedHandler();
        }

        public virtual void OnLogMessagesUpdated(object? sender, EventArgs e)
        {
            NotifyOfPropertyChange("LogMessagesList");
        }

        public virtual void GameEndedHandler()
        {
            //throw new NotImplementedException();
            MessageBox.Show($"Game ended. Winner is {(boardVM.PlayerOneWon ? "Player 1" : "Player 2")}!");
        }
    }
}
