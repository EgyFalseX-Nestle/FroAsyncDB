SELECT route_number,route_name,salesman_chain,salesman,distribution_center,rsm,asm,supervisor,sales_district,sales_division,settlement_date,active,jd_customer,route_class
FROM
(SELECT 
 RTERTENUM AS route_number
,RTELNGDES AS route_name
, CASE WHEN RTEDRVNUM = 999999999 THEN 99997 ELSE 99999 END AS salesman_chain
,RTEDRVNUM AS salesman
,CASE WHEN RTEDSTCTR = 0 OR RTEDSTCTR = 999 THEN NULL ELSE RTEDSTCTR END AS distribution_center
,CASE WHEN RTEREGNUM = 0 THEN NULL ELSE RTEREGNUM END AS rsm
,CASE WHEN RTESUBREG = 0 THEN NULL ELSE RTESUBREG END AS asm
,CASE WHEN RTESLSNUM = 0 THEN NULL ELSE RTESLSNUM END AS supervisor
,CASE WHEN RTESLSDIS = 0 THEN NULL ELSE RTESLSDIS END AS sales_district
,CASE WHEN RTESLSDIV = 0 THEN NULL ELSE RTESLSDIV END AS sales_division
,CASE WHEN RTESTLDTE = 0 THEN NULL ELSE convert(datetime,convert(char(8),RTESTLDTE)) END AS settlement_date
,RTEHNDHLD AS active
,RTEBPCCUS AS jd_customer
,CASE WHEN RTERTECLS = 0 THEN NULL ELSE RTERTECLS END AS route_class
,RTECHGDTE
,RTECHGTIM
FROM CXPRDDTA.RMRTEP) T