using Android.App;
using Android.Bluetooth;
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

namespace TempaApp.Droid
{
    public class BleGattCallbacks : BluetoothGattCallback
    {
        public CancellationTokenSource ConnectionCancellationTokenSource = null;
        public CancellationTokenSource SendDataCancellationTokenSource = null;
        public CancellationTokenSource DescriptorWriteCancellationTokenSource = null;
        private BluetoothDevice mDevice;        
        public BluetoothGatt mGatt;
        private bool mConnected = false;
        private bool mSendOk = false;
        private bool mWriteDescriptorOk = false;
        public int MTU=23;
        //private bool mtuchanged = false;
        private bool characteristicWriteOK = false;
        //private List<byte> receiveBuffer = new List<byte>();
        private int requestMTUSize = 247;

        public event EventHandler<DataReceivedEventArgs> DataReceivedFromCallback;
        public event EventHandler<bool> ConnectedFromCallback;
        public event EventHandler<bool> DisconnectedFromCallback;
        //BLE_COMMAND_SIZE 4;
        //#define BLE_RESPONSE_SIZE    4
        //#define BLE_DATA_BUFFER_SIZE    4096
        public class BleData
        {
            public int status = 0;
            public byte[] command = new byte[3];
            // public byte[] response = new byte[3];
            public UInt16 dataLength;
            // public byte[] data = new byte[4096];
            public List<byte> data = new List<byte>();
            public UInt16 crc16;
            public bool dataReady = false;
            public bool dataCancelled = false;
        };

        public class DataReceivedEventArgs : EventArgs
        {
            public byte[] Data { get; set; }
        }


        BleData bleData = new BleData();

        public BleGattCallbacks (BluetoothDevice device)
        {
            
            mDevice = device;
//            ConnectionCancellationTokenSource = new CancellationTokenSource();
//            SendDataCancellationTokenSource = new CancellationTokenSource();
//            DescriptorWriteCancellationTokenSource = new CancellationTokenSource ();
        }

        ~BleGattCallbacks ()
        {
            mConnected = false;
            MTU = 23;
            mGatt?.Close();
            mGatt = null;
        }

        public override void OnConnectionStateChange(BluetoothGatt gatt, [GeneratedEnum] GattStatus status, [GeneratedEnum] ProfileState newState)
        {
            base.OnConnectionStateChange(gatt, status, newState);

            if (gatt == null)
                return;

            if (!gatt.Device.Address.Equals(mDevice.Address))
            {
                return;
            }

            if (status != GattStatus.Success)
            {

                mConnected = false;
                DisconnectedFromCallback?.Invoke(this, mConnected);
                gatt?.Close();
                mGatt = null;
                return;    

            }
     

            switch (newState)
            {
                case ProfileState.Connected:
                    mGatt = gatt;
                    mGatt?.DiscoverServices();
                    break;

                case ProfileState.Disconnected:
                    mConnected = false;
                    MTU = 23;
                    gatt?.Close();
                    mGatt = null;
                    DisconnectedFromCallback?.Invoke(this, mConnected);
                    break;                    
                default:
                    break;
            }
            
        }

        public override void OnServicesDiscovered(BluetoothGatt gatt, [GeneratedEnum] GattStatus status)
        {
            base.OnServicesDiscovered(gatt, status);
            if (gatt == null)
                return;

            if (!gatt.Device.Address.Equals(mDevice.Address))
            {
                return;
            }

            if (status == GattStatus.Success)
            {
                mConnected = true;                
                mGatt.RequestMtu(requestMTUSize);
                ConnectionCancellationTokenSource?.Cancel();
                ConnectedFromCallback?.Invoke(this, mConnected);
            }
            else
            {
                mConnected = false;
                MTU = 23;
                gatt?.Close();
                mGatt = null;
                DisconnectedFromCallback?.Invoke(this, mConnected);
            }
            

        }

        public override void OnMtuChanged(BluetoothGatt gatt, int mtu, [GeneratedEnum] GattStatus status)
        {
            base.OnMtuChanged(gatt, mtu, status);
            if (gatt == null)
                return;

            if (!gatt.Device.Address.Equals(mDevice.Address))
            {
                return;
            }

            if (status == GattStatus.Success)
            {
                MTU = mtu;               
            }

        }

        public bool IsConnected()
        {
            return mConnected;
        }

        public void SetConnect(bool val)
        {
            mConnected = val;
        }

        public override void OnCharacteristicChanged(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic)
        {
            base.OnCharacteristicChanged(gatt, characteristic);
            if (gatt == null)
                return;

            if (!gatt.Device.Address.Equals(mDevice.Address))
            {
                return;
            }

            var buff = characteristic.GetValue();
            OnDataReceived(buff);

        }

        public override void OnCharacteristicWrite(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic, [GeneratedEnum] GattStatus status)
        {
            base.OnCharacteristicWrite(gatt, characteristic, status);
            if (gatt == null)
                return;

            if (!gatt.Device.Address.Equals(mDevice.Address))
            {
                return;
            }
            if (status != GattStatus.Success)
            {
                mSendOk = false;                
            }            
            else
            {
                mSendOk = true;
            }            
            SendDataCancellationTokenSource?.Cancel();
        }


        public override void OnCharacteristicRead(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic, [GeneratedEnum] GattStatus status)
        {
            base.OnCharacteristicRead(gatt, characteristic, status);
            if (gatt == null)
                return;
            if (!gatt.Device.Address.Equals(mDevice.Address))
            {
                return;
            }
            if (status != GattStatus.Success)
            {
                return;
            }

            var buff = characteristic.GetValue();
            OnDataReceived(buff);            
            
        }

        public override void OnDescriptorWrite(BluetoothGatt gatt, BluetoothGattDescriptor descriptor, [GeneratedEnum] GattStatus status)
        {
            base.OnDescriptorWrite(gatt, descriptor, status);
            if (gatt == null)
                return;
            if (!gatt.Device.Address.Equals(mDevice.Address))
            {
                return;
            }
            if (status == GattStatus.Success)
            {
                mWriteDescriptorOk = true;
            }
            else
            {
                mWriteDescriptorOk = false;
            }
            DescriptorWriteCancellationTokenSource?.Cancel();   
            

        }

        public bool IsDescriptorWriteOk ()
        {
             return mWriteDescriptorOk; 
        }

        public void setDescriptorWriteOk (bool val) { mWriteDescriptorOk = val; }

        public bool IsSendOk()
        {
            return mSendOk;
        }

        public void ClearSendOk()
        { mSendOk = false; }

        protected virtual void OnDataReceived(byte[] data)
        {
            DataReceivedFromCallback?.Invoke(this,new DataReceivedEventArgs { Data = data});
            
        }

        public bool GetConnectionStatus()
        {
            var ctx = Android.App.Application.Context;

            if (!ctx.PackageManager.HasSystemFeature(PackageManager.FeatureBluetoothLe))
            {

                return false;
            }

            var _bluetoothManager = (BluetoothManager)ctx.GetSystemService(Context.BluetoothService);

            var status = _bluetoothManager.GetConnectionState(mDevice, ProfileType.Gatt);
            if (status == ProfileState.Connected)
            {
                if (!mConnected)
                {
                    mConnected = true;
                    ConnectedFromCallback?.Invoke(this, mConnected);
                }
            }
            else if (status == ProfileState.Disconnected)
            {
                if (mConnected)
                {
                    mGatt?.Close();
                    mGatt = null;
                    mConnected = false;
                    DisconnectedFromCallback?.Invoke(this, mConnected);
                }


            }
            return mConnected;
            
            
            
        }





    }
}