SELECT sales_number,customer_code,delivery_date,ticket_number,pay_type,route_number,salesman_code,distribution_center,quantity,value,dump_amount,dump_quantity,discount_amount,a_r_discount_amount
,invoice_number,a_r_entered_date,cash_payments,tax_amount,a_r_customer_code,invoice_run,a_r_transaction_type,tax_collected_amount
FROM
(SELECT
 SLHSLSNBR AS sales_number
,'E' + CAST(SLHCUSNUM AS NVARCHAR) + '-' + CAST(SLHCUSCHN AS NVARCHAR) AS customer_code
,convert(datetime,convert(char(8),SLHDELDTE)) AS delivery_date
,SLHTCKNUM AS ticket_number
,CASE WHEN SLHSLSTYP = 0 THEN NULL ELSE SLHSLSTYP END AS pay_type
,SLHRTENUM AS route_number
,'E' + CAST(SLHDRVNUM AS NVARCHAR) + '-99999' AS salesman_code
,CASE WHEN SLHDSTCTR = 0 OR SLHDSTCTR = 999 THEN NULL ELSE SLHDSTCTR END AS distribution_center
,SLHTOTQTY AS quantity
,SLHTOTVAL AS value
,SLHSLWAMT AS dump_amount
,SLHSLWQTY AS dump_quantity
,SLHDSCAMT AS discount_amount
,SLHARRDSC AS a_r_discount_amount
,SLHIVCNUM AS invoice_number
, CASE WHEN SLHARENTD = 0 THEN NULL ELSE convert(datetime,convert(char(8),SLHARENTD)) END AS a_r_entered_date
,SLHCSHRCV AS cash_payments
,SLHTAXAMT AS tax_amount
,CASE WHEN SLHARCHNN = 0 AND SLHARCUSN = 0 THEN NULL ELSE 'E' + CAST(SLHARCUSN AS NVARCHAR) + '-' + CAST(SLHARCHNN AS NVARCHAR) END AS a_r_customer_code
,SLHINVRUN AS invoice_run
,SLHARRTYP AS a_r_transaction_type
,SLHTAXCOL AS tax_collected_amount
,SLHCHGDTE AS edit_d
,SLHCHGTIM AS edit_t
FROM CXPRDDTA.RMSLHP) T
