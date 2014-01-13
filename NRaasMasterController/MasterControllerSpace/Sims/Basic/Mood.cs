using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Basic
{
    public class Mood : SimFromList, IBasicOption
    {
        public override string GetTitlePrefix()
        {
            return "Mood";
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            if (me.CreatedSim == null) return false;

            if (me.CreatedSim.Motives == null) return false;

            return true;
        }

        protected class MotiveValue
        {
            public readonly string mName;
            public readonly int mValue;
            public readonly bool mExists;

            public MotiveValue(CommodityKind name, Motive value)
            {
                if (!Common.Localize("CommodityKind:" + name, false, new object[0], out mName))
                {
                    mName = name.ToString();
                }

                if (value != null)
                {
                    mValue = (int)value.Value;
                    mExists = true;
                }
            }
            public MotiveValue(MotiveID name, Motive value)
            {
                mName = Common.LocalizeEAString("Ui/Caption/HUD/MotivesPanel:Motive" + name.ToString());

                if (value != null)
                {
                    mValue = (int)value.Value;
                    mExists = true;
                }
            }

            public static int OnSort(MotiveValue l, MotiveValue r)
            {
                return l.mName.CompareTo(r.mName);
            }
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            Sim sim = me.CreatedSim;
            if (sim == null) return false;

            if (sim.Motives == null) return false;

            List<MotiveValue> values = new List<MotiveValue> ();

            if (Common.kDebugging)
            {
                foreach (Motive motive in sim.Motives.AllMotives)
                {
                    values.Add(new MotiveValue(motive.Commodity, motive));
                }

                values.Sort(MotiveValue.OnSort);
            }
            else
            {
                values.Add(new MotiveValue(MotiveID.Bladder, sim.Motives.GetMotive(CommodityKind.Bladder)));
                values.Add(new MotiveValue(MotiveID.Energy, sim.Motives.GetMotive(CommodityKind.Energy)));
                values.Add(new MotiveValue(MotiveID.Energy, sim.Motives.GetMotive(CommodityKind.AlienBrainPower)));
                values.Add(new MotiveValue(MotiveID.Fun, sim.Motives.GetMotive(CommodityKind.Fun)));
                values.Add(new MotiveValue(MotiveID.Hunger, sim.Motives.GetMotive(CommodityKind.Hunger)));
                values.Add(new MotiveValue(MotiveID.Hunger, sim.Motives.GetMotive(CommodityKind.VampireThirst)));
                values.Add(new MotiveValue(MotiveID.Hygiene, sim.Motives.GetMotive(CommodityKind.Hygiene)));
                values.Add(new MotiveValue(MotiveID.Hygiene, sim.Motives.GetMotive(CommodityKind.CatScratch)));
                values.Add(new MotiveValue(MotiveID.Hygiene, sim.Motives.GetMotive(CommodityKind.DogDestruction)));
                values.Add(new MotiveValue(MotiveID.Social, sim.Motives.GetMotive(CommodityKind.Social)));
                values.Add(new MotiveValue(CommodityKind.Temperature, sim.Motives.GetMotive(CommodityKind.Temperature)));
                values.Add(new MotiveValue(CommodityKind.MagicFatigue, sim.Motives.GetMotive(CommodityKind.MagicFatigue)));
            }

            Common.StringBuilder msg = new Common.StringBuilder (me.FullName + Common.NewLine + Common.NewLine + Common.Localize("Mood:Range"));

            foreach (MotiveValue value in values)
            {
                if (!value.mExists) continue;

                msg += Common.Localize("Mood:Element", sim.IsFemale, new object[] { value.mName, value.mValue });
            }

            SimpleMessageDialog.Show(Name, msg.ToString());

            Common.DebugWriteLog(msg);
            return true;
        }
    }
}
