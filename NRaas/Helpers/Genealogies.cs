using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.CustomContent;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;

namespace NRaas.CommonSpace.Helpers
{
    public class Genealogies
    {
        public static bool IsOfDepth(Genealogy firstGenealogy, int maximum)
        {
            return (GetFamilyLevel(firstGenealogy, maximum) >= maximum);
        }

        public static int GetFamilyLevel(IGenealogy firstGenealogy)
        {
            return GetFamilyLevel(firstGenealogy, int.MaxValue);
        }
        private static int GetFamilyLevel(IGenealogy firstGenealogy, int maximum)
        {
            if (firstGenealogy == null) return 0;

            List<Pair<IGenealogy, int>> list = new List<Pair<IGenealogy, int>>();
            list.Add(new Pair<IGenealogy, int>(firstGenealogy, 0));

            int maxDepth = 0;

            int index = 0;
            while (index < list.Count)
            {
                Pair<IGenealogy, int> gene = list[index];
                index++;

                if (maxDepth < gene.Second)
                {
                    maxDepth = gene.Second;
                }

                if (maxDepth >= maximum)
                {
                    return maxDepth;
                }

                if (gene.First.IChildren != null)
                {
                    foreach (IGenealogy child in gene.First.IChildren)
                    {
                        if (child == null) continue;

                        list.Add(new Pair<IGenealogy, int>(child, gene.Second + 1));
                    }
                }
            }

            return maxDepth;
        }
    }
}

