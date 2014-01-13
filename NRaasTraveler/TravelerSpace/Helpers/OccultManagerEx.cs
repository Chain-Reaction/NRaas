using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.Visa;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.GameEntry;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace NRaas.TravelerSpace.Helpers
{
    public class OccultManagerEx
    {
        public static void MergeOccults(OccultManager ths, OccultManager source)
        {
            Common.StringBuilder msg = new Common.StringBuilder("MergeOccults " + ths.mOwnerDescription.FullName + Common.NewLine);
            Traveler.InsanityWriteLog(msg);

            foreach (OccultTypes type in Enum.GetValues(typeof(OccultTypes)))
            {
                try
                {
                    switch (type)
                    {
                        case OccultTypes.None:
                        case OccultTypes.Ghost:
                            break;

                        default:
                            if ((source.HasOccultType(type)) && (!ths.HasOccultType(type)))
                            {
                                msg += Common.NewLine + "A " + type;
                                Traveler.InsanityWriteLog(msg);

                                ths.MergeOccult(type);
                            }

                            if ((!source.HasOccultType(type)) && (ths.HasOccultType(type)))
                            {
                                msg += Common.NewLine + "B " + type;
                                Traveler.InsanityWriteLog(msg);

                                OccultTypeHelper.Remove(ths.mOwnerDescription, type, false);
                            }
                            break;
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(ths.mOwnerDescription, null, "Type: " + type, e);
                }
            }
        }
    }
}