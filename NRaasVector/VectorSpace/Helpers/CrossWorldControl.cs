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
    public class CrossWorldControl : ITransition
    {
        static Dictionary<string, List<DiseaseVector>> sDiseases = new Dictionary<string, List<DiseaseVector>>();

        public void Store(SimDescription sim)
        {
            sDiseases[sim.FullName] = new List<DiseaseVector>(Vector.Settings.GetVectors(sim));
        }

        public void Restore(SimDescription sim)
        {
            List<DiseaseVector> vectors;
            if (!sDiseases.TryGetValue(sim.FullName, out vectors)) return;

            Vector.Settings.ClearVectors(sim);
            foreach(DiseaseVector vector in vectors)
            {
                Vector.Settings.AddVector(sim, vector);
            }
        }
    }
}
