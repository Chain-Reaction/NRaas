using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Objects.RabbitHoles;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Utilities;
using Sims3.UI;
using System;

namespace NRaas.OverwatchSpace.Loadup
{
	public class FixComboRabbitHoleOpportunities : Common.IWorldLoadFinished
	{
		public void OnWorldLoadFinished()
		{
			new Common.DelayedEventListener (EventTypeId.kOpportunityTracked, OnAcceptedComboRabbitHoleOpportunity);

			//Excessive?
			/*foreach(Sim sim in LotManager.Actors)
			{
				OpportunityManager oppManager = sim.OpportunityManager;
				if (oppManager != null)
				{
					foreach (Opportunity opp in oppManager.List)
					{
						FixComboRabbitHoleOpportunity (opp);
					}
				}
			}*/
		}

		public static void FixComboRabbitHoleOpportunity (Opportunity opp)
		{
			ComboRabbitHole target = opp.TargetObject as ComboRabbitHole;
			if (target != null && opp.mSharedData.mTargetInteractionName == null)
			{
				RabbitHoleType type;
				if (ParserFunctions.TryParseEnum<RabbitHoleType> (opp.TargetData, out type, RabbitHoleType.None))
				{
					foreach (RabbitHole r in target.ContainedRabbitholes.Keys)
					{
						if (r.Guid == type)
						{
							opp.TargetObject = r;
							if (opp.CompletionListener != null)
							{
								opp.mCompletionListener.SetTargetObject (r);
							}
							return;
						}
					}
				}
			}
		}

		public static void OnAcceptedComboRabbitHoleOpportunity(Event e)
		{
			OpportunityEvent opportunityEvent = e as OpportunityEvent;
			if (opportunityEvent != null)
			{
				FixComboRabbitHoleOpportunity (opportunityEvent.Opportunity);
			}
		}
	}
}