using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.SimIFace;
using System;

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
			}
		}
	}
}