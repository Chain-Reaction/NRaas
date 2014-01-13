using NRaas.CareerSpace;
using NRaas.CareerSpace.Booters;
using NRaas.CareerSpace.Interfaces;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.Gameplay.Careers
{
    [Persistable]
    public class Unemployed : OmniCareer
    {
        static List<string> sTitles = new List<string>();

        [Persistable(false)]
        XmlDbRow mOriginalLevelData = null;

        string mName = null;

        public int mStipend = 0;

        static Unemployed()
        {
            for (int i = 0; i < 20; i++)
            {
                sTitles.Add("NRaasUnemployedName" + i.ToString());
            }
        }
        public Unemployed()
        { }
        public Unemployed(XmlDbRow myRow, XmlDbTable levelTable, XmlDbTable eventDataTable)
            : base(myRow, levelTable, eventDataTable)
        {
            try
            {
                if (mOriginalLevelData == null)
                {
                    if ((levelTable != null) && (levelTable.Rows.Count > 0))
                    {
                        mOriginalLevelData = levelTable.Rows[0];
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception(Name, e);
            }
        }

        protected static void Notify(Sim sim, string msg)
        {
            if (sim == null) return;

            if (sim.Household == Household.ActiveHousehold)
            {
                StyledNotification.Show(new StyledNotification.Format(msg, sim.ObjectId, ObjectGuid.InvalidObjectGuid, StyledNotification.NotificationStyle.kSystemMessage));
            }
        }

        protected override int Stipend
        {
            get { return mStipend; }
        }

        public override bool HasOpenHours
        {
            get { return true; }
        }

        public override bool CanRetire()
        {
            return false;
        }

        public override int CalculateBoostedStartingLevel()
        {
            return -1;
        }

        public override bool ShouldBeAtWork()
        {
            mPayPerHourExtra = mStipend;

            if ((OwnerDescription != null) &&
               (OwnerDescription.CareerManager != null) &&
               (OwnerDescription.CareerManager.RetiredCareer != null))
            {
                try
                {
                    mPayPerHourExtra += OwnerDescription.CareerManager.RetiredCareer.PensionAmount();
                }
                catch
                { }
            }

            return base.ShouldBeAtWork();
        }

        public List<Pair<string, string>> GetLocalizedTitles(bool isFemale)
        {
            string title = "Unemployed";
            if (mOriginalLevelData != null)
            {
                title = mOriginalLevelData.GetString("Title");
            }

            List<Pair<string, string>> results = new List<Pair<string, string>>();
            foreach (string name in sTitles)
            {
                string key = "Gameplay/Excel/Careers/" + name + ":" + title;

                string text = Common.LocalizeEAString(isFemale, key);
                if ((string.IsNullOrEmpty(text)) || (text == key)) continue;

                results.Add(new Pair<string, string>(text, name));
            }

            return results;
        }

        public void ChangeName()
        {
            List<ObjectPicker.HeaderInfo> headers = new List<ObjectPicker.HeaderInfo>();
            headers.Add(new ObjectPicker.HeaderInfo("NRaasUnemployed:OptionColumn", "NRaasUnemployed:OptionColumnTooltip", 230));

            List<ObjectPicker.RowInfo> rowInfo = new List<ObjectPicker.RowInfo>();
            foreach (Pair<string, string> name in GetLocalizedTitles(OwnerDescription.IsFemale))
            {
                ObjectPicker.RowInfo item = new ObjectPicker.RowInfo(name.Second, new List<ObjectPicker.ColumnInfo>());

                item.ColumnInfo.Add(new ObjectPicker.TextColumn(name.First));

                rowInfo.Add(item);
            }

            List<ObjectPicker.TabInfo> tabInfo = new List<ObjectPicker.TabInfo>();
            tabInfo.Add(new ObjectPicker.TabInfo("shop_all_r2", Common.LocalizeEAString("Ui/Caption/ObjectPicker:All"), rowInfo));

            string buttonTrue = Common.LocalizeEAString("NRaasUnemployed:Ok");
            string buttonFalse = Common.LocalizeEAString("Ui/Caption/ObjectPicker:Cancel");

            List<ObjectPicker.RowInfo> list = ObjectPickerDialog.Show(true, ModalDialog.PauseMode.PauseSimulator, Common.LocalizeEAString("NRaasUnemployed:Title"), buttonTrue, buttonFalse, tabInfo, headers, 1, new Vector2(-1f, -1f), true);

            List<string> selection = new List<string>();

            if ((list == null) || (list.Count == 0)) return;

            UpdateName(list[0].Item as string);
        }

        public override void OnStartup()
        {
            base.OnStartup();

            if (mOriginalLevelData == null)
            {
                Unemployed staticCareer = CareerManager.GetStaticCareer(Guid) as Unemployed;
                if (staticCareer != null)
                {
                    mOriginalLevelData = staticCareer.mOriginalLevelData;
                }
            }

            UpdateName(mName);
        }

        public void UpdateName(string name)
        {
            if (name == null) return;

            mName = name;

            if (mOriginalLevelData != null)
            {
                mCurLevel = new CareerLevel(mOriginalLevelData, name, SharedData.ProductVersion);
            }

            if (OwnerDescription.CareerManager != null)
            {
                OwnerDescription.CareerManager.UpdateCareerUI();
            }
        }

        [Persistable]
        public class ChangeNameInteraction : Computer.ComputerInteraction, Common.IAddInteraction
        {
            // Fields
            public static readonly InteractionDefinition Singleton = new Definition();

            public ChangeNameInteraction()
            { }

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

                    Unemployed job = Actor.Occupation as Unemployed;
                    if (job != null)
                    {
                        job.ChangeName();
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

            // Nested Types
            private sealed class Definition : InteractionDefinition<Sim, Computer, ChangeNameInteraction>
            {
                // Methods
                public override string GetInteractionName(Sim actor, Computer target, InteractionObjectPair iop)
                {
                    return Common.LocalizeEAString(actor.IsFemale, "NRaasUnemployed:InteractionMenuName", new object[0]);
                }

                public override bool Test(Sim a, Computer target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    Unemployed job = a.Occupation as Unemployed;
                    if (job == null) return false;

                    if (!target.IsComputerUsable(a, true, false, isAutonomous)) return false;

                    return (!isAutonomous);
                }
            }
        }
    }
}
