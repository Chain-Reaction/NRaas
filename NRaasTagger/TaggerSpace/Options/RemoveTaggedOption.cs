using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Register;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.TaggerSpace.Options
{
    public class RemoveTaggedOption : OperationSettingOption<GameObject>, IPrimaryOption<GameObject>    
    {
        public override string GetTitlePrefix()
        {
            return "RemoveTaggedSims";
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            Sim actorSim = parameters.mActor as Sim;

            SimSelection sims = SimSelection.Create("", Name, actorSim.SimDescription);
            if (sims.IsEmpty)
            {                
                return OptionResult.Failure;
            }

            SimSelection.Results results = sims.SelectMultiple();
            foreach (SimDescription desc in results)
            {
                Tagger.Settings.mTaggedSims.Remove(desc.SimDescriptionId);
            }

            return OptionResult.SuccessClose;
        }

        protected class SimSelection : ProtoSimSelection<SimDescription>
        {
            private SimSelection(string title, string subTitle, SimDescription me)
                : base(title, subTitle, me, true, true)
            {               
            }

            public static SimSelection Create(string title, string subTitle, SimDescription me)
            {
                SimSelection selection = new SimSelection(title, subTitle, me);

                bool canceled = false;
                selection.FilterSims(null, null, false, out canceled);
                
                return selection;
            }

            protected override bool Allow(SimDescription sim)
            {
                return Tagger.Settings.mTaggedSims.Contains(sim.SimDescriptionId);
            }            
        }
    }
}