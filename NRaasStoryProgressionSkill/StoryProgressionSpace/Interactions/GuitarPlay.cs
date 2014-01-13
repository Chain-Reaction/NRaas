using NRaas.CommonSpace.Helpers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Interactions
{
    public class GuitarPlay : Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<Guitar, Guitar.Play.Definition, Definition>(false);

            sOldSingleton = Guitar.Play.Singleton;
            Guitar.Play.Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Guitar, Guitar.Play.Definition>(Guitar.Play.Singleton);
        }

        protected class Definition : Guitar.Play.Definition
        {
            public override string GetInteractionName(Sim actor, MusicalInstrument target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override bool Test(Sim a, MusicalInstrument target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (!base.Test(a, target, isAutonomous, ref greyedOutTooltipCallback)) return false;

                if (isAutonomous)
                {
                    if (!NRaas.StoryProgression.Main.GetValue<Option, bool>())
                    {
                        if (Party.IsInvolvedInAnyTypeOfParty(a))
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
        }

        public class Option : BooleanManagerOptionItem<ManagerSkill>
        {
            public Option()
                : base(false)
            { }

            public override string GetTitlePrefix()
            {
                return "AllowPartyInstrument";
            }

            public override bool Progressed
            {
                get { return false; }
            }
        }
    }
}
