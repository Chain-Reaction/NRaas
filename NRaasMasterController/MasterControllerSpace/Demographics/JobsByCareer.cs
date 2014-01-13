using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Demographics
{
    public class JobsByCareer : DemographicOption
    {
        public override string GetTitlePrefix()
        {
 	        return "JobsByCareer";
        }

        protected class Item : ValueSettingOption<OccupationNames>
        {
            public List<CareerElement> mSims = new List<CareerElement>();

            public Item(OccupationNames career)
                : base(career, career.ToString(), 0)
            {
                Occupation job = CareerManager.GetStaticOccupation(career);
                if (job!= null)
                {
                    mName = job.CareerName;

                    SetThumbnail(job.CareerIconColored, ProductVersion.BaseGame);
                }
            }
        }

        protected class CareerElement
        {
            public int mLevel;
            public SimDescription mSim;
            public SimDescription mBoss;
            public float mPerf;

            public CareerElement(int level, SimDescription sim, SimDescription boss, float perf)
            {
                mLevel = level;
                mSim = sim;
                mBoss = boss;
                mPerf = perf;
            }

            protected static string FullName (SimDescription sim)
            {
                if (sim == null) return null;

                return sim.FirstName + " " + sim.LastName;
            }

            public class CompareByLevel : IComparer<CareerElement>
            {
                public int Compare(CareerElement x, CareerElement y)
                {
                    if (x.mLevel < y.mLevel)
                    {
                        return -1;
                    }
                    else if (x.mLevel > y.mLevel)
                    {
                        return 1;
                    }
                    else if (FullName(x.mSim).CompareTo(FullName(y.mSim)) < 0)
                    {
                        return -1;
                    }
                    else
                    {
                        return 1;
                    }
                }
            }
        }

        protected override OptionResult RunAll(List<IMiniSimDescription> sims)
        {
            Dictionary<OccupationNames, Item> lookup = new Dictionary<OccupationNames, Item>();

            foreach (IMiniSimDescription miniSim in sims)
            {
                SimDescription member = miniSim as SimDescription;
                if (member == null) continue;

                if (member.Occupation == null) continue;

                Item item;
                if (!lookup.TryGetValue(member.Occupation.Guid, out item))
                {
                    item = new Item(member.Occupation.Guid);
                    lookup.Add(member.Occupation.Guid, item);
                }

                item.IncCount();
                item.mSims.Add(new CareerElement(member.Occupation.CareerLevel, member, member.Occupation.Boss, StatusJobPerformance.GetPerformance(member)));
            }

            Item choice = new CommonSelection<Item>(Name, lookup.Values).SelectSingle();
            if (choice == null) return OptionResult.Failure;

            List<CareerElement> list = choice.mSims;

            list.Sort(new CareerElement.CompareByLevel());

            string msg = null;

            foreach (CareerElement element in list)
            {
                if (msg != null)
                {
                    msg += Common.NewLine;
                }
                msg += Common.Localize ("JobsByCareer:Employee", element.mSim.IsFemale, new object[] { element.mLevel, element.mSim, element.mPerf } );

                if (element.mBoss != null)
                {
                    msg += Common.NewLine + Common.Localize("JobsByCareer:Boss", element.mBoss.IsFemale, new object[] { element.mBoss });
                }
            }

            string careerName = null;

            Occupation career = CareerManager.GetStaticOccupation(choice.Value);
            if (career != null)
            {
                careerName = career.CareerName;
            }

            Sims3.UI.SimpleMessageDialog.Show(careerName, msg);
            return OptionResult.SuccessRetain;
        }
    }
}
