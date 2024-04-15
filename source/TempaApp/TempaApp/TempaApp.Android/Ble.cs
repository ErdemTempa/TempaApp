using Android.App;
using Android.Bluetooth;
using Android.Bluetooth.LE;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TempaApp.Droid;
using TempaApp.Interface;
using Xamarin.Forms;

[assembly: Dependency(typeof(Ble))]
namespace TempaApp.Droid
{
    public class Ble : IBle
    {
        private int requestMTUSize = 247;
        private int currentMTUsize = 20;
        private BluetoothManager _bluetoothManager;
        private BluetoothAdapter _bluetoothAdapter;
        private ICollection<BluetoothDevice> Devicelist;// = new ICollection<BluetoothDevice>();
        private BluetoothDevice _device;
        private BluetoothGatt _gatt;
        private IList<BluetoothGattService> _gattServices;
        private Context mContext;
        private BleGattCallbacks _callbacks;
        private BluetoothGattService _gattService;
        private BluetoothGattCharacteristic _gattCharacteristicSender;
        private BluetoothGattCharacteristic _gattCharacteristicReceiver;
        private BluetoothGattDescriptor _gattDescriptorReceiver;
        
        //public event EventHandler<DataReceivedEventArgs> DataReceived;

        public event EventHandler<byte[]> DataReceived;
        public event EventHandler<bool> Connected;
        public event EventHandler<bool> Disconnected;


        public class MyScanCallback : ScanCallback
        {            
            public IList<BluetoothDevice> ScannedDeviceList { get; private set; }
            public CancellationTokenSource bleScanCancellationTokenSource = null;

            public MyScanCallback()
            { 
                ScannedDeviceList = new List<BluetoothDevice>();
            }
                          

            public override void OnBatchScanResults(IList<ScanResult> results)
            {
                base.OnBatchScanResults(results);
            }

            public override void OnScanResult([GeneratedEnum] ScanCallbackType callbackType, ScanResult result)
            {
                base.OnScanResult(callbackType, result);
                if (result.Device != null)
                {
                    ScannedDeviceList.Add(result.Device);
                    bleScanCancellationTokenSource?.Cancel();
                }
            }

            public override void OnScanFailed([GeneratedEnumAttribute] ScanFailure errorCode)
            {
                base.OnScanFailed(errorCode);
            }
        }

        public BluetoothDevice GetBondedDevice(string DeviceName, string serviceUUID)
        {
            ////var ctx = Android.App.Application.Context;
            //////BluetoothAdapter mBluetoothAdapter = BluetoothAdapter.DefaultAdapter;
            ////_bluetoothManager = (BluetoothManager)ctx.GetSystemService(Context.BluetoothService);

            ////var adapter = _bluetoothManager.Adapter;

            //var ctx = Android.App.Application.Context;
            //mContext = ctx;
            //if (!ctx.PackageManager.HasSystemFeature(PackageManager.FeatureBluetoothLe))
            //    return null;

            //_bluetoothManager = (BluetoothManager)ctx.GetSystemService(Context.BluetoothService);
            //_bluetoothAdapter = _bluetoothManager.Adapter;

            var pairedDevices = _bluetoothAdapter.BondedDevices;


            List<string> list = new List<string>();
            foreach (BluetoothDevice device in pairedDevices)
            {
                if (device.Name.IndexOf(DeviceName) != -1)
                {
                    return device;                    
                }
            }            

            return null;
        }

        //public System.Collections.Generic.ICollection<Android.Bluetooth.BluetoothDevice>? BondedDevices
        //{
        //    [Android.Runtime.Register("getBondedDevices", "()Ljava/util/Set;", "")]
        //    [Android.Runtime.RequiresPermission("android.permission.BLUETOOTH_CONNECT")]
        //    get;
        //}

        public bool Init ()
        {


            var ctx = Android.App.Application.Context;
            
            if (!ctx.PackageManager.HasSystemFeature(PackageManager.FeatureBluetoothLe))
            {
                mContext = null;
                return false;
            }

            mContext = ctx;
            _bluetoothManager = (BluetoothManager)ctx.GetSystemService(Context.BluetoothService);
            _bluetoothAdapter = _bluetoothManager.Adapter;

            _device = null;
            _callbacks = null;
            UpdateConnectionStatus();
            

            return true;



        }

        protected virtual void OnDataReceived(byte[] data)
        {
            DataReceived?.Invoke(this, data);
        }

        public void OnDataReceivedFromCallback(object source, BleGattCallbacks.DataReceivedEventArgs eventArgs)
        {
            OnDataReceived(eventArgs.Data);
        }

        protected virtual void OnConnected(bool connection)
        {
            Connected?.Invoke(this,connection);
        }
        public void OnConnectedFromCallback(object source, bool eventArgs)
        {
            OnConnected(eventArgs);
        }

