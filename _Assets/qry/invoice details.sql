SELECT sales_number,sales_detail_seq,material_number,sales_type,transaction_quantity,_transaction_price,transaction_amount
,discount_amount,unit_discount,a_r_discount_amount,price_override,price_source,price_group,base_price,tax_amount,edit_d,edit_t
FROM
(SELECT  
SLDSLSNBR AS sales_number
,SLDSLSSEQ AS sales_detail_seq
,SLDITMNUM AS material_number
,SLDSLSTYP AS sales_type
,SLDTRNQTY AS transaction_quantity
,SLDTRNPRC AS _transaction_price
,SLDTRNAMT AS transaction_amount
,SLDDSCAMT AS discount_amount
,SLDUNTDSC AS unit_discount
,SLDARRDSC AS a_r_discount_amount
,SLDPRCOVR AS price_override
,SLDPRCSRC AS price_source
,SLDPRCGRP AS price_group
,SLDBASPRC AS base_price
,SLDTAXAMT AS tax_amount
,SLDCRTDTE AS edit_d
,SLDCRTTIM AS edit_t
FROM CXPRDDTA.RMSLDP INNER JOIN CXPRDDTA.RMSLHP ON RMSLDP.SLDSLSNBR = RMSLHP.SLHSLSNBR
) T