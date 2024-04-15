using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using TempaApp.iOS.Background;
using UIKit;
using Xamarin.Forms;
using static TempaApp.Background.HandleBleReceivedLongRunningTaskMessages;

namespace TempaApp.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        IOS_BleLongRunningTask bleLongRunningTask;
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new App());

            WireUpBleLongRunningTask();

            return base.FinishedLaunching(app, options);
        }
        void WireUpBleLongRunningTask()
        {
            MessagingCenter.Subscribe<BleStartLongRunningMessage>(this, "BleStartLongRunningMessage", async message =>
            {
                bleLongRunningTask = new IOS_BleLongRunningTask();
                await bleLongRunningTask.Start();
            });

            MessagingCenter.Subscribe<BleStopLongRunningMessage>(this, "BleStopLongRunningMessage", message =>
            {
                bleLongRunningTask.Stop();
            });
        }
    }
}
