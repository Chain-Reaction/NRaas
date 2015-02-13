using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.TaggerSpace.Options.CustomTitles
{
    public class RemoveOption : OperationSettingOption<GameObject>
    {
        Sim mTarget;
        string mTitle;

        public RemoveOption(Sim sim, string str)
        {
            mTarget = sim;
            mTitle = str;
        }

        public override string GetTitlePrefix()
        {
            return "";
        }

        public override string Name
        {
            get
            {
                return mTitle;
            }
        }        

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            Sim target = mTarget as Sim;
            if (target != null && Tagger.Settings.SimHasCustomTitles(mTarget.SimDescription.SimDescriptionId))
            {
                Tagger.Settings.mCustomSimTitles[mTarget.SimDescription.SimDescriptionId].Remove(mTitle);

                if (Tagger.Settings.mCustomSimTitles[mTarget.SimDescription.SimDescriptionId].Count == 0)
                {
                    Tagger.Settings.mCustomSimTitles.Remove(mTarget.SimDescription.SimDescriptionId);
                }

                Common.Notify(Common.Localize("General:Success"));
            }

            return OptionResult.SuccessLevelDown;
        }
    }
}