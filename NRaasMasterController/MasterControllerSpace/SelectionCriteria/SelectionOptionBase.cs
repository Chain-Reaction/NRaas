using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using NRaas.MasterControllerSpace.Sims;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.MapTags;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.SelectionCriteria
{
    public abstract class SelectionOptionBase : InteractionOptionItem<IMiniSimDescription, IMiniSimDescription, MiniSimDescriptionParameters>, ITestableOption, IPersistence
    {
        [Persistable(false)]
        protected bool mEnabled = false;

        protected bool mRandomCriteria = false;

        protected bool mRandomValue = false;

        public SelectionOptionBase()
            : base(null, 0)
        { }
        public SelectionOptionBase(string name, int count)
            : base(name, count)
        { }

        public string OptionName
        {
            get { return string.Empty; }
        }

        public string OptionValue
        {
            get { return string.Empty; }
        }

        public bool CanBeRandomValue
        {
            get { return false; }
            set { }
        }

        public int OptionHitValue
        {
            get { return 0; }
            set { }
        }

        public int OptionMissValue
        {
            get { return 0; }
            set { }
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

        public bool CanBeRandomCriteria
        {
            get
            {
                return mRandomCriteria;
            }
            set
            {
                mRandomCriteria = value;
            }
        }

        public bool CanHaveRandomValue
        {
            get
            {
                return mRandomValue;
            }
            set
            {
                mRandomValue = value;
            }
        }

        public virtual bool IsSpecial
        {
            get { return false; }
        }

        public virtual bool IsRandom
        {
            get { return false; }
        }

        public override string DisplayValue
        {
            get { return null; }
        }

        public override void Reset()
        {
            mEnabled = true;
            mRandomCriteria = false;

            base.Reset();
        }

        public List<ICommonOptionItem> GetOptions(IMiniSimDescription actor, IEnumerable<SimSelection.ICriteria> criteria, List<IMiniSimDescription> sims)
        {
            return null;
        }

        public void SetOptions(List<ICommonOptionItem> opts)
        {
        }

        public int GetScoreValue(IMiniSimDescription me, IMiniSimDescription actor, bool satisfies, int divisior)
        {
            return 0;
        }

        public bool Test(IMiniSimDescription me, bool fullFamily, IMiniSimDescription actor, bool testRandom)
        {
            return Test(me, fullFamily, actor);
        }

        public bool Test(IMiniSimDescription me, bool fullFamily, IMiniSimDescription actor)
        {
            if (fullFamily)
            {
                SimDescription trueSim = actor as SimDescription;
                if ((trueSim != null) && (trueSim.Household != null) && (!SimTypes.IsSpecial(trueSim)))
                {
                    foreach (SimDescription member in CommonSpace.Helpers.Households.All(trueSim.Household))
                    {
                        if (me is SimDescription)
                        {
                            if (Allow(me as SimDescription, member))
                            {
                                return true;
                            }
                        }
                        else if (me is MiniSimDescription)
                        {
                            if (Allow(me as MiniSimDescription, member))
                            {
                                return true;
                            }
                        }
                    }

                    return false;
                }
            }

            if (me is SimDescription)
            {
                return Allow(me as SimDescription, actor);
            }
            else if (me is MiniSimDescription)
            {
                return Allow(me as MiniSimDescription, actor);
            }
            return false;
        }

        protected abstract bool Allow(SimDescription me, IMiniSimDescription actor);

        protected virtual bool Allow(MiniSimDescription me, IMiniSimDescription actor)
        {
            return false;
        }

        public SimSelection.UpdateResult Update(IMiniSimDescription actor, IEnumerable<SimSelection.ICriteria> criteria, List<IMiniSimDescription> allSims, bool secondStage, bool silent, bool promptForMatchAll)
        {
            return Update(actor, criteria, allSims, secondStage);
        }

        public virtual SimSelection.UpdateResult Update(IMiniSimDescription actor, IEnumerable<SimSelection.ICriteria> criteria, List<IMiniSimDescription> allSims, bool secondStage)
        {
            if (secondStage) return SimSelection.UpdateResult.Success;

            bool fullFamily = false;
            foreach (SimSelection.ICriteria crit in criteria)
            {
                if (crit is FullFamily)
                {
                    fullFamily = true;
                    break;
                }
            }

            for (int i = allSims.Count - 1; i >= 0; i--)
            {
                if (!Test(allSims[i], fullFamily, actor))
                {
                    allSims.RemoveAt(i);
                }
            }

            return SimSelection.UpdateResult.Success;
        }

        protected override OptionResult Run(MiniSimDescriptionParameters parameters)
        {
            throw new NotImplementedException();
        }

        public static List<SimSelection.ICriteria> List
        {
            get
            {
                List<SimSelection.ICriteria> criteria = new List<SimSelection.ICriteria>();

                foreach (SimSelection.ICriteria crit in Common.DerivativeSearch.Find<SimSelection.ICriteria>())
                {
                    if (string.IsNullOrEmpty(crit.Name)) continue;

                    crit.Reset();

                    criteria.Add(crit);
                }

                return criteria;
            }
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
    }
}
