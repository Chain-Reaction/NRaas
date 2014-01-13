using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Options;
using NRaas.StoryProgressionSpace.Personalities;
using NRaas.StoryProgressionSpace.Scenarios;
using NRaas.StoryProgressionSpace.Scenarios.Sims;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.MapTags;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.MapTags
{
    public class TrackedSim : MapTag
    {
        ulong mCount = 0;

        private static CommodityKind[] sMotives = new CommodityKind[] 
        { 
            CommodityKind.Hunger, 
            CommodityKind.Energy, 
            CommodityKind.Bladder, 
            CommodityKind.Hygiene, 
            CommodityKind.Fun, 
            CommodityKind.Social 
        };

        public TrackedSim(Sim targetsim, Sim owner) 
            : base(targetsim, owner)
        {}

        public override void ClickedOn(UIMouseEventArgs eventArgs)
        {
            Sim target = this.Target as Sim;
            if (((target == null) || (target.Household == null)) || (target.Household.LotHome == null))
            {
                target = null;
            }
            if ((target != null) && 
                (eventArgs.MouseKey == MouseKeys.kMouseRight) && 
                (eventArgs.Modifiers == (Modifiers.kModifierMaskNone | Modifiers.kModifierMaskControl)))
            {
                PlumbBob.ForceSelectActor(target);
            }
            else
            {
                base.ClickedOn(eventArgs);
            }
        }

        public override float RelationshipLevel
        {
            get
            {
                try
                {
                    using (Common.TestSpan span = new Common.TestSpan(NRaas.StoryProgression.Main.Scenarios, "TrackedSim RelationshipLevel"))
                    {
                        Sim target = Target as Sim;
                        if ((target == null) || (target.HasBeenDestroyed))
                        {
                            return base.RelationshipLevel;
                        }
                        Relationship relationship = Owner.GetRelationship(target, false);
                        if (relationship != null)
                        {
                            return relationship.LTR.Liking;
                        }
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(Sim.ActiveActor, Target, e);
                }

                return 0f;
            }
        }

        public override bool ShowHotSpotGlow
        {
            get
            {
                using (Common.TestSpan span = new Common.TestSpan(NRaas.StoryProgression.Main.Scenarios, "TrackedSim ShowHotSpotGlow"))
                {
                    if (NRaas.StoryProgression.Main.GetValue<Scenarios.Sims.HandleMapTagsScenario.PersonalityTagOption, bool>())
                    {
                        Sim target = Target as Sim;

                        if (NRaas.StoryProgression.Main.Personalities.GetClanLeadership(target.SimDescription).Count > 0)
                        {
                            return true;
                        }
                    }

                    return base.ShowHotSpotGlow;
                }
            }
        }

        public override MapTagType TagType
        {
            get
            {
                try
                {
                    if (mCount < HandleMapTagsScenario.sCount)
                    {
                        using (Common.TestSpan span = new Common.TestSpan(NRaas.StoryProgression.Main.Scenarios, "TrackedSim TagType"))
                        {
                            mCount = HandleMapTagsScenario.sCount;

                            mType = MapTagType.NPCSim;

                            Sim target = Target as Sim;
                            if ((target != null) && (!target.HasBeenDestroyed))
                            {
                                Sim active = Sims3.Gameplay.Actors.Sim.ActiveActor;

                                if ((target.SimDescription != null) && (active != null))
                                {
                                    if (target.Household == Household.ActiveHousehold)
                                    {
                                        if (target == active)
                                        {
                                            mType = MapTagType.SelectedSim;
                                        }
                                        else
                                        {
                                            mType = MapTagType.FamilySim;
                                        }
                                    }
                                    else if ((target.LotHome == null) && (target.SimDescription.AssignedRole is Proprietor))
                                    {
                                        mType = MapTagType.Proprietor;
                                    }
                                    else if ((target.SimDescription.IsCelebrity) && (target.SimDescription.CelebrityLevel > 3))
                                    {
                                        mType = MapTagType.Celebrity;
                                    }
                                    else if (active.OccultManager != null)
                                    {
                                        OccultVampire vampire = active.OccultManager.GetOccultType(Sims3.UI.Hud.OccultTypes.Vampire) as OccultVampire;
                                        if ((vampire != null) && (vampire.PreyMapTag != null))
                                        {
                                            if (target == vampire.PreyMapTag.Target)
                                            {
                                                mType = MapTagType.VampirePrey;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(Sim.ActiveActor, Target, e);
                }

                return mType;
            }            
        }

        public override string HoverText
        {
            get
            {
                string str = null;

                try
                {
                    Sim target = Target as Sim;

                    return StoryProgression.Main.Sims.GetStatus(target.SimDescription);
                }
                catch (Exception exception)
                {
                    Common.Exception(Target, exception);
                }

                return str;
            }
        }

        public override string Hours
        {
            get
            {
                try
                {
                    Sim target = Target as Sim;
                    if (target != null)
                    {
                        string hours = ManagerSim.GetRoleHours(target.SimDescription);
                        if (!string.IsNullOrEmpty(hours))
                        {
                            return hours;
                        }
                    }

                    return base.Hours;
                }
                catch (Exception e)
                {
                    Common.Exception(Target, e);
                    return null;
                }
            }
        }

        public override string HouseholdName
        {
            get
            {
                try
                {
                    Sim target = Target as Sim;
                    if (target != null)
                    {
                        if (target.SimDescription.AssignedRole is Proprietor)
                        {
                            GameObject roleGivingObject = target.SimDescription.AssignedRole.RoleGivingObject as GameObject;
                            if ((roleGivingObject != null) && (roleGivingObject.LotCurrent != null))
                            {
                                return Common.LocalizeEAString(false, "Gameplay/MapTags/MapTag:LotNameWithProprietor", new object[] { target, roleGivingObject.LotCurrent.Name });
                            }
                        }
                        else if ((target.Household != null) && (!SimTypes.IsSpecial(target.Household)))
                        {
                            return target.Household.Name;
                        }
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(Target, e);
                }

                return string.Empty;
            }
        }

        public override string LotName
        {
            get
            {
                try
                {
                    Sim target = Target as Sim;
                    if (target != null)
                    {
                        GameObject roleGivingObject = target.SimDescription.AssignedRole.RoleGivingObject as GameObject;
                        if (roleGivingObject != null)
                        {
                            string str = string.Empty;
                            switch (roleGivingObject.LotCurrent.CommercialLotSubType)
                            {
                                case CommercialLotSubType.kSmallPark:
                                case CommercialLotSubType.kBigPark:
                                    return Common.LocalizeEAString("Gameplay/Excel/Venues/CommuntiyTypes:BigPark");

                                case CommercialLotSubType.kEP6_Bistro:
                                    return Common.LocalizeEAString("Gameplay/Excel/Venues/CommuntiyTypes:kEP6_Bistro");

                                case CommercialLotSubType.kEP6_PerformanceClub:
                                    return Common.LocalizeEAString("Gameplay/Excel/Venues/CommuntiyTypes:kEP6_PerformanceClub");

                                case CommercialLotSubType.kEP6_PrivateVenue:
                                    return Common.LocalizeEAString("Gameplay/Excel/Venues/CommuntiyTypes:kEP6_PrivateVenue");

                                case CommercialLotSubType.kEP6_BigShow:
                                    return Common.LocalizeEAString("Gameplay/Excel/Venues/CommuntiyTypes:kEP6_BigShow");
                            }
                            return str;
                        }
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(Target, e);
                }

                return string.Empty;
            }
        }

        public override Color ShadeColor
        {
            get
            {
                try
                {
                    using (Common.TestSpan span = new Common.TestSpan(NRaas.StoryProgression.Main.Scenarios, "TrackedSim ShadeColor"))
                    {
                        Sim target = Target as Sim;
                        if ((target == null) || (Sim.ActiveActor == null) || (target == Sim.ActiveActor))
                        {
                            return base.ShadeColor;
                        }

                        if (Sim.ActiveActor.Household == target.Household)
                        {
                            return base.ShadeColor;
                        }

                        if (target.SimDescription.AssignedRole is RoleSpecialMerchant)
                        {
                            return new Color(0, 0, 0);
                        }
                        else if (target.SimDescription.AssignedRole is Proprietor)
                        {
                            return new Color(0, 0, 0);
                        }

                        if (StoryProgression.Main.GetValue<HandleMapTagsScenario.ColorOption, bool>())
                        {
                            if (StoryProgression.Main.GetValue<HandleMapTagsScenario.ColorByAgeOption, bool>())
                            {
                                switch (target.SimDescription.Age)
                                {
                                    case CASAgeGenderFlags.Baby:
                                        // White
                                        return new Color(255, 255, 255);
                                    case CASAgeGenderFlags.Toddler:
                                        // Orange
                                        return new Color(255, 128, 0);
                                    case CASAgeGenderFlags.Teen:
                                        // Red
                                        return new Color(255, 0, 0);
                                    case CASAgeGenderFlags.YoungAdult:
                                        // Yellow
                                        return new Color(255, 255, 0);
                                    case CASAgeGenderFlags.Adult:
                                        // Cyan
                                        return new Color(0, 255, 255);
                                    case CASAgeGenderFlags.Elder:
                                        // Purple
                                        return new Color(128, 0, 255);
                                }
                            }
                            else
                            {
                                if (Relationships.IsCloselyRelated(Sim.ActiveActor.SimDescription, target.SimDescription, false))
                                {
                                    // Cyan
                                    return new Color(0, 255, 255);
                                }

                                Relationship relation = Relationship.Get(Sim.ActiveActor, target, false);
                                if (relation == null)
                                {
                                    // White
                                    return new Color(255, 255, 255);
                                }
                                else if (relation.AreRomantic())
                                {
                                    // Pink
                                    return new Color(255, 128, 255);
                                }
                                else if (ManagerCareer.IsCoworkerOrBoss(Sim.ActiveActor.Occupation, target.SimDescription))
                                {
                                    // Purple
                                    return new Color(128, 0, 255);
                                }
                                else if (relation.LTR.Liking <= (int)SimScenarioFilter.RelationshipLevel.Disliked)
                                {
                                    // Red
                                    return new Color(255, 0, 0);
                                }
                                else if (relation.LTR.Liking >= (int)SimScenarioFilter.RelationshipLevel.Friend)
                                {
                                    // Orange
                                    return new Color(255, 128, 0);
                                }
                                else if (relation.LTR.Liking != 0)
                                {
                                    // Yellow
                                    return new Color(255, 255, 0);
                                }
                                else if (ManagerCareer.IsCoworkerOrBoss(Sim.ActiveActor.School, target.SimDescription))
                                {
                                    // Purple
                                    return new Color(128, 0, 255);
                                }
                            }
                        }

                        return base.ShadeColor;
                    }
                }
                catch (Exception exception)
                {
                    Common.Exception(Target, exception);
                }

                // White
                return new Color(255, 255, 255);
            }
        }

        public override MapTagFilterType FilterType
        {
            get
            {
                try
                {
                    using (Common.TestSpan span = new Common.TestSpan(NRaas.StoryProgression.Main.Scenarios, "TrackedSim FilterType"))
                    {
                        Sim target = Target as Sim;
                        if ((target == null) || (target.SimDescription == null) || (target.HasBeenDestroyed))
                        {
                            return MapTagFilterType.None;
                        }

                        if (Sim.ActiveActor == null)
                        {
                            return MapTagFilterType.None;
                        }

                        if (target == Sim.ActiveActor)
                        {
                            return ~MapTagFilterType.None;
                        }

                        MapTagFilterType result = MapTagFilterType.PublicSpacesAndActivities;

                        Relationship relation = Relationship.Get(Sim.ActiveActor, target, false);
                        if (relation != null)
                        {
                            if (relation.AreFriends())
                            {
                                result |= MapTagFilterType.FriendsHomes;
                            }
                        }

                        if (target.SimDescription.AssignedRole is Proprietor)
                        {
                            GameObject roleGivingObject = target.SimDescription.AssignedRole.RoleGivingObject as GameObject;
                            if (((roleGivingObject != null) && (roleGivingObject.LotCurrent != null)) && Occupation.IsLotAPerformanceCareerLocation(roleGivingObject.LotCurrent))
                            {
                                result |= MapTagFilterType.OpportunitiesAndJobs;
                            }
                        }

                        if (Sim.ActiveActor.Household == target.Household)
                        {
                            result |= MapTagFilterType.HouseholdAndWork;
                        }

                        if (ManagerCareer.IsCoworkerOrBoss(Sim.ActiveActor.Occupation, target.SimDescription))
                        {
                            result |= MapTagFilterType.HouseholdAndWork;
                            result |= MapTagFilterType.Work;
                        }

                        if (ManagerCareer.IsCoworkerOrBoss(Sim.ActiveActor.School, target.SimDescription))
                        {
                            result |= MapTagFilterType.HouseholdAndWork;
                            result |= MapTagFilterType.Work;
                        }
                        return result;
                    }
                }
                catch (Exception exception)
                {
                    Common.Exception(Target, exception);
                    return MapTagFilterType.None;
                }
            }
        }

        public override bool IsSimInRabbithole
        {
            get
            {
                if (MapTagManager.ActiveMapTagManager == null)
                {
                    return false;
                }

                Sim target = Target as Sim;

                return ((target.RabbitHoleCurrent != null) && MapTagManager.ActiveMapTagManager.HasTag(target.RabbitHoleCurrent));
            }
        }
    }
}

