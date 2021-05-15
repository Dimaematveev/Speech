using NAudioWpfDemo.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using NAudio.FileFormats;
using Microsoft.Win32;

namespace Prob_CMD
{
    class Class1
    {
        
        //private readonly AudioPlayback audioPlayback;
        public ICommand OpenFileCommand { get; }
        public ICommand PlayCommand { get; }
        public ICommand PauseCommand { get; }
        public ICommand StopCommand { get; }
        private string selectedFile;


        public Class1()
        {
           
            PlayCommand = new DelegateCommand(Play);
            OpenFileCommand = new DelegateCommand(OpenFile);
            StopCommand = new DelegateCommand(Stop);
            PauseCommand = new DelegateCommand(Pause);
        }

        private void Pause()
        {
            //audioPlayback.Pause();
        }

        private void OpenFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "All Supported Files (*.wav;*.mp3)|*.wav;*.mp3|All Files (*.*)|*.*";
            bool? result = openFileDialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                this.selectedFile = openFileDialog.FileName;
                //audioPlayback.Load(this.selectedFile);
            }
        }

        private void Play()
        {
            if (this.selectedFile == null)
            {
                OpenFile();
            }
            if (this.selectedFile != null)
            {
                //audioPlayback.Play();
            }
        }

        private void Stop()
        {
            //audioPlayback.Stop();
        }

    }
}
