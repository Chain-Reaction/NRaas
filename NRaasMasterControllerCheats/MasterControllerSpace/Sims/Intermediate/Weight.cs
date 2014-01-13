using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Intermediate
{
    public class Weight : SimFromList, IIntermediateOption
    {
        float mWeight = 0;

        bool mApplyToDisplay = true;

        public override string GetTitlePrefix()
        {
            return "Weight";
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        protected override bool AllowSpecies(IMiniSimDescription me)
        {
            return me.IsHuman;
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            if (!ApplyAll)
            {
                string text = StringInputDialog.Show(Name, Common.Localize("Weight:Prompt", me.IsFemale, new object[] { me }), me.Weight.ToString(), 256, StringInputDialog.Validation.None);
                if (string.IsNullOrEmpty(text)) return false;

                mWeight = 0;
                if (!float.TryParse(text, out mWeight))
                {
                    SimpleMessageDialog.Show(Name, Common.Localize("Numeric:Error"));
                    return false;
                }

                mApplyToDisplay = AcceptCancelDialog.Show(Common.Localize(GetTitlePrefix() + ":ApplyToDisplay", me.IsFemale, new object[] { me, mWeight }));
            }

            /*
            if (mWeight < -1)
            {
                mWeight = -1;
            }
            else if (mWeight > 1)
            {
                mWeight = 1;
            }
            */

            me.mWeightShapeDelta = 0f;

            if (mApplyToDisplay)
            {
                me.ForceSetBodyShape(mWeight, me.mCurrentShape.Fit);

                if (me.CreatedSim != null)
                {
                    me.CreatedSim.RequireImmediateBodyShapeUpdate = true;
                    me.UpdateBodyShape(0f, me.CreatedSim.ObjectId);
                }
            }
            else
            {
                me.mInitialShape.Weight = mWeight;
            }
            return true;
        }
    }
}
