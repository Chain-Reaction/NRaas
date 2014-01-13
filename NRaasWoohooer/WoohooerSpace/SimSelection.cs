using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Selection;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.WoohooerSpace.Helpers;
using NRaas.WoohooerSpace.Interactions;
using NRaas.WoohooerSpace.Skills;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.WoohooerSpace
{
    public class SimSelection : ProtoSimSelection<SimDescription>
    {
        public enum Type
        {
            Matchmaker,
            Rendezvous,
            Tantraport,
            ScanRoom,
        }

        Dictionary<SimDescription,int> mScores = new Dictionary<SimDescription,int>();

        int mMinimum;

        Type mType;

        public SimSelection(string title, SimDescription me, ICollection<SimDescription> sims, Type type, int minimum)
            : base(title, me, sims, true, false)
        {
            mMinimum = minimum;

            mType = type;

            switch(mType)
            {
                case Type.Rendezvous:
                    AddColumn(new SimPriceColumn());
                    AddColumn(new RelationshipColumn(this));
                    break;
                case Type.Matchmaker:
                case Type.ScanRoom:
                    AddColumn(new SimAttractionColumn(mScores));
                    AddColumn(new RelationshipColumn(this));
                    break;
            }
        }

        protected override void GetName(SimDescription sim, out string firstName, out string lastName)
        {
            if (mType == Type.Rendezvous)
            {
                firstName = "\"" + KamaSimtra.Settings.GetAlias(sim) + "\"";
                lastName = "";
            }
            else
            {
                base.GetName(sim, out firstName, out lastName);
            }
        }

        protected override bool AllowRow(SimDescription item)
        {
            if (item == Me) return false;

            switch (mType)
            {
                case Type.Matchmaker:
                    if (item.Partner != null) return false;
                    break;
                default:
                    if (SimTypes.IsDead(item)) return false;
                    break;
            }

            string reason;
            GreyedOutTooltipCallback greyedOutTooltipCallback = null;
            if (!CommonSocials.CanGetRomantic(Me, item, false, false, true, ref greyedOutTooltipCallback, out reason))
            {
                if (greyedOutTooltipCallback != null)
                {
                    Common.DebugNotify(item.FullName + Common.NewLine + greyedOutTooltipCallback());
                }
                return false;
            }

            switch(mType)
            {
                case Type.Matchmaker:
                case Type.ScanRoom:
                    if (Woohooer.Settings.mOnlyResidentMatchmaker)
                    {
                        if (item.LotHome == null) return false;
                    }

                    int score = (int)RelationshipEx.GetAttractionScore(item, Me, true);

                    mScores.Add(item, score);

                    return (score >= mMinimum);
                default:
                    return true;
            }
        }

        protected class SimAttractionColumn : ObjectPickerDialogEx.CommonHeaderInfo<SimDescription>
        {
            Dictionary<SimDescription, int> mScores;

            public SimAttractionColumn(Dictionary<SimDescription, int> scores)
                : base("NRaas.Woohooer.SimSelection:AttractionTitle", "NRaas.Woohooer.SimSelection:AttractionTooltip", 40)
            {
                mScores = scores;
            }

            public override ObjectPicker.ColumnInfo GetValue(SimDescription sim)
            {
                int value;
                if (!mScores.TryGetValue(sim, out value))
                {
                    value = 0;
                }

                return new ObjectPicker.TextColumn(value.ToString("D4"));
            }
        }

        protected class SimPriceColumn : ObjectPickerDialogEx.CommonHeaderInfo<SimDescription>
        {
            public SimPriceColumn()
                : base("NRaas.Woohooer.SimSelection:PriceTitle", "NRaas.Woohooer.SimSelection:PriceTooltip", 40)
            {}

            public override ObjectPicker.ColumnInfo GetValue(SimDescription sim)
            {
                int price = sim.SkillManager.GetSkillLevel(KamaSimtra.StaticGuid) * KamaSimtra.Settings.mRendezvousCostPerLevel;

                return new ObjectPicker.TextColumn(EAText.GetMoneyString(price));
            }
        }

        protected class RelationshipColumn : ObjectPickerDialogEx.CommonHeaderInfo<SimDescription>
        {
            SimSelection mSelection;

            public RelationshipColumn(SimSelection selection)
                : base("NRaas.Woohooer.SimSelection:RelationTitle", "NRaas.Woohooer.SimSelection:RelationTooltip", 40)
            {
                mSelection = selection;
            }

            public override ObjectPicker.ColumnInfo GetValue(SimDescription sim)
            {
                string result = "";

                switch (sim.Age)
                {
                    case CASAgeGenderFlags.Teen:
                    case CASAgeGenderFlags.YoungAdult:
                    case CASAgeGenderFlags.Adult:
                    case CASAgeGenderFlags.Elder:
                        result += Common.Localize("Matchmaker:" + sim.Age.ToString(), sim.IsFemale);
                        break;
                }
               

                if (SimTypes.InServicePool(sim))
                {
                    result += Common.Localize("Matchmaker:Service", sim.IsFemale);
                }

                if (sim.AssignedRole != null)
                {
                    result += Common.Localize("Matchmaker:Role", sim.IsFemale);
                }

                if (sim.LotHome == null)
                {
                    result += Common.Localize("Matchmaker:Homeless", sim.IsFemale);
                }

                if (Relationships.IsCloselyRelated(mSelection.Me, sim, false))
                {
                    result += Common.Localize("Matchmaker:Related", sim.IsFemale);
                }

                return new ObjectPicker.TextColumn(result);
            }
        }
    }
}
