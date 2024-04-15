using Foundation;
using TempaApp.Background;
using static TempaApp.Background.BleBackgroundTask;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Forms;

namespace TempaApp.iOS.Background
{
    public class IOS_BleLongRunningTask
    {
        nint _taskId;
        CancellationTokenSource _cts;

        public async Task Start()
        {
            _cts = new CancellationTokenSource();
            _taskId = UIApplication.SharedApplication.BeginBackgroundTask("BleLongRunningTask", OnExpiration);

            try
            {
                var bleBackground = new BleBackgroundTask();
                await bleBackground.RunBleBackground(_cts.Token);

            }
            catch (OperationCanceledException)
            {

            }
            finally
            {
                if (_cts.IsCancellationRequested)
                {
                    //  var message = new BleCancelledMessage();
                    //  Device.BeginInvokeOnMainThread(
                    //      () => MessagingCenter.Send(message, "BleCancelledMessage")
                    //  );
                }
            }
            UIApplication.SharedApplication.EndBackgroundTask(_taskId);
        }

        public void Stop()
        {
            _cts.Cancel();
        }

        void OnExpiration()
        {
            _cts.Cancel();
        }
    }
}