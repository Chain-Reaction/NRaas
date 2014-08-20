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
        private uint metaAutonomyType;

        public TagSettingKey()
        { }
        public TagSettingKey(uint guid)
        {
            this.guid = guid;
        }

        public uint GUID
        {
            get { return guid; }
            private set {  }
        }

        public uint Color
        {
            get { return colorHEX; }
            set { colorHEX = value; }
        }

        public uint MetaAutonomyType
        {
            get { return metaAutonomyType; }
            set { metaAutonomyType = value; }
        }

        public void SetGUID(string val)
        {
            this.guid = ResourceUtils.HashString32(val);            
        }

        public void SetColorHex(string hex)
        {
            this.colorHEX = uint.Parse(hex, System.Globalization.NumberStyles.HexNumber);
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
