using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
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

namespace NRaas.RegisterSpace.Options.GlobalRoles
{
    public class RemoveBySim : OperationSettingOption<GameObject>, IGlobalRolesOption
    {
        public override string GetTitlePrefix()
        {
            return "RemoveBySim";
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (RoleManagerTaskEx.IsLoading) return false;

            return base.Allow(parameters);
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            Sim actorSim = parameters.mActor as Sim;

            SimSelection sims = SimSelection.Create(Name, actorSim.SimDescription);
            if (sims.IsEmpty)
            {
                Common.Notify(Common.Localize(GetTitlePrefix() + ":None"));
                return OptionResult.Failure;
            }

            SimSelection.Results selection = sims.SelectMultiple();
            if ((selection == null) || (selection.Count == 0)) return OptionResult.Failure;

            foreach (SimDescription sim in selection)
            {
                sim.AssignedRole.RemoveSimFromRole();
            }

            return OptionResult.SuccessRetain;
        }

        protected class SimSelection : ProtoSimSelection<SimDescription>
        {
            private SimSelection(string title, SimDescription me)
                : base(title, me, true, true)
            {
                AddColumn(new TypeColumn());
            }

            public static SimSelection Create(string title, SimDescription me)
            {
                SimSelection selection = new SimSelection(title, me);
                bool canceled;
                selection.FilterSims(new List<ICriteria>(), null, true, out canceled);
                return selection;
            }

            protected override bool Allow(SimDescription sim)
            {
                return (sim.AssignedRole != null);
            }

            protected class TypeColumn : ObjectPickerDialogEx.CommonHeaderInfo<SimDescription>
            {
                public TypeColumn()
                    : base("NRaas.Register.RoleType:ColumnHeader", "NRaas.Register.RoleType:ColumnTooltip", 30)
                { }

                public override ObjectPicker.ColumnInfo GetValue(SimDescription item)
                {
                    return new ObjectPicker.TextColumn(Roles.GetLocalizedName(item.AssignedRole));
                }
            }
        }
    }
}