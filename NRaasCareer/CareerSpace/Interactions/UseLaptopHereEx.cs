using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;

namespace NRaas.CareerSpace.Interactions
{
    public class UseLaptopHereEx : Sim.UseLaptopHere, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<GameObject, Sim.UseLaptopHere.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<GameObject, Sim.UseLaptopHere.Definition>(Singleton);
        }

        public override void ConfigureInteraction()
        {
            try
            {
                if (base.Actor.Inventory.Find<IComputer>() != null)
                {
                    List<Tone> newTones = new List<Tone>();
                    newTones.Add(new PlayGamesTone());

                    if (Actor.OccupationAsAcademicCareer != null)
                    {
                        newTones.Add(new StudyTone());
                    }

                    SetAvailableTones(newTones);
                }
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

        public new class Definition : Sim.UseLaptopHere.Definition
        {
            public Definition()
            { }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new UseLaptopHereEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim a, GameObject target, InteractionObjectPair interaction)
            {
                return base.GetInteractionName(a, target, new InteractionObjectPair(sOldSingleton, target));
            }
        }
    }
}
