using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Synthesis;

namespace BlindfoldTrainer
{
    public static class Speech
    {
        
        /// <summary>
        /// Speaks the user defined message
        /// </summary>
        /// <param name="move"></param>
        public static void Speak(string message)
        {
            using (SpeechSynthesizer synth = new SpeechSynthesizer())
            {
                synth.SetOutputToDefaultAudioDevice();
                synth.Speak(message);
            }
        }
    }
}
