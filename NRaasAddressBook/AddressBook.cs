using NRaas.CommonSpace.Booters;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NRaas
{
	public class AddressBook : Common, Common.IWorldLoadFinished, Common.IWorldQuit
    {
        [Tunable, TunableComment("Scripting Mod Instantiator, value does not matter, only its existence")]
        protected static bool kInstantiator = false;

		static AddressBook()
        {
            Bootstrap();
        }

		[PersistableStatic]
		public static Dictionary<ulong, string> mAddresses;

		public static void AddAddress (ulong id, string address)
		{
			if (address == null) return;

			if (mAddresses == null) mAddresses = new Dictionary<ulong, string> ();

			if (mAddresses.ContainsKey (id))
				mAddresses [id] = address;
			else
				mAddresses.Add (id, address);
		}

		public void OnWorldLoadFinished()
		{
			if (mAddresses != null)
			{
				List<ulong> invalidIds = new List<ulong> ();
				foreach (ulong lotId in mAddresses.Keys)
				{
					Lot lot;
					if (LotManager.sLots.TryGetValue (lotId, out lot))
					{
						lot.mAddressLocalizationKey = mAddresses [lotId];
					}
					else
					{
						invalidIds.Add (lotId);
					}
				}
				foreach (ulong id in invalidIds)
				{
					mAddresses.Remove (id);

					BooterLogger.AddTrace("Removed: " + id);
				}
			}
		}

		public void OnWorldQuit()
		{
			mAddresses = null;
		}
    }
}
