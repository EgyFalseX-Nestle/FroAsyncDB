SELECT route_number,customer_code,delivery_type,delivery_week,delivery_frequency,delivery_day_struc1
FROM 
(SELECT 
RVCRTENUM AS route_number
, 'E' + CAST(RVCCUSNUM AS NVARCHAR) + '-' + CAST(RVCCUSCHN AS NVARCHAR) AS customer_code
,RVCCUSDLT AS delivery_type
,CASE WHEN RVCCUSBWK = 0 THEN NULL ELSE convert(datetime,convert(char(8),RVCCUSBWK)) END AS delivery_week
,RVCCUSFRQ AS delivery_frequency
,RVCCUSWK1 AS delivery_day_struc1
,MAX(RVCCHGDTE) AS RVCCHGDTE
,MAX(RVCCHGTIM) AS RVCCHGTIM
FROM CXPRDDTA.RMRVCP
WHERE RVCEFFDTE <> 0 AND RVCENDDTE <> 0 AND RVCCUSSTR <> 0 AND RVCCUSEND <> 0 AND 
(GETDATE() BETWEEN convert(datetime,convert(char(8),RVCEFFDTE)) AND convert(datetime,convert(char(8),RVCENDDTE)))
AND (GETDATE() BETWEEN convert(datetime,convert(char(8),RVCCUSSTR)) AND convert(datetime,convert(char(8),RVCCUSEND)))
GROUP BY RVCRTENUM,RVCCUSCHN,RVCCUSNUM,RVCCUSDLT,RVCCUSBWK,RVCCUSFRQ,RVCCUSWK1
)T