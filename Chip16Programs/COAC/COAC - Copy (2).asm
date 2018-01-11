; ChipOnAChip - Chip-8 Emulator for Chip16
; Written by Magnus Wedmark, 2013

C8SpriteSize 		equ $0402        ; 4 x 4 pixel
C8FillSize			equ $0804		 ; 8 x 8 pixel for fill
SCSpriteSize 		equ $0201        ; 2 x 2 pixel
MaxResX				equ 64
MaxResY				equ 32
C8Font				equ	0x8000   ; chip8 font location
SCFont				equ 0x8100   ; SuperChip font location
C8Memory			equ 0x8200 	 ; original is at 0x0200	
;----------------------------------------------------------------
; R7 = raw opcode
; R8 = opcode select (4 MSB of opcode)
; R9 = nnn
; RA = x
; RB = y
; RC = kk
; RD = n
; RE = PC (using the +0x8000 offset)
; RF = SP (Locked! do not USE!)
;----------------------------------------------------------------
main:
; init Chip16 environment
	bgc 2
	call FillScreenWith2
	spr  C8SpriteSize
	pal Palette
; ** init Chip8 virtual machine **
	call Reset
	call CopyFont
	;ldi  R0, 63
	;ldi  R1, 31
	;ldi  R2, 1
	;call SetPixelInBuffer
	;call DrawScreen
; load standard font into chip-8 memory
; ** load c8 program into executable memory **
	call LoadChip8Program
InitMainLoop:
	ldi  R0, 12
MainLoop:
; ** start executing Chip-8 code **	
	call InterpretOpcode
	call ExecuteOpcode
	subi R0, 1
	cmpi R0, 0
	jnz MainLoop
	call UpdateTimers
	;call DrawScreen
	vblnk
	jmp InitMainLoop  
ExecuteOpcode:
	cmpi R8, 0x0 ; 0*** - Misc family of opcodes
	jz 	 MiscFam
	cmpi R8, 0x1 ; 1nnn - JUMP nnn
	jz	 opJP
	cmpi R8, 0x2 ; 2nnn - CALL addr
	jz 	 opCALL
	cmpi R8, 0x3 ; 3xkk - SE Vx, byte
	jz	 opSE
	cmpi R8, 0x4 ; 4xkk - SE Vx, byte
	jz	 opSNE
	cmpi R8, 0x5 ; 5xy0 - SKEQ Vx, Vy
	jz   opSKEQ
	cmpi R8, 0x6 ; 6xkk - LD Vx, byte
	jz   opLDx
	cmpi R8, 0x7 ; 7xkk - ADD Vx, byte
	jz   opADDx
	cmpi R8, 0x8 ; 8xy* - Family of XY opcodes
	jz   Fam8xy
	cmpi R8, 0x9 ; 9xy0 - SNE Vx, Vy
	jz   Fam9xy
	cmpi R8, 0xA ; Annn - LD I, addr
	jz 	 opLDI
	cmpi R8, 0xC ; Cxkk - RAND vx, kk
	jz   opRAND
	cmpi R8, 0xD ; Dxyn - DRW Vx, Vy, nibble
	jz	 opDRW
	cmpi R8, 0xE ; Exkk - Keyboard family
	jz   FamKbd
	cmpi R8, 0xF ; F*** - Fx family of opcodes
	jz   FxFam
	jmp Error1 ; invalid opcode found
	
FinishOpcode:
	; make some last touches to executed opcode
	; add to PC if not jump inst. 
	addi RE, 2
	ret

Fam8xy:
	andi  RC, 0x0F
	cmpi  RC, 0x00 ; 8xy0 - LD Vx, Vy
	jz    opLDxy
	cmpi  RC, 0x01 ; 8xy1 - OR Vx, Vy
	jz    opORxy
	cmpi  RC, 0x02 ; 8xy2 - AND Vx, Vy
	jz    opANDxy
	cmpi  RC, 0x03 ; 8xy3 - XOR Vx, Vy
	jz    opXORxy
	cmpi  RC, 0x04 ; 8xy4 - ADD Vx, Vy
	jz    opADDxy
	cmpi  RC, 0x05 ; 8xy5 - SUB Vx, Vy
	jz    opSUBxy
	cmpi  RC, 0x06 ; 8xy6 - SHR Vx, Vy
	jz    opSHRxy
	cmpi  RC, 0x07 ; 8xy7 - SUBN Vx, Vy
	jz    opSUBNxy
	cmpi  RC, 0x0E ; 8xyE - SHL Vx, Vy
	jz    opSHLxy
	jmp   Error1 ; invalid opcode found

Fam9xy:
	andi RC, 0x0F
	cmpi RC, 0x0 ; 9xy0 - SNE Vx, Vy
	jz	 opSNExy
	
FamKbd:
	cmpi  RC, 0x9E
	jz    opSkpr
	cmpi  RC, 0xA1
	jz	  opSkup
	jmp   Error1 ; invalid opcode found

FxFam:
	cmpi  RC, 0x07
	jz    opGDelay
	cmpi  RC, 0x0A
	jz	  opWaitKey
	cmpi  RC, 0x15
	jz    opSDelay
	cmpi  RC, 0x18
	jz    opSSound
	cmpi  RC, 0x1E
	jz    opADIvx
	cmpi  RC, 0x29
	jz	  opLDCHI
	cmpi  RC, 0x30
	jz    opLDXCHI
	cmpi  RC, 0x33
	jz    opBCDvx
	cmpi  RC, 0x55
	jz    opSTv0vx
	cmpi  RC, 0x65
	jz    opLDvxI	
	cmpi  RC, 0x75
	jz	  opSTRv0vx
	cmpi  RC, 0x85
	jz	  opLDRv0vx
	jmp Error1 ; invalid opcode found

	MiscFam:
	cmpi RC, 0xE0
	jz	 opCLS
	cmpi RC, 0xEE
	jz	 opRET
	cmpi RC, 0xFE
	jz   opSetLow
	cmpi RC, 0xFF
	jz   opSetHigh
