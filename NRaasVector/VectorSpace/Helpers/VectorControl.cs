using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using NRaas.CommonSpace.Replacers;
using NRaas.VectorSpace.Booters;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Rewards;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;

namespace NRaas.VectorSpace.Helpers
{
    public class VectorControl : Common.IWorldLoadFinished
    {
        public enum Virulence : uint
        {
            Inert = 0x00,
            Room = 0x01,
            Social = 0x02,
            Woohoo = 0x04,
            Fight = 0x08,
            Outdoors = 0x10,
        }

        public enum StageType : uint
        {
            None = 0x0,
            Contagious = 0x0001,
            ShowingSigns = 0x0002,
            Inoculated = 0x0004,
            Mutate = 0x0008,
            Remission = 0x0010,
            Resisted = 0x0020,
        }

        static Dictionary<ulong, long> sLastUsedObjectCheck = new Dictionary<ulong, long>();

        public void OnWorldLoadFinished()
        {
            Dictionary<ulong, List<SimDescription>> sims = SimListing.AllSims<SimDescription>(null, true, false);

            List<ulong> remove = new List<ulong>();

            foreach (KeyValuePair<ulong,List<DiseaseVector>> vectors in Vector.Settings.AllVectors)
            {
                if (sims.ContainsKey(vectors.Key))
                {
                    for (int i = vectors.Value.Count - 1; i >= 0; i--)
                    {
                        DiseaseVector vector = vectors.Value[i];

                        if (!vector.OnLoadFixup())
                        {
                            vectors.Value.RemoveAt(i);
                        }
                    }
                }
                else
                {
                    remove.Add(vectors.Key);
                }
            }

            foreach (ulong sim in remove)
            {
                Vector.Settings.RemoveSim(sim);
            }

            Dictionary<string, ResistanceBooter.Data> resistances = new Dictionary<string, ResistanceBooter.Data>();

            foreach (VectorBooter.Data vector in VectorBooter.Vectors)
            {
                vector.GetResistances(resistances);
            }

            Dictionary<EventTypeId, bool> events = new Dictionary<EventTypeId, bool>();

            foreach (ResistanceBooter.Data resistance in resistances.Values)
            {
                resistance.GetEvents(events);
            }

            foreach (EventTypeId id in events.Keys)
            {
                new Common.DelayedEventListener(id, OnResistance);
            }

            // Must be immediate
            new Common.ImmediateEventListener(EventTypeId.kUsedObect, OnUsedObject);

            new Common.DelayedEventListener(EventTypeId.kTraitGained, OnTraitGained);

            new Common.DelayedEventListener(EventTypeId.kWooHooed, OnWoohoo);
            new Common.DelayedEventListener(EventTypeId.kSocialInteraction, OnSocial);
            new Common.DelayedEventListener(EventTypeId.kRoomChanged, OnRoomChanged);
            new Common.DelayedEventListener(EventTypeId.kFought, OnFought);

            new Common.DelayedEventListener(EventTypeId.kBabyToddlerSnuggle, OnInoculate);
            new Common.DelayedEventListener(EventTypeId.kHadBaby, OnBirth);

            new Common.AlarmTask(30, TimeUnit.Minutes, OnVectorCheck, 30, TimeUnit.Minutes);
        }

        protected static void OnTraitGained(Event e)
        {
            TraitGainedEvent traitEvent = e as TraitGainedEvent;
            if (traitEvent == null) return;

            if (traitEvent.TraitName == TraitNames.Simmunity)
            {
                Sim actor = e.Actor as Sim;
                if (actor == null) return;

                foreach (DiseaseVector vector in Vector.Settings.GetVectors(actor))
                {
                    vector.Inoculate(0, false);
                }
            }
        }

        protected static void OnBirth(Event e)
        {
            PregnancyEvent pregEvent = e as PregnancyEvent;
            if (pregEvent == null) return;

            Sim actor = e.Actor as Sim;
            if (actor == null) return;

            // Means of differentiating between parents
            if (!actor.SimDescription.IsPregnant) return;

            foreach (Sim child in pregEvent.Babies)
            {
                Inoculate(child, actor);
            }
        }

        protected static void OnInoculate(Event e)
        {
            Sim actor = e.Actor as Sim;
            if (actor == null) return;

            Sim target = e.TargetObject as Sim;
            if (target == null) return;

            Inoculate(target, actor);
        }

