using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
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
    public class SchoolByPerf : DemographicOption
    {
        public override string GetTitlePrefix()
        {
            return "SchoolByPerf";
        }

        protected string GetDetails(List<IMiniSimDescription> sims)
        {
            long lNoSchool = 0, lNoPerf = 0, lTotalSchool = 0;

            List<long> negPerfCount = new List<long>();
            List<long> posPerfCount = new List<long>();

            for (int perf = 0; perf < 10; perf++)
            {
                negPerfCount.Add(0);
                posPerfCount.Add(0);
            }

            foreach (IMiniSimDescription miniSim in sims)
            {
                SimDescription member = miniSim as SimDescription;
                if (member == null) continue;

                if (SimTypes.IsSpecial(member)) continue;

                if ((!member.Child) && (!member.Teen)) continue;

                if ((member.CareerManager == null) || (member.CareerManager.School == null))
                {
                    if (!member.IsPet)
                    {
                        lNoSchool++;
                    }
                }
                else
                {
                    Sims3.Gameplay.Careers.School school = member.CareerManager.School;
                    if (school.Performance == 0)
                    {
                        lNoPerf++;
                    }
                    else if (school.Performance > 0)
                    {
                        int iPerf = (int)Math.Ceiling(school.Performance / 10f);
                        if (iPerf > 9) iPerf = 9;
                        posPerfCount[iPerf]++;
                    }
                    else
                    {
                        int iPerf = (int)Math.Ceiling(-school.Performance / 10f);
                        if (iPerf > 9) iPerf = 9;
                        negPerfCount[iPerf]++;
                    }

                    lTotalSchool++;
                }
            }

            string posBody = null, negBody = null;

            for (int perf = negPerfCount.Count - 1; perf >= 1; perf--)
            {
                negBody += Common.Localize("SchoolByPerf:Element1", false, new object[] { (perf * 10) });
                if (negPerfCount[perf] == 0)
                {
                    negBody += "-";
                }
                else
                {
                    negBody += Common.Localize("SchoolByPerf:Element3", false, new object[] { negPerfCount[perf] });
                }
            }

            for (int perf = 1; perf < posPerfCount.Count; perf++)
            {
                posBody += Common.Localize("SchoolByPerf:Element2", false, new object[] { (perf * 10) });
                if (posPerfCount[perf] == 0)
                {
                    posBody += "-";
                }
                else
                {
                    posBody += Common.Localize("SchoolByPerf:Element3", false, new object[] { posPerfCount[perf] });
                }
            }

            return Common.Localize("SchoolByPerf:Body", false, new object[] { lNoSchool, negBody, lNoPerf, posBody, lTotalSchool });
        }

        protected override OptionResult RunAll(List<IMiniSimDescription> sims)
        {
            Sims3.UI.SimpleMessageDialog.Show(Common.Localize("SchoolByPerf:Header"), GetDetails(sims));
            return OptionResult.SuccessRetain;
        }
    }
}
