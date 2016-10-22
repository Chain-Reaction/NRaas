using NRaas.CommonSpace.Helpers;
using NRaas.HybridSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.HybridSpace.Interactions
{
    public class SpinWithEx : SkatingRink.SpinWith, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<GameObject, SkatingRink.SpinWith.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<GameObject, SkatingRink.SpinWith.Definition>(Singleton);
        }

        public new class Definition : SkatingRink.SpinWith.Definition
        {
            public Definition()
            { }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new SpinWithEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, GameObject target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            private new List<Sim> GetValidSimsToSpinWith(Sim actor, ISkatableObject skateLocation)
            {
                List<Sim> lazyList = null;
                if ((skateLocation.Skaters == null) || (skateLocation.Skaters.Count < 2))
                {
                    return null;
                }
                foreach (KeyValuePair<ulong, SkatingRink.Lane> pair in skateLocation.Skaters)
                {
                    Sim createdSim = SimDescription.GetCreatedSim(pair.Key);
                    if (createdSim != null)
                    {
                        SkateEx currentInteraction = createdSim.CurrentInteraction as SkateEx;
                        if (((currentInteraction != null) && currentInteraction.IsSkating) && !SkateHelper.CalculateIfActorIsOccultSkaterEx(actor))
                        {
                            Lazy.Add<List<Sim>, Sim>(ref lazyList, createdSim);
                        }
                    }
                }
                return lazyList;
            }

            public override void PopulatePieMenuPicker(ref InteractionInstanceParameters parameters, out List<ObjectPicker.TabInfo> listObjs, out List<ObjectPicker.HeaderInfo> headers, out int NumSelectableRows)
            {
                Sim actor = parameters.Actor as Sim;
                NumSelectableRows = 1;
                ISkatableObject skateLocationFromParameters = base.GetSkateLocationFromParameters(parameters);
                base.PopulateSimPicker(ref parameters, out listObjs, out headers, this.GetValidSimsToSpinWith(actor, skateLocationFromParameters), false);
            }

            public override bool Test(Sim a, GameObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return true;
            }
        }
    }
}
