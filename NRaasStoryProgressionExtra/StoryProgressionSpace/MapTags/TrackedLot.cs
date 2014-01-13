using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scenarios;
using NRaas.StoryProgressionSpace.Scenarios.Lots;
using NRaas.StoryProgressionSpace.Situations;
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
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.MapTags
{
    public class TrackedLot : NeighborLotMapTag, INpcPartyMapTag
    {
        ulong mCount = 0;

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
                }
                return null;
            }
        }

        protected static Party TryGetPartyOnLot(Lot lot)
        {
            NpcParty npcParty = NpcParty.TryGetNpcPartyOnLot(lot);
            if (npcParty != null) return npcParty;

            if (lot.Household != null)
            {
                foreach (HousePartySituation sit in Situation.GetSituations<HousePartySituation>())
                {
                    if (sit.Lot == lot) return sit;
                }
            }

            return null;
        }

        public override MapTagType TagType
        {
            get
            {
                try
                {
                    if (mCount < HandleMapTagsScenario.sCount)
                    {
                        using (Common.TestSpan span = new Common.TestSpan(NRaas.StoryProgression.Main.Scenarios, "TrackedLot TagType"))
                        {
                            mCount = HandleMapTagsScenario.sCount;

                            Lot lot = Target as Lot;
                            if (lot != null)
                            {
                                mType = MapTagType.NeighborLot;

                                Party npcParty = TryGetPartyOnLot(lot);
                                if (npcParty != null)
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
                    using (Common.TestSpan span = new Common.TestSpan(NRaas.StoryProgression.Main.Scenarios, "TrackedLot ShadeColor"))
                    {
                        Lot target = Target as Lot;
                        if ((target == null) || (target.Household == null) || (Sim.ActiveActor == null) || (target.Household == Sim.ActiveActor.Household))
                        {
                            return base.ShadeColor;
                        }

                        if (NRaas.StoryProgression.Main.GetValue<HandleMapTagsScenario.ColorOption, bool>())
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
                                    else if ((ManagerCareer.IsCoworkerOrBoss(Sim.ActiveActor.Occupation, member)) ||
                                             (ManagerCareer.IsCoworkerOrBoss(Sim.ActiveActor.School, member)))
                                    {
                                        career = true;

                                    }
                                    else if (relation.LTR.Liking <= (int)SimScenarioFilter.RelationshipLevel.Disliked)
                                    {
                                        enemy = true;
                                    }
                                    else if (relation.LTR.Liking >= (int)SimScenarioFilter.RelationshipLevel.Friend)
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
                                return new Color(0, 255, 255);
                            }
                            else if (romantic)
                            {
                                return new Color(255, 128, 255);
                            }
                            else if (career)
                            {
                                return new Color(128, 0, 255);
                            }
                            else if (friend)
                            {
                                if (enemy)
                                {
                                    return new Color(128, 128, 128);
                                }
                                else
                                {
                                    return new Color(255, 128, 0);
                                }
                            }
                            else if (enemy)
                            {
                                return new Color(255, 0, 0);
                            }
                            else if (known)
                            {
                                return new Color(255, 255, 0);
                            }

                        }
                        else
                        {
                            return base.ShadeColor;
                        }
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
                    using (Common.TestSpan span = new Common.TestSpan(NRaas.StoryProgression.Main.Scenarios, "TrackedLot FilterType"))
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

                            if (ManagerCareer.IsCoworkerOrBoss(Sim.ActiveActor.Occupation, sim))
                            {
                                result |= MapTagFilterType.HouseholdAndWork;
                                result |= MapTagFilterType.Work;
                            }

                            if (ManagerCareer.IsCoworkerOrBoss(Sim.ActiveActor.School, sim))
                            {
                                result |= MapTagFilterType.HouseholdAndWork;
                                result |= MapTagFilterType.Work;
                            }
                        }

                        return result;
                    }
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

