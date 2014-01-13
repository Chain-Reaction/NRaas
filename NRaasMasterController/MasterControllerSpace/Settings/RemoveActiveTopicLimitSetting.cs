using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Settings
{
    public class RemoveActiveTopicLimitSetting : BooleanSettingOption<GameObject>, ISettingOption
    {
        static int sOriginalValue = 0;

        protected override bool Value
        {
            get
            {
                return NRaas.MasterController.Settings.mRemoveActiveTopicLimit;
            }
            set
            {
                NRaas.MasterController.Settings.mRemoveActiveTopicLimit = value;

                Perform(value);
            }
        }

        public static void Perform(bool value)
        {
            if (value)
            {
                sOriginalValue = STCData.sNumSocialsDuringConversation;
                STCData.SetNumSocialsDuringConversation(int.MaxValue);
            }
            else if (sOriginalValue != 0)
            {
                STCData.SetNumSocialsDuringConversation(sOriginalValue);
            }
        }

        public override string GetTitlePrefix()
        {
            return "RemoveActiveTopicLimitSetting";
        }

        public override ITitlePrefixOption ParentListingOption
        {
            get { return new SimListingOption(); }
        }
    }
}
