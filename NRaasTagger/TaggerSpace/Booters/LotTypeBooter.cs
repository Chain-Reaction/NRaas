using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.TaggerSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Academics;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.TaggerSpace.Booters
{
    public class LotTypeBooter : BooterHelper.ByRowListingBooter
    {
        public LotTypeBooter()
            : this(VersionStamp.sNamespace + ".LotType", true)
        { }
        public LotTypeBooter(string reference, bool testDirect)
            : base("Types", "LotTypeFile", "File", reference, testDirect)
        { }

        protected override void Perform(BooterHelper.BootFile file, XmlDbRow row)
        {
            TagStaticData data = new TagStaticData();

            string name = row.GetString("TypeName");
            string icon = row.GetString("Icon");
            string color = row.GetString("ColorHEX");
            bool business = row.GetBool("isBusinessType");
            int openHour = row.GetInt("OpenHour");
            int closeHour = row.GetInt("CloseHour");

            try
            {
                data.SetGUID(name);
            }
            catch (ArgumentException e)
            {
                Common.Exception("", e);
            }

            if (!data.Valid)
            {
                return;
            }

            data.name = name;
            data.icon = icon;
            data.isBusinessType = business;
            data.openHour = openHour;
            data.closeHour = closeHour;
            data.SetColorHex(color);

            if (!Tagger.staticData.ContainsKey(data.GUID))
            {                
                Tagger.staticData.Add(data.GUID, data);
                EnumInjection.InjectEnums<CommercialLotSubType>(new string[] { name }, new object[] { data.GUID }, false);
                EnumInjection.InjectEnums<Lot.MetaAutonomyType>(new string[] { name }, new object[] { data.GUID }, false);
                EnumInjection.InjectEnums<MetaAutonomyVenueType>(new string[] { name }, new object[] { data.GUID }, false); 
            }
        }
    }
}