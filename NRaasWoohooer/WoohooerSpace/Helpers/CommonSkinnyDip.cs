using NRaas.CommonSpace.Replacers;
using NRaas.WoohooerSpace.Helpers;
using NRaas.WoohooerSpace.Scoring;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Pools;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.TuningValues;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.WoohooerSpace.Helpers
{
    public class CommonSkinnyDip : Common.IPreLoad
    {
        public void OnPreLoad()
        {
            if (GameUtils.IsInstalled(ProductVersion.EP3))
            {
                SocialRHSReplacer.Perform<CommonSkinnyDip>("Ask To Go Skinny Dipping", "OnAskedToGoSkinnyDipping");

                ActionDataReplacer.Perform<CommonSkinnyDip>("TestAskToGoSkinnyDipping");
            }
        }

        public static bool CanSkinnyDipAtLocation(Sim a, Vector3 position, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            return CanSkinnyDipAtLocation(a, position, ref greyedOutTooltipCallback, false, false);
        }
        public static bool CanSkinnyDipAtLocation(Sim a, Vector3 position, ref GreyedOutTooltipCallback greyedOutTooltipCallback, bool okIfNoExistingSkinnyDippers, bool okIfAloneAndRomantic)
        {
            if (!GameUtils.IsInstalled(ProductVersion.EP3))
            {
                return false;
            }

            if (a.Posture is CarryingChildPosture)
            {
                return false;
            }
            if (a.HasTrait(TraitNames.NeverNude) || a.HasTrait(TraitNames.Shy))
            {
                return false;
            }

            if (Woohooer.Settings.mAllowTeenSkinnyDip)
            {
                if (a.SimDescription.ChildOrBelow)
                {
                    return false;
                }
            }
            else
            {
                if (a.SimDescription.TeenOrBelow)
                {
                    return false;
                }
            }
            /*
            if (a.SimDescription.IsVisuallyPregnant)
            {
                greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(Common.LocalizeEAString(a.IsFemale, "Gameplay/Actors/Sim:PregnantFailure", new object[0x0]));
              
                return false;
            }
            */
            if (!Pool.SimOutfitSupportsSkinnyDipping(a, ref greyedOutTooltipCallback))
            {
                return false;
            }

            bool flag = okIfNoExistingSkinnyDippers;
            bool notRomantic = false;
            bool result = false;
            bool kidsAround = false;
            LotLocation location = new LotLocation();
            ulong lotLocation = World.GetLotLocation(position, ref location);
            bool outside = World.IsPositionOutside(position);
            short mRoom = location.mRoom;

            List<Sim> lotSims = new List<Sim>();

            if (Woohooer.Settings.mEnforceSkinnyDipPrivacy)
            {
                Lot lot = LotManager.GetLot(lotLocation);
                if ((lot != null) && (!lot.IsWorldLot))
                {
                    lotSims = new List<Sim>(lot.GetSims());
                    if (outside)
                    {
                        List<Sim> list2 = new List<Sim>(Sims3.Gameplay.Queries.GetObjects<Sim>(position, Pool.kRadiusToCheckForKids));
                        foreach (Sim sim in list2)
                        {
                            if (sim.LotCurrent == null) continue;

                            if (sim.LotCurrent.LotId != lotLocation)
                            {
                                lotSims.Add(sim);
                            }
                        }
                    }
                }
            }

            foreach (Sim sim2 in lotSims)
            {
                if (sim2 == a) continue;
                
                if (sim2.SimDescription.ToddlerOrBelow) continue;

                bool checkAge = false;
                if (Woohooer.Settings.mAllowTeenSkinnyDip)
                {
                    if (sim2.SimDescription.ChildOrBelow)
                    {
                        checkAge = true;
                    }
                }
                else
                {
                    if (sim2.SimDescription.TeenOrBelow)
                    {
                        checkAge = true;
                    }
                }

                if (checkAge)
                {
                    if (sim2.LotCurrent == null) continue;

                    if (sim2.LotCurrent.LotId == lotLocation)
                    {
                        kidsAround = true;
                    }
                    else if (outside && sim2.IsOutside)
                    {
                        kidsAround = true;
                    }
                    continue;
                }

                if (sim2.IsSkinnyDipping())
                {
                    flag = true;
                }

                if (okIfAloneAndRomantic && (sim2.RoomId == mRoom))
                {
                    notRomantic = notRomantic || !sim2.IsInRomanticRelationshipWith(a);
                }
            }

            if (flag)
            {
                result = true;
            }

            if ((a.HasTrait(TraitNames.Daredevil) || a.HasTrait(TraitNames.PartyAnimal)) || a.HasTrait(TraitNames.Inappropriate))
            {
                result = true;
            }

            if (okIfAloneAndRomantic && !notRomantic)
            {
                result = true;
            }

            float hoursPassedOfDay = SimClock.HoursPassedOfDay;
            if ((hoursPassedOfDay <= (World.GetSunriseTime() + Pool.kTimeOffsetFromSunriseWhenCanGoSkinnyDip)) || (hoursPassedOfDay >= (World.GetSunsetTime() + Pool.kTimeOffsetFromSunsetWhenCanGoSkinnyDip)))
            {
                result = true;
            }

            if (result && kidsAround)
            {
                greyedOutTooltipCallback = new GrayedOutTooltipHelper(a.IsFemale, "KidsAroundTooltip", null).GetTooltip;
                return false;
            }

            return result;
        }

        public static bool TestAskToGoSkinnyDipping(Sim actor, Sim target, ActiveTopic topic, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
        {
            try
            {
                if (actor.LotCurrent == target.LotCurrent)
                {
                    if ((target.Posture is ISkinnyDippingPosture) || (actor.Posture is ISkinnyDippingPosture))
                    {
                        return false;
                    }
                    if (!Pool.SimOutfitSupportsSkinnyDipping(actor, ref greyedOutTooltipCallback) || !Pool.SimOutfitSupportsSkinnyDipping(target, ref greyedOutTooltipCallback))
                    {
                        return false;
                    }

                    /*
                    if (target.SimDescription.IsVisuallyPregnant)
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(Common.LocalizeEAString(target.IsFemale, "Gameplay/Actors/Sim:PregnantFailure", new object[0x0]));
                        return false;
                    }
                    */

                    foreach (Pool pool in actor.LotCurrent.GetObjects<Pool>())
                    {
                        #region CHANGED
                        if (CommonSkinnyDip.CanSkinnyDipAtLocation(actor, pool.Position, ref greyedOutTooltipCallback))
                        #endregion
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
            }
            return false;
        }

        public static void OnAskedToGoSkinnyDipping(Sim actor, Sim target, string interaction, ActiveTopic topic, InteractionInstance i)
        {
            try
            {
                GreyedOutTooltipCallback greyedOutTooltipCallback = null;
                Pool[] objects = actor.LotCurrent.GetObjects<Pool>();
                float num = 0f;
                Pool pool = null;

                foreach (Pool pool2 in objects)
                {
                    #region CHANGED
                    if (CanSkinnyDipAtLocation(actor, pool2.Position, ref greyedOutTooltipCallback))
                    #endregion
                    {
                        float distanceToObject = actor.GetDistanceToObject(pool2);
                        if ((pool == null) || (distanceToObject < num))
                        {
                            pool = pool2;
                            num = distanceToObject;
                        }
                    }
                }

                if (pool != null)
                {
                    GetInPool entry = GetInPool.SkinnyDipSingleton.CreateInstance(pool, actor, i.GetPriority(), true, true) as GetInPool;
                    GetInPool pool4 = GetInPool.SkinnyDipSingleton.CreateInstance(pool, target, i.GetPriority(), true, true) as GetInPool;
                    entry.SkinnyDippingBuddyID = target.SimDescription.SimDescriptionId;
                    pool4.SkinnyDippingBuddyID = actor.SimDescription.SimDescriptionId;
                    actor.InteractionQueue.AddNext(entry);
                    target.InteractionQueue.AddNext(pool4);
                }
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
            }
        }
    }
}
