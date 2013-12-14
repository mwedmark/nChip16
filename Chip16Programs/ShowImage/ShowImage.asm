PictureSpriteSize	equ		$F0A0        ; 320 x 240 pixel
HeaderSize			equ 	3
main:	
		pal Palette
		spr PictureSpriteSize
		ldi  R2, Picture
		addi R2, HeaderSize
		drw R0, R1, R2
loop:	jmp loop
		
EndOfProgram:
db 0x00
importbin R36_320_240_16COL_DIT.bmp.PAL 0 48 Palette
importbin R36_320_240_16COL_DIT.bmp.bin 0 38403 Picture
