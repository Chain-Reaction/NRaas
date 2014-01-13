using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.SimIFace;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Demographics
{
    public class JobsByLevel : DemographicOption
    {
        public override string GetTitlePrefix()
        {
            return "JobsByLevel";
        }

        protected string GetDetails(List<IMiniSimDescription> sims)
        {
            long lUnemployed = 0, lRetired = 0, lWorkingPension = 0, lTotalWorking = 0;
            long lDemotionPerf = 0;
            long lPositivePerf = 0;

            List<TallyElement> levels = new List<TallyElement>();

            for (int level = 0; level < 10; level++)
            {
                levels.Add(new TallyElement(level + 1));
            }

            foreach (IMiniSimDescription miniSim in sims)
            {
                SimDescription member = miniSim as SimDescription;
                if (member == null) continue;

                if (SimTypes.IsSpecial(member)) continue;

                if (!member.YoungAdultOrAbove) continue;

                if (member.Occupation == null)
                {
                    if ((member.CareerManager != null) && (member.CareerManager.RetiredCareer != null))
                    {
                        lRetired++;
                    }
                    else
                    {
                        lUnemployed++;
                    }
                }
                else
                {
                    Career career = member.Occupation as Career;
                    if ((career != null) && (career.CareerLevel == 1) && (career.CurLevel != null) && (career.CurLevel.DayLength == 0))
                    {
                        lRetired++;
                    }
                    else
                    {
                        float performance = StatusJobPerformance.GetPerformance(member);

                        levels[member.Occupation.CareerLevel - 1].Add(performance);

                        if (performance >= 0.0)
                        {
                            lPositivePerf++;
                        }
                        else if (performance < Sims3.Gameplay.Careers.Career.kDemotionThreshold)
                        {
                            lDemotionPerf++;
                        }

                        if (member.CareerManager.RetiredCareer != null)
                        {
                            lWorkingPension++;
                        }
                        lTotalWorking++;
                    }
                }
            }

            string body = null;
            for (int level = 0; level < levels.Count; level++)
            {
                body += levels[level].ToString();
            }

            return Common.Localize("JobsByLevel:Body", false, new object[] { lUnemployed, lRetired, lWorkingPension, body, lTotalWorking, lPositivePerf, lDemotionPerf });
        }

        protected override OptionResult RunAll(List<IMiniSimDescription> sims)
        {
            Sims3.UI.SimpleMessageDialog.Show(Common.Localize ("JobsByLevel:Header"), GetDetails(sims));
            return OptionResult.SuccessRetain;
        }

        protected class TallyElement
        {
            public long mLevel;

            public long mPosCount;
            public long mNegCount;
            public long mZeroCount;

            public float mPosTally;
            public float mNegTally;

            public TallyElement(long vLevel)
            {
                mLevel = vLevel;
                mPosCount = 0;
                mNegCount = 0;
                mZeroCount = 0;
                mPosTally = 0f;
                mNegTally = 0f;
            }

            public void Add(float performance)
            {
                if (performance > 0f)
                {
                    mPosCount++;
                    mPosTally += performance;
                }
                else if (performance < 0f)
                {
                    mNegCount++;
                    mNegTally += performance;
                }
                else
                {
                    mZeroCount++;
                }
            }

            public override string ToString()
            {
                string msg = Common.Localize("JobsByLevel:Element1", false, new object[] { mLevel, (mPosCount + mNegCount + mZeroCount) });

                if (mPosCount > 0)
                {
                    msg += Common.Localize("JobsByLevel:Element2", false, new object[] { mPosCount, (mPosTally / mPosCount) });
                }

                if (mNegCount > 0)
                {
                    msg += Common.Localize("JobsByLevel:Element2", false, new object[] { mNegCount, (mNegTally / mNegCount) });
                }

                if (mZeroCount > 0)
                {
                    msg += Common.Localize("JobsByLevel:Element2", false, new object[] { mZeroCount, 0 });
                }

                return msg;
            }
        }
    }
}
