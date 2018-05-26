SELECT sales_number,chain_number,customer_number,delivery_date,ticket_number,pay_type,route_number,salesman_chain,salesman,distribution_center,quantity,value,dump_amount,dump_quantity,discount_amount,a_r_discount_amount
,invoice_number,a_r_entered_date,cash_payments,tax_amount,a_r_chain_number,a_r_customer_number,invoice_run,a_r_transaction_type,tax_collected_amount,edit_d,edit_t
FROM
(SELECT
 SLHSLSNBR AS sales_number
,SLHCUSCHN AS chain_number
,SLHCUSNUM AS customer_number
,convert(datetime,convert(char(8),SLHDELDTE)) AS delivery_date
,SLHTCKNUM AS ticket_number
,CASE WHEN SLHSLSTYP = 0 THEN NULL ELSE SLHSLSTYP END AS pay_type
,SLHRTENUM AS route_number
, 99999 AS salesman_chain
,SLHDRVNUM AS salesman
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
,CASE WHEN SLHARCHNN = 0 AND SLHARCUSN = 0 THEN NULL ELSE SLHARCHNN END AS a_r_chain_number
,CASE WHEN SLHARCHNN = 0 AND SLHARCUSN = 0 THEN NULL ELSE SLHARCUSN END AS a_r_customer_number
,SLHINVRUN AS invoice_run
,SLHARRTYP AS a_r_transaction_type
,SLHTAXCOL AS tax_collected_amount
,SLHCHGDTE AS edit_d
,SLHCHGTIM AS edit_t
FROM CXPRDDTA.RMSLHP) T