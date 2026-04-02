using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Interactions;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Beds;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay;
using static Sims3.Gameplay.Objects.Electronics.TV;
using Sims3.Gameplay.Socializing;
using Sims3.UI.Controller;
using Sims3.UI;

namespace NRaas.OverwatchSpace.Interactions
{
    public class PlayVideoGameSystemOnTVOnlineEx : TV.PlayVideoGameSystemOnTVOnline, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<Sim, TV.PlayVideoGameSystemOnTVOnline.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition(TV.MultiplayerGameType.None);
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Sim, TV.PlayVideoGameSystemOnTVOnline.Definition>(Singleton);
        }
        private static new List<Sim> GetPotentialPartners(Sim actor, TV tv)
        {
            List<Sim> list = new List<Sim>();

            if (actor == null) return list;
            if (tv == null) return list;

            foreach (Sim actor2 in LotManager.Actors)
            {
                if (actor2 == null) continue;

                if (actor2 != actor && !actor2.IsPerformingAService && actor2.IsHuman && actor2.SimDescription.YoungAdultOrAbove && (actor2.SimDescription.DeathStyle == SimDescription.DeathType.None || actor2.SimDescription.IsPlayableGhost) && !actor2.SimDescription.HasActiveRole && !actor2.IsSleeping && !Sims3.Gameplay.Passport.Passport.IsPassportSim(actor2) && (actor2.InteractionQueue != null && !(actor2.InteractionQueue.GetCurrentInteraction() is IShowSlave)))
                {
                    Relationship relationship = Relationship.Get(actor, actor2, createIfNone: false);
                    if (relationship == null) continue;
                    
                    if (relationship.CurrentLTR != LongTermRelationshipTypes.Stranger && relationship.CurrentLTR != LongTermRelationshipTypes.Enemy && relationship.CurrentLTR != LongTermRelationshipTypes.OldEnemies && !(relationship.CurrentLTRLiking <= 0f) && tv.LotCurrent != actor2.LotCurrent && !actor2.IsAtWork && actor2.School == null && !(actor2.CurrentInteraction is RabbitHole.AttendClassInRabbitHole))
                    {
                        list.Add(actor2);
                    }
                }
            }
            return list;
        }

        public new class Definition : TV.PlayVideoGameSystemOnTVOnline.Definition
        {
            public Definition()
            { }
            public Definition(TV.MultiplayerGameType type)
            {
                base.mGameType = type;
            }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new PlayVideoGameSystemOnTVOnlineEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, TV target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }
            public override void AddInteractions(InteractionObjectPair iop, Sim actor, TV target, List<InteractionObjectPair> results)
            {
                results.Add(new InteractionObjectPair(new Definition(MultiplayerGameType.Online), iop.Target));
                results.Add(new InteractionObjectPair(new Definition(MultiplayerGameType.OnlineWith), iop.Target));
            }
            public override void PopulatePieMenuPicker(ref InteractionInstanceParameters parameters, out List<ObjectPicker.TabInfo> listObjs, out List<ObjectPicker.HeaderInfo> headers, out int NumSelectableRows)
            {
                PlayVideoGameSystemOnTVOnlineEx playVideoGameSystemOnTVOnline = this.CreateInstance(ref parameters) as PlayVideoGameSystemOnTVOnlineEx;
                if (!playVideoGameSystemOnTVOnline.Autonomous && base.mGameType == MultiplayerGameType.OnlineWith)
                {
                    TV closestObject = GlobalFunctions.GetClosestObject<TV>(playVideoGameSystemOnTVOnline.Target, skipObjectsInUse: false, searchAllRooms: false, playVideoGameSystemOnTVOnline.Target.RoomId, null, null);
                    NumSelectableRows = 1;
                    Sim actor = parameters.Actor as Sim;
                    PopulateSimPicker(ref parameters, out listObjs, out headers, PlayVideoGameSystemOnTVOnlineEx.GetPotentialPartners(actor, closestObject), includeActor: false);
                }
                else
                {
                    base.PopulatePieMenuPicker(ref parameters, out listObjs, out headers, out NumSelectableRows);
                }
            }

            public override bool Test(Sim a, TV target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if ((isAutonomous && a.HasTrait(TraitNames.AntiTV)) || (isAutonomous && target.bIsBeingUsedForWorkOut))
                {
                    return false;
                }
                bool flag = target.IntegratedVideoGameSystem;
                VideoGameSystemBase closestObject = GlobalFunctions.GetClosestObject<VideoGameSystemBase>(target, skipObjectsInUse: false, searchAllRooms: false, target.RoomId, null, null);
                if (!flag && closestObject != null)
                {
                    flag = GlobalFunctions.ObjectsWithinRadiusOfEachOther(closestObject, target, closestObject.UsableRadiusAroundVGS);
                }
                if (a.SkillManager.GetSkillLevel(SkillNames.InfluenceNerd) < kNerdUnlockLevel)
                {
                    return false;
                }
                if (flag && !target.CanSimPlayVideoGames(a, mGameType, ref greyedOutTooltipCallback))
                {
                    return false;
                }
                if (!flag)
                {
                    greyedOutTooltipCallback = VgsFarAway;
                    return false;
                }
                if (target.Repairable != null && target.Repairable.Broken)
                {
                    greyedOutTooltipCallback = BrokenTv;
                    return false;
                }
                if (mGameType == MultiplayerGameType.OnlineWith && PlayVideoGameSystemOnTVOnlineEx.GetPotentialPartners(a, target).Count == 0)
                {
                    greyedOutTooltipCallback = NoOneOnline;
                    return false;
                }
                return true;
            }
        }
    }
}