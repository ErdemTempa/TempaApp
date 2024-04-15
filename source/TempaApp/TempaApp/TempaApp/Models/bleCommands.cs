using System;
using System.Collections.Generic;
using System.Text;

namespace TempaApp.Models
{
    public static class bleCommands
    {
        public static string SoftwareVersion
        {
            get { return "GVC"; }
        }
        public static string GetClock
        {
            get { return "GCC"; }
        }
        public static string SetClock
        {
            get { return "SCC"; }
        }
        public static string GetDate
        {
            get { return "GDC"; }
        }
        public static string SetDate
        {
            get { return "SDC"; }
        }
        public static string CheckCommunication
        {
            get { return "CCC"; }
        }

        public static string InitForNewMcuFirmware
        {
            get { return "MIC"; }
        }

        public static string InitForNewAssets
        {
            get { return "AIC"; }
        }

        public static string FirmwareUpdate
        {
            get { return "FUC"; }
        }

        public static string AssetsUpdate
        {
            get { return "AUC"; }
        }

        public static string CheckFirmwareFiles
        {
            get { return "CFC"; }
        }

        public static string RunFirmwareUpdate
        {
            get { return "RFC"; }
        }

        public static string CancelFirmwareUpdate
        {
            get { return "CUC"; }
        }

        public static string SetKey
        {
            get { return "SKC"; }
        }

        public static string GetScreen
        {
            get { return "GSC"; }
        }

    }

    public static class bleCommandsResponse
    {
        public static string SoftwareVersion
        {
            get { return "GVR"; }
        }
        public static string GetClock
        {
            get { return "GCR"; }
        }
        public static string SetClock
        {
            get { return "SCR"; }
        }
        public static string GetDate
        {
            get { return "GDR"; }
        }
        public static string SetDate
        {
            get { return "SDR"; }
        }
        public static string CheckCommunication
        {
            get { return "CCR"; }
        }

        public static string InitForNewMcuFirmware
        {
            get { return "MIR"; }
        }

        public static string InitForNewAssets
        {
            get { return "AIR"; }
        }

        public static string FirmwareUpdate
        {
            get { return "FUR"; }
        }

        public static string AssetsUpdate
        {
            get { return "AUR"; }
        }

        public static string CheckFirmwareFiles
        {
            get { return "CFR"; }
        }

        public static string RunFirmwareUpdate
        {
            get { return "RFR"; }
        }

        public static string CancelFirmwareUpdate
        {
            get { return "CUR"; }
        }

        public static string SetKey
        {
            get { return "SKR"; }
        }

        public static string GetScreen
        {
            get { return "GSR"; }
        }

    }
}
