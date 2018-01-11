; Fullscreen 320x240 mandelbrot 
; using straight-forward shifting for floating point values
; Written by Magnus Wedmark, 2015

SpriteSize 	equ $0101        ; 2 x 1 pixel
XSize		equ 320
YSize		equ 240
f			equ 5 ; 2^5=32
;----------------------------------------------------------------
; R0 = X for pixel set
; R1 = Y for pixel set
; Rx = 
;----------------------------------------------------------------
main:
; init program
	pal Palette
	spr SpriteSize
	cls
	ldi R1, 0
yLoop:
	ldi R0, 0
xLoop:
	call CalcPixel
	muli  R2, 13
	stm  R2, PixelSprite
	call SetPixel
	addi R0, 1
	cmpi R0, XSize
	jnz  xLoop
	addi R1, 1
	cmpi R1, YSize
	jnz  yLoop
finished:
	jmp  finished
	
SetPixel: ; R0=X R1=Y
	drw R0, R1, PixelSprite
	ret

; R0 = x in
; R1 = y in
; R2 = pixelcolor out
CalcPixel:
; R3 = fixpt(xmin)
; R4 = fixpt(ymin)
; R5 = i
; R6 = xn
; R7 = x0
; R8 = y0
; R9 = p
; RA = q
; RC = temp
;const double xmin = -2.3, ymin = -1.3, xmax = 0.8, ymax = 1.3;

xmin equ -73 ; -2.3<<f
ymin equ -41  ; -1.3<<f
xmax equ 25   ; 0.8<<f
ymax equ 41   ; 1.3<<f
xs   equ 98  ;25+73 ;xmax-xmin
ys	 equ 82  ;41+41  ;ymax-ymin
fixpt4 equ 128 ;4<<f
fixpt2 equ 64 ;2<<f

maxit equ 64
;const int maxIterations = 64; 

;short xn = 0;
ldi R6, 0
;short x0 = 0;
ldi R7, 0
;short y0 = 0;
ldi R8, 0
;short i = 0;
ldi R5, 0

;short p = (short)(fixpt(xmin) + (x*xs/320));
mov  R9, R0
muli R9, xs
divi R9, XSize
addi R9, xmin

;short q = (short)(fixpt(ymin) + (y*ys/240));
mov  RA, R1
muli RA, ys
divi RA, YSize
addi RA, ymin

;while ((mul(xn, xn) + mul(y0, y0)) < fixpt(4) && ++i < maxIterations)
nextIter:
addi R5, 1
cmpi R5, maxit
jz   endOfIters ; ++i < maxIterations

; (mul(xn, xn) + mul(y0, y0)) < fixpt(4)
mov RD, R6
mov RE, R6
call mul
mov RC, RF

mov  RD, R8
mov  RE, R8
call mul
add  RC, RF
cmpi RC, fixpt4
jge  endOfIters

;{
;	xn = (short)(mul((short)(x0 + y0),(short)(x0 - y0)) + p)
mov RD, R7
add RD, R8
mov RE, R7
sub RE, R8
call mul
add RF, R9
mov R6, RF
;	y0 = (short)(mul(fixpt(2), mul(x0, y0)) + q);
mov RD, R7
mov RE, R8
call mul
mov RD, RF
ldi RE, fixpt2
call mul
add RF, RA
mov R8, RF
;	x0 = xn;
mov R7, R6
jmp  nextIter
;}
endOfIters:
;if (i == maxIterations) i = 0;
cmpi R5, maxit
jnz notZero
ldi R5, 0x0
notZero:
mov R2, R5
;var rc = (i * 32) & 0xFF;
;var gc = (i * 16) & 0xFF;
;return Color.FromArgb(255, rc, gc, rc);
ret

; --------------
; RD = x
; RE = y
; RF = result
mul:
mov RF, RD
mul RF, RE
sar RF, f
;divi RF, 32
ret
;private short mul(short x, short y)
;{
;	return (short)((x*y) >> f);
;}


include ..\Libraries\TimeLib.asm

:PixelSprite
db 	0x00,0x00 ; single pixel sprite

:Palette
db	0x00,0x00,0x00
db	0x20,0x00,0x20
db	0x40,0x00,0x40
db	0x60,0x00,0x60
db	0x80,0x00,0x80
db	0xA0,0x00,0xA0
db	0xC0,0x00,0xC0
db	0xE0,0x00,0xE0
db	0xF2,0x00,0xF2
db	0xF4,0x00,0xF4
db	0xF6,0x00,0xF6
db	0xF8,0x00,0xF8
db	0xFA,0x00,0xFA
db	0xFC,0x00,0xFC
db	0xFE,0x00,0xFE
db	0xFF,0x00,0xFF