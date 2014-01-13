using NRaas.CommonSpace.Options;
using NRaas.WoohooerSpace.Interactions;
using NRaas.WoohooerSpace.Skills;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Moving;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.WoohooerSpace.Options
{
    public class ActivateRendezvous : OperationSettingOption<Sim>, ISimOption
    {
        bool mEnabled;

        protected override bool Allow(GameHitParameters<Sim> parameters)
        {
            if (!Skills.KamaSimtra.Settings.mShowRendezvousInteraction) return false;

            if (Woohooer.Settings.mAllowTeenWoohoo)
            {
                if (parameters.mTarget.SimDescription.ChildOrBelow) return false;
            }
            else
            {
                if (parameters.mTarget.SimDescription.TeenOrBelow) return false;
            }

            Skills.KamaSimtra skill = parameters.mTarget.SkillManager.GetSkill<Skills.KamaSimtra>(Skills.KamaSimtra.StaticGuid);
            if (skill == null) return false;

            if (skill.SkillLevel <= 0) return false;

            mEnabled = skill.RendezvousActive;

            return base.Allow(parameters);
        }

        public override string GetTitlePrefix()
        {
            if (mEnabled)
            {
                return "DisableRendezvous";
            }
            else
            {
                return "EnableRendezvous";
            }
        }

        protected override OptionResult Run(GameHitParameters<Sim> parameters)
        {
            Skills.KamaSimtra skill = parameters.mTarget.SkillManager.GetSkill<Skills.KamaSimtra>(Skills.KamaSimtra.StaticGuid);
            if (skill != null)
            {
                skill.RendezvousActive = !skill.RendezvousActive;
            }

            return OptionResult.SuccessClose;
        }
    }
}
