using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Objects.Electronics;
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
    public class HackEx : Computer.Hack, Common.IPreLoad
    {
        public new static InteractionDefinition Singleton = new Definition();

        public void OnPreLoad()
        {
            Tunings.Inject<Computer, Computer.Hack.Definition, Definition>(false);
        }

        public override void ConfigureInteraction()
        {
            TimeOfDayStage stage = new TimeOfDayStage(GetInteractionName(), Target.ComputerTuning.HackEndHour, Target.ComputerTuning.HackTotalHoursAvailable);
            Stages = new List<Stage>(new Stage[] { stage });
        }

        protected new class Definition : Computer.Hack.Definition
        {
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new HackEx();
                na.Init(ref parameters);
                return na;
            }
        }
    }
}
