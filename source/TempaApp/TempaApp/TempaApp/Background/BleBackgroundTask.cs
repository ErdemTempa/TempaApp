﻿
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using TempaApp.Models;
using System.IO;
using Xamarin.CommunityToolkit.Helpers;
//using static TempaApp.Background.HandleBleReceivedLongRunningTaskMessages;

namespace TempaApp.Background
{
    public class BleBackgroundTask
    {
        string mcuFirmwareFilePath = null;
        string assetsFilePath = null;
        int comErrorCount = 0;
        const int errorMaxSize = 50;
        byte[] mcuNewFirmware = Array.Empty<byte>();
        byte[] assetsNewFirmware = Array.Empty<byte>();

        public async Task RunBleBackground(CancellationToken token)
        {
            

            await Task.Run(async () =>
            {
                int count = 0;
                await Task.Delay(5);
                

                int loopMs = 100;
                var indBle = new BleFoundDevice();
                //int clockSyncTime = 60*1000*10; //10dk da bir clock senkronizasyon yapilacak
                //int clockSyncCnt = 0;

                long tickCount = 0;
                App.IsBleBackgroundAlive = true;




                while (true)
                {
                    //token.ThrowIfCancellationRequested();
                    if (token.IsCancellationRequested)
                    {
                        //var messageCancelled = new BleCancelledMessage();
                        //Device.BeginInvokeOnMainThread(() =>
                        //{
                        //    MessagingCenter.Send<BleCancelledMessage>(messageCancelled, "BleCancelledMessage");
                        //});

                        token.ThrowIfCancellationRequested();
                    }

                    if (App.MyBle.Connection == false)
                    {
                        App.MyBle.ConnectMyBleDevice();
                    }
                    else
                    {
                       if ( await App.MyBle.GetDisplayData() == false )
                        {
                            count++;
                            if (count > 5)
                            {
                                App.MyBle.RefreshConnection();
                                count = 0;
                            }
                        }
                       else
                        {
                            count = 0;
                        }

                    }

                    //if (App.MyBleDevice.IsConnect == false)
                    //{
                    //    if (await App.MyBleDevice.ConnectSpecificDeviceAsync(indicator.Guid) == true)
                    //    {
                    //        //var str = await App.MyBleDevice.GetSoftwareVersion();
                    //        //if (str != null)
                    //        //{
                    //        //    if (diveComputer.SoftwareVersion != str)
                    //        //    {
                    //        //        diveComputer.SoftwareVersion = str;
                    //        //        await Database.SaveDiveComputerAsync(diveComputer);
                    //        //    }
                    //        //    App.MyBleDevice.SoftwareVersion = diveComputer.SoftwareVersion;
                    //        //}

                    //    }
                    //}
                    //else
                    //{
                    //    //if (App.MyBleDevice.DoFirmwareUpdate)
                    //    //{
                    //    //    //await FirmwareUpdateProcess();
                    //    //}
                    //    //else
                    //    //{
                    //    //    //if (diveComputer.SoftwareVersion == null)
                    //    //    //{
                    //    //    //    var str = await App.MyBleDevice.GetSoftwareVersion();
                    //    //    //    if (str != null)
                    //    //    //    {
                    //    //    //        diveComputer.SoftwareVersion = str;
                    //    //    //        App.MyBleDevice.SoftwareVersion = diveComputer.SoftwareVersion;
                    //    //    //    }
                    //    //    //}
                    //    //    //else
                    //    //    //{
                    //    //    //    App.MyBleDevice.SoftwareVersion = diveComputer.SoftwareVersion;
                    //    //    //}

                    //    //    //if (clockSyncCnt != 0)
                    //    //    //{
                    //    //    //    clockSyncCnt -= loopMs;
                    //    //    //}

                    //    //    //if (clockSyncCnt == 0)
                    //    //    //{
                    //    //    //    if (await App.MyBleDevice.GetDate())
                    //    //    //    {
                    //    //    //        if (await App.MyBleDevice.GetClock())
                    //    //    //        {
                    //    //    //            DateTime deviceTime = new DateTime(App.MyBleDevice.Date.Year, App.MyBleDevice.Date.Month, App.MyBleDevice.Date.Day,
                    //    //    //                App.MyBleDevice.Time.Hour, App.MyBleDevice.Time.Minute, App.MyBleDevice.Time.Second);

                    //    //    //            var timeDif = deviceTime.Subtract(DateTime.Now);

                    //    //    //            if (timeDif.TotalMinutes > 2 || timeDif.TotalMinutes < -2)
                    //    //    //            {
                    //    //    //                DateTime currentTime = DateTime.Now;
                    //    //    //                if (await App.MyBleDevice.SetDate(currentTime))
                    //    //    //                {
                    //    //    //                    if (await App.MyBleDevice.SetClock(currentTime))
                    //    //    //                    {
                    //    //    //                        clockSyncCnt = clockSyncTime;
                    //    //    //                    }
                    //    //    //                }

                    //    //    //            }
                    //    //    //            else
                    //    //    //            {
                    //    //    //                clockSyncCnt = clockSyncTime;
                    //    //    //            }
                    //    //    //        }
                    //    //    //    }
                    //    //    //}

                    //    //    //mcuFirmwareFilePath = null;
                    //    //    //assetsFilePath = null;
                    //    //    await Task.Delay(loopMs);

                    //    //}
                    //}



                    //tickCount++;
                    //var message = new BleTickedMessage
                    //{
                    //    Message = tickCount.ToString()
                    //};

                    //Device.BeginInvokeOnMainThread(() =>
                    //{
                    //    MessagingCenter.Send<BleTickedMessage>(message, "BleTickedMessage");
                    //});
                    
                    //App.MyBleDevice.CheckWeightDataAndUpdateScreen();
                    await Task.Delay(loopMs);
                }



            }, token);

            App.IsBleBackgroundAlive = false;
        }

        

    //    private async Task FirmwareUpdateProcess()
    //    {
    //        double d;
    //        int i;

    //        if (App.MyBleDevice.DoFirmwareUpdate)
    //        {
    //            //App.LoadPersistedValues();

