using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Intermediate
{
    public class GenderPreference : SimFromList, IIntermediateOption
    {
        int mPreferenceMale = 0;
        int mPreferenceFemale = 0;

        public override string GetTitlePrefix()
        {
            return "GenderPreference";
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            if (!ApplyAll)
            {
                string text = StringInputDialog.Show(Name, Common.Localize("GenderPreference:MalePrompt", me.IsFemale, new object[] { me }), me.mGenderPreferenceMale.ToString(), 256, StringInputDialog.Validation.None);
                if ((text == null) || (text == "")) return false;

                mPreferenceMale = 0;
                if (!int.TryParse(text, out mPreferenceMale))
                {
                    SimpleMessageDialog.Show(Name, Common.Localize("Numeric:Error"));
                    return false;
                }

                text = StringInputDialog.Show(Name, Common.Localize("GenderPreference:FemalePrompt", me.IsFemale, new object[] { me }), me.mGenderPreferenceFemale.ToString(), 256, StringInputDialog.Validation.None);
                if ((text == null) || (text == "")) return false;

                mPreferenceFemale = 0;
                if (!int.TryParse(text, out mPreferenceFemale))
                {
                    SimpleMessageDialog.Show(Name, Common.Localize("Numeric:Error"));
                    return false;
                }
            }

            me.mGenderPreferenceMale = mPreferenceMale;
            me.mGenderPreferenceFemale = mPreferenceFemale;
            return true;
        }
    }
}
