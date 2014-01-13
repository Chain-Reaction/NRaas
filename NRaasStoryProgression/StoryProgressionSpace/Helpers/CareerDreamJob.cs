using NRaas.StoryProgressionSpace.Careers;
using NRaas.StoryProgressionSpace.Managers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
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
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.GameEntry;
using System;
using System.Collections.Generic;

namespace NRaas.StoryProgressionSpace.Helpers
{
    public class CareerDreamJob : DreamJob
    {
        public CareerDreamJob(OccupationNames career)
            : base (career)
        { }

        protected override bool PrivateSatisfies(ManagerCareer manager, SimDescription sim, Lot newLot, bool inspecting)
        {
            if (inspecting) return true;
            
            if (sim.Occupation is Retired) return false;

            CareerLocation location = Career.FindClosestCareerLocation(sim, mCareer);
            if (location == null) return false;

            return true;
        }
    }

    public class AcademicDreamJob : DreamJob
    {
        public AcademicDreamJob()
            : base(OccupationNames.AcademicCareer)
        { }

        protected override bool PrivateSatisfies(ManagerCareer manager, SimDescription sim, Lot newLot, bool inspecting)
        {
            if (inspecting) return true;

            if (sim.Occupation is Retired) return false;

            return (Sims3.Gameplay.Queries.CountObjects<AdminstrationCenter>() > 0);
        }
    }

    public class FirefighterDreamJob : DreamJob
    {
        public FirefighterDreamJob()
            : base (OccupationNames.Firefighter)
        { }

        protected override bool PrivateSatisfies(ManagerCareer manager, SimDescription sim, Lot newLot, bool inspecting)
        {
            if (inspecting) return true;

            if (sim.Occupation is Retired) return false;

            if (sim.Elder) return false;

            return (ManagerSituation.FindLotType(CommercialLotSubType.kEP2_FireStation) != null);
        }
    }

    public class GhostHunterDreamJob : DreamJob
    {
        public GhostHunterDreamJob()
            : base(OccupationNames.GhostHunter)
        { }

        protected override bool PrivateSatisfies(ManagerCareer manager, SimDescription sim, Lot newLot, bool inspecting)
        {
            if (inspecting) return true;

            if (sim.Occupation is Retired) return false;

            return (ManagerSituation.FindRabbitHole(RabbitHoleType.ScienceLab) != null);
        }
    }

    public class PrivateEyeDreamJob : DreamJob
    {
        public PrivateEyeDreamJob()
            : base(OccupationNames.PrivateEye)
        { }

        protected override bool PrivateSatisfies(ManagerCareer manager, SimDescription sim, Lot newLot, bool inspecting)
        {
            if (inspecting) return true;

            return (ManagerSituation.FindRabbitHole(RabbitHoleType.PoliceStation) != null);
        }
    }

    public class StylistDreamJob : DreamJob
    {
        public StylistDreamJob()
            : base(OccupationNames.Stylist)
        { }

        protected override bool PrivateSatisfies(ManagerCareer manager, SimDescription sim, Lot newLot, bool inspecting)
        {
            if (!inspecting)
            {
                if (sim.Occupation is Retired) return false;

                if (ManagerSituation.FindLotType(CommercialLotSubType.kEP2_Salon) == null) return false;
            }

            if ((newLot == null) || (newLot.CountObjects<DraftingTable>() == 0)) return false;

            return true;
        }
    }

    public class InteriorDesignerDreamJob : DreamJob
    {
        public InteriorDesignerDreamJob()
            : base(OccupationNames.InteriorDesigner)
        { }

        protected override bool PrivateSatisfies(ManagerCareer manager, SimDescription sim, Lot newLot, bool inspecting)
        {
            if (!inspecting)
            {
                if (sim.Occupation is Retired) return false;

                if (ManagerSituation.FindRabbitHole(RabbitHoleType.CityHall) == null) return false;
            }

            if ((newLot == null) || (newLot.CountObjects<DraftingTable>() == 0)) return false;

            return true;
        }
    }
}