    //            if (mcuFirmwareFilePath == null)
    //            {

    //                    mcuFirmwareFilePath = null;

    //                mcuFirmwareFilePath = FirmwareInfoQuery.GetMcuFirmwareFilePath(App.MyBleDevice.FirmwareUpdateSelectedVersion);
    //                if (mcuFirmwareFilePath == null)
    //                {
    //                    mcuNewFirmware = Array.Empty<byte>();
    //                    App.MyBleDevice.FirmwareUpdateTotalPackCount = 0;
    //                    App.MyBleDevice.DoFirmwareUpdate = false;
    //                    App.MyBleDevice.FirmwareUpdateStatus = bleDevice.FirmwareUpdateStatusEnum.FIRMWARE_UPDATE_IDLE;
    //                    App.MyBleDevice.FirmwareUpdatePackNumber = 0;
    //                    App.MyBleDevice.AssetsUpdateTotalPackCount = 0;
    //                    App.MyBleDevice.AssetsUpdatePackNumber = 0;
    //                    App.MyBleDevice.FirmwareUpdateSelectedVersion = "";
    //                    App.SavePersistedValues();
    //                    return;
    //                }
    //                else
    //                {
    //                    try
    //                    {
    //                        using (FileStream fsSource = new FileStream(mcuFirmwareFilePath, FileMode.Open, FileAccess.Read))
    //                        {
    //                            mcuNewFirmware = Array.Empty<byte>();
    //                            mcuNewFirmware = new byte[fsSource.Length];
    //                            int numBytesToRead = (int)fsSource.Length;
    //                            int numBytesRead = 0;
    //                            while (numBytesToRead > 0)
    //                            {
    //                                // Read may return anything from 0 to numBytesToRead.
    //                                int n = fsSource.Read(mcuNewFirmware, numBytesRead, numBytesToRead);

    //                                // Break when the end of the file is reached.
    //                                if (n == 0)
    //                                {
    //                                    break;
    //                                }

    //                                numBytesRead += n;
    //                                numBytesToRead -= n;
    //                            }
    //                        }
    //                    }
    //                    catch (Exception ex)
    //                    {
    //                        mcuFirmwareFilePath = null;
    //                        App.MyBleDevice.FirmwareUpdateTotalPackCount = 0;
    //                        App.MyBleDevice.DoFirmwareUpdate = false;
    //                        App.MyBleDevice.FirmwareUpdateStatus = bleDevice.FirmwareUpdateStatusEnum.FIRMWARE_UPDATE_IDLE;
    //                        App.MyBleDevice.FirmwareUpdatePackNumber = 0;
    //                        App.MyBleDevice.AssetsUpdateTotalPackCount = 0;
    //                        App.MyBleDevice.AssetsUpdatePackNumber = 0;
    //                        App.MyBleDevice.FirmwareUpdateSelectedVersion = "";
    //                        App.SavePersistedValues();
    //                        return;
    //                    }
    //                }
    //            }

    //            if (assetsFilePath == null)
    //            {
    //                assetsFilePath = null;
    //                assetsFilePath = FirmwareInfoQuery.GetAssetsFilePath(App.MyBleDevice.FirmwareUpdateSelectedVersion);
    //                if (assetsFilePath == null)
    //                {
    //                    assetsNewFirmware = Array.Empty<byte>();
    //                    App.MyBleDevice.FirmwareUpdateTotalPackCount = 0;
    //                    App.MyBleDevice.DoFirmwareUpdate = false;
    //                    App.MyBleDevice.FirmwareUpdateStatus = bleDevice.FirmwareUpdateStatusEnum.FIRMWARE_UPDATE_IDLE;
    //                    App.MyBleDevice.FirmwareUpdatePackNumber = 0;
    //                    App.MyBleDevice.AssetsUpdateTotalPackCount = 0;
    //                    App.MyBleDevice.AssetsUpdatePackNumber = 0;
    //                    App.MyBleDevice.FirmwareUpdateSelectedVersion = "";
    //                    App.SavePersistedValues();
    //                    return;
    //                }
    //                else
    //                {
    //                    try
    //                    {
    //                        using (FileStream fsSource = new FileStream(assetsFilePath, FileMode.Open, FileAccess.Read))
    //                        {
    //                            assetsNewFirmware = Array.Empty<byte>();
    //                            assetsNewFirmware = new byte[fsSource.Length];
    //                            int numBytesToRead = (int)fsSource.Length;
    //                            int numBytesRead = 0;
    //                            while (numBytesToRead > 0)
    //                            {
    //                                // Read may return anything from 0 to numBytesToRead.
    //                                int n = fsSource.Read(assetsNewFirmware, numBytesRead, numBytesToRead);

    //                                // Break when the end of the file is reached.
    //                                if (n == 0)
    //                                {
    //                                    break;
    //                                }

    //                                numBytesRead += n;
    //                                numBytesToRead -= n;
    //                            }
    //                        }
    //                    }
    //                    catch (Exception ex)
    //                    {
    //                        assetsFilePath = null;
    //                        App.MyBleDevice.FirmwareUpdateTotalPackCount = 0;
    //                        App.MyBleDevice.DoFirmwareUpdate = false;
    //                        App.MyBleDevice.FirmwareUpdateStatus = bleDevice.FirmwareUpdateStatusEnum.FIRMWARE_UPDATE_IDLE;
    //                        App.MyBleDevice.FirmwareUpdatePackNumber = 0;
    //                        App.MyBleDevice.AssetsUpdateTotalPackCount = 0;
    //                        App.MyBleDevice.AssetsUpdatePackNumber = 0;
    //                        App.MyBleDevice.FirmwareUpdateSelectedVersion = "";
    //                        App.SavePersistedValues();
    //                        return;
    //                    }

    //                    //App.MyBleDevice.FirmwareUpdateTotalPackCount = 0;
    //                    //App.MyBleDevice.FirmwareUpdateStatus = bleDevice.FirmwareUpdateStatusEnum.FIRMWARE_UPDATE_INIT_NEW_MCU_FW;
    //                    //App.MyBleDevice.FirmwareUpdatePackNumber = 0;
    //                    //App.MyBleDevice.AssetsUpdateTotalPackCount = 0;
    //                    //App.MyBleDevice.AssetsUpdatePackNumber = 0;
    //                    //App.SavePersistedValues();
    //                }
    //            }

