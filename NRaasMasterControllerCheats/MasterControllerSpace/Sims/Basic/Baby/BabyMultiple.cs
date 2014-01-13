using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Basic.Baby
{
    public class BabyMultiple : SimFromList, IBabyOption
    {
        private float mMultiple;

        public override string GetTitlePrefix()
        {
            return "BabyMultiple";
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
            if (Common.AssemblyCheck.IsInstalled("NRaasWoohooer"))
            {
                return true;
            }
            else
            {
                return me.IsHuman;
            }
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            return (me.Pregnancy != null);
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            if (!ApplyAll)
            {
                string text = StringInputDialog.Show(Name, Common.Localize("BabyMultiple:Prompt", me.IsFemale, new object[] { me, Pregnancy.kMaxBabyMultiplier }), me.Pregnancy.mMultipleBabiesMultiplier.ToString());
                if (string.IsNullOrEmpty(text)) return false;

                mMultiple = 0;
                if (!float.TryParse(text, out mMultiple))
                {
                    SimpleMessageDialog.Show(Name, Common.Localize("Numeric:Error"));
                    return false;
                }

                if (mMultiple < 0)
                {
                    mMultiple = 0;
                }
                else if (mMultiple > Pregnancy.kMaxBabyMultiplier)
                {
                    mMultiple = Pregnancy.kMaxBabyMultiplier;
                }
            }

            me.Pregnancy.mMultipleBabiesMultiplier = mMultiple;
            return true;
        }
    }
}
