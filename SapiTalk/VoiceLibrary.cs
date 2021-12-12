﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Yomiage.SDK;
using Yomiage.SDK.Config;

namespace SapiTalk
{
    internal class VoiceLibrary : VoiceLibraryBase
    {
        private string exePath => Path.Combine(DllDirectory, "DnfSynthe.exe");

        private string voicesFile => Path.Combine(DllDirectory, "voiceNames.txt");
        private string voiceFile => Path.Combine(DllDirectory, "voiceName.txt");
        private string textFile => Path.Combine(DllDirectory, "text.txt");
        private string speedFile => Path.Combine(DllDirectory, "speed.txt");
        private string wavFile => Path.Combine(DllDirectory, "output.wav");

        public override void Initialize(string configDirectory, string dllDirectory, LibraryConfig config)
        {
            base.Initialize(configDirectory, dllDirectory, config);

            if (File.Exists(voicesFile)) { File.Delete(voicesFile); }
            if (File.Exists(voiceFile)) { File.Delete(voiceFile); }
            if (File.Exists(textFile)) { File.Delete(textFile); }
            if (File.Exists(speedFile)) { File.Delete(speedFile); }
            if (File.Exists(wavFile)) { File.Delete(wavFile); }

            Excute();

            if (File.Exists(voicesFile))
            {
                var lines = File.ReadAllLines(voicesFile).Where(x => !string.IsNullOrWhiteSpace(x));
                if(Settings.Strings?.TryGetSetting("voiceName", out var setting) == true)
                {
                    setting.ComboItems = lines.ToArray();
                }
            }
        }

        private void Excute()
        {
            if (!File.Exists(exePath))
            {
                return;
            }

            var processStartInfo = new ProcessStartInfo()
            {
                FileName = exePath,
                CreateNoWindow = true,
                UseShellExecute = false,
                WorkingDirectory = DllDirectory,
            };
            var process = Process.Start(processStartInfo);
            process.WaitForExit();
            process.Dispose();
        }
    }
}
