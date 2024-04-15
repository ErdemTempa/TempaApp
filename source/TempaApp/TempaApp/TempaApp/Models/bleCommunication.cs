using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TempaApp.Interface;
using Xamarin.Essentials;
using Xamarin.Forms;
using static Xamarin.Essentials.Permissions;

namespace TempaApp.Models
{
    public class bleCommunication
    {
        private IBle BleInterface;
        public List<byte> rxData;
        public List<byte> weightData;
        public List<byte> infoData;
        public List<byte> unitData;

        public int MaxDataLengthPerPack = 23-3;
        private bool connection = false;
        public bool Connection {            
            get
            {                
                return connection;
            }
        }

        private bool alarm = false;
        public bool Alarm
        {
            get
            {
                return alarm;
            }
        }

        private bool TxBusy = false;

        private byte[] responseBytes;
        public CancellationTokenSource SendCommandCancellationTokenSource = null;

        public bleCommunication ()
        {
            BleInterface = DependencyService.Get<IBle>();
            BleInterface.Init();
            connection = false;
            alarm = false;
            rxData = new List<byte>();
            weightData = new List<byte>();
            infoData = new List<byte>();
            unitData = new List<byte>();
            bleData = new BleData();
            BleInterface.DataReceived += OnDataReceivedEvent;
            BleInterface.Connected += OnConnectedEvent;
            BleInterface.Disconnected += OnDisconnectedEvent;
            //fix me burada eventleri oluşturnak lazım. bir defa oluşturulacak ve hep olacak.
        }

        ~bleCommunication()
        {
            BleInterface.DataReceived -= OnDataReceivedEvent;
            BleInterface.Connected -= OnConnectedEvent;
            BleInterface.Disconnected -= OnDisconnectedEvent;
        }
       
        public class bleServices
        {
            public static string SERVICE_UUID = "6E400001-B5A3-F393-E0A9-E50E24DCCA9E";
            public static string CHARACTERISTIC_UUID_SENDER = "6E400002-B5A3-F393-E0A9-E50E24DCCA9E";
            public static string CHARACTERISTIC_UUID_RESPONSE = "6E400003-B5A3-F393-E0A9-E50E24DCCA9E";

            public static string CHARACTERISTIC_UPDATE_NOTIFICATION_DESCRIPTOR_UUID = "00002902-0000-1000-8000-00805f9b34fb";
            public static string DEVICENAME = "TEMPA-XTYPE-SMART";

        }

        public class BleData
        {
            public int status = 0;
            public byte[] command = new byte[3];            
            public UInt16 dataLength;            
            public List<byte> data = new List<byte>();
            public UInt16 crc16;
            public bool dataReady = false;
            public bool dataCancelled = false;
            public void clear()
            {
                status = 0; 
                dataLength = 0;
                crc16 = 0;
                dataReady = false;
                dataCancelled = false;
                data.Clear();
                command[0] = 0;
                command[1] = 0;
                command[2] = 0;
            }
        };
        BleData bleData;

        public static class bleCommands
        {

            public static string SetKey
            {
                get { return "SKC"; }
            }

            public static string SetZero
            {
                get { return "SZC"; }
            }

            public static string GetWeight
            {
                get { return "GWC"; }
            }
            public static string GetDisplay
            {
                get { return "DDC"; }
            }

            public static string StartLoad
            {
                get { return "SLC"; }
            }

            public static string StartUnLoad
            {
                get { return "SUC"; }
            }


            public static bool CheckCommand(byte[] NewCommand)
            {
                string s = Encoding.Default.GetString(NewCommand);

                if (String.Equals(s, bleCommandsResponse.SetKey))
                {
                    return true;
                }
                else if (String.Equals(s, bleCommandsResponse.SetZero))
                {
                    return true;
                }
                else if (String.Equals(s, bleCommandsResponse.GetDisplay ))
                {
                    return true;
                }
                else if (String.Equals(s, bleCommandsResponse.GetWeight))
                {
                    return true;
                }
                else if (String.Equals(s, bleCommandsResponse.StartLoad))
                {
                    return true;
                }
                else if (String.Equals(s, bleCommandsResponse.StartUnLoad))
                {
                    return true;
                }
                return false;
            }

        }

        public static class bleCommandsResponse
        {


            public static string SetKey
            {
                get { return "SKR"; }
            }

            public static string SetZero
            {
                get { return "SZR"; }
            }

            public static string GetWeight
            {
                get { return "GWR"; }
            }

