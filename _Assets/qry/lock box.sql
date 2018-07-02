--pk=box_number,route_number
SELECT box_number,route_number,batch_date,batch_status,entry_count,in_used,used_by,used_date,closed_by,closed_date,payment
FROM
(
SELECT 
ILHBOXNUM as box_number
, ILHBATNUM as route_number
, convert(datetime,convert(char(8),ILHBATDTE )) as batch_date
, ILHBATSTA as batch_status
, CAST(ILHENTCNT/2 AS INT) as entry_count
, CAST(ILHUSESTA AS BIT) as in_used
, ILHUSEDBY as used_by
, ILHUSEDTE AS used_date
, ILHTRNUSR as closed_by
, CASE WHEN ILHTRNDTE = 0 THEN NULL ELSE convert(datetime,convert(char(8),ILHTRNDTE )) END AS closed_date
, ISNULL(payment, 0) AS payment
, ILHCHGDTE as edit_d
, ILHCHGTIM as edit_t
FROM CXPRDDTA.RMILHP
LEFT OUTER JOIN
(SELECT ILDBOXNUM as box_number, ILDBATNUM as route_number, SUM(ILDPAYAMT) as payment FROM CXPRDDTA.RMILDP WHERE ILDCHNNUM <> 99999 
GROUP BY ILDBOXNUM, ILDBATNUM) detail ON RMILHP.ILHBOXNUM = detail.box_number AND RMILHP.ILHBATNUM = detail.route_number
) AS T
