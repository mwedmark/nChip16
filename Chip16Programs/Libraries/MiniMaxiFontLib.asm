; -------------------------------------------------
; MiniMaxiFontLib - Library for drawing big font
; 2013-11-10 Written by Magnus Wedmark
; Usage in your program:
; ldi R0, 0 ; Xcoord on screen
; ldi R1, 0 ; Ycoord on screen
; ldi R8, 5 ; Char to draw
; include MiniMaxiFontLib.asm
; importbin MiniMaxiFont.bmp.bin 0 3075 FontSprites
; importbin MiniMaxiFont.bmp.PAL 0 48 Palette
; -------------------------------------------------

SpriteSize		equ		$2010        ; 32 x 32 pixel
SpriteDataSize  equ		512          ; 512 bytes/char

InitMiniMaxi:
	ldi  R2, FontSprites
	addi R2, 3 			;Bend over left
	mov  R3, R2 
	addi R3, 512        ;Right angle line /
	mov  R4, R3
	addi R4, 512        ;Vertical line |
	mov  R5, R4
	addi R5, 512		;Horizontal short line -
	mov  R6, R5
	addi R6, 512		;Quad fill without AA
	mov  R7, R6
	addi R7, 512		;AA-point
	pal  Palette
	spr  SpriteSize
	ret

; R0 = Xcoord
; R1 = Ycoord
; R8 = Char to draw
DrawNumber:
	cmpi R8, 00
	jz   Draw0
	cmpi R8, 01
	jz 	 Draw1
	cmpi R8, 02
	jz   Draw2
	cmpi R8, 03
	jz   Draw3
	cmpi R8, 04
	jz   Draw4
	cmpi R8, 05
	jz   Draw5
	cmpi R8, 06
	jz   Draw6
	cmpi R8, 07
	jz   Draw7
	cmpi R8, 08
	jz   Draw8
	cmpi R8, 09
	jz   Draw9
	cmpi R8, 0xA
	jz   DrawA
	cmpi R8, 0xB
	jz   DrawB
	cmpi R8, 0xC
	jz   DrawC
	cmpi R8, 0xD
	jz   DrawD
	cmpi R8, 0xE
	jz   DrawE
	cmpi R8, 0xF
	jz   DrawF
	ret

; all routines drawing single chars uses PUSH/POP so all
; registers are unaffected.
Draw0:	
	push R0
	push R1
	flip 0,0
	drw  R0,R1,R2
	flip 1,0
	addi R0, 31
	drw  R0,R1,R2
	flip 1,0
	subi  R0, 31
	addi  R1, 32
	drw  R0,R1,R4
	flip 0,0
	addi R0, 31
	drw  R0,R1,R4
	subi R0, 31
	addi R1, 31
	flip 0,1
	drw  R0,R1,R2
	addi R0, 31
	flip 1,1
	drw  R0,R1,R2
	pop  R1
	pop  R0
	ret
Draw1:
	push R0
	push R1
	flip 1,1
	addi R0, 12
	addi R1, 12
	drw  R0,R1,R3
	addi R0, 4
	drw  R0,R1,R4
	subi R0, 4
	subi R1, 12
	drw  R0,R1,R6
	addi R0, 4
	addi R1, 43
	drw  R0,R1,R4
	addi R1, 7
	drw  R0,R1,R4
	pop  R1
	pop  R0
	ret
Draw2:
	push R0
	push R1
	flip 0,0
	drw  R0,R1,R2
	flip 1,0
	addi R0, 31
	drw  R0,R1,R2
	flip 0,0
	subi R0, 31
	addi R1, 31
	drw  R0,R1,R2
	flip 1,1
	addi R0, 31
	drw  R0,R1,R2
	flip 0,0
	subi R0, 31
	addi R1, 31
	spr  $1510
	drw  R0,R1,R4
	addi R0, 12
	spr  SpriteSize
	drw  R0,R1,R5
	addi R0, 14
	drw  R0,R1,R5
	pop  R1
	pop  R0
	ret
Draw3:
	push R0
	push R1
	flip 0,0
	drw  R0,R1,R2
	flip 1,0
	addi R0, 31
	drw  R0,R1,R2
	flip 1,1
	addi R1, 31
	drw  R0,R1,R2
	flip 1,0
	drw  R0,R1,R2
	subi R0, 10
	drw  R0,R1,R6
	addi R0, 10
	flip 1,1
	addi R1, 31
	drw  R0,R1,R2
	flip 0,1
	subi R0, 31
	drw  R0,R1,R2
	pop  R1
	pop  R0
	ret
