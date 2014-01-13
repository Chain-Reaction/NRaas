using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Selection;
using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.ChildAndTeenUpdates;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Rewards;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Intermediate.Careers
{
    public class ChangeAlmaMater : SimFromList, ICareerOption
    {
        AlmaMater mAlmaMater = AlmaMater.None;
        string mAlmaMaterName = null;

        public override string GetTitlePrefix()
        {
            return "AlmaMater";
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
            if (me.TeenOrBelow) return false;

            return base.PrivateAllow(me);
        }

        protected static void Add(List<AlmaMaterCriteria.Item> allOptions, string name, AlmaMater mater, SimDescription me, Dictionary<string,bool> lookup)
        {
            if (lookup.ContainsKey(name)) return;
            lookup.Add(name, true);

            int value = 0;
            if (name == me.AlmaMaterName)
            {
                value = 1;
            }

            allOptions.Add(new AlmaMaterCriteria.Item(mater, name, value));
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            if (!ApplyAll)
            {
                Dictionary<string, bool> lookup = new Dictionary<string, bool>();

                List<AlmaMaterCriteria.Item> allOptions = new List<AlmaMaterCriteria.Item>();

                if (BoardingSchool.BoardingSchoolData.sBoardingSchoolDataList != null)
                {
                    foreach (BoardingSchool.BoardingSchoolData data in BoardingSchool.BoardingSchoolData.sBoardingSchoolDataList.Values)
                    {
                        Add(allOptions, Common.LocalizeEAString(data.SchoolNameKey), data.AlmaMaterType, me, lookup);
                    }
                }

                Add(allOptions, Common.LocalizeEAString("Gameplay/Careers/School:Community"), AlmaMater.Community, me, lookup);

                foreach (RabbitHole hole in Sims3.Gameplay.Queries.GetObjects<RabbitHole>())
                {
                    foreach (CareerLocation loc in hole.CareerLocations.Values)
                    {
                        if (loc.Career is School)
                        {
                            Add(allOptions, hole.CatalogName, AlmaMater.Community, me, lookup);
                        }
                    }
                }

                AlmaMaterCriteria.Item choice = new CommonSelection<AlmaMaterCriteria.Item>(Name, me.FullName, allOptions).SelectSingle();
                if (choice == null) return false;

                mAlmaMater = choice.Value.mAlmaMater;
                mAlmaMaterName = choice.Name;
            }

            me.AlmaMater = mAlmaMater;
            me.AlmaMaterName = mAlmaMaterName;

            switch (me.GraduationType)
            {
                case GraduationType.None:
                case GraduationType.NoSchool:
                    me.GraduationType = GraduationType.Graduate;
                    break;
            }

            if (me.BoardingSchool != null)
            {
                me.BoardingSchool.SetGraduatedSchoolType(BoardingSchool.BoardingSchoolTypes.None, false);
                me.BoardingSchool.CurrentSchoolType = BoardingSchool.BoardingSchoolTypes.None;
            }

            if (BoardingSchool.BoardingSchoolData.sBoardingSchoolDataList != null)
            {
                foreach (BoardingSchool.BoardingSchoolData data in BoardingSchool.BoardingSchoolData.sBoardingSchoolDataList.Values)
                {
                    if (me.AlmaMater == data.AlmaMaterType)
                    {
                        if (me.BoardingSchool == null)
                        {
                            me.BoardingSchool = new BoardingSchool.BoardingSchoolInfo(me, BoardingSchool.BoardingSchoolTypes.None);
                        }

                        me.BoardingSchool.SetGraduatedSchoolType(data.SchoolType, false);
                        break;
                    }
                }
            }

            if (me.CreatedSim != null)
            {
                foreach (Diploma diploma in Inventories.QuickFind<Diploma>(me.CreatedSim.Inventory))
                {
                    diploma.mSchoolName = me.AlmaMaterName;
                }
            }

            return true;
        }
    }
}
