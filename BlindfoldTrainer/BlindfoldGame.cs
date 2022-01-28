using ilf.pgn;
using ilf.pgn.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BlindfoldTrainer
{
    public class BlindfoldGame
    {
        public bool UserNotifiedGameCompleted { get; set; } = false;
        public bool IncludeGameInSimul { get; set; } = false;
        public string UniqueIdentifier { get; private set; }
        public string PgnIdentifier { get; private set; }
        public string WhitePlayer => _pgnGame.WhitePlayer;
        public string BlackPlayer => _pgnGame.BlackPlayer;
        public string Tournament => _pgnGame.Event;
        public bool IsFirstMove => _currentMoveIndex == 0;
        public List<BlindfoldMove> Moves { get; private set; } = new List<BlindfoldMove>();
        

        private Game _pgnGame { get; set; }
        private int _currentMoveIndex { get; set; }
        
        public BlindfoldGame(Game g, string pgnIdentifier)
        {
            PgnIdentifier = pgnIdentifier;
            _pgnGame = g;
            _currentMoveIndex = 0;

            GetMovesFromPGN();
            UpdateUniqueIdentifier();
        }

        public void ResetGame()
        {
            _currentMoveIndex = 0;
            UserNotifiedGameCompleted = false;
        }

        public BlindfoldMove GetNextMove()
        {
            if(_currentMoveIndex < Moves.Count)
            {
                BlindfoldMove move = Moves.ElementAt(_currentMoveIndex);
                _currentMoveIndex++;
                return move;
            }
            return null;
        }

        public bool HasNextMove()
        {
            return _currentMoveIndex < Moves.Count;
        }

        private void GetMovesFromPGN()
        {
            foreach(Move move in _pgnGame.MoveText.GetMoves())
            {
                BlindfoldMove blindfoldMove = new BlindfoldMove(move);
                if(blindfoldMove != null)
                {
                    Moves.Add(blindfoldMove);
                }
            }
        }

        /// <summary>
        /// Gets unique identifier that represents a chess game. 
        /// </summary>
        /// <remarks>
        /// Games are differentiated based on the moves played
        /// Function must be called after all moves are obtained
        /// </remarks>
        private void UpdateUniqueIdentifier()
        {
            //Create a unique identifier consisting of the destination squares of every move
            string identifier = "";

            foreach (BlindfoldMove m in Moves)
            {
                if (m.TargetSquare != null)
                {
                    identifier += m.TargetSquare.File.ToString() + m.TargetSquare.Rank.ToString();
                }
            }

            //Hash to simplify storage
            //MD5 is fast and security is not a concern
            byte[] hashBytes = { };
            using (HashAlgorithm algorithm = MD5.Create())
            {
                hashBytes = algorithm.ComputeHash(Encoding.UTF8.GetBytes(identifier));
            }

            StringBuilder sb = new StringBuilder();
            foreach (byte b in hashBytes)
            {
                sb.Append(b.ToString("X2"));
            }

            UniqueIdentifier = sb.ToString();
        }
    }
}
