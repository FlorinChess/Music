using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;

namespace Music.NAudio
{
    public sealed class AudioNotificationClient : IMMNotificationClient
    {
        public void OnDefaultDeviceChanged(DataFlow flow, Role role, string defaultDeviceId)
        {
            throw new System.NotImplementedException();
        }

        public void OnDeviceAdded(string pwstrDeviceId)
        {
            throw new System.NotImplementedException();
        }

        public void OnDeviceRemoved(string deviceId)
        {
            throw new System.NotImplementedException();
        }

        public void OnDeviceStateChanged(string deviceId, DeviceState newState)
        {
            throw new System.NotImplementedException();
        }

        public void OnPropertyValueChanged(string pwstrDeviceId, PropertyKey key)
        {
            throw new System.NotImplementedException();
        }
    }
}
