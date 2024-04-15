using System;
using System.Globalization;
using TempaApp.Models;
using TempaApp.MyResources;
using TempaApp.Services;
using TempaApp.Views;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using TempaApp.Background;
//using static TempaApp.Background.HandleBleReceivedLongRunningTaskMessages;
using TempaApp.Interface;

namespace TempaApp
{
    public partial class App : Application
    {
        public CultureInfo ci;
        HandleBleReceivedLongRunningTaskMessages handleBleReceivedLongRunningTaskMessages = new HandleBleReceivedLongRunningTaskMessages();
        //public IBle myBle;
        //private static bleDevice myBleDevice;
        //public static bleDevice MyBleDevice
        //{
        //    get
        //    {
        //        if (myBleDevice == null)
        //        {
        //            myBleDevice = new bleDevice();
        //        }
        //        return myBleDevice;
        //    }
        //}

        private static bleCommunication myBle;
        public static bleCommunication MyBle
        {
            get
            {
                if  (myBle == null)
                {
                    myBle = new bleCommunication();
                }
                return myBle;
            }
        }

        private static bool _isBleBackgroundAlive = false;
        public static bool IsBleBackgroundAlive
        {
            get { return _isBleBackgroundAlive; }
            set { _isBleBackgroundAlive = value; }
        }

        public App()
        {
            //myBle = DependencyService.Get<IBle>();
            //myBle.Init();
            LocalizationResourceManager.Current.Init(AppResources.ResourceManager);
            ci = CultureInfo.CurrentCulture;


            if (String.Equals(ci.ToString(), "tr-TR") || String.Equals(ci.ToString(), "en-US"))
            {
                LocalizationResourceManager.Current.CurrentCulture = ci;
            }
            else
            {
                ci = new CultureInfo("en-US");
                LocalizationResourceManager.Current.CurrentCulture = ci;
            }
            
            
            InitializeComponent();
            MainPage = new AppShell();
            
        }

        protected override async void OnStart()
        {
            //MyBleDevice.CheckPhoneBluetoothOn();

//            var _devices = myBle.GetBondedDevices();
            //foreach (var device in _devices)
            //{
            //    if ( device.IndexOf("TEMPA") != -1 )
            //    {
            //        if ( myBle.ConnectSpecificDevice(device, bleServices.SERVICE_UUID, bleServices.CHARACTERISTIC_UUID_SENDER, bleServices.CHARACTERISTIC_UUID_RESPONSE, bleServices.CHARACTERISTIC_UPDATE_NOTIFICATION_DESCRIPTOR_UUID) == true )
            //        {
                        
            //        }
            //    }
            //}

            ////LoadPersistedValues();

            //var myIndicators = await Database.GetIndicatorsAsync();
            //if (myIndicators != null && myIndicators.Count > 0)
            //{

                StartIndicatorBackgroudCommunication();
            //}
            //else
            //{

            //    //var message = new BleStartLongRunningMessage();
            //    //MessagingCenter.Send(message, "BleStartLongRunningMessage");

            //    //var HandleBleReceivedLongRunningTaskMessages = new HandleBleReceivedLongRunningTaskMessages();
            //}
        }

        protected override void OnSleep()
        {
            StopIndicatorBackgroudCommunication();
        }

        protected override void OnResume()
        {
            StartIndicatorBackgroudCommunication();
        }

        

        public static void StartIndicatorBackgroudCommunication()
        {
            if (App.IsBleBackgroundAlive == false)
            {
                var message = new HandleBleReceivedLongRunningTaskMessages.BleStartLongRunningMessage();
                MessagingCenter.Send(message, "BleStartLongRunningMessage");
            }
            
            //HandleBleReceivedLongRunningTaskMessages.HandleLongRunningTaskMessages();

        }

        public static void StopIndicatorBackgroudCommunication()
        {

            if (App.IsBleBackgroundAlive)
            {
                var message = new HandleBleReceivedLongRunningTaskMessages.BleStopLongRunningMessage();
                MessagingCenter.Send(message, "BleStopLongRunningMessage");
            }
        }

    }
}
