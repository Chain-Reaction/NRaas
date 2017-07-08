using NRaas.CommonSpace.Helpers;
using NRaas.TaggerSpace.Helpers;
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
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.TaggerSpace.MapTags
{
    public class TrackedSim : MapTag
    {
        static Common.MethodStore sGetIsClanLeader = new Common.MethodStore("NRaasStoryProgression", "NRaas.StoryProgression", "IsPersonalityLeader", new Type[] { typeof(SimDescription) });
        
        public TrackedSim(Sim targetsim, Sim owner)
            : base(targetsim, owner)
        { }

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
                Sim target = Target as Sim;

                if (target != null)
                {
                    if (target.FullName == "Nathaniel Raas")
                    {
                        return true;
                    }

                    if (NRaas.Tagger.Settings.mHotspotPersonalityTags)
                    {
                        if (sGetIsClanLeader.Valid && CameraController.IsMapViewModeEnabled())
                        {
                            return sGetIsClanLeader.Invoke<bool>(new object[] { target.SimDescription });
                        }
                    }
                }

                return base.ShowHotSpotGlow;
            }
        }

        public override MapTagType TagType
        {
            get
            {
                try
                {
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
                            
                            if (!Tagger.Settings.mSubtleTaggedSims && Tagger.Settings.mTaggedSims.Contains(target.SimDescription.SimDescriptionId))
                            {
                                mType = MapTagType.PrivateEyeCase;
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

                    return TagDataHelper.GetStatus(target.SimDescription);
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
                        string hours = TagDataHelper.GetRoleHours(target.SimDescription);
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
                    Sim target = Target as Sim;
                    if ((target == null) || (Sim.ActiveActor == null) || (target == Sim.ActiveActor) || (target.SimDescription == null) || (target.HasBeenDestroyed))
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

                    if (target.FullName == "Nathaniel Raas")
                    {
                        return new Color(4294916352);
                    }

                    if (Tagger.Settings.mColorTagsByAge)
                    {
                        switch (target.SimDescription.Age)
                        {
                            case CASAgeGenderFlags.Baby:
                                // White
                                // return new Color(255, 255, 255);
                                return new Color(Tagger.Settings.mAgeColorSettings[CASAgeGenderFlags.Baby]);
                            case CASAgeGenderFlags.Toddler:
                                // Orange
                                //return new Color(255, 128, 0);
                                return new Color(Tagger.Settings.mAgeColorSettings[CASAgeGenderFlags.Toddler]);
                            case CASAgeGenderFlags.Child:
                                return new Color(Tagger.Settings.mAgeColorSettings[CASAgeGenderFlags.Child]);
                            case CASAgeGenderFlags.Teen:
                                // Red
                                //return new Color(255, 0, 0);
                                return new Color(Tagger.Settings.mAgeColorSettings[CASAgeGenderFlags.Teen]);
                            case CASAgeGenderFlags.YoungAdult:
                                // Yellow
                                // return new Color(255, 255, 0);
                                return new Color(Tagger.Settings.mAgeColorSettings[CASAgeGenderFlags.YoungAdult]);
                            case CASAgeGenderFlags.Adult:
                                // Cyan
                                //return new Color(0, 255, 255);
                                return new Color(Tagger.Settings.mAgeColorSettings[CASAgeGenderFlags.Adult]);
                            case CASAgeGenderFlags.Elder:
                                // Purple
                                //return new Color(128, 0, 255);
                                return new Color(Tagger.Settings.mAgeColorSettings[CASAgeGenderFlags.Elder]);
                        }
                    }

                    if (Tagger.Settings.mColorTagsByRelationship)
                    {
                        if (Relationships.IsCloselyRelated(Sim.ActiveActor.SimDescription, target.SimDescription, false))
                        {
                            // Cyan
                            //return new Color(0, 255, 255);
                            return new Color(Tagger.Settings.mRelationshipColorSettings[TagDataHelper.TagRelationshipType.Family]);
                        }

                        Relationship relation = Relationship.Get(Sim.ActiveActor, target, false);
                        if (relation == null)
                        {
                            // White
                            return new Color(Tagger.Settings.mRelationshipColorSettings[TagDataHelper.TagRelationshipType.Unknown]);
                        }
                        else if (relation.AreRomantic())
                        {
                            // Pink
                            //return new Color(255, 128, 255);
                            return new Color(Tagger.Settings.mRelationshipColorSettings[TagDataHelper.TagRelationshipType.Romantic]);
                        }
                        else if (Relationships.IsCoworkerOrBoss(Sim.ActiveActor.Occupation, target.SimDescription))
                        {
                            // Purple
                            //return new Color(128, 0, 255);
                            return new Color(Tagger.Settings.mRelationshipColorSettings[TagDataHelper.TagRelationshipType.Coworker]);
                        }
                        else if (relation.LTR.Liking <= -20)
                        {
                            // Red
                            //return new Color(255, 0, 0);
                            return new Color(Tagger.Settings.mRelationshipColorSettings[TagDataHelper.TagRelationshipType.Enemy]);
                        }
                        else if (relation.LTR.Liking >= 40)
                        {
                            // Orange
                            //return new Color(255, 128, 0);
                            return new Color(Tagger.Settings.mRelationshipColorSettings[TagDataHelper.TagRelationshipType.Friend]);
                        }
                        else if (relation.LTR.Liking != 0)
                        {
                            // Yellow
                            //return new Color(255, 255, 0);
                            return new Color(Tagger.Settings.mRelationshipColorSettings[TagDataHelper.TagRelationshipType.Acquaintance]);
                        }
                        else if (Relationships.IsCoworkerOrBoss(Sim.ActiveActor.School, target.SimDescription))
                        {
                            // Purple
                            //return new Color(128, 0, 255);
                            return new Color(Tagger.Settings.mRelationshipColorSettings[TagDataHelper.TagRelationshipType.Coworker]);
                        }
                    }

                    if (Tagger.Settings.mColorTagsBySimType)
                    {
                        if (SimTypes.Matches(target.SimDescription, SimType.Occult) && SimTypes.Matches(target.SimDescription, SimType.Hybrid))
                        {
                            return new Color(Tagger.Settings.mSimTypeColorSettings[SimType.Hybrid]);
                        }

                        // IsDead returns false on playable ghosts
                        if (target.SimDescription.IsPlayableGhost)
                        {
                            return new Color(Tagger.Settings.mSimTypeColorSettings[SimType.Dead]);
                        }

                        foreach (SimType flag in Enum.GetValues(typeof(SimType)))
                        {
                            if (flag == SimType.Service && target.SimDescription.IsWildAnimal)
                            {
                                continue;
                            }

                            if (flag == SimType.Service && target.SimDescription.HasActiveRole)
                            {
                                continue;
                            }

                            if (flag == SimType.Horse && target.SimDescription.IsWildAnimal)
                            {
                                continue;
                            }

                            if (flag == SimType.Human)
                            {
                                continue;
                            }

                            if (SimTypes.Matches(target.SimDescription, flag) && Tagger.Settings.mSimTypeColorSettings.ContainsKey(flag))
                            {
                                return new Color(Tagger.Settings.mSimTypeColorSettings[flag]);
                            }
                        }

                        return new Color(Tagger.Settings.mSimTypeColorSettings[SimType.Human]);
                    }

                    /*
                     * Custom Tag Colors:
                     * F2666C - japan rest, FF4E20 - toy, DC3023 - rest, FF0000 - pizza, 407A52 - garden, 317589 - carnival, F3C13A - deli, 7C532F - bakery
                     * 
                     * Orientation:
                     * A3A3A3 asex, FFCC40 bic, 9B4F96 bis, 0074C5 str8, FF8C00 gay, C6C2B6 undecided
                     * 
                     * Rel Status:
                     * f80622 single, 06f8de preg, girl f8539d, boy 539df8, partnered eb7812, faf214 married
                     * 
                     * SimType:
                     * service 3e4352, dead 403f37, tourist f0d80f, mummy 1b1917, simbot 995f0b, human d7d4d4, vampire ab0f16, imaginaryfriend 9fc7e9
                     * genie 6422b6, fairy 22b664, werewolf 7d5827, witch aba72c, zombie 103119, bonehilda 373a38, alien 69bc81, hybrid c9e31c
                     * plantsim 158d48, mermaid 0363ce, plumbot 8d8a87, deer a0774b, wildhorse 63e4a6, role e463dc, dog 51514e, cat d4d294, 
                     * horse 957B38, raccoon C6C2B6, stray 757D75
                     * 
                     * Age and Active Relationship types are commented out above
                     * 
                     */

                    if (target.SimDescription.TeenOrAbove && !target.SimDescription.IsPet)
                    {
                        if (Tagger.Settings.mColorTagsByRelationshipStatus)
                        {
                            if (target.SimDescription.IsPregnant && target.SimDescription.Pregnancy != null)
                            {
                                if (Tagger.Settings.mColorPregnancyTag)
                                {
                                    if (target.SimDescription.Pregnancy.GetCurrentBabyGender() == CASAgeGenderFlags.Male)
                                    {
                                        return new Color(4283670008);
                                    }
                                    else if (target.SimDescription.Pregnancy.GetCurrentBabyGender() == CASAgeGenderFlags.Female)
                                    {
                                        return new Color(4294464413);
                                    }
                                }

                                return new Color(Tagger.Settings.mSimStatusColorSettings[SimType.Pregnant]);
                            }

                            if (target.SimDescription.Partner == null)
                            {
                                return new Color(Tagger.Settings.mSimStatusColorSettings[SimType.Single]);
                            }
                            else
                            {
                                if (target.SimDescription.IsMarried)
                                {
                                    return new Color(Tagger.Settings.mSimStatusColorSettings[SimType.Married]);
                                }

                                return new Color(Tagger.Settings.mSimStatusColorSettings[SimType.Partnered]);
                            }
                        }

                        if (Tagger.Settings.mColorTagsByOrientation)
                        {
                            TagDataHelper.TagOrientationType type = TagDataHelper.GetOrientation(target.SimDescription);
                            if (Tagger.Settings.mSimOrientationColorSettings.ContainsKey(type))
                            {
                                return new Color(Tagger.Settings.mSimOrientationColorSettings[type]);
                            }
                        }

                        if (Tagger.Settings.mColorByJobPerformance)
                        {
                            if (target.Occupation != null)
                            {
                                float performance = (target.Occupation.Performance - -100) / (100 - -100) * 100;
                                
                                int performance2 = (int)Math.Floor(performance);
                                int inverted = (performance2 - 100) * -1;
                                
                                return TagDataHelper.ColorizePercent(inverted);
                            }
                        }

                        if (Tagger.Settings.mColorByCash)
                        {
                            int wealthPercent = 0;
                            if (TagDataHelper.moneyGraph.TryGetValue(target.SimDescription.SimDescriptionId, out wealthPercent))
                            {                               
                                int inverted = wealthPercent - 100 * -1;
                                
                                return TagDataHelper.ColorizePercent(inverted);
                            }
                        }
                    }

                    if (Tagger.Settings.mColorByMood)
                    {                        
                        if (target.MoodManager != null)
                        {
                            int mood = (target.MoodManager.MoodValue - -100);
                            float m = (float)mood / (200 - -100) * 100;
                            int mood2 = (int)Math.Ceiling(m);
                            int mood3 = (mood2 - 100) * -1;
                            
                            return TagDataHelper.ColorizePercent(mood3);
                        }
                    }

                    if (Tagger.Settings.mColorByCommodity != CommodityKind.None)
                    {
                        if (target.Autonomy != null && target.Autonomy.Motives.HasMotive(Tagger.Settings.mColorByCommodity))
                        {
                            float motive = (target.Autonomy.Motives.GetValue(Tagger.Settings.mColorByCommodity) - -100) / (100 - -100) * 100;
                            int motive2 = (int)Math.Ceiling(motive);
                            int motive3 = (motive2 - 100) * -1;
                            
                            // fanciness for vampires and temps
                            if (Tagger.Settings.mColorByCommodity == CommodityKind.VampireThirst)
                            {
                                if (motive2 > 50)
                                {
                                    return TagDataHelper.ColorizePercentCustom(new Color(119, 17, 132), new Color(237, 237, 33), motive3);
                                }
                                else
                                {
                                    return TagDataHelper.ColorizePercentCustom(new Color(229, 29, 22), new Color(237, 237, 33), motive3);
                                }
                            }

                            if (Tagger.Settings.mColorByCommodity == CommodityKind.Temperature)
                            {
                                if (motive2 > 50)
                                {
                                    return TagDataHelper.ColorizePercentCustom(new Color(229, 29, 22), new Color(237, 237, 33), motive3);
                                }
                                else
                                {
                                    // 24,  201, 24g
                                    return TagDataHelper.ColorizePercentCustom(new Color(21, 82, 153), new Color(237, 237, 33), motive3);
                                }
                            }

                            return TagDataHelper.ColorizePercent(motive3);
                        }
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

                    if (Relationships.IsCoworkerOrBoss(Sim.ActiveActor.Occupation, target.SimDescription))
                    {
                        result |= MapTagFilterType.HouseholdAndWork;
                        result |= MapTagFilterType.Work;
                    }

                    if (Relationships.IsCoworkerOrBoss(Sim.ActiveActor.School, target.SimDescription))
                    {
                        result |= MapTagFilterType.HouseholdAndWork;
                        result |= MapTagFilterType.Work;
                    }
                    return result;
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