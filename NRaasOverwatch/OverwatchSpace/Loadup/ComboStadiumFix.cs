using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Skills;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.OverwatchSpace.Loadup
{
	public class ComboStadiumFix : Common.IWorldLoadFinished
	{
		public void OnWorldLoadFinished()
		{
			new Common.ImmediateEventListener (EventTypeId.kAttendedProSportsGame, OnAttendedProSportsGame);
			new Common.ImmediateEventListener (EventTypeId.kExitedRabbithole, OnExitComboStadium);
		}

		public static void OnAttendedProSportsGame(Event e)
		{
			RabbitHole rabbitHole = e.TargetObject as RabbitHole;
			if (rabbitHole != null && rabbitHole.RabbitHoleProxy != rabbitHole)
			{
				rabbitHole.AddToUseList (e.Actor as Sim);
			}
		}

		public static void OnExitComboStadium(Event e)
		{
			ComboRabbitHole comboRH = e.TargetObject as ComboRabbitHole;
			if (comboRH != null)
			{
				foreach(RabbitHole rabbitHole in comboRH.ContainedRabbitholes.Keys)
				{
					if (rabbitHole is IStadium)
					{
						Sim a = e.Actor as Sim;
						if (rabbitHole.ActorsUsingMe.Contains(a))
						{
							rabbitHole.RemoveFromUseList (a);
						}
						return;
					}
				}
			}
		}
	}
}

