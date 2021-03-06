﻿namespace MusCat.Core.Interfaces.Audio
{
    public interface IAudioPlayer
    {
        void Play(string location);
        void Stop(bool manualStop = true);
        void Pause();
        void Resume();
        void Seek(double percent);
        double PlayedTime { get; }
        double PlayedTimePercent { get; }

        void SetVolume(float volume);

        PlaybackState SongPlaybackState { get; set; }
        bool IsStopped { get; }
        bool IsStoppedManually { get; set; }
        
        void Close();
    }
}
