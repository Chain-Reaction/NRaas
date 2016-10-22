using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.SimIFace;
using System;
using System.Reflection;

namespace NRaas.OverwatchSpace.Loadup
{
	public class FixComboRabbitHoleMetaAds : Common.IDelayedWorldLoadFinished, Common.IExitBuildBuy
	{
		public void OnDelayedWorldLoadFinished()
		{
			foreach(ComboRabbitHole comboRH in Sims3.Gameplay.Queries.GetObjects<ComboRabbitHole>())
			{
				AddContainedRabbitHolesToLot (comboRH);
			}
		}

		public void OnExitBuildBuy(Lot lot)
		{
			foreach(ComboRabbitHole comboRH in lot.GetObjects<ComboRabbitHole>())
			{
				AddContainedRabbitHolesToLot (comboRH);
			}
		}

		public static void AddContainedRabbitHolesToLot (ComboRabbitHole comboRH)
		{
			foreach(RabbitHole rH in comboRH.ContainedRabbitholes.Keys)
			{
				if (rH.LotCurrent == comboRH.LotCurrent)
				{
					return;
				}
				LotManager.AddObjectToLot (rH, rH.ObjectId, comboRH.LotCurrent.LotId, 0, 0, null);
				rH.AddToWorld ();
				comboRH.MetaAds.AddRange (rH.MetaAds);
				rH.MetaAds.Clear ();

                if (rH is ITheatre)
                {
                    MetaAutonomyTuning tuning = MetaAutonomyManager.GetTuning(MetaAutonomyVenueType.Theatre);
                    if (tuning != null)
                    {
                        MethodInfo info = Type.GetType("NRaas.OverwatchSpace.Loadup.FixComboRabbitHoleMetaAds,NRaasOverwatch").GetMethod("IsShowing");
                        tuning.DesiredSimFunction = info;
                    }
                }
			}
		}

        public static bool IsShowing(RabbitHole venue, float hours24)
        {
            Theatre theatre = venue as Theatre;
            if (theatre == null)
            {
                ComboCriminalTheater theatre2 = venue as ComboCriminalTheater;
                if (theatre2 == null)
                {
                    ComboStadiumTheatre theatre3 = venue as ComboStadiumTheatre;

                    if (theatre3 == null)
                    {
                        return false;
                    }

                    theatre = theatre3.GetTheatre();
                }

                theatre = theatre2.GetTheatre();
            }

            if (theatre == null)
            {
                return false;
            }            

            return theatre.IsOpenAt(hours24);
        }
	}
}