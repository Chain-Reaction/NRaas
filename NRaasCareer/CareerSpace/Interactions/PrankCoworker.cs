using NRaas.CommonSpace.Helpers;
using NRaas.Gameplay.Careers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.CareerSpace.Interactions
{
    public class PrankCoworker : BusinessAndJournalismRabbitHole.PrankCoworker, Common.IPreLoad, Common.IAddInteraction
    {
        // Fields
        public new static InteractionDefinition Singleton = new Definition();

        public void OnPreLoad()
        {
            Tunings.Inject<RabbitHole, BusinessAndJournalismRabbitHole.PrankCoworker.Definition, Definition>(false);
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<BusinessAndJournalismRabbitHole>(Singleton);
        }

        public override string GetInteractionName()
        {
            Definition interactionDefinition = InteractionDefinition as Definition;
            return LocalizeString(Actor.SimDescription, "PrankParticularCoworker", new object[] { interactionDefinition.SimAffectedByPrank });
        }

        public override bool InRabbitHole()
        {
            try
            {
                StartStages();
                BeginCommodityUpdates();

                Business job = OmniCareer.Career<Business>(base.Actor.Occupation);

                bool succeeded = false;

                Definition interactionDefinition = base.InteractionDefinition as Definition;

                try
                {
                    string randomStringFromList = RandomUtil.GetRandomStringFromList(BusinessAndJournalismRabbitHole.PrankCoworker.sKeyNames);
                    base.Actor.ShowTNSIfSelectable(Common.LocalizeEAString(Actor.IsFemale, BusinessAndJournalismRabbitHole.PrankCoworker.sKeyNameSpace + randomStringFromList, new object[] { base.Actor, interactionDefinition.SimAffectedByPrank }), StyledNotification.NotificationStyle.kGameMessagePositive);

                    succeeded = DoLoop(~(ExitReason.Replan | ExitReason.MidRoutePushRequested | ExitReason.ObjectStateChanged | ExitReason.PlayIdle | ExitReason.MaxSkillPointsReached));
                }
                finally
                {
                    EndCommodityUpdates(succeeded);
                }

                if ((succeeded && (job != null)) && (interactionDefinition != null))
                {
                    job.SimAffectedByPrank = interactionDefinition.SimAffectedByPrank;
                    job.SimConspiringPrank = interactionDefinition.SimConspiringPrank;
                    job.IsPrankSet = true;
                }
                return succeeded;
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
                return false;
            }
        }

        protected static string LocalizeString(SimDescription sim, string name, params object[] parameters)
        {
            return OmniCareer.LocalizeString(sim, name, "Gameplay/Objects/RabbitHoles/BusinessAndJournalismRabbitHole/PrankCoworker:" + name, parameters);
        }

        // Nested Types
        [DoesntRequireTuning]
        public new class Definition : InteractionDefinition<Sim, RabbitHole, PrankCoworker>
        {
            // Fields
            public string[] MenuPath;
            public string MenuText;
            public SimDescription SimAffectedByPrank;
            public SimDescription SimConspiringPrank;

            // Methods
            public Definition()
            {
            }

            public Definition(SimDescription pranker, SimDescription prankee, string menuText, string[] menuPath)
            {
                SimConspiringPrank = pranker;
                SimAffectedByPrank = prankee;
                MenuText = menuText;
                MenuPath = menuPath;
            }

            public override void AddInteractions(InteractionObjectPair iop, Sim actor, RabbitHole target, List<InteractionObjectPair> results)
            {
                if ((actor.Occupation != null) && (actor.Occupation.Coworkers != null))
                {
                    string[] menuPath = new string[] { PrankCoworker.LocalizeString(actor.SimDescription, "PrankCoworker", new object[0x0]) };
                    foreach (SimDescription description in actor.Occupation.Coworkers)
                    {
                        if (description != null)
                        {
                            Definition interaction = new Definition(actor.SimDescription, description, description.FullName, menuPath);
                            results.Add(new InteractionObjectPair(interaction, target));
                        }
                    }
                    SimDescription boss = actor.Occupation.Boss;
                    if (boss != null)
                    {
                        Definition definition2 = new Definition(actor.SimDescription, boss, boss.FullName, menuPath);
                        results.Add(new InteractionObjectPair(definition2, target));
                    }
                }
            }

            public override string GetInteractionName(Sim a, RabbitHole target, InteractionObjectPair interaction)
            {
                return MenuText;
            }

            public override string[] GetPath(bool isFemale)
            {
                return MenuPath;
            }

            public override bool Test(Sim a, RabbitHole target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                OmniCareer omni = a.Occupation as OmniCareer;
                if (omni != null)
                {
                    if (!omni.CanPrank()) return false;
                }

                Business business = OmniCareer.Career<Business>(a.Occupation);
                if (!isAutonomous && (business != null))
                {
                    if ((business != null) && !business.IsRegularWorkTime())
                    {
                        return !business.IsPrankSet;
                    }
                }
                return false;
            }
        }
    }
}
