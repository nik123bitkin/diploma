﻿using NAudio.Utils;
using NAudio.Wave;

namespace Core.SampleChunkConverters
{
    public class Stereo24SampleChunkConverter : ISampleChunkConverter
    {
        private int offset;
        private byte[] sourceBuffer;
        private int sourceBytes;

        public bool Supports(WaveFormat waveFormat)
        {
            return waveFormat.Encoding == WaveFormatEncoding.Pcm &&
                waveFormat.BitsPerSample == 24 &&
                waveFormat.Channels == 2;
        }


        public void LoadNextChunk(IWaveProvider source, int samplePairsRequired)
        {
            int sourceBytesRequired = samplePairsRequired * 6;
            sourceBuffer = BufferHelpers.Ensure(sourceBuffer, sourceBytesRequired);
            sourceBytes = source.Read(sourceBuffer, 0, sourceBytesRequired);
            offset = 0;
        }

        public bool GetNextSample(out float sampleLeft, out float sampleRight)
        {
            if (offset < sourceBytes)
            {
                sampleLeft = (((sbyte)sourceBuffer[offset + 2] << 16) | (sourceBuffer[offset + 1] << 8) | sourceBuffer[offset]) / 8388608f;
                offset += 3;
                sampleRight = (((sbyte)sourceBuffer[offset + 2] << 16) | (sourceBuffer[offset + 1] << 8) | sourceBuffer[offset]) / 8388608f;
                offset += 3;
                return true;
            }
            else
            {
                sampleLeft = 0.0f;
                sampleRight = 0.0f;
                return false;
            }
        }

    }
}
