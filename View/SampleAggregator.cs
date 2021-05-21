//using System;
//using NAudio.Dsp;

//namespace View
//{
//    public class SampleAggregator
//    {
//        private float volumeLeftMaxValue;
//        private float volumeLeftMinValue;
//        private float volumeRightMaxValue;
//        private float volumeRightMinValue;
//        private readonly Complex[] channelData;
//        private readonly int bufferSize;
//        private readonly int binaryExponentitation;
//        private int channelDataPosition;

//        public SampleAggregator(int bufferSize)
//        {
//            this.bufferSize = bufferSize;
//            binaryExponentitation = (int)Math.Log(bufferSize, 2);
//            channelData = new Complex[bufferSize];
//        }

//        public void Clear()
//        {
//            volumeLeftMaxValue = float.MinValue;
//            volumeRightMaxValue = float.MinValue;
//            volumeLeftMinValue = float.MaxValue;
//            volumeRightMinValue = float.MaxValue;
//            channelDataPosition = 0;
//        }
             
//        public void Add(float leftValue, float rightValue)
//        {            
//            if (channelDataPosition == 0)
//            {
//                volumeLeftMaxValue = float.MinValue;
//                volumeRightMaxValue = float.MinValue;
//                volumeLeftMinValue = float.MaxValue;
//                volumeRightMinValue = float.MaxValue;
//            }

//            // Make stored channel data stereo by averaging left and right values.
//            channelData[channelDataPosition].X = (leftValue + rightValue) / 2.0f;
//            channelData[channelDataPosition].Y = 0;
//            channelDataPosition++;            

//            volumeLeftMaxValue = Math.Max(volumeLeftMaxValue, leftValue);
//            volumeLeftMinValue = Math.Min(volumeLeftMinValue, leftValue);
//            volumeRightMaxValue = Math.Max(volumeRightMaxValue, rightValue);
//            volumeRightMinValue = Math.Min(volumeRightMinValue, rightValue);

//            if (channelDataPosition >= channelData.Length)
//            {
//                channelDataPosition = 0;
//            }
//        }

//        public void GetFFTResults(float[] fftBuffer)
//        {
//            Complex[] channelDataClone = new Complex[bufferSize];
//            channelData.CopyTo(channelDataClone, 0);
//            FastFourierTransform.FFT(true, binaryExponentitation, channelDataClone);
//            for (int i = 0; i < channelDataClone.Length / 2; i++)
//            {
//                fftBuffer[i] = (float)Math.Sqrt(channelDataClone[i].X * channelDataClone[i].X + channelDataClone[i].Y * channelDataClone[i].Y);
//            }
//        }

//        public float LeftMaxVolume
//        {
//            get { return volumeLeftMaxValue; }
//        }

//        public float LeftMinVolume
//        {
//            get { return volumeLeftMinValue; }
//        }

//        public float RightMaxVolume
//        {
//            get { return volumeRightMaxValue; }
//        }

//        public float RightMinVolume
//        {
//            get { return volumeRightMinValue; }
//        }        
//    }
//}
