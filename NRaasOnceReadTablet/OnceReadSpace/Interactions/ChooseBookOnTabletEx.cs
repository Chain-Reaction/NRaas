using NRaas.CommonSpace.Helpers;
using NRaas.OnceReadSpace.Helpers;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.Store.Objects;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace NRaas.OnceReadSpace.Interactions
{
    public class ChooseBookOnTabletEx : Tablet.ChooseBookOnTablet, Common.IPreLoad, Common.IAddInteraction
    {
        static InteractionDefinition sOldSingleton;

        public void AddInteraction(Common.InteractionInjectorList interactions)
        {
            interactions.Replace<Tablet, Tablet.ChooseBookOnTablet.Definition>(Singleton);
        }

        public void OnPreLoad()
        {
            Tunings.Inject<Tablet, Tablet.ChooseBookOnTablet.Definition, Definition>(false);

            sOldSingleton = Singleton;
            Singleton = new Definition();
        }

        public override bool Run()
        {
            try
            {
                Book target = null;
                if (Autonomous)
                {
                    Definition interactionDefinition = InteractionDefinition as Definition;
                    if (interactionDefinition == null)
                    {
                        return false;
                    }
                    target = interactionDefinition.mAutonomyBook;
                }
                else
                {
                    target = GetSelectedObject() as Book;
                }

                if (target == null)
                {
                    return false;
                }

                bool succeeded = false;
                BeginCommodityUpdates();
                Actor.PopCanePostureIfNecessary();
                if (Actor.Inventory.Contains(Target))
                {
                    InteractionDefinition definition = new Tablet.ReadBookOnTablet.Definition(true);
                    InteractionInstance instance = definition.CreateInstance(target, Actor, GetPriority(), Autonomous, true);
                    (instance as Tablet.ReadBookOnTablet).mTablet = Target;
                    succeeded = Actor.InteractionQueue.PushAsContinuation(instance, false);
                }
                else
                {
                    InteractionDefinition definition2 = new Tablet.PickUpTabletForReading.Definition(target);
                    InteractionInstance instance2 = definition2.CreateInstance(Target, Actor, GetPriority(), Autonomous, true);
                    succeeded = Actor.InteractionQueue.PushAsContinuation(instance2, false);
                }
                EndCommodityUpdates(succeeded);
                return true;
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception e)
            {
                Common.Exception(Actor, Target, e);
                return false;
            }
        }

        public new class Definition : InteractionDefinition<Sim, Tablet, ChooseBookOnTabletEx>, IOverridesCalculateScore
        {
            public Book mAutonomyBook;

            public float CalculateScore(InteractionObjectPair interactionObjectPair, Sims3.Gameplay.Autonomy.Autonomy autonomy)
            {
                float num = 0f;
                mAutonomyBook = null;
                foreach (Book book in TabletEx.GetBooksInTown(autonomy.Actor, false, true, true))
                //foreach (Book book in Tablet.GetBooksOnMyLot(autonomy.Actor, false, true))
                {
                    // Custom
                    ReadBookData data;
                    if (autonomy.Actor.ReadBookDataList.TryGetValue(book.Data.ID, out data))
                    {
                        if (data.TimesRead > 0)
                        {
                            continue;
                        }
                    }

                    GreyedOutTooltipCallback greyedOutTooltipCallback = null;
                    if (book.TestReadBook(autonomy.Actor, true, ref greyedOutTooltipCallback))
                    {
                        float interestInBook = BookEx.GetInterestInBook(autonomy.Actor, book);
                        if (interestInBook > num)
                        {
                            mAutonomyBook = book;
                            num = interestInBook;
                        }
                    }
                }

                if (mAutonomyBook == null)
                {
                    return 0f;
                }

                if (autonomy.Actor.HasTrait(TraitNames.ComputerWhiz))
                {
                    num *= Tablet.kCompWhizMultiplier;
                }
                else if (autonomy.Actor.HasTrait(TraitNames.AntiTV))
                {
                    num *= Tablet.kTechnophobeMultiplier;
                }

                return (num * autonomy.CalculateScoreForObjectInteraction(interactionObjectPair));
            }

            public override string GetInteractionName(Sim actor, Tablet target, InteractionObjectPair iop)
            {
                return Tablet.LocalizeString("ReadBook", new object[0x0]);
            }

            public override void PopulatePieMenuPicker(ref InteractionInstanceParameters parameters, out List<ObjectPicker.TabInfo> listObjs, out List<ObjectPicker.HeaderInfo> headers, out int NumSelectableRows)
            {
                NumSelectableRows = 0x1;
                headers = new List<ObjectPicker.HeaderInfo>();
                listObjs = new List<ObjectPicker.TabInfo>();
                headers.Add(new ObjectPicker.HeaderInfo("Ui/Caption/ObjectPicker:Title", "Ui/Tooltip/ObjectPicker:Name", 0xfa));

                Sim actor = parameters.Actor as Sim;

                List<ObjectPicker.RowInfo> rowInfo = new List<ObjectPicker.RowInfo>();
                GreyedOutTooltipCallback greyedOutTooltipCallback = null;
                foreach (Book book in TabletEx.GetBooksInTown(parameters.Actor as Sim, false, true, parameters.Autonomous))
                //foreach (Book book in Tablet.GetBooksOnMyLot(parameters.Actor as Sim, false, true))
                {
                    // Custom
                    ReadBookData bookData;
                    if (actor.ReadBookDataList.TryGetValue(book.Data.ID, out bookData))
                    {
                        if (bookData.TimesRead > 0)
                        {
                            continue;
                        }
                    }

                    if (!(book is SheetMusic) && book.TestReadBook(parameters.Actor as Sim, parameters.Autonomous, ref greyedOutTooltipCallback))
                    {
                        List<ObjectPicker.ColumnInfo> columnInfo = new List<ObjectPicker.ColumnInfo>();
                        ResourceKey objectDescKey = new ResourceKey((ulong)ResourceUtils.XorFoldHashString32("book_standard"), 0x1661233, 0x1);
                        ThumbnailKey thumbnail = new ThumbnailKey(objectDescKey, ThumbnailSize.Medium, ResourceUtils.HashString32(book.Data.GeometryState), ResourceUtils.HashString32(book.Data.MaterialState));
                        MedicalJournalData data = book.Data as MedicalJournalData;
                        if (data != null)
                        {
                            columnInfo.Add(new ObjectPicker.ThumbAndTextColumn(thumbnail, data.GetTitle((book as MedicalJournal).mOwner, data.CurrentEdition)));
                        }
                        else
                        {
                            columnInfo.Add(new ObjectPicker.ThumbAndTextColumn(thumbnail, book.Data.Title));
                        }
                        ObjectPicker.RowInfo info = new ObjectPicker.RowInfo(book, columnInfo);
                        rowInfo.Add(info);
                    }
                }

                ObjectPicker.TabInfo item = new ObjectPicker.TabInfo("Coupon", Localization.LocalizeString("Ui/Caption/ObjectPicker:Books", new object[0x0]), rowInfo);
                listObjs.Add(item);
            }

            public override bool Test(Sim actor, Tablet target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (TabletEx.GetBooksInTown(actor, true, false, isAutonomous).Count <= 0x0)
                {
                    return false;
                }
                return true;
            }
        }
    }
}


