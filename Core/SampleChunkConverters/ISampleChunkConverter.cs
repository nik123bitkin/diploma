using NAudio.Wave;

namespace Core.SampleChunkConverters
{
    public interface ISampleChunkConverter
    {
        bool Supports(WaveFormat format);
        void LoadNextChunk(IWaveProvider sourceProvider, int samplePairsRequired);
        bool GetNextSample(out float sampleLeft, out float sampleRight);
    }
}
