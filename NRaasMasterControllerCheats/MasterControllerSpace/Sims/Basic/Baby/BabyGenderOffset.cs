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
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Basic.Baby
{
    public class BabyGenderOffset : SimFromList, IBabyOption
    {
        private float mOffset;

        public override string GetTitlePrefix()
        {
            return "BabyGenderOffset";
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

            return (me.Pregnancy != null);
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            if (!ApplyAll)
            {
                string text = StringInputDialog.Show(Name, Common.Localize("BabyGenderOffset:Prompt", me.IsFemale, new object[] { me, Pregnancy.kMaxGenderOffset }), me.Pregnancy.mBabySexOffset.ToString());
                if ((text == null) || (text == "")) return false;

                mOffset = 0;
                if (!float.TryParse(text, out mOffset))
                {
                    SimpleMessageDialog.Show(Name, Common.Localize("Numeric:Error"));
                    return false;
                }

                mOffset = Math.Min(Pregnancy.kMaxGenderOffset, Math.Max(-Pregnancy.kMaxGenderOffset, mOffset));
            }

            me.Pregnancy.mBabySexOffset = mOffset;
            return true;
        }
    }
}
