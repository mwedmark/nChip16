; Single Pixel FPGA Debugging Demo
; Used to validate writing sprites at the right place in Framebuffer
; Written by Magnus Wedmark, 2021

SpriteSize equ $0101        ; 2 x 1 pixel
MaxPoints equ 65535 ;640
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
	LDI R0, 50
	LDI R1, 0
	SPR SpriteSize
:loop
	CLS
:nextpixel
	DRW R0,R1,PixelSprite
	ADDI R0, 1
	ADDI R1, 1
	CMPI R1, 240 
	JNZ nextpixel
:stop
	JMP stop
:PixelSprite
db 	0xF4,0x00 ; single pixel sprite

