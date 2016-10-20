namespace Pdg.Splorr.MerchantsAndTraders.DataLayer

open System
open System.Linq

type Position<'T> =
    { X: 'T;
      Y: 'T}

type Site =
    { SiteId: int;
      SiteName: string;
      Position: Position<float>;
      WorldId: int }

module SiteRepository =
    let mapSite (dbRecord: MaToSplorrProvider.dataContext.``dbo.SitesEntity``) : Site =
        { SiteId = dbRecord.SiteId;
          SiteName = dbRecord.SiteName;
          WorldId = dbRecord.WorldId;
          Position = {X = dbRecord.SiteX; Y = dbRecord.SiteY} }

    let fetchOne (siteId: int) (context:MaToSplorrProvider.dataContext) : Site =
        let result =
            query {
                for site in context.Dbo.Sites do
                where (site.SiteId=siteId)
                select (site)
                exactlyOne
            }
        { SiteId = result.SiteId;
          SiteName = result.SiteName;
          WorldId = result.WorldId;
          Position = {X = result.SiteX; Y = result.SiteY} }

    let create (site:Site) (context:MaToSplorrProvider.dataContext) : Site =
        let row = context.Dbo.Sites.Create()

        row.SiteName <- site.SiteName
        row.WorldId <- site.WorldId
        row.SiteX <- site.Position.X
        row.SiteY <- site.Position.Y

        context.SubmitUpdates()

        context
        |> fetchOne row.SiteId

    let fetchForWorld (worldId:int)  (context:MaToSplorrProvider.dataContext) : seq<Site> =
        let result =
            query {
                for site in context.Dbo.Sites do
                where (site.WorldId = worldId)
                select (site)
            }

        result
        |> Seq.map(fun x->x.MapTo<Site>())