        protected virtual void OnDisconnected(bool connection)
        {
            Disconnected?.Invoke(this, connection);
        }
        public void OnDisconnectedFromCallback(object source, bool eventArgs)
        {
            OnDisconnected(eventArgs);
        }

        public BluetoothDevice ScanForSpecificDevice (string DeviceName, string serviceUUID)
        {
            BluetoothDevice retVal = null;
            var ctx = Android.App.Application.Context;
            mContext = ctx;
            if (!ctx.PackageManager.HasSystemFeature(PackageManager.FeatureBluetoothLe))
                return null;

            _bluetoothManager = (BluetoothManager)ctx.GetSystemService(Context.BluetoothService);
            _bluetoothAdapter = _bluetoothManager.Adapter;

            if (_gatt != null)
            {
                _gatt.Disconnect();
                _gatt.Close();
            }
            ScanFilter.Builder filter = new ScanFilter.Builder();
            //filter.SetServiceUuid(ParcelUuid.FromString(serviceUUID));
            filter.SetDeviceName(DeviceName);

            ScanSettings.Builder settings = new ScanSettings.Builder();
            settings.SetCallbackType(ScanCallbackType.FirstMatch);            
            settings.SetScanMode(Android.Bluetooth.LE.ScanMode.LowLatency);
            

            MyScanCallback myScanCallback = new MyScanCallback();  
            IList<ScanFilter> filters = new List<ScanFilter>();
            filters.Add(filter.Build());

            _bluetoothAdapter.BluetoothLeScanner.StartScan(filters, settings.Build(),  myScanCallback);
            //_bluetoothAdapter.BluetoothLeScanner.StartScan( myScanCallback);

            myScanCallback.bleScanCancellationTokenSource = new CancellationTokenSource();
            var token = myScanCallback.bleScanCancellationTokenSource.Token;

            try
            {

                Task.Delay(5000,token).Wait();
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
                _bluetoothAdapter.BluetoothLeScanner.StopScan(myScanCallback);
                if (token.IsCancellationRequested)
                {
                     foreach (BluetoothDevice device in myScanCallback.ScannedDeviceList )
                    {
                        retVal = device;
                        break;
                    }                   
                }

            }
            myScanCallback.bleScanCancellationTokenSource?.Dispose();
            myScanCallback.bleScanCancellationTokenSource=null;
            return retVal;

        }
        public bool ConnectSpecificDevice(string DeviceName, string serviceUUID, string senderCharacteristicUUID, string responseCharacteristicUUID, string descriptorUUID,bool ScanEnable)
        {
            bool retVal = false;
            BluetoothDevice NewDevice;
            if (ScanEnable)
            {
                 NewDevice = ScanForSpecificDevice(DeviceName, serviceUUID);
            }
            else
            {
                NewDevice = GetBondedDevice(DeviceName, serviceUUID);   
            }


            if (NewDevice == null)
            {
                return false;
            }

            if (_device != null)
            {
                if (NewDevice.Name != _device.Name)
                {
                    if (_callbacks!=null)
                    {
                        _callbacks.DataReceivedFromCallback -= OnDataReceivedFromCallback;
                        _callbacks.DisconnectedFromCallback -= OnDisconnectedFromCallback;
                        _callbacks.ConnectedFromCallback -= OnConnectedFromCallback;
                        _callbacks?.Dispose();
                        _callbacks = null;
                        
                    }
                    _device = NewDevice;

                }


            }
            else
            {
                _device = NewDevice; 
            }

            
            if (_callbacks == null)
            {
                _callbacks = new BleGattCallbacks( _device);
                _callbacks.DataReceivedFromCallback += OnDataReceivedFromCallback;
                _callbacks.ConnectedFromCallback += OnConnectedFromCallback;
                _callbacks.DisconnectedFromCallback += OnDisconnectedFromCallback;
            }


            _callbacks.ConnectionCancellationTokenSource = new CancellationTokenSource();
            var token = _callbacks.ConnectionCancellationTokenSource.Token;
            _gatt=_device.ConnectGatt(mContext, false, _callbacks);
            
            
            try
            {
                Task.Delay(10000, token).Wait();
            }
            catch(Exception ex)
            {
                if (token.IsCancellationRequested)
                {
                    retVal = false;
                }
            }
            finally
            {


                if (_callbacks.IsConnected())
                {

                    _gattCharacteristicSender = _callbacks.mGatt.GetService(Java.Util.UUID.FromString(serviceUUID))?.GetCharacteristic(Java.Util.UUID.FromString(senderCharacteristicUUID));
                    _gattCharacteristicReceiver = _callbacks.mGatt.GetService(Java.Util.UUID.FromString(serviceUUID))?.GetCharacteristic(Java.Util.UUID.FromString(responseCharacteristicUUID));
                    _gattDescriptorReceiver = _gattCharacteristicReceiver.GetDescriptor(Java.Util.UUID.FromString(descriptorUUID));
                    if (_gattCharacteristicReceiver == null || _gattCharacteristicSender == null)
                    {
                        //_callbacks.DataReceivedFromCallback -= OnDataReceivedFromCallback;
                        //_callbacks.DisconnectedFromCallback -= OnDisconnectedFromCallback;
                        //_callbacks.ConnectedFromCallback -= OnConnectedFromCallback;
                        if (_callbacks.IsConnected())
                        {
                            _callbacks.SetConnect(false);
                            OnDisconnected(false);
                        }
                        retVal = false;
                    }
                    else
                    {

                        if (EnableNotifications(_gattCharacteristicReceiver, _gattDescriptorReceiver) == false)
                        {
                            if (_callbacks.IsConnected())
                            {
                                _callbacks.SetConnect(false);
                                OnDisconnected(false);
                            }
                            retVal = false;
                        }
                        else
                        {
                            //_callbacks.DataReceivedFromCallback += OnDataReceivedFromCallback;  //fix me bu çıkarılaacak mı disconnect te
                            //_callbacks.ConnectedFromCallback += OnConnectedFromCallback;
                            //_callbacks.DisconnectedFromCallback += OnDisconnectedFromCallback;
                            retVal = true;
                        }
                    }

                }
                else
                {
                    //_callbacks.DataReceivedFromCallback -= OnDataReceivedFromCallback;
                    //_callbacks.DisconnectedFromCallback -= OnDisconnectedFromCallback;
                    //_callbacks.ConnectedFromCallback -= OnConnectedFromCallback;
                    retVal = false;
                }
            }

            _callbacks.ConnectionCancellationTokenSource?.Dispose();
            _callbacks.ConnectionCancellationTokenSource = null;

            return retVal;
        }
        public bool SendData(byte[] data, bool WriteWithResponse)
        {
            bool retVal = false;
            if (data == null)
                return false;

            if (_gattCharacteristicSender==null || _gattCharacteristicReceiver==null)
                return false;

            if (_callbacks == null)
                return false;

            //if (_callbacks.GetConnectionStatus() == false)
            //{
            //    return false;
            //}
            if (_callbacks.IsConnected()==false)    
                return false;

            _callbacks.ClearSendOk();
            if (WriteWithResponse)
            {
                _gattCharacteristicSender.WriteType = GattWriteType.Default;
            }
            else
            {
                _gattCharacteristicSender.WriteType = GattWriteType.NoResponse;
            }

            if (WriteWithResponse == true)
            {
                _callbacks.SendDataCancellationTokenSource = new CancellationTokenSource();
                var token = _callbacks.SendDataCancellationTokenSource.Token;
                _gattCharacteristicSender.SetValue(data);
                _callbacks.mGatt.WriteCharacteristic(_gattCharacteristicSender);
                try
                {
                    Task.Delay(5000,token).Wait();
                }
                catch (Exception ex)
                {
                    if (token.IsCancellationRequested)
                    {
                        retVal = false;
                    }
                }
                finally
                {
                    if (token.IsCancellationRequested && _callbacks.IsSendOk() )
                    {
                        retVal = true;
                    }
                    else
                    {
                        retVal = false;
                    }

                }
                _callbacks.SendDataCancellationTokenSource?.Dispose();
                _callbacks.SendDataCancellationTokenSource=null;
                return retVal;

            }
            else
            {
                _gattCharacteristicSender.SetValue(data);
                _callbacks.mGatt.WriteCharacteristic(_gattCharacteristicSender);
                return true;
            }
        }

