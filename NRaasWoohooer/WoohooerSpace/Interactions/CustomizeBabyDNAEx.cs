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
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.PetSystems;
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
    public class CustomizeBabyDNAEx : Hospital.CustomizeBabyDNA, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Hospital, Hospital.CustomizeBabyDNA.Definition>(Singleton);
        }

        public void OnPreLoad()
        {
            InteractionTuning tuning = Tunings.GetTuning<Hospital, Hospital.CustomizeBabyDNA.Definition>();
            if (tuning != null)
            {
                tuning.Availability.Teens = true;
            }

            Tunings.Inject<Hospital, Hospital.CustomizeBabyDNA.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }      

        public new class Definition : Hospital.CustomizeBabyDNA.Definition
        {
            public override string GetInteractionName(Sim actor, Hospital target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, iop);
            }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance result = new CustomizeBabyDNAEx();
                result.Init(ref parameters);
                return result;
            }

            public override bool Test(Sim a, Hospital target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {                
                return true;
            }
        }
    }
}
