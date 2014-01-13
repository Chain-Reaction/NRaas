using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.WoohooerSpace.Skills;
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
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.WoohooerSpace.Interactions
{
    public class Tantraport : Interaction<Sim, Sim>, Common.IAddInteraction
    {
        static InteractionDefinition Singleton = new Definition();

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<Sim>(Singleton);
        }

        public override bool Run()
        {
            Common.StringBuilder msg = new Common.StringBuilder("Tantraport:Run");

            try
            {
                SimDescription choice = GetChoices(Actor, Common.Localize("Tantraport:MenuName", Target.IsFemale));
                if (choice == null) return false;

                TerrainInteraction instance = Terrain.TeleportMeHere.Singleton.CreateInstance(Terrain.Singleton, Target, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, true) as TerrainInteraction;

                Vector3 frontDoorPosition = Vector3.Invalid;

                Door frontDoor = choice.CreatedSim.LotCurrent.FindFrontDoor();
                if (frontDoor != null)
                {
                    frontDoorPosition = frontDoor.Position;

                    msg += "A";
                }

                Lot lot = choice.CreatedSim.LotCurrent;

                Vector3[] vectorArray = new Vector3[] { choice.CreatedSim.Position, frontDoorPosition, lot.Position, (lot.Corners[Corner.Origin] + lot.Corners[Corner.XAxis]) / 2f, (lot.Corners[Corner.XAxis] + lot.Corners[Corner.Extent]) / 2f, (lot.Corners[Corner.YAxis] + lot.Corners[Corner.Extent]) / 2f, (lot.Corners[Corner.Origin] + lot.Corners[Corner.YAxis]) / 2f };
                for (int i = 0x0; i < vectorArray.Length; i++)
                {
                    if (vectorArray[i] == Vector3.Invalid) continue;

                    Vector3 forward;
                    World.FindGoodLocationParams fglParams = new World.FindGoodLocationParams(vectorArray[i]);
                    if (GlobalFunctions.FindGoodLocation(Target, fglParams, out instance.Destination, out forward))
                    {
                        msg += "B";

                        Target.InteractionQueue.PushAsContinuation(instance, true);
                        return true;
                    }
                }

                return true;
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, msg, e);
                return false;
            }
            finally
            {
                Common.DebugNotify(msg);
            }
        }

        public static List<SimDescription> GetChoices(Sim actor)
        {
            List<SimDescription> choices = new List<SimDescription>();

            foreach (Relationship relation in Relationship.GetRelationships(actor))
            {
                if (!relation.AreRomantic()) continue;

                SimDescription target = relation.GetOtherSimDescription(actor.SimDescription);
                if (target == null) continue;

                Sim targetSim = target.CreatedSim;
                if (targetSim == null) continue;

                if (targetSim.LotCurrent == null) continue;

                if (targetSim.LotCurrent.IsWorldLot) continue;

                if (targetSim.CurrentInteraction is ICountsAsWorking) continue;

                if (targetSim.CurrentInteraction is ISleeping) continue;

                choices.Add(target);
            }

            return choices;
        }

        public static SimDescription GetChoices(Sim actor, string title)
        {
            List<SimDescription> choices = GetChoices(actor);
            if (choices.Count == 0)
            {
                Common.Notify(actor, Common.Localize("Tantraport:NoChoices"));
                return null;
            }

            bool okayed;
            SimDescription result = new SimSelection(title, actor.SimDescription, choices, SimSelection.Type.Tantraport, 0).SelectSingle(out okayed);

            if (!okayed) return null;

            if (result == null)
            {
                result = RandomUtil.GetRandomObjectFromList(choices);
            }

            return result;
        }

        private class Definition : SoloSimInteractionDefinition<Tantraport>
        {
            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return Common.Localize("Tantraport:MenuName", target.IsFemale);
            }

            public override bool Test(Sim actor, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (actor != target) return false;

                if (actor.SkillManager.GetSkillLevel(KamaSimtra.StaticGuid) < KamaSimtra.Settings.mMinLevelTantraport)
                {
                    greyedOutTooltipCallback = Common.DebugTooltip("Skill Too Low");
                    return false;
                }

                return base.Test(actor, target, isAutonomous, ref greyedOutTooltipCallback);
            }
        }
    }
}
