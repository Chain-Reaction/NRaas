using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Interactions;
using NRaas.DebugEnablerSpace.Interfaces;
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
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.DebugEnablerSpace.Interactions
{
    public class CrateTraitChip : DebugEnablerInteraction<Sim>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override void AddPair(GameObject obj, List<InteractionObjectPair> list)
        {
            if (obj is Sim)
            {
                list.Add(new InteractionObjectPair(Singleton, obj));
            }
        }

        public override bool Run()
        {
            try
            {
                Definition definition = InteractionDefinition as Definition;

                TraitChip chip = TraitChipManager.CreateTraitChip(definition.mChip);
                if (chip == null) return false;

                Inventories.TryToMove(chip, Target);
                return true;
            }
            catch (Exception exception)
            {
                Common.Exception(Actor, Target, exception);
                return false;
            }
        }

        [DoesntRequireTuning]
        private sealed class Definition : DebugEnablerDefinition<CrateTraitChip>
        {
            public readonly TraitChipName mChip;

            public Definition()
            { }
            public Definition(TraitChipName chip)
            {
                mChip = chip;
            }

            public override void AddInteractions(InteractionObjectPair iop, IActor actor, Sim target, List<InteractionObjectPair> results)
            {
                if (mChip == TraitChipName.Undefined)
                {
                    foreach (TraitChipName chip in Enum.GetValues(typeof(TraitChipName)))
                    {
                        switch (chip)
                        {
                            case TraitChipName.Undefined:
                            case TraitChipName.MaxTraitChipName:
                                continue;
                        }

                        results.Add(new InteractionObjectPair(new Definition(chip), target));
                    }
                }
                else
                {
                    base.AddInteractions(iop, actor, target, results);
                }
            }

            public override string GetInteractionName(IActor a, Sim target, InteractionObjectPair interaction)
            {
                TraitChipStaticData chipData = TraitChipManager.GetStaticElement(mChip);
                if (chipData != null)
                {
                    return Localization.LocalizeString(chipData.TraitChipNameKey, new object[0x0]);
                }
                return null;
            }

            public override string[] GetPath(bool isFemale)
            {
                return new string[] { Common.Localize("CrateTraitChip:MenuName") };
            }

            public override bool Test(IActor a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return base.Test(a, target, isAutonomous, ref greyedOutTooltipCallback);
            }
        }
    }
}