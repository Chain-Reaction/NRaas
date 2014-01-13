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
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.RegisterSpace.Options.RoleGiver
{
    public class Remove : OperationSettingOption<IGameObject>, IRoleGiverOption
    {
        IRoleGiver mTarget;

        public override string GetTitlePrefix()
        {
            return "Remove";
        }

        public override string Name
        {
            get
            {
                if ((mTarget != null) && (mTarget.CurrentRole != null))
                {
                    SimDescription sim = mTarget.CurrentRole.mSim;
                    if (sim != null)
                    {
                        return Register.Localize("Remove:MenuName", sim.IsFemale, new object[] { sim });
                    }
                }

                return "Debug: Remove";
            }
        }

        protected override bool Allow(GameHitParameters<IGameObject> parameters)
        {
            mTarget = Select.GetGiver(parameters.mTarget);
            if (mTarget == null) return false;

            if (RoleManagerTaskEx.IsLoading) return false;

            if (mTarget.CurrentRole == null) return false;

            if (!Common.kDebugging)
            {
                SimDescription sim = mTarget.CurrentRole.mSim;
                if (sim == null) return false;
            }

            return base.Allow(parameters);
        }

        protected override OptionResult Run(GameHitParameters<IGameObject> parameters)
        {
            IRoleGiver giver = Select.GetGiver(parameters.mTarget);
            if (giver == null) return OptionResult.Failure;

            if (giver.CurrentRole == null) return OptionResult.Failure;

            Sim sim = giver.CurrentRole.SimInRole;

            giver.CurrentRole.RemoveSimFromRole();

            if (sim != null)
            {
                SimpleMessageDialog.Show(Name, Common.Localize("Remove:Success", sim.IsFemale, new object[] { sim }));
            }
            return OptionResult.SuccessRetain;
        }
    }
}