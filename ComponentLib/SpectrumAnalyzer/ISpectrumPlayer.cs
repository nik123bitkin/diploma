using System.ComponentModel;

namespace CompomentLib.SpectrumAnalyzer
{
    public interface ISpectrumPlayer : INotifyPropertyChanged
    {
        bool GetFFTData(float[] fftDataBuffer);

        int GetFFTFrequencyIndex(int frequency);

        bool IsPlaying { get; }
    }
}
