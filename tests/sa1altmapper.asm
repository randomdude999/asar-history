;`000000 69 13 37
;`080000 42 99 12
;`100000 53 54 41
;`180000 41 54 53
;`200000 12 34 56
;`280000 68 01 11
;`300000 99 99 99
;`380000 01 23 45
;`
;`400000 00 FF 00
;`480000 69 96 69
;`500000 42 42 42
;`580000 DE DE DE
;`600000 DE AD 55
;`680000 BE EF 55
;`700000 A1 CA 40
;`780000 BE DE AD

fullsa1rom

org $008000
db $69,$13,$37
org $108000
db $42,$99,$12
org $208000
db $53,$54,$41
org $308000
db $41,$54,$53
org $808000
db $12,$34,$56
org $908000
db $68,$01,$11
org $A08000
db $99,$99,$99
org $B08000
db $01,$23,$45

org $C00000
db $00,$FF,$00
org $C80000
db $69,$96,$69
org $D00000
db $42,$42,$42
org $D80000
db $DE,$DE,$DE
org $E00000
db $DE,$AD,$55
org $E80000
db $BE,$EF,$55
org $F00000
db $A1,$CA,$40
org $F80000
db $BE,$DE,$AD