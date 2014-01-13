using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.ChildrenObjects;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Pools;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas.RelativitySpace.Helpers
{
    public class PriorValues
    {
        public static bool sFactorChanged = true;

        public int mPreviousSpeed = -1;

        long mCount = 0;

        Dictionary<Sim, PriorCommodities> mCommodities = new Dictionary<Sim, PriorCommodities>();

        public PriorValues()
        { }

        public MotiveDelta CloneDelta(MotiveDelta delta, float decay)
        {
            MotiveDelta result = new MotiveDelta(delta.Motive, decay, delta.EnabledBelowZero, delta.EnabledAboveZero);

            // replaced with "decay"
            //result.ChangePerHour = delta.ChangePerHour;
            result.Enabled = delta.Enabled;
            result.Multiplier = delta.Multiplier;

            return result;
        }

        public void ApplyCommodityGains()
        {
            mCount++;
            if ((mCount > Relativity.Settings.mCyclesPerUpdate) || (mCommodities.Count == 0))
            {
                mCount = 0;

                Relativity.Logger.Append("Relative Speed: " + PersistedSettings.sRelativeFactor);

                foreach (Sim sim in LotManager.Actors)
                {
                    try
                    {
                        if (sim.InteractionQueue == null) continue;

                        InteractionInstance head = sim.InteractionQueue.GetHeadInteraction();
                        if (head == null) continue;

                        PriorCommodities commodities = null;
                        if (mCommodities.TryGetValue(sim, out commodities))
                        {
                            if ((object.ReferenceEquals(commodities.mInteraction, head)) && (head.mActiveCommodityUpdates.Count == commodities.ActiveCommodities))
                            {
                                continue;
                            }

                            mCommodities.Remove(sim);
                        }

                        if (head.mActiveCommodityUpdates.Count == 0) continue;

                        /*
                        Relativity.Logger.Append(Common.NewLine + head.InstanceActor.FullName);
                        Relativity.Logger.Append(Common.NewLine + head.InteractionDefinition.GetType().ToString());
                        */

                        commodities = new PriorCommodities(head);
                        mCommodities.Add(sim, commodities);

                        if (head.mSkillMultipliers != null)
                        {
                            commodities.EndSkillCommodityUpdates(head);

                            commodities.mSkillMultipliers = head.mSkillMultipliers;
                            head.mSkillMultipliers = new Dictionary<SkillNames, float>();

                            commodities.BeginSkillCommodityUpdates(head, commodities.mSkillMultipliers, true, true);
                        }

                        commodities.mMotives = head.mMotiveDeltas;
                        head.mMotiveDeltas = new List<MotiveDelta>();

                        foreach (MotiveDelta delta in commodities.mMotives)
                        {
                            if (delta == null) continue;

                            float adjusted = delta.ChangePerHour;
                            float original = adjusted;

                            if (adjusted > 0)
                            {
                                adjusted *= Relativity.Settings.GetMotiveFactor(new MotiveKey(sim.SimDescription, delta.Motive), true);
                            }
                            else
                            {
                                adjusted *= Relativity.Settings.GetMotiveDecayFactor(new MotiveKey(sim.SimDescription, delta.Motive));
                            }

                            Relativity.Logger.Append(delta.Motive + ": " + original + " -> " + adjusted);

                            head.mMotiveDeltas.Add(CloneDelta(delta, adjusted));
                        }
                    }
                    catch (Exception e)
                    {
                        Common.Exception(sim, e);
                    }
                }
            }
        }

        public void RevertCommodityGains()
        {
            foreach (KeyValuePair<Sim, PriorCommodities> sim in mCommodities)
            {
                try
                {
                    if (sim.Key.InteractionQueue == null) continue;

                    InteractionInstance head = sim.Key.InteractionQueue.GetHeadInteraction();
                    if (head == null) continue;

                    if (sim.Value.mInteraction != head) continue;

                    head.mMotiveDeltas = sim.Value.mMotives;

                    sim.Value.EndSkillCommodityUpdates(head);

                    head.mSkillMultipliers = sim.Value.mSkillMultipliers;

                    sim.Value.BeginSkillCommodityUpdates(head, sim.Value.mSkillMultipliers, false, false);
                }
                catch (Exception e)
                {
                    Common.Exception(sim.Key, e);
                }
            }

            mCommodities.Clear();
        }

        protected class PriorCommodities
        {
            public InteractionInstance mInteraction;

            public Dictionary<SkillNames, float> mSkillMultipliers;

            public List<MotiveDelta> mMotives;

            Dictionary<CommodityKind,CommodityChange> mActiveCommodities = new Dictionary<CommodityKind,CommodityChange>();

            public PriorCommodities(InteractionInstance interaction)
            {
                mInteraction = interaction;
                foreach (CommodityChange change in interaction.mActiveCommodityChanges)
                {
                    if (change == null) continue;

                    mActiveCommodities[change.Commodity] = change;
                }
            }

            public int ActiveCommodities
            {
                get
                {
                    return mActiveCommodities.Count;
                }
            }

            public void BeginSkillCommodityUpdates(InteractionInstance ths, Dictionary<SkillNames, float> multipliers, bool applyFactor, bool log)
            {
                foreach (CommodityChange change in mActiveCommodities.Values)
                {
                    try
                    {
                        if (!CommodityTest.IsSkill(change.Commodity)) continue;

                        float multiplier = 1f;

                        SkillNames skill;
                        if (SkillManager.SkillCommodityMap.TryGetValue(change.Commodity, out skill))
                        {
                            if (!multipliers.TryGetValue(skill, out multiplier))
                            {
                                multiplier = 1f;
                            }
                        }

                        float original = multiplier;

                        if (applyFactor)
                        {
                            multiplier *= Relativity.Settings.GetDynamicSkillFactor(skill);
                        }

                        if (log)
                        {
                            Relativity.Logger.Append(Common.NewLine + ths.InstanceActor.FullName + " " + skill + ": " + original + " -> " + multiplier);
                        }

                        ths.BeginCommodityUpdate(change, multiplier);
                    }
                    catch (Exception e)
                    {
                        Common.Exception("Commodity: " + ((change != null) ? change.Commodity.ToString () : "<Null>"), e);
                    }
                }
            }

            public void EndSkillCommodityUpdates(InteractionInstance ths)
            {
                foreach (CommodityChange change in ths.mActiveCommodityChanges)
                {
                    if (!CommodityTest.IsSkill(change.Commodity)) continue;

                    ths.EndCommodityUpdate(change, false);
                }
            }
        }
    }
}