            public static string GetDisplay
            {
                get { return "DDR"; }
            }

            public static string StartLoad
            {
                get { return "SLR"; }
            }

            public static string StartUnLoad
            {
                get { return "SUR"; }
            }

        }

        public bool ConnectMyBleDevice ()
        {
            //var _devices = BleInterface.GetBondedDevices();
            //foreach (var device in _devices)
            //{
                //if (device.IndexOf(bleServices.DEVICENAME) != -1)
                //{
            if (BleInterface.ConnectSpecificDevice(bleServices.DEVICENAME, bleServices.SERVICE_UUID, bleServices.CHARACTERISTIC_UUID_SENDER, bleServices.CHARACTERISTIC_UUID_RESPONSE, bleServices.CHARACTERISTIC_UPDATE_NOTIFICATION_DESCRIPTOR_UUID,true) == true)
            {
                //connection = true;
                //BleInterface.DataReceived += OnDataReceivedEvent;
                //BleInterface.Connected += OnConnectedEvent;
                //BleInterface.Disconnected += OnDisconnectedEvent;
                rxData.Clear();
                MaxDataLengthPerPack = BleInterface.getMTUsize()-3;
                return true;    
            }
                //}
            //}
            //connection = false;
            return false;
        }

        private void OnConnectedEvent (object source, bool eventArgs)
        {
            connection = eventArgs;
        }
        private void OnDisconnectedEvent(object source, bool eventArgs)
        {
            connection = eventArgs;
        }

        private void OnDataReceivedEvent(object source, byte[] eventArgs)
        {
            //rxData.AddRange(eventArgs); deneme . çalışan

            responseBytes = eventArgs; // e.Characteristic.Value;


            //ResponseData.AddRange(responseBytes);


            for (int i = 0; i < responseBytes.Length; i++)
            {
                switch (bleData.status)
                {
                    case 0:
                        bleData.clear();
                        bleData.command[0] = responseBytes[i];
                        bleData.status++;                       
                        break;

                    case 1:
                        bleData.command[1] = responseBytes[i];
                        bleData.status++;
                        break;

                    case 2:
                        bleData.command[2] = responseBytes[i];
                        if (bleCommands.CheckCommand(bleData.command))
                        {
                            bleData.status++;
                        }
                        else
                        {
                            bleData.status = 0;
                        }
                        break;

                    case 3:
                        if (responseBytes[i] > 0x10)
                        {
                            bleData.status = 0;
                        }
                        else
                        {
                            bleData.dataLength = responseBytes[i];
                            bleData.dataLength <<= 8;
                            bleData.status++;
                        }
                        break;

                    case 4:
                        bleData.dataLength |= responseBytes[i];
                        if (bleData.dataLength > 4096)
                        {
                            bleData.status = 0;
                        }
                        else if (bleData.dataLength == 0)
                        {
                            bleData.status += 2;
                        }
                        else
                        {
                            bleData.status++;
                        }
                        break;

                    case 5:
                        bleData.data.Add(responseBytes[i]);
                        if (bleData.data.Count == bleData.dataLength)
                        {
                            bleData.status++;
                        }
                        break;

                    case 6:
                        bleData.crc16 = responseBytes[i];
                        bleData.crc16 <<= 8;
                        bleData.status++;
                        break;

                    case 7:
                        bleData.crc16 |= responseBytes[i];

                        if (bleData.dataLength == 0)
                        {
                            //if (bleData.crc16 == 0)
                            //{
                                bleData.dataReady = true;
                                responseBytes = Array.Empty<byte>();
                                //responseCancelationTokenSource.Cancel();
                                SendCommandCancellationTokenSource?.Cancel();
                                return;
                            //}
                            //else
                            //{
                            //    bleData.status = 0;
                            //}
                        }
                        else
                        {
                            //Crc16 crc16 = new Crc16();

                            byte[] dataArray = bleData.data.ToArray();
                            var crc = Crc16.calculateCRC16(dataArray, (UInt16)dataArray.Length);


                            //if (/*crc16.ComputeChecksum(bleData.data.ToArray())*/ crc == bleData.crc16)
                            //{
                                bleData.dataReady = true;
                                responseBytes = Array.Empty<byte>();
                                //responseCancelationTokenSource.Cancel();
                                SendCommandCancellationTokenSource?.Cancel();
                                return;
                            //}
                            //else
                            //{
                            //    bleData.status = 0;
                            //}
                        }
                        break;


                }
            }

            responseBytes = Array.Empty<byte>();

        }