Error1:
	jmp Error1 ; invalid opcode found
	
opCLS:
	;call FillScreenWith2
	call ClearBuffer
	cls
	jmp FinishOpcode

; 00EE - RET
; return from subroutine
opRET:
	ldm  RE, RF
	subi RF, 2
	jmp FinishOpcode

; 00FE - SetLow
; Set Chip8 graphics with 64*32 resolution
opSetLow:
	push R0
	ldi  R0, 0x00
	stm  R0, GraphicsMode
	spr  C8SpriteSize
	pop  R0
	jmp FinishOpcode

; 00FF - SetHigh
; Set SuperChip graphics with 128*64 resolution
opSetHigh:
	push R0
	ldi  R0, 0x01
	stm  R0, GraphicsMode
	spr  SCSpriteSize
	pop  R0
	jmp FinishOpcode
	
; 1nnn - JP addr
; Jump to location nnn.
opJP:
	mov RE, R9
	ori RE, 0x8000
	ret

; 2nnn - CALL addr
; Call subroutine at nnn.	
opCALL:
	addi RF, 2
	stm  RE, RF
	jmp  opJP
	
; 3xkk - SE Vx, byte
; Skip next instruction if Vx = kk.
opSE:
	push R0
	push R1
	mov  R1, RA ; move x into R1 before call to ReadVREGx
	call ReadVREGx
	cmp  R0, RC
	jnz  opSE_NoSkip
	addi RE, 2
opSE_NoSkip:
	pop  R1
	pop  R0
	jmp FinishOpcode

; 4xkk - SNE Vx, byte
; Skip next instruction if Vx = kk.	
opSNE:
	push R0
    push R1
	mov  R1, RA ; move x into R1 before call to ReadVREGx
	call ReadVREGx
	cmp  R0, RC
	jz   opSNE_NoSkip
	addi RE, 2
opSNE_NoSkip:
	pop R1
	pop R0
	jmp FinishOpcode

; 5xy0 - SKEQ Vx, Vy
; skip next instruction if register VX == register VY
opSKEQ:
	push R0
	push R1
	push R2
	mov  R1, RA
	call ReadVREGx
	mov  R2, R0
	mov  R1, RB
	call ReadVREGx
	cmp  R2, R0
	jnz  SKEQNE
	addi RE, 2
SKEQNE:
	pop  R2
	pop  R1
	pop  R0
	jmp  FinishOpcode
	
; 6xkk - LD Vx, byte
; Set Vx = kk
opLDx:
	push R0
	push R1
	mov  R0, RC     ; R0 = kk
	mov  R1, RA 	; move x into R1 before call to WriteVREGx
	call WriteVREGx ; Vx = kk
	pop  R1
	pop  R0
	jmp  FinishOpcode

; 7xkk - ADD Vx, kk
; Set Vx = Vx + kk
opADDx:
	push R0
	push R1
	mov  R1, RA
	call ReadVREGx
	add  R0, RC
	call WriteVREGx
	pop  R1
	pop  R0
	jmp  FinishOpcode
	
; 8xy0 - LD Vx, Vy
; Set Vx = Vy
opLDxy:
	push R0
	push R1
	mov  R1, RB 	; move y into R1 before call to ReadVREGx
	call ReadVREGx 	; R0 = Vy value
	mov  R1, RA 	; move x into R1 before call to WriteVREGx
	call WriteVREGx ; Vx = Vy
	pop  R1
	pop  R0
	jmp  FinishOpcode

; 8xy1 - OR Vx, Vy
; Set Vx = Vx OR Vy
opORxy:
	push R0
	push R1
	push R2
	mov  R1, RB 	; move y into R1 before call to ReadVREGx
	call ReadVREGx 	; R0 = Vy value
	mov  R2, R0		; R2 = R0
	mov  R1, RA
	call ReadVREGx 	; R0 = Vx value
	or   R2, R0
	andi R2, 0xFF
	mov  R0, R2     ; move res. into R1 before call to WriteVREGx
	call WriteVREGx ; Vx = Vx + Vy
	pop  R2
	pop  R1
	pop  R0
	jmp  FinishOpcode

; 8xy2 - AND Vx, Vy
; Set Vx = Vx AND Vy
opANDxy:
	push R0
	push R1
	push R2
	mov  R1, RB 	; move y into R1 before call to ReadVREGx
	call ReadVREGx 	; R0 = Vy value
	mov  R2, R0		; R2 = R0
	mov  R1, RA
	call ReadVREGx 	; R0 = Vx value
	and  R2, R0
	andi R2, 0xFF
	mov  R0, R2     ; move res. into R1 before call to WriteVREGx
	call WriteVREGx ; Vx = Vx + Vy
	pop  R2
	pop  R1
	pop  R0
	jmp  FinishOpcode

