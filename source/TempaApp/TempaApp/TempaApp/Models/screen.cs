using System;
using System.Collections.Generic;
using System.Text;

namespace TempaApp.Models
{

    public class Screen
    {
        public int status = 0;
        public byte[] unnecessaryData = new byte[50];
        public bool stabilityLed = false;
        public bool zeroLed = false;
        public bool tareLed = false;
        public bool netLed = false; 
        public bool memoryLed = false;          
        public List<byte> weightData = new List<byte>();
        public List<byte> infoData = new List<byte>();
        public List<byte> unitDaya = new List<byte>();
        public bool screenDataReady = false;        
    };
}
