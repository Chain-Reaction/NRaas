using NRaas.CareerSpace.Booters;
using NRaas.CareerSpace.Careers;
using NRaas.CareerSpace.Interfaces;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Electronics;
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
    public class HomemakerReport : Computer.ComputerInteraction, Common.IAddInteraction
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
                try
                {
                    if (!Target.StartComputing(this, SurfaceHeight.Table, true))
                    {
                        StandardExit();
                        return false;
                    }

                    AnimateSim("GenericTyping");

                    Homemaker career = Actor.Occupation as Homemaker;

                    string notice = career.GetNotice();
                    
                    Common.Notify(notice);

                    Common.DebugWriteLog(notice);

                    Target.StopComputing(this, Computer.StopComputingAction.TurnOff, false);
                }
                finally
                {
                    StandardExit();
                }
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

        private sealed class Definition : InteractionDefinition<Sim, Computer, HomemakerReport>
        {
            public override string GetInteractionName(ref InteractionInstanceParameters parameters)
            {
                return Common.Localize("HomemakerReport:MenuName");
            }

            public override bool Test(Sim a, Computer target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                Homemaker career = a.Occupation as Homemaker;
                if (career == null) return false;

                if (!target.IsComputerUsable(a, true, false, isAutonomous)) return false;

                return true;
            }
        }
    }
}
