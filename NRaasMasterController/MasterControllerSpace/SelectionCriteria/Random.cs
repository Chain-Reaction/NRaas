using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.MapTags;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.SelectionCriteria
{
    public class Random : SelectionOption
    {
        int mValue;

        public override string GetTitlePrefix()
        {
            return "Criteria.Random";
        }

        public override void Reset()
        {
            mValue = 0;

            base.Reset();
        }

        public override SimSelection.UpdateResult Update(IMiniSimDescription actor, IEnumerable<SimSelection.ICriteria> criteria, List<IMiniSimDescription> allSims, bool secondStage)
        {
            if (!secondStage)
            {
                string text = StringInputDialog.Show(Name, Common.Localize(GetTitlePrefix() + ":Prompt", actor.IsFemale), "");
                if (string.IsNullOrEmpty(text)) return SimSelection.UpdateResult.Failure;

                if (!int.TryParse(text, out mValue))
                {
                    Common.Notify(Common.Localize("Numeric:Error"));
                    return SimSelection.UpdateResult.Failure;
                }

                return SimSelection.UpdateResult.Delay;
            }
            else
            {
                RandomUtil.RandomizeListOfObjects(allSims);

                allSims.RemoveRange(mValue, allSims.Count - mValue);

                return SimSelection.UpdateResult.Success;
            }
        }

        protected override bool Allow(SimDescription me, IMiniSimDescription actor)
        {
            return true;
        }
        protected override bool Allow(MiniSimDescription me, IMiniSimDescription actor)
        {
            return true;
        }
    }
}
