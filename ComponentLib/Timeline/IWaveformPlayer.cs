using System;
using System.ComponentModel;

namespace CompomentLib.Timeline
{
    public interface IWaveformPlayer : INotifyPropertyChanged
    {
        double ChannelPosition { get; set; }

        // total channel length in seconds.
        double ChannelLength { get; }

        // Gets the raw level data for the waveform.
        // Level data should be structured in an array where each sucessive index
        // alternates between left or right channel data, starting with left. Index 0
        // should be the first left level, index 1 should be the first right level, index
        // 2 should be the second left level, etc.
        float[] WaveformData { get; }

        // starting time for a section of repeat/looped audio
        TimeSpan SelectionBegin { get; set; }

        // ending time for a section of repeat/looped audio
        TimeSpan SelectionEnd { get; set; }

        bool IsPlaying { get; }
    }
}
