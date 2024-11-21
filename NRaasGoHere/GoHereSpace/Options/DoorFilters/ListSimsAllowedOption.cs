using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interfaces;
using Sims3.SimIFace;

namespace NRaas.GoHereSpace.Options.DoorFilters
{
	public class ListSimsAllowedOption : OperationSettingOption<GameObject>, IDoorOption, IInteractionOptionItem<IActor, GameObject, GameHitParameters<GameObject>>, ICommonOptionItem
	{
		protected class SimSelection : ProtoSimSelection<SimDescription>
		{
			private static GameObject mTarget;

			private SimSelection(string title, string subTitle, SimDescription me)
				: base(title, subTitle, me, true, true)
			{
			}

			public static SimSelection Create(string title, string subTitle, SimDescription me, GameObject target)
			{
				mTarget = target;
				SimSelection simSelection = new SimSelection(title, subTitle, me);
				bool canceled = false;
				simSelection.FilterSims(null, null, false, out canceled);
				return simSelection;
			}

			protected override bool Allow(SimDescription sim)
			{
				if (mTarget == null)
				{
					return false;
				}
				if (sim.CreatedSim == null || sim.CreatedSim.LotCurrent == null)
				{
					return false;
				}
				if (sim.CreatedSim.LotCurrent != mTarget.LotCurrent)
				{
					return false;
				}
				CommonDoor commonDoor = mTarget as CommonDoor;
				CommonDoor.tSide side = CommonDoor.tSide.Front;
				commonDoor.GetSideOfDoorInRoom(commonDoor.RoomId, out side);
				int num = commonDoor.RoomId;
				if (side != CommonDoor.tSide.Back)
				{
					num = commonDoor.GetAdjoiningRoom(num);
				}
				Route route = sim.CreatedSim.CreateRoute();
				route.SetValidRooms(mTarget.LotCurrent.LotId, new int[num]);
				bool flag = route.Plan().Succeeded();
				bool flag2 = GoHere.Settings.GetDoorSettings(mTarget.ObjectId).IsSimAllowedThrough(sim.SimDescriptionId);
				if (Common.kDebugging && flag2 && !flag)
				{
					Common.Notify(sim.FullName + " allowed in door by settings but blocked by route (possibly another locked door or not lot greeted).");
				}
				return flag2;
			}
		}

		public override string GetTitlePrefix()
		{
			return "ListAllowedSims";
		}

		protected override bool Allow(GameHitParameters<GameObject> parameters)
		{
			if (GoHere.Settings.GetDoorSettings(parameters.mTarget.ObjectId).FiltersEnabled == 0)
			{
				return false;
			}
			return base.Allow(parameters);
		}

		protected override OptionResult Run(GameHitParameters<GameObject> parameters)
		{
			Sim sim = parameters.mActor as Sim;
			if (parameters.mTarget != null)
			{
				SimSelection simSelection = SimSelection.Create("", Name, sim.SimDescription, parameters.mTarget);
				if (simSelection.IsEmpty)
				{
					return OptionResult.Failure;
				}
				simSelection.SelectSingle();
			}
			return OptionResult.SuccessClose;
		}
	}
}