    //        switch (App.MyBleDevice.FirmwareUpdateStatus)
    //            {
    //                case bleDevice.FirmwareUpdateStatusEnum.FIRMWARE_UPDATE_IDLE:
    //                    if(mcuNewFirmware.Length>0 && assetsNewFirmware.Length>0)
    //                    {
    //                        App.MyBleDevice.FirmwareUpdateStatus = bleDevice.FirmwareUpdateStatusEnum.FIRMWARE_UPDATE_INIT_NEW_MCU_FW;
    //                        App.SavePersistedValues();
    //                    }
    //                    else
    //                    {
    //                        mcuFirmwareFilePath = null;
    //                        assetsFilePath = null;
    //                    }
    //                    break;
    //                case bleDevice.FirmwareUpdateStatusEnum.FIRMWARE_UPDATE_INIT_NEW_MCU_FW:
    //                    //var firmwareSize = NewFirmwareQuery.GetMcuFirmwareSize();
    //                    var firmwareSize = mcuNewFirmware.Length;
    //                    d = (double)(firmwareSize) / 4096.0;
    //                    i = (int)(firmwareSize) / 4096;
    //                    //d = (double)(firmwareSize)/((double)(App.MyBleDevice.MTU_Size * App.MyBleDevice.MaxPacketPerInterval - 7-4)); 
    //                    //i = (int)(firmwareSize / (App.MyBleDevice.MTU_Size * App.MyBleDevice.MaxPacketPerInterval - 7-4));

    //                    //if(NewFirmwareQuery.GetNewSoftwareVersion()==null)
    //                    if (App.MyBleDevice.FirmwareUpdateSelectedVersion == null || App.MyBleDevice.FirmwareUpdateSelectedVersion == "")
    //                    {
    //                        assetsFilePath = null;
    //                        mcuFirmwareFilePath = null;
    //                        App.MyBleDevice.FirmwareUpdateTotalPackCount = 0;
    //                        App.MyBleDevice.DoFirmwareUpdate = false;
    //                        App.MyBleDevice.FirmwareUpdateStatus = bleDevice.FirmwareUpdateStatusEnum.FIRMWARE_UPDATE_IDLE;
    //                        App.MyBleDevice.FirmwareUpdatePackNumber = 0;
    //                        App.MyBleDevice.AssetsUpdateTotalPackCount = 0;
    //                        App.MyBleDevice.AssetsUpdatePackNumber = 0;
    //                        App.MyBleDevice.FirmwareUpdateSelectedVersion = "";
    //                        App.SavePersistedValues();
    //                        return;
    //                    }

    //                    comErrorCount = 0;
    //                    if (d > i)
    //                    {
    //                        App.MyBleDevice.FirmwareUpdateTotalPackCount = i + 1;
    //                    }
    //                    else
    //                    {
    //                        App.MyBleDevice.FirmwareUpdateTotalPackCount = i;
    //                    }

    //                    if (firmwareSize == 0)
    //                    {
    //                        App.MyBleDevice.DoFirmwareUpdate = false;
    //                    }
    //                    else
    //                    {
    //                        //if(await App.MyBleDevice.InitNewMcuFirmware(NewFirmwareQuery.GetNewSoftwareVersion(), firmwareSize))
    //                        if (await App.MyBleDevice.InitNewMcuFirmware(App.MyBleDevice.FirmwareUpdateSelectedVersion, firmwareSize))
    //                        {
    //                            if(App.MyBleDevice.FirmwareUpdateStatus == bleDevice.FirmwareUpdateStatusEnum.FIRMWARE_UPDATE_CANCEL)
    //                            {
    //                                return;
    //                            }
    //                            App.MyBleDevice.FirmwareUpdateStatus = bleDevice.FirmwareUpdateStatusEnum.FIRMWARE_UPDATE_INIT_NEW_ASSETS;
    //                            App.MyBleDevice.FirmwareUpdatePackNumber = 0;
    //                            App.SavePersistedValues();
    //                        }
    //                    }
    //                    break;

    //                case bleDevice.FirmwareUpdateStatusEnum.FIRMWARE_UPDATE_INIT_NEW_ASSETS:
    //                    var assetsSize = assetsNewFirmware.Length;
    //                    //var assetsSize = NewFirmwareQuery.GetAssetsSize();
    //                    //d = (double)(assetsSize) / ((double)(App.MyBleDevice.MTU_Size * App.MyBleDevice.MaxPacketPerInterval - 7-4)); 
    //                    //i = (int)(assetsSize / (App.MyBleDevice.MTU_Size * App.MyBleDevice.MaxPacketPerInterval - 7-4));
    //                    d = (double)(assetsSize) / 4096.0;
    //                    i = (int)(assetsSize) / 4096;
    //                    comErrorCount = 0;

    //                    //if (NewFirmwareQuery.GetNewSoftwareVersion() == null)
    //                    if (App.MyBleDevice.FirmwareUpdateSelectedVersion == null || App.MyBleDevice.FirmwareUpdateSelectedVersion == "")
    //                    {
    //                        assetsFilePath = null;
    //                        mcuFirmwareFilePath = null;
    //                        App.MyBleDevice.FirmwareUpdateTotalPackCount = 0;
    //                        App.MyBleDevice.DoFirmwareUpdate = false;
    //                        App.MyBleDevice.FirmwareUpdateStatus = bleDevice.FirmwareUpdateStatusEnum.FIRMWARE_UPDATE_IDLE;
    //                        App.MyBleDevice.FirmwareUpdatePackNumber = 0;
    //                        App.MyBleDevice.AssetsUpdateTotalPackCount = 0;
    //                        App.MyBleDevice.AssetsUpdatePackNumber = 0;
    //                        App.MyBleDevice.FirmwareUpdateSelectedVersion = "";
    //                        App.SavePersistedValues();
    //                        return;
    //                    }

