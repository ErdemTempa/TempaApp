using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TempaApp.Interface
{
    public interface IBle
    {

       // List<string> GetBondedDevices();

        bool Init ();
        event EventHandler<byte[]> DataReceived;
        event EventHandler<bool> Connected;
        event EventHandler<bool> Disconnected;


        bool ConnectSpecificDevice(string DeviceName, string serviceUUID, string senderCharacteristicUUID, string responseCharacteristicUUID, string descriptorUUID, bool ScanEnable);
        bool SendData(byte[] data, bool WriteWithResponse);
        void RefreshConnection();
        int getMTUsize();
        //Task<byte[]> SendCommand(string command, UInt16 dataLength, byte[] data, string response, int timeout);
    }
}
