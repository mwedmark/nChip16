; 320x240 fill screen 
; Written by Magnus Wedmark, 2015

SpriteSize 	equ $0101        ; 2 x 1 pixel
XSize		equ 320
YSize		equ 12 ; 8-10% of x86 CPU
; 8-9 makes full time for x86
;12 makes the full frame för Chip16
;----------------------------------------------------------------
; R0 = X for pixel set
; R1 = Y for pixel set
; R2 =  temp
; R3 = pointer to PCache
;----------------------------------------------------------------
main:
; init program
	spr SpriteSize
	cls
	ldi R0, 0
	ldi R1, 0
init:
	vblnk
	ldi R1, 0
yloop:
	ldi  R0, 0
xloop:
	drw  R0,R1,PixelSprite
	addi R0, 1
	cmpi R0, XSize
	jnz  xloop
	addi R1, 1
	cmpi R1, YSize
	jnz  yloop
	jmp  init
	
finished:
	jmp finished
PixelSprite:
db 	0xFF,0x00 ; pixel sprite
