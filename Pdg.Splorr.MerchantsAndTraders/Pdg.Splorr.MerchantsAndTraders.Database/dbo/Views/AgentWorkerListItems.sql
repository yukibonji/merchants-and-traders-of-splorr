CREATE VIEW [dbo].[AgentWorkerListItems]
	AS
SELECT
	a.AgentId,
	w.WorkerId,
	w.WorkerName,
	w.WorkerStateId,
	sa.SiteId AtSiteId,
	sa.SiteName AtSiteName,

	CASE wrs.Reversed
		WHEN CAST(1 AS BIT) THEN st.SiteId
		ELSE sf.SiteId
	END FromSiteId,
	CASE wrs.Reversed
		WHEN CAST(1 AS BIT) THEN st.SiteName
		ELSE sf.SiteName
	END  FromSiteName,

	CASE wrs.Reversed
		WHEN CAST(1 AS BIT) THEN sf.SiteId
		ELSE st.SiteId
	END ToSiteId,
	CASE wrs.Reversed
		WHEN CAST(1 AS BIT) THEN sf.SiteName
		ELSE st.SiteName
	END ToSiteName,

	wrs.CompletesOn
FROM
	Agents a
	JOIN AgentWorkers aw ON aw.AgentId=a.AgentId
	JOIN Workers w ON w.WorkerId=aw.WorkerId
	LEFT JOIN WorkerSiteStates wss ON wss.WorkerId=w.WorkerId
	LEFT JOIN Sites sa ON sa.SiteId=wss.SiteId
	LEFT JOIN WorkerRouteStates wrs ON wrs.WorkerId=w.WorkerId
	LEFT JOIN [Routes] r ON wrs.RouteId=r.RouteId
	LEFT JOIN Sites sf ON r.FromSiteId = sf.SiteId
	LEFT JOIN Sites st ON r.ToSiteId = st.SiteId
