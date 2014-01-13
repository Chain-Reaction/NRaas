using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Interactions;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
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
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Socializing;
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

namespace NRaas.RegisterSpace.Options.Service
{
    public class SelectService : OperationSettingOption<Lot>
    {
        public readonly ServiceType mType;

        public SelectService(ServiceType type)
        {
            mType = type;
        }

        public override string GetTitlePrefix()
        {
            return "SelectService";
        }

        public override string Name
        {
            get
            {
                string serviceName;
                if (!Localization.GetLocalizedString("Ui/Caption/Services/Service:" + mType.ToString(), out serviceName))
                {
                    serviceName = "Ui/Caption/Services/Service:" + mType.ToString();
                }

                return serviceName;
            }
        }

        protected override bool Allow(GameHitParameters<Lot> parameters)
        {
            if (!Register.Settings.mShowLotMenu) return false;

            if (parameters.mTarget.Household == null) return false;

            if (parameters.mTarget.Household.Sims.Count == 0) return false;  // Humans

            return base.Allow(parameters);
        }

        protected override OptionResult Run(GameHitParameters<Lot> parameters)
        {
            Sim actorSim = parameters.mActor as Sim;

            SimSelection selection = new SimSelection(Name, actorSim.SimDescription, mType);
            if (selection.IsEmpty)
            {
                Common.Notify(Common.Localize(GetTitlePrefix() + ":NoChoices"));
                return OptionResult.Failure;
            }

            SimDescription sim = selection.SelectSingle();
            if (sim == null) return OptionResult.Failure;

            SimDescription existing;
            if (sim.CreatedByService.mPreferredServiceNpc.TryGetValue(parameters.mTarget.Household.HouseholdId, out existing))
            {
                if (existing.CreatedSim != null)
                {
                    ServiceSituation.Fire(existing.CreatedSim, parameters.mTarget.Household.Sims[0]);  // Human
                }
            }

            sim.CreatedByService.mPreferredServiceNpc[parameters.mTarget.Household.HouseholdId] = sim;

            Common.Notify(Common.Localize(GetTitlePrefix() + ":Success", sim.IsFemale, new object[] { sim, parameters.mTarget.Household.Name }));
            return OptionResult.SuccessRetain;
        }

        protected class SimSelection : ProtoSimSelection<SimDescription>
        {
            ServiceType mType;

            public SimSelection(string title, SimDescription actor, ServiceType type)
                : base(title, actor, Household.NpcHousehold.AllSimDescriptions, true, false)
            {
                mType = type;
            }

            protected override bool Allow(SimDescription item)
            {
                return SimTypes.InServicePool(item, mType);
            }
        }

        public class ListingOption : InteractionOptionList<SelectService, Lot>, ILotOption
        {
            Lot mLot;

            public override string GetTitlePrefix()
            {
                return "SelectService";
            }

            public override ITitlePrefixOption ParentListingOption
            {
                get { return null; }
            }

            protected override bool Allow(GameHitParameters<Lot> parameters)
            {
                if (!Register.Settings.mShowLotMenu) return false;

                mLot = parameters.mTarget;

                if (mLot.Household == null) return false;

                if (mLot.Household.Sims.Count == 0) return false; // Humans

                return base.Allow(parameters);
            }

            public override List<SelectService> GetOptions()
            {
                List<SelectService> items = new List<SelectService>();

                foreach (Sims3.Gameplay.Services.Service service in Services.AllServices)
                {
                    if (!service.AlwaysTryToSendSameSim) continue;

                    //if (!service.IsRecurrent()) continue;

                    //if (!service.IsServiceRequested(mLot)) continue;

                    items.Add(new SelectService(service.ServiceType));
                }

                return items;
            }
        }
    }
}