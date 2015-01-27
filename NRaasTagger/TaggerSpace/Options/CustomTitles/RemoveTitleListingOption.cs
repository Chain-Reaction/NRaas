using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.TaggerSpace.Options.CustomTitles
{
    public class RemoveTitleListingOption : InteractionOptionList<RemoveOption, GameObject>, ICustomTitleOption
    {
        Sim mTarget;        

        public override string GetTitlePrefix()
        {
            return "RemoveTitleRoot";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return null; }
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (!Tagger.Settings.SimHasCustomTitles(parameters.mActor.SimDescription.SimDescriptionId)) return false;            

            mTarget = parameters.mActor as Sim;

            return base.Allow(parameters);
        }        

        public override List<RemoveOption> GetOptions()
        {            
            List<RemoveOption> results = new List<RemoveOption>();
            List<string> titles;

            if (mTarget != null)
            {

                Tagger.Settings.mCustomSimTitles.TryGetValue(mTarget.SimDescription.SimDescriptionId, out titles);

                foreach (string title in titles)
                {
                    results.Add(new RemoveOption(mTarget, title));
                }
            }

            return results;
        }
    }
}