    //                    if (d > i)
    //                    {
    //                        App.MyBleDevice.AssetsUpdateTotalPackCount = i + 1;
    //                    }
    //                    else
    //                    {
    //                        App.MyBleDevice.AssetsUpdateTotalPackCount = i;
    //                    }

    //                    if (assetsSize == 0)
    //                    {
    //                        App.MyBleDevice.DoFirmwareUpdate = false;
    //                    }
    //                    else
    //                    {
    //                        //if (await App.MyBleDevice.InitNewAssets(NewFirmwareQuery.GetNewSoftwareVersion(), assetsSize))
    //                        if (await App.MyBleDevice.InitNewAssets(App.MyBleDevice.FirmwareUpdateSelectedVersion, assetsSize))
    //                        {
    //                            if (App.MyBleDevice.FirmwareUpdateStatus == bleDevice.FirmwareUpdateStatusEnum.FIRMWARE_UPDATE_CANCEL)
    //                            {
    //                                return;
    //                            }
    //                            App.MyBleDevice.FirmwareUpdateStatus = bleDevice.FirmwareUpdateStatusEnum.FIRMWARE_UPDATE_SENDING_MCU_FW;
    //                            App.MyBleDevice.AssetsUpdatePackNumber = 0;
    //                            App.SavePersistedValues();
    //                        }
    //                    }
    //                    break;

    //                case bleDevice.FirmwareUpdateStatusEnum.FIRMWARE_UPDATE_SENDING_MCU_FW:
    //                    byte[] mcuData;
    //                    if (mcuNewFirmware.Length >= (App.MyBleDevice.FirmwareUpdatePackNumber * 4096) + 4096)
    //                    {
    //                        mcuData = new byte[4096];
    //                        Array.Copy(mcuNewFirmware, App.MyBleDevice.FirmwareUpdatePackNumber * 4096, mcuData, 0, 4096);
    //                    }
    //                    else
    //                    {
    //                        mcuData = new byte[mcuNewFirmware.Length - App.MyBleDevice.FirmwareUpdatePackNumber * 4096];
    //                        Array.Copy(mcuNewFirmware, App.MyBleDevice.FirmwareUpdatePackNumber * 4096, mcuData, 0, mcuNewFirmware.Length - App.MyBleDevice.FirmwareUpdatePackNumber * 4096);
    //                    }
    //                    List<byte> mcuDataBytes = new List<byte>(mcuData);

    //                    mcuDataBytes.Insert(0, (byte)App.MyBleDevice.FirmwareUpdatePackNumber);
    //                    mcuDataBytes.Insert(0, (byte)(App.MyBleDevice.FirmwareUpdatePackNumber >> 8));
    //                    mcuDataBytes.Insert(0, (byte)(App.MyBleDevice.FirmwareUpdatePackNumber >> 16));
    //                    mcuDataBytes.Insert(0, (byte)(App.MyBleDevice.FirmwareUpdatePackNumber >> 24));

    //                    App.MyBleDevice.FirmwareUpdateProgressText = LocalizationResourceManager.Current.GetValue("Firmware is sending");
    //                    App.MyBleDevice.FirmwareUpdateProgressValue = (double)App.MyBleDevice.FirmwareUpdatePackNumber / (double)App.MyBleDevice.FirmwareUpdateTotalPackCount;
    //                    App.MyBleDevice.FirmwareUpdateProgressRate = App.MyBleDevice.FirmwareUpdatePackNumber.ToString() + "/" + App.MyBleDevice.FirmwareUpdateTotalPackCount.ToString();

    //                    var fuResp = await App.MyBleDevice.FirmwareUpdatePack(mcuDataBytes.ToArray(), (ushort)mcuDataBytes.Count);
    //                    if (App.MyBleDevice.FirmwareUpdateStatus == bleDevice.FirmwareUpdateStatusEnum.FIRMWARE_UPDATE_CANCEL)
    //                    {
    //                        return;
    //                    }
    //                    switch (fuResp)
    //                    {
    //                        case bleDevice.PackResponse.BLE_RES_SUCCESS:
    //                            App.MyBleDevice.FirmwareUpdatePackNumber++;
    //                            comErrorCount = 0;

    //                            if (App.MyBleDevice.FirmwareUpdatePackNumber >= App.MyBleDevice.FirmwareUpdateTotalPackCount)
    //                            {
    //                                App.MyBleDevice.FirmwareUpdateStatus = bleDevice.FirmwareUpdateStatusEnum.FIRMWARE_UPDATE_SENDING_ASSETS;
    //                            }
    //                            App.SavePersistedValues();
    //                            break;

    //                        case bleDevice.PackResponse.BLE_RES_FAIL:
    //                            comErrorCount++;
    //                            if (comErrorCount >= errorMaxSize)
    //                            {
    //                                App.MyBleDevice.FirmwareUpdateProgressText = LocalizationResourceManager.Current.GetValue("Firmware couldn't be uploaded! Please try again!");
    //                                App.MyBleDevice.FirmwareUpdateProgressValue = 0;
    //                                assetsFilePath = null;
    //                                mcuFirmwareFilePath = null;
    //                                App.MyBleDevice.FirmwareUpdateTotalPackCount = 0;
    //                                App.MyBleDevice.DoFirmwareUpdate = false;
    //                                App.MyBleDevice.FirmwareUpdateStatus = bleDevice.FirmwareUpdateStatusEnum.FIRMWARE_UPDATE_IDLE;
    //                                App.MyBleDevice.FirmwareUpdatePackNumber = 0;
    //                                App.MyBleDevice.AssetsUpdateTotalPackCount = 0;
    //                                App.MyBleDevice.AssetsUpdatePackNumber = 0;
    //                                App.MyBleDevice.FirmwareUpdateSelectedVersion = "";
    //                                App.SavePersistedValues();

    //                            }
    //                            else
    //                            {

