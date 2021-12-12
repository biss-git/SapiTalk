using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DnfSynthe
{
    internal class Program
    {
        private static readonly string voicesFile = "voiceNames.txt";
        private static readonly string voiceFile = "voiceName.txt";
        private static readonly string textFile = "text.txt";
        private static readonly string speedFile = "speed.txt";
        private static readonly string wavFile = "output.wav";

        static void Main(string[] args)
        {
            var synthe = new SpeechSynthesizer();

            {
                // 見つけたライブラリを列挙する
                var voiceNames = string.Empty;
                foreach (var voice in synthe.GetInstalledVoices())
                {
                    voiceNames += voice.VoiceInfo.Name + Environment.NewLine;
                }
                File.WriteAllText(voicesFile, voiceNames);
            }

            {
                // 音声の指定を受け取る
                var voiceName = string.Empty;
                if (File.Exists(voiceFile))
                {
                    voiceName = File.ReadAllLines(voiceFile).FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));
                }

                if (!string.IsNullOrWhiteSpace(voiceName))
                {
                    // 音声を設定する
                    try
                    {
                        synthe.SelectVoice(voiceName);
                    }
                    catch (Exception)
                    {
                        // 名前が不正な場合はそこで終わり
                        // 文字列が間違えている場合もあるが、音声は見つかるのに設定ができない場合もある
                        return;
                    }
                }
            }

            {
                // 話速を設定する。
                if (File.Exists(speedFile))
                {
                    var speedText = File.ReadAllLines(speedFile).FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));
                    if(int.TryParse(speedText, out var speed))
                    {
                        synthe.Rate = Math.Max(-10, Math.Min(speed, 10));
                    }
                }
            }

            {
                // 読み上げるテキストを受け取る
                var text = string.Empty;
                if (File.Exists(textFile))
                {
                    text = File.ReadAllText(textFile);
                }

                // 音声を保存する。
                if (!string.IsNullOrWhiteSpace(text))
                {
                    synthe.SetOutputToWaveFile(wavFile);
                    Thread.Sleep(100);
                    synthe.Speak(text);
                    Thread.Sleep(100);
                }
            }
            synthe.Dispose();
        }
    }
}
