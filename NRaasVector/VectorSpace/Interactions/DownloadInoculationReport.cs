using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Selection;
using NRaas.VectorSpace.Booters;
using NRaas.VectorSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.RealEstate;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.VectorSpace.Interactions
{
    public class DownloadInoculationReport : Computer.ComputerInteraction, Common.IAddInteraction
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
                        return false;
                    }

                    AnimateSim("GenericTyping");

                    string result = Common.Localize("InoculationReport:Title", Actor.IsFemale);

                    int count = 0;

                    bool foundAny = false;

                    foreach (SimDescription sim in Households.All(Actor.Household))
                    {
                        bool found = false, first = true;

                        foreach (DiseaseVector vector in Vector.Settings.GetVectors(sim))
                        {
                            if (!vector.IsIdentified) continue;

                            if (vector.InoculationCost <= 0) continue;

                            if (first)
                            {
                                first = false;
                                result += Common.NewLine + Common.NewLine + Common.Localize("InoculationReport:Patient", sim.IsFemale, new object[] { sim });
                            }

                            string incurable = null;
                            if (!vector.CanInoculate)
                            {
                                incurable = "Incurable";
                            }

                            result += Common.NewLine + Common.Localize("InoculationReport:Vector" + incurable, sim.IsFemale, new object[] { vector.GetLocalizedName(sim.IsFemale), Common.Localize("YesNo:" + vector.IsInoculated) });

                            if (vector.IsInoculated)
                            {
                                result += Common.Localize("InoculationReport:InoculationDuration", sim.IsFemale, new object[] { vector.InoculationStrain, Vector.Settings.GetCurrentStrain(vector.Data).Strain });
                            }

                            found = true;

                            count++;
                        }

                        if ((!found) && (!first))
                        {
                            result += Common.NewLine + Common.Localize("InoculationReport:None", sim.IsFemale);
                        }

                        if (found)
                        {
                            foundAny = true;
                        }

                        if (count > 10)
                        {
                            Common.Notify(result);

                            result = Common.Localize("InoculationReport:Title", Actor.IsFemale);

                            count = 0;
                        }
                    }

                    if (!foundAny)
                    {
                        result += Common.NewLine + Common.Localize("InoculationReport:None", Actor.IsFemale);

                        count++;
                    }

                    if (count > 0)
                    {
                        Common.Notify(result);
                    }

                    Target.StopComputing(this, Computer.StopComputingAction.TurnOff, false);
                    return true;
                }
                finally
                {
                    StandardExit();
                }
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

        private class Definition : InteractionDefinition<Sim, Computer, DownloadInoculationReport>
        {
            public override string GetInteractionName(Sim actor, Computer target, InteractionObjectPair iop)
            {
                return Common.Localize("DownloadInoculationReport:MenuName", actor.IsFemale);
            }

            public override bool Test(Sim a, Computer target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (!VectorBooter.HasVectors) return false;

                return target.IsComputerUsable(a, true, false, isAutonomous);
            }
        }
    }
}

