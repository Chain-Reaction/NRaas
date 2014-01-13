using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.RegisterSpace.Criteria;
using NRaas.RegisterSpace.Helpers;
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

namespace NRaas.RegisterSpace.Options
{
    public abstract class ProtoSelect<T> : OperationSettingOption<T>
        where T : class, IGameObject
    {
        static string sFailureConditions;

        protected SimDescription PrivateRun(IActor actor, Role.RoleType type, GameObjectHit hit)
        {
            Sim actorSim = actor as Sim;

            sFailureConditions = null;

            SimSelection sims = SimSelection.Create(Register.GetRoleName(type), Name, actorSim.SimDescription, type);
            if (sims.IsEmpty) 
            {
                if (!string.IsNullOrEmpty(sFailureConditions))
                {
                    Common.DebugNotify(sFailureConditions);

                    Common.DebugWriteLog(sFailureConditions);
                }

                SimpleMessageDialog.Show(Name, Register.Localize("Select:Error"));
                return null;
            }

            SimDescription sim = sims.SelectSingle();
            if (sim != null)
            {
                if (sim.CreatedByService != null)
                {
                    sim.CreatedByService.EndService(sim);
                }

                if (sim.AssignedRole != null)
                {
                    sim.AssignedRole.RemoveSimFromRole();
                }

                if (CriteriaItem.HasRealJob(sim))
                {
                    if (AcceptCancelDialog.Show(Common.Localize(GetTitlePrefix() + ":EmployedPrompt", sim.IsFemale, new object[] { sim })))
                    {
                        sim.Occupation.RetireNoConfirmation();
                    }
                }
            }

            return sim;
        }

        protected class SimSelection : ProtoSimSelection<SimDescription>
        {
            RoleData mRole;

            private SimSelection(string title, string subTitle, SimDescription me, Role.RoleType type)
                : base(title, subTitle, me, true, false)
            {
                mRole = RoleData.GetDataForCurrentWorld(type, true);

                AddColumn(new TypeColumn());
            }

            public static SimSelection Create(string title, string subTitle, SimDescription me, Role.RoleType type)
            {
                SimSelection selection = new SimSelection(title, subTitle, me, type);

                List<ICriteria> criteria = new List<ICriteria>();
                foreach (IRoleCriteria crit in Common.DerivativeSearch.Find<IRoleCriteria>())
                {
                    criteria.Add(crit);
                }

                bool canceled;
                selection.FilterSims(criteria, null, false, out canceled);
                return selection;
            }

            protected override bool Allow(SimDescription sim)
            {
                sFailureConditions += Common.NewLine + sim.FullName + ": ";

                if (mRole == null)
                {
                    sFailureConditions += "No Role";
                    return false;
                }

                if (sim.Household == null)
                {
                    sFailureConditions += "No Household";
                    return false;
                }

                if ((SimTypes.IsSkinJob(sim)) && (!sim.IsEP11Bot))
                {
                    sFailureConditions += "Skin Job";
                    return false;
                }

                if (SimTypes.InCarPool(sim))
                {
                    sFailureConditions += "Taxi Driver";
                    return false;
                }

                string failure = null;
                bool result = RoleEx.IsSimGoodForRole(sim, mRole, null, out failure);

                sFailureConditions += failure;

                return result;
            }

            protected class TypeColumn : ObjectPickerDialogEx.CommonHeaderInfo<SimDescription>
            {
                public TypeColumn()
                    : base("NRaas.Register.Type:ColumnHeader", "NRaas.Register.Type:ColumnTooltip", 30)
                { }

                public override ObjectPicker.ColumnInfo GetValue(SimDescription item)
                {
                    string type = null;

                    if (SimTypes.InServicePool(item))
                    {
                        type += Common.Localize("Type:Service", item.IsFemale);
                    }

                    if (item.AssignedRole != null)
                    {
                        type += Common.Localize("Type:Role", item.IsFemale);
                    }

                    if (item.LotHome != null)
                    {
                        type += Common.Localize("Type:Resident", item.IsFemale);
                    }
                    else if ((item.Household == null) || (!item.Household.IsServiceNpcHousehold))
                    {
                        type += Common.Localize("Type:Homeless", item.IsFemale);
                    }

                    if (CriteriaItem.HasRealJob(item))
                    {
                        type += Common.Localize("Type:Employed", item.IsFemale);
                    }

                    return new ObjectPicker.TextColumn(type);
                }
            }
        }
    }
}