    //                                App.MyBleDevice.FirmwareUpdateProgressText = comErrorCount.ToString() + " " + LocalizationResourceManager.Current.GetValue("Communication error, please wait!");
    //                                await Task.Delay(6000);
    //                            }
    //                            break;
    //                        case bleDevice.PackResponse.BLE_RES_NULL:
    //                            comErrorCount++;
    //                            if (comErrorCount >= errorMaxSize)
    //                            {
    //                                App.MyBleDevice.FirmwareUpdateProgressText = LocalizationResourceManager.Current.GetValue("Firmware couldn't be uploaded! Please try again!");
    //                                App.MyBleDevice.FirmwareUpdateProgressValue = 0;
    //                                assetsFilePath = null;
    //                                mcuFirmwareFilePath = null;
    //                                App.MyBleDevice.FirmwareUpdateTotalPackCount = 0;
    //                                App.MyBleDevice.DoFirmwareUpdate = false;
    //                                App.MyBleDevice.FirmwareUpdateStatus = bleDevice.FirmwareUpdateStatusEnum.FIRMWARE_UPDATE_IDLE;
    //                                App.MyBleDevice.FirmwareUpdatePackNumber = 0;
    //                                App.MyBleDevice.AssetsUpdateTotalPackCount = 0;
    //                                App.MyBleDevice.AssetsUpdatePackNumber = 0;
    //                                App.MyBleDevice.FirmwareUpdateSelectedVersion = "";
    //                                App.SavePersistedValues();

    //                            }
    //                            else
    //                            {
    //                                App.MyBleDevice.FirmwareUpdateProgressText = comErrorCount.ToString() + " " + LocalizationResourceManager.Current.GetValue("Communication error, please wait!");
    //                                await Task.Delay(6000);
    //                            }
    //                            break;
    //                        case bleDevice.PackResponse.BLE_RES_UNKNOWN:
    //                            comErrorCount++;
    //                            if (comErrorCount >= errorMaxSize)
    //                            {
    //                                App.MyBleDevice.FirmwareUpdateProgressText = LocalizationResourceManager.Current.GetValue("Firmware couldn't be uploaded! Please try again!");
    //                                App.MyBleDevice.FirmwareUpdateProgressValue = 0;
    //                                assetsFilePath = null;
    //                                mcuFirmwareFilePath = null;
    //                                App.MyBleDevice.FirmwareUpdateTotalPackCount = 0;
    //                                App.MyBleDevice.DoFirmwareUpdate = false;
    //                                App.MyBleDevice.FirmwareUpdateStatus = bleDevice.FirmwareUpdateStatusEnum.FIRMWARE_UPDATE_IDLE;
    //                                App.MyBleDevice.FirmwareUpdatePackNumber = 0;
    //                                App.MyBleDevice.AssetsUpdateTotalPackCount = 0;
    //                                App.MyBleDevice.AssetsUpdatePackNumber = 0;
    //                                App.MyBleDevice.FirmwareUpdateSelectedVersion = "";
    //                                App.SavePersistedValues();

    //                            }
    //                            else
    //                            {
    //                                App.MyBleDevice.FirmwareUpdateProgressText = comErrorCount.ToString() + " " + LocalizationResourceManager.Current.GetValue("Communication error, please wait!");
    //                                await Task.Delay(6000);
    //                            }
    //                            break;
    //                    }
    //                    break;

    //                case bleDevice.FirmwareUpdateStatusEnum.FIRMWARE_UPDATE_SENDING_ASSETS:
    //                    byte[] assetsData;
    //                    if (assetsNewFirmware.Length >= (App.MyBleDevice.AssetsUpdatePackNumber * 4096) + 4096)
    //                    {
    //                        assetsData = new byte[4096];
    //                        Array.Copy(assetsNewFirmware, App.MyBleDevice.AssetsUpdatePackNumber * 4096, assetsData, 0, 4096);
    //                    }
    //                    else
    //                    {
    //                        assetsData = new byte[assetsNewFirmware.Length - App.MyBleDevice.AssetsUpdatePackNumber * 4096];
    //                        Array.Copy(assetsNewFirmware, App.MyBleDevice.AssetsUpdatePackNumber * 4096, assetsData, 0, assetsNewFirmware.Length - App.MyBleDevice.AssetsUpdatePackNumber * 4096);
    //                    }
    //                    List<byte> assetsDataBytes = new List<byte>(assetsData);

    //                    assetsDataBytes.Insert(0, (byte)App.MyBleDevice.AssetsUpdatePackNumber);
    //                    assetsDataBytes.Insert(0, (byte)(App.MyBleDevice.AssetsUpdatePackNumber >> 8));
    //                    assetsDataBytes.Insert(0, (byte)(App.MyBleDevice.AssetsUpdatePackNumber >> 16));
    //                    assetsDataBytes.Insert(0, (byte)(App.MyBleDevice.AssetsUpdatePackNumber >> 24));

    //                    App.MyBleDevice.FirmwareUpdateProgressText = LocalizationResourceManager.Current.GetValue("Assests is sending");
    //                    App.MyBleDevice.FirmwareUpdateProgressValue = (double)App.MyBleDevice.AssetsUpdatePackNumber / (double)App.MyBleDevice.AssetsUpdateTotalPackCount;
    //                    App.MyBleDevice.FirmwareUpdateProgressRate = App.MyBleDevice.AssetsUpdatePackNumber.ToString() + "/" + App.MyBleDevice.AssetsUpdateTotalPackCount.ToString();

    //                    var asResp = await App.MyBleDevice.AssetsUpdatePack(assetsDataBytes.ToArray(), (ushort)assetsDataBytes.Count);
    //                    if (App.MyBleDevice.FirmwareUpdateStatus == bleDevice.FirmwareUpdateStatusEnum.FIRMWARE_UPDATE_CANCEL)
    //                    {
    //                        return;
    //                    }
    //                    switch (asResp)
    //                    {
    //                        case bleDevice.PackResponse.BLE_RES_SUCCESS:
    //                            App.MyBleDevice.AssetsUpdatePackNumber++;
    //                            comErrorCount = 0;
    //                            if (App.MyBleDevice.AssetsUpdatePackNumber >= App.MyBleDevice.AssetsUpdateTotalPackCount)
    //                            {
    //                                App.MyBleDevice.FirmwareUpdateStatus = bleDevice.FirmwareUpdateStatusEnum.FIRMWARE_UPDATE_CHECKING_UPLODED_FILES;
    //                                App.MyBleDevice.FirmwareUpdateProgressText = LocalizationResourceManager.Current.GetValue("Assets have been sent");
    //                                App.MyBleDevice.FirmwareUpdateProgressValue = (double)App.MyBleDevice.AssetsUpdatePackNumber / (double)App.MyBleDevice.AssetsUpdateTotalPackCount;
    //                                App.MyBleDevice.FirmwareUpdateProgressRate = App.MyBleDevice.AssetsUpdatePackNumber.ToString() + "/" + App.MyBleDevice.AssetsUpdateTotalPackCount.ToString();
    //                                await Task.Delay(6000);
    //                            }
    //                            App.SavePersistedValues();
    //                            break;

