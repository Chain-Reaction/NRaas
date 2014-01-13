using NRaas.WoohooerSpace.Interactions;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.ChildAndTeenUpdates;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Plumbing;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.WoohooerSpace.Helpers
{
    public class ShowerEx
    {
        public static void ApplyPostShowerEffects(Sim actor, IShowerable shower)
        {
            BuffManager buffManager = actor.BuffManager;
            buffManager.RemoveElement(BuffNames.Singed);
            buffManager.RemoveElement(BuffNames.SingedElectricity);
            buffManager.RemoveElement(BuffNames.GarlicBreath);
            SimDescription simDescription = actor.SimDescription;
            simDescription.RemoveFacePaint();
            if (simDescription.IsMummy)
            {
                buffManager.AddElement(BuffNames.Soaked, Origin.FromShower);
            }

            if (RandomUtil.RandomChance((float)shower.TuningShower.ChanceOfExhileratingShowerBuff))
            {
                buffManager.AddElement(BuffNames.ExhilaratingShower, Origin.FromNiceShower);
            }

            if (actor.HasTrait(TraitNames.Hydrophobic))
            {
                actor.PlayReaction(ReactionTypes.Cry, shower as GameObject, ReactionSpeed.AfterInteraction);
            }
            else if (shower.ShouldGetColdShower)
            {
                actor.BuffManager.AddElement(BuffNames.ColdShower, Origin.FromCheapShower);
                EventTracker.SendEvent(EventTypeId.kGotColdShowerBuff, actor, shower);
            }
            // Custom
            else if ((shower.Cleanable != null) && (shower.Cleanable.NeedsToBeCleaned))
            {
                actor.PlayReaction(ReactionTypes.Retch, shower as GameObject, ReactionSpeed.AfterInteraction);
            }
            actor.Motives.SetMax(CommodityKind.Hygiene);
        }
    }
}