        public bool EnableNotifications (BluetoothGattCharacteristic characteristic, BluetoothGattDescriptor descriptor)
        {
            bool retVal = false;
            _callbacks.setDescriptorWriteOk(false);
            if ( _callbacks.mGatt.SetCharacteristicNotification(characteristic, true) == false)
                return false ;
            if ( descriptor.SetValue(BluetoothGattDescriptor.EnableNotificationValue.ToArray()) == false )
                return false ;
            if (_callbacks.mGatt.WriteDescriptor(descriptor) == false)
                return false;
            _callbacks.DescriptorWriteCancellationTokenSource = new CancellationTokenSource();
            var token = _callbacks.DescriptorWriteCancellationTokenSource.Token;
            try
            {
                Task.Delay(5000,token).Wait();
            }
            catch(Exception ex)
            {
                if ( token.IsCancellationRequested )
                {
                    retVal = false;
                }
            }
            finally
            {
                if (token.IsCancellationRequested && _callbacks.IsDescriptorWriteOk())
                {
                    retVal = true;
                }
                else
                {
                    retVal= false;
                }
            }
            _callbacks.DescriptorWriteCancellationTokenSource?.Dispose();
            _callbacks.DescriptorWriteCancellationTokenSource=null;
            return retVal ;
            
        }

