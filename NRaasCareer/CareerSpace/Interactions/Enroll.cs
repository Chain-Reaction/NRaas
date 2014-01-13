using NRaas.CareerSpace.Booters;
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
   [Persistable]
    public class Enroll : Computer.ComputerInteraction, Common.IAddInteraction
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public Enroll()
        { }

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Add<Computer>(Singleton);
        }

        protected static void ChooseSchool(Sim sim)
        {
            List<ObjectPicker.HeaderInfo> headers = new List<ObjectPicker.HeaderInfo>();
            headers.Add(new ObjectPicker.HeaderInfo("NRaasSchooling:OptionColumn", "NRaasSchooling:OptionColumnTooltip", 230));

            List<ObjectPicker.RowInfo> rowInfo = new List<ObjectPicker.RowInfo>();
            foreach (Career career in CareerManager.CareerList)
            {
                if (career is SchoolElementary)
                {
                    if (!sim.SimDescription.Child) continue;
                }
                else if (career is SchoolHigh)
                {
                    if (!sim.SimDescription.Teen) continue;
                }
                else if (career is School)
                {
                    if ((!sim.SimDescription.Child) && (!sim.SimDescription.Teen)) continue;
                }
                else
                {
                    continue;
                }

                GreyedOutTooltipCallback greyedOutTooltipCallback = null;
                if (!career.CanAcceptCareer(sim.ObjectId, ref greyedOutTooltipCallback)) continue;

                CareerLocation location = Career.FindClosestCareerLocation(sim, career.Guid);
                if (location == null) continue;

                ObjectPicker.RowInfo item = new ObjectPicker.RowInfo(location, new List<ObjectPicker.ColumnInfo>());

                item.ColumnInfo.Add(new ObjectPicker.TextColumn(career.Name));

                rowInfo.Add(item);
            }

            List<ObjectPicker.TabInfo> tabInfo = new List<ObjectPicker.TabInfo>();
            tabInfo.Add(new ObjectPicker.TabInfo("shop_all_r2", Common.LocalizeEAString("Ui/Caption/ObjectPicker:All"), rowInfo));

            string buttonTrue = Common.LocalizeEAString("Ui/Caption/Global:Accept");
            string buttonFalse = Common.LocalizeEAString("Ui/Caption/ObjectPicker:Cancel");

            List<ObjectPicker.RowInfo> list = ObjectPickerDialog.Show(true, ModalDialog.PauseMode.PauseSimulator, Common.LocalizeEAString("NRaasSchooling:Title"), buttonTrue, buttonFalse, tabInfo, headers, 1, new Vector2(-1f, -1f), true);

            if ((list == null) || (list.Count == 0)) return;

            CareerLocation sel = list[0].Item as CareerLocation;
            if (sel == null) return;

            SchoolBooter.Enroll(sim, null, sel);
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

                    ChooseSchool(Actor);

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

        // Nested Types
        private sealed class Definition : InteractionDefinition<Sim, Computer, Enroll>
        {
            // Methods
            public override string GetInteractionName(ref InteractionInstanceParameters parameters)
            {
                return Common.LocalizeEAString("NRaasSchooling:InteractionMenuName");
            }

            public override bool Test(Sim a, Computer target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (!target.IsComputerUsable(a, true, false, isAutonomous)) return false;

                if ((!a.SimDescription.Child) && (!a.SimDescription.Teen)) return false;

                return (!isAutonomous);
            }
        }
    }
}
