using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Advanced
{
    public class ChangePlumbotQuality : SimFromList, IAdvancedOption
    {
        new int mCount = 0;

        public override string GetTitlePrefix()
        {
            return "ChangePlumbotQuality";
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }        

        protected override bool PrivateAllow(SimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            if (me.CreatedSim == null) return false;

            return (me.TraitManager != null && me.TraitChipManager != null && me.IsEP11Bot);
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            Sim sim = me.CreatedSim;
            if (sim == null) return false;

            int value;
            CASRobotData supernaturalData = me.SupernaturalData as CASRobotData;
            if (supernaturalData != null)
            {
                value = supernaturalData.BotQualityLevel;
            }
            else
            {
                return false;
            }

            if (!ApplyAll)
            {
                string text = StringInputDialog.Show(Name, Common.Localize(GetTitlePrefix() + ":Prompt", me.IsFemale, new object[] { me }), value.ToString(), 256, StringInputDialog.Validation.None);
                if (string.IsNullOrEmpty(text)) return false;

                mCount = 0;
                if (!int.TryParse(text, out mCount) || (mCount < 1 || mCount > 10))
                {
                    SimpleMessageDialog.Show(Name, Common.Localize("Numeric:Error"));
                    return false;
                }
            }

            supernaturalData.BotQualityLevel = mCount;

            return true;
        }
    }
}