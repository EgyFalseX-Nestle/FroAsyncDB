SELECT customer_chain,customer_number,equipment_number,install_date,eqp_payment,year,month,edit_d,edit_t
FROM
(SELECT 
 EQACUSCHN AS customer_chain
,EQACUSNUM AS customer_number
,EQAEQMNUM AS equipment_number
, CASE WHEN EQAINSDTE = 0 THEN NULL ELSE convert(datetime,convert(char(8),EQAINSDTE )) END AS install_date
,EQAPAYAMT AS eqp_payment
, Year(GETDATE()) AS [year]
, Month(GETDATE()) AS [month]
,EQACHGDTE AS edit_d
,EQACHGTIM AS edit_t
FROM CXPRDDTA.RMEQAP INNER JOIN CXPRDDTA.RMEQMP ON RMEQAP.EQAEQMNUM = RMEQMP.EQMEQMNUM
INNER JOIN CXPRDDTA.RMCUSP ON RMEQAP.EQACUSCHN = RMCUSP.CUSCUSCHN AND RMEQAP.EQACUSNUM = RMCUSP.CUSCUSNUM
) T

