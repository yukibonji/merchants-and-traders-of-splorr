CREATE VIEW [dbo].[SiteRouteListItems]
	AS
SELECT
	r.RouteId,
	r.FromSiteId,
	sf.SiteName FromSiteName,
	r.ToSiteId,
	st.SiteName ToSiteName,
	CAST(0 AS BIT) Reversed,
	SQRT((sf.SiteX-st.SiteX)*(sf.SiteX-st.SiteX)+(sf.SiteY-st.SiteY)*(sf.SiteY-st.SiteY)) Distance
FROM
	WorkerRouteStates wrs
	JOIN [Routes] r ON wrs.RouteId = r.RouteId
	JOIN Sites sf ON r.FromSiteId = sf.SiteId
	JOIN Sites st ON r.ToSiteId = st.SiteId

UNION ALL

SELECT
	r.RouteId,
	r.ToSiteId FromSiteId,
	st.SiteName FromSiteName,
	r.FromSiteId ToSiteId,
	sf.SiteName ToSiteName,
	CAST(1 AS BIT) Reversed,
	SQRT((sf.SiteX-st.SiteX)*(sf.SiteX-st.SiteX)+(sf.SiteY-st.SiteY)*(sf.SiteY-st.SiteY)) Distance
FROM
	WorkerRouteStates wrs
	JOIN [Routes] r ON wrs.RouteId = r.RouteId
	JOIN Sites sf ON r.FromSiteId = sf.SiteId
	JOIN Sites st ON r.ToSiteId = st.SiteId
