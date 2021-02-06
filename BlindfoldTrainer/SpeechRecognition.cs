using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Recognition;
using System.Speech.Synthesis;

namespace BlindfoldTrainer
{
    public class SpeechRecognition
    {
        private SpeechRecognizer _speechRecognizer = new SpeechRecognizer();
        private const string ACKNOWLEDGE = "Accept the move";
        private object _acknowledgePhraseLock = new object();
        public bool AcknowledgePhraseRecognized { get; private set; } = false;

        public SpeechRecognition()
        {
            DisableSpeechRecognition();
            AddPhrases();
            _speechRecognizer.SpeechRecognized += sr_SpeechRecognized;
            
        }

        public void EnableSpeechReconition()
        {
            _speechRecognizer.Enabled = true;
        }
        public void DisableSpeechRecognition()
        {
            _speechRecognizer.Enabled = false;
        }

        /// <summary>
        /// Accepts that the acknowledge phrase has been recognized, and disables speech recognition
        /// </summary>
        /// <remarks>This is a hack. Fix later.</remarks>
        public void AcceptAcknowledgeRecognized()
        {
            lock (_acknowledgePhraseLock)
            {
                AcknowledgePhraseRecognized = false;
                DisableSpeechRecognition();
            }
        }

        private void AddPhrases()
        {
            _speechRecognizer.LoadGrammar(new Grammar(new GrammarBuilder(ACKNOWLEDGE)));
        }
        private void sr_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Text.Equals(ACKNOWLEDGE))
            {
                lock (_acknowledgePhraseLock)
                {
                    if (_speechRecognizer.Enabled)
                    {
                        AcknowledgePhraseRecognized = true;
                    }
                }
            }
        }
    }
}
