; R0 = Xcoord (used by DrawNumber in MiniMaxiFontLib)
; R1 = Ycoord (used by DrawNumber in MiniMaxiFontLib)
; R2,R3,R4,R5,R6,R7 => Currently shown byte values
; R8 = Number to draw (used byDrawNumber in MiniMaxiFontLib)
; R9 = Remember low/high nibble
; RC = Current Scroll
; RA = Current Byte value to render

Init:
	call InitMiniMaxi
Main:
	ldi  R0, 0
	ldi  R1, 75
	ldi  RC, 0 
	ldi  RD, 0
	ldi  R9, 0 ; switches between 0/1 for high/low nibble in data
DrawNewFrame:	
	cls
	call DrawFrame
	call ScrollFrame
	vblnk
	jmp DrawNewFrame
	
DrawFrame:
	ldi  RA, StartOfTiles
	mov  R0, RC
DrawNextNumber:
	ldm  RB, RA
	andi RB, 0xFF
	mov  R8, RB
	call DrawNumber
	addi RA, 1
	cmpi R8, 0x20
	jz	 DrawSpace
	addi R0, 35
DrawSpace:
	addi R0, 10
	cmpi RA, EndOfTiles
	jnz  DrawNextNumber
	ret

ScrollFrame:
	subi RC, 1
	cmpi RC, 0xFFD3 ;0-45 for off-screen scroll
	jnz  ScrollFinished
	ldi  RC, 0
	call ShiftValuesLeft
	call ReadNewValue
ScrollFinished:
	ret
	
ShiftValuesLeft:
	push R0
	push R1
	ldi  R0, StartOfTiles
	addi R0, 1
ShiftNext:
	call ReadByte
	subi R0, 1
	call WriteByte
	addi R0, 2
	cmpi R0, EndOfTiles
	jnz  ShiftNext
	pop  R1
	pop  R0
	ret
	
ReadNewValue:
	ldm   RE, RD
	cmpi  R9, 0
	jnz   NoShiftNibble
	shr   RE, 4 ; use just 4 MSB 
NoShiftNibble:
	xori  R9, 0x1
	cmpi  R9, 0x1
	jz    NoAddAddress
	addi  RD, 1 ; inc to next value
NoAddAddress:
	andi  RE, 0xF
	ldi   RF, StartOfTiles
	addi  RF, 9
	stm   RE, RF
	ret
	
; Complete lib for drawing big chars using reusable
; small images. (only support for digits now, alot of work even these..)
include ..\Libraries\MiniMaxiFontLib.asm
include ..\Libraries\ByteAccessLib.asm
; importbin does not work in files included using 'include'
; thus these files is located here though they should
; have been located in MiniMaxiFontLib.asm
importbin ..\Libraries\MiniMaxiFont.bmp.bin 0 3075 FontSprites
importbin ..\Libraries\MiniMaxiFont.bmp.PAL 0 48 Palette

StartOfTiles: ;0xFF is shown as empty chars
db 0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF
EndOfTiles:
db 0x00

EndOfProgram:
db 0x00