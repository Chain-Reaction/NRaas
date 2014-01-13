using NRaas.CareerSpace.Booters;
using NRaas.CareerSpace.Interfaces;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.CareerSpace.Interactions
{
    public class CallToQuitWorkEx : Common.IPreLoad, Common.IAddInteraction
    {
        public static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<Phone, Phone.CallToQuitWork.Definition, Definition>(false);

            sOldSingleton = Phone.CallToQuitWork.Singleton;
            Phone.CallToQuitWork.Singleton = new Definition ();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Phone, Phone.CallToQuitWork.Definition>(Phone.CallToQuitWork.Singleton);
        }

        public class Definition : Phone.Call.CallDefinition<Phone.CallToQuitWork>
        {
            public Definition()
            { }

            public override string GetInteractionName(Sim actor, Phone target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override bool Test(Sim a, Phone target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                try
                {
                    if (!base.Test(a, target, isAutonomous, ref greyedOutTooltipCallback))
                    {
                        return false;
                    }

                    return (a.Occupation != null) /*&& occupation.CanQuit())*/;
                }
                catch (Exception e)
                {
                    Common.Exception(a, target, e);
                    return false;
                }
            }
        }
    }
}