; 8xy3 - XOR Vx, Vy
; Set Vx = Vx XOR Vy
opXORxy:
	push R0
	push R1
	push R2
	mov  R1, RB 	; move y into R1 before call to ReadVREGx
	call ReadVREGx 	; R0 = Vy value
	mov  R2, R0		; R2 = R0
	mov  R1, RA
	call ReadVREGx 	; R0 = Vx value
	xor  R2, R0
	andi R2, 0xFF
	mov  R0, R2     ; move res. into R1 before call to WriteVREGx
	call WriteVREGx ; Vx = Vx + Vy
	pop  R2
	pop  R1
	pop  R0
	jmp  FinishOpcode
	
; 8xy4 - ADD Vx, Vy
; Set Vx = Vx + Vy, set VF = carry.
opADDxy:
	push R0
	push R1
	push R2
	mov  R1, RB 	; move y into R1 before call to ReadVREGx
	call ReadVREGx 	; R0 = Vy value
	mov  R2, R0		; R2 = R0
	mov  R1, RA
	call ReadVREGx 	; R0 = Vx value
	add  R2, R0
	mov  R0, R2     ; move res. into R1 before call to WriteVREGx
	call WriteVREGx ; Vx = Vx + Vy
	shr  R2, 0x8
	mov  R0, R2
	ldi  R1, 0xF
	call WriteVREGx ; VF = (Vx+Vy)>>8 (Write bit 8 to VF)
	pop  R2
	pop  R1
	pop  R0
	jmp  FinishOpcode

; 8xy5 - SUB Vx, Vy
; Set Vx = Vx - Vy
opSUBxy:
	push R0
	push R1
	push R2
	push R3
	push R4
	mov  R1, RB 	; move y into R1 before call to ReadVREGx
	call ReadVREGx 	; R0 = Vx value
	mov  R2, R0		; R2 = VREG[y]
	mov  R1, RA
	call ReadVREGx 	; R0 = Vx value
	mov  R4, R0		; R4 = VREG[x]
; compare X and Y, save VF value	
	ldi  R0, 1	
	cmp  R4, R2
	jge	 GreaterThanSub
	ldi  R0, 0
GreaterThanSub:
	ldi  R1, 0xF
	call WriteVREGx ; VF = is X>Y
; VREG[X] = VREG[X]-VREG[Y]	
	mov  R1, RA ; R1 = x before call WriteVREGx
	mov  R0, R4
	sub  R0, R2
	call WriteVREGx ; Vx = Vx - Vy
	pop  R4
	pop  R3
	pop  R2
	pop  R1
	pop  R0
	jmp  FinishOpcode

; 8xy6 - SHR Vx{, Vy}
; Set Vx = Vx SHR 1, set VF = NOT borrow.
opSHRxy:
	push R0
	push R1
	push R2
	mov  R1, RA
	call ReadVREGx
	mov  R2, R0
	andi R0, 0x1
	ldi  R1, 0xF
	call WriteVREGx ; Write LSB to VF before SHR
	shr  R2, 1
	mov  R0, R2
	mov  R1, RA
	call WriteVREGx ; Write LSB to VF before SHR
	pop  R2
	pop  R1
	pop  R0
	jmp  FinishOpcode

; 8xy7 - Vx = Vy - Vx
; Set Vx = Vy - Vx, set VF = NOT borrow.
opSUBNxy:
	push R0
	push R1
	push R2
	push R3
	push R4
	mov  R1, RA 	; move y into R1 before call to ReadVREGx
	call ReadVREGx 	; R0 = Vx value
	mov  R2, R0		; R2 = VREG[y]
	mov  R1, RB
	call ReadVREGx 	; R0 = Vx value
	mov  R4, R0		; R4 = VREG[x]
; compare X and Y, save VF value	
	ldi  R0, 1	
	cmp  R4, R2
	jge	 GreaterThanNSub
	ldi  R0, 0
GreaterThanNSub:
	ldi  R1, 0xF
	call WriteVREGx ; VF = is X>Y
; VREG[X] = VREG[X]-VREG[Y]	
	mov  R1, RA ; R1 = x before call WriteVREGx
	mov  R0, R4
	sub  R0, R2
	call WriteVREGx ; Vx = Vx - Vy
	pop  R4
	pop  R3
	pop  R2
	pop  R1
	pop  R0
	jmp  FinishOpcode

; 8xyE - SHL Vx, Vy
; Set Vx = Vx SHL 1.
opSHLxy:
	push R0
	push R1
	push R2
	mov  R1, RA
	call ReadVREGx
	mov  R2, R0
	andi R0, 0x80
	shr  R0, 7
	ldi  R1, 0xF
	call WriteVREGx ; Write MSB to VF before SHL
	shl  R2, 1
	mov  R0, R2
	mov  R1, RA
	call WriteVREGx ; Write LSB to VF before SHR
	pop  R2
	pop  R1
	pop  R0
	jmp  FinishOpcode

; 9xy0 - SNE Vx, Vy
; Skip next instruction if Vx != Vy.
opSNExy:
	push R0
	push R1
	push R2
	mov  R1, RA
	call ReadVREGx
	mov  R2, R0
	mov  R1, RB
	call ReadVREGx
	cmp  R2, R0
	jz   SNEEQ
	addi RE, 2
SNEEQ:
	pop  R2
	pop  R1
	pop  R0
	jmp  FinishOpcode

; Annn - LD I, addr
; Set I = nnn.
opLDI:
	stm R9, I
	jmp  FinishOpcode

; Cxkk - RND Vx, byte	
; Set Vx = random byte AND kk.
opRAND:
	push R0
	push R1
	rnd  R0, 0xFFFF
	and  R0, RC
	mov  R1, RA
	call WriteVREGx
	pop  R1
	pop  R0
	jmp  FinishOpcode