        protected static void Inoculate(Sim target, Sim source)
        {
            string vectors = null;

            foreach (DiseaseVector vector in Vector.Settings.GetVectors(source))
            {
                if (!vector.IsInoculated) continue;

                if (!vector.PaidInoculation) continue;

                if (Inoculate(target.SimDescription, vector, false, false))
                {
                    vectors += Common.NewLine + " " + vector.GetLocalizedName(target.IsFemale);
                }
            }

            if ((SimTypes.IsSelectable(target)) || (SimTypes.IsSelectable(source)))
            {
                if (!string.IsNullOrEmpty(vectors))
                {
                    Common.Notify(target, Common.Localize("Inoculate:Success", target.IsFemale, new object[] { target }) + vectors);
                }
            }
        }
        public static bool Inoculate(SimDescription sim, VectorBooter.Data vector, bool paid, bool testExisting)
        {
            DiseaseVector inoculate = new DiseaseVector(vector, Vector.Settings.GetCurrentStrain(vector));
            inoculate.Inoculate(vector.InoculationStrain, paid);

            return Inoculate(sim, inoculate, paid, testExisting);
        }
        public static bool Inoculate(SimDescription sim, DiseaseVector vector, bool paid, bool testExisting)
        {
            if (vector.InoculationCost <= 0) return false;

            DiseaseVector newVector = Vector.Settings.GetVector(sim, vector.Guid);
            if (newVector == null)
            {
                newVector = new DiseaseVector(vector, sim);
                newVector.Inoculate(vector.InoculationStrain, paid);

                Vector.Settings.AddVector(sim, newVector);
                return true;
            }
            else
            {
                if (testExisting)
                {
                    if (newVector.IsInoculationUpToDate) return false;
                }

                if (!newVector.CanInoculate) return false;

                return newVector.Inoculate(vector.InoculationStrain, paid);
            }
        }

        protected static void OnUsedObject(Event e)
        {
            Sim actor = e.Actor as Sim;
            if (actor == null) return;

            Sim target = e.TargetObject as Sim;
            if (target == null) return;

            if (actor.InteractionQueue == null) return;

            if (actor.CurrentInteraction is SocialInteractionB)
            {
                Sim.ZombifiedSocials social = target.CurrentInteraction as Sim.ZombifiedSocials;
                if (social != null)
                {
                    long lastTime;
                    if (sLastUsedObjectCheck.TryGetValue(actor.SimDescription.SimDescriptionId, out lastTime))
                    {
                        if (SimClock.CurrentTicks < (lastTime + SimClock.kSimulatorTicksPerSimMinute * 20)) return;
                    }

                    OnSocial(new SocialEvent(EventTypeId.kSocialInteraction, target, actor, "Brains", false, true, true, Sims3.Gameplay.Socializing.CommodityTypes.Creepy));

                    sLastUsedObjectCheck[actor.SimDescription.SimDescriptionId] = SimClock.CurrentTicks;
                }
            }
        }

        protected static void OnResistance(Event e)
        {
            Sim actor = e.Actor as Sim;
            if (actor == null) return;

            foreach (DiseaseVector vector in Vector.Settings.GetVectors(actor))
            {
                vector.Resist(e);
            }
        }

        protected static void OnFought(Event e)
        {
            Sim actorSim = e.Actor as Sim;
            if (actorSim == null) return;

            Sim targetSim = e.TargetObject as Sim;
            if (targetSim == null) return;

            if (actorSim.LotCurrent != targetSim.LotCurrent) return;

            SimDescription actor = actorSim.SimDescription;
            SimDescription target = targetSim.SimDescription;

            foreach (DiseaseVector vector in Vector.Settings.GetVectors(actor))
            {
                if (vector.Infect(target, actor, Virulence.Fight, e))
                {
                    ScoringLog.sLog.IncStat(vector.UnlocalizedName + " Success Fight");
                }
            }
        }

        protected static void OnWoohoo(Event e)
        {
            WooHooEvent wEvent = e as WooHooEvent;
            if (wEvent == null) return;

            Sim actorSim = wEvent.Actor as Sim;
            if (actorSim == null) return;

            Sim targetSim = wEvent.TargetObject as Sim;
            if (targetSim == null) return;

            if (actorSim.LotCurrent != targetSim.LotCurrent) return;

            SimDescription actor = actorSim.SimDescription;
            SimDescription target = targetSim.SimDescription;

            foreach (DiseaseVector vector in Vector.Settings.GetVectors(actor))
            {
                if (vector.Infect(target, actor, Virulence.Woohoo, e))
                {
                    ScoringLog.sLog.IncStat(vector.UnlocalizedName + " Success Woohoo");
                }
            }
        }