Draw4:
	push R0
	push R1
	flip 0,0
	addi R0, 3
	addi R1, 30
	drw  R0,R1,R3
	addi R0, 5
	subi R1, 8
	drw  R0,R1,R3
	addi R0, 7
	subi R1, 12
	drw  R0,R1,R3
	addi R0, 7
	subi R1, 12
	drw  R0,R1,R3
	addi R0, 8
	addi R1, 14
	drw  R0,R1,R4
	addi R1, 31
	drw  R0,R1,R4
	addi R1, 8
	drw  R0,R1,R4
	subi R1, 39
	drw  R0,R1,R6
	subi R1, 8
	drw  R0,R1,R6
	subi R0, 18
	addi R1, 37
	drw  R0,R1,R5
	addi R0, 7
	drw  R0,R1,R5
	subi R0, 10
	drw  R0,R1,R6
	pop  R1
	pop  R0
	ret
Draw5:
	push R0
	push R1
	flip 0,0
	addi R1, 12
	drw  R0,R1,R4
	addi R0, 5
	subi R1, 12
	drw  R0,R1,R5
	addi R0, 16
	drw  R0,R1,R5
	subi R0, 20
	addi R1, 1
	drw  R0,R1,R6
	addi R0, 7
	addi R1, 30
	drw  R0,R1,R5
	subi R0, 7
	drw  R0,R1,R6
	addi R0, 30
	flip 1,0
	drw  R0,R1,R2
	flip 1,1
	addi R1, 31
	drw  R0,R1,R2
	flip 0,1
	subi R0,31
	drw  R0,R1,R2
	pop  R1
	pop  R0
	ret
Draw6:
	push R0
	push R1
	flip 0,0
	drw  R0,R1,R2
	flip 1,0
	addi R0, 31
	drw  R0,R1,R2
	flip 1,0
	subi R0, 31
	addi R1, 31
	drw  R0,R1,R4
	flip 0,0
	drw  R0,R1,R2
	addi R0, 1
	addi R1, 5
	drw  R0,R1,R6
	addi R1, 3
	drw  R0,R1,R6
	flip 1,0
	addi R0, 30
	subi R1, 8
	drw  R0,R1,R2
	subi R0, 31
	addi R1, 31 
	flip 0,1
	drw  R0,R1,R2
	addi R0, 31
	flip 1,1
	drw  R0,R1,R2
	pop  R1
	pop  R0
	ret
Draw7:
	push R0
	push R1
	flip 0,0
	addi R0, 11
	addi R1, 15
	drw  R0,R1,R3
	addi R0, 7
	subi R1, 12
	drw  R0,R1,R3
	addi R0, 3
	subi R1, 5
	drw  R0,R1,R3
	subi R0, 17
	addi R1, 2	
	drw  R0,R1,R5
	addi R0, 14
	drw  R0,R1,R5
	addi R0, 8
	addi R1, 4
	drw  R0,R1,R6
	subi R0, 15
	addi R1, 43
	drw  R0,R1,R4
	addi R1, 5
	drw  R0,R1,R4
	pop  R1
	pop  R0
	ret
Draw8:
	push R0
	push R1
	flip 0,0
	drw  R0,R1,R2
	flip 1,0
	addi R0, 31
	drw  R0,R1,R2
	flip 0,1
	subi R0, 31
	addi R1, 31
	drw  R0,R1,R2
	flip 1,1
	addi R0, 31
	drw  R0,R1,R2
	flip 0,0
	subi R0, 31
	addi R1, 1
	drw  R0,R1,R2
	flip 1,0
	addi R0, 31
	drw  R0,R1,R2
	subi R0, 22
	drw  R0,R1,R6
	addi R0, 8
	drw  R0,R1,R6
	addi R0, 4
	drw  R0,R1,R6
	subi R0, 21
	addi R1, 31
	flip 0,1
	drw  R0,R1,R2
	addi R0, 31
	flip 1,1
	drw  R0,R1,R2
	pop  R1
	pop  R0
	ret	
Draw9:
	push R0
	push R1
	flip 0,0
	drw  R0,R1,R2
	flip 1,0
	addi R0, 31
	drw  R0,R1,R2
	flip 0,1
	subi R0, 31
	addi R1, 31
	drw  R0,R1,R2
	flip 1,1
	addi R0, 31
	drw  R0,R1,R2
	flip 0,0
	addi R1, 1
	drw  R0,R1,R4
	subi R1, 12
	drw  R0,R1,R6
	addi R1, 7
	drw  R0,R1,R6
	subi R0, 31
	addi R1, 36 
	flip 0,1
	drw  R0,R1,R2
	addi R0, 31
	flip 1,1
	drw  R0,R1,R2
	pop  R1
	pop  R0
	ret
DrawA:
	push R0
	push R1
	flip 0,0
	drw  R0,R1,R2
	flip 1,0
	addi R0, 31
	drw  R0,R1,R2
	flip 1,0
	subi R0, 31
	addi R1, 32
	drw  R0,R1,R4
	addi R1, 19
	drw  R0,R1,R4
	flip 0,0
	addi R0, 31
	subi R1, 19
	drw  R0,R1,R4
	addi R1, 19
	drw  R0,R1,R4
	subi R0, 19
	subi R1, 8
	drw  R0,R1,R5
	addi R0, 8
	drw  R0,R1,R5
	pop  R1
	pop  R0
	ret
