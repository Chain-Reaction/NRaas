using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Advanced.Pregnancy
{
    public class ResumePregnancy : SimFromList, IPregnancyOption
    {
        public override string GetTitlePrefix()
        {
            return "ResumePregnancy";
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

            if (!me.IsPregnant) return false;

            if (me.Pregnancy.PreggersAlarm != AlarmHandle.kInvalidHandle) return false;

            return true;
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            if (!ApplyAll)
            {
                if (!AcceptCancelDialog.Show(Common.Localize(GetTitlePrefix () + ":Prompt", me.IsFemale, new object[] { me })))
                {
                    return false;
                }
            }

            if (me.IsPregnant)
            {
                me.CreatedSim.RemoveAlarm(me.Pregnancy.PreggersAlarm);
                me.Pregnancy.PreggersAlarm = AlarmHandle.kInvalidHandle;

                if (me.IsHuman)
                {
                    me.Pregnancy.PreggersAlarm = me.CreatedSim.AddAlarmRepeating(1f, TimeUnit.Hours, me.Pregnancy.HourlyCallback, 1f, TimeUnit.Hours, "Hourly Human Pregnancy Update Alarm", AlarmType.AlwaysPersisted);

                }
                else
                {
                    me.Pregnancy.PreggersAlarm = me.CreatedSim.AddAlarmRepeating(1f, TimeUnit.Hours, me.Pregnancy.HourlyCallback, 1f, TimeUnit.Hours, "Hourly Pet Pregnancy Update Alarm", AlarmType.AlwaysPersisted);
                }
            }
            
            return true;
        }
    }
}
