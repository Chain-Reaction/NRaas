using NRaas.CommonSpace.Helpers;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.TaggerSpace.Options
{
    public class TagStaticData
    {
        private uint guid;
        private bool isValid;
        private uint colorHEX;
        public string name;
        public bool isBusinessType;
        public string color;        
        public string icon;
        public int openHour;
        public int closeHour;

        public TagStaticData()
        { }

        public uint GUID
        {
            get { return guid; }
            private set { }
        }

        public bool Valid
        {
            get { return isValid; }
            private set { }
        }

        public ResourceKey IconKey
        {
            get { return ResourceKey.CreatePNGKey(icon, 0); }
            private set { }
        }

        public uint Color
        {
            get { return colorHEX; }
            private set { }
        }

        public void SetGUID(string val)
        {
            this.guid = ResourceUtils.HashString32(val);
            this.isValid = true;

            if (this.guid < 95)
            {
                this.isValid = false;
                throw new ArgumentException("Value for " + val + " is within EA range");
            }            
        }

        public void SetColorHex(string hex)
        {
            hex = "FF" + hex;
            this.color = hex;
            this.colorHEX = uint.Parse(hex, System.Globalization.NumberStyles.HexNumber);
        }
    }
}