    //                        case bleDevice.PackResponse.BLE_RES_FAIL:
    //                            comErrorCount++;
    //                            if (comErrorCount >= errorMaxSize)
    //                            {
    //                                App.MyBleDevice.FirmwareUpdateProgressText = LocalizationResourceManager.Current.GetValue("Firmware couldn't be uploaded! Please try again!");
    //                                App.MyBleDevice.FirmwareUpdateProgressValue = 0;
    //                                assetsFilePath = null;
    //                                mcuFirmwareFilePath = null;
    //                                App.MyBleDevice.FirmwareUpdateTotalPackCount = 0;
    //                                App.MyBleDevice.DoFirmwareUpdate = false;
    //                                App.MyBleDevice.FirmwareUpdateStatus = bleDevice.FirmwareUpdateStatusEnum.FIRMWARE_UPDATE_IDLE;
    //                                App.MyBleDevice.FirmwareUpdatePackNumber = 0;
    //                                App.MyBleDevice.AssetsUpdateTotalPackCount = 0;
    //                                App.MyBleDevice.AssetsUpdatePackNumber = 0;
    //                                App.MyBleDevice.FirmwareUpdateSelectedVersion = "";
    //                                App.SavePersistedValues();

    //                            }
    //                            else
    //                            {
    //                                App.MyBleDevice.FirmwareUpdateProgressText = comErrorCount.ToString() + " " + LocalizationResourceManager.Current.GetValue("Communication error, please wait!");
    //                                await Task.Delay(6000);
    //                            }
    //                            break;
    //                        case bleDevice.PackResponse.BLE_RES_NULL:
    //                            comErrorCount++;
    //                            if (comErrorCount >= errorMaxSize)
    //                            {
    //                                App.MyBleDevice.FirmwareUpdateProgressText = LocalizationResourceManager.Current.GetValue("Firmware couldn't be uploaded! Please try again!");
    //                                App.MyBleDevice.FirmwareUpdateProgressValue = 0;
    //                                assetsFilePath = null;
    //                                mcuFirmwareFilePath = null;
    //                                App.MyBleDevice.FirmwareUpdateTotalPackCount = 0;
    //                                App.MyBleDevice.DoFirmwareUpdate = false;
    //                                App.MyBleDevice.FirmwareUpdateStatus = bleDevice.FirmwareUpdateStatusEnum.FIRMWARE_UPDATE_IDLE;
    //                                App.MyBleDevice.FirmwareUpdatePackNumber = 0;
    //                                App.MyBleDevice.AssetsUpdateTotalPackCount = 0;
    //                                App.MyBleDevice.AssetsUpdatePackNumber = 0;
    //                                App.MyBleDevice.FirmwareUpdateSelectedVersion = "";
    //                                App.SavePersistedValues();

    //                            }
    //                            else
    //                            {
    //                                App.MyBleDevice.FirmwareUpdateProgressText = comErrorCount.ToString() + " " + LocalizationResourceManager.Current.GetValue("Communication error, please wait!");
    //                                await Task.Delay(6000);
    //                            }
    //                            break;
    //                        case bleDevice.PackResponse.BLE_RES_UNKNOWN:
    //                            comErrorCount++;
    //                            if (comErrorCount >= errorMaxSize)
    //                            {
    //                                App.MyBleDevice.FirmwareUpdateProgressText = LocalizationResourceManager.Current.GetValue("Firmware couldn't be uploaded! Please try again!");
    //                                App.MyBleDevice.FirmwareUpdateProgressValue = 0;
    //                                assetsFilePath = null;
    //                                mcuFirmwareFilePath = null;
    //                                App.MyBleDevice.FirmwareUpdateTotalPackCount = 0;
    //                                App.MyBleDevice.DoFirmwareUpdate = false;
    //                                App.MyBleDevice.FirmwareUpdateStatus = bleDevice.FirmwareUpdateStatusEnum.FIRMWARE_UPDATE_IDLE;
    //                                App.MyBleDevice.FirmwareUpdatePackNumber = 0;
    //                                App.MyBleDevice.AssetsUpdateTotalPackCount = 0;
    //                                App.MyBleDevice.AssetsUpdatePackNumber = 0;
    //                                App.MyBleDevice.FirmwareUpdateSelectedVersion = "";
    //                                App.SavePersistedValues();

    //                            }
    //                            else
    //                            {
    //                                App.MyBleDevice.FirmwareUpdateProgressText = comErrorCount.ToString() + " " + LocalizationResourceManager.Current.GetValue("Communication error, please wait!");
    //                                await Task.Delay(6000);
    //                            }
    //                            break;
    //                    }
    //                    break;

    //                case bleDevice.FirmwareUpdateStatusEnum.FIRMWARE_UPDATE_CHECKING_UPLODED_FILES:
    //                    App.MyBleDevice.FirmwareUpdateProgressText = LocalizationResourceManager.Current.GetValue("Checking uploaded files");
    //                    var cfResp = await App.MyBleDevice.CheckFirmwareFiles();
    //                    if (App.MyBleDevice.FirmwareUpdateStatus == bleDevice.FirmwareUpdateStatusEnum.FIRMWARE_UPDATE_CANCEL)
    //                    {
    //                        return;
    //                    }
    //                    switch (cfResp)
    //                    {
    //                        case bleDevice.PackResponse.BLE_RES_SUCCESS:
    //                            comErrorCount = 0;
    //                            App.MyBleDevice.FirmwareUpdateStatus = bleDevice.FirmwareUpdateStatusEnum.FIRMWARE_UPDATE_RUN_NEW_FIRMWARE;
    //                            App.MyBleDevice.FirmwareUpdateProgressText = LocalizationResourceManager.Current.GetValue("Uploaded files have been checked");
    //                            App.SavePersistedValues();
    //                            await Task.Delay(6000);
    //                            break;

