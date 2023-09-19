using NAudio.Dsp;
using System;

namespace Music.NAudio.WaveformGenerator
{
    public sealed class SampleAggregator
    {
        #region Private Members

        private float _volumeLeftMaxValue;
        private float _volumeLeftMinValue;
        private float _volumeRightMaxValue;
        private float _volumeRightMinValue;
        private int _channelDataPosition;
        private readonly int _bufferSize;
        private readonly int _binaryExponentitation;
        private readonly Complex[] _channelData;

        #endregion Private Members

        #region Properties

        public float LeftMaxVolume => _volumeLeftMaxValue;
        public float LeftMinVolume => _volumeLeftMinValue;
        public float RightMaxVolume => _volumeRightMaxValue;
        public float RightMinVolume => _volumeRightMinValue;

        #endregion Properties

        public SampleAggregator(int bufferSize)
        {
            _bufferSize = bufferSize;
            _binaryExponentitation = (int)Math.Log(bufferSize, 2);
            _channelData = new Complex[bufferSize];
        }

        #region Public Methods

        public void Clear()
        {
            _volumeLeftMaxValue = float.MinValue;
            _volumeRightMaxValue = float.MinValue;
            _volumeLeftMinValue = float.MaxValue;
            _volumeRightMinValue = float.MaxValue;
            _channelDataPosition = 0;
        }

        /// <summary>
        /// Add a sample value to the aggregator.
        /// </summary>
        /// <param name="value">The value of the sample.</param>
        public void AddSample(float leftValue, float rightValue)
        {
            if (_channelDataPosition == 0)
            {
                _volumeLeftMaxValue = float.MinValue;
                _volumeRightMaxValue = float.MinValue;
                _volumeLeftMinValue = float.MaxValue;
                _volumeRightMinValue = float.MaxValue;
            }

            // Make stored channel data stereo by averaging left and right values.
            _channelData[_channelDataPosition].X = (leftValue + rightValue) / 2.0f;
            _channelData[_channelDataPosition].Y = 0;
            _channelDataPosition++;

            _volumeLeftMaxValue = Math.Max(_volumeLeftMaxValue, leftValue);
            _volumeLeftMinValue = Math.Min(_volumeLeftMinValue, leftValue);
            _volumeRightMaxValue = Math.Max(_volumeRightMaxValue, rightValue);
            _volumeRightMinValue = Math.Min(_volumeRightMinValue, rightValue);

            if (_channelDataPosition >= _channelData.Length)
            {
                _channelDataPosition = 0;
            }
        }

        /// <summary>
        /// Performs an FFT calculation on the channel data upon request.
        /// </summary>
        /// <param name="fftBuffer">A buffer where the FFT data will be stored.</param>
        public void GetFFTResults(float[] fftBuffer)
        {
            var channelDataClone = new Complex[_bufferSize];
            _channelData.CopyTo(channelDataClone, 0);
            FastFourierTransform.FFT(true, _binaryExponentitation, channelDataClone);
            for (int i = 0; i < channelDataClone.Length / 2; i++)
            {
                // Calculate actual intensities for the FFT results.
                fftBuffer[i] = (float)Math.Sqrt(channelDataClone[i].X * channelDataClone[i].X + channelDataClone[i].Y * channelDataClone[i].Y);
            }
        }

        #endregion Public Methods
    }
}
