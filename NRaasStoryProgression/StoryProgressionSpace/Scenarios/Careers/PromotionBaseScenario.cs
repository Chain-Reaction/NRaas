using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Interfaces;
using NRaas.StoryProgressionSpace.Managers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.StoryProgressionSpace.Scenarios.Careers
{
    public abstract class PromotionBaseScenario<TData> : SimScenario
    {
        public PromotionBaseScenario(SimDescription sim)
            : base (sim)
        { }
        protected PromotionBaseScenario(PromotionBaseScenario<TData> scenario)
            : base (scenario)
        { }

        protected override bool CheckBusy
        {
            get { return false; }
        }

        protected override bool Progressed
        {
            get { return false; }
        }

        protected override bool AllowActive
        {
            get { return true; }
        }

        protected override ICollection<SimDescription> GetSims()
        {
            return Sims.All;
        }

        protected override bool Allow(SimDescription sim)
        {
            if (!(sim.Occupation is Career))
            {
                IncStat("No Job");
                return false;
            }
            else if (sim.Occupation is Retired)
            {
                IncStat("Retired");
                return false;
            }
            else if (!Careers.Allow(this, sim))
            {
                IncStat("User Denied");
                return false;
            }

            return base.Allow(sim);
        }

        public class PromotionDataSet
        {
            Dictionary<OccupationNames, Dictionary<int, Dictionary<string, TData>>> mLevels = new Dictionary<OccupationNames, Dictionary<int, Dictionary<string, TData>>>();

            public delegate TData OnPopulate(XmlDbRow row);

            public TData GetDataForLevel(SimDescription sim)
            {
                if (sim.Occupation != null)
                {
                    Dictionary<int, Dictionary<string, TData>> levels;
                    if (mLevels.TryGetValue(sim.Occupation.Guid, out levels))
                    {
                        Dictionary<string, TData> branches;
                        if (levels.TryGetValue(sim.Occupation.CareerLevel, out branches))
                        {
                            string branch = "Base";

                            Career career = sim.Occupation as Career;
                            if (career != null)
                            {
                                branch = career.CurLevelBranchName;
                            }

                            TData data;
                            if (branches.TryGetValue(branch, out data))
                            {
                                return data;
                            }
                        }
                    }
                }

                return default(TData);
            }

            public void Parse(string key, OnPopulate populate)
            {
                XmlDbData careerFile = XmlDbData.ReadData(key);
                if ((careerFile != null) && (careerFile.Tables != null) && (careerFile.Tables.ContainsKey("CareerData")))
                {
                    XmlDbTable table = careerFile.Tables["CareerData"];

                    foreach (XmlDbRow row in table.Rows)
                    {
                        string guid = row.GetString("Career");

                        string branch = row.GetString("Branch");
                        if (string.IsNullOrEmpty(branch))
                        {
                            branch = "Base";
                        }

                        int level = row.GetInt("Level");
                        if (level == 1) continue;

                        OccupationNames careerGuid = OccupationNames.Undefined;
                        ParserFunctions.TryParseEnum<OccupationNames>(guid, out careerGuid, OccupationNames.Undefined);

                        if (careerGuid == OccupationNames.Undefined)
                        {
                            careerGuid = unchecked((OccupationNames)ResourceUtils.HashString64(guid));
                        }

                        Dictionary<int, Dictionary<string, TData>> levels;
                        if (!mLevels.TryGetValue(careerGuid, out levels))
                        {
                            levels = new Dictionary<int, Dictionary<string, TData>>();
                            mLevels.Add(careerGuid, levels);
                        }

                        Dictionary<string, TData> branches;
                        if (!levels.TryGetValue(level, out branches))
                        {
                            branches = new Dictionary<string, TData>();
                            levels.Add(level, branches);
                        }

                        if (branches.ContainsKey(branch)) continue;

                        branches.Add(branch, populate(row));
                    }
                }
            }
        }
    }
}
