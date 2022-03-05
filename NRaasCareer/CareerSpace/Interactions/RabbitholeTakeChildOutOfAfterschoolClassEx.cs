using NRaas.CareerSpace.Booters;
using NRaas.CareerSpace.Helpers;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.RabbitHoles;
using System.Collections.Generic;

namespace NRaas.CareerSpace.Interactions
{
    public class RabbitholeTakeChildOutOfAfterschoolClassEx : SchoolRabbitHole.TakeChildOutOfAfterschoolClass, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<RabbitHole, SchoolRabbitHole.TakeChildOutOfAfterschoolClass.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<SchoolRabbitHole>(Singleton);
        }

        public new class Definition : SchoolRabbitHole.TakeChildOutOfAfterschoolClass.Definition
        {
            public Definition()
            { }
            public Definition(AfterschoolActivityType type)
                : base(type)
            { }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new RabbitholeTakeChildOutOfAfterschoolClassEx();
                na.Init(ref parameters);
                return na;
            }

            public override void AddInteractions(InteractionObjectPair iop, Sim actor, SchoolRabbitHole target, List<InteractionObjectPair> results)
            {
                foreach (AfterschoolActivityData data in AfterschoolActivityBooter.Activities.Values)
                {
                    results.Add(new InteractionObjectPair(new Definition(data.mActivity.CurrentActivityType), iop.Target));
                }
            }

            public override string GetInteractionName(Sim a, SchoolRabbitHole target, InteractionObjectPair interaction)
            {
                return base.GetInteractionName(a, target, new InteractionObjectPair(sOldSingleton, target));
            }
        }
    }
}
