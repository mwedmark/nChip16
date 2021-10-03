PictureSpriteSize	equ		$2010        ; 320 x 240 pixel
HeaderSize			equ 	3
main:	SPR PictureSpriteSize
		LDI  R2, Picture
		ADDI R2, HeaderSize
		DRW R0, R1, R2
		VBLNK
loop:	JMP loop
		
EndOfProgram:
db 0x00,0x00
db 0x00,0x00
db 0x00,0x00
db 0x00,0x00
db 0x00,0x00
;importbin R36_320_240_16COL_DIT.bmp.PAL 0 48 Palette
importbin B.bin 0 1024 Picture
