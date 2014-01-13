using NRaas.CommonSpace.Helpers;
using NRaas.StoryProgressionSpace.Managers;
using NRaas.StoryProgressionSpace.Scenarios.Careers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Objects.Counters;
using Sims3.Gameplay.Objects.Electronics;
using Sims3.Gameplay.Objects.Gardening;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Objects.HobbiesSkills.Inventing;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.GameEntry;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Helpers
{
    public class SkillDreamJob : DreamJob
    {
        static Dictionary<SkillNames, OccupationNames> sSkillCareers = null;

        static Dictionary<SkillNames, SkillDreamJob> sDreamSkillJobs = new Dictionary<SkillNames, SkillDreamJob>();

        List<Type> mRequiredObjects;

        static SkillDreamJob()
        {
            sDreamSkillJobs.Add(SkillNames.Writing, new WritingDreamJob());
            sDreamSkillJobs.Add(SkillNames.Sculpting, new SkillDreamJob(SkillNames.Sculpting, typeof(SculptingStation)));
            sDreamSkillJobs.Add(SkillNames.Inventing, new SkillDreamJob(SkillNames.Inventing, typeof(InventionWorkbench)));
            sDreamSkillJobs.Add(SkillNames.Painting, new SkillDreamJob(SkillNames.Painting, new Type[] { typeof(Easel), typeof(DraftingTable) }));
            sDreamSkillJobs.Add(SkillNames.Nectar, new SkillDreamJob(SkillNames.Nectar, typeof(NectarMaker)));
            sDreamSkillJobs.Add(SkillNames.Fishing, new SkillDreamJob(SkillNames.Fishing, typeof(IFishContainer)));
            sDreamSkillJobs.Add(SkillNames.Gardening, new SkillDreamJob(SkillNames.Gardening, typeof(HarvestPlant)));
            sDreamSkillJobs.Add(SkillNames.Guitar, new SkillDreamJob(SkillNames.Guitar));
            sDreamSkillJobs.Add(SkillNames.Drums, new SkillDreamJob(SkillNames.Drums));
            sDreamSkillJobs.Add(SkillNames.Piano, new SkillDreamJob(SkillNames.Piano));
            sDreamSkillJobs.Add(SkillNames.BassGuitar, new SkillDreamJob(SkillNames.BassGuitar));
            sDreamSkillJobs.Add(SkillNames.Photography, new SkillDreamJob(SkillNames.Photography));
            sDreamSkillJobs.Add(SkillNames.Athletic, new SkillDreamJob(SkillNames.Athletic, typeof(AthleticGameObject)));
            sDreamSkillJobs.Add(SkillNames.Chess, new SkillDreamJob(SkillNames.Chess, typeof(ChessTable)));
            sDreamSkillJobs.Add(SkillNames.Logic, new SkillDreamJob(SkillNames.Logic, new Type[] { typeof(ChessTable), typeof(Telescope) }));
            sDreamSkillJobs.Add(SkillNames.Handiness, new SkillDreamJob(SkillNames.Handiness));
            sDreamSkillJobs.Add(SkillNames.MartialArts, new SkillDreamJob(SkillNames.MartialArts, new Type[] { typeof(TrainingDummy), typeof(BoardBreaker) }));
            sDreamSkillJobs.Add(SkillNames.Riding, new SkillDreamJob(SkillNames.Riding, new Type[] { typeof(IBoxStall) }));
            sDreamSkillJobs.Add(SkillNames.RockBand, new BandDreamJob());
        }
        protected SkillDreamJob(SkillNames skill)
            : base (SkillToCareer(skill))
        { }
        protected SkillDreamJob(SkillNames skill, Type requiredObject)
            : this(skill)
        {
            if (requiredObject != null)
            {
                mRequiredObjects = new List<Type>();
                mRequiredObjects.Add(requiredObject);
            }
        }
        protected SkillDreamJob(SkillNames skill, Type[] requiredObjects)
            : this(skill)
        {
            mRequiredObjects = new List<Type>(requiredObjects);
        }

        public static OccupationNames SkillToCareer(SkillNames skill)
        {
            OccupationNames career;
            if (!SkillCareers.TryGetValue(skill, out career)) return OccupationNames.Undefined;

            return career;
        }

        public static SkillDreamJob Get(SkillNames skill)
        {
            SkillDreamJob dreamJob;
            if (!sDreamSkillJobs.TryGetValue(skill, out dreamJob)) return null;

            return dreamJob;
        }

        protected static Dictionary<SkillNames, OccupationNames> SkillCareers
        {
            get
            {
                if (sSkillCareers == null)
                {
                    sSkillCareers = new Dictionary<SkillNames, OccupationNames>();

                    foreach (Occupation occupation in CareerManager.sDictionary.Values)
                    {
                        SkillBasedCareer skillCareer = occupation as SkillBasedCareer;
                        if (skillCareer == null) continue;

                        SkillBasedCareerStaticData skillData = skillCareer.GetOccupationStaticDataForSkillBasedCareer();
                        if (skillData == null) continue;

                        if (sSkillCareers.ContainsKey(skillData.CorrespondingSkillName)) continue;

                        sSkillCareers.Add(skillData.CorrespondingSkillName, skillCareer.Guid);
                    }
                }
                return sSkillCareers;
            }
        }

        private bool Matches(Type objType)
        {
            foreach (Type type in mRequiredObjects)
            {
                if (type.IsAssignableFrom(objType)) return true;
            }

            return false;
        }

        protected override bool PrivateSatisfies(ManagerCareer manager, SimDescription sim, Lot newLot, bool inspecting)
        {
            if (!inspecting)
            {
                if (!manager.GetValue<ManagerCareer.AssignSelfEmployedOption, bool>()) return false;
            }

            if ((mRequiredObjects == null) || (mRequiredObjects.Count == 0)) return true;

            if (newLot != null)
            {
                foreach (GameObject obj in newLot.GetObjects<GameObject>())
                {
                    if (Matches(obj.GetType())) return true;
                }
            }

            return false;
        }

        public class BandDreamJob : SkillDreamJob
        {
            public BandDreamJob()
                : base(SkillNames.RockBand)
            { }

            protected override bool PrivateSatisfies(ManagerCareer manager, SimDescription sim, Lot newLot, bool inspecting)
            {
                if (base.PrivateSatisfies(manager, sim, newLot, inspecting)) return true;

                RockBand skill = sim.SkillManager.GetSkill<RockBand>(SkillNames.RockBand);
                if (skill != null)
                {
                    if (!skill.HasBandInfo()) return false;
                }

                return false;
            }
        }
        public class WritingDreamJob : SkillDreamJob
        {
            public WritingDreamJob()
                : base(SkillNames.Writing, typeof(Computer))
            { }

            protected override bool PrivateSatisfies(ManagerCareer manager, SimDescription sim, Lot newLot, bool inspecting)
            {
                if (base.PrivateSatisfies(manager, sim, newLot, inspecting)) return true;

                if (!inspecting)
                {
                    return (Inventories.InventoryFindAll<Computer>(sim).Count > 0);
                }
                else
                {
                    return false;
                }
            }
        }
    }
}

