//using Sims3.Gameplay;
//using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
//using Sims3.Gameplay.Actors;
//using Sims3.Gameplay.ActorSystems;
//using Sims3.Gameplay.Autonomy;
//using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
//using Sims3.Gameplay.EventSystem;
//using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
//using Sims3.SimIFace.CAS;
//using Sims3.SimIFace.Enums;
using Sims3.UI;
//using Sims3.UI.CAS;
//using System;
//using System.Collections.Generic;
//using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Intermediate.Careers
{
	public class ChangeStageName : CareerOption
	{
		public override string GetTitlePrefix()
		{
			return "";
		}
		public override string Name
		{
			get 
			{
				return Common.LocalizeEAString("Gameplay/PerformanceCareers:GiveName");
			}
		}

		protected override int GetMaxSelection()
		{
			return 0;
		}

		protected override bool PrivateAllow(SimDescription me)
		{
			if (!base.PrivateAllow(me)) return false;

			return me.OccupationAsPerformanceCareer != null;
		}

		protected override bool Run(SimDescription me, bool singleSelection)
		{
			PerformanceCareer performer = me.OccupationAsPerformanceCareer;
			if (performer != null)
			{
				//me.OccupationAsPerformanceCareer.GivePerformerName (me.CreatedSim, false);
				string text2;
				if (string.IsNullOrEmpty(performer.StageName))
				{
					string localizedString = StringTable.GetLocalizedString((!me.IsFemale) ? (performer.MaleGivenNameKeyBase + RandomUtil.GetInt(performer.GetNumberOfMaleGivenNames - 1).ToString()) : (performer.FemaleGivenNameKeyBase + RandomUtil.GetInt(performer.GetNumberFemaleGivenNames - 1).ToString()));
					string text = Common.LocalizeEAString(me.IsFemale, performer.FamilyNameKeyBase + RandomUtil.GetInt(performer.GetNumberOfFamilyNames - 1).ToString());
					if (StringTable.GetLocale() == "ja-jp")
						text2 = text + " " + localizedString;
					else
						text2 = localizedString + " " + text;
				}
				else
				{
					text2 = performer.StageName;
				}
				string text3 = StringInputDialogRandom.Show(Common.LocalizeEAString("Ui/CreateNameUI:Title"), me.FullName + " (" + performer.CareerName + ")", text2, 27, StringInputDialog.Validation.TextOnly, false, new StringInputDialogRandom.RandomDelegate(performer.GetRandomStageName));
				if (!string.IsNullOrEmpty (text3))
				{
					me.OccupationAsPerformanceCareer.StageName = text3;
					me.CareerManager.UpdateCareerUI ();
					return true;
				}
			}
			return false;
		}
	}
}
