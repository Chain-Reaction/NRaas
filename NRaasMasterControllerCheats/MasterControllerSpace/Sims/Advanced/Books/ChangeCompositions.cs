using NRaas.CommonSpace.Dialogs;
using NRaas.CommonSpace.Options;
using NRaas.CommonSpace.Selection;
using NRaas.MasterControllerSpace.SelectionCriteria;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using Sims3.UI.CAS;
using System;
using System.Collections.Generic;
using System.Text;

namespace NRaas.MasterControllerSpace.Sims.Advanced.Books
{
    public class ChangeCompositions : SimFromList, IBooksOption
    {
        CommonSelection<Item>.Results mSelection = null;

        public override string GetTitlePrefix()
        {
            return "ChangeCompositions";
        }

        protected override int GetMaxSelection()
        {
            return 0;
        }

        protected override bool CanApplyAll()
        {
            return true;
        }

        protected override OptionResult RunResult
        {
            get { return OptionResult.SuccessRetain; }
        }

        protected override bool PrivateAllow(SimDescription me)
        {
            if (!base.PrivateAllow(me)) return false;

            if (me.SkillManager == null) return false;

            bool hasSkill = false;

            if (me.SkillManager.GetElement(SkillNames.Guitar) != null)
            {
                hasSkill = true;
            }
            else if (me.SkillManager.GetElement(SkillNames.BassGuitar) != null)
            {
                hasSkill = true;
            }
            else if (me.SkillManager.GetElement(SkillNames.Drums) != null)
            {
                hasSkill = true;
            }
            else if (me.SkillManager.GetElement(SkillNames.Piano) != null)
            {
                hasSkill = true;
            }
            else if (me.SkillManager.GetElement(SkillNames.LaserHarp) != null)
            {
                hasSkill = true;
            }

            return hasSkill;
        }

        protected static void CollectCompositions(List<Item> allOptions, BandSkill skill, List<Guitar.Composition> regularCompositions, List<Guitar.Composition> masterCompositions)
        {
            if (skill == null) return;

            List<Guitar.Composition> compositions = new List<Guitar.Composition>(regularCompositions);
            compositions.AddRange(masterCompositions);

            foreach (Guitar.Composition song in compositions)
            {
                if (song.InstrumentSkillNameKey != skill.Guid) continue;

                if ((skill.KnownCompositions.Contains(song)) || (skill.KnownMasterCompositions.Contains(song)))
                {
                    allOptions.Add(new Item(skill, song, 1));
                }
                else
                {
                    allOptions.Add(new Item(skill, song, 0));
                }
            }
        }

        protected override bool Run(SimDescription me, bool singleSelection)
        {
            if (!ApplyAll)
            {
                List<Item> allOptions = new List<Item>();

                allOptions.Add(null);

                CollectCompositions(allOptions, me.SkillManager.GetElement(SkillNames.Guitar) as BandSkill, Guitar.GuitarCompositions, Guitar.GuitarMasterCompositions);
                CollectCompositions(allOptions, me.SkillManager.GetElement(SkillNames.Drums) as BandSkill, DrumSkill.DrumsCompositions, DrumSkill.DrumsMasterCompositions);
                CollectCompositions(allOptions, me.SkillManager.GetElement(SkillNames.Piano) as BandSkill, Piano.PianoCompositions, Piano.PianoMasterCompositions);
                CollectCompositions(allOptions, me.SkillManager.GetElement(SkillNames.BassGuitar) as BandSkill, BassGuitarSkill.BassGuitarCompositions, BassGuitarSkill.BassGuitarMasterCompositions);
                CollectCompositions(allOptions, me.SkillManager.GetElement(SkillNames.LaserHarp) as BandSkill, LaserHarpSkill.LaserHarpCompositions, LaserHarpSkill.LaserHarpMasterCompositions);

                mSelection = new CommonSelection<Item>(Name, me.FullName, allOptions, -40, new SkillNameColumn()).SelectMultiple();
                if ((mSelection == null) || (mSelection.Count == 0)) return false;

                CommonSelection<Item>.HandleAllOrNothing(mSelection);
            }

            foreach (Item item in mSelection)
            {
                if (item == null) continue;

                BandSkill skill = item.mSkill;

                Guitar.Composition song = item.Value;

                if (!item.IsSet)
                {
                    skill.CompositionLearned(song.AudioClip);
                }
                else if (song.IsMasterTrack)
                {
                    skill.KnownMasterCompositions.Remove(song);
                }
                else
                {
                    skill.KnownCompositions.Remove(song);
                }
            }

            List<BandSkill> skills = new List<BandSkill>();

            skills.Add(me.SkillManager.GetElement(SkillNames.Guitar) as BandSkill);
            skills.Add(me.SkillManager.GetElement(SkillNames.Drums) as BandSkill);
            skills.Add(me.SkillManager.GetElement(SkillNames.Piano) as BandSkill);
            skills.Add(me.SkillManager.GetElement(SkillNames.BassGuitar) as BandSkill);
            skills.Add(me.SkillManager.GetElement(SkillNames.LaserHarp) as LaserHarpSkill);

            foreach (SheetMusicData music in BookData.SheetMusicDataList.Values)
            {
                bool found = false;
                foreach (BandSkill skill in skills)
                {
                    if (skill == null) continue;

                    if (skill.KnownCompositions.Contains(music.Composition))
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    me.ReadBookDataList.Remove(music.ID);
                }
            }

            return true;
        }

        protected class SkillNameColumn : ObjectPickerDialogEx.CommonHeaderInfo<Item>
        {
            public SkillNameColumn()
                : base("NRaas.MasterController.OptionList:TypeTitle", "NRaas.MasterController.OptionList:TypeTooltip", 100)
            { }

            public override ObjectPicker.ColumnInfo GetValue(Item item)
            {
                if (item == null) return null;

                return new ObjectPicker.TextColumn(item.mSkill.Name);
            }
        }

        protected class Item : ValueSettingOption<Guitar.Composition>
        {
            public BandSkill mSkill;

            public Item(BandSkill skill, Guitar.Composition song, int count)
                : base(song, song.Name, count)
            {
                mSkill = skill;
            }

            public override string DisplayKey
            {
                get { return "Knows"; }
            }
        }
    }
}
