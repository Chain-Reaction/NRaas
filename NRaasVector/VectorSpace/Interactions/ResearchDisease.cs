using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Selection;
using NRaas.VectorSpace.Booters;
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
    public class ResearchDisease : Computer.ComputerInteraction, Common.IAddInteraction
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

                    List<VectorBooter.Item> choices = VectorBooter.GetVectorItems(null);

                    VectorBooter.Item item = new CommonSelection<VectorBooter.Item>(GetInteractionName(), choices, new StrainColumn()).SelectSingle();
                    if (item == null) return false;

                    Common.Notify(item.Value.GetResearch(Actor.IsFemale));

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

        private class Definition : InteractionDefinition<Sim, Computer, ResearchDisease>
        {
            public override string GetInteractionName(Sim actor, Computer target, InteractionObjectPair iop)
            {
                return Common.Localize("ResearchDisease:MenuName", actor.IsFemale);
            }

            public override bool Test(Sim a, Computer target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (!VectorBooter.HasVectors) return false;

                return target.IsComputerUsable(a, true, false, isAutonomous);
            }
        }

        protected class StrainColumn : ObjectPickerDialogEx.CommonHeaderInfo<VectorBooter.Item>
        {
            public StrainColumn()
                : base("NRaas.Vector.OptionList:StrainTitle", "NRaas.Vector.OptionList:StrainTooltip", 30)
            { }

            public override ObjectPicker.ColumnInfo GetValue(VectorBooter.Item item)
            {
                return new ObjectPicker.TextColumn(EAText.GetNumberString(item.Count));
            }
        }
    }
}

