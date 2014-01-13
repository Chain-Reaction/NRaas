using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.StoryProgression;
using System;

namespace NRaas.OverwatchSpace.Loadup
{
    public class CleanupBogusNewsArticles : DelayedLoadupOption
    {
        public CleanupBogusNewsArticles()
        { }

        public override void OnDelayedWorldLoadFinished()
        {
            Overwatch.Log("CleanupBogusNewsArticles");

            foreach (Newspaper paper in Sims3.Gameplay.Queries.GetObjects<Newspaper> ())
            {
                try
                {
                    if ((paper.LatestNews != null) && (paper.LatestNews.mArticles != null))
                    {
                        int index = 0;
                        while (index < paper.LatestNews.mArticles.Count)
                        {
                            News.HotSpotArticle hotspot = paper.LatestNews.mArticles[index] as News.HotSpotArticle;
                            if ((hotspot != null) && (hotspot.mLot == null))
                            {
                                paper.LatestNews.mArticles.RemoveAt(index);

                                Overwatch.Log("Bogus News Article Dropped");
                            }
                            else
                            {
                                index++;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Common.Exception(paper, e);
                }
            }
        }
    }
}
