using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using TempaApp.Background;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using static TempaApp.Background.BleBackgroundTask;
using static TempaApp.Background.HandleBleReceivedLongRunningTaskMessages;

namespace TempaApp.Droid.Background
{
	[Service]
	public class BleLongRunningTaskService : Service
	{
		CancellationTokenSource _cts;

		public override IBinder OnBind(Intent intent)
		{
			return null;
		}

		public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
		{
			_cts = new CancellationTokenSource();

			Task.Run(() => {
				try
				{
					//INVOKE THE SHARED CODE
					var bleBackground = new BleBackgroundTask();
					bleBackground.RunBleBackground(_cts.Token).Wait();
				}
				catch (Android.OS.OperationCanceledException)   //Android.OS.OperationCanceledException
				{
					//if (ex.InnerException != null)
     //               {

     //               }

				}

				finally
				{
					if (_cts.IsCancellationRequested)
					{
						//App.IsBleBackgroundAlive = false;
                        //var message1 = new BleTickedMessage();
                        //Device.BeginInvokeOnMainThread(() =>
                        //	MessagingCenter.Send(message1, "BleTickedMessage")
                        //);


                    }
					//var message = new BleStartLongRunningMessage();
					//MessagingCenter.Send(message, "BleStartLongRunningMessage");

					//var HandleBleReceivedLongRunningTaskMessages = new HandleBleReceivedLongRunningTaskMessages();
					
					App.IsBleBackgroundAlive = false;
					var message = new BleCancelledMessage();

					Device.BeginInvokeOnMainThread(() =>
					MessagingCenter.Send(message, "BleCancelledMessage")
					);
				}

			}, _cts.Token);

			return StartCommandResult.Sticky;
		}

		public override void OnDestroy()
		{
			if (_cts != null)
			{

				//_cts.Token.ThrowIfCancellationRequested();
				_cts.Cancel();
			}
			base.OnDestroy();
		}
		
	}
}