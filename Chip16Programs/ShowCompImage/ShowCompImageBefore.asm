PixelSpriteSize		equ		$0101        ; 2 x 1 pixel
PaletteSize			equ     48

FontSize   			equ		$0804        ; 8 x 8 pixel

Main:	
		ldm  RB, NumberOfPics
		andi RB, 0xFF 
		ldi  R9, Palette
		subi R9, PaletteSize
		spr  PixelSpriteSize
		ldi  R4, Picture
		ldi  R2, PixelBuffer ;Use high address for PixelBuffer
		ldi  R3, 0x00
		call InitString
NewPicture:
		cls
		addi R9, PaletteSize
		pal  ZeroPalette
		call ReadHeader ; after this call R4 points to data AFTER header
		add  RF, R4
		ldi  R0, 1
		ldm	 R0, XStartMiddle ; Xcoord start
		ldm  R1, YStartMiddle  ; Ycoord start
MainPixelLoop:
		ldm  R5, R4
		mov  R8, R5
		andi R8, 0xFF
		cmpi R8, 0x00
		jz   DataCollMode ;first byte in group=00 => DataColl
		mov  R6, R5
		andi R6, 0xF0
		shr  R6, 4 ; nibble count in R6
		andi R5, 0x0F ; nibble value in R5
		ldi  R2, PixelBuffer ; reset to initial pixel
		add  R2, R5 ; Choose color of pixel from buffer
		ldm  R7, R4
		andi R7, 0x80
		cnz  BigDataMode
NibblePixelLoop:		
		drw  R0,R1,R2
		call IncreasePixelPointer
		subi R6, 1
		cmpi R6, 0
		jnz  NibblePixelLoop
		addi R4, 1
		cmp  RF, R4
		jnz  MainPixelLoop
		cls
loop:   jmp  loop

IncreasePixelPointer:
		ldm  RD, XEndMiddle
		addi R0, 1
		cmp  R0, RD
		jnz  IPP_Return
		;ldi  R0, 0
		mov  R0, RC
		addi R1, 1
		ldm  RE, YEndMiddle
		cmp  R1, RE
		jz   EndOfPicture
IPP_Return:
		ret
EndOfPicture:
		pop  RA ; dummy pop to remove return address from stack
		addi R4, 1
; make palette tone from Black=>Colors
		;ldi  R0, ZeroPalette
		;mov  R1, R9
		;call TonePalette
		pal R9
		call DrawStringAndInc
; wait 5 seconds
		ldi  R0, 1
		call WaitSeconds
; make palette tone from Colors=>Black
		mov  R0,R9
		ldi  R1, ZeroPalette
		call TonePalette
		subi RB, 1
		jz   Main
		jmp  NewPicture
DataCollMode:
		addi R4, 1
		ldm  R5, R4 ; store nibble count in R5
		andi R5, 0xFF
		addi R4, 1
DataCollLoop:
		push R1
		push R0
		mov  R0, R4
		call ReadByte
		pop  R0
		mov  R2, R1
		mov  R3, R1
		pop  R1
		andi R2, 0xF0
		shr  R2, 4 ; first high nibble as first byte
		addi R2, PixelBuffer
		drw  R0,R1,R2
		call IncreasePixelPointer
		subi R5, 1
		cmpi R5, 0
		jz   EndOfDataColl
		andi R3, 0x0F
		addi R3, PixelBuffer
		drw  R0,R1,R3
		call IncreasePixelPointer
		addi R4, 1
		subi R5, 1
		cmpi R5, 0
		jnz  DataCollLoop
		jmp  MainPixelLoop
EndOfDataColl:
		addi r4, 1
		jmp  MainPixelLoop
		
BigDataMode: ;extract full count of nibbles into R6
		push R7
		ldm  R7, R4 ;reread first byte
		andi R7, 0x70
		shl  R7, 4
		addi R4, 1
		ldm  R6,R4
		andi R6,0xFF
		add  R6, R7
		pop  R7
		ret

ReadHeader:
		push R5
		ldm  RF, R4 ; RF = compressed image data size in bytes
		addi R4, 2
		ldm  RD, R4 ; RD = X-size
		ldi  RC, XSize
		stm  RD, RC ; Store Xsize in XSize mem.
		ldi  R5, 320
		sub  R5, RD
		shr  R5, 1
		ldi  RC, XStartMiddle
		stm  R5, RC
		add  R5, RD
		ldi  RC, XEndMiddle
		stm  R5, RC		
		
		addi R4, 2
		ldm  RE, R4 
		andi RE, 0xFF ; RE = Y-size (only 8-bit used)
		ldi  RC, YSize
		stm  RE,RC ; Store Ysize in YSize mem. 
		ldi  R5, 240
		sub  R5, RE
		shr  R5, 1
		stm  R5, YStartMiddle
		add  R5, RE
		stm  R5, YEndMiddle
		addi R4, 1
; Calculate the support values for image centering
; XStart = (320-XSize)/2
		ldi  RC, 320
		sub  RC, RD
		shr  RC, 1
		add  RD, RC
		pop  R5
		ret
InitString:
	ldi  R0, PictureStrings
	stm  R0, CurrentStringAddr
	ret
	
DrawStringAndInc:
	spr  FontSize
	push R0
	push R1
	push R2
	push R3
	push R4
	ldi  R2, 72
	ldi  R3, 230
	ldi  R4, CurrentStringAddr
	ldm  R0, R4
StringLoop:
	call ReadByte
	cmpi R1, 0x20
	jnz  NoSpace
	addi R1, 0x20
NoSpace:
	subi R1, 0x30
	muli R1, 32
	addi R1, MarioFont
	addi R1, 3
	drw  R2, R3, R1
	addi R2, 8
	addi R0, 1
	cmpi R2, 248
	jnz  StringLoop
	stm  R0, R4
	pop  R4
	pop  R3
	pop  R2
	pop  R1
	pop  R0
	spr  PixelSpriteSize
	ret
	
include ..\Libraries\ByteAccessLib.asm		
include ..\Libraries\PaletteToneLib.asm
include ..\Libraries\TimeLib.asm

EndOfProgram:
db 0x00

XStartMiddle: ; The X start coordinate to get image centered
db 0x00, 0x00
YStartMiddle: ; The Y start coordinate to get image centered
db 0x00, 0x00
XEndMiddle:	  ; The X end coordinate to get image centered
db 0x00, 0x00
YEndMiddle:	  ; The Y end coordinate to get image centered
db 0x00, 0x00
XSize:
db 0x00, 0x00
YSize:
db 0x00, 0x00

PixelBuffer:
db 0x00, 0x10, 0x20, 0x30, 0x40, 0x50, 0x60, 0x70, 0x80, 0x90, 0xA0, 0xB0, 0xC0, 0xD0, 0xE0, 0xF0

CurrentStringAddr:
db 0x00, 0x00

; Use Full MarioFont - ASCII correct for alpha&digit => (CHAR-0x30)
importbin Graphics\MarioFontLong.bmp.bin 0 1539 MarioFont