        protected static void OnSocial(Event e)
        {
            SocialEvent socialEvent = e as SocialEvent;
            if (socialEvent == null) return;

            Sim actorSim = socialEvent.Actor as Sim;
            if (actorSim == null) return;

            Sim targetSim = socialEvent.TargetObject as Sim;
            if (targetSim == null) return;

            if (actorSim.LotCurrent != targetSim.LotCurrent) return;

            if (actorSim.RoomId != targetSim.RoomId) return;

            if (socialEvent.SocialName == "Vaccinate")
            {
                if (socialEvent.WasAccepted)
                {
                    Inoculate(targetSim, actorSim);
                }
            }
            else
            {
                SimDescription actor = actorSim.SimDescription;
                SimDescription target = targetSim.SimDescription;

                foreach (DiseaseVector vector in Vector.Settings.GetVectors(actor))
                {
                    if (vector.Infect(target, actor, Virulence.Social, e))
                    {
                        ScoringLog.sLog.IncStat(vector.UnlocalizedName + " Success Social");
                    }
                }
            }
        }

        protected static void OnVectorCheck()
        {
            foreach (SimDescription sim in Household.EverySimDescription())
            {
                VectorCheck(sim, null, true);
            }
        }

        protected static void OnRoomChanged(Event e)
        {
            Sim sim = e.Actor as Sim;
            if (sim == null) return;

            VectorCheck(sim.SimDescription, e, false);
        }

        protected static void VectorCheck(SimDescription sim, Event e, bool testVector)
        {
            try
            {
                bool invalidRoom = false;

                RabbitHole hole = null;

                Virulence roomVirulence = Virulence.Room;

                Sim createdSim = sim.CreatedSim;
                if (createdSim != null)
                {
                    if ((createdSim.LotCurrent == null) || (createdSim.LotCurrent.IsWorldLot))
                    {
                        invalidRoom = true;
                    }

                    if (createdSim.CurrentInteraction != null)
                    {
                        hole = createdSim.CurrentInteraction.Target as RabbitHole;
                    }

                    if (createdSim.IsOutside)
                    {
                        roomVirulence = Virulence.Outdoors;
                    }
                }
                else
                {
                    invalidRoom = true;
                }

                if ((invalidRoom) && (!testVector)) return;

                IEnumerable<Sim> lotSims = null;

                List<SimDescription> coworkers = null;

                bool showingSigns = false;

                foreach (DiseaseVector vector in Vector.Settings.GetVectors(sim))
                {
                    if ((!testVector) || (vector.Process(sim)))
                    {
                        if (vector.ShowingSigns)
                        {
                            showingSigns = true;
                        }

                        if (invalidRoom) continue;

                        if (lotSims == null)
                        {
                            lotSims = createdSim.LotCurrent.GetSims();

                            if ((hole != null) && (vector.IsSocial))
                            {
                                if ((sim.Occupation != null) && (createdSim.CurrentInteraction is WorkInRabbitHole))
                                {
                                    coworkers = new List<SimDescription>(createdSim.Occupation.Coworkers);

                                    if ((coworkers != null) && (createdSim.Occupation.Boss != null))
                                    {
                                        coworkers.Add(createdSim.Occupation.Boss);
                                    }
                                }
                                else if ((createdSim.School != null) && (createdSim.CurrentInteraction is GoToSchoolInRabbitHole))
                                {
                                    coworkers = new List<SimDescription>(createdSim.School.Coworkers);
                                }
                            }
                        }

                        if (vector.IsSocial)
                        {
                            if (coworkers != null)
                            {
                                foreach (SimDescription other in coworkers)
                                {
                                    if ((other.CreatedSim != null) && (other.CreatedSim.LotCurrent != createdSim.LotCurrent)) continue;

                                    if (vector.Infect(other, sim, Virulence.Social, e))
                                    {
                                        ScoringLog.sLog.IncStat(vector.UnlocalizedName + " Success Coworkers");
                                    }
                                }
                            }
                        }

                        if (vector.HasVirulence(roomVirulence))
                        {
                            foreach (Sim other in lotSims)
                            {
                                if (other == createdSim) continue;

                                if (hole != null)
                                {
                                    RabbitHole otherHole = null;
                                    if (other.CurrentInteraction != null)
                                    {
                                        otherHole = other.CurrentInteraction.Target as RabbitHole;
                                    }

                                    if (otherHole == hole)
                                    {
                                        if (vector.Infect(other.SimDescription, sim, Virulence.Room, e))
                                        {
                                            ScoringLog.sLog.IncStat(vector.UnlocalizedName + " Success Rabbithole");
                                        }

                                        continue;
                                    }
                                }

                                if (other.RoomId == createdSim.RoomId)
                                {
                                    if (vector.Infect(other.SimDescription, sim, roomVirulence, e))
                                    {
                                        ScoringLog.sLog.IncStat(vector.UnlocalizedName + " Success " + roomVirulence);
                                    }
                                }
                            }
                        }
                    }
                }

                if ((!showingSigns) && (createdSim != null) && (createdSim.BuffManager != null))
                {
                    createdSim.BuffManager.RemoveElement(BuffNames.Germy);
                }
            }
            catch (Exception exception)
            {
                Common.Exception(sim, exception);
            }
        }
    }
}
