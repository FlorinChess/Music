using System;

namespace Music.NAudio.WaveformGenerator
{
    public sealed class WaveformDataEventArgs : EventArgs
    {
        public float[] WaveformData { get; set; }
        public WaveformDataEventArgs(float[] waveformData)
        {
            WaveformData = waveformData;
        }
    }
}