        private async Task<byte[]> SendCommand(string command, UInt16 dataLength, byte[] data, string response, int timeout)
        {
           
            byte[] senderBytes;
            byte[] responseBytes;
            byte[] retVal;
            int k=0;
            if (connection == false)
            {
                
                return null;
            }
            while (TxBusy)
            {
                await Task.Delay(50);
                k += 50;
                if (TxBusy == false)
                {
                    break;
                }
                if (k >= timeout)
                {
                    return null;
                }
                    
            }

            TxBusy = true;

            SendCommandCancellationTokenSource = new CancellationTokenSource();
            var token = SendCommandCancellationTokenSource.Token;

            //bleData.dataCancelled = false;
            try
            {



//                bleData.status = 0;


                //await responseCharacteristic.StartUpdatesAsync(token);


                senderBytes = Array.Empty<byte>();
                responseBytes = Array.Empty<byte>();

                List<byte> bytes = new List<byte>(Encoding.ASCII.GetBytes(command));

                bytes.Add((byte)(dataLength >> 8));
                bytes.Add((byte)(dataLength));


                UInt16 crc16 = Crc16.calculateCRC16(data, dataLength);
                if (dataLength > 0)
                {
                    bytes.AddRange(data);
                    bytes.Add((byte)(crc16 >> 8));
                    bytes.Add((byte)(crc16));
                }
                else
                {
                    bytes.Add(0);
                    bytes.Add(0);
                }


                //bleData.dataReady = false;
                bleData.clear();
                //responseCharacteristic.ValueUpdated += Responsecharacteristic_ValueUpdated;
                

                if (bytes.Count > MaxDataLengthPerPack)
                {
                    for (int i = 0; i < bytes.Count; i += MaxDataLengthPerPack)
                    {
                        senderBytes = Array.Empty<byte>();
                        if (bytes.Count - i > MaxDataLengthPerPack)
                        {
                            senderBytes = bytes.GetRange(i, MaxDataLengthPerPack).ToArray();
                        }
                        else
                        {
                            senderBytes = bytes.GetRange(i, bytes.Count - i).ToArray();
                        }

                        if (BleInterface.SendData(senderBytes, true) == false)
                        {
                            SendCommandCancellationTokenSource?.Dispose();
                            SendCommandCancellationTokenSource = null;
                            TxBusy = false;
                            return null;
                        }


                        //if (await senderCharacteristic.WriteAsync(senderBytes, token) == false)
                        //{
                        //    responseCharacteristic.ValueUpdated -= Responsecharacteristic_ValueUpdated;
                        //    if (bleCancellationTokenSource != null)
                        //    {
                        //        bleCancellationTokenSource.Dispose();
                        //        bleCancellationTokenSource = null;
                        //    }
                        //    bleData.status = 0;
                        //    return null;
                        //}
                    }
                }
                else
                {
                    senderBytes = bytes.ToArray();

                    if (BleInterface.SendData(senderBytes, true) == false)
                    {
                        SendCommandCancellationTokenSource?.Dispose();
                        SendCommandCancellationTokenSource = null;
                        TxBusy = false;
                        return null;
                    }

                    //if (await senderCharacteristic.WriteAsync(senderBytes, token) == false)
                    //{
                    //    responseCharacteristic.ValueUpdated -= Responsecharacteristic_ValueUpdated;
                    //    if (bleCancellationTokenSource != null)
                    //    {
                    //        bleCancellationTokenSource.Dispose();
                    //        bleCancellationTokenSource = null;
                    //    }

                    //    bleData.status = 0;
                    //    return null;
                    //}

                }

                await Task.Delay(timeout, token);
            }
            catch (Exception ex)
            {
                if (token.IsCancellationRequested)
                {
                    retVal = null;
                }
            }

            finally
            {
               // responseCharacteristic.ValueUpdated -= Responsecharacteristic_ValueUpdated;
                if (token.IsCancellationRequested)
                {
                    if (bleData.dataCancelled)
                    {
                        retVal = null;
                    }
                    else if (bleData.dataReady)
                    {
                        string str = Encoding.ASCII.GetString(bleData.command, 0, 3);


                        if (String.Equals(str, response) == false)
                        {
                            retVal = null;
                        }
                        else
                        {

                            if (bleData.dataLength > 0)
                            {

                                retVal = bleData.data.ToArray();

                            }
                            else
                            {
                                //if (bleData.crc16 != 0)
                                //{
                                //    retVal = null;
                                //}
                                //else
                                //{
                                    List<byte> b = new List<byte>();
                                    b.Add(0);
                                    retVal = b.ToArray();
                                //}

                            }
                        }
                    }
                    else
                    {
                        retVal = null;
                    }
                }
                else
                {
                    retVal = null;
                }
            }
            SendCommandCancellationTokenSource?.Dispose();
            SendCommandCancellationTokenSource = null;
            //            bleData.status = 0;
            bleData.clear();
            TxBusy = false;
            return retVal;

        }

