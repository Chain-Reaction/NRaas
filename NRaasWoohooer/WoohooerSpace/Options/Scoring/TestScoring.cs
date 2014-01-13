using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.CommonSpace.Selection;
using NRaas.WoohooerSpace.Scoring;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.WoohooerSpace.Options.Scoring
{
    public class TestScoring : OptionItem, IScoringOption
    {
        protected override bool Allow(GameHitParameters<GameObject> parameters)
        {
            if (ScoringLookup.Count == 0) return false;

            return NRaas.Woohooer.Settings.Debugging;
        }

        public override string GetTitlePrefix()
        {
            return "TestScoring";
        }

        protected override OptionResult Run(GameHitParameters<GameObject> parameters)
        {
            List<Item> options = new List<Item>();

            foreach (string name in ScoringLookup.ScoringKeys)
            {
                options.Add(new Item(name));
            }

            CommonSelection<Item>.Results choices = new CommonSelection<Item>(Name, options).SelectMultiple();
            if (choices.Count == 0) return OptionResult.SuccessClose;

            StringBuilder msg = new StringBuilder();

            foreach (Item item in choices)
            {
                IListedScoringMethod scoring = ScoringLookup.GetScoring(item.Name);
                if (scoring == null) continue;

                msg.Append(Common.NewLine + "Scoring: " + scoring.Name);

                Selection.Results allSims = new Selection(Household.EverySimDescription(), item.Name).SelectMultiple();

                float totalNegScore = 0, totalPosScore = 0;
                int totalNegCount = 0, totalPosCount = 0;

                foreach (SimDescription sim in allSims)
                {
                    msg.Append(Common.NewLine + "Actor: " + sim.FullName);

                    if (scoring is DualSimListedScoringMethod)
                    {
                        SimDescription other = Sim.ActiveActor.SimDescription;

                        msg.Append(Common.NewLine + "Other: " + other.FullName);

                        int score = scoring.IScore(new DualSimScoringParameters(sim, other, true));

                        if (score > 0)
                        {
                            totalPosScore += score;
                            totalPosCount++;
                        }
                        else
                        {
                            totalNegScore += score;
                            totalNegCount++;
                        }

                        msg.Append(Common.NewLine + "Score: " + score);
                    }
                    else
                    {
                        int score = scoring.IScore(new SimScoringParameters(sim, true));

                        if (score > 0)
                        {
                            totalPosScore += score;
                            totalPosCount++;
                        }
                        else
                        {
                            totalNegScore += score;
                            totalNegCount++;
                        }

                        msg.Append(Common.NewLine + "Score: " + score);
                    }

                    msg.Append(Common.NewLine);
                }

                msg.Append(Common.NewLine + "Average Scoring: " + scoring.Name);
                msg.Append(Common.NewLine + "Pos: " + totalPosCount + " " + (totalPosScore / totalPosCount));
                msg.Append(Common.NewLine + "Neg: " + totalNegCount + " " + (totalNegScore / totalNegCount));
            }

            Common.WriteLog(msg.ToString());
            return OptionResult.SuccessRetain;
        }

        public class Item : ValueSettingOption<string>
        {
            public Item(string name)
                : base (name, name, 0)
            { }
        }

        public class Selection : ProtoSimSelection<SimDescription>
        {
            public Selection(ICollection<SimDescription> list, string subTitle)
                : base(Common.Localize("TestScoring:MenuName"), null, list, true, true)
            {
                mSubTitle = subTitle;
            }
        }
    }
}
