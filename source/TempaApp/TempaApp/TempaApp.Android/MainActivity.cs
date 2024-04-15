using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Android;
using AndroidX.Core.Content;
using AndroidX.Core.App;
using Xamarin.Forms;
using static TempaApp.Background.HandleBleReceivedLongRunningTaskMessages;
using TempaApp.Droid.Background;
using Android.Content;
using Android.Bluetooth;
using System.Collections.Generic;
using Xamarin.Forms.Platform.Android;
using Google.Android.Material.BottomNavigation;
using AndroidX.Core.View;

namespace TempaApp.Droid
{
    

    [Activity(Label = "TempaApp", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize , ScreenOrientation = ScreenOrientation.Landscape)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {

            //Burası Pluging.BLE nin calismasi icin konuldu
            var locationPermissions = new[]
{
                Manifest.Permission.AccessCoarseLocation,
                Manifest.Permission.AccessFineLocation
            };
            // check if the app has permission to access coarse location
            var coarseLocationPermissionGranted =
                ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessCoarseLocation);
            // check if the app has permission to access fine location
            var fineLocationPermissionGranted =
                 ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation);
            // if either is denied permission, request permission from the user
            const int locationPermissionsRequestCode = 1000;
            if (coarseLocationPermissionGranted == Permission.Denied ||
                fineLocationPermissionGranted == Permission.Denied)
            {
                ActivityCompat.RequestPermissions(this, locationPermissions,
                locationPermissionsRequestCode);
            }
            ///////////////////////////////////////////////////////
            ///
            var windowInsetsController = WindowCompat.GetInsetsController(Window, Window.DecorView);
            windowInsetsController.SystemBarsBehavior = WindowInsetsControllerCompat.BehaviorShowTransientBarsBySwipe;
            windowInsetsController.Hide(WindowInsetsCompat.Type.SystemBars());
            base.OnCreate(savedInstanceState);


            //BluetoothAdapter mBluetoothAdapter = BluetoothAdapter.DefaultAdapter;
            //var pairedDevices = mBluetoothAdapter.BondedDevices;

            //List<string> list = new List<string>();
            //foreach (BluetoothDevice device in pairedDevices)
            //    list.Add(device.Name);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());

            WireUpBleLongRunningTask();
        }

        //public System.Collections.Generic.ICollection<Android.Bluetooth.BluetoothDevice>? BondedDevices
        //{
        //    [Android.Runtime.Register("getBondedDevices", "()Ljava/util/Set;", "")]
        //    [Android.Runtime.RequiresPermission("android.permission.BLUETOOTH_CONNECT")]
        //    get;
        //}

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        void WireUpBleLongRunningTask()
        {
            MessagingCenter.Subscribe<BleStartLongRunningMessage>(this, "BleStartLongRunningMessage", message =>
            {
                var intent = new Intent(this, typeof(BleLongRunningTaskService));

                //if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
                //{
                //    //MainActivity.Instance.StartForegroundService(intent);
                //    StartForegroundService(intent);
                //}
                //else
                //{
                //MainActivity.Instance.StartService(intent);
                //if (App.IsBleBackgroundAlive == false)
                //{
                    StartService(intent);
                //}
                //}
                
            });


            MessagingCenter.Subscribe<BleStopLongRunningMessage>(this, "BleStopLongRunningMessage", message =>
            {
                var intent = new Intent(this, typeof(BleLongRunningTaskService));
                StopService(intent);
            });


        }
    }

    
    
}