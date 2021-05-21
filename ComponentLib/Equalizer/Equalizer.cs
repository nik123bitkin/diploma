using Core.Handlers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace CompomentLib.Equalizer
{
    [DisplayName("Equalizer")]
    [Description("Displays and edits banded frequency amplification.")]
    [ToolboxItem(true)]
    [TemplatePart(Name = "PART_EqualizerGrid", Type = typeof(Grid))]  
    public class Equalizer : Control
    {
        #region Fields
        private readonly List<Slider> sliders = new List<Slider>();
        private Grid equalizerGrid;
        private IFilterHandler _filterHandler;
        #endregion

        #region DependencyProperties
        #region EqualizerValues
        public static readonly DependencyProperty EqualizerValuesProperty = DependencyProperty.Register("EqualizerValues", typeof(float[]), typeof(Equalizer), 
            new UIPropertyMetadata(new float[] { 0f, 0f, 0f, 0f, 0f, 0f, 0f }, OnEqualizerValuesChanged, OnCoerceEqualizerValues));

        private static object OnCoerceEqualizerValues(DependencyObject o, object value)
        {
            return o is Equalizer equalizer ? equalizer.OnCoerceEqualizerValues((float[])value) : value;
        }

        private static void OnEqualizerValuesChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (o is Equalizer equalizer)
            {
                equalizer.OnEqualizerValuesChanged((float[])e.OldValue, (float[])e.NewValue);
            }
        }

        protected virtual float[] OnCoerceEqualizerValues(float[] value)
        {
            return value == null || value.Length != NumberOfBands ? (new float[NumberOfBands]) : value;
        }

        protected virtual void OnEqualizerValuesChanged(float[] oldValue, float[] newValue)
        {
            if (newValue == null || newValue.Length != NumberOfBands)
                SetEqualizerValues(new float[NumberOfBands]);
            else
                SetEqualizerValues(newValue);
        }

        public float[] EqualizerValues
        {
            get
            {
                return (float[])GetValue(EqualizerValuesProperty);
            }
            set
            {
                SetValue(EqualizerValuesProperty, value);
            }
        }
        #endregion

        #region NumberOfBands

        public static readonly DependencyProperty NumberOfBandsProperty = 
            DependencyProperty.Register("NumberOfBands", typeof(int), typeof(Equalizer), 
                new UIPropertyMetadata(7, OnNumberOfBandsChanged, OnCoerceNumberOfBands));

        private static object OnCoerceNumberOfBands(DependencyObject o, object value)
        {
            return o is Equalizer equalizer ? equalizer.OnCoerceNumberOfBands((int)value) : value;
        }

        private static void OnNumberOfBandsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (o is Equalizer equalizer)
            {
                equalizer.OnNumberOfBandsChanged((int)e.OldValue, (int)e.NewValue);
            }
        }

        protected virtual int OnCoerceNumberOfBands(int value)
        {
            value = Math.Max(1, value);
            return value;
        }

        protected virtual void OnNumberOfBandsChanged(int oldValue, int newValue)
        {
            CreateSliders();
            SetEqualizerValues(new float[NumberOfBands]);
        }

        public int NumberOfBands
        {
            get
            {
                return (int)GetValue(NumberOfBandsProperty);
            }
            set
            {
                SetValue(NumberOfBandsProperty, value);
            }
        }

        #endregion
        #endregion

        #region Constructors

        public Equalizer()
        {
            CreateSliders();
        }

        static Equalizer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Equalizer), new FrameworkPropertyMetadata(typeof(Equalizer)));
        }
        #endregion

        #region Template Overrides

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (equalizerGrid != null)
                equalizerGrid.Children.Clear();

            equalizerGrid = GetTemplateChild("PART_EqualizerGrid") as Grid;

            CreateSliders();
        }

        protected override void OnTemplateChanged(ControlTemplate oldTemplate, ControlTemplate newTemplate)
        {
            base.OnTemplateChanged(oldTemplate, newTemplate);
        }
        #endregion

        #region Private Utility Methods
        private void CreateSliders()
        {
            if (equalizerGrid == null)
                return;

            ClearSliders();
            equalizerGrid.ColumnDefinitions.Clear();
            for (int i = 0; i < NumberOfBands; i++)
            {
                ColumnDefinition channelColumn = new ColumnDefinition()
                {
                    Width = new GridLength(10.0d, GridUnitType.Star)
                };
                equalizerGrid.ColumnDefinitions.Add(channelColumn);
                Slider slider = new Slider()
                {
                    Style = (Style)FindResource("EqualizerSlider"),
                    Maximum = 7.0,
                    Minimum = -7.0,
                    SmallChange = 0.1,
                    LargeChange = 0.5,
                };

                Grid.SetColumn(slider, i);
                sliders.Add(slider);
                equalizerGrid.Children.Add(slider);
                slider.ValueChanged += Slider_ValueChanged;
            }
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            EqualizerValues = GetEqualizerValues();
        }

        private void ClearSliders()
        {
            foreach (Slider slider in sliders)
            {
                slider.ValueChanged -= Slider_ValueChanged;
                equalizerGrid.Children.Remove(slider);
            }
            sliders.Clear();
        }

        private float[] GetEqualizerValues()
        {
            float[] sliderValues = new float[NumberOfBands];
            for (int i = 0; i < NumberOfBands; i++)
                sliderValues[i] = (float)sliders[i].Value;
            if (_filterHandler != null)
            {
                _filterHandler.Gains = sliderValues;
                _filterHandler.CreateFilters();
            }
            return sliderValues;
        }

        private void SetEqualizerValues(float[] values)
        {
            for (int i = 0; i < NumberOfBands; i++)
                sliders[i].Value = values[i];
        }
        #endregion

        public void RegisterFilterHandler(IFilterHandler filterHandler)
        {
            _filterHandler = filterHandler;
            GetEqualizerValues();
        }
    }
}
