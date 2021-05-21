using CompomentLib.Equalizer;
using Core.Entities;
using Core.Filters;
using Core.Handlers;
using Core.SampleChunkConverters;
using NAudio.Wave;
using System;

namespace ViewModel
{
    public class WaveChannel32 : WaveStream, ISampleNotifier, IFilterHandler
    {
        private WaveStream sourceStream;
        private readonly WaveFormat waveFormat;
        private readonly long length;
        private readonly int destBytesPerSample;
        private readonly int sourceBytesPerSample;
        private volatile float volume;
        private volatile float pan;
        private long position;
        private readonly ISampleChunkConverter sampleProvider;
        private readonly object lockObject = new object();

        public WaveChannel32(WaveStream sourceStream, float volume, float pan)
        {
            PadWithZeroes = true;

            var providers = new ISampleChunkConverter[]
            {
                new Mono8SampleChunkConverter(),
                new Stereo8SampleChunkConverter(),
                new Mono16SampleChunkConverter(),
                new Stereo16SampleChunkConverter(),
                new Mono24SampleChunkConverter(),
                new Stereo24SampleChunkConverter(),
                new MonoFloatSampleChunkConverter(),
                new StereoFloatSampleChunkConverter(),
            };
            foreach (var provider in providers)
            {
                if (provider.Supports(sourceStream.WaveFormat))
                {
                    this.sampleProvider = provider;
                    break;
                }
            }

            if (this.sampleProvider == null)
            {
                throw new ArgumentException("Unsupported sourceStream format");
            }

            // always outputs stereo 32 bit
            waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sourceStream.WaveFormat.SampleRate, 2);
            destBytesPerSample = 8; // includes stereo factoring

            this.sourceStream = sourceStream;
            this.volume = volume;
            this.pan = pan;
            sourceBytesPerSample = sourceStream.WaveFormat.Channels * sourceStream.WaveFormat.BitsPerSample / 8;

            length = SourceToDest(sourceStream.Length);
            position = 0;
        }

        private long SourceToDest(long sourceBytes)
        {
            return (sourceBytes / sourceBytesPerSample) * destBytesPerSample;
        }

        private long DestToSource(long destBytes)
        {
            return (destBytes / destBytesPerSample) * sourceBytesPerSample;
        }

        public WaveChannel32(WaveStream sourceStream, Equalizer equalizer)
            :
            this(sourceStream, 1.0f, 0.0f)
        {
            equalizer.RegisterFilterHandler(this);
            CreateFilters();
        }

        public WaveChannel32(WaveStream sourceStream)
            :
            this(sourceStream, 1.0f, 0.0f)
        {
        }

        public override int BlockAlign => (int)SourceToDest(sourceStream.BlockAlign);

        public override long Length => length;

        public override long Position
        {
            get
            {
                return position;
            }
            set
            {
                lock (lockObject)
                {
                    // make sure we don't get out of sync
                    value -= (value % BlockAlign);
                    if (value < 0)
                    {
                        sourceStream.Position = 0;
                    }
                    else
                    {
                        sourceStream.Position = DestToSource(value);
                    }
                    // source stream may not have accepted the reposition we gave it
                    position = SourceToDest(sourceStream.Position);
                }
            }
        }

