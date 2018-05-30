SELECT 
ARHCUSCHN AS a_r_customer_chain
, ARHCUSNUM AS a_r_customer_number
, ARHIVCDTE AS doc_date
, ARHIVCNUM AS doc_number
, ARHARRTYP AS doc_type -----------------WARTTH-----------------------
, ARHIVCSTA AS doc_status ---------------WARIST-----------------------
, ARHOPNAMT AS open_amount
, ARHTRNDES AS item_description
, ARHSLSCHN AS customer_chain
, ARHSTRNUM AS customer_number
, ARHDUEDTE AS due_date
FROM CXPRDDTA.RMARHP
