using System;

namespace Music.NAudio.WaveformGenerator
{
    public interface IWaveformGenerator
    {
        public float[] WaveformData { get; set; }
        public void GenerateWaveformData(string filePath);
    }
}