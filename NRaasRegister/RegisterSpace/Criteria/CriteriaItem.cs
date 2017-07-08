using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Register;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.RegisterSpace.Criteria
{
    public abstract class CriteriaItem : CommonOptionItem, IRoleCriteria
    {
        bool mEnabled;

        protected abstract string GetTitleSuffix();

        public override string Name
        {
            get
            {
                return Common.Localize("Criteria:" + GetTitleSuffix());
            }
        }

        public bool Enabled
        {
            get
            {
                return mEnabled;
            }
            set
            {
                mEnabled = value;
            }
        }

        public bool IsSpecial
        {
            get { return false; }
        }

        public bool IsRandom
        {
            get { return false; }
        }

        public bool CanBeRandomCriteria
        {
            get { return false; }
            set { }
        }

        public bool CanHaveRandomValue
        {
            get { return false; }
            set { }
        }

        public bool AllowCriteria()
        {
            return true;
        }

        public abstract bool Test(SimDescription sim, SimDescription me);

        public ProtoSimSelection<SimDescription>.UpdateResult Update(SimDescription sim, IEnumerable<ProtoSimSelection<SimDescription>.ICriteria> criteria, List<SimDescription> sims, bool secondStage, bool silent, bool promptForMatchAll)
        {
            return Update(sim, criteria, sims, secondStage);
        }

        public ProtoSimSelection<SimDescription>.UpdateResult Update(SimDescription sim, IEnumerable<ProtoSimSelection<SimDescription>.ICriteria> criteria, List<SimDescription> sims, bool secondStage)
        {
            for (int i = sims.Count - 1; i >= 0; i--)
            {
                if (Test(sims[i], sim)) continue;

                sims.RemoveAt(i);
            }
            return ProtoSimSelection<SimDescription>.UpdateResult.Success;
        }

        public static bool HasRealJob(SimDescription sim)
        {
            if (sim == null) return false;

            if (sim.Occupation == null) return false;

            Career career = sim.Occupation as Career;
            if ((career == null) || (career.CareerLevel != 1) || (career.CurLevel == null) || (career.CurLevel.DayLength != 0))
            {
                return true;
            }

            return false;
        }

        public void Export(Persistence.Lookup settings)
        {
            settings.ReflectionAdd(this);
        }

        public void Import(Persistence.Lookup settings)
        {
            settings.ReflectionGet(this);
        }

        public string PersistencePrefix
        {
            get { return null; }
        }

        public override string DisplayValue
        {
            get { return null; }
        }

        public List<ICommonOptionItem> GetOptions(SimDescription sim, IEnumerable<ProtoSimSelection<SimDescription>.ICriteria> criteria, List<SimDescription> sims)
        {
            return null;
        }

        public void SetOptions(List<ICommonOptionItem> opts)
        {
        }
    }
}