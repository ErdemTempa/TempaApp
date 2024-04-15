using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MvvmHelpers;
using MvvmHelpers.Commands;
using TempaApp.Models;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TempaApp.ViewModels
{
    public class mainPageViewModel : BaseViewModel
    {
        private string weightVal;
        public string WeightVal
        {
            get { return weightVal; }
            set { SetProperty(ref weightVal, value); }
        }

        private string infoVal;
        public string InfoVal
        {
            get { return infoVal; }
            set { SetProperty (ref infoVal, value); }
        }

        private string unitVal;
        public string UnitVal
        {
            get { return unitVal; }
            set { SetProperty(ref unitVal, value); }
        }

        private string connectionIcon;
        public string ConnectionIcon
        {
            get => connectionIcon;
            set { SetProperty(ref connectionIcon, value); }
        }

        private string alarmIcon;
        public string AlarmIcon
        {
            get => alarmIcon;
            set { SetProperty(ref alarmIcon, value); }
        }

        public AsyncCommand<object> SendSetZeroCommand { get; }
        public AsyncCommand<object> SendSetTareCommand { get; }
        public AsyncCommand<object> SendTotParCommand { get; }
        public AsyncCommand<object> SendBlockCommand { get; }
        public AsyncCommand<object> SendMenuCommand { get; }
        public AsyncCommand<object> SendLeftArrowCommand { get; }
        public AsyncCommand<object> SendUpArrowCommand { get; }
        public AsyncCommand<object> SendOkCommand { get; }

        public AsyncCommand<object> SendLoadCommand { get; }
        public AsyncCommand<object> SendUnloadCommand { get; }
        public AsyncCommand<object> TotParKeyPressedCommand { get; }
        public AsyncCommand<object> TotParKeyReleasedCommand { get; }
        public AsyncCommand<object> CheckBackgroundCommand { get; }

        int totparKeyTime = 0;



        public mainPageViewModel()
        {
            WeightVal = "0";
            InfoVal = "0";
            UnitVal = "   ";
            Device.StartTimer(System.TimeSpan.FromSeconds(0.1), mainPageUpdateUI);
            SendSetTareCommand = new AsyncCommand<object>(SetTare);
            SendSetZeroCommand = new AsyncCommand<object>(SetZero);
            SendTotParCommand = new AsyncCommand<object>(SetTotParKey);
            SendMenuCommand = new AsyncCommand<object>(SetMenuKey);
            SendLeftArrowCommand = new AsyncCommand<object>(SetLeftArrowKey);
            SendUpArrowCommand = new AsyncCommand<object>(SetUpArrowKey);
            SendOkCommand = new AsyncCommand<object>(SetOkKey);
            SendBlockCommand = new AsyncCommand<object>(SetBlockKey);
            SendLoadCommand = new AsyncCommand<object>(SetLoadKey);
            SendUnloadCommand = new AsyncCommand<object>(SetUnloadKey);
            TotParKeyPressedCommand = new AsyncCommand<object>(TotParKeyPressed);
            TotParKeyReleasedCommand = new AsyncCommand<object>(TotParKeyReleased);
            CheckBackgroundCommand = new AsyncCommand<object>(CheckBackgroundTask);

            // CheckIndicator();
        }

        private Task CheckBackgroundTask(object arg)
        {
            if (App.IsBleBackgroundAlive==false)
            {
                App.StartIndicatorBackgroudCommunication();
            }
            return Task.CompletedTask;
        }

        private async Task TotParKeyReleased(object arg)
        {
            if (totparKeyTime == 0)
            {
                await App.MyBle.SetZeroAsync();
            }
            else
            {
                await App.MyBle.SendKey("KEY_TOTPAR");
            }
            totparKeyTime = 0;

        }

        private Task TotParKeyPressed(object arg)
        {
            totparKeyTime = 3000;
            Vibration.Vibrate(100);
            return Task.CompletedTask;
        }

        private async Task SetUnloadKey(object arg)
        {
            Vibration.Vibrate(100);
            await App.MyBle.StartUnLoadAsync();
        }

        private async Task SetLoadKey(object arg)
        {
            Vibration.Vibrate(100);
            await App.MyBle.StartLoadAsync();
        }

        private async Task SetTotParKey(object arg)
        {
            Vibration.Vibrate(100);
            await App.MyBle.SendKey("KEY_TOTPAR");
        }

        private async Task SetMenuKey(object arg)
        {
            Vibration.Vibrate(100);
            await App.MyBle.SendKey("KEY_MENU");
        }

        private async Task SetLeftArrowKey(object arg)
        {
            Vibration.Vibrate(100);
            await App.MyBle.SendKey("KEY_LEFTARROW");
        }

        private async Task SetUpArrowKey(object arg)
        {
            Vibration.Vibrate(100);
            await App.MyBle.SendKey("KEY_UPARROW");
        }

        private async Task SetOkKey(object arg)
        {
            Vibration.Vibrate(100);
            await App.MyBle.SendKey("KEY_OK");
        }

        private async Task SetBlockKey(object arg)
        {
            Vibration.Vibrate(100);
            await App.MyBle.SendKey("KEY_BLOCK");
        }



        private async Task SetZero(object arg)
        {
            Vibration.Vibrate(100);
            await App.MyBle.SetZeroAsync();

        }

        private async Task SetTare(object arg)
        {
            Vibration.Vibrate(100);
            await App.MyBle.GetDisplayData();
        }

        

        private bool mainPageUpdateUI()
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                //int i;
                //i = int.Parse(this.WeightVal);
                //i++;
                //this.WeightVal = i.ToString();
                if (totparKeyTime >= 100)
                {
                    totparKeyTime -= 100;
                    if (totparKeyTime <= 0)
                    {
                        Vibration.Vibrate(100);
                        totparKeyTime = 0;
                    }
                }
                else
                {
                    totparKeyTime = 0;
                }

                if (App.MyBle.Connection)
                {
                    this.ConnectionIcon = "connect_256.png";
                }
                else
                {
                    this.ConnectionIcon = "disconnect_256.png";
                }

                if (App.MyBle.Alarm)
                {
                    this.AlarmIcon = "alarmRed_64px.png";
                }
                else
                {
                    this.AlarmIcon = "alarmRedNul_64px.png";
                }
                //if (App.MyBle.rxData.Count > 0)
                //{
                //    this.WeightVal = Encoding.ASCII.GetString(App.MyBle.rxData.GetRange(0, App.MyBle.rxData.Count).ToArray());
                //    //this.WeightVal = App.MyBle.rxData.GetRange(0, App.MyBle.rxData.Count)
                //    App.MyBle.rxData.Clear();
                //}
                    if (App.MyBle.weightData.Count > 0)
                {
                    if (Encoding.ASCII.GetString(App.MyBle.weightData.GetRange(0, App.MyBle.weightData.Count).ToArray()) != this.WeightVal)
                    {

                        this.WeightVal = Encoding.ASCII.GetString(App.MyBle.weightData.GetRange(0, App.MyBle.weightData.Count).ToArray());
                        this.UnitVal = Encoding.ASCII.GetString(App.MyBle.unitData.GetRange(0, App.MyBle.unitData.Count).ToArray());
                        //string temp = Encoding.ASCII.GetString(App.MyBle.weightData.GetRange(0, App.MyBle.weightData.Count).ToArray());
                        //float num;
                        //if ( float.TryParse(temp, out num) )
                        //{
                        //    this.WeightVal = temp + " kg";
                        //}
                        //else
                        //{
                        //    this.WeightVal = temp + "    ";
                        //}

                    }

                }

                if (App.MyBle.infoData.Count > 0)
                {
                    if (Encoding.ASCII.GetString(App.MyBle.infoData.GetRange(0, App.MyBle.infoData.Count).ToArray()) != this.InfoVal)
                    {
                        this.InfoVal = Encoding.ASCII.GetString(App.MyBle.infoData.GetRange(0, App.MyBle.infoData.Count).ToArray());
                    }
                }
                await Task.Delay(2);
            });            
            return true;
        }


        //async Task CheckIndicator()
        //{
        //    var indicator = await Database.GetCurrentIndicatorAsync();

        //    if (indicator == null)
        //    {
        //        var bleDevices = App.MyBleDevice.GetPairedDevices();

        //        if ((bleDevices == null) || bleDevices.Count == 0)
        //        {
        //            //var message = LocalizationResourceManager.Current.GetValue("No Mikrosub dive computer found in your current paired device list!") + "\r\n" +
        //            //    LocalizationResourceManager.Current.GetValue("Please follow the steps below") + "\r\n" +
        //            //    LocalizationResourceManager.Current.GetValue("1- Go to connectivity settings from your dive computer") + "\r\n" +
        //            //    LocalizationResourceManager.Current.GetValue("2- Make sure airplane mode is OFF") + "\r\n" +
        //            //    LocalizationResourceManager.Current.GetValue("3- Open pairing settings") + "\r\n" +
        //            //    LocalizationResourceManager.Current.GetValue("4- Turn on your mobile device bluetooth ON") + "\r\n" +
        //            //    LocalizationResourceManager.Current.GetValue("5- Scan bluetooth devices") + "\r\n" +
        //            //    LocalizationResourceManager.Current.GetValue("6- Find your dive computer and pair with your device");
        //            //LocalizationResourceManager.Current.GetValue("7- Restart this app again");
        //            //await AppShell.Current.DisplayAlert(LocalizationResourceManager.Current.GetValue("Warning!"), message,
        //            //LocalizationResourceManager.Current.GetValue("Ok"));
        //            //var closer = DependencyService.Get<ICloseApplication>();
        //            //closer?.closeApplication();

        //        }
        //        else if (bleDevices.Count > 1)
        //        {
        //            //var message = LocalizationResourceManager.Current.GetValue("More than one paired Mikrosub dive computer in your mobile device list!") + "\r\n" +
        //            //    LocalizationResourceManager.Current.GetValue("Please leave only one device in the paired device list and restart this app again");
        //            //await AppShell.Current.DisplayAlert(LocalizationResourceManager.Current.GetValue("Warning!"), message,
        //            //LocalizationResourceManager.Current.GetValue("Ok"));
        //            //var closer = DependencyService.Get<ICloseApplication>();
        //            //closer?.closeApplication();
        //        }
        //        else
        //        {
        //            var newIndıcator = new Indicator();
        //            newIndıcator.IsCurrentIndicator = true;
        //            newIndıcator.Name = bleDevices[0].Name;
        //            newIndıcator.Guid = bleDevices[0].Device.Id.ToString();
        //            newIndıcator.SoftwareVersion = null;

        //            await Database.SaveIndicatorAsync(newIndıcator);


        //                if (App.IsBleBackgroundAlive)
        //            {
        //                App.StopIndicatorBackgroudCommunication();
        //                while (App.IsBleBackgroundAlive)
        //                {
        //                    await Task.Delay(500);
        //                }

        //            }
        //            App.StartIndicatorBackgroudCommunication();
        //        }
        //        //var message = LocalizationResourceManager.Current.GetValue("New firmware will be downloaded on your dive computer") + "\r\n" +
        //        //    LocalizationResourceManager.Current.GetValue("This will take time") + LocalizationResourceManager.Current.GetValue("Until that moment you can use your dive computer") + "\r\n" +
        //        //    LocalizationResourceManager.Current.GetValue("Keep your mobile device bluetooth is ON") + "\r\n" +
        //        //    LocalizationResourceManager.Current.GetValue("Please, allow this application run on background") + "\r\n" +
        //        //    LocalizationResourceManager.Current.GetValue("You will get notification after download completed") + "\r\n" +
        //        //    LocalizationResourceManager.Current.GetValue("Do you want to download and update your device with selected firmware?");
        //        //if (await AppShell.Current.DisplayAlert(LocalizationResourceManager.Current.GetValue("Warning!"), message,
        //        //LocalizationResourceManager.Current.GetValue("Ok"),
        //        //LocalizationResourceManager.Current.GetValue("Cancel")))
        //        //{

        //        //}
        //    }
        //}
    }


   
}
