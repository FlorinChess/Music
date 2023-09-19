using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace Music.NAudio.WaveformGenerator
{
    public sealed class WaveformGenerator : IWaveformGenerator
    {
        public event EventHandler<WaveformDataEventArgs> WaveformDataUpdated;

        #region Private Members

        private bool _waveformComplete = false;
        private float[] _waveformData;
        private string _filePath = string.Empty;
        private SampleAggregator _waveformAggregator;
        private readonly BackgroundWorker _worker = new() { WorkerSupportsCancellation = true };

        #endregion Private Members

        public float[] WaveformData
        {
            get => _waveformData;
            set
            {
                _waveformData = value;
                WaveformDataUpdated?.Invoke(this, new WaveformDataEventArgs(WaveformData));
            }
        }

        public WaveformGenerator()
        {
            _worker.DoWork += OnGenerateData;
            _worker.RunWorkerCompleted += OnGenerateDataComplete;
        }

        #region Private Methods

        private void OnGenerateDataComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
                if (!_worker.IsBusy && WaveformData.Length != 0)
                    _worker.RunWorkerAsync(new WaveformDataEventArgs(WaveformData));
        }

        private void OnWaveInputStreamSample(object sender, SampleEventArgs e)
        {
            _waveformAggregator.AddSample(e.Left, e.Right);
        }

        private void OnGenerateData(object sender, DoWorkEventArgs e)
        {
            _waveformComplete = false;

            int numberOfPoints = 1500;

            using var waveformMp3Stream = new Mp3FileReader(_filePath);
            using var waveformInputStream = new WaveChannel32(waveformMp3Stream);

            waveformInputStream.Sample += OnWaveInputStreamSample;

            int frameLength = 2048; //fftDataSize; FFT2048
            int frameCount = (int)(waveformInputStream.Length / frameLength);
            var waveformLength = frameCount * 2;
            var readBuffer = new byte[frameLength];

            _waveformAggregator = new SampleAggregator(frameLength);

            var maxLeftPointLevel = float.MinValue;
            var maxRightPointLevel = float.MinValue;
            var currentPointIndex = 0;
            var waveformCompressedPoints = new float[numberOfPoints];
            var waveformData = new List<float>();
            var waveMaxPointIndexes = new List<int>();

            for (int i = 1; i <= numberOfPoints; i++)
            {
                waveMaxPointIndexes.Add((int)Math.Round(waveformLength * ((double)i / numberOfPoints), 0));
            }
            int readCount = 0;
            while (currentPointIndex * 2 < numberOfPoints)
            {
                waveformInputStream.Read(readBuffer, 0, readBuffer.Length);

                waveformData.Add(_waveformAggregator.LeftMaxVolume);
                waveformData.Add(_waveformAggregator.RightMaxVolume);

                if (_waveformAggregator.LeftMaxVolume > maxLeftPointLevel)
                    maxLeftPointLevel = _waveformAggregator.LeftMaxVolume;
                if (_waveformAggregator.RightMaxVolume > maxRightPointLevel)
                    maxRightPointLevel = _waveformAggregator.RightMaxVolume;

                if (readCount > waveMaxPointIndexes[currentPointIndex])
                {
                    waveformCompressedPoints[(currentPointIndex * 2)] = maxLeftPointLevel; // Set even indexes
                    waveformCompressedPoints[(currentPointIndex * 2) + 1] = maxRightPointLevel; // Set odd indexes
                    maxLeftPointLevel = float.MinValue; // Reset float
                    maxRightPointLevel = float.MinValue; // Reset float
                    currentPointIndex++; // Increase index
                }
                if (readCount % 3000 == 0) // Every 3000 samples update UI
                {
                    float[] clonedData = (float[])waveformCompressedPoints.Clone();
                    WaveformData = clonedData;
                }

                if (_worker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }
                readCount++;
            }

            float[] finalClonedData = (float[])waveformCompressedPoints.Clone();
            WaveformData = finalClonedData;

            _waveformComplete = true;
        }

        #endregion Private Methods

        /// <summary>
        /// Generates waveform data from a given file path.
        /// </summary>
        /// <param name="filePath">The file path of a music file.</param>
        public void GenerateWaveformData(string filePath)
        {
            if (_filePath == filePath && _waveformComplete) return;

            _filePath = filePath;

            try
            {
                if (_worker.IsBusy)
                    _worker.CancelAsync();

                if (!_worker.IsBusy)
                    _worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}