; Dxyn - DRW Vx, Vy, nibble
; Display n-byte sprite starting at memory 
; location I at (Vx, Vy), set VF = collision.
; R0 = Current X coord to draw
; R1 = Current Y coord to draw
; R2 = Current bit/pixel value
; R3 = Current address for sprite data target
; R4 = Org. value of [I]
; R5 = Current sprite data value (shifted)
opDRW:
	push R0
	push R1
	push R2
	push R3
	push R4
	push R5
	; n number of rows with start at adr. [I], 
	; always 8 pixel
	ldm	 R3, I
	addi R3, 0x8000
	mov  R4, R3 ; save org. value of [I]
	mov  R1, RA
	call ReadVREGx ; R0 = V[R1] x
	;add  R3, R0 ; left top corner of target addr
	push R0 ; save X-coord
	mov  R1, RB
	call ReadVREGx ; R0 = V[R1] y
	mov  R1, R0
	mov  R7, R1 ;snatching raw opcode??
	pop  R0 ; restore X-coord
	mov  R6, R0 ; use R6 as back-up
	add  R3, RD ; set start addr=y+n	
	add  R1, RD ; set start Ycoord=y+n
DrawRowLoop:
	mov	 R0, R6 ; restore start coord
	;add  R1, RD ; set start Ycoord=y+n
	subi R3, 1
	subi R1, 1
	ldm  R5, R3 ; read actual sprite data
	mov  R2, R5
	;mov  R0, RA already added
	addi R0, 8   ; set start Xcoord=x+8
DrawPixelLoop:
	subi R0, 1   ; but start loop as Xcoord=x+7
	andi R2, 0x1
	;call SetChip8Pixel
	call SetPixelInBuffer
	shr  R5, 1
	mov	 R2, R5
	cmp  R0, R6
	jnz  DrawPixelLoop
	cmp  R7, R1
	jl   DrawRowLoop
	pop  R5
	pop  R4
	pop  R3
	pop  R2
	pop  R1
	pop  R0
	jmp  FinishOpcode

; Er9E - skip if key (register rk) pressed (The key is a key number, see the chip-8 documentation)
;Bit[0] - Up
;Bit[1] - Down
;Bit[2] - Left
;Bit[3] - Right
;Bit[4] - Select
;Bit[5] - Start
;Bit[6] - A
;Bit[7] - B
opSkpr:
	push R0
	push R1
	push R2
	mov  R1, RC
	call ReadVREGx
	ldm  R2, 0xFFF0 ; read joystick
	cmpi R0, 2
	jz   ChkKeyUp
	cmpi R0, 4
	jz   ChkKeyLeft
	cmpi R0, 6
	jz   ChkKeyRight
	cmpi R0, 8
	jz   ChkKeyDown
	cmpi R0, 0xF
	jz   ChkKeyFire
	jmp  EndCheckKey
ChkKeyUp:
	tsti R2, 1
	jnz  ChkKeySkipNext
	jmp  EndCheckKey
ChkKeyLeft:
	tsti R2, 4
	jnz  ChkKeySkipNext
	jmp  EndCheckKey
ChkKeyRight:
	tsti R2, 8
	jnz  ChkKeySkipNext
	jmp  EndCheckKey
ChkKeyDown:
	tsti R2, 16
	jnz  ChkKeySkipNext
	jmp  EndCheckKey
ChkKeyFire:
	tsti R2, 64
	jnz   ChkKeySkipNext
	jmp  EndCheckKey
ChkKeySkipNext:
	addi RE, 2
EndCheckKey:
	pop  R2
	pop  R1
	pop  R0
	jmp  FinishOpcode

; ErA1 - skip if key (register rk) not pressed
opSkup:
	jmp FinishOpcode

; Fr07 - get delay timer into vr
opGDelay:
	push R0
	push R1
	ldm  R0, DelayTimer
	mov  R1, RA
	call WriteVREGx
	pop  R1
	pop  R0
	jmp FinishOpcode

; Fr0A - Wait for a key press, store the value of the key in Vx.
opWaitKey:
	push R0
	push R1
	call WaitForKeyPress
	call ConvertKey
	cmpi R1, 0
	jz   opWaitKey
	mov  R0, R1
	mov  R1, RA ; was RC but probably wrong..
	call WriteVREGx
	pop  R1
	pop  R0
	jmp  FinishOpcode
	
WaitForKeyPress:
	ldm  R0, 0xFFF0
	cmpi R0, 0
	jz  WaitForKeyPress
	ret

; gets value in R0 and converts it to Chip8 value
;Bit[0] - Up 	 => 2
;Bit[1] - Down   => 8
;Bit[2] - Left   => 4
;Bit[3] - Right  => 6
;Bit[4] - Select => E
;Bit[5] - Start =>  F
;Bit[6] - A => A
;Bit[7] - B => B
ConvertKey:
	ldi  R1, 0 ; init R1
	tsti R0, 1 ; CheckUp
	jz  ChkKey2Down
	ldi  R1, 2
	ret
ChkKey2Down:
	tsti R0, 2
	jz  ChkKey2Left
	ldi  R1, 8
	ret
ChkKey2Left:
	tsti R0, 4
	jz  ChkKey2Right
	ldi  R1, 4
	ret
ChkKey2Right:
	tsti R0, 8
	jz  ChkKey2Select
	ldi  R1, 6
	ret