    //                        case bleDevice.PackResponse.BLE_RES_FAIL:
    //                            App.MyBleDevice.FirmwareUpdateProgressText = LocalizationResourceManager.Current.GetValue("The dive computer firmware update process is failed!") + " " +
    //                            LocalizationResourceManager.Current.GetValue("Uploaded files CRC error!") + " " +
    //                            LocalizationResourceManager.Current.GetValue("Please try again!");
    //                            App.MyBleDevice.FirmwareUpdateProgressValue = 0;
    //                            assetsFilePath = null;
    //                            mcuFirmwareFilePath = null;
    //                            App.MyBleDevice.FirmwareUpdateTotalPackCount = 0;
    //                            App.MyBleDevice.DoFirmwareUpdate = false;
    //                            App.MyBleDevice.FirmwareUpdateStatus = bleDevice.FirmwareUpdateStatusEnum.FIRMWARE_UPDATE_IDLE;
    //                            App.MyBleDevice.FirmwareUpdatePackNumber = 0;
    //                            App.MyBleDevice.AssetsUpdateTotalPackCount = 0;
    //                            App.MyBleDevice.AssetsUpdatePackNumber = 0;
    //                            App.MyBleDevice.FirmwareUpdateSelectedVersion = "";
    //                            App.SavePersistedValues();
    //                            await Task.Delay(6000);
    //                            break;
    //                        case bleDevice.PackResponse.BLE_RES_NULL:
    //                            comErrorCount++;
    //                            if (comErrorCount >= errorMaxSize)
    //                            {
    //                                App.MyBleDevice.FirmwareUpdateProgressText = LocalizationResourceManager.Current.GetValue("The dive computer firmware update process is failed!") + " " +
    //                                LocalizationResourceManager.Current.GetValue("Uploaded files CRC error!") + " " +
    //                                LocalizationResourceManager.Current.GetValue("Please try again!");
    //                                App.MyBleDevice.FirmwareUpdateProgressValue = 0;
    //                                assetsFilePath = null;
    //                                mcuFirmwareFilePath = null;
    //                                App.MyBleDevice.FirmwareUpdateTotalPackCount = 0;
    //                                App.MyBleDevice.DoFirmwareUpdate = false;
    //                                App.MyBleDevice.FirmwareUpdateStatus = bleDevice.FirmwareUpdateStatusEnum.FIRMWARE_UPDATE_IDLE;
    //                                App.MyBleDevice.FirmwareUpdatePackNumber = 0;
    //                                App.MyBleDevice.AssetsUpdateTotalPackCount = 0;
    //                                App.MyBleDevice.AssetsUpdatePackNumber = 0;
    //                                App.MyBleDevice.FirmwareUpdateSelectedVersion = "";
    //                                App.SavePersistedValues();
    //                                await Task.Delay(6000);

    //                            }
    //                            else
    //                            {
    //                                App.MyBleDevice.FirmwareUpdateProgressText = comErrorCount.ToString() + " " + LocalizationResourceManager.Current.GetValue("Communication error, please wait!");
    //                                await Task.Delay(6000);
    //                            }
    //                            break;
    //                        case bleDevice.PackResponse.BLE_RES_UNKNOWN:
    //                            comErrorCount++;
    //                            if (comErrorCount >= errorMaxSize)
    //                            {
    //                                App.MyBleDevice.FirmwareUpdateProgressText = LocalizationResourceManager.Current.GetValue("The dive computer firmware update process is failed!") + " " +
    //                                LocalizationResourceManager.Current.GetValue("Uploaded files CRC error!") + " " +
    //                                LocalizationResourceManager.Current.GetValue("Please try again!");
    //                                App.MyBleDevice.FirmwareUpdateProgressValue = 0;
    //                                assetsFilePath = null;
    //                                mcuFirmwareFilePath = null;
    //                                App.MyBleDevice.FirmwareUpdateTotalPackCount = 0;
    //                                App.MyBleDevice.DoFirmwareUpdate = false;
    //                                App.MyBleDevice.FirmwareUpdateStatus = bleDevice.FirmwareUpdateStatusEnum.FIRMWARE_UPDATE_IDLE;
    //                                App.MyBleDevice.FirmwareUpdatePackNumber = 0;
    //                                App.MyBleDevice.AssetsUpdateTotalPackCount = 0;
    //                                App.MyBleDevice.AssetsUpdatePackNumber = 0;
    //                                App.MyBleDevice.FirmwareUpdateSelectedVersion = "";
    //                                App.SavePersistedValues();

    //                            }
    //                            else
    //                            {
    //                                App.MyBleDevice.FirmwareUpdateProgressText = comErrorCount.ToString() + " " + LocalizationResourceManager.Current.GetValue("Communication error, please wait!");
    //                                await Task.Delay(6000);
    //                            }
    //                            break;
    //                    }
    //                    break;

    //                case bleDevice.FirmwareUpdateStatusEnum.FIRMWARE_UPDATE_RUN_NEW_FIRMWARE:

