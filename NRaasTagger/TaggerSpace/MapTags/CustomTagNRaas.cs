using NRaas.CommonSpace.Helpers;
using NRaas.TaggerSpace.Options;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.MapTags;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.TaggerSpace.MapTags
{
    public class CustomTagNRaas : MapTag, IIconOverrideMapTag
    {
        public CustomTagNRaas()
        { }
        public CustomTagNRaas(Lot targetLot, Sim owner)
            : base(targetLot, owner)
        {
            if (targetLot != null)
            {
                LotType = (uint)targetLot.CommercialLotSubType;
            }
        }

        public uint LotType = 0;
        
        public override MapTagType TagType
        {
            get
            {
                try
                {
                    MapTagType type;
                    if (ParserFunctions.TryParseEnum<MapTagType>("CustomTagNRaas", out type, MapTagType.Count, true))
                    {                        
                        return type;                       
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(Sim.ActiveActor, Target, e);
                }

                return MapTagType.Venue;
            }
        }

        public ResourceKey IconKey
        {
            get
            {
                TagStaticData data;
                if(Tagger.staticData.TryGetValue(this.LotType, out data))
                {                    
                    return data.IconKey;
                } else {
                    return ResourceKey.kInvalidResourceKey;
                }
            }
        }

        public override Color ShadeColor
        {
            get
            {
                try
                {
                    uint color = Tagger.Settings.GetCustomTagColor(this.LotType);

                    if (color == 0)
                    {
                        return base.ShadeColor;
                    }
                    else
                    {
                        return new Color(color);
                    }
                    
                    /*
                    int type = Convert.ToInt32(target.CommercialLotSubType);
                    if (type == 95)
                    {
                        return new Color(252, 121, 5);
                    }
                    else if (type == 96)
                    {
                        return new Color(124, 83, 47);
                    }
                    else if (type == 97)
                    {
                        return new Color(47, 124, 89);
                    }
                    else if (type == 98)
                    {
                        return new Color(213, 11, 11);
                    }
                    else if (type == 99)
                    {
                        return new Color(203, 196, 10);
                    }
                     */
                    
                }
                catch (Exception exception)
                {
                    Common.Exception(Sim.ActiveActor, Target, exception);
                }

                return new Color(255, 255, 255);
            }
        }

        public override MapTagFilterType FilterType
        {
            get
            {
                try
                {
                    // can be used to alter the type based on working at the lots, etc in the future
                    return MapTagFilterType.PublicSpacesAndActivities;
                }
                catch (Exception exception)
                {
                    Common.Exception(Sim.ActiveActor, Target, exception);
                    return MapTagFilterType.None;
                }
            }
        }

        public override string HoverText
        {
            get
            {
                return this.LotName + Common.NewLine + this.LotAddress;
            }
        }

        public override string LotName
        {
            get
            {
                Lot target = this.Target as Lot;
                if (target != null)
                {
                    return target.Name;
                }
                return string.Empty;
            }
        }

        public override string LotAddress
        {
            get
            {
                Lot target = this.Target as Lot;
                if (target != null)
                {
                    return target.Address;
                }
                return string.Empty;
            }
        }

    }
}