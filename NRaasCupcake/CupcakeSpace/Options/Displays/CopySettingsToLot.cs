using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.CupcakeSpace.Helpers;
using NRaas.CupcakeSpace.Options.Displays;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.MapTags;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Decorations;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.Store.Objects;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.CupcakeSpace.Options.Cases
{
    public class CopySettingsToLot : OperationSettingOption<GameObject>, ICaseOption
    {
        IGameObject mTarget;

        public CopySettingsToLot()
        { }

        public override string GetTitlePrefix()
        {
            return "CopySettingsToLot";
        }

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            mTarget = parameters.mTarget;
            if (mTarget == null) return false;

            if (!(mTarget is CraftersConsignmentDisplay)) return false;

            return base.Allow(parameters);
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            if (mTarget.LotCurrent == null)
            {
                return OptionResult.Failure;
            }

            Dictionary<int, Dictionary<string, List<Quality>>> settings = Cupcake.Settings.GetDisplaySettings(mTarget.ObjectId);

            if (settings == null)
            {
                Common.Notify("Unable to find settings for that display.");
                return OptionResult.Failure;
            }

            DisplayHelper.DisplayTypes displayType = DisplayHelper.GetDisplayType(mTarget as CraftersConsignmentDisplay);

            bool complain = false;            
            CraftersConsignmentDisplay[] displays = mTarget.LotCurrent.GetObjects<CraftersConsignmentDisplay>();

            foreach (CraftersConsignmentDisplay display in displays)
            {
                Dictionary<int, Dictionary<string, List<Quality>>> newSettings = new Dictionary<int, Dictionary<string, List<Quality>>>();

                DisplayHelper.DisplayTypes copyDisplayType;
                Dictionary<int, Slot> containmentSlots = DisplayHelper.GetEmptyOrFoodSlots(display as CraftersConsignmentDisplay, out copyDisplayType);

                if (copyDisplayType != displayType)
                {
                    complain = true;
                    continue;
                }

                foreach (int slot in containmentSlots.Keys)
                {
                    if (settings.ContainsKey(slot))
                    {
                        newSettings.Add(slot, settings[slot]);
                    }
                }

                Cupcake.Settings.SetDisplaySettings(display.ObjectId, newSettings);
            }

            if (complain)
            {
                Common.Notify("Some displays were skipped as they were not of this displays type.");
            }

            Common.Notify(Common.Localize("General:Success"));

            return OptionResult.SuccessClose;
        }
    }
}