        public override int Read(byte[] destBuffer, int offset, int numBytes)
        {
            lock (lockObject)
            {
                int bytesWritten = 0;
                WaveBuffer destWaveBuffer = new WaveBuffer(destBuffer);

                // 1. fill with silence
                if (position < 0)
                {
                    bytesWritten = (int)Math.Min(numBytes, 0 - position);
                    for (int n = 0; n < bytesWritten; n++)
                        destBuffer[n + offset] = 0;
                }
                if (bytesWritten < numBytes)
                {
                    sampleProvider.LoadNextChunk(sourceStream, (numBytes - bytesWritten) / 8);

                    int outIndex = (offset / 4) + bytesWritten / 4;
                    while (sampleProvider.GetNextSample(out float left, out float right) && bytesWritten < numBytes)
                    {
                        // implement better panning laws. 
                        left = (pan <= 0) ? left : (left * (1 - pan) / 2.0f);
                        right = (pan >= 0) ? right : (right * (pan + 1) / 2.0f);
                        left *= volume;
                        right *= volume;

                        if (Filters[0, 0] != null)
                        {
                            for (int band = 0; band < 7; band++)
                            {
                                left = Filters[0, band].Transform(left);
                            }

                            for (int band = 0; band < 7; band++)
                            {
                                right = Filters[1, band].Transform(right);
                            }
                        }

                        destWaveBuffer.FloatBuffer[outIndex++] = left ;
                        destWaveBuffer.FloatBuffer[outIndex++] = right;
                        bytesWritten += 8;
                        if (Sample != null) RaiseSample(left, right);
                    }
                }
                // 3. Fill out with zeroes
                if (PadWithZeroes && bytesWritten < numBytes)
                {
                    Array.Clear(destBuffer, offset + bytesWritten, numBytes - bytesWritten);
                    bytesWritten = numBytes;
                }
                position += bytesWritten;
                return bytesWritten;
            }
        }

        public BiQuadFilter[,] Filters { get; set; } = new BiQuadFilter[2, 7];
        public float[] Gains { get; set; }
        public void CreateFilters()
        {
            var bands = new EqualizerBand[]
            {
                        new EqualizerBand {Bandwidth = 0.8f, Frequency = 100, Gain = Gains[0]},
                        new EqualizerBand {Bandwidth = 0.8f, Frequency = 200, Gain = Gains[1]},
                        new EqualizerBand {Bandwidth = 0.8f, Frequency = 400, Gain = Gains[2]},
                        new EqualizerBand {Bandwidth = 0.8f, Frequency = 800, Gain = Gains[3]},
                        new EqualizerBand {Bandwidth = 0.8f, Frequency = 1200, Gain = Gains[4]},
                        new EqualizerBand {Bandwidth = 0.8f, Frequency = 2400, Gain = Gains[5]},
                        new EqualizerBand {Bandwidth = 0.8f, Frequency = 4800, Gain = Gains[6]},
            };

            for (int bandIndex = 0; bandIndex < bands.Length; bandIndex++)
            {
                var band = bands[bandIndex];
                for (int n = 0; n < 2; n++)
                {
                    if (Filters[n, bandIndex] == null)
                        Filters[n, bandIndex] = BiQuadFilter.PeakingEQ(WaveFormat.SampleRate, band.Frequency, band.Bandwidth, band.Gain);
                    else
                        Filters[n, bandIndex].SetPeakingEq(WaveFormat.SampleRate, band.Frequency, band.Bandwidth, band.Gain);
                }
            }
        }

        public bool PadWithZeroes { get; set; }

        public override WaveFormat WaveFormat => waveFormat;

        public float Volume
        {
            get { return volume; }
            set { volume = value; }
        }

        public float Pan
        {
            get { return pan; }
            set { pan = value; }
        }

        public override bool HasData(int count)
        {
            // Check whether the source stream has data.
            bool sourceHasData = sourceStream.HasData(count);

            if (sourceHasData)
            {
                if (position + count < 0)
                    return false;
                return (position < length) && (volume != 0);
            }
            return false;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (sourceStream != null)
                {
                    sourceStream.Dispose();
                    sourceStream = null;
                }
            }
            else
            {
                System.Diagnostics.Debug.Assert(false, "WaveChannel32 was not Disposed");
            }
            base.Dispose(disposing);
        }

        public event EventHandler<SampleEventArgs> Sample;

        private readonly SampleEventArgs sampleEventArgs = new SampleEventArgs(0, 0);

        private void RaiseSample(float left, float right)
        {
            sampleEventArgs.Left = left;
            sampleEventArgs.Right = right;
            Sample(this, sampleEventArgs);
        }
    }
}
