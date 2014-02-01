using NRaas.CommonSpace.Helpers;
using NRaas.WoohooerSpace.Helpers;
using NRaas.WoohooerSpace.Scoring;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Pools;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.WoohooerSpace.Interactions
{
    public class VisitRabbitHoleWithEx : RabbitHole.VisitRabbitHoleWithBase<VisitRabbitHoleWithEx>, Common.IPreLoad, Common.IAddInteraction
    {
        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.AddCustom(new CustomInjector());
        }

        public void OnPreLoad()
        {
            Tunings.Inject<RabbitHole, RabbitHole.VisitRabbitHoleWith.Definition, Definition>(false);
        }

        public override string GetInteractionName()
        {
            Definition definition = InteractionDefinition as Definition;
            if (definition != null)
            {
                return definition.GetInteractionName(Actor, Target, InteractionObjectPair);
            }
            else
            {
                return base.GetInteractionName();
            }
        }

        public override InteractionInstance CreateVisitInteractionForSim(Sim sim, InteractionDefinition defToPush, List<Sim> alreadyAdded, ref Dictionary<Sim, bool> simArrivalStatus)
        {
            VisitRabbitHoleEx hole = defToPush.CreateInstance(Target, sim, mPriority, false, true) as VisitRabbitHoleEx;
            if (hole != null)
            {
                hole.TourGroup.AddRange(alreadyAdded);
            }
            return hole;
        }

        public override InteractionDefinition GetInteractionDefinition(string interactionName, RabbitHole.VisitRabbitHoleTuningClass visitTuning, Origin visitBuffOrigin)
        {
            VisitRabbitHoleEx.Definition definition = new VisitRabbitHoleEx.Definition(interactionName, visitTuning, visitBuffOrigin);
            definition.IsGroupAddition = true;
            return definition;
        }

        public class Definition : RabbitHole.VisitRabbitHoleWithBase<VisitRabbitHoleWithEx>.BaseDefinition
        {
            public Definition()
            { }
            public Definition(VisitRabbitHoleEx.InteractionParameters parameters)
                : base(parameters.mPrefix + "VisitWithInteractionName", parameters.mPrefix + parameters.mVisitName, parameters.mTuning, parameters.mOrigin)
            { }

            public override string GetInteractionName(Sim actor, RabbitHole target, InteractionObjectPair iop)
            {
                if (InteractionName.StartsWith("VisitWithInteraction"))
                {
                    return Common.LocalizeEAString(actor.IsFemale, "Gameplay/Core/VisitCommunityLot:VisitNamedLotWith", new object[] { target.CatalogName });
                }
                else
                {
                    return base.GetInteractionName(actor, target, iop);
                }
            }

            public override bool Test(Sim a, RabbitHole target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (!base.Test(a, target, isAutonomous, ref greyedOutTooltipCallback)) return false;

                if ((a.Posture != null) && (a.Posture.Container == target))
                {
                    return false;
                }

                return true;
            }
        }

        public class CustomInjector : Common.InteractionInjector<RabbitHole>
        {
            public CustomInjector()
            { }

            protected override bool Perform(GameObject obj, InteractionDefinition definition, Dictionary<Type, bool> existing)
            {
                RabbitHole hole = obj as RabbitHole;
				if (hole == null || hole is EiffelTower) return false;

                VisitRabbitHoleEx.InteractionParameters parameters;
                if (VisitRabbitHoleEx.Parameters.TryGetValue(hole.Guid, out parameters))
                {
                    if (base.Perform(obj, new Definition(parameters), existing))
                    {
                        Type type = typeof(RabbitHole.VisitRabbitHoleWith.Definition);

                        Common.RemoveInteraction(obj, type);
                        existing.Remove(type);

                        return true;
                    }
                }

                return false;
            }
        }
    }
}
