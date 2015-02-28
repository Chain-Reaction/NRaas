using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects.Counters;
using Sims3.Gameplay.Skills;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.OverwatchSpace.Loadup
{
	public class BarMoonlightingListener : Common.IWorldLoadFinished
	{
		public void OnWorldLoadFinished()
		{
			new Common.ImmediateEventListener (EventTypeId.kMadeMoneyBartending, OnStopBartending);
		}

		public static void OnStopBartending(Event e)
		{
			IncrementalEvent iE = e as IncrementalEvent;
			if (iE != null)
			{
				IActor actor = iE.Actor;
				if (actor != null && !(actor.InteractionQueue.GetCurrentInteraction () is BarAdvanced.MakeDrink))
				{
					actor.ModifyFunds ((int)iE.Increment);
				}
			}
		}
	}
}