        public async Task<bool> SendKey(string keyData)
        {
            byte[] CmdData = Encoding.ASCII.GetBytes(keyData);
            byte[] data = await SendCommand(bleCommands.SetKey, (ushort)CmdData.Length, CmdData, bleCommandsResponse.SetKey, 10000);
            if (data != null)
            {
                if (data.Length >= 2)
                {
                    if (data[0] == 'O' && data[1] == 'K')
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public async Task<bool> SetZeroAsync()
        {

            byte[] CmdData = Encoding.ASCII.GetBytes(bleCommands.SetZero.ToString());
            byte[] data = await SendCommand(bleCommands.SetZero, (ushort)CmdData.Length, CmdData, bleCommandsResponse.SetZero, 10000);
            if ( data != null )
            {
                if (data.Length >= 2)
                {
                    if (data[0] == 'O' && data[1] == 'K')
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public async Task<bool> StartLoadAsync()
        {

            byte[] CmdData = Encoding.ASCII.GetBytes(bleCommands.StartLoad.ToString());
            byte[] data = await SendCommand(bleCommands.StartLoad, (ushort)CmdData.Length, CmdData, bleCommandsResponse.StartLoad, 10000);
            if (data != null)
            {
                if (data.Length >= 2)
                {
                    if (data[0] == 'O' && data[1] == 'K')
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public async Task<bool> StartUnLoadAsync()
        {

            byte[] CmdData = Encoding.ASCII.GetBytes(bleCommands.StartUnLoad.ToString());
            byte[] data = await SendCommand(bleCommands.StartUnLoad, (ushort)CmdData.Length, CmdData, bleCommandsResponse.StartUnLoad, 10000);
            if (data != null)
            {
                if (data.Length >= 2)
                {
                    if (data[0] == 'O' && data[1] == 'K')
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public async Task<bool> GetWeight()
        {

            byte[] CmdData = Encoding.ASCII.GetBytes(bleCommands.GetWeight.ToString());
            byte[] data = await SendCommand(bleCommands.GetWeight, (ushort)CmdData.Length, CmdData, bleCommandsResponse.GetWeight, 10000);
            if (data != null)
            {
                if (data.Length >= 2)
                {
                    if (data[0] == 'O' && data[1] == 'K')
                    {
                        return true;
                    }
                }
            }
            return false;

        }

        public async Task<bool> GetDisplayData()
        {
            byte[] CmdData = Encoding.ASCII.GetBytes(bleCommands.GetDisplay.ToString());
            byte[] data = await SendCommand(bleCommands.GetDisplay, (ushort)CmdData.Length, CmdData, bleCommandsResponse.GetDisplay, 1000);
            if (data != null)
            {
                if (data.Length > 27)
                {                    
                    if (data[0] == '#' && data[11] == '#' && data[26] == '#')
                    {
                        var tempString = Encoding.ASCII.GetString(data);
                        var TotalStrings = tempString.Split(new char[] { '#' });
                        if (TotalStrings.Length != 6)
                        {
                            return false;
                        }
                        weightData.Clear();
                        var bytes = Encoding.ASCII.GetBytes(TotalStrings[1]);
                        foreach (byte b in bytes)
                        {
                            weightData.Add(b);
                        }
                        unitData.Clear();
                        var bytes2 = Encoding.ASCII.GetBytes(TotalStrings[2]);
                        foreach (byte b in bytes2)
                        {
                            unitData.Add(b);
                        }
                        infoData.Clear();
                        var bytes3 = Encoding.ASCII.GetBytes(TotalStrings[3]);
                        foreach (byte b in bytes3)
                        {
                            infoData.Add(b);
                        }
                        if (alarm == false && data[27] =='1')
                        {
                            Vibration.Vibrate(2000);
                        }
                        if (data[27] == '1')
                        {
                            alarm = true;
                        }
                        else
                        {
                            alarm = false;
                        }
                        return true;





                    }

                }

            }
            return false;
        }

        public void RefreshConnection()
        {
            BleInterface.RefreshConnection();
        }

        }
}
