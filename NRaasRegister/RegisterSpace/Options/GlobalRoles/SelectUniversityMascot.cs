using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Options;
using NRaas.RegisterSpace.Tasks;
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
using Sims3.Gameplay.Objects.Register;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.RegisterSpace.Options.GlobalRoles
{
    public class SelectUniversityMascot : ProtoSelect<GameObject>, IGlobalRolesOption
    {
        public override string GetTitlePrefix()
        {
            return "SelectUniversityMascot";
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (RoleManagerTaskEx.IsLoading) return false;

            return base.Allow(parameters);
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            SimDescription sim = PrivateRun(parameters.mActor, Role.RoleType.UniversityMascot, parameters.mHit);
            if (sim == null) return OptionResult.Failure;

            if (!Register.DropRole(sim, null)) return OptionResult.Failure;

            if (Register.AssignRole(sim, Role.RoleType.UniversityMascot, null))
            {
                return OptionResult.SuccessClose;
            }
            else
            {
                return OptionResult.Failure;
            }
        }
    }
}