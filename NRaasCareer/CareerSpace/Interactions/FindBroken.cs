using NRaas.CareerSpace.Booters;
using NRaas.CareerSpace.Interfaces;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.CareerSpace.Interactions
{
    public class FindBroken : Computer.ComputerInteraction, Common.IAddInteraction
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<Computer>(Singleton);
        }

        public override bool Run()
        {
            try
            {
                StandardEntry();
                if (!Target.StartComputing(this, SurfaceHeight.Table, true))
                {
                    StandardExit();
                    return false;
                }

                AnimateSim("GenericTyping");

                List<GameObject> broken = new List<GameObject>();
                List<GameObject> repairable = new List<GameObject>();
                foreach (GameObject obj in Sims3.Gameplay.Queries.GetObjects<GameObject>())
                {
                    if (!obj.InWorld) continue;

                    if (obj.InUse) continue;

                    if (!obj.IsRepairable) continue;

                    if (obj.InInventory) continue;

                    if (obj.LotCurrent == Actor.LotHome) continue;

                    if (obj.LotCurrent == null) continue;

                    if (obj.LotCurrent.Household == null) continue;

                    if (obj.IsInHiddenResidentialRoom) continue;

                    RepairableComponent component = obj.Repairable;
                    if (component == null) continue;

                    bool found = false;
                    foreach (Sim sim in obj.LotCurrent.Household.Sims)
                    {
                        if (!sim.SimDescription.TeenOrAbove) continue;

                        if (sim.LotCurrent != obj.LotCurrent) continue;

                        found = true;
                        break;
                    }

                    if ((found) && (component.Broken))
                    {
                        broken.Add(obj);
                    }
                    else
                    {
                        if (obj.LotCurrent == Actor.LotCurrent) continue;

                        repairable.Add(obj);
                    }                   
                }

                GameObject choice = null;
                if (broken.Count > 0)
                {
                    choice = RandomUtil.GetRandomObjectFromList(broken);
                }
                else if (repairable.Count > 0)
                {
                    if (NRaas.Careers.Settings.mRepairAllowToBreak)
                    {
                        choice = RandomUtil.GetRandomObjectFromList(repairable);

                        if (!choice.Repairable.Broken)
                        {
                            choice.Repairable.BreakObject();
                        }
                    }
                    else
                    {
                        Common.Notify(Actor, Common.Localize("FindBroken:Failure", Actor.IsFemale));
                    }
                }

                if (choice != null)
                {
                    Camera.FocusOnGivenPosition(choice.Position, Camera.kDefaultLerpDuration);
                }

                Target.StopComputing(this, Computer.StopComputingAction.TurnOff, false);
                StandardExit();
            }
            catch (Exception exception)
            {
                Common.Exception(Actor, Target, exception);
            }
            return true;
        }

        private sealed class Definition : InteractionDefinition<Sim, Computer, FindBroken>
        {
            public override string GetInteractionName(Sim actor, Computer target, InteractionObjectPair iop)
            {
                return Common.Localize("Repairman:FindBroken", actor.IsFemale, new object[0]);
            }

            public override bool Test(Sim a, Computer target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (isAutonomous) return false;

                if (!target.IsComputerUsable(a, true, false, isAutonomous)) return false;

                SkillBasedCareer career = SkillBasedCareerBooter.GetSkillBasedCareer(a, SkillNames.Handiness);
                return (career != null);
            }
        }
    }
}
