using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.MapTags;
using Sims3.Gameplay.Objects;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Basic
{
    public class FindGrave : SimFromList, IBasicOption
    {
        Sim mActor;

        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            mActor = parameters.mActor as Sim;

            Sim sim = parameters.mTarget as Sim;
            if (sim != null) 
            {
                if (!sim.SimDescription.IsDead) return false;

                Urnstone urnstone = Urnstone.FindGhostsGrave(sim.SimDescription);
                if (urnstone == null) return false;

                Inventory inventory = Inventories.ParentInventory(urnstone);
                if ((inventory != null) && (inventory.Owner == sim)) return false;
            }

            return base.Allow(parameters);
        }

        public override string GetTitlePrefix()
        {
            return "FindGrave";
        }

        protected override List<SimSelection.ICriteria> GetCriteria(GameHitParameters<GameObject> parameters)
        {
            List<SimSelection.ICriteria> elements = new List<SimSelection.ICriteria>();

            List<SimTypeOr.Item> options = new List<SimTypeOr.Item>();
            options.Add(new SimTypeOr.Item(SimType.Dead));

            elements.Add(new SimTypeOr(options));

            return elements;
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            //if (!base.Allow(me)) return false;

            if (me.IsGhost) return true;

            if (me.Household == null) return true;

            return (me.IsDead);
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            Urnstone urnstone = Urnstones.CreateGrave(me, false);
            if (urnstone == null) return false;

            if ((urnstone.InInventory) || (!urnstone.InWorld))
            {
                if (urnstone.InInventory)
                {
                    Inventory inventory = Inventories.ParentInventory(urnstone);
                    if ((inventory != null) && (inventory.Owner == me.CreatedSim))
                    {
                        Camera.FocusOnGivenPosition(me.CreatedSim.Position, 6f);
                        return false;
                    }

                    if (!AcceptCancelDialog.Show(Common.Localize("FindGrave:Prompt")))
                    {
                        return false;
                    }
                }

                bool bOriginalValue = mActor.Inventory.IgnoreInventoryValidation;
                mActor.Inventory.IgnoreInventoryValidation = true;

                try
                {
                    mActor.Inventory.TryToMove(urnstone);
                }
                finally
                {
                    mActor.Inventory.IgnoreInventoryValidation = bOriginalValue;
                }
            }
            else
            {
                urnstone.FadeIn();

                Camera.FocusOnGivenPosition(urnstone.Position, 6f);
            }

            return true;
        }
    }
}
