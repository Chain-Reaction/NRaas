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
    public class CelebrityPoints : SimFromList, IIntermediateOption
    {
        uint mPoints = 0;

        public override string GetTitlePrefix()
        {
            return "CelebrityPoints";
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

            if (me.CelebrityManager == null) return false;

            if (!me.CanBeCelebrity) return false;

            return true;
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            if (!ApplyAll)
            {
                int remainingPoints = (int)(me.CelebrityManager.GetGoalPointsForCurrentLevel() - me.CelebrityManager.Points);

                string text = StringInputDialog.Show(Name, Common.Localize("CelebrityPoints:Prompt", me.IsFemale, new object[] { me, remainingPoints }), me.CelebrityManager.Points.ToString(), 256, StringInputDialog.Validation.None);
                if (string.IsNullOrEmpty(text)) return false;

                mPoints = 0;
                if (!uint.TryParse(text, out mPoints))
                {
                    SimpleMessageDialog.Show(Name, Common.Localize("Numeric:Error"));
                    return false;
                }
            }

            CelebrityLevel.CleanupFreeStuffAlarm(me);

            me.CelebrityManager.AddPointsInternal(mPoints);
            return true;
        }
    }
}