        public int getMTUsize()
        {
            return _callbacks.MTU;
        }

        //public bool CheckConnection()
        //{
        //    if (_callbacks == null )    
        //        return false ;
        //    return _callbacks.CheckConnection();
        //}

        private async Task UpdateConnectionStatus()
        {
            await Task.Run( async () =>
            {
                while (true)
                {
                    await Task.Delay(1000);
                    if (_callbacks == null)
                    {
                        break;
                    }

                    if (_bluetoothManager == null || _bluetoothAdapter == null || _gatt == null || _device == null)
                    {
                        if (_callbacks.IsConnected())
                        {
                            _callbacks.SetConnect(false);
                            OnDisconnected(false);
                        }

                    }

                    if (_bluetoothManager.GetConnectionState(_device, ProfileType.Gatt) == ProfileState.Connected)
                    {
                        if (_callbacks.IsConnected() == false)
                        {
                            _callbacks.SetConnect(true);
                            OnConnected(true);
                        }

                    }
                    else
                    {
                        if (_callbacks.IsConnected())
                        {
                            _callbacks.SetConnect(false);
                            OnDisconnected(false);
                        }

                    }

                }

            }
            );
        }

        public void RefreshConnection()
        {
            _callbacks.GetConnectionStatus();
        }



        //public byte[] ReceiveData ()
        //{


        //    if (_gattCharacteristicSender == null || _gattCharacteristicReceiver == null)
        //        return null;

        //    if (_callbacks == null)
        //        return null;

        //    if (_callbacks.IsConnected() == false)
        //        return null;


        //    return null;
        //}

        //public bool sendData (byte[] data)
        //{

        //    if ((_gattCharacteristicSender.Properties & GattProperty.Write) > 0)
        //    {
        //        _gattCharacteristicSender.WriteType = GattWriteType.NoResponse;
        //        _gattCharacteristicSender.SetValue(data);
        //        _gatt.WriteCharacteristic(_gattCharacteristicSender);
        //    }
        //}
        //public Task<byte[]> SendCommand(string command, UInt16 dataLength, byte[] data, string response,  int timeout)
        //{
        //    byte[] senderBytes;

        //    if (_callbacks.IsConnected()==false)
        //    {
        //        return null;
        //    }
        //    if ((_gattCharacteristicSender.Properties & GattProperty.Write) == 0)
        //    {
        //        return null;
        //    }
        //    senderBytes = Array.Empty<byte>();
        //    //responseBytes = Array.Empty<byte>();

        //    List<byte> bytes;
        //    if (command == null)
        //    {
        //        bytes = new List<byte>();
        //    }
        //    else
        //    {
        //       bytes = new List<byte>(Encoding.ASCII.GetBytes(command));
        //    }

        //    bytes.Add((byte)(dataLength >> 8));
        //    bytes.Add((byte)(dataLength));

        //    UInt16 crc16 = Crc16.calculateCRC16(data, dataLength);
        //    if (dataLength > 0)
        //    {
        //        bytes.AddRange(data);
        //        bytes.Add((byte)(crc16 >> 8));
        //        bytes.Add((byte)(crc16));
        //    }
        //    else
        //    {
        //        bytes.Add(0);
        //        bytes.Add(0);
        //    }


        //    if (bytes.Count > currentMTUsize-3)
        //    {
        //        for (int i = 0; i < bytes.Count; i += (currentMTUsize - 3))
        //        {
        //            senderBytes = Array.Empty<byte>();
        //            if (bytes.Count - i > (currentMTUsize - 3))
        //            {
        //                senderBytes = bytes.GetRange(i, (currentMTUsize - 3)).ToArray();
        //            }
        //            else
        //            {
        //                senderBytes = bytes.GetRange(i, bytes.Count - i).ToArray();
        //            }

        //            _gattCharacteristicSender.WriteType = GattWriteType.NoResponse;
        //            _gattCharacteristicSender.SetValue(senderBytes);
        //            _gatt.WriteCharacteristic(_gattCharacteristicSender);
        //        }
        //    }
        //    else
        //    {
        //        senderBytes = bytes.ToArray();

        //        _gattCharacteristicSender.WriteType = GattWriteType.NoResponse;
        //        _gattCharacteristicSender.SetValue(senderBytes);
        //        _gatt.WriteCharacteristic(_gattCharacteristicSender);


        //    }
        //    int timeout = 5000;
        //    while (_callbacks.IsConnected() == false && timeout > 0)
        //    {
        //        timeout -= 100;
        //        Task.Delay(100).Wait();
        //    }



        //}





    }
}
