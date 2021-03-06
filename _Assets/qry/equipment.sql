SELECT eqp_number,serial_number,eqp_maker,eqp_model,eqp_description,purchase_date,parent_asset,asset_type,status_code,eqp_type,book_value,edit_d,edit_t
FROM
(SELECT  
EQMEQMNUM  AS eqp_number
,EQMSERNUM AS serial_number
,CASE WHEN EQMEQPMAK = '0' OR EQMEQPMAK = '' THEN NULL ELSE EQMEQPMAK END AS eqp_maker
,CASE WHEN EQMEQPMOD = '0' OR EQMEQPMOD = '' THEN NULL ELSE EQMEQPMOD END AS eqp_model
,CASE WHEN EQMLDSCDE = 0 THEN NULL ELSE EQMLDSCDE END AS eqp_description
, convert(datetime,convert(char(8),EQMPURDTE )) AS purchase_date
,EQMPARAST AS parent_asset
,CASE WHEN EQMASTTYP = 0 THEN NULL ELSE EQMASTTYP END AS asset_type
,CASE WHEN EQMSTACDE = 0 THEN NULL ELSE EQMSTACDE END AS status_code
,CASE WHEN EQMEQPTYP = 0 THEN NULL ELSE EQMEQPTYP END AS eqp_type
,EQMCURVAL AS book_value
, CASE WHEN EQMCHGDTE >= EQMCRTDTE THEN EQMCHGDTE ELSE EQMCRTDTE END AS edit_d
, CASE WHEN EQMCHGTIM >= EQMCRTTIM THEN EQMCHGTIM ELSE EQMCRTTIM END AS edit_t
FROM eRMSTEG.CXPRDDTA.RMEQMP
) T
