using NRaas.VectorSpace.Booters;
using NRaas.CommonSpace.Helpers;
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
    public class OutbreakControl : Common.IWorldLoadFinished
    {
        public void OnWorldLoadFinished()
        {
            new Common.AlarmTask(1, TimeUnit.Hours, OnPerform, 6, TimeUnit.Hours);

            foreach (VectorBooter.Data vector in VectorBooter.Vectors)
            {
                vector.RegisterInstigators();
            }
        }

        public class InstigatorListener : Common.DelayedEventListener
        {
            VectorBooter.Data mVector;

            InstigatorBooter.Data mInstigator;

            public InstigatorListener(EventTypeId id, VectorBooter.Data vector, InstigatorBooter.Data instigator)
                : base(id, null)
            {
                mVector = vector;
                mInstigator = instigator;
            }

            protected override void OnPerform(Event e)
            {
                if (!mVector.CanOutbreak) return;

                Sim actor = e.Actor as Sim;
                if (actor == null) return;

                if (!mInstigator.Perform(e)) return;

                DiseaseVector vector = new DiseaseVector(mVector, Vector.Settings.GetNewStrain(mVector));

                if (vector.Infect(actor.SimDescription, true))
                {
                    mInstigator.ShowStory(actor.SimDescription);

                    ScoringLog.sLog.IncStat(vector.UnlocalizedName + " Success " + mInstigator.Guid);
                }
            }
        }

        protected static void OnPerform()
        {
            Dictionary<ulong, List<SimDescription>> allSims = null;

            foreach (VectorBooter.Data vector in VectorBooter.Vectors)
            {
                if (!vector.IsEnabled) continue;

                if (vector.HasInstigators) continue;

                if (!vector.CanOutbreak) continue;

                if (!Vector.Settings.IsAutomated(vector.Guid)) continue;

                bool active = false;
                foreach (KeyValuePair<ulong, List<DiseaseVector>> vectors in Vector.Settings.AllVectors)
                {
                    if (allSims == null)
                    {
                        allSims = SimListing.AllSims<SimDescription>(null, false);
                    }

                    List<SimDescription> sims;
                    if (!allSims.TryGetValue(vectors.Key, out sims)) continue;

                    if (SimTypes.IsDead(sims[0])) continue;

                    foreach (DiseaseVector subVector in vectors.Value)
                    {
                        if (subVector.Guid != vector.Guid) continue;

                        if (subVector.IsActive)
                        {
                            active = true;
                            break;
                        }
                    }

                    if (active)
                    {
                        break;
                    }
                }

                if (active)
                {
                    ScoringLog.sLog.IncStat(vector.Guid + " Active In Town");
                }
                else
                {
                    StartOutbreak(vector, false);
                }
            }
        }

        public static void ShowNotice(SimDescription sim, DiseaseVector vector, string prefix)
        {
            if (Vector.Settings.mOutbreakShowNotices)
            {
                Common.Notify(sim.CreatedSim, prefix + Common.Localize("Outbreak:Success", sim.IsFemale, new object[] { sim, vector.GetLocalizedName(sim.IsFemale) }));
            }
        }

        public static void StartOutbreak(VectorBooter.Data vector, bool verbose)
        {
            DiseaseVector disease = new DiseaseVector(vector, Vector.Settings.GetNewStrain(vector));

            ScoringLog.sLog.IncStat(disease.UnlocalizedName + " Attempt Outbreak");

            List<Sim> sims = new List<Sim>(LotManager.Actors);

            if (sims.Count > 0)
            {
                int count = 0;
                while ((count < Vector.Settings.mNumPatientZero) && (sims.Count > 0))
                {
                    Sim choice = RandomUtil.GetRandomObjectFromList(sims);
                    sims.Remove(choice);

                    if (choice.SimDescription.ToddlerOrBelow) continue;

                    if (!Vector.Settings.mOutbreakAllowActive)
                    {
                        if (choice.Household == Household.ActiveHousehold) continue;
                    }

                    if (disease.Infect(choice.SimDescription, true))
                    {
                        count++;

                        if (verbose)
                        {
                            ShowNotice(choice.SimDescription, disease, Common.kDebugging ? "Outbreak" : "");
                        }

                        ScoringLog.sLog.IncStat(disease.UnlocalizedName + " Outbreak");
                    }
                }
            }
        }
    }
}
