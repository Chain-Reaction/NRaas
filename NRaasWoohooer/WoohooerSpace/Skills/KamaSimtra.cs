using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Scoring;
using NRaas.CommonSpace.ScoringMethods;
using NRaas.CommonSpace.Skills;
using NRaas.WoohooerSpace.Helpers;
using NRaas.WoohooerSpace.Interactions;
using NRaas.WoohooerSpace.Options.Romance.RoleTypes;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Objects.Beds;
using Sims3.Gameplay.Objects.Entertainment;
using Sims3.Gameplay.Objects.Environment;
using Sims3.Gameplay.Objects.Elevator;
using Sims3.Gameplay.Objects.HobbiesSkills.Inventing;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Objects.Plumbing;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Objects.ShelvesStorage;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.WoohooerSpace.Skills
{
    [Persistable]
    public class KamaSimtra : CommonSkill<KamaSimtra,KamaSimtra.KamaSimtraMajorStat,KamaSimtra.KamaSimtraMinorStat,KamaSimtra.KamaSimtraOpportunity>, Common.IPreLoad, Common.IWorldLoadFinished
    {
        public enum OccultTypesEx : int
        {
            None = 0,
            Vampire,
            Mummy,
            Frankenstein,
            GrimReaper,
            Ghost,
            ImaginaryFriend,
            Unicorn,
            Genie,
            Werewolf,
            Fairy,
            Zombie,
            Witch,
            Bonehilda,
            Alien,
            PlantSim,
            Mermaid,
            Plumbot,
            TimeTraveler,
        }

        static Dictionary<ulong, float> sLastNotch = new Dictionary<ulong, float>();
        
        [PersistableStatic]
        static KamaSimtraSettings sSettings = null;

        static List<OccultTypesEx> sOccultTypes = new List<OccultTypesEx>();

        static List<SimDescription.DeathType> sDeathTypes = new List<SimDescription.DeathType>();

        static BuffNames sOrgasmic = unchecked((BuffNames)ResourceUtils.HashString64("NRaasPositivelyOrgasmic"));
        static BuffNames sGoodBuff = unchecked((BuffNames)ResourceUtils.HashString64("NRaasGoodWoohoo"));
        static BuffNames sBadBuff = unchecked((BuffNames)ResourceUtils.HashString64("NRaasBadWoohoo"));

        static BuffNames sLikeProfessional = unchecked((BuffNames)ResourceUtils.HashString64("NRaasLikeProfessional"));
        static BuffNames sDislikeProfessional = unchecked((BuffNames)ResourceUtils.HashString64("NRaasDislikeProfessional"));

        public static BuffNames sLikeCyberWoohoo = unchecked((BuffNames)ResourceUtils.HashString64("NRaasLikeCyberWoohoo"));
        public static BuffNames sDislikeCyberWoohoo = unchecked((BuffNames)ResourceUtils.HashString64("NRaasDislikeCyberWoohoo"));
        public static BuffNames sPremature = unchecked((BuffNames)ResourceUtils.HashString64("NRaasPremature"));

        static Origin sGoodOrigin = unchecked((Origin)ResourceUtils.HashString64("NRaasGoodWoohoo"));
        public static Origin sBadOrigin = unchecked((Origin)ResourceUtils.HashString64("NRaasBadWoohoo"));
        static Origin sKamaSimtraOrigin = unchecked((Origin)ResourceUtils.HashString64("NRaasKamaSimtra"));

        [Persistable(false)]
        Stats mTotal = new Stats();

        Stats mAffair = new Stats();
        Stats mCheats = new Stats();
        Stats mFidelity = new Stats();

        Dictionary<ulong, Stats> mSims = new Dictionary<ulong, Stats>();

        Dictionary<SimDescription.DeathType, Stats> mGhosts = new Dictionary<SimDescription.DeathType, Stats>();

        Dictionary<OccultTypesEx, Stats> mOccult = new Dictionary<OccultTypesEx, Stats>();

        Dictionary<ServiceType, Stats> mService = new Dictionary<ServiceType, Stats>();

        Dictionary<Role.RoleType, Stats> mRoles = new Dictionary<Role.RoleType, Stats>();

        Dictionary<uint, Stats> mCelebrities = new Dictionary<uint, Stats>();

        Dictionary<WorldName, Stats> mForeign = new Dictionary<WorldName, Stats>();

        Stats mCasual = new Stats();
        Stats mPublic = new Stats();
        Stats mExhibitionist = new Stats();
        Stats mWhoring = new Stats();

        Stats mOlder = new Stats();
        Stats mYounger = new Stats();

        Stats mPositive = new Stats();
        Stats mNegative = new Stats();

        bool mWhoringActive;
        bool mRendezvousActive;

        static KamaSimtra()
        {
            foreach (OccultTypesEx type in Enum.GetValues(typeof(OccultTypesEx)))
            {
                sOccultTypes.Add(type);
            }

            foreach (SimDescription.DeathType type in Enum.GetValues(typeof(SimDescription.DeathType)))
            {
                if (type == SimDescription.DeathType.None) continue;

                sDeathTypes.Add(type);
            }
        }
        public KamaSimtra()
        { }
        public KamaSimtra(SkillNames guid)
            : base(guid)
        { }

        public static KamaSimtraSettings Settings
        {
            get
            {
                if (sSettings == null)
                {
                    sSettings = new KamaSimtraSettings();
                }

                return sSettings;
            }
        }

        public static void ResetSettings()
        {
            sSettings = null;
        }

        public bool WhoringActive
        {
            get { return mWhoringActive; }
            set { mWhoringActive = value; }
        }

        public bool RendezvousActive
        {
            get { return mRendezvousActive; }
            set { mRendezvousActive = value; }
        }

        protected override string LocalizationKey
        {
            get { return "NRaasKamaSimtra"; }
        }

        public override bool OwnerCanAcquireSkill()
        {
            return SkillOwner.TeenOrAbove;
        }

        public override bool ExportContent(IPropertyStreamWriter writer)
        {
            try
            {
                uint skillHash = GetSkillHash() + 1;

                writer.WriteBool(skillHash, mWhoringActive);
                skillHash++;

                mAffair.Export(skillHash, writer);
                skillHash++;

                mCheats.Export(skillHash, writer);
                skillHash++;

                mCasual.Export(skillHash, writer);
                skillHash++;

                mPublic.Export(skillHash, writer);
                skillHash++;

                mExhibitionist.Export(skillHash, writer);
                skillHash++;

                mWhoring.Export(skillHash, writer);
                skillHash++;

                mOlder.Export(skillHash, writer);
                skillHash++;

                mYounger.Export(skillHash, writer);
                skillHash++;

                mPositive.Export(skillHash, writer);
                skillHash++;

                mNegative.Export(skillHash, writer);
                skillHash++;

                Export<OccultTypesEx>(skillHash, mOccult, writer, ConvertOccultTypesEx);
                skillHash += 100;

                Export<ServiceType>(skillHash, mService, writer, ConvertServiceType);
                skillHash += 100;

                Export<Role.RoleType>(skillHash, mRoles, writer, ConvertRoleType);
                skillHash += 100;

                Export<uint>(skillHash, mCelebrities, writer, ConvertUint);
                skillHash += 100;

                Export<WorldName>(skillHash, mForeign, writer, ConvertWorldName);
                skillHash += 100;

                ExportSims(skillHash, mSims, writer);
                skillHash += 10000;

                writer.WriteBool(skillHash, mRendezvousActive);
                skillHash++;

                mFidelity.Export(skillHash, writer);
                skillHash++;

                Export<SimDescription.DeathType>(skillHash, mGhosts, writer, ConvertDeathType);
                skillHash += 100;

                return base.ExportContent(writer);
            }
            catch (Exception e)
            {
                Common.Exception("ExportContent", e);
                return false;
            }
        }

        protected static uint ConvertDeathType(SimDescription.DeathType value)
        {
            return (uint)value;
        }
        protected static uint ConvertOccultTypesEx(OccultTypesEx value)
        {
            return (uint)value;
        }
        protected static uint ConvertServiceType(ServiceType value)
        {
            return (uint)value;
        }
        protected static uint ConvertRoleType(Role.RoleType value)
        {
            return (uint)value;
        }
        protected static uint ConvertWorldName(WorldName value)
        {
            return (uint)value;
        }

        protected delegate uint ConvertTo<T>(T value);

        protected void Export<T>(uint key, Dictionary<T, Stats> stats, IPropertyStreamWriter writer, ConvertTo<T> converter)
        {
            int count = 0;
            foreach(KeyValuePair<T, Stats> stat in stats)
            {
                stat.Value.Export(key, writer);
                key++;

                writer.WriteUint32(key, converter(stat.Key));
                key++;

                count++;
                if (count > 100) break;
            }
        }

        protected void ExportSims(uint key, Dictionary<ulong, Stats> stats, IPropertyStreamWriter writer)
        {
            int count = 0;
            foreach (KeyValuePair<ulong, Stats> stat in stats)
            {
                stat.Value.Export(key, writer);
                key++;

                writer.WriteUint64(key, stat.Key);
                key++;

                count++;
                if (count > 10000) break;
            }
        }

        public override bool ImportContent(IPropertyStreamReader reader)
        {
            try
            {
                uint skillHash = GetSkillHash() + 1;

                reader.ReadBool(skillHash, out mWhoringActive, false);
                skillHash++;

                mAffair.Import(skillHash, reader);
                skillHash++;

                mCheats.Import(skillHash, reader);
                skillHash++;

                mCasual.Import(skillHash, reader);
                skillHash++;

                mPublic.Import(skillHash, reader);
                skillHash++;

                mExhibitionist.Import(skillHash, reader);
                skillHash++;

                mWhoring.Import(skillHash, reader);
                skillHash++;

                mOlder.Import(skillHash, reader);
                skillHash++;

                mYounger.Import(skillHash, reader);
                skillHash++;

                mPositive.Import(skillHash, reader);
                skillHash++;

                mNegative.Import(skillHash, reader);
                skillHash++;

                Import<OccultTypesEx>(skillHash, mOccult, reader, ConvertOccultTypesEx);
                skillHash += 100;

                Import<ServiceType>(skillHash, mService, reader, ConvertServiceType);
                skillHash += 100;

                Import<Role.RoleType>(skillHash, mRoles, reader, ConvertRoleType);
                skillHash += 100;

                Import<uint>(skillHash, mCelebrities, reader, ConvertUint);
                skillHash += 100;

                Import<WorldName>(skillHash, mForeign, reader, ConvertWorldName);
                skillHash += 100;

                ImportSims(skillHash, mSims, reader);
                skillHash += 10000;

                reader.ReadBool(skillHash, out mRendezvousActive, false);
                skillHash++;

                mFidelity.Import(skillHash, reader);
                skillHash++;

                Import<SimDescription.DeathType>(skillHash, mGhosts, reader, ConvertDeathType);
                skillHash += 100;
               
                return base.ImportContent(reader);
            }
            catch (Exception e)
            {
                Common.Exception("ImportContent", e);
                return false;
            }
        }

        protected static SimDescription.DeathType ConvertDeathType(uint value)
        {
            return (SimDescription.DeathType)value;
        }
        protected static OccultTypesEx ConvertOccultTypesEx(uint value)
        {
            return (OccultTypesEx)value;
        }
        protected static ServiceType ConvertServiceType(uint value)
        {
            return (ServiceType)value;
        }
        protected static Role.RoleType ConvertRoleType(uint value)
        {
            return (Role.RoleType)value;
        }
        protected static uint ConvertUint(uint value)
        {
            return value;
        }
        protected static WorldName ConvertWorldName(uint value)
        {
            return (WorldName)value;
        }

        protected delegate T ConvertFrom<T>(uint value);

        protected bool Import<T>(uint key, Dictionary<T, Stats> results, IPropertyStreamReader reader, ConvertFrom<T> converter)
        {
            Stats stats = null;

            while (true)
            {
                stats = new Stats();
                if (!stats.Import(key, reader)) break;
                key++;

                uint id = 0;
                reader.ReadUint32(key, out id, 0);
                key++;

                results[converter (id)] = stats;
            }

            return true;
        }

        protected bool ImportSims(uint key, Dictionary<ulong, Stats> results, IPropertyStreamReader reader)
        {
            Stats stats = null;

            while (true)
            {
                stats = new Stats();
                if (!stats.Import(key, reader)) break;
                key++;

                ulong id = 0;
                reader.ReadUint64(key, out id, 0);
                key++;

                results.Add(id, stats);
            }

            return true;
        }

        public void OnPreLoad()
        {
            // This death type is unused apparently
            sDeathTypes.Remove(SimDescription.DeathType.InvisibleSim);

            if (!GameUtils.IsInstalled(ProductVersion.EP1))
            {
                sOccultTypes.Remove(OccultTypesEx.Mummy);

                sDeathTypes.Remove(SimDescription.DeathType.MummyCurse);
            }

            if (!GameUtils.IsInstalled(ProductVersion.EP2))
            {
                sOccultTypes.Remove(OccultTypesEx.Frankenstein);
            }

            if (!GameUtils.IsInstalled(ProductVersion.EP3))
            {
                if (!GameUtils.IsInstalled(ProductVersion.EP7))
                {
                    sOccultTypes.Remove(OccultTypesEx.Vampire);

                    sDeathTypes.Remove(SimDescription.DeathType.Thirst);
                }

                sDeathTypes.Remove(SimDescription.DeathType.Meteor);
            }

            if (!GameUtils.IsInstalled(ProductVersion.EP4))
            {
                sOccultTypes.Remove(OccultTypesEx.ImaginaryFriend);
            }

            if (!GameUtils.IsInstalled(ProductVersion.EP5))
            {
                sOccultTypes.Remove(OccultTypesEx.Unicorn);

                sDeathTypes.Remove(SimDescription.DeathType.PetOldAgeBad);
                sDeathTypes.Remove(SimDescription.DeathType.PetOldAgeGood);
            }

            if (!GameUtils.IsInstalled(ProductVersion.EP6))
            {
                sOccultTypes.Remove(OccultTypesEx.Genie);

                sDeathTypes.Remove(SimDescription.DeathType.HumanStatue);
                sDeathTypes.Remove(SimDescription.DeathType.WateryGrave);
            }

            if (!GameUtils.IsInstalled(ProductVersion.EP7))
            {
                sOccultTypes.Remove(OccultTypesEx.Bonehilda);
                sOccultTypes.Remove(OccultTypesEx.Fairy);
                sOccultTypes.Remove(OccultTypesEx.Werewolf);
                sOccultTypes.Remove(OccultTypesEx.Witch);
                sOccultTypes.Remove(OccultTypesEx.Zombie);

                sDeathTypes.Remove(SimDescription.DeathType.HauntingCurse);
                sDeathTypes.Remove(SimDescription.DeathType.JellyBeanDeath);
                sDeathTypes.Remove(SimDescription.DeathType.Transmuted);
            }

            if (!GameUtils.IsInstalled(ProductVersion.EP8))
            {
                sOccultTypes.Remove(OccultTypesEx.Alien);

                sDeathTypes.Remove(SimDescription.DeathType.Freeze);
            }

            if (!GameUtils.IsInstalled(ProductVersion.EP10))
            {
                sOccultTypes.Remove(OccultTypesEx.Mermaid);

                sDeathTypes.Remove(SimDescription.DeathType.MermaidDehydrated);
                sDeathTypes.Remove(SimDescription.DeathType.ScubaDrown);
                sDeathTypes.Remove(SimDescription.DeathType.Shark);
            }

            if (!GameUtils.IsInstalled(ProductVersion.EP11))
            {
                sOccultTypes.Remove(OccultTypesEx.Plumbot);
                sOccultTypes.Remove(OccultTypesEx.TimeTraveler);

                sDeathTypes.Remove(SimDescription.DeathType.Causality);
                sDeathTypes.Remove(SimDescription.DeathType.Robot);
                sDeathTypes.Remove(SimDescription.DeathType.Jetpack);
            }

            // Removed since the sims are not interactable
            sOccultTypes.Remove(OccultTypesEx.Bonehilda);
            sOccultTypes.Remove(OccultTypesEx.TimeTraveler);

            // Removed until the pet check
            sOccultTypes.Remove(OccultTypesEx.Unicorn);
        }

        public static List<WoohooLocationControl> GetOptionedLocations(SimDescription sim)
        {
            List<WoohooLocationControl> results = new List<WoohooLocationControl>();

            foreach (WoohooLocationControl location in Common.DerivativeSearch.Find<WoohooLocationControl>())
            {
                if (!location.AllowLocation(sim, true)) continue;

                results.Add(location);
            }

            return results;
        }

        public override void MergeTravelData(Skill paramSkill)
        {
            base.MergeTravelData(paramSkill);

            KamaSimtra skill = paramSkill as KamaSimtra;

            mTotal = new Stats(skill.mTotal);

            mAffair = new Stats(skill.mAffair);
            mCheats = new Stats(skill.mCheats);
            mFidelity = new Stats(skill.mFidelity);

            mSims = MergeTravelData(skill.mSims);
            mGhosts = MergeTravelData(skill.mGhosts);
            mOccult = MergeTravelData(skill.mOccult);
            mService = MergeTravelData(skill.mService);
            mRoles = MergeTravelData(skill.mRoles);
            mCelebrities = MergeTravelData(skill.mCelebrities);
            mForeign = MergeTravelData(skill.mForeign);

            mCasual = new Stats(skill.mCasual);
            mPublic = new Stats(skill.mPublic);
            mExhibitionist = new Stats(skill.mExhibitionist);
            mWhoring = new Stats(skill.mWhoring);

            mOlder = new Stats(skill.mOlder);
            mYounger = new Stats(skill.mYounger);

            mPositive = new Stats(skill.mPositive);
            mNegative = new Stats(skill.mNegative);

            mWhoringActive = skill.mWhoringActive;
            mRendezvousActive = skill.mRendezvousActive;
        }

        public override Skill Clone(SimDescription owner)
        {
            KamaSimtra skill = base.Clone(owner) as KamaSimtra;

            skill.MergeTravelData(this);

            return skill;
        }

        protected static Dictionary<T, Stats> MergeTravelData<T>(Dictionary<T, Stats> source)
        {
            Dictionary<T, Stats> dest = new Dictionary<T, Stats>();

            foreach (KeyValuePair<T, Stats> value in source)
            {
                dest.Add(value.Key, new Stats(value.Value));
            }

            return dest;
        }

        public override bool OnLoadFixup()
        {
            try
            {
                mTotal = new Stats();

                foreach (Stats stat in mSims.Values)
                {
                    stat.OnLoadFixup();

                    mTotal.Add(stat);
                }

                mTotal.OnLoadFixup();

                mAffair.OnLoadFixup();
                mCheats.OnLoadFixup();
                mFidelity.OnLoadFixup();

                OnLoadFixup(mGhosts.Values);
                OnLoadFixup(mOccult.Values);
                OnLoadFixup(mService.Values);
                OnLoadFixup(mForeign.Values);
                OnLoadFixup(mCelebrities.Values);

                mCasual.OnLoadFixup();
                mPublic.OnLoadFixup();
                mExhibitionist.OnLoadFixup();
                mWhoring.OnLoadFixup();

                mOlder.OnLoadFixup();
                mYounger.OnLoadFixup();

                mPositive.OnLoadFixup();
                mNegative.OnLoadFixup();

                return base.OnLoadFixup();
            }
            catch (Exception e)
            {
                Common.Exception(SkillOwner, e);
                return false;
            }
        }

        protected static void OnLoadFixup(IEnumerable<Stats> list)
        {
            foreach (Stats stats in list)
            {
                stats.OnLoadFixup();
            }
        }

        public void OnWorldLoadFinished()
        {
            sLastNotch.Clear();

            Settings.ApplySkillPoints();

            new Common.DelayedEventListener(EventTypeId.kRoomChanged, OnRoomChanged);
            new Common.DelayedEventListener(EventTypeId.kSocialInteraction, OnSocialInteraction);

            new Common.AlarmTask(KamaSimtraSettings.kDailyAlarmTime, DaysOfTheWeek.All, OnDailyAlarm);
        }

        protected static void OnRoomChanged(Event e)
        {
            Sim actor = e.Actor as Sim;
            if (actor != null)
            {
                if ((actor.LotCurrent != null) && (!actor.LotCurrent.IsWorldLot))
                {
                    List<Sim> buffers = new List<Sim>();
                    List<Sim> others = new List<Sim>();

                    IEnumerable<Sim> sims = actor.LotCurrent.GetAllActors();

                    foreach (Sim sim in sims)
                    {
                        if (!SimTypes.IsEquivalentSpecies(actor.SimDescription, sim.SimDescription)) continue;

                        if (actor.RoomId != sim.RoomId) continue;

                        if (KamaSimtra.GetSkillLevel(sim) >= 10)
                        {
                            buffers.Add(sim);
                        }

                        others.Add(sim);
                    }

                    if (buffers.Count > 0)
                    {
                        foreach (Sim sim in others)
                        {
                            if (buffers.Count == 1)
                            {
                                if (buffers[0] == sim) continue;
                            }

                            if (Woohooer.Settings.mAllowTeenWoohoo)
                            {
                                if (sim.SimDescription.ChildOrBelow) continue;
                            }
                            else
                            {
                                if (sim.SimDescription.TeenOrBelow) continue;
                            }

                            bool allow = false;
                            foreach (Sim buffer in buffers)
                            {
                                GreyedOutTooltipCallback callBack = null;

                                string reason;
                                if (CommonSocials.CanGetRomantic(buffer.SimDescription, sim.SimDescription, true, false, true, ref callBack, out reason))
                                {
                                    allow = true;
                                    break;
                                }
                            }

                            if (!allow) continue;

                            /*
                            if (!Woohooer.Settings.mAllowNearRelationRomanceV2[PersistedSettings.GetSpeciesIndex(sim)])
                            {
                                bool allow = false;
                                foreach (Sim buffer in buffers)
                                {
                                    if (!Relationships.IsCloselyRelated(buffer.SimDescription, sim.SimDescription, false))
                                    {
                                        allow = true;
                                        break;
                                    }
                                }

                                if (!allow) continue;
                            }
                            */

                            sim.BuffManager.AddElement(sOrgasmic, sKamaSimtraOrigin);
                        }
                    }
                }
            }
        }

        protected static void OnSocialInteraction(Event e)
        {
            using (Common.TestSpan span = new Common.TestSpan(ScoringLookup.Stats, "Duration KamaSimtra:OnSocialInteraction"))
            {
                SocialEvent social = e as SocialEvent;
                if (social != null)
                {
                    if (social.WasAccepted)
                    {
                        Sim actor = social.Actor as Sim;
                        Sim target = social.TargetObject as Sim;

                        if (KamaSimtra.GetSkillLevel(actor) >= 8)
                        {
                            switch (social.SocialName)
                            {
                                case "FirstKiss":
                                case "KissGoodbye":
                                case "Kiss":
                                case "Make Out":
                                    target.BuffManager.AddElement(BuffNames.GreatKisser, sKamaSimtraOrigin);

                                    break;
                            }
                        }
                    }
                }
            }
        }

        protected static void OnDailyAlarm()
        {
            try
            {                
                List<SimDescription> sims = Household.AllSimsLivingInWorld();
                foreach (SimDescription sim in sims)
                {
                    if (sim.SkillManager == null) continue;

                    try
                    {
                        KamaSimtra skill = sim.SkillManager.GetSkill<KamaSimtra>(KamaSimtra.StaticGuid);
                        if (skill == null) continue;

                        if (skill.SkillLevel < 7) continue;

                        if (sim.TraitManager == null) continue;

                        if (sim.TraitManager.HasElement(TraitNames.LongDistanceFriend)) continue;

                        Charisma cSkill = sim.SkillManager.GetSkill<Charisma>(SkillNames.Charisma);

                        if (cSkill != null)
                        {
                            if (cSkill.IsSuperFriendly()) continue;
                        }

                        if (Settings.mDailyRelationshipChange != 0)
                        {
                            foreach (Relationship relation in Relationship.Get(sim))
                            {
                                SimDescription other = relation.GetOtherSimDescription(sim);
                                if (other.ChildOrBelow) continue;

                                if (!SimTypes.IsEquivalentSpecies(sim, other)) continue;

                                if (other.TraitManager == null) continue;

                                if (other.TraitManager.HasElement(TraitNames.LongDistanceFriend)) continue;

                                Charisma oCSkill = other.SkillManager.GetSkill<Charisma>(SkillNames.Charisma);

                                if (oCSkill != null)
                                {
                                    if (oCSkill.IsSuperFriendly()) continue;
                                }

                                try
                                {
                                    int like = 0;

                                    foreach (KamaSimtraOpportunity opp in skill.AllOpportunities)
                                    {
                                        if (!opp.Completed) continue;

                                        like += opp.GetLikingChange(other);
                                    }

                                    if (like != 0)
                                    {
                                        if (Common.kDebugging)
                                        {
                                            Common.DebugNotify("KamaSimtra" + Common.NewLine + sim.FullName + Common.NewLine + other.FullName + Common.NewLine + like);
                                        }

                                        relation.LTR.UpdateLiking(like * Settings.mDailyRelationshipChange);
                                    }
                                }
                                catch (Exception e)
                                {
                                    Common.Exception(sim, other, e);
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Common.Exception(sim, e);
                    }
                }
            }
            catch (Exception e)
            {
                Common.Exception("OnDailyAlarm", e);
            }
        }

        public bool CanWhore()
        {
            return (SkillLevel >= 1);
        }

        public static bool IsWhoring(Sim sim)
        {
            return IsWhoring(sim.SimDescription);
        }
        public static bool IsWhoring(SimDescription sim)
        {
            KamaSimtra skill = sim.SkillManager.GetSkill<KamaSimtra>(KamaSimtra.StaticGuid);
            if (skill == null) return false;

            return skill.WhoringActive;
        }

        public static bool IsRendezvous(Sim sim)
        {
            return IsRendezvous(sim.SimDescription);
        }
        public static bool IsRendezvous(SimDescription sim)
        {
            KamaSimtra skill = sim.SkillManager.GetSkill<KamaSimtra>(KamaSimtra.StaticGuid);
            if (skill == null) return false;

            return skill.RendezvousActive;
        }

        public int GetWomanizerNotches()
        {
            int count = 0;

            foreach (Stats stats in mSims.Values)
            {
                if (stats.GetTally(CASAgeGenderFlags.AgeMask, CASAgeGenderFlags.Female) > 0)
                {
                    count++;
                }
            }

            return count;
        }

        public int GetManEaterNotches()
        {
            int count = 0;

            foreach (Stats stats in mSims.Values)
            {
                if (stats.GetTally(CASAgeGenderFlags.AgeMask, CASAgeGenderFlags.Male) > 0)
                {
                    count++;
                }
            }

            return count;
        }

        public int GetGalaxyOfStarsNotches()
        {
            uint result = 0;
            foreach (KeyValuePair<uint, Stats> stats in mCelebrities)
            {
                result += (uint)(stats.Key * stats.Value.Total);
            }

            return (int)result;
        }

        public int GetCyberJunkieNotches()
        {
            int result = 0;

            foreach (Stats stat in mSims.Values)
            {
                if (stat.GetTally(CommonWoohoo.WoohooLocation.Computer) > 0)
                {
                    result++;
                }
            }

            return result;
        }

        public int GetRisqueNotches()
        {
            return mTotal.GetTally(CommonWoohoo.WoohooStyle.Risky);
        }

        public int GetFidelityNotches()
        {
            return mFidelity.Total;
        }

        public int GetSatisfiedCustomers()
        {
            return mWhoring.Total;
        }

        public int GetProlificNotches()
        {
            return mTotal.Total;
        }

        public int GetCougarNotches()
        {
            return mYounger.GetTally(CASAgeGenderFlags.AgeMask, CASAgeGenderFlags.Male);
        }

        public int GetCradleRobberNotches()
        {
            return mYounger.Total;
        }

        public int GetGoldDiggerNotches()
        {
            return mOlder.GetTally(CASAgeGenderFlags.Elder, CASAgeGenderFlags.Female|CASAgeGenderFlags.Male);
        }

        public int GetPrecociousNotches()
        {
            return mOlder.Total;
        }

        public int GetGigoloNotches()
        {
            return mWhoring.Total;
        }

        public int GetBikeNotches()
        {
            return mAffair.Total;
        }

        public int GetEasyRiderNotches()
        {
            return mCasual.Total;
        }

        public int GetCheaterNotches()
        {
            return mCheats.Total;
        }

        public int GetPromiscuousNotches()
        {
            return mSims.Count;
        }

        public int GetExhibitionistNotches()
        {
            return mExhibitionist.Total;
        }

        public int GetCasanovaNotches()
        {
            return mPositive.Total;
        }

        public int GetOccultistNotches()
        {
            int total = 0;
            foreach (KeyValuePair<OccultTypesEx,Stats> stats in mOccult)
            {
                if (stats.Key == OccultTypesEx.None) continue;

                total += stats.Value.Total;
            }

            return total;
        }

        public int GetFreshMeatNotches()
        {
            Stats stats;
            if (!mOccult.TryGetValue(OccultTypesEx.None, out stats)) return 0;

            return stats.Total;
        }

        public int GetJourneymanNotches()
        {
            int count = 0;

            foreach (Stats stats in mService.Values)
            {
                count += stats.Total;
            }

            return count;
        }

        public int GetStarryEyedNotches()
        {
            int count = 0;

            foreach (Stats stats in mCelebrities.Values)
            {
                count += stats.Total;
            }

            return count;
        }

        public int GetGraveRobberNotches()
        {
            int total = 0;
            foreach (KeyValuePair<SimDescription.DeathType, Stats> stats in mGhosts)
            {
                if (stats.Value.Total < Settings.mGraveRobberMinNotchPerDeathType) continue;

                total++;
            }

            return total;
        }

        public int GetGraveRobberMinNotches()
        {
            List<SimDescription.DeathType> types = new List<SimDescription.DeathType>();
            if (SkillOwner.IsHuman)
            {
                types.AddRange(sDeathTypes);
                types.Remove(SimDescription.DeathType.PetOldAgeBad);
                types.Remove(SimDescription.DeathType.PetOldAgeGood);
            }
            else if (SkillOwner.IsHorse || SkillOwner.IsCat || SkillOwner.IsADogSpecies)
            {
                types.Add(SimDescription.DeathType.PetOldAgeBad);
                types.Add(SimDescription.DeathType.PetOldAgeGood);
            }

            return types.Count;
        }

        public bool IsGraveRobber()
        {
            return (GetGraveRobberNotches() >= GetGraveRobberMinNotches());
        }

        public int GetMonsterMasherNotches()
        {
            List<OccultTypesEx> myTypes = GetOccultType(SkillOwner, false);

            int total = 0;
            foreach (KeyValuePair<OccultTypesEx, Stats> stats in mOccult)
            {
                if (stats.Key == OccultTypesEx.None) continue;

                if (myTypes.Contains(stats.Key)) continue;

                if (stats.Value.Total < Settings.mMonsterMashMinNotchPerOccult) continue;

                total++;
            }

            return total;
        }

        public int GetMonsterMasherMinNotches()
        {
            List<OccultTypesEx> myTypes = GetOccultType(SkillOwner, false);

            List<OccultTypesEx> allTypes = new List<OccultTypesEx>();
            if (SkillOwner.IsHuman)
            {
                allTypes.AddRange(sOccultTypes);
            }
            else if (SkillOwner.IsHorse)
            {
                allTypes.Add(OccultTypesEx.Unicorn);
            }
            else if (SkillOwner.IsADogSpecies || SkillOwner.IsCat)
            {
                allTypes.Add(OccultTypesEx.Ghost);
            }

            foreach (OccultTypesEx type in myTypes)
            {
                allTypes.Remove(type);
            }

            allTypes.Remove(OccultTypesEx.None);

            return allTypes.Count;
        }

        public bool IsMonsterMasher()
        {
            return (GetMonsterMasherNotches() >= GetMonsterMasherMinNotches());
        }

        public bool IsStarryEyed()
        {
            float count = 0, total = 0;

            foreach (KeyValuePair<uint, Stats> value in mCelebrities)
            {
                total += (value.Key * value.Value.Total);

                count += value.Value.Total;
            }

            if (GetStarryEyedNotches() < Settings.mStarryEyedMinNotches) return false;

            return ((total / count) >= Settings.mStarryEyedAverageLevel);
        }

        static List<WorldName> sWorldlyNames;

        public int GetWorldlyNotches()
        {
            int locales = 0;

            if (sWorldlyNames == null)
            {
                sWorldlyNames = new List<WorldName>();

                if (GameUtils.IsInstalled(ProductVersion.EP1))
                {
                    sWorldlyNames.Add(WorldName.China);
                    sWorldlyNames.Add(WorldName.Egypt);
                    sWorldlyNames.Add(WorldName.France);
                }

                if (GameUtils.IsInstalled(ProductVersion.EP9))
                {
                    sWorldlyNames.Add(WorldName.University);
                }

                if (GameUtils.IsInstalled(ProductVersion.EP11))
                {
                    sWorldlyNames.Add(WorldName.FutureWorld);
                }
            }

            foreach (WorldName world in sWorldlyNames)
            {
                Stats stats;
                if (!mForeign.TryGetValue(world, out stats))
                {
                    continue;
                }

                if (stats.Total >= Settings.mWorldlyMinPerWorld)
                {
                    locales++;
                }
            }

            return locales;
        }

        public int GetExperiencedLocations()
        {
            int locales = 0;

            foreach (WoohooLocationControl location in GetOptionedLocations(SkillOwner))
            {
                int count = 0;
                foreach (Stats stats in mSims.Values)
                {
                    count += stats.GetTally(location.Location);

                    if (count >= Settings.mExperiencedMinPerLocation)
                    {
                        locales++;
                        break;
                    }
                }
            }

            return locales;
        }

        public int GetRenown()
        {
            int renown = SkillLevel * Settings.mRenownPerLevel;

            foreach (KamaSimtraOpportunity opp in AllOpportunities)
            {
                if (!opp.AppliesToRenown) continue;

                if (opp.Completed)
                {
                    renown += opp.GetRenown();
                }
            }

            return renown;
        }

        public static int GetRenown(SimDescription sim)
        {
            KamaSimtra skill = sim.SkillManager.GetSkill<KamaSimtra>(KamaSimtra.StaticGuid);
            if (skill == null) return 0;

            return skill.GetRenown();
        }

        public int GetPayment()
        {
            return GetRenown() * Settings.mRenownToPaymentMultiple;
        }

        // Externalized to StoryProgression
        public static bool ActivateProfessional(SimDescription sim)
        {
            try
            {
                KamaSimtra skill = EnsureSkill(sim);
                if (skill != null)
                {
                    skill.WhoringActive = true;
                }

                return true;
            }
            catch (Exception e)
            {
                Common.Exception(sim, e);
                return false;
            }
        }

        // Externalized to StoryProgression
        public static bool ActivateRendezvous(SimDescription sim)
        {
            try
            {
                KamaSimtra skill = EnsureSkill(sim);
                if (skill != null)
                {
                    skill.RendezvousActive = true;
                }

                return true;
            }
            catch (Exception e)
            {
                Common.Exception(sim, e);
                return false;
            }
        }

        // Externalized to StoryProgression
        public static void AddNotches(SimDescription actor, SimDescription target, bool risky, bool tryForBaby)
        {
            try
            {
                CommonWoohoo.WoohooStyle style = CommonWoohoo.WoohooStyle.Safe;
                if (risky)
                {
                    style = CommonWoohoo.WoohooStyle.Risky;
                }
                else if (tryForBaby)
                {
                    style = CommonWoohoo.WoohooStyle.TryForBaby;
                }

                CommonWoohoo.WoohooLocation location = CommonWoohoo.WoohooLocation.Bed;

                Lot lot = actor.LotHome;
                if (lot == null)
                {
                    lot = target.LotHome;
                }

                if (lot == null) return;

                switch (actor.Species)
                {
                    case CASAgeGenderFlags.Human:
                        List<WoohooLocationControl> locations = GetOptionedLocations(actor);

                        RandomUtil.RandomizeListOfObjects(locations);

                        bool success = false;

                        foreach(WoohooLocationControl choice in locations)
                        {
                            location = choice.Location;

                            foreach(SimDescription sim in new SimDescription[] { actor, target })
                            {
                                Lot testLot = sim.LotHome;
                                if ((testLot == null) && (sim.CreatedSim != null))
                                {
                                    testLot = sim.CreatedSim.LotCurrent;
                                }

                                if (testLot == null) continue;

                                if (choice.HasLocation(testLot))
                                {
                                    success = true;
                                    break;
                                }
                            }

                            if (success) break;
                        }

                        if (!success)
                        {
                            location = CommonWoohoo.WoohooLocation.Bed;
                        }

                        break;
                    case CASAgeGenderFlags.Cat:
                    case CASAgeGenderFlags.Dog:
                    case CASAgeGenderFlags.LittleDog:
                        location = CommonWoohoo.WoohooLocation.PetHouse;
                        break;
                    case CASAgeGenderFlags.Horse:
                        location = CommonWoohoo.WoohooLocation.BoxStall;
                        break;
                }

                AddNotch(target, actor, lot, location, style);
                AddNotch(actor, target, lot, location, style);
            }
            catch (Exception e)
            {
                Common.Exception(actor, target, e);
            }
        }

        public static void AddNotch(SimDescription actor, SimDescription target, Lot lot, CommonWoohoo.WoohooLocation location, CommonWoohoo.WoohooStyle style)
        {
            if ((!SimTypes.IsSelectable(actor)) || (!SimTypes.IsSelectable(target)))
            {
                if (!Settings.mInactiveGain) return;
            }

            Skill commonSkill = actor.SkillManager.AddElement(StaticGuid);

            KamaSimtra skill = commonSkill as KamaSimtra;
            if (skill == null) return;

            skill.AddNotch(target, lot, location, style);
        }

        protected int GetPositiveBuffLength()
        {
            int length = Settings.mBasePositiveBuffLength;

            foreach (KamaSimtraOpportunity opp in AllOpportunities)
            {
                if (!opp.Completed) continue;

                length += opp.GetPositiveBuffLength();
            }

            return length;
        }

        public int GetSatisfactionChance()
        {
            int positive = 0, negative = 0;

            foreach (KamaSimtraOpportunity opp in AllOpportunities)
            {
                opp.GetBuffChance(opp.Completed, ref positive, ref negative);
            }

            positive += Settings.mBaseSatisfaction;

            positive += (SkillLevel * Settings.mSatisfactionPerLevel);

            if (positive > 100)
            {
                positive = 100;
            }
            else if (positive < 0)
            {
                positive = 0;
            }

            return positive;
        }

        public int GetDisappointmentChance()
        {
            int positive = 0, negative = 0;

            foreach (KamaSimtraOpportunity opp in AllOpportunities)
            {
                opp.GetBuffChance(opp.Completed, ref positive, ref negative);
            }

            negative += Settings.mBaseDisappointment;

            negative -= (SkillLevel * Settings.mSatisfactionPerLevel);

            if (negative > 100)
            {
                negative = 100;
            }
            else if (negative < 0)
            {
                negative = 0;
            }

            return negative;
        }

        public int GetPositiveMood()
        {
            int moodValue = (int)(GetRenown() * Settings.mRenownToMoodMultiple);

            if (moodValue > 100)
            {
                moodValue = 100;
            }

            return moodValue;
        }

        protected void AddBuff(Sim target, Key key, bool previousExperience)
        {
            if (!Woohooer.Settings.mApplyBuffs) return;

            if (target == null) return;

            if (target.BuffManager == null) return;

            int positive = GetSatisfactionChance(), negative = GetDisappointmentChance();

            if (previousExperience)
            {
                negative /= 2;
            }

            int moodValue = GetPositiveMood();

            if ((moodValue > 0) && (positive > 0) && (RandomUtil.RandomChance(positive)))
            {
                mPositive.Add(key);

                if (SkillLevel >= 3)
                {
                    if (target.BuffManager.AddElement(sGoodBuff, moodValue, GetPositiveBuffLength(), sGoodOrigin))
                    {
                        target.BuffManager.RemoveElement(sBadBuff);
                    }
                }
            }
            else if ((negative > 0) && (RandomUtil.RandomChance(negative)))
            {
                mNegative.Add(key);

                if (target.BuffManager.AddElement(sBadBuff, sBadOrigin))
                {
                    target.BuffManager.RemoveElement(sGoodBuff);
                }
            }

            if ((target.Partner != SkillOwner) && (target.Household != SkillOwner.Household))
            {
                if ((WhoringActive) && (!IsWhoring(target)))
                {
                    int score = ScoringLookup.GetScore("LikeProfessional", target.SimDescription);
                    if (score > 0)
                    {
                        target.BuffManager.RemoveElement(sDislikeProfessional);
                        target.BuffManager.AddElement(sLikeProfessional, WoohooBuffs.sWoohooOrigin);
                    }
                    else if (score < 0)
                    {
                        target.BuffManager.RemoveElement(sLikeProfessional);
                        target.BuffManager.AddElement(sDislikeProfessional, WoohooBuffs.sWoohooOrigin);
                    }
                }
            }
        }

        public static List<OccultTypesEx> GetOccultType(SimDescription sim, bool includeHuman)
        {
            List<OccultTypesEx> types = new List<OccultTypesEx>();

            if (sim.OccultManager.HasAnyOccultType())
            {
                if (sim.OccultManager.HasOccultType(OccultTypes.Frankenstein))
                {
                    types.Add(OccultTypesEx.Frankenstein);
                }
                
                if (sim.OccultManager.HasOccultType(OccultTypes.Mummy))
                {
                    types.Add(OccultTypesEx.Mummy);
                }
                
                if (sim.OccultManager.HasOccultType(OccultTypes.Vampire))
                {
                    types.Add(OccultTypesEx.Vampire);
                }
                
                if (sim.OccultManager.HasOccultType(OccultTypes.ImaginaryFriend))
                {
                    types.Add(OccultTypesEx.ImaginaryFriend);
                }
                
                if (sim.OccultManager.HasOccultType(OccultTypes.Unicorn))
                {
                    types.Add(OccultTypesEx.Unicorn);
                }

                if (sim.OccultManager.HasOccultType(OccultTypes.Witch))
                {
                    types.Add(OccultTypesEx.Witch);
                }

                if (sim.OccultManager.HasOccultType(OccultTypes.Werewolf))
                {
                    types.Add(OccultTypesEx.Werewolf);
                }

                if (sim.OccultManager.HasOccultType(OccultTypes.Fairy))
                {
                    types.Add(OccultTypesEx.Fairy);
                }

                if (sim.OccultManager.HasOccultType(OccultTypes.Genie))
                {
                    types.Add(OccultTypesEx.Genie);
                }

                if (sim.OccultManager.HasOccultType(OccultTypes.PlantSim))
                {
                    types.Add(OccultTypesEx.PlantSim);
                }

                if (sim.OccultManager.HasOccultType(OccultTypes.Mermaid))
                {
                    types.Add(OccultTypesEx.Mermaid);
                }

                if (sim.OccultManager.HasOccultType(OccultTypes.Robot))
                {
                    types.Add(OccultTypesEx.Plumbot);
                }

                if (sim.OccultManager.HasOccultType(OccultTypes.TimeTraveler))
                {
                    types.Add(OccultTypesEx.TimeTraveler);
                }
            }

            if (sim.IsAlien)
            {
                types.Add(OccultTypesEx.Alien);
            }

            if (sim.IsDead)
            {
                types.Add(OccultTypesEx.Ghost);
            }
            
            if (sim.CreatedByService is GrimReaper)
            {
                types.Add(OccultTypesEx.GrimReaper);
            }

            if (sim.IsBonehilda)
            {
                types.Add(OccultTypesEx.Bonehilda);
            }

            if (sim.IsZombie)
            {
                types.Add(OccultTypesEx.Zombie);
            }

            if ((includeHuman) && (types.Count == 0))
            {
                types.Add(OccultTypesEx.None);
            }

            return types;
        }

        public static void SeedServicePool(bool seedPros)
        {
            foreach (SimDescription sim in Households.Humans(Household.NpcHousehold))
            {
                if (!SimTypes.InServicePool(sim)) continue;

                if (Woohooer.Settings.AllowTeen(true))
                {
                    if (sim.ChildOrBelow) continue;
                }
                else
                {
                    if (sim.TeenOrBelow) continue;
                }

                bool done = false;                
                if (sim.SkillManager.GetSkillLevel(StaticGuid) == -1)
                {
                    KamaSimtra skill = sim.SkillManager.AddElement(StaticGuid) as KamaSimtra;
                    if (skill != null)
                    {
                        done = true;

                        skill.ForceSkillLevelUp(RandomUtil.GetInt(1, 10));
                        skill.RendezvousActive = true;

                        if (KamaSimtra.Settings.mSeedServicePool && RandomUtil.CoinFlip())
                        {
                            skill.WhoringActive = true;
                        }
                    }
                }

                if (!done && seedPros)
                {
                    KamaSimtra skill = sim.SkillManager.GetSkill<KamaSimtra>(StaticGuid);
                    if (skill != null)
                    {
                        if (KamaSimtra.Settings.mSeedServicePool && RandomUtil.CoinFlip())
                        {
                            skill.WhoringActive = true;
                        }
                    }
                }
                
            }    
        }       

        public static Dictionary<int, List<SimDescription>> GetPotentials(CASAgeGenderFlags allowAges, bool professionals)
        {
            Dictionary<int, List<SimDescription>> results = new Dictionary<int, List<SimDescription>>();

            SeedServicePool(false);       

            foreach (SimDescription sim in Household.EverySimDescription())
            {
                if((sim.Age & allowAges) == CASAgeGenderFlags.None)
                {
                    continue;
                }

                if (!professionals)
                {                    
                    if (!IsRendezvous(sim)) continue;
                }
                else
                {                    
                    if (!IsWhoring(sim)) continue;
                }                

                int level = sim.SkillManager.GetSkillLevel(StaticGuid);
                if (level <= 0) continue;

                if (sim.CreatedSim != null)
                {
                    // Working
                    if ((sim.Service != null) && (sim.Service.IsSimAssignedTask(sim))) continue;

                    if (sim.CreatedSim.CurrentInteraction != null)
                    {
                        if (sim.CreatedSim.CurrentInteraction is ICountsAsWorking) continue;
                    }
                }

                List<SimDescription> result;
                if (!results.TryGetValue(level, out result))
                {
                    result = new List<SimDescription>();
                    results.Add(level, result);
                }                

                result.Add(sim);
            }

            return results;
        }

        protected void AddNotch(SimDescription target, Lot lot, CommonWoohoo.WoohooLocation location, CommonWoohoo.WoohooStyle style)
        {
            if (SkillOwner.Teen)
            {
                if (!Woohooer.Settings.mAllowTeenWoohoo) return;
            }

            float now = SimClock.ElapsedTime(TimeUnit.Minutes);

            float lastTime;
            if (sLastNotch.TryGetValue(SkillOwner.SimDescriptionId, out lastTime))
            {
                if (now - lastTime < 5) return;
            }

            sLastNotch[SkillOwner.SimDescriptionId] = now;

            if (Settings.mShowNotices)
            {
                Common.Notify("AddNotch:" + Common.NewLine + SkillOwner.FullName + Common.NewLine + target.FullName + Common.NewLine + location + Common.NewLine + style + Common.NewLine + Common.NewLine + "Points: " + SkillPoints);
            }

            float previous = SkillOwner.SkillManager.OverallModifier;
            SkillOwner.SkillManager.mOverallModifier = 0;

            try
            {
                AddPoints(1, false);
            }
            finally
            {
                SkillOwner.SkillManager.mOverallModifier = previous;
            }

            Key key = new Key(target.Age | target.Gender, location, style);

            mTotal.Add(key);

            bool newSim = false;

            Stats stats;
            if (!mSims.TryGetValue(target.SimDescriptionId, out stats))
            {
                stats = new Stats();
                mSims.Add(target.SimDescriptionId, stats);

                newSim = true;
            }

            if (SkillOwner.Partner == target)
            {
                mFidelity.Add(key);
            }
            else
            {
                mFidelity = new Stats();
            }

            bool isNew = stats.Add(key);

            if (Settings.mDistinctSimStats)
            {
                isNew = newSim;
            }

            AddBuff(target.CreatedSim, key, !isNew);

            KamaSimtraSettings.ServiceData data = KamaSimtra.Settings.GetServiceData(target.SimDescriptionId, false);

            if (WhoringActive)
            {
                if ((SkillOwner.Partner != target) && 
                    (SkillOwner.Household != target.Household) &&
                    ((target.IsPlayableGhost) || (!target.IsDead)))
                {
                    int payment = GetPayment();

                    if (data != null && !data.mWasRandom && data.mProfessional == SkillOwner.SimDescriptionId)
                    {                        
                        payment = payment *= 2;
                    }                    

                    if (!target.Household.IsSpecialHousehold)
                    {
                        if (payment > target.FamilyFunds)
                        {
                            payment = target.FamilyFunds;
                        }

                        target.ModifyFunds(-payment);
                    }

                    SkillOwner.ModifyFunds(payment);

                    MoneyEarned(payment);

                    if (isNew)
                    {
                        mWhoring.Add(key);
                    }
                }
            }

            if ((isNew) || (!Settings.mDistinctSimStats))
            {
                WorldName homeWorld = target.HomeWorld;

                if (homeWorld == SkillOwner.HomeWorld)
                {
                    homeWorld = GameUtils.GetCurrentWorld();
                }

                if (SkillOwner.HomeWorld != homeWorld)
                {
                    if (!mForeign.TryGetValue(homeWorld, out stats))
                    {
                        stats = new Stats();
                        mForeign.Add(homeWorld, stats);
                    }

                    stats.Add(key);
                }
            }

            // Only new shags may pass
            if (isNew)
            {
                if (target.CelebrityLevel > 0)
                {
                    if (!mCelebrities.TryGetValue(target.CelebrityLevel, out stats))
                    {
                        stats = new Stats();
                        mCelebrities.Add(target.CelebrityLevel, stats);
                    }

                    stats.Add(key);
                }

                if (SkillOwner.Age < target.Age)
                {
                    mOlder.Add(key);
                }
                else if (SkillOwner.Age > target.Age)
                {
                    mYounger.Add(key);
                }

                if (SkillOwner.Partner != target)
                {
                    if (SkillOwner.Partner != null)
                    {
                        mCheats.Add(key);
                    }

                    if (target.Partner != null)
                    {
                        mAffair.Add(key);
                    }
                }

                Relationship relation = Relationship.Get(SkillOwner, target, false);
                if (relation != null)
                {
                    if (!relation.AreRomantic())
                    {
                        mCasual.Add(key);
                    }
                }

                if (SkillOwner.CreatedSim != null)
                {
                    bool witnessed = false;
                    foreach (Sim sim in lot.GetAllActors())
                    {
                        if (sim.SimDescription == SkillOwner) continue;

                        if (sim.SimDescription == target) continue;

                        if ((SkillOwner.IsHuman) && (!sim.IsHuman)) continue;

                        if (sim.RoomId == SkillOwner.CreatedSim.RoomId)
                        {
                            witnessed = true;
                            break;
                        }
                    }

                    if (witnessed)
                    {
                        mExhibitionist.Add(key);
                    }
                }

                if (!lot.IsResidentialLot)
                {
                    mPublic.Add(key);
                }

                List<OccultTypesEx> ownerOccultType = GetOccultType(SkillOwner, true);
                List<OccultTypesEx> targetOccultType = GetOccultType(target, true);

                foreach (OccultTypesEx targetOccult in targetOccultType)
                {
                    if (ownerOccultType.Contains(targetOccult)) continue;

                    if (!mOccult.TryGetValue(targetOccult, out stats))
                    {
                        stats = new Stats();
                        mOccult.Add(targetOccult, stats);
                    }

                    stats.Add(key);
                }

                if (target.DeathStyle != SimDescription.DeathType.None)
                {
                    if (!mGhosts.TryGetValue(target.DeathStyle, out stats))
                    {
                        stats = new Stats();
                        mGhosts.Add(target.DeathStyle, stats);
                    }

                    stats.Add(key);
                }

                if ((SimTypes.IsService(target)) || (target.AssignedRole != null))
                {
                    if (target.AssignedRole != null)
                    {
                        if (!mRoles.TryGetValue(target.AssignedRole.Type, out stats))
                        {
                            stats = new Stats();
                            mRoles.Add(target.AssignedRole.Type, stats);
                        }

                        stats.Add(key);
                    }
                    else if (SimTypes.InServicePool(target))
                    {
                        if (!mService.TryGetValue(target.CreatedByService.ServiceType, out stats))
                        {
                            stats = new Stats();
                            mService.Add(target.CreatedByService.ServiceType, stats);
                        }

                        stats.Add(key);
                    }
                }
            }

            TestForNewLifetimeOpp();
        }

        protected void CreateRow(string key, Stats stats, List<ObjectPicker.RowInfo> rowInfo)
        {
            CreateRowLocalized(LocalizeString(key), stats, true, rowInfo);
        }
        protected void CreateRowLocalized(string title, Stats stats, bool testTotal, List<ObjectPicker.RowInfo> rowInfo)
        {
            if ((!testTotal) || (stats.Total > 0))
            {
                rowInfo.Add(CreateRow(title, stats.Total, stats.GetDescription(this)));
            }
        }

        protected void GetWorldStats(List<ObjectPicker.TabInfo> tabInfo)
        {
            List<ObjectPicker.RowInfo> rowInfo = new List<ObjectPicker.RowInfo>();

            foreach (KeyValuePair<WorldName, Stats> stats in mForeign)
            {
                CreateRowLocalized(Common.LocalizeEAString("Ui/Caption/Global/WorldName/EP01:" + stats.Key.ToString()), stats.Value, true, rowInfo);
            }

            if (rowInfo.Count == 0) return;

            tabInfo.Add(new ObjectPicker.TabInfo("coupon", LocalizeString("ForeignTitle"), rowInfo));
        }

        protected void GetCelebrityStats(List<ObjectPicker.TabInfo> tabInfo)
        {
            List<ObjectPicker.RowInfo> rowInfo = new List<ObjectPicker.RowInfo>();

            foreach (KeyValuePair<uint, Stats> stats in mCelebrities)
            {
                CreateRowLocalized(EAText.GetNumberString(stats.Key), stats.Value, true, rowInfo);
            }

            if (rowInfo.Count == 0) return;

            tabInfo.Add(new ObjectPicker.TabInfo("coupon", LocalizeString("CelebrityTitle"), rowInfo));
        }

        protected void GetServiceStats(List<ObjectPicker.TabInfo> tabInfo)
        {
            List<ObjectPicker.RowInfo> rowInfo = new List<ObjectPicker.RowInfo>();

            foreach (KeyValuePair<ServiceType, Stats> stats in mService)
            {
                string title;
                if (!Localization.GetLocalizedString("Ui/Caption/Services/Service:" + stats.Key.ToString(), out title))
                {
                    title = "Ui/Caption/Services/Service:" + stats.Key.ToString();
                }

                CreateRowLocalized(title, stats.Value, true, rowInfo);
            }

            foreach (KeyValuePair<Role.RoleType, Stats> stats in mRoles)
            {
                string title = RoleTypeSetting.GetRoleName(stats.Key);

                CreateRowLocalized(title, stats.Value, true, rowInfo);
            }

            if (rowInfo.Count == 0) return;

            tabInfo.Add(new ObjectPicker.TabInfo("coupon", LocalizeString("ServiceTitle"), rowInfo));
        }

        protected void GetOccultStats(List<ObjectPicker.TabInfo> tabInfo)
        {
            List<ObjectPicker.RowInfo> rowInfo = new List<ObjectPicker.RowInfo>();

            foreach (KeyValuePair<OccultTypesEx, Stats> stats in mOccult)
            {
                string name = null;
                switch (stats.Key)
                {
                    case OccultTypesEx.Bonehilda:
                    case OccultTypesEx.GrimReaper:
                    case OccultTypesEx.Zombie:
                    case OccultTypesEx.Alien:
                    case OccultTypesEx.None:
                        name = LocalizeString("Occult" + stats.Key);
                        break;
                    case OccultTypesEx.Fairy:
                        name = OccultTypeHelper.GetLocalizedName(OccultTypes.Fairy);
                        break;
                    case OccultTypesEx.Frankenstein:
                        name = OccultTypeHelper.GetLocalizedName(OccultTypes.Frankenstein);
                        break;
                    case OccultTypesEx.Genie:
                        name = OccultTypeHelper.GetLocalizedName(OccultTypes.Genie);
                        break;
                    case OccultTypesEx.ImaginaryFriend:
                        name = OccultTypeHelper.GetLocalizedName(OccultTypes.ImaginaryFriend);
                        break;
                    case OccultTypesEx.Mummy:
                        name = OccultTypeHelper.GetLocalizedName(OccultTypes.Mummy);
                        break;
                    case OccultTypesEx.Unicorn:
                        name = OccultTypeHelper.GetLocalizedName(OccultTypes.Unicorn);
                        break;
                    case OccultTypesEx.Vampire:
                        name = OccultTypeHelper.GetLocalizedName(OccultTypes.Vampire);
                        break;
                    case OccultTypesEx.Werewolf:
                        name = OccultTypeHelper.GetLocalizedName(OccultTypes.Werewolf);
                        break;
                    case OccultTypesEx.Witch:
                        name = OccultTypeHelper.GetLocalizedName(OccultTypes.Witch);
                        break;
                    case OccultTypesEx.PlantSim:
                        name = OccultTypeHelper.GetLocalizedName(OccultTypes.PlantSim);
                        break;
                    case OccultTypesEx.Mermaid:
                        name = OccultTypeHelper.GetLocalizedName(OccultTypes.Mermaid);
                        break;
                    case OccultTypesEx.Ghost:
                        name = OccultTypeHelper.GetLocalizedName(OccultTypes.Ghost);
                        break;
                    case OccultTypesEx.TimeTraveler:
                        name = OccultTypeHelper.GetLocalizedName(OccultTypes.TimeTraveler);
                        break;
                    case OccultTypesEx.Plumbot:
                        name = OccultTypeHelper.GetLocalizedName(OccultTypes.Robot);
                        break;
                    default:
                        name = "(" + stats.Key + ")";
                        break;
                }


                CreateRowLocalized(name, stats.Value, true, rowInfo);
            }

            foreach (KeyValuePair<SimDescription.DeathType, Stats> stats in mGhosts)
            {
                CreateRowLocalized(Urnstone.DeathTypeToLocalizedString(stats.Key), stats.Value, true, rowInfo);
            }

            if (rowInfo.Count == 0) return;

            tabInfo.Add(new ObjectPicker.TabInfo("coupon", LocalizeString("OccultTitle"), rowInfo));
        }

        protected void GetLocationStats(List<ObjectPicker.TabInfo> tabInfo)
        {
            List<ObjectPicker.RowInfo> rowInfo = new List<ObjectPicker.RowInfo>();

            foreach (WoohooLocationControl location in GetOptionedLocations(SkillOwner))
            {
                Stats stats = new Stats();

                foreach (KeyValuePair<Key, int> value in mTotal.Tallies)
                {
                    if (value.Key.mLocation != location.Location) continue;

                    stats.Add(value.Key, value.Value);
                }

                // Show all available locations
                //if (stats.Total == 0) continue;

                CreateRowLocalized(Common.Localize("Location:" + location.Location), stats, false, rowInfo);
            }

            if (rowInfo.Count == 0) return;

            tabInfo.Add(new ObjectPicker.TabInfo("coupon", LocalizeString("LocationTitle"), rowInfo));
        }

        protected void GetSimStats(List<ObjectPicker.TabInfo> tabInfo)
        {
            List<ObjectPicker.RowInfo> rowInfo = new List<ObjectPicker.RowInfo>();

            Dictionary<ulong, IMiniSimDescription> simsLookup = new Dictionary<ulong,IMiniSimDescription>();

            Dictionary<ulong, SimDescription> sims = SimListing.GetResidents(true);
            foreach (KeyValuePair<ulong, SimDescription> sim in sims)
            {
                simsLookup.Add(sim.Key, sim.Value);
            }

            if (MiniSimDescription.sMiniSims != null)
            {
                foreach (MiniSimDescription miniSim in MiniSimDescription.sMiniSims.Values)
                {
                    if (!simsLookup.ContainsKey(miniSim.SimDescriptionId))
                    {
                        simsLookup.Add(miniSim.SimDescriptionId, miniSim);
                    }
                }
            }

            foreach (KeyValuePair<ulong,Stats> stats in mSims)
            {
                if (stats.Value.Total == 0) continue;

                IMiniSimDescription sim;
                if (!simsLookup.TryGetValue(stats.Key, out sim)) continue;

                CreateRowLocalized(sim.FullName, stats.Value, true, rowInfo);
            }

            if (rowInfo.Count == 0) return;

            tabInfo.Add(new ObjectPicker.TabInfo("coupon", LocalizeString("SimTitle"), rowInfo));
        }

        protected void GetStyleStats(List<ObjectPicker.TabInfo> tabInfo)
        {
            List<ObjectPicker.RowInfo> rowInfo = new List<ObjectPicker.RowInfo>();

            foreach (CommonWoohoo.WoohooStyle style in Enum.GetValues(typeof(CommonWoohoo.WoohooStyle)))
            {
                Stats stats = new Stats();

                foreach (KeyValuePair<Key, int> value in mTotal.Tallies)
                {
                    if (value.Key.mStyle != style) continue;

                    stats.Add(value.Key, value.Value);
                }

                if (stats.Total == 0) continue;

                CreateRow("Style" + style, stats, rowInfo);
            }

            CreateRow("Exhibitionist", mExhibitionist, rowInfo);
            CreateRow("Public", mPublic, rowInfo);

            CreateRow("Casual", mCasual, rowInfo);
            CreateRow("Whoring", mWhoring, rowInfo);

            CreateRow("Affair", mAffair, rowInfo);
            CreateRow("Cheats", mCheats, rowInfo);

            CreateRow("Positive", mPositive, rowInfo);
            CreateRow("Negative", mNegative, rowInfo);

            CreateRow("Older", mOlder, rowInfo);
            CreateRow("Younger", mYounger, rowInfo);

            if (rowInfo.Count == 0) return;

            tabInfo.Add(new ObjectPicker.TabInfo("coupon", LocalizeString("StyleTitle"), rowInfo));
        }

        public override List<ObjectPicker.TabInfo> SecondaryTabs
        {
            get
            {
                List<ObjectPicker.TabInfo> tabInfo = base.SecondaryTabs;

                GetWorldStats(tabInfo);
                GetCelebrityStats(tabInfo);
                GetServiceStats(tabInfo);
                GetOccultStats(tabInfo);
                GetLocationStats(tabInfo);
                GetStyleStats(tabInfo);
                GetSimStats(tabInfo);

                return tabInfo;
            }
        }

        [Persistable]
        protected struct Key
        {
            public readonly CASAgeGenderFlags mAgeGender;

            public readonly CommonWoohoo.WoohooLocation mLocation;

            public readonly CommonWoohoo.WoohooStyle mStyle;

            public Key(int[] values, int index)
            {
                mAgeGender = (CASAgeGenderFlags)values[index];
                mLocation = (CommonWoohoo.WoohooLocation)values[index+1];
                mStyle = (CommonWoohoo.WoohooStyle)values[index+2];
            }
            public Key(CASAgeGenderFlags ageGender, CommonWoohoo.WoohooLocation location, CommonWoohoo.WoohooStyle style)
            {
                mAgeGender = ageGender;
                mLocation = location;
                mStyle = style;
            }

            public override string ToString()
            {
                return mAgeGender + " " + mLocation + " " + mStyle;
            }

            public void Export(List<int> values)
            {
                values.Add((int)mAgeGender);
                values.Add((int)mLocation);
                values.Add((int)mStyle);
            }
        }

        [Persistable]
        protected class Stats
        {
            [Persistable(false)]
            int mTotal;

            Dictionary<Key, int> mTallies = new Dictionary<Key, int>();

            public Stats()
            { }
            public Stats(Stats stats)
            {
                foreach (KeyValuePair<Key, int> value in stats.mTallies)
                {
                    Add(value.Key, value.Value);
                }
            }

            public IEnumerable<KeyValuePair<Key, int>> Tallies
            {
                get { return mTallies; }
            }

            public int Total
            {
                get { return mTotal; }
            }

            public void Export(uint key, IPropertyStreamWriter writer)
            {
                List<int> values = new List<int>();

                foreach (KeyValuePair<Key, int> tally in mTallies)
                {
                    tally.Key.Export(values);
                    values.Add((int)tally.Value);
                }

                writer.WriteInt32(key, values.ToArray());
            }

            public bool Import(uint key, IPropertyStreamReader reader)
            {
                mTallies.Clear();

                int[] values;
                if (!reader.ReadInt32(key, out values)) return false;

                int index = 0;
                while (index < values.Length)
                {
                    mTallies.Add(new Key(values, index), values[index + 3]);
                    index += 4;
                }

                return true;
            }

            public string GetDescription(KamaSimtra skill)
            {
                string result = null;

                foreach (KeyValuePair<Key, int> tally in mTallies)
                {
                    if (result != null)
                    {
                        result += Common.NewLine;
                    }

                    string plural = null;
                    if (tally.Value > 1)
                    {
                        plural = "Plural";
                    }

                    result += skill.LocalizeString("StatDescription", new object[] { 
                        skill.LocalizeString("Age" + (tally.Key.mAgeGender & CASAgeGenderFlags.AgeMask)),
                        skill.LocalizeString("Gender" + (tally.Key.mAgeGender & CASAgeGenderFlags.GenderMask) + plural),
                        Common.Localize("Location:" + tally.Key.mLocation),
                        skill.LocalizeString("Style" + tally.Key.mStyle),
                        tally.Value
                    });
                }

                return result;
            }

            public int GetTally(CommonWoohoo.WoohooLocation location)
            {
                int tally = 0;

                foreach (KeyValuePair<Key, int> value in mTallies)
                {
                    if (value.Key.mLocation == location)
                    {
                        tally += value.Value;
                    }
                }

                return tally;
            }
            public int GetTally(CommonWoohoo.WoohooStyle style)
            {
                int tally = 0;

                foreach (KeyValuePair<Key, int> value in mTallies)
                {
                    if (value.Key.mStyle == style)
                    {
                        tally += value.Value;
                    }
                }

                return tally;
            }
            public int GetTally(CASAgeGenderFlags age, CASAgeGenderFlags gender)
            {
                int tally = 0;

                foreach (KeyValuePair<Key, int> value in mTallies)
                {
                    if ((value.Key.mAgeGender & age) == CASAgeGenderFlags.None) continue;

                    if ((value.Key.mAgeGender & gender) == CASAgeGenderFlags.None) continue;

                    tally += value.Value;
                }

                return tally;
            }

            public bool Add(Key key)
            {
                return Add(key, 1);
            }
            public bool Add(Key key, int tally)
            {
                if (mTallies.ContainsKey(key))
                {
                    mTallies[key] += tally;
                }
                else
                {
                    mTallies.Add(key, tally);
                }

                mTotal += tally;

                return (mTallies[key] == tally);
            }
            public void Add(Stats stat)
            {
                foreach (KeyValuePair<Key, int> value in stat.mTallies)
                {
                    Add(value.Key, value.Value);
                }
            }

            public void OnLoadFixup()
            {
                mTotal = 0;
                foreach (int value in mTallies.Values)
                {
                    mTotal += value;
                }
            }
        }

        public abstract class KamaSimtraMajorStat : MajorStat
        { }

        public class TotalNotches : KamaSimtraMajorStat
        {
            public TotalNotches()
            { }

            protected override string LocalizationKey
            {
                get { return "TotalNotches"; }
            }

            public override int Count
            {
                get { return mSkill.GetProlificNotches(); }
            }

            public override int Order
            {
                get { return 10; }
            }

            public override bool Allow(SimDescription sim)
            {
                return true;
            }

            public override KamaSimtraMajorStat Clone()
            {
                return new TotalNotches();
            }
        }

        public class DifferentSimNotches : KamaSimtraMajorStat
        {
            public DifferentSimNotches()
            { }

            protected override string LocalizationKey
            {
                get { return "DifferentSimNotches"; }
            }

            public override int Count
            {
                get { return mSkill.mSims.Count; }
            }

            public override int Order
            {
                get { return 20; }
            }

            public override bool Allow(SimDescription sim)
            {
                return true;
            }

            public override KamaSimtraMajorStat Clone()
            {
                return new DifferentSimNotches();
            }
        }

        public class RenownStat : KamaSimtraMajorStat
        {
            public RenownStat()
            { }

            protected override string LocalizationKey
            {
                get { return "Renown"; }
            }

            public override int Count
            {
                get { return mSkill.GetRenown(); }
            }

            public override int Order
            {
                get { return 30; }
            }

            public override bool Allow(SimDescription sim)
            {
                return true;
            }

            public override KamaSimtraMajorStat Clone()
            {
                return new RenownStat();
            }
        }

        public class ChanceOfSatisfy : KamaSimtraMajorStat
        {
            public ChanceOfSatisfy()
            { }

            protected override string LocalizationKey
            {
                get { return "ChanceOfSatisfy"; }
            }

            public override int Count
            {
                get { return mSkill.GetSatisfactionChance(); }
            }

            public override int Order
            {
                get { return 40; }
            }

            public override bool Allow(SimDescription sim)
            {
                return true;
            }

            public override KamaSimtraMajorStat Clone()
            {
                return new ChanceOfSatisfy();
            }
        }

        public class BuffValueStat : KamaSimtraMajorStat
        {
            public BuffValueStat()
            { }

            protected override string LocalizationKey
            {
                get { return "BuffValue"; }
            }

            public override int Count
            {
                get { return mSkill.GetPositiveMood(); }
            }

            public override int Order
            {
                get { return 60; }
            }

            public override bool Allow(SimDescription sim)
            {
                return true;
            }

            public override KamaSimtraMajorStat Clone()
            {
                return new BuffValueStat();
            }
        }
    
        public class BuffLengthStat : KamaSimtraMajorStat
        {
            public BuffLengthStat()
            { }

            protected override string LocalizationKey
            {
                get { return "BuffLength"; }
            }

            public override int Count
            {
                get { return mSkill.GetPositiveBuffLength(); }
            }

            public override int Order
            {
                get { return 60; }
            }

            public override bool Allow(SimDescription sim)
            {
                return true;
            }

            public override KamaSimtraMajorStat Clone()
            {
                return new BuffLengthStat();
            }
        }
        
        public class ChanceOfDisappointing : KamaSimtraMajorStat
        {
            public ChanceOfDisappointing()
            { }

            protected override string LocalizationKey
            {
                get { return "ChanceOfDisappointing"; }
            }

            public override int Count
            {
                get { return mSkill.GetDisappointmentChance(); }
            }

            public override int Order
            {
                get { return 70; }
            }

            public override bool Allow(SimDescription sim)
            {
                return true;
            }

            public override KamaSimtraMajorStat Clone()
            {
                return new ChanceOfDisappointing();
            }
        }

        public class WhoringActiveStat : KamaSimtraMajorStat
        {
            public WhoringActiveStat()
            { }

            protected override string LocalizationKey
            {
                get { return "WhoringActive"; }
            }

            public override int Count
            {
                get { return 0; }
            }

            public override string Description
            {
                get 
                { 
                    if (mSkill.WhoringActive)
                    {
                        return mSkill.LocalizeString("WhoringActive"); 
                    }
                    else
                    {
                        return mSkill.LocalizeString("WhoringInactive"); 
                    }
                }
            }

            public override int Order
            {
                get { return 110; }
            }

            public override bool Allow(SimDescription sim)
            {
                return sim.IsHuman;
            }

            public override KamaSimtraMajorStat Clone()
            {
                return new WhoringActiveStat();
            }
        }

        public class RendezvousActiveStat : KamaSimtraMajorStat
        {
            public RendezvousActiveStat()
            { }

            protected override string LocalizationKey
            {
                get { return "RendezvousActive"; }
            }

            public override int Count
            {
                get { return 0; }
            }

            public override string Description
            {
                get
                {
                    if (mSkill.RendezvousActive)
                    {
                        return mSkill.LocalizeString("RendezvousActive");
                    }
                    else
                    {
                        return mSkill.LocalizeString("RendezvousInactive");
                    }
                }
            }

            public override int Order
            {
                get { return 100; }
            }

            public override bool Allow(SimDescription sim)
            {
                return sim.IsHuman;
            }

            public override KamaSimtraMajorStat Clone()
            {
                return new RendezvousActiveStat();
            }
        }

        public class WhoringPayment : KamaSimtraMajorStat
        {
            public WhoringPayment()
            { }

            protected override string LocalizationKey
            {
                get { return "WhoringPayment"; }
            }

            public override int Count
            {
                get { return mSkill.GetPayment(); }
            }

            public override int Order
            {
                get { return 120; }
            }

            public override bool Allow(SimDescription sim)
            {
                return sim.IsHuman;
            }

            public override KamaSimtraMajorStat Clone()
            {
                return new WhoringPayment();
            }
        }

        public class TotalFundsMade : KamaSimtraMajorStat
        {
            public TotalFundsMade()
            { }

            protected override string LocalizationKey
            {
                get { return "TotalFundsMade"; }
            }

            public override int Count
            {
                get { return mSkill.GetCashMade(); }
            }

            public override int Order
            {
                get { return 130; }
            }

            public override bool Allow(SimDescription sim)
            {
                return sim.IsHuman;
            }

            public override KamaSimtraMajorStat Clone()
            {
                return new TotalFundsMade();
            }
        }

        public class SatisfiedCustomers : KamaSimtraMajorStat
        {
            public SatisfiedCustomers()
            { }

            protected override string LocalizationKey
            {
                get { return "SatisfiedCustomers"; }
            }

            public override int Count
            {
                get { return mSkill.GetSatisfiedCustomers(); }
            }

            public override int Order
            {
                get { return 140; }
            }

            public override bool Allow(SimDescription sim)
            {
                return sim.IsHuman;
            }

            public override KamaSimtraMajorStat Clone()
            {
                return new SatisfiedCustomers();
            }
        }

        public abstract class KamaSimtraMinorStat : MinorStat
        { }
        
        public abstract class KamaSimtraOpportunity : CommonOpportunity
        {
            public KamaSimtraOpportunity()
            { }

            public virtual bool AppliesToRenown
            {
                get { return true; }
            }

            public override bool Allow(SimDescription sim)
            {
                return true;
            }

            public virtual int GetRenown()
            {
                return Settings.mRenownPerOpportunity;
            }

            public virtual int GetPositiveBuffLength()
            {
                return 0;
            }

            public virtual void GetBuffChance(bool completed, ref int positive, ref int negative)
            {
                if (!completed)
                {
                    negative += 5;
                }
            }

            public abstract int GetLikingChange(SimDescription sim);
        }

        public class Womanizer : KamaSimtraOpportunity
        {
            public Womanizer()
            { }

            protected override string LocalizationKey
            {
                get { return "Womanizer"; }
            }

            public override int MinValue
            {
                get { return Settings.mWomanizerMinNotches; }
            }

            public override int CurrentValue
            {
                get { return mSkill.GetWomanizerNotches(); }
            }

            public override int GetRenown()
            {
                return base.GetRenown() * 2;
            }

            public override int GetLikingChange(SimDescription sim)
            {
                if (sim.IsFemale)
                {
                    return -1;
                }

                return 0;
            }

            public override KamaSimtraOpportunity Clone()
            {
                return new Womanizer();
            }
        }

        public class ManEater : KamaSimtraOpportunity
        {
            public ManEater()
            { }

            protected override string LocalizationKey
            {
                get { return "ManEater"; }
            }

            public override int MinValue
            {
                get { return Settings.mManEaterMinNotches; }
            }

            public override int CurrentValue
            {
                get { return mSkill.GetManEaterNotches(); }
            }

            public override int GetRenown()
            {
                return base.GetRenown() * 2;
            }

            public override int GetLikingChange(SimDescription sim)
            {
                if (sim.IsMale)
                {
                    return -1;
                }

                return 0;
            }

            public override KamaSimtraOpportunity Clone()
            {
                return new ManEater();
            }
        }

        public class Cougar : KamaSimtraOpportunity
        {
            public Cougar()
            { }

            protected override string LocalizationKey
            {
                get { return "Cougar"; }
            }

            public override int MinValue
            {
                get { return Settings.mCougarMinNotches; }
            }

            public override int CurrentValue
            {
                get { return mSkill.GetCougarNotches(); }
            }

            public override int GetLikingChange(SimDescription sim)
            {
                if ((sim.IsMale) && (sim.Age < SkillOwner.Age))
                {
                    return 1;
                }

                return 0;
            }

            public override KamaSimtraOpportunity Clone()
            {
                return new Cougar();
            }
        }

        public class GoldDigger : KamaSimtraOpportunity
        {
            public GoldDigger()
            { }

            protected override string LocalizationKey
            {
                get { return "GoldDigger"; }
            }

            public override int MinValue
            {
                get { return Settings.mGoldDiggerMinNotches; }
            }

            public override int CurrentValue
            {
                get { return mSkill.GetGoldDiggerNotches(); }
            }

            public override int GetLikingChange(SimDescription sim)
            {
                if (SkillOwner.Elder) return 0;

                if (sim.Elder)
                {
                    return 1;
                }
                else
                {
                    foreach (SimDescription parent in Relationships.GetParents(sim))
                    {
                        if (parent.Elder)
                        {
                            return -1;
                        }
                    }
                }

                return 0;
            }

            public override KamaSimtraOpportunity Clone()
            {
                return new GoldDigger();
            }
        }

        public class Gigolo : KamaSimtraOpportunity
        {
            public Gigolo()
            { }

            protected override string LocalizationKey
            {
                get { return "Gigolo"; }
            }

            public override int MinValue
            {
                get { return Settings.mGigoloMinNotches; }
            }

            public override int CurrentValue
            {
                get { return mSkill.GetGigoloNotches(); }
            }

            public override bool Allow(SimDescription sim)
            {
                return sim.IsHuman;
            }

            public override int GetRenown()
            {
                return base.GetRenown() * 2;
            }

            public override int GetLikingChange(SimDescription sim)
            {
                if ((mSkill.WhoringActive) && 
                    (ScoringLookup.GetScore("DislikeGigolo", sim) > 0))
                {
                    return -1;
                }

                return 0;
            }

            public override KamaSimtraOpportunity Clone()
            {
                return new Gigolo();
            }
        }

        public class Worldly : KamaSimtraOpportunity
        {
            public Worldly()
            { }

            protected override string LocalizationKey
            {
                get { return "Worldly"; }
            }

            public override int MinValue
            {
                // Used in the Description
                get { return Settings.mWorldlyMinPerWorld; }
            }

            public override bool Completed
            {
                // Overridden to not use MinValue
                get { return (CurrentValue >= 3); }
            }

            public override int CurrentValue
            {
                get { return mSkill.GetWorldlyNotches(); }
            }

            public override int GetRenown()
            {
                return base.GetRenown() * 2;
            }

            public override void GetBuffChance(bool completed, ref int positive, ref int negative)
            {
                if (completed)
                {
                    positive += 10;
                    negative -= 10;
                }
                else
                {
                    base.GetBuffChance(completed, ref positive, ref negative);
                }
            }

            public override int GetLikingChange(SimDescription sim)
            {
                if (ScoringLookup.GetScore("LikeWorldly", sim) > 0)
                {
                    return 1;
                }

                return 0;
            }

            public override KamaSimtraOpportunity Clone()
            {
                return new Worldly();
            }
        }

        public class Prolific : KamaSimtraOpportunity
        {
            public Prolific()
            { }

            protected override string LocalizationKey
            {
                get { return "Prolific"; }
            }

            public override bool IsMajor
            {
                get { return true; }
            }
 
            public override int MinValue
            {
                get { return Settings.mProlificMinNotches; }
            }

            public override int CurrentValue
            {
                get { return mSkill.GetProlificNotches(); }
            }

            public override int GetLikingChange(SimDescription sim)
            {
                return 0;
            }

            public override void GetBuffChance(bool completed, ref int positive, ref int negative)
            {
                if (completed)
                {
                    positive += 30;
                    negative -= 20;
                }
                else
                {
                    base.GetBuffChance(completed, ref positive, ref negative);
                }
            }

            public override KamaSimtraOpportunity Clone()
            {
                return new Prolific();
            }
        }

        public class Experienced : KamaSimtraOpportunity
        {
            public Experienced()
            { }

            protected override string LocalizationKey
            {
                get { return "Experienced"; }
            }

            public override bool IsMajor
            {
                get { return true; }
            }

            public override int MinValue
            {
                get { return Settings.mExperiencedMinPerLocation; }
            }

            public override int CurrentValue
            {
                get { return mSkill.GetExperiencedLocations(); }
            }

            public override bool Completed
            {
                get { return (CurrentValue >= GetOptionedLocations(SkillOwner).Count); }
            }

            public override int GetPositiveBuffLength()
            {
                return 240;
            }

            public override int GetLikingChange(SimDescription sim)
            {
                return 0;
            }

            public override void GetBuffChance(bool completed, ref int positive, ref int negative)
            {
                if (completed)
                {
                    positive += 20;
                    negative -= 10;
                }
                else
                {
                    base.GetBuffChance(completed, ref positive, ref negative);
                }
            }

            public override KamaSimtraOpportunity Clone()
            {
                return new Experienced();
            }
        }

        public class Promiscuous : KamaSimtraOpportunity
        {
            public Promiscuous()
            { }

            protected override string LocalizationKey
            {
                get { return "Promiscuous"; }
            }

            public override int MinValue
            {
                get { return Settings.mPromiscuousMinNotches; }
            }

            public override int CurrentValue
            {
                get { return mSkill.GetPromiscuousNotches(); }
            }

            public override int GetRenown()
            {
                return base.GetRenown() * 2;
            }

            public override int GetLikingChange(SimDescription sim)
            {
                if (!Woohooer.Settings.UsingTraitScoring) return 0;

                if (ScoringLookup.GetScore("Monogamous", sim) > 0)
                {
                    return -1;
                }

                return 0;
            }

            public override void GetBuffChance(bool completed, ref int positive, ref int negative)
            {
                if (completed)
                {
                    positive += 10;
                    negative -= 10;
                }
                else
                {
                    base.GetBuffChance(completed, ref positive, ref negative);
                }
            }

            public override KamaSimtraOpportunity Clone()
            {
                return new Promiscuous();
            }
        }

        public class Exhibitionist : KamaSimtraOpportunity
        {
            public Exhibitionist()
            { }

            protected override string LocalizationKey
            {
                get { return "Exhibitionist"; }
            }

            public override int MinValue
            {
                get { return Settings.mExhibitionistMinNotches; }
            }

            public override int CurrentValue
            {
                get { return mSkill.GetExhibitionistNotches(); }
            }

            public override bool Allow(SimDescription sim)
            {
                return sim.IsHuman;
            }

            public override int GetLikingChange(SimDescription sim)
            {
                int score = ScoringLookup.GetScore("LikeExhibitionist", sim);
                if (score > 0)
                {
                    return 1;
                }
                else if (score < 0)
                {
                    return -1;
                }

                return 0;
            }

            public override void GetBuffChance(bool completed, ref int positive, ref int negative)
            {
                if (completed)
                {
                    positive += 20;
                    negative -= 10;
                }
                else
                {
                    base.GetBuffChance(completed, ref positive, ref negative);
                }
            }

            public override KamaSimtraOpportunity Clone()
            {
                return new Exhibitionist();
            }
        }

        public class Casanova : KamaSimtraOpportunity
        {
            public Casanova()
            { }

            protected override string LocalizationKey
            {
                get { return "Casanova"; }
            }

            public override int MinValue
            {
                get { return Settings.mCasanovaMinNotches; }
            }

            public override int CurrentValue
            {
                get { return mSkill.GetCasanovaNotches(); }
            }

            public override int GetRenown()
            {
                return base.GetRenown() * 3;
            }

            public override int GetLikingChange(SimDescription sim)
            {
                string reason;
                if (CommonSocials.CanGetRomantic(sim, SkillOwner, out reason))
                {
                    return 1;
                }

                return 0;
            }

            public override void GetBuffChance(bool completed, ref int positive, ref int negative)
            {
                if (completed)
                {
                    positive += 20;
                    negative -= 20;
                }
                else
                {
                    base.GetBuffChance(completed, ref positive, ref negative);
                }
            }

            public override KamaSimtraOpportunity Clone()
            {
                return new Casanova();
            }
        }

        public class StarryEyed : KamaSimtraOpportunity
        {
            public StarryEyed()
            { }

            protected override string LocalizationKey
            {
                get { return "StarryEyed"; }
            }

            public override int MinValue
            {
                get { return Settings.mStarryEyedMinNotches; }
            }

            public override int CurrentValue
            {
                get { return mSkill.GetStarryEyedNotches(); }
            }

            public override bool Completed
            {
                get { return mSkill.IsStarryEyed(); }
            }

            public override bool Allow(SimDescription sim)
            {
                return sim.IsHuman;
            }

            public override int GetLikingChange(SimDescription sim)
            {
                if (sim.CelebrityLevel > 0)
                {
                    return 1;
                }

                return 0;
            }

            public override KamaSimtraOpportunity Clone()
            {
                return new StarryEyed();
            }
        }

        public class Journeyman : KamaSimtraOpportunity
        {
            public Journeyman()
            { }

            protected override string LocalizationKey
            {
                get { return "Journeyman"; }
            }

            public override int MinValue
            {
                get { return Settings.mJourneymanMinNotches; }
            }

            public override int CurrentValue
            {
                get { return mSkill.GetJourneymanNotches(); }
            }

            public override bool Allow(SimDescription sim)
            {
                return sim.IsHuman;
            }

            public override int GetLikingChange(SimDescription sim)
            {
                if (sim.CreatedByService != null)
                {
                    return 1;
                }

                return 0;
            }

            public override KamaSimtraOpportunity Clone()
            {
                return new Journeyman();
            }
        }

        public class Occultist : KamaSimtraOpportunity
        {
            public Occultist()
            { }

            protected override string LocalizationKey
            {
                get { return "Occultist"; }
            }

            public override int MinValue
            {
                get { return Settings.mOccultistMinNotches; }
            }

            public override int CurrentValue
            {
                get { return mSkill.GetOccultistNotches(); }
            }

            public override int GetLikingChange(SimDescription sim)
            {
                if ((SkillOwner.OccultManager.HasAnyOccultType()) || (SkillOwner.IsDead))
                {
                    return 0;
                }

                if ((sim.OccultManager.HasAnyOccultType()) || (sim.IsDead) || (sim.Service is GrimReaper))
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }

            public override KamaSimtraOpportunity Clone()
            {
                return new Occultist();
            }
        }

        public class MonsterMasher : KamaSimtraOpportunity
        {
            public MonsterMasher()
            { }

            protected override string LocalizationKey
            {
                get { return "MonsterMasher"; }
            }

            public override int MinValue
            {
                get { return mSkill.GetMonsterMasherMinNotches(); }
            }

            public override int CurrentValue
            {
                get { return mSkill.GetMonsterMasherNotches(); }
            }

            public override int GetLikingChange(SimDescription sim)
            {
                if ((SkillOwner.OccultManager.HasAnyOccultType()) || (SkillOwner.IsDead))
                {
                    return 0;
                }

                if ((sim.OccultManager.HasAnyOccultType()) || (sim.IsDead) || (sim.Service is GrimReaper))
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }

            public override KamaSimtraOpportunity Clone()
            {
                return new MonsterMasher();
            }
        }

        public class GraveRobber : KamaSimtraOpportunity
        {
            public GraveRobber()
            { }

            protected override string LocalizationKey
            {
                get { return "GraveRobber"; }
            }

            public override int MinValue
            {
                get { return mSkill.GetGraveRobberMinNotches(); }
            }

            public override int CurrentValue
            {
                get { return mSkill.GetGraveRobberNotches(); }
            }

            public override int GetLikingChange(SimDescription sim)
            {
                if (sim.IsDead)
                {
                    return 1;
                }
                else if (!sim.OccultManager.HasAnyOccultType())
                {
                    return -1;
                }

                return 0;
            }

            public override KamaSimtraOpportunity Clone()
            {
                return new GraveRobber();
            }
        }

        public class Cheater : KamaSimtraOpportunity
        {
            public Cheater()
            { }

            protected override string LocalizationKey
            {
                get { return "Cheater"; }
            }

            public override int MinValue
            {
                get { return Settings.mCheaterMinNotches; }
            }

            public override int CurrentValue
            {
                get { return mSkill.GetCheaterNotches(); }
            }

            public override bool Allow(SimDescription sim)
            {
                return sim.IsHuman;
            }

            public override int GetLikingChange(SimDescription sim)
            {
                if (sim.Partner != null)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }

            public override void GetBuffChance(bool completed, ref int positive, ref int negative)
            {
                if (completed)
                {
                    positive -= 10;
                    negative += 20;
                }
                else
                {
                    base.GetBuffChance(completed, ref positive, ref negative);
                }
            }

            public override KamaSimtraOpportunity Clone()
            {
                return new Cheater();
            }
        }

        public class Bike : KamaSimtraOpportunity
        {
            public Bike()
            { }

            protected override string LocalizationKey
            {
                get { return "Bike"; }
            }

            public override int MinValue
            {
                get { return Settings.mBikeMinNotches; }
            }

            public override int CurrentValue
            {
                get { return mSkill.GetBikeNotches(); }
            }

            public override bool Allow(SimDescription sim)
            {
                return sim.IsHuman;
            }

            public override int GetLikingChange(SimDescription sim)
            {
                if (sim.Partner != null)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }

            public override void GetBuffChance(bool completed, ref int positive, ref int negative)
            {
                if (completed)
                {
                    positive -= 10;
                    negative += 20;
                }
                else
                {
                    base.GetBuffChance(completed, ref positive, ref negative);
                }
            }

            public override KamaSimtraOpportunity Clone()
            {
                return new Bike();
            }
        }

        public class EasyRider : KamaSimtraOpportunity
        {
            public EasyRider()
            { }

            protected override string LocalizationKey
            {
                get { return "EasyRider"; }
            }

            public override int MinValue
            {
                get { return Settings.mEasyRiderMinNotches; }
            }

            public override int CurrentValue
            {
                get { return mSkill.GetEasyRiderNotches(); }
            }

            public override int GetLikingChange(SimDescription sim)
            {
                int score = ScoringLookup.GetScore("LikeEasyRider", sim);
                if (score > 0)
                {
                    return 1;
                }
                else if (score < 0)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }

            public override void GetBuffChance(bool completed, ref int positive, ref int negative)
            {
                if (completed)
                {
                    positive -= 10;
                    negative += 20;
                }
                else
                {
                    base.GetBuffChance(completed, ref positive, ref negative);
                }
            }

            public override KamaSimtraOpportunity Clone()
            {
                return new EasyRider();
            }
        }

        public class GalaxyOfStars : KamaSimtraOpportunity
        {
            public GalaxyOfStars()
            { }

            protected override string LocalizationKey
            {
                get { return "GalaxyOfStars"; }
            }

            public override int MinValue
            {
                get { return Settings.mGalaxyOfStarsMinNotches; }
            }

            public override int CurrentValue
            {
                get { return mSkill.GetGalaxyOfStarsNotches(); }
            }

            public override bool Allow(SimDescription sim)
            {
                return sim.IsHuman;
            }

            public override int GetLikingChange(SimDescription sim)
            {
                if (sim.CelebrityLevel > 0)
                {
                    return 1;
                }

                return 0;
            }

            public override KamaSimtraOpportunity Clone()
            {
                return new GalaxyOfStars();
            }
        }

        public class Fidelity : KamaSimtraOpportunity
        {
            public Fidelity()
            { }

            protected override string LocalizationKey
            {
                get { return "Fidelity"; }
            }

            public override int MinValue
            {
                get { return Settings.mFidelityMinNotches; }
            }

            public override int CurrentValue
            {
                get { return mSkill.GetFidelityNotches(); }
            }

            public override int GetRenown()
            {
                return base.GetRenown() * 2;
            }

            public override int GetLikingChange(SimDescription sim)
            {
                if (!Woohooer.Settings.UsingTraitScoring) return 0;

                if (ScoringLookup.GetScore("Monogamous", sim) > 0)
                {
                    return 1;
                }

                return 0;
            }

            public override void GetBuffChance(bool completed, ref int positive, ref int negative)
            {
                if (completed)
                {
                    positive += 20;
                    negative -= 20;
                }
                else
                {
                    base.GetBuffChance(completed, ref positive, ref negative);
                }
            }

            public override KamaSimtraOpportunity Clone()
            {
                return new Fidelity();
            }
        }

        public class Precocious : KamaSimtraOpportunity
        {
            public Precocious()
            { }

            protected override string LocalizationKey
            {
                get { return "Precocious"; }
            }

            public override int MinValue
            {
                get { return Settings.mPrecociousMinNotches; }
            }

            public override int CurrentValue
            {
                get { return mSkill.GetPrecociousNotches(); }
            }

            public override int GetLikingChange(SimDescription sim)
            {
                if (sim.Age > SkillOwner.Age)
                {
                    return 1;
                }

                return 0;
            }

            public override int GetPositiveBuffLength()
            {
                return 120;
            }

            public override KamaSimtraOpportunity Clone()
            {
                return new Precocious();
            }
        }

        public class CradleRobber : KamaSimtraOpportunity
        {
            public CradleRobber()
            { }

            protected override string LocalizationKey
            {
                get { return "CradleRobber"; }
            }

            public override int MinValue
            {
                get { return Settings.mCradleRobberMinNotches; }
            }

            public override int CurrentValue
            {
                get { return mSkill.GetCradleRobberNotches(); }
            }

            public override int GetLikingChange(SimDescription sim)
            {
                if (sim.Age < SkillOwner.Age)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }

            public override int GetPositiveBuffLength()
            {
                return 120;
            }

            public override KamaSimtraOpportunity Clone()
            {
                return new CradleRobber();
            }
        }

        public class FreshMeat : KamaSimtraOpportunity
        {
            public FreshMeat()
            { }

            protected override string LocalizationKey
            {
                get { return "FreshMeat"; }
            }

            public override int MinValue
            {
                get { return Settings.mFreshMeatMinNotches; }
            }

            public override int CurrentValue
            {
                get { return mSkill.GetFreshMeatNotches(); }
            }

            public override int GetLikingChange(SimDescription sim)
            {
                if (!sim.OccultManager.HasAnyOccultType())
                {
                    return 1;
                }

                return 0;
            }

            public override void GetBuffChance(bool completed, ref int positive, ref int negative)
            {
                if (completed)
                {
                    positive += 20;
                    negative -= 20;
                }
                else
                {
                    base.GetBuffChance(completed, ref positive, ref negative);
                }
            }

            public override KamaSimtraOpportunity Clone()
            {
                return new FreshMeat();
            }
        }

        public class Risque : KamaSimtraOpportunity
        {
            public Risque()
            { }

            protected override string LocalizationKey
            {
                get { return "Risque"; }
            }

            public override int MinValue
            {
                get { return Settings.mRisqueMinNotches; }
            }

            public override int CurrentValue
            {
                get { return mSkill.GetRisqueNotches(); }
            }

            public override int GetRenown()
            {
                return base.GetRenown() * 2;
            }

            public override int GetLikingChange(SimDescription sim)
            {
                if (!Woohooer.Settings.mUseTraitScoring) return 0;

                int scoring = ScoringLookup.GetScore("LikeRisky", sim);
                if (scoring > 0)
                {
                    return 1;
                }
                else if (scoring < 0)
                {
                    return -1;
                }

                return 0;
            }

            public override KamaSimtraOpportunity Clone()
            {
                return new Risque();
            }
        }

        public class CyberJunkie : KamaSimtraOpportunity
        {
            public CyberJunkie()
            { }

            protected override string LocalizationKey
            {
                get { return "CyberJunkie"; }
            }

            public override int MinValue
            {
                get { return Settings.mCyberJunkieMinNotches; }
            }

            public override int CurrentValue
            {
                get { return mSkill.GetCyberJunkieNotches(); }
            }

            public override bool Allow(SimDescription sim)
            {
                return sim.IsHuman;
            }

            public override void GetBuffChance(bool completed, ref int positive, ref int negative)
            {
                if (completed)
                {
                    negative -= 20;
                }
                else
                {
                    base.GetBuffChance(completed, ref positive, ref negative);
                }
            }

            public override int GetLikingChange(SimDescription sim)
            {
                int scoring = ScoringLookup.GetScore("LikeCyberWoohoo", sim);
                if (scoring > 0)
                {
                    return 1;
                }
                else if (scoring < 0)
                {
                    return -1;
                }

                return 0;
            }

            public override KamaSimtraOpportunity Clone()
            {
                return new CyberJunkie();
            }
        }
    }
}
