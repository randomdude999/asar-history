;`78           56           34           12         
;`78 56        56 34        34 12        12 00      
;`78 56 34     56 34 12     34 12 00     12 00 00   
;`78 56 34 12  56 34 12 00  34 12 00 00  12 00 00 00
;`
;`78           56           34           12         
;`78 56        56 34        34 12        12 00      
;`78 56 34     56 34 12     34 12 00     12 00 00   
;`78 56 34 12  56 34 12 00  34 12 00 00  12 00 00 00
;`
;`78           56           34           12         
;`78 56        56 34        34 12        12 00      
;`78 56 34     56 34 12     34 12 00     12 00 00   
;`78 56 34 12  56 34 12 00  34 12 00 00  12 00 00 00
;`10 20 30 10
;`10 20 30 10
;`11 21 31 11

org $008000

table "data/table.tbl"
db "ABCD"
dw "ABCD"
dl "ABCD"
dd "ABCD"

table "data/table.tbl",ltr
db "ABCD"
dw "ABCD"
dl "ABCD"
dd "ABCD"

table "data/table-rtl.tbl",rtl
db "ABCD"
dw "ABCD"
dl "ABCD"
dd "ABCD"

't' = $10
'e' = $20
's' = $30

db "test"
db 't','e','s','t'
db 't'+1,'e'+1,'s'+1,'t'+1
