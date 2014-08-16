using NRaas.CommonSpace.Helpers;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.TaggerSpace.Options
{
    // eh small key but using this format just in case I decide to add something to it
    [Persistable]
    public class TagSettingKey : IPersistence
    {
        private uint guid;
        private uint colorHEX;        

        public TagSettingKey()
        { }

        public uint GUID
        {
            get { return guid; }
            private set {  }
        }

        public uint Color
        {
            get { return colorHEX; }
            private set { }
        }

        public void SetGUID(string val)
        {
            this.guid = ResourceUtils.HashString32(val);            
        }

        public void SetColorHex(string hex)
        {
            this.colorHEX = uint.Parse(hex, System.Globalization.NumberStyles.HexNumber);
        }

        public void SetColor(uint color)
        {
            this.colorHEX = color;
        }

        public void Export(Persistence.Lookup settings)
        {
        }

        public void Import(Persistence.Lookup settings)
        {
        }

        public string PersistencePrefix
        {
            get { return guid.ToString(); }
        }
    }
}
