using ilf.pgn;
using ilf.pgn.Data;
using ilf.pgn.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BlindfoldTrainer
{
    public class BlindfoldTrainerVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<BlindfoldGame> Games { get; private set; } = new ObservableCollection<BlindfoldGame>();
        private BlindfoldSimul _blindfoldSimul = null;

        private bool _simulIsRunning = false;
        public bool SimulIsRunning
        {
            get => _simulIsRunning;
            set { _simulIsRunning = value; NotifyPropertyChanged(); }
        }

        public void StartSimul()
        {
            SimulIsRunning = true;
            List<BlindfoldGame> simulGames = new List<BlindfoldGame>();
            foreach(BlindfoldGame game in Games)
            {
                if (game.IncludeGameInSimul)
                {
                    game.ResetGame();
                    simulGames.Add(game);
                }
            }

            _blindfoldSimul = new BlindfoldSimul(simulGames);
            _blindfoldSimul.StartSimul();
        }
        public void StopSimul()
        {
            SimulIsRunning = false;
            if (_blindfoldSimul != null)
            {
                _blindfoldSimul.StopSimul();
                _blindfoldSimul = null;
            }
        }
        public void AcknowledgeSimulMove()
        {
            if (SimulIsRunning && _blindfoldSimul != null)
            {
                _blindfoldSimul.AcknowledgeMove();
            }
            
        }
        public bool AddPgn(string pgnPath, string pgnIdentifier, out string msg )
        {
            msg = "";
            if (System.IO.File.Exists(pgnPath))
            {
                PgnReader reader = new PgnReader();

                try
                {
                    List<Game> games = reader.ReadFromFile(pgnPath).Games;

                    foreach(Game game in games)
                    {
                        BlindfoldGame blindfoldGame = new BlindfoldGame(game, pgnIdentifier);

                        if(Games.Where(_ => _.UniqueIdentifier.Equals(blindfoldGame.UniqueIdentifier)).ToList().Count == 0)
                        {
                            Games.Add(blindfoldGame);
                        }
                    }
                    NotifyPropertyChanged("Games");
                    return true;
                }
                catch (IOException ex)
                {
                    msg = ex.Message;
                }
                catch(PgnFormatException ex)
                {
                    msg = ex.Message;
                }
            }
            else
            {
                msg = "File does not exist.";
            }
            return false;
        }

        public bool RemovePgn(string pgnIdentifier)
        {
            List<BlindfoldGame> removedItems = new List<BlindfoldGame>();

            foreach(BlindfoldGame g in Games)
            {
                if (g.PgnIdentifier.Equals(pgnIdentifier))
                {
                    removedItems.Add(g);
                }
            }

            int numRemoved = removedItems.Count;
            foreach(BlindfoldGame g in removedItems)
            {
                Games.Remove(g);
            }
            NotifyPropertyChanged("Games");
            return numRemoved > 0;
        }

        public void ClearGames()
        {
            Games.Clear();
            NotifyPropertyChanged("Games");
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