ChkKey2Select:
	tsti R0, 16
	jz  ChkKey2Start
	ldi  R1, 0xE
	ret
ChkKey2Start:
	tsti R0, 32
	jz  ChkKey2A
	ldi  R1, 0xF
	ret
ChkKey2A:
	tsti R0, 64
	jz  ChkKey2B
	ldi  R1, 0xA
	ret
ChkKey2B:
	tsti R0, 128
	jz  ChkKey2End
	ldi  R1, 0xB
ChkKey2End:
	ret
	
; Fr15 - set the delay timer to value in vr
opSDelay:
	push R0
	push R1
	mov  R1, RA
	call ReadVREGx
	stm  R0, DelayTimer ; DelayTimer = RC
	pop  R1
	pop  R0
	jmp  FinishOpcode
	
; Fr18 - set the sound timer to vr
opSSound:
	stm RC, SoundTimer ; SoundTimer = RC
	jmp FinishOpcode
	
; Fr1E - add register vr to the index register, if I+Vr > 0xFFF => VF=1
opADIvx:
	push R0
	push R1
	push R2
	push R3
	push R4
	push R5
	mov  R1, RA
	call ReadVREGx
	mov  R5, R0
	ldm  R2, I
	ldi  R4, 0
	mov  R3, R2
	add  R3, R0
	cmpi R3, 0xFFF
	jle  ADIResNotOverflow
	ldi  R4, 1
ADIResNotOverflow:
	mov  R0, R4
	ldi  R1, 0xF
	call WriteVREGx
	andi R3, 0xFFF
	stm  R3, I
	pop  R5
	pop  R4
	pop  R3
	pop  R2
	pop  R1
	pop  R0
	jmp  FinishOpcode
	
; Fx29 - Load startaddr. of char x (0-F) into I
opLDCHI:
	push R0
	push R1
	push R3
	;ldi	 R3, C8Font ;Chip8Font is the original place
	ldi  R3, 0 ; 0x8000 is added later
	mov  R1, RA
	call ReadVREGx ; R0 = V[R1]
	muli R0, 5
	add  R3, R0
	stm  R3, I
	pop  R3
	pop  R1
	pop  R0
	jmp  FinishOpcode

; Fx30 - Load startaddr. of high-res char x (0-F) into I
opLDXCHI:
	push R0
	push R1
	push R3
	ldi  R3, 0x100 ; 0x8000 is added later
	mov  R1, RA
	call ReadVREGx ; R0 = V[R1]
	muli R0, 10
	add  R3, R0
	stm  R3, I
	pop  R3
	pop  R1
	pop  R0
	jmp  FinishOpcode	

; Fx33 - BCD	
;	Stores the Binary-coded decimal representation of VX
opBCDvx:
	push R0
	push R1
	push R2
	push R3
	mov R1, RA
	call ReadVREGx ; R0 = V[R1]
	mov  R1, R0 ; make a copy of original value
	divi R1, 100 ; VX/100 => Hundreds digit
	ldi  R2, I   ; first read value in I
	ldm  R3, R2  ; then read value I points to
	ori  R3, 0x8000
	stm  R1, R3  ; store [I]=R1
	muli R1, 100
	sub  R0, R1
	mov  R1, R0
	divi R1, 10 ; VX-hundreds/10 => Tenths digit
	addi R3, 1
	stm  R1, R3
	muli R1, 10
	sub  R0, R1
	addi R3, 1
	stm  R0, R3
	pop  R3
	pop  R2
	pop  R1
	pop  R0
	jmp FinishOpcode

; Fx55 - ST Vx, [I] ;ERROR?
; Stores V0 to VX in memory starting at address I.
opSTv0vx:
	push R0
	push R1
	push R2
	push R3
	ldi  R0, I ; store inital I in R0
	ldm  R0, R0 ; read [I] into R0
	ori  R0, 0x8000
	add  R0, RA
	ldi  R1, VREGS
	mov  R2, RA
	shl  R2, 1
	add  R1, R2 ; last Vreg to copy
CopyRegLoop:
	ldm  R3, R1 ; read from source
	andi R3, 0xFF ; remove 8 MSB
	;stm  R3, R0 ; write to target
	call WriteByteValue
	subi R0, 1
	subi R1, 2 ; move 2 because registers are 16-bit
	cmpi R1, VREGS
	jge  CopyRegLoop ;ERROR JG does not work??
	pop  R3
	pop  R2
	pop  R1
	pop  R0
	jmp FinishOpcode
	
; Fx65 - LD Vx, [I]
; Fills V0 to VX with values from memory starting at address I.
; R0 = Memory read pointer
; R1 = temp read register
; R2 = VREGS write pointer
; R3 = Initial memory read address, used for loop control
opLDvxI:
	push R0
	push R1
	push R2
	push R3
	ldm  R0, I
	ori  R0, 0x8000
	mov  R3, R0 ; store mem address read pointer 
	add  R0, RA ; points to last mem address 
	ldi  R2, VREGS
	mov  R1, RA
	shl  R1, 1
	add  R2, R1 ; Last VREGS address to write to
ReadRegLoop:
	ldm  R1, R0
	andi R1, 0xFF ; only use the 8 LSB
	stm  R1, R2
	subi R2, 2
	subi R0, 1
	cmp  R0, R3 ; compare with org. value from I
	jnn  ReadRegLoop
	pop  R3
	pop  R2
	pop  R1
	pop  R0
	jmp  FinishOpcode

