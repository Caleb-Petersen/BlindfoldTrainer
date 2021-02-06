using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlindfoldTrainer
{
    public class BlindfoldSimul
    {
        private object _cancelSimulLock = new object();
        private object _acceptAcknowledgeLock = new object();
        private bool _isSimulCancelled = false;
        private bool AcknowledgeAccepted { get; set; } = false;
        private bool WaitingForAcknowledge { get; set; } = false;
        private List<BlindfoldGame> SimulGames { get; set; }
        private SpeechRecognition speechRecognition = null;

        public bool IsSimualCancelled
        {
            get => _isSimulCancelled;
            private set
            {
                lock (_cancelSimulLock)
                {
                    _isSimulCancelled = value;
                }
            }
        }
        

        public BlindfoldSimul(List<BlindfoldGame> games)
        {
            SimulGames = games;

            if (Configs.UseSpeechRecognizer)
            {
                speechRecognition = new SpeechRecognition();
            }
        }

        public void StartSimul()
        {
            //TODO: Improve threading technique. Add cancellation token instead of bool cancel
            Task.Run(() =>
            {
                bool isResponsePlayed = true;

                while (isResponsePlayed && !IsSimualCancelled)
                {
                    isResponsePlayed = false;

                    for (int i = 0; i < SimulGames.Count; i++)
                    {
                        if (!IsSimualCancelled)
                        {
                            int gameNum = (i + 1);
                            BlindfoldGame g = SimulGames.ElementAt(i);

                            if (g.IsFirstMove && i % 2 == 0)
                            {
                                //Play white in even games. Note that the first move for white is different stylistically
                                BlindfoldMove move = g.GetNextMove();
                                if (move != null)
                                {
                                    isResponsePlayed = true;
                                    
                                    string msg = "Game " + gameNum.ToString() + " playing white. Your first move is " + move.SpeechText;
                                    Speech.Speak(msg);
                                    WaitForAcknowledgeOrCancel();
                                }
                            }
                            else
                            {
                                BlindfoldMove opponentsMove = g.GetNextMove();
                                if (opponentsMove != null)
                                {
                                    string msg = "Game " + gameNum.ToString() + ". Opponent plays " + opponentsMove.SpeechText;
                                    Speech.Speak(msg);
                                    WaitForAcknowledgeOrCancel();
                                    BlindfoldMove myMove = g.GetNextMove();

                                    if (myMove != null)
                                    {
                                        isResponsePlayed = true;
                                        msg = "Game " + gameNum.ToString() + ". You play " + myMove.SpeechText;
                                        Speech.Speak(msg);
                                        WaitForAcknowledgeOrCancel();
                                    }
                                }
                            }
                        }
                    }
                }

                Console.WriteLine("Completed and ready to Reset");
                //Reset everything once completed
                foreach(BlindfoldGame game in SimulGames)
                {
                    game.ResetGame();
                }
            });
        }

        public void StopSimul()
        {
            IsSimualCancelled = true;
            if (Configs.UseSpeechRecognizer)
            {
                speechRecognition.DisableSpeechRecognition();
            }
        }

        private void WaitForAcknowledgeOrCancel()
        {
            if (Configs.UseSpeechRecognizer)
            {
                Console.WriteLine("Waiting for an Acknowledge");
                speechRecognition.EnableSpeechReconition();

                while (!speechRecognition.AcknowledgePhraseRecognized && !IsSimualCancelled)
                {
                    Thread.Sleep(10);
                }
                Console.WriteLine("Received and Acknowledge");
                speechRecognition.AcceptAcknowledgeRecognized();
            }
            else
            {
                lock (_acceptAcknowledgeLock)
                {
                    AcknowledgeAccepted = false;
                    WaitingForAcknowledge = true;
                }
                
                while (!IsSimualCancelled)
                {
                    Thread.Sleep(10);
                    lock (_acceptAcknowledgeLock)
                    {
                        if (AcknowledgeAccepted)
                        {
                            WaitingForAcknowledge = false;
                            return;
                        }
                    }
                    
                }
                
            }
        }

        public void AcknowledgeMove()
        {
            lock (_acceptAcknowledgeLock)
            {
                if (WaitingForAcknowledge)
                {
                    AcknowledgeAccepted = true;
                }
            }
        }
    }
}
