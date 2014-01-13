using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.MasterControllerSpace.Helpers;
using NRaas.MasterControllerSpace.Sims.Advanced;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Settings.SkillStamps
{
    public class RemoveStampSetting : OptionItem, ISkillStampOption
    {
        public override string GetTitlePrefix()
        {
            return "RemoveSkillStamp";
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (MasterController.Settings.SkillStamps.Count == 0) return false;

            return base.Allow(parameters);
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            List<ApplySkillStamp.Item> allOptions = new List<ApplySkillStamp.Item>();
            foreach(SkillStamp stamp in MasterController.Settings.SkillStamps)
            {
                allOptions.Add(new ApplySkillStamp.Item(stamp));
            }

            CommonSelection<ApplySkillStamp.Item>.Results choices = new CommonSelection<ApplySkillStamp.Item>(Name, allOptions).SelectMultiple();
            if ((choices == null) || (choices.Count == 0)) return OptionResult.Failure;

            foreach (ApplySkillStamp.Item choice in choices)
            {
                MasterController.Settings.SkillStamps.Remove(choice.mStamp);
            }

            MasterController.Settings.UpdateStamp();

            return OptionResult.SuccessRetain;
        }
    }
}