; Fx75 - ST R=>Vx
; Writes V0 to VX register values to the same HP48 Registers.
; R0 = temp reg
; R1 = temp reg
opSTRv0vx:
	push R0
	push R1
	mov R1, RA
STRLoop:	
	call ReadVREGx
	call WriteHP48REGx
	subi R1, 1
	cmpi R1, 0
	jnz  STRLoop
	pop  R1
	pop  R0
	jmp FinishOpcode

; Fx85 - LD R=>Vx
; Loads V0 to VX register values with values from HP48 Registers.
; R0 = temp reg
; R1 = temp reg
opLDRv0vx:
	push R0
	push R1
	mov R1, RA
LDRLoop:	
	call ReadHP48REGx
	call WriteVREGx
	subi R1, 1
	cmpi R1, 0
	jnz  LDRLoop
	pop  R1
	pop  R0
	jmp FinishOpcode
	
InterpretOpcode: ; Extract/splits opcode at PC into registers R7-RD
	ldm	 R7, RE ; bytes need to be swapped
	shr  R7, 8
	ldm  R8, RE
	shl  R8, 8 
	or   R7, R8
	mov  R8, R7
	shr  R8, 12 	; R8 = opcode sel
	mov  R9, R7
	andi R9, 0xFFF	; R9 = nnn
	mov  RA, R7
	shr  RA, 8	
	andi RA, 0xF 	; RA = x
	mov	 RB, R7
	shr  RB, 4
	andi RB, 0xF 	; RB = y
	mov  RC, R7
	andi RC, 0xFF 	; RC = kk
	mov  RD, RC
	andi RD, 0x0F   ; RD = n
	ret

; Write the 8 LSB of R1 to R0
; R0 = address to write to
; R3 = 8-bit value to write	
WriteByteValue:
	push R2
	push R4
	mov  R2, R3
	andi R2, 0xFF
	ldm  R4, R0     ; read back original value
	andi R4, 0xFF00 ; remove original 8 LSB
	or   R4, R2     ; insert new 8 LSB
    stm  R4, R0     ; write back new value	
	pop  R4
	pop  R2
	ret

; destroys/uses: R0
ReadVREGx: ; R1=x Returns value in R0 (R0=Vx)
	push R1
	push R2
	ldi  R2, VREGS
	shl  R1, 1
	add  R1, R2
	ldm  R0, R1
	andi R0, 0xFF
	pop  R2
	pop  R1
	ret
	
WriteVREGx: ; R1=x R0=value => (Vx=R0)
	push R1
	push R2
	ldi  R2, VREGS
	shl  R1, 1
	add  R1, R2
	andi R0, 0xFF
	stm  R0, R1
	pop  R2
	pop  R1
	ret

; destroys/uses: R0
ReadHP48REGx: ; R1=x Returns value in R0 (R0=HP48x)
	push R1
	push R2
	ldi  R2, HP48REGS
	shl  R1, 1
	add  R1, R2
	ldm  R0, R1
	andi R0, 0xFF
	pop  R2
	pop  R1
	ret
	
WriteHP48REGx: ; R1=x R0=value => (HP48x=R0)
	push R1
	push R2
	ldi  R2, HP48REGS
	shl  R1, 1
	add  R1, R2
	andi R0, 0xFF
	stm  R0, R1
	pop  R2
	pop  R1
	ret
	
Reset:
; clear all VREGS
	ldi  R0, VREGS
	addi R0, 32
ClearRegsLoop:
	subi R0, 2
	ldi  R1, 0
	stm  R1, R0
	cmpi R0, VREGS
	jnz	 ClearRegsLoop
; set PC=0x8200
	ldi  RE, C8Memory
; set I=0x000
	ldi  R0, 0
	stm  R0, I
; reset stack and stack pointer
	ldi  R0, STACK
	addi R0, 32
	ldi  R1, 0
ClearStackLoop:
	subi R0, 2
	stm  R1, R0
	cmpi R0, STACK
	jnz  ClearStackLoop ; clear all stack
	ldi  R0, STACK 
	subi R0, 2
	mov  RF, R0 ; reset SP to STACK-2
	ret
	
CopyFont:
	ldi  R0, Chip8Font 	; source address 
    ldi  R1, C8Font 	; target address, 0x0 in Chip8
	addi R0, 80
	addi R1, 80
C8FontLoop:
	subi R1, 2
	subi R0, 2
	ldm  R2, R0
	stm  R2, R1
	cmpi R1, C8Font
	jnz  C8FontLoop
	
CopySCFont:
	ldi  R0, SuperChipFont 	; source address 
    ldi  R1, SCFont 		; target address, 0x100 in Chip8
	addi R0, SCFontSize
	addi R1, SCFontSize
SCFontLoop:
	subi R1, 2
	subi R0, 2
	ldm  R2, R0
	stm  R2, R1
	cmpi R1, SCFont
	jnz  SCFontLoop
	ret
	
LoadChip8Program:	
	ldi R0, Chip8Program
	ldi R1, C8Memory
	addi R0, Chip8PrgSize
	addi R1, Chip8PrgSize
LoadProgramLoop:
	subi R0, 1
	subi R1, 1
	ldm	 R2, R0 
	stm	 R2, R1
	cmpi R1, C8Memory
	jnz  LoadProgramLoop
	ret

UpdateTimers:
	push R0  
	ldm  R0, DelayTimer
	cmpi R0, 0
	jz   SkipDecDelayTimer
	subi R0, 1
	stm  R0, DelayTimer
