using NRaas.CommonSpace.Helpers;
using NRaas.RegisterSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.RegisterSpace.Interactions
{
    public class GoToLotThatSatisfiesMyRoleEx : Sim.GoToLotThatSatisfiesMyRole, Common.IPreLoad
    {
        public new static InteractionDefinition Singleton = new Definition();
   
        public void OnPreLoad()
        {
            Tunings.Inject<Sim, Sim.GoToLotThatSatisfiesMyRole.Definition, Definition>(false);
        }

        public new class Definition : Sim.GoToLotThatSatisfiesMyRole.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new GoToLotThatSatisfiesMyRoleEx();
                na.Init(ref parameters);
                return na;
            }

            public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (a.LotCurrent == null) return true;

                if (a.LotCurrent.IsBaseCampLotType) return true;

                if (a.LotCurrent.CommercialLotSubType == CommercialLotSubType.kEP10_Resort) return true;

                return base.Test(a, target, isAutonomous, ref greyedOutTooltipCallback);
            }
        }
    }
}
