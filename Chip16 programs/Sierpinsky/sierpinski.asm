; Sierpinski triangle fractal demo
; using the chaos game method. 
; Written by Magnus Wedmark, 2013

SpriteSize equ $0101        ; 2 x 1 pixel
MaxPoints equ 640
;----------------------------------------------------------------
; R0 = Current X
; R1 = Current Y
; R2 = Random point
; R3 = Address in :points
; R4 = Random X
; R5 = Random Y
; R6 = Current number of points on-screen
; R7 = Current Color Index
; R8 = temp reg
; R9 = temp reg
; RA = temp reg
; RB = used when showing multi-color tri
; RC = temp
; RD = points/frame
;----------------------------------------------------------------
:main
; init program
	ldi RD, MaxPoints
	spr SpriteSize
:loop
	ldi R8, Points
	ldm R0, R8
	addi R8,2 
	ldm R1, R8
:shortloop
	call SetPixel
	cmpi R7, 1
	ja	ColorNotZero ; if RandomColor=0 => Make Multi-color tri
	; set new color each point on triangle
	rnd RC, 0xF
	mov R8, RC
	shl R8, 4
	or  R8, RC
	stm	R8, PixelSprite
	
:ColorNotZero
	call RandomizePoint
	addi R6, 1
	cmp R6, RD
	jnz  shortloop
	vblnk
	cls
	ldi	 R6, 0
	call RandomizeColor
	call RandomizeCoord
	jmp loop
	
:RandomizePoint ; R0=xcoord R1=ycoord
; randomize next point: 0,1 or 2
	rnd R2, 2
	shl R2, 2
	ldi R3, Points
	add R3, R2
	ldm R4, R3 ; R0 = xcoord 
	addi R3, 2
	ldm R5, R3 ; R1 = ycoord
	; calculate half way between current and random point
	add	R0, R4
	shr	R0, 1
	add R1, R5
	shr R1, 1
	ret
	
:SetPixel ; R0=X R1=Y
	drw R0,R1, PixelSprite
	ret

:RandomizeColor ; R7=Random color R8=changed
	rnd R7, 0xF
	mov R8, R7
	shl R8, 4
	or  R8, R7
	stm	R8, PixelSprite
	ret

; function randomizes all coordinates for all 3 points (6 values)
:RandomizeCoord ;R9 RA RB = changed
	ldi RA, Points
	rnd R9, 319
	stm R9, RA
	addi RA, 2
	rnd R9, 240
	stm R9, RA
	addi RA, 2
	
	rnd R9, 319
	stm R9, RA
	addi RA, 2
	rnd R9, 240
	stm R9, RA
	addi RA, 2
	
	rnd R9, 319
	stm R9, RA
	addi RA, 2
	rnd R9, 240
	stm R9, RA
	addi RA, 2
	ret
	
:PixelSprite
db 	0xFF,0x00 ; single pixel sprite

:Points
db	0x00,0x00,0xF0,0x00
db	0xA0,0x00,0x01,0x00
db 	0x3F,0x01,0xF0,0x00