SkipDecDelayTimer:
	pop  R0
	ret
	
	;ldm  R0, SoundTimer
	;cmpi R0, 0
	;jz   SkipDecSoundTimer
	;subi R0, 1
	;stm  R0, SoundTimer
;SkipDecSoundTimer:
;	ret

SetPixel:
	ldm  R0, GraphicsMode
	cmpi R0, 0x00
	jnz  HighModeEnabled
	;call SetChip8Pixel
	call SetPixelInBuffer
	ret
HighModeEnabled:
	call SetSuperChipPixel
	ret

SetPixelInBuffer: ; R0=Chip8X R1=Chip8Y R2=0/1 (OFF/ON)
	; first calculate bitmask to use for XOR pixel insertion
	;call SetChip8Pixel
	push R2
	push R3
	push R4
	push R5
	push R6
	push R7
	push R0
	push R1
	mov  R3, R0
	andi R3, 0x7
	ldi  R4, 0x80
	shr  R4, R3 ;R4 = bitmask
	; calculate address to use for pixel insertion
	mov  R5, R0
	shr  R5, 0x3
	mov  R6, R1
	shl  R6, 0x3
	add  R6, R5 ; R6 = address
	; set pixel with XOR
	ldi  R0, GraphicsBuffer
	add  R0, R6
	call ReadByte
	mov  R7, R1 ; save read value for pixel drawing 
	cmpi R2, 0
	jnz  SetPixelTo1  
SetPixelTo0:
	xori R4, 0xFF
	and  R1, R4
	jmp	 WritePixelBack
SetPixelTo1:	
	xor  R1, R4
WritePixelBack:
	call WriteByte
	; compare R2 with bitmasked value of R7 => Set pixel
	pop R1
	pop R0
	tst  R7, R4
	jz   SetChipPixelAsR2  
	cmpi R2, 1
	jnz  SetChipPixelAsR2
	ldi  R2, 0
SetChipPixelAsR2:
	call SetChip8Pixel
	pop R7
	pop R6
	pop R5
	pop R4
	pop R3
	pop R2
	ret

DrawScreen:
	cls
	ldi  R3, GraphicsBuffer
	mov  R5, R3
	addi R5, 256 ; R5 - byte directly after buffer in mem
	ldi  R0, 0
	ldi  R1, 0
	ldi  R2, 1 ; for debugging
AddressLoop:
	ldi  R6, 0x80
	ldm  R4, R3
ValueLoop:
	ldi  R2, 0
	tst  R4, R6
	jz   BitValueZero
	ldi  R2, 1
BitValueZero:
	; TODO: add code to set R2 with pixel on/off
	call SetChip8Pixel
	addi R0, 1
	shr  R6, 1
	cmpi R6, 0
	jnz  ValueLoop
	cmpi R0, 64 ; x-count
	jnz  NoYinc
	ldi  R0, 0
	addi R1, 1
NoYinc:
	addi R3, 1
	cmp  R3, R5
	jnz  AddressLoop
	ret
	
SetChip8Pixel: ; R0=Chip8X R1=Chip8Y R2=0/1 (OFF/ON)
	push R0
	push R1
	muli R0, 5
	muli R1, 5
	addi R0, 0 	; offset X for getting Chip-8 in the middle
	addi R1, 40 ; offset Y for getting Chip-8 in the middle
	cmpi R2, 1
	jnz  SetChip8PixelOff
	drw R0, R1, Chip8PixelOn
	;jnc NoCollision
	jmp   NoCollision
SetChip8PixelOff:
	; set VF = 1 when collide, otherwise VF=0
	drw R0, R1, Chip8PixelOver ;does not work as expected
NoCollision:
	pop R1
	pop R0
	ret

ClearBuffer:
	push R0
	push R1
	ldi  R0, GraphicsBuffer
	addi R0, 254
	ldi  R1, 0
ClearBufferLoop:
	stm  R1, R0
	subi R0, 2
	cmpi R0, GraphicsBuffer
	jnz  ClearBufferLoop
	pop  R1
	pop  R0
	ret
	
FillScreenWith2:
	spr C8FillSize
	ldi R0, 0
	ldi R1, 0
FillLoop:
	drw  R0, R1, Chip8Fill
	addi R0, 8
	cmpi R0, 320
	jnz  FillLoop
	addi R1, 8
	cmpi R1, 240
	jnz  FillLoop
	ret
;SetChip8PixelOff:
	;drw R0, R1, Chip8PixelOff
	;pop R1
	;pop R0
	;ret
	
SetSuperChipPixel: ; R0=SuperChipX R1=SuperChipY
	push R0
	push R1
	muli R0, 2
	muli R1, 2
	addi R0, 0 	; offset X for getting Chip-8 in the middle
	addi R1, 40 ; offset Y for getting Chip-8 in the middle
	cmpi R2, 1
	jnz  SetSuperChipPixelOff
	drw R0, R1, SuperChipPixelOn
	jnc NoSCCollision
SetSuperChipPixelOff:
	drw R0, R1, SuperChipPixelOver
NoSCCollision:
	pop R1
	pop R0
	ret
	
EndOfProgram:
db  0x00

Chip8PixelOn: ; QuadPixel
db 	0xCF,0xFC
db 	0xFF,0xFF
db 	0xFF,0xFF
db 	0xCF,0xFC

Chip8PixelOver: ; QuadPixel used to clear pixel
db 	0x22,0x22
db 	0x22,0x22
db 	0x22,0x22
db 	0x22,0x22

