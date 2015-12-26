using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Options;
using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Controller;
using System;
using System.Collections.Generic;

namespace NRaas.CommonSpace.Selection
{
    public abstract class ProtoSimSelection<T> : ProtoSelection<T>
            where T : class, IMiniSimDescription
    {
        bool mLastFirst;

        bool mAddAll;

        T mMe;

        protected ProtoSimSelection(string title, T me, bool lastFirst, bool addAll)
            : this(title, me, null, lastFirst, addAll)
        { }
        protected ProtoSimSelection(string title, string subTitle, T me, bool lastFirst, bool addAll)
            : this(title, subTitle, me, null, lastFirst, addAll)
        { }
        protected ProtoSimSelection(string title, T me, ICollection<T> sims, bool lastFirst, bool addAll)
            : this(title, null, me, sims, lastFirst, addAll)
        { }
        protected ProtoSimSelection(string title, string subTitle, T me, ICollection<T> sims, bool lastFirst, bool addAll)
            : base(title, subTitle, sims)
        {
            if (addAll)
            {
                AllOnNull = true;
            }

            mLastFirst = lastFirst;
            mAddAll = addAll;
            mMe = me;

            if ((mMe == null) && (Sim.ActiveActor != null))
            {
                mMe = Sim.ActiveActor.SimDescription as T;
            }

            AddColumn(new SimNameColumn());
            AddColumn(new SimRelationshipColumn());
        }

        public bool IsEmpty
        {
            get { return ((mItems == null) || (mItems.Count == 0) || (!mHadResults)); }
        }

        protected T Me
        {
            get { return mMe; }
        }

        public class CriteriaSelection : ProtoSelection<ICriteria>
        {
            string mNamespace = string.Empty;

            public CriteriaSelection(string title, ICollection<ICriteria> criteria)
                : base(Common.Localize("SimSelection:CriteriaTitle"), title, criteria)
            {
                AddColumn(new NameColumn());                
            }
            public CriteriaSelection(string title, ICollection<ICriteria> criteria, string callingMod)
                : base(Common.Localize("SimSelection:CriteriaTitle"), title, criteria)
            {
                AddColumn(new NameColumn());
                mNamespace = callingMod;
            }

            protected override bool AllowRow(ICriteria item)
            {                
                // remove filters set for mods from MC listing
                if (mNamespace == string.Empty && item.Name.StartsWith("nraas") && !Common.kDebugging) return false;

                // remove filters from mods that didn't create them
                if (((item.Name.StartsWith("nraas") && !item.Name.Contains(mNamespace)) || (mNamespace != string.Empty && !item.Name.StartsWith("nraas") && item.GetType().ToString().Contains("SavedFilter"))) && !Common.kDebugging) return false;                

                return item.AllowCriteria();
            }

            public class NameColumn : ObjectPickerDialogEx.CommonHeaderInfo<ICriteria>
            {
                public NameColumn()
                    : base(VersionStamp.sNamespace + ".SimSelection:CriteriaHeader", VersionStamp.sNamespace + ".SimSelection:CriteriaTooltip", 370)
                { }

                public override ObjectPicker.ColumnInfo GetValue(ICriteria item)
                {
                    return new ObjectPicker.TextColumn(item.Name);
                }
            }
        }

        protected virtual bool Allow(T item)
        {
            return true;
        }

        protected virtual IEnumerable<ICriteria> AlterCriteria(IEnumerable<ICriteria> criteria, bool manual, bool canceled)
        {
            return criteria;
        }

        public void FilterSims(ICollection<ICriteria> fullCriteria, IEnumerable<ICriteria> savedFilters, bool automatic, out bool canceled)
        {
            bool showSpecial = false;

            IEnumerable<ICriteria> criteria = null;

            canceled = false;

            bool manual = false;
            if (fullCriteria != null)
            {
                if (fullCriteria.Count > 1)
                {
                    List<ICriteria> allCriteria = new List<ICriteria>(fullCriteria);

                    if (savedFilters != null)
                    {
                        allCriteria.AddRange(savedFilters);
                    }

                    if (!automatic)
                    {
                        CriteriaSelection.Results results = new CriteriaSelection(Title, allCriteria).SelectMultiple();

                        criteria = results;

                        canceled = !results.mOkayed;
                    }
                    else
                    {
                        criteria = allCriteria;
                    }

                    manual = true;
                }
                else
                {
                    criteria = new List<ICriteria>(fullCriteria);
                }
            }
            else
            {
                showSpecial = true;
            }

            if (criteria == null)
            {
                criteria = new List<ICriteria>();
            }

            criteria = AlterCriteria(criteria, manual, canceled);

            foreach (ICriteria crit in criteria)
            {
                if (crit.IsSpecial)
                {
                    showSpecial = true;
                }
            }

            List<T> chosen = new List<T>();
            foreach (List<T> sims in SimListing.AllSims(mMe, showSpecial).Values)
            {
                foreach (T sim in sims)
                {
                    if (!showSpecial)
                    {
                        if (IsSpecial(sim))
                        {
                            continue;
                        }
                    }

                    if (!Allow(sim))
                    {
                        continue;
                    }

                    chosen.Add(sim);
                }
            }

            List<ICriteria> delayed = new List<ICriteria>();

            foreach (ICriteria crit in criteria)
            {
                if (crit.Update(mMe, criteria, chosen, false) == UpdateResult.Delay)
                {
                    delayed.Add(crit);
                }
            }

            foreach (ICriteria crit in delayed)
            {
                crit.Update(mMe, criteria, chosen, true);
            }

            if (chosen.Count == 0)
            {
                mItems = null;
            }
            else
            {
                mItems = chosen;
            }
        }

        protected virtual void GetName(T sim, out string firstName, out string lastName)
        {
            string suffix = null;
            /*
            if (sim is MiniSimDescription)
            {
                suffix = " (M)";
            }
            */

            if ((sim.FullName == null) || (string.IsNullOrEmpty(sim.FullName.Trim())))
            {
                Genealogy genealogy = sim.CASGenealogy as Genealogy;
                if ((genealogy != null) && (!string.IsNullOrEmpty(genealogy.Name)))
                {
                    firstName = "(" + genealogy.Name + ")";
                    lastName = "";
                }
                else
                {
                    firstName = "(" + sim.SimDescriptionId.ToString() + ")";
                    lastName = "";
                }
            }
            else if (mLastFirst)
            {
                if (!string.IsNullOrEmpty(sim.LastName))
                {
                    firstName = sim.LastName + ",";
                }
                else
                {
                    firstName = "";
                }

                lastName = sim.FirstName + suffix;
            }
            else
            {
                firstName = sim.FirstName;
                lastName = sim.LastName + suffix;
            }
        }

        public List<PhoneSimPicker.SimPickerInfo> GetPickerInfo()
        {
            List<PhoneSimPicker.SimPickerInfo> sims = new List<PhoneSimPicker.SimPickerInfo>();

            if (mAddAll)
            {
                PhoneSimPicker.SimPickerInfo all = new PhoneSimPicker.SimPickerInfo();

                if (mLastFirst)
                {
                    all.FirstName = "";
                    all.LastName = "(" + Common.LocalizeEAString("Ui/Caption/ObjectPicker:All") + ")";
                }
                else
                {
                    all.FirstName = "(" + Common.LocalizeEAString("Ui/Caption/ObjectPicker:All") + ")";
                    all.LastName = "";
                }

                all.CoWorker = false;
                all.Thumbnail = new ThumbnailKey(ResourceKey.CreatePNGKey("shop_all_r2", ResourceUtils.ProductVersionToGroupId(ProductVersion.BaseGame)), ThumbnailSize.Large);
                all.RelationShip = 0f;
                all.LtrType = LongTermRelationshipTypes.Stranger;
                all.Friend = false;
                all.RelationshipText = "";
                all.RelationshipWithFirstName = mMe.FirstName;

                sims.Add(all);
            }

            foreach (T sim in All)
            {
                if (!AllowRow(sim)) continue;

                PhoneSimPicker.SimPickerInfo item = new PhoneSimPicker.SimPickerInfo();

                string firstName = null;
                string lastName = null;
                GetName (sim, out firstName, out lastName);

                item.FirstName = firstName;
                item.LastName = lastName;
                item.CoWorker = false;

                IMiniRelationship relation = null;

                if (sim is SimDescription)
                {
                    SimDescription simDesc = sim as SimDescription;

                    if (simDesc.GetOutfit(OutfitCategories.Everyday, 0x0) != null)
                    {
                        try
                        {
                            item.Thumbnail = sim.GetThumbnailKey(ThumbnailSize.Large, 0);
                        }
                        catch
                        { }
                    }

                    if (mMe is SimDescription)
                    {
                        SimDescription meDesc = mMe as SimDescription;

                        relation = Relationship.Get(simDesc, meDesc, false);

                        if ((meDesc.CareerManager != null) && (meDesc.CareerManager.Occupation != null))
                        {
                            if (sim == meDesc.CareerManager.Occupation.Boss)
                            {
                                item.CoWorker = true;
                            }
                            else if (meDesc.CareerManager.Occupation.Coworkers != null)
                            {
                                foreach (SimDescription description2 in meDesc.CareerManager.Occupation.Coworkers)
                                {
                                    if (description2 == sim)
                                    {
                                        item.CoWorker = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        relation = mMe.GetMiniRelationship(simDesc);
                    }
                }
                else
                {
                    try
                    {
                        item.Thumbnail = sim.GetThumbnailKey(ThumbnailSize.Large, 0);
                    }
                    catch
                    { }

                    relation = sim.GetMiniRelationship(mMe);
                }

                item.SimDescription = sim;

                if (sim == mMe)
                {
                    item.RelationShip = 100f;
                    item.LtrType = LongTermRelationshipTypes.BestFriend;
                    item.Friend = true;
                }
                else if (relation != null)
                {
                    item.RelationShip = relation.CurrentLTRLiking;
                    item.LtrType = relation.CurrentLTR;
                    item.Friend = relation.AreFriends() || relation.AreRomantic();
                }
                else
                {
                    item.RelationShip = 0f;
                    item.LtrType = LongTermRelationshipTypes.Stranger;
                    item.Friend = false;
                }

                item.RelationshipText = LTRData.Get(item.LtrType).GetName(mMe, sim);
                item.RelationshipWithFirstName = mMe.FirstName;

                sims.Add(item);
            }

            return sims;
        }

        protected override List<ObjectPicker.TabInfo> GetTabInfo(out List<ObjectPicker.RowInfo> preSelectedRows)
        {
            return PhoneSimPicker.MassageData(GetPickerInfo(), false, out preSelectedRows);
        }

        public static bool IsSpecial(IMiniSimDescription sim)
        {
            if ((sim.IsDead) && (!sim.IsPlayableGhost)) return true;

            if (sim is MiniSimDescription)
            {
                return true;
            }

            return false;
        }

        public class SimNameColumn : ObjectPickerDialogEx.CommonHeaderInfo<T>
        {
            public SimNameColumn()
                : base("Ui/Caption/ObjectPicker:Sim", "Ui/Tooltip/ObjectPicker:FirstName", 370)
            { }

            public override bool IsStub
            {
                get { return true; }
            }

            public override ObjectPicker.ColumnInfo GetValue(T item)
            {
                return null;
            }
        }

        public class SimRelationshipColumn : ObjectPickerDialogEx.CommonHeaderInfo<T>
        {
            public SimRelationshipColumn()
                : base("glb_i_relationship_sort_r2", "Ui/Tooltip/ObjectPicker:Relationship", 40, true)
            { }

            public override bool IsStub
            {
                get { return true; }
            }

            public override ObjectPicker.ColumnInfo GetValue(T item)
            {
                return null;
            }
        }

        public enum UpdateResult
        {
            Delay,
            Failure,
            Success,
        }

        [Persistable]
        public interface ICriteria : IPersistence
        {
            string Name
            {
                get;
            }

            bool IsSpecial
            {
                get;
            }

            void Reset();

            ICommonOptionItem Clone();

            bool AllowCriteria();

            UpdateResult Update(T sim, IEnumerable<ICriteria> criteria, List<T> sims, bool secondStage);
        }
    }
}

