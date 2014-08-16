using NRaas.CommonSpace.Helpers;
using NRaas.TaggerSpace.Helpers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.MapTags;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.TimeTravel;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.TaggerSpace.MapTags
{
    public class TrackedLot : NeighborLotMapTag, INpcPartyMapTag
    {
        static Common.MethodStore sGetPartyOnLot = new Common.MethodStore("NRaasStoryProgression", "NRaas.StoryProgression", "GetNPCPartyInvolvingLot", new Type[] { typeof(Dictionary<string, object>) });
        
        public TrackedLot(Lot targetLot, Sim owner)
            : base(targetLot, owner)
        { }

        public uint CelebrityLevel
        {
            get
            {
                Lot lot = Target as Lot;
                if (lot != null)
                {
                    Party party = TryGetPartyOnLot(lot);
                    if (party != null)
                    {
                        Sim host = party.Host;
                        if (host != null)
                        {
                            return host.CelebrityManager.Level;
                        }
                    }

                    Dictionary<string, object> spParty = TryGetSPPartyOnLot(lot);
                    if (spParty != null)
                    {
                        if (spParty.ContainsKey("Host"))
                        {
                            Sim host = spParty["Host"] as Sim;
                            if (host != null)
                            {
                                return host.CelebrityManager.Level;
                            }
                        }
                    }
                }
                return 0x0;
            }
        }

        public static string GetLocalizedClothingStyle(OutfitCategories category)
        {
            switch (category)
            {
                case OutfitCategories.Everyday:
                    return Common.LocalizeEAString("Ui/Caption/Party:Casual");

                case OutfitCategories.Formalwear:
                    return Common.LocalizeEAString("Ui/Caption/Party:FormalAttire");

                case OutfitCategories.Swimwear:
                    return Common.LocalizeEAString("Ui/Caption/Party:Swim");
            }
            return string.Empty;
        }

        public string ClothingStyle
        {
            get
            {
                Lot lot = Target as Lot;
                if (lot != null)
                {
                    Party party = TryGetPartyOnLot(lot);
                    if (party != null)
                    {
                        return Common.LocalizeEAString(false, "Gameplay/MapTags/NpcPartyMapTag:Clothing", new object[] { GetLocalizedClothingStyle(party.GetClothingStyle()) });
                    }

                    Dictionary<string, object> spParty = TryGetSPPartyOnLot(lot);
                    if (spParty != null)
                    {
                        if (spParty.ContainsKey("DressCode"))
                        {
                            string sCode = spParty["DressCode"] as string;
                            OutfitCategories code;
                            if (ParserFunctions.TryParseEnum<OutfitCategories>(sCode, out code, OutfitCategories.None))
                            {
                                return Common.LocalizeEAString(false, "Gameplay/MapTags/NpcPartyMapTag:Clothing", new object[] { GetLocalizedClothingStyle(code) });
                            }
                        }
                    }
                }

                return null;
            }
        }

        public ObjectGuid HostGuid
        {
            get
            {
                Lot lot = Target as Lot;
                if (lot != null)
                {
                    Party party = TryGetPartyOnLot(lot);
                    if (party != null)
                    {
                        Sim host = party.Host;
                        if (host != null)
                        {
                            return host.ObjectId;
                        }
                    }

                    Dictionary<string, object> spParty = TryGetSPPartyOnLot(lot);
                    if (spParty != null)
                    {
                        if (spParty.ContainsKey("Host"))
                        {
                            Sim host = spParty["Host"] as Sim;
                            if (host != null)
                            {
                                return host.ObjectId;
                            }
                        }
                    }
                }

                return ObjectGuid.InvalidObjectGuid;
            }
        }

        public string HostName
        {
            get
            {
                Lot lot = Target as Lot;
                if (lot != null)
                {
                    Party party = TryGetPartyOnLot(lot);
                    if (party != null)
                    {
                        Sim host = party.Host;
                        if (host != null)
                        {
                            return Common.LocalizeEAString(host.IsFemale, "Gameplay/MapTags/NpcPartyMapTag:Host", new object[] { host });
                        }
                    }

                    Dictionary<string, object> spParty = TryGetSPPartyOnLot(lot);
                    if (spParty != null)
                    {
                        if (spParty.ContainsKey("Host"))
                        {
                            Sim host = spParty["Host"] as Sim;
                            if (host != null)
                            {
                                return Common.LocalizeEAString(host.IsFemale, "Gameplay/MapTags/NpcPartyMapTag:Host", new object[] { host });
                            }
                        }
                    }
                }
                return null;
            }
        }

        public string StartTime
        {
            get
            {
                Lot lot = Target as Lot;
                if (lot != null)
                {
                    Party party = TryGetPartyOnLot(lot);
                    if (party != null)
                    {
                        return Common.LocalizeEAString(false, "Gameplay/MapTags/NpcPartyMapTag:Time", new object[] { party.StartTime });
                    }

                    Dictionary<string, object> spParty = TryGetSPPartyOnLot(lot);
                    if (spParty != null)
                    {
                        if (spParty.ContainsKey("StartTime"))
                        {
                            return spParty["StartTime"] as string;                            
                        }
                    }
                }
                return null;
            }
        }

        protected static Party TryGetPartyOnLot(Lot lot)
        {
            NpcParty npcParty = NpcParty.TryGetNpcPartyOnLot(lot);
            if (npcParty != null) return npcParty;            

            return null;
        }

        protected static Dictionary<string, object> TryGetSPPartyOnLot(Lot lot)
        {
            if (sGetPartyOnLot.Valid)
            {
                return sGetPartyOnLot.Invoke<Dictionary<string, object>>(new object[] { lot });
            }

            return null;
        }

        public override MapTagType TagType
        {
            get
            {
                try
                {
                    Lot lot = Target as Lot;
                    if (lot != null)
                    {
                        mType = MapTagType.NeighborLot;

                        Party npcParty = TryGetPartyOnLot(lot);
                        Dictionary<string, object> dic = TryGetSPPartyOnLot(lot);
                        if (npcParty != null || dic != null)
                        {
                            mType = MapTagType.NpcParty;
                        }
                        else if (lot.IsFutureDescendantLot)
                        {
                            mType = MapTagType.DescendantFamily;
                        }

                        if (mType == MapTagType.NeighborLot)
                        {
                            if (lot.IsCelebrityLot)
                            {
                                mType = MapTagType.CelebrityLot;
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    Common.Exception(Sim.ActiveActor, Target, exception);
                }

                return mType;
            }
        }

        public override Color ShadeColor
        {
            get
            {
                try
                {
                    Lot target = Target as Lot;
                    if ((target == null) || (target.Household == null) || (Sim.ActiveActor == null) || (target.Household == Sim.ActiveActor.Household))
                    {
                        return base.ShadeColor;
                    }

                    if (Tagger.Settings.mColorLotTagsByRelationship)
                    {
                        bool family = false, romantic = false, career = false, friend = false, enemy = false, known = false;

                        foreach (SimDescription member in target.Household.AllSimDescriptions)
                        {
                            if (Relationships.IsCloselyRelated(Sim.ActiveActor.SimDescription, member, false))
                            {
                                family = true;
                            }

                            Relationship relation = Relationship.Get(Sim.ActiveActor.SimDescription, member, false);
                            if (relation != null)
                            {
                                if (relation.AreRomantic())
                                {
                                    romantic = true;
                                }
                                else if ((Relationships.IsCoworkerOrBoss(Sim.ActiveActor.Occupation, member)) ||
                                         (Relationships.IsCoworkerOrBoss(Sim.ActiveActor.School, member)))
                                {
                                    career = true;

                                }
                                else if (relation.LTR.Liking <= -20)
                                {
                                    enemy = true;
                                }
                                else if (relation.LTR.Liking >= 40)
                                {
                                    friend = true;
                                }
                                else if (relation.LTR.Liking != 0)
                                {
                                    known = true;
                                }
                            }
                        }

                        if (family)
                        {
                            // return new Color(0, 255, 255);
                            return new Color(Tagger.Settings.mRelationshipColorSettings[TagDataHelper.TagRelationshipType.Family]);
                        }
                        else if (romantic)
                        {
                            //return new Color(255, 128, 255);
                            return new Color(Tagger.Settings.mRelationshipColorSettings[TagDataHelper.TagRelationshipType.Romantic]);
                        }
                        else if (career)
                        {
                            //return new Color(128, 0, 255);
                            return new Color(Tagger.Settings.mRelationshipColorSettings[TagDataHelper.TagRelationshipType.Coworker]);
                        }
                        else if (friend)
                        {
                            if (enemy)
                            {
                                // ?? how does this happen
                                return new Color(Tagger.Settings.mRelationshipColorSettings[TagDataHelper.TagRelationshipType.Enemy]);
                            }
                            else
                            {
                                // return new Color(255, 128, 0);
                                return new Color(Tagger.Settings.mRelationshipColorSettings[TagDataHelper.TagRelationshipType.Friend]);
                            }
                        }
                        else if (enemy)
                        {
                            //return new Color(255, 0, 0);
                            return new Color(Tagger.Settings.mRelationshipColorSettings[TagDataHelper.TagRelationshipType.Enemy]);
                        }
                        else if (known)
                        {
                            //return new Color(255, 255, 0);
                            return new Color(Tagger.Settings.mRelationshipColorSettings[TagDataHelper.TagRelationshipType.Acquaintance]);
                        }

                    }
                    else
                    {
                        return base.ShadeColor;
                    }

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
                    Lot target = Target as Lot;
                    if ((target == null) || (target.Household == null))
                    {
                        return MapTagFilterType.None;
                    }

                    if ((Sim.ActiveActor == null) || (Sim.ActiveActor.Household == null))
                    {
                        return MapTagFilterType.None;
                    }

                    if (target.CanSimTreatAsHome(Sim.ActiveActor))
                    {
                        return ~MapTagFilterType.None;
                    }

                    MapTagFilterType result = MapTagFilterType.PublicSpacesAndActivities;

                    foreach (SimDescription sim in Households.All(target.Household))
                    {
                        Relationship relation = Relationship.Get(Sim.ActiveActor.SimDescription, sim, false);
                        if (relation != null)
                        {
                            if (relation.AreFriends())
                            {
                                result |= MapTagFilterType.FriendsHomes;
                            }
                        }

                        if (Relationships.IsCoworkerOrBoss(Sim.ActiveActor.Occupation, sim))
                        {
                            result |= MapTagFilterType.HouseholdAndWork;
                            result |= MapTagFilterType.Work;
                        }

                        if (Relationships.IsCoworkerOrBoss(Sim.ActiveActor.School, sim))
                        {
                            result |= MapTagFilterType.HouseholdAndWork;
                            result |= MapTagFilterType.Work;
                        }
                    }

                    return result;
                }
                catch (Exception exception)
                {
                    Common.Exception(Sim.ActiveActor, Target, exception);
                    return MapTagFilterType.None;
                }
            }
        }
    }
}