Chip8Fill: ; QuadPixel used to clear pixel
db 	0x22,0x22,0x22,0x22
db 	0x22,0x22,0x22,0x22
db 	0x22,0x22,0x22,0x22
db 	0x22,0x22,0x22,0x22
db 	0x22,0x22,0x22,0x22
db 	0x22,0x22,0x22,0x22
db 	0x22,0x22,0x22,0x22
db 	0x22,0x22,0x22,0x22

;Chip8PixelOff: ; QuadPixel
;db 	0x00,0x00
;db 	0x00,0x00
;db 	0x00,0x00
;db 	0x00,0x00

SuperChipPixelOn: ;DoublePixel
db	0xFF
db	0xFF

;SuperChipPixelOff: ;DoublePixel
;db	0xFF
;db	0xFF

SuperChipPixelOver: ;DoublePixel
db	0x22
db	0x22

include ..\Libraries\ByteAccessLib.asm

Palette:
db 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 
db 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
db 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 
db 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,

Chip8Font:
db 	0xF0, 0x90, 0x90, 0x90, 0xF0, ; 0
db  0x20, 0x60, 0x20, 0x20, 0x70, ; 1
db  0xF0, 0x10, 0xF0, 0x80, 0xF0, ; 2
db  0xF0, 0x10, 0xF0, 0x10, 0xF0, ; 3
db  0x90, 0x90, 0xF0, 0x10, 0x10, ; 4
db  0xF0, 0x80, 0xF0, 0x10, 0xF0, ; 5
db  0xF0, 0x80, 0xF0, 0x90, 0xF0, ; 6
db  0xF0, 0x10, 0x20, 0x40, 0x40, ; 7
db  0xF0, 0x90, 0xF0, 0x90, 0xF0, ; 8
db  0xF0, 0x90, 0xF0, 0x10, 0xF0, ; 9
db  0xF0, 0x90, 0xF0, 0x90, 0x90, ; A
db  0xE0, 0x90, 0xE0, 0x90, 0xE0, ; B
db  0xF0, 0x80, 0x80, 0x80, 0xF0, ; C
db  0xE0, 0x90, 0x90, 0x90, 0xE0, ; D
db  0xF0, 0x80, 0xF0, 0x80, 0xF0, ; E
db  0xF0, 0x80, 0xF0, 0x80, 0x80  ; F

;SuperChipFont:
importbin superchip.font 0 144 SuperChipFont
SCFontSize equ 144

; ** CHIP 8 internals **
PC: ; Program counter
db 0x00, 0x02 ; Chip8 programs start at 0x0200

VREGS: ; 16 8-bit registers V0-VF (VF is a reserved flag register)
; uses 8 LSB for each registers for better performance/simpler code. 
db 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
db 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
db 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
db 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00

HP48REGS: ; part of SuperChip spec.  
; 16 8-bit registers V0-VF (VF is a reserved flag register)
; uses 8 LSB for each registers for better performance/simpler code. 
db 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
db 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
db 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
db 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00

GraphicsMode: ; 0x00=>Chip8Mode 0x01=>SuperChipMode
db 0x00

I: ; I register, used for addresses
db 0x00,0x00

; these timers uses only 8-bit timer values, but is placed as 16-bit to better suit Chip16 opcodes 
DelayTimer:
db 0x00, 0x00

SoundTimer:
db 0x00, 0x00

STACK: ; used for nested calls within Chip8, 16 levels of 16-bit each
db 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
db 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
db 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
db 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00

GraphicsBuffer:
db 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
db 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
db 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
db 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
db 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
db 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
db 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
db 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
db 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
db 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
db 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
db 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
db 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
db 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
db 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
db 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
db 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
db 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
db 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
db 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
db 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
db 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
db 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
db 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
db 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
db 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
db 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
db 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
db 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
db 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
db 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
db 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 

;Chip8Program: ; write a 0-F starting at 0,0
;db 0x60,0x00,0x61,0x00,0x62,0x00,0xF2,0x29,
;db 0xD0,0x15,0x70,0x05,0x72,0x01,0x30,0x28
;db 0x12,0x06,0x71,0x06,0x60,0x00,0x32,0x10
;db 0x12,0x06,0x12,0x1A

;just display a number using BCD opcode
;db 0x60,0xFF,0xA3,0x10,0xF0,0x33
;db 0xF2,0x65, ; V0=Hundred V1=Tenth V2=Single
;db 0xF0,0x29
;db 0x63,0x00,0x64,0x00,0xD3,0x45
;db 0xF1,0x29
;db 0x63,0x05,0x64,0x00,0xD3,0x45
;db 0xF2,0x29
;db 0x63,0x0A,0x64,0x00,0xD3,0x45
;db 0x12,0x20

; test fx55
;db 0x60,0xAA,0x61,0xBB,0x62,0xCC
;db 0xA2,0x80
;db 0xF2,0x55,0x12,0x08

;importbin SCTEST 0 672 Chip8Program
importbin C:\Share\Chip8\Games\BLINKY 0 2356 Chip8Program
;importbin C:\Share\Chip8\Games\TETRIS 0 494 Chip8Program
;importbin C:\Share\Chip8\Games\15PUZZLE 0 264 Chip8Program
;importbin C:\Share\Chip8\Games\HIDDEN 0 850 Chip8Program
importbin C:\Share\Chip8\Tests\KeypadTest\KeypadTest.c8 0 114 Chip8Program
;importbin c8pixeltest.c8 0 32 Chip8Program
;importbin C:\Share\Chip8\FNC\RushHour.c8 0 3582 Chip8Program
Chip8PrgSize equ 114
