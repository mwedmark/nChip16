PictureSpriteSize	equ		$F0A0        ; 320 x 240 pixel
main:	
		pal Palette
		spr PictureSpriteSize
		drw R0,R1, Picture
loop:	jmp loop
		
EndOfProgram:
db 0x00
importbin R36_320_240_NODITHER.bmp.PAL 0 48 Palette
importbin R36_320_240_NODITHER.bmp.bin 0 38400 Picture
