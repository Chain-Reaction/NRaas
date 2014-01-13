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
    public class Fitness : SimFromList, IIntermediateOption
    {
        float mFitness = 0;

        bool mApplyToDisplay = true;

        public override string GetTitlePrefix()
        {
            return "Fitness";
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
                string text = StringInputDialog.Show(Name, Common.Localize(GetTitlePrefix() + ":Prompt", me.IsFemale, new object[] { me }), me.Fitness.ToString(), 256, StringInputDialog.Validation.None);
                if (string.IsNullOrEmpty(text)) return false;

                mFitness = 0;
                if (!float.TryParse(text, out mFitness))
                {
                    SimpleMessageDialog.Show(Name, Common.Localize("Numeric:Error"));
                    return false;
                }

                mApplyToDisplay = AcceptCancelDialog.Show(Common.Localize(GetTitlePrefix() + ":ApplyToDisplay", me.IsFemale, new object[] { me, mFitness }));
            }

            /*
            if (mFitness < 0)
            {
                mFitness = 0;
            }
            else if (mFitness > 1)
            {
                mFitness = 1;
            }
            */

            me.mFitnessShapeDelta = 0f;

            if (mApplyToDisplay)
            {
                me.ForceSetBodyShape(me.mCurrentShape.Weight, mFitness);

                if (me.CreatedSim != null)
                {
                    me.CreatedSim.RequireImmediateBodyShapeUpdate = true;
                    me.UpdateBodyShape(0f, me.CreatedSim.ObjectId);
                }
            }
            else
            {
                me.mInitialShape.mFit = mFitness;
            }

            return true;
        }
    }
}
