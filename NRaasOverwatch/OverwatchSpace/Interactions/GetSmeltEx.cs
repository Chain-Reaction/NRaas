using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Interactions;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Spawners;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.OverwatchSpace.Interactions
{
    public class GetSmeltEx : Metal.GetSmelt, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<Metal, Metal.GetSmelt.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Metal, Metal.GetSmelt.Definition>(Singleton);
        }

        public override void MailedSuccesfully(Mailbox box, IGameObject obj)
        {
            try
            {
                Metal metal = obj as Metal;
                Collecting collecting = Actor.SkillManager.AddElement(SkillNames.Collecting) as Collecting;
                if (metal != null)
                {
                    // Custom
                    if (!collecting.mMetalData.ContainsKey(metal.Guid))
                    {
                        collecting.mMetalData.Add(metal.Guid, new Collecting.MetalStats(0));
                    }
                }

                base.MailedSuccesfully(box, obj);
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
            }
        }

        public new class Definition : Metal.GetSmelt.Definition
        {
            public Definition()
            { }
            public Definition(List<IGameObject> list)
                : base(list)
            { }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new GetSmeltEx();
                na.Init(ref parameters);
                return na;
            }

            public override void AddInteractions(InteractionObjectPair iop, Sim actor, Metal target, List<InteractionObjectPair> results)
            {
                results.Add(new InteractionObjectPair(new Definition(), target));
                uint stackNumber = actor.Inventory.GetStackNumber(target);
                if (stackNumber != 0x0)
                {
                    List<IGameObject> stackObjects = actor.Inventory.GetStackObjects(stackNumber, true);
                    results.Add(new InteractionObjectPair(new Definition(stackObjects), target));
                }
            }

            public override string GetInteractionName(Sim actor, Metal target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }
        }
    }
}
