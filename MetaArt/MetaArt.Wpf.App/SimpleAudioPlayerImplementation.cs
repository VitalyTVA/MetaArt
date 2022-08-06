using System;
using System.IO;
using System.Linq;
using System.Windows.Media;

namespace MetaArt.Wpf {
#nullable disable
    //TODO rewrite
    public class SimpleAudioPlayerImplementation  {
        event EventHandler PlaybackEnded;

        static int index;

        MediaPlayer player;

        public bool IsPlaying {
            get {
                if(player == null)
                    return false;
                return isPlaying;
            }
        }
        bool isPlaying;
        public bool Loop { get; set; }

        public bool CanSeek => player != null;

        public bool Load(Stream audioStream) {
            DeletePlayer();

            player = GetPlayer();

            if(player != null) {
                var fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), $"{++index}.wav");
                using(var fileStream = File.OpenWrite(fileName)) audioStream.CopyTo(fileStream);

                player.Open(new Uri(fileName));
                player.MediaEnded += OnPlaybackEnded;
            }

            return player != null && player.Source != null;
        }

        public bool Load(string fileName) {
            DeletePlayer();

            player = GetPlayer();

            if(player != null) {
                player.Open(new Uri(@"Assets\" + fileName, UriKind.Relative));
                player.MediaEnded += OnPlaybackEnded;
            }

            return player != null && player.Source != null;
        }

        void DeletePlayer() {
            Stop();

            if(player != null) {
                player.MediaEnded -= OnPlaybackEnded;
                player = null;
            }
        }

        void OnPlaybackEnded(object sender, EventArgs args) {
            if(isPlaying && Loop) {
                Play();
            }

            PlaybackEnded?.Invoke(sender, args);
        }

        public void Play() {
            if(player == null || player.Source == null)
                return;

            if(IsPlaying) {
                Pause();
                Seek(0);
            }

            isPlaying = true;
            player.Play();
        }

        public void Pause() {
            isPlaying = false;
            player?.Pause();
        }

        public void Stop() {
            Pause();
            Seek(0);
            PlaybackEnded?.Invoke(this, EventArgs.Empty);
        }

        public void Seek(double position) {
            if(player == null) return;
            player.Position = TimeSpan.FromSeconds(position);
        }

        static MediaPlayer GetPlayer() {
            return new MediaPlayer();
        }

        bool isDisposed;

        protected virtual void Dispose(bool disposing) {
            if(isDisposed || player == null)
                return;

            if(disposing)
                DeletePlayer();

            isDisposed = true;
        }

        ~SimpleAudioPlayerImplementation() {
            Dispose(false);
        }

        public void Dispose() {
            Dispose(true);

            GC.SuppressFinalize(this);
        }
    }
}