    //                    var rfuResp = await App.MyBleDevice.RunFirmwareUpdate();
    //                    if (App.MyBleDevice.FirmwareUpdateStatus == bleDevice.FirmwareUpdateStatusEnum.FIRMWARE_UPDATE_CANCEL)
    //                    {
    //                        return;
    //                    }
    //                    switch (rfuResp)
    //                    {
    //                        case bleDevice.PackResponse.BLE_RES_SUCCESS:
    //                            comErrorCount = 0;
    //                            App.MyBleDevice.FirmwareUpdateProgressText = LocalizationResourceManager.Current.GetValue("Firmware update files have been sent successfully") + " " +
    //                            LocalizationResourceManager.Current.GetValue("Follow your dive computer directives to update firmware");
    //                            App.MyBleDevice.FirmwareUpdateProgressValue = 0;
    //                            assetsFilePath = null;
    //                            mcuFirmwareFilePath = null;
    //                            App.MyBleDevice.FirmwareUpdateTotalPackCount = 0;
    //                            App.MyBleDevice.DoFirmwareUpdate = false;
    //                            App.MyBleDevice.FirmwareUpdateStatus = bleDevice.FirmwareUpdateStatusEnum.FIRMWARE_UPDATE_IDLE;
    //                            App.MyBleDevice.FirmwareUpdatePackNumber = 0;
    //                            App.MyBleDevice.AssetsUpdateTotalPackCount = 0;
    //                            App.MyBleDevice.AssetsUpdatePackNumber = 0;
    //                            App.MyBleDevice.FirmwareUpdateSelectedVersion = "";
    //                            App.SavePersistedValues();
    //                            await Task.Delay(6000);
    //                            break;

    //                        case bleDevice.PackResponse.BLE_RES_FAIL:
    //                        case bleDevice.PackResponse.BLE_RES_NULL:
    //                        case bleDevice.PackResponse.BLE_RES_UNKNOWN:
    //                            App.MyBleDevice.FirmwareUpdateProgressText = LocalizationResourceManager.Current.GetValue("Firmware update files have been sent successfully") + " " +
    //                            LocalizationResourceManager.Current.GetValue("But there is no response from your dive computer") + " " +
    //                            LocalizationResourceManager.Current.GetValue("Please turn off and turn on your dive computer to start firmware update");
    //                            App.MyBleDevice.FirmwareUpdateProgressValue = 0;
    //                            assetsFilePath = null;
    //                            mcuFirmwareFilePath = null;
    //                            App.MyBleDevice.FirmwareUpdateTotalPackCount = 0;
    //                            App.MyBleDevice.DoFirmwareUpdate = false;
    //                            App.MyBleDevice.FirmwareUpdateStatus = bleDevice.FirmwareUpdateStatusEnum.FIRMWARE_UPDATE_IDLE;
    //                            App.MyBleDevice.FirmwareUpdatePackNumber = 0;
    //                            App.MyBleDevice.AssetsUpdateTotalPackCount = 0;
    //                            App.MyBleDevice.AssetsUpdatePackNumber = 0;
    //                            App.MyBleDevice.FirmwareUpdateSelectedVersion = "";
    //                            App.SavePersistedValues();
    //                            await Task.Delay(6000);
    //                            break;

    //                    }
    //                    break;

    //                case bleDevice.FirmwareUpdateStatusEnum.FIRMWARE_UPDATE_CANCEL:
    //                    var cfuResp = await App.MyBleDevice.CancelFirmwareUpdate();

    //                    switch (cfuResp)
    //                    {
    //                        case bleDevice.PackResponse.BLE_RES_SUCCESS:
    //                        case bleDevice.PackResponse.BLE_RES_FAIL:
    //                        case bleDevice.PackResponse.BLE_RES_NULL:
    //                        case bleDevice.PackResponse.BLE_RES_UNKNOWN:
    //                            comErrorCount = 0;
    //                            App.MyBleDevice.FirmwareUpdateProgressText = LocalizationResourceManager.Current.GetValue("Firmware update process has been cancelled");
    //                            App.MyBleDevice.FirmwareUpdateProgressValue = 0;
    //                            assetsFilePath = null;
    //                            mcuFirmwareFilePath = null;
    //                            App.MyBleDevice.FirmwareUpdateTotalPackCount = 0;
    //                            App.MyBleDevice.FirmwareUpdateStatus = bleDevice.FirmwareUpdateStatusEnum.FIRMWARE_UPDATE_IDLE;
    //                            App.MyBleDevice.FirmwareUpdatePackNumber = 0;
    //                            App.MyBleDevice.AssetsUpdateTotalPackCount = 0;
    //                            App.MyBleDevice.AssetsUpdatePackNumber = 0;
    //                            App.MyBleDevice.FirmwareUpdateSelectedVersion = "";
    //                            await Task.Delay(2000);
    //                            App.MyBleDevice.DoFirmwareUpdate = false;
    //                            App.SavePersistedValues();
    //                            await Task.Delay(4000);
    //                            break;

    //                    }
    //                    break;


    //            }
    //        }
    //    }

    }



    public  class HandleBleReceivedLongRunningTaskMessages
    {

        public HandleBleReceivedLongRunningTaskMessages()
        {
            HandleLongRunningTaskMessages();
        }
        public class BleCancelledMessage
        {
            
        }

        public class BleStartLongRunningMessage
        {
        }

        public class BleStopLongRunningMessage
        {
        }

        
        public class BleTickedMessage
        {
            //public string Message { get; set; }
        }

        //public HandleBleReceivedLongRunningTaskMessages()
        //{
        //    MessagingCenter.Subscribe<BleTickedMessage>(this, "BleTickedMessage", message =>
        //    {
        //        Device.BeginInvokeOnMainThread(() =>
        //        {

        //            System.Threading.Thread.Sleep(10);
        //        });
        //    });

        //    MessagingCenter.Subscribe<BleCancelledMessage>(this, "BleCancelledMessage", message =>
        //    {
        //        Device.BeginInvokeOnMainThread(() =>
        //        {
        //            App.IsBleBackgroundAlive = false;
        //        });
        //    });
        //}


        public void HandleLongRunningTaskMessages()
        {
            MessagingCenter.Subscribe<BleTickedMessage>(this, "BleTickedMessage", message =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {

                    System.Threading.Thread.Sleep(10);
                });
            });

            MessagingCenter.Subscribe<BleCancelledMessage>(this, "BleCancelledMessage", message =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    App.IsBleBackgroundAlive = false;
                    //var msg = new BleStartLongRunningMessage();
                   // App.StartIndicatorBackgroudCommunication();
                    //MessagingCenter.Send(msg, "BleStartLongRunningMessage");
                });
            });
        }
    }


}