DrawB:
	push R0
	push R1
	flip 1,0
	addi R0, 31
	drw  R0,R1,R2
	flip 0,0
	subi R0, 31
	addi R1, 12
	drw  R0,R1,R4
	flip 0,0
	addi R0, 5
	subi R1, 12
	drw  R0,R1,R5
	addi R0, 3
	drw  R0,R1,R5
	subi R0, 7
	addi R1, 1
	drw  R0,R1,R6
	subi R0, 1
	addi R1, 41
	drw  R0,R1,R4
	addi R1, 10
	drw  R0,R1,R4
	addi R0, 31
	subi R1, 21
	flip 1,1
	drw  R0,R1,R2
	flip 1,0
	addi R1, 1
	drw  R0,R1,R2
	subi R0, 23
	subi R1, 1
	drw  R0,R1,R5
	subi R0, 8
	drw  R0,R1,R6
	addi R1, 1
	drw  R0,R1,R6
	addi R0, 21
	drw  R0,R1,R6
	subi R0, 1
	drw  R0,R1,R6
	addi R0, 11
	addi R1, 31
	flip 1,1
	drw  R0,R1,R2
	subi R0, 27
	drw  R0,R1,R5
	addi R0, 3
	drw  R0,R1,R5
	subi R0, 7
	subi R1, 1
	drw  R0,R1,R6
	pop  R1
	pop  R0
	ret
DrawC:
	push R0
	push R1
	flip 0,0
	drw  R0,R1,R2
	flip 1,0
	addi R0, 31
	drw  R0,R1,R2
	flip 1,0
	subi  R0, 31
	addi  R1, 32
	drw  R0,R1,R4
	flip 0,0
	addi R1, 31
	flip 0,1
	drw  R0,R1,R2
	addi R0, 31
	flip 1,1
	drw  R0,R1,R2
	pop  R1
	pop  R0
	ret
DrawD:
	push R0
	push R1
	flip 1,0
	addi R0, 31
	drw  R0,R1,R2
	flip 0,0
	subi R0, 31
	addi R1, 12
	drw  R0,R1,R4
	flip 0,0
	addi R0, 5
	subi R1, 12
	drw  R0,R1,R5
	addi R0, 3
	drw  R0,R1,R5
	subi R0, 7
	addi R1, 1
	drw  R0,R1,R6
	subi R0, 1
	addi R1, 41
	drw  R0,R1,R4
	addi R1, 10
	drw  R0,R1,R4
	subi R1, 20
	flip 0,0
	addi R0, 31
	drw  R0,R1,R4
	addi R1, 31
	flip 1,1
	drw  R0,R1,R2
	subi R0, 27
	drw  R0,R1,R5
	addi R0, 3
	drw  R0,R1,R5
	subi R0, 7
	subi R1, 1
	drw  R0,R1,R6
	pop  R1
	pop  R0
	ret
DrawE:
	push R0
	push R1
	flip 0,0
	addi R1, 12
	drw  R0,R1,R4
	addi R1, 24
	drw  R0,R1,R4
	addi R1, 16
	drw  R0,R1,R4
	addi R0, 5
	subi R1, 52
	drw  R0,R1,R5
	subi R0, 4
	addi R1, 2
	drw  R0,R1,R6
	addi R0, 20
	subi R1, 2
	drw  R0,R1,R5
	subi R0, 16
	addi R1, 32
	drw  R0,R1,R5
	addi R0, 5
	drw  R0,R1,R5
	subi R0, 9
	drw  R0,R1,R6
	addi R1, 1
	drw  R0,R1,R6
	addi R0, 4
	subi R1, 1
	flip 0,1
	addi R1, 31
	drw  R0,R1,R5
	subi R0, 4
	subi R1, 1
	drw  R0,R1,R6
	addi R0, 20
	addi R1, 1
	drw  R0,R1,R5
	pop  R1
	pop  R0
	ret
DrawF:
	push R0
	push R1
	flip 0,0
	addi R1, 12
	drw  R0,R1,R4
	addi R1, 24
	drw  R0,R1,R4
	addi R1, 16
	drw  R0,R1,R4
	addi R0, 5
	subi R1, 52
	drw  R0,R1,R5
	subi R0, 4
	addi R1, 2
	drw  R0,R1,R6
	addi R0, 20
	subi R1, 2
	drw  R0,R1,R5
	subi R0, 16
	addi R1, 32
	drw  R0,R1,R5
	addi R0, 5
	drw  R0,R1,R5
	subi R0, 9
	drw  R0,R1,R6
	addi R1, 1
	drw  R0,R1,R6
	addi R0, 4
	subi R1, 1
	pop  R1
	pop  R0
	ret
	
;importbin MiniMaxiFont.bmp.bin 0 3075 FontSprites
;importbin MiniMaxiFont.bmp.PAL 0 48 Palette