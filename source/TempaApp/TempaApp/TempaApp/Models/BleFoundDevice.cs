using Plugin.BLE.Abstractions.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace TempaApp.Models
{
    public class BleFoundDevice
    {
        public string Name { get; set; }
        public IDevice Device { get; set; }
    }
}
