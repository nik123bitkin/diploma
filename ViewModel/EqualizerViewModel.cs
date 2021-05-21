using Core.Entities;
using Core.Handlers;
using NAudio.Wave;
using System.ComponentModel;

namespace ViewModel
{
    public class EqualizerViewModel : ViewModelBase
    {
        private EqualizerHandler _equalizerHandler;

        private readonly EqualizerBand[] _bands;

        public EqualizerViewModel(float[] gains)
        {
            _bands = new EqualizerBand[]
            {
                        new EqualizerBand {Bandwidth = 0.8f, Frequency = 100, Gain = gains[0]},
                        new EqualizerBand {Bandwidth = 0.8f, Frequency = 200, Gain = gains[1]},
                        new EqualizerBand {Bandwidth = 0.8f, Frequency = 400, Gain = gains[2]},
                        new EqualizerBand {Bandwidth = 0.8f, Frequency = 800, Gain = gains[3]},
                        new EqualizerBand {Bandwidth = 0.8f, Frequency = 1200, Gain = gains[4]},
                        new EqualizerBand {Bandwidth = 0.8f, Frequency = 2400, Gain = gains[5]},
                        new EqualizerBand {Bandwidth = 0.8f, Frequency = 4800, Gain = gains[6]}
            };

            PropertyChanged += OnPropertyChanged;
        }

        public void UpdateViewModel(ISampleProvider provider)
        {
            _equalizerHandler = new EqualizerHandler(provider, _bands);            
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            _equalizerHandler?.Update();
        }
    }
}
