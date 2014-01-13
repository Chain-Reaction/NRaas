using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.HybridSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.HybridSpace.Interactions
{
    public class CreateCloneOfSimEx : ScienceLab.CreateCloneOfSim, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void OnPreLoad()
        {
            Tunings.Inject<ScienceLab, ScienceLab.CreateCloneOfSim.Definition, Definition>(false);

            sOldSingleton = ScienceLab.CreateCloneOfSim.Singleton;
            ScienceLab.CreateCloneOfSim.Singleton = new Definition();
        }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<ScienceLab, ScienceLab.CreateCloneOfSim.Definition>(ScienceLab.CreateCloneOfSim.Singleton);
        }

        public override bool InRabbitHole()
        {
            try
            {
                SimDescription oldSim = Actor.SimDescription;
                SimDescription newSim = Genetics.MakeDescendant(oldSim, oldSim, CASAgeGenderFlags.Child, oldSim.Gender, 100f, new Random(), false, true, true);
                newSim.WasCasCreated = false;
                
                /*
                if (!Household.ActiveHousehold.CanAddSpeciesToHousehold(Actor.SimDescription.Species))
                {
                    newSim.Dispose();
                    return false;
                }
                */

                oldSim.Household.Add(newSim);
                newSim.FirstName = StringInputDialog.Show(Localization.LocalizeString("Gameplay/Objects/RabbitHoles/ScienceLab:NameCloneTitle", new object[0x0]), Localization.LocalizeString("Gameplay/Objects/RabbitHoles/ScienceLab:NameCloneDesc", new object[0x0]), string.Empty, 256, StringInputDialog.Validation.SimNameText);
                Target.SetupNewClone(oldSim, newSim, "ep4CloneTransitionChild");

                // Custom
                foreach (OccultTypes type in OccultTypeHelper.CreateList(oldSim))
                {
                    OccultTypeHelper.Add(newSim, type, false, false);
                }

                IGameObject voucher = Actor.Inventory.Find<IVoucherCloneMe>();
                if (voucher != null)
                {
                    Actor.Inventory.RemoveByForce(voucher);
                    voucher.Destroy();
                    voucher = null;
                }

                EventTracker.SendEvent(EventTypeId.kNewOffspring, Actor, newSim.CreatedSim);
                EventTracker.SendEvent(EventTypeId.kParentAdded, newSim.CreatedSim, Actor);
                EventTracker.SendEvent(EventTypeId.kChildBornOrAdopted, null, newSim.CreatedSim);
                return true;
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
                return false;
            }
        }

        public new class Definition : ScienceLab.CreateCloneOfSim.Definition
        {
            public Definition()
            { }

            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance na = new CreateCloneOfSimEx();
                na.Init(ref parameters);
                return na;
            }

            public override string GetInteractionName(Sim actor, ScienceLab target, InteractionObjectPair iop)
            {
                return base.GetInteractionName(actor, target, new InteractionObjectPair(sOldSingleton, target));
            }

            public override bool Test(Sim actor, ScienceLab target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (actor.Inventory.Find<IVoucherCloneMe>() == null)
                {
                    return false;
                }

                /*
                if (!Household.ActiveHousehold.CanAddSpeciesToHousehold(actor.SimDescription.Species))
                {
                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(Localization.LocalizeString(actor.IsFemale, "Gameplay/Objects/RabbitHoles/ScienceLab:HouseholdTooLarge", new object[0x0]));
                    return false;
                }
                if (actor.OccultManager.HasAnyOccultType())
                {
                    return false;
                }

                if (actor.SimDescription.IsGhost)
                {
                    return false;
                }
                */

                if (GameUtils.IsOnVacation())
                {
                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(Localization.LocalizeString(actor.IsFemale, "Ui/Tooltip/Vacation/GreyedoutTooltip:InteractionNotValidOnVacation", new object[0x0]));
                    return false;
                }
                return true;
            }
        }
    }
}
