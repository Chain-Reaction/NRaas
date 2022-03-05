using NRaas.CareerSpace.Skills;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.UI.Controller;
using System;
using System.Collections.Generic;

namespace NRaas.CareerSpace.Interactions
{
    public class GreetKill : SocialInteractionA, Common.IAddInteraction
    {
        static InteractionDefinition Singleton = new Definition();

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<Sim>(Singleton);
        }

        public override bool Run()
        {
            try
            {
                Target.InteractionQueue.CancelAllInteractions();

                return base.Run();
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
                return false;
            }
        }

        public new class Definition : SocialInteractionA.Definition
        {
            public Definition()
            { }
            public Definition(string key)
                : base(key, new string[0], null, false)
            { }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new GreetKill();
                na.Init(ref parameters);
                return na;
            }

            public override void AddInteractions(InteractionObjectPair iop, Sim actor, Sim target, List<InteractionObjectPair> results)
            {
                bool ungreeted = false;

                if (Skills.Assassination.StaticGuid != SkillNames.None)
                {
                    Relationship relationship = Relationship.Get(target, actor, false);
                    if ((relationship == null) || (relationship.LTR.CurrentLTR == LongTermRelationshipTypes.Stranger))
                    {
                        ungreeted = true;
                    }
                    else if (target.NeedsToBeGreeted(actor))
                    {
                        ungreeted = true;
                    }
                }

                if (ungreeted)
                {
                    foreach (SimDescription.DeathType type in Assassination.Types.Keys)
                    {
                        if (ActionData.Get("NRaas Assassin " + type) != null)
                        {
                            results.Add(new InteractionObjectPair(new Definition("NRaas Assassin " + type), target));
                        }
                    }
                }
            }
        }
    }
}
