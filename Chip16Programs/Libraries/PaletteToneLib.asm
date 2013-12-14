;Library: PaletteTone
; TonePalette - Used to get a smooth transition from one palette to another
; CopyPalette - Copy a palette (48 bytes) from source to destination
; CreateBwPalette - Create a grey version of a complete palette which gives
;                   the apperance of a Black-and-White version of an image.
; uses: R0,R1,R2,R3,R4,R5,R6,R7,R8,R9 but all registers are backedup!
ptPaletteSize			equ     48

; tone from palette address in R0 to palette address in R1 (R0=>R1)
; TODO: Add using R2 as speed control
TonePalette:
		pushall
		push R1
		ldi  R1, WorkingPalette 
		call CopyPalette
		pop  R7 ; R7 - target
		ldi  R0, WorkingPalette ; R0 - source
		pal  WorkingPalette
		ldi  R8, 255
PaletteToningMainLoop:
		mov  R3, R0
		mov  R6, R3
		mov  R2, R7
		addi R2, ptPaletteSize
		addi R3, ptPaletteSize
		vblnk
		;vblnk ; add for better effect with Color=>BW
		;vblnk
		;vblnk
		;vblnk
ToneLoop:
		subi R2, 1
		mov  R0, R2
		call ReadByte
		mov  R4, R1 ; mov target value to r4
		subi R3, 1
		mov  R0, R3
		call ReadByte
		mov  R5, R1 ; mov current value to r5
		cmp  R4, R5
		jz   ChangesDone
		jl   ToneMinus
TonePlus:
		addi R5, 1 ; speed control, just now program only handles 1
		jp   WriteTone
ToneMinus:
		subi R5, 1 ; speed control, just now program only handles 1
WriteTone:
		mov  R1, R5
		mov  R0, R3
		call WriteByte
ChangesDone:
		cmp  R3, R6 ; last color in palette?
		jnz  ToneLoop
		subi R8, 1
		cmpi R8, 0
		jnz  PaletteToningMainLoop
		popall
		ret
;ToneStopLoop:
;		jmp ToneStopLoop

 ; [R0] => [R1]
 ; Move palette (48 bytes) pointed to by R0 to R1
 ; No registers destroyed
CopyPalette:
		push R0
		push R1
		push R2
		push R3
		mov  R3, R1
		addi R0, PaletteSize
		addi R1, PaletteSize
CopyPaletteLoop:
		subi R1,2
		subi R0,2
		ldm  R2, R0
		stm  R2, R1
		cmp  R3, R1
		jnz  CopyPaletteLoop
		pop R3
		pop R2
		pop R1
		pop R0
		ret 

; [R1] = BW[R0]
; Create black-and-white version of palette (48 bytes) 
; at R0 in address pointed to by R1
; No registers destroyed
CreateBwPalette:
		pushall ; only R0-R9 is needed but saves space
		mov  R3, R1 ;used for end-of-loop
		mov  R4, R1 ; R4 used as target pointer
		mov  R5, R0 ; R5 used as source pointer
		addi R5, PaletteSize
		addi R4, PaletteSize
BwPaletteLoop:
; Read all parts of current color (R G B)
		subi R5,1
		mov  R0, R5
		call ReadByte
		mov  R6, R1 ; R6 = B
		subi R5,1
		mov  R0, R5
		call ReadByte
		mov  R7, R1 ; R7 = G
		subi R5,1
		mov  R0, R5
		call ReadByte
		mov  R8, R1 ; R8 = R
; calculate the average of the current color
		mov  R9, R6
		add  R9, R7
		add  R9, R8
		divi R9, 3 ; R9 = (R+G+B)/3
; write the average as value for both R G B => Grey
		;stm  R9, R4	; B = R8
		mov  R0, R4
		mov  R1, R9
		subi R4, 1
		mov  R0, R4
		call WriteByte
		subi R4, 1
		mov  R0, R4
		call WriteByte
		subi R4, 1
		mov  R0, R4
		call WriteByte
		cmp  R3, R4
		jnz  BwPaletteLoop
		popall
		ret 
		
FromPalette:
db 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
db 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
db 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
db 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
ToPalette:
db 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
db 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
db 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
db 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
ZeroPalette:
db 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
db 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
db 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
db 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
WorkingPalette:
db 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
db 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
db 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
db 0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00