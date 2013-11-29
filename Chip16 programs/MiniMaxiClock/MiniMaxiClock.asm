InitMain:
	call InitMiniMaxi ; uses/locks R2-R7, do NOT use!
	call InitTime	  ; uses/locks RA-RF, do NOT use!
Main:
	call DrawTime
	call WaitSecond
	call IncreaseTime
	cls
	jmp  Main

InitTime:
	ldi  RA, 01 ; Hour Tenth
	ldi  RB, 01 ; Hour Single		
	ldi  RC, 04 ; Minute Tenth	
	ldi  RD, 02 ; Minute Single	
	ldi  RE, 05 ; Second Tenth	
	ldi  RF, 00 ; Second Single	
	ret

IncreaseTime:
	addi RF, 1
	cmpi RF, 10
	jnz  EndOfInc
	ldi  RF, 0
	addi RE, 1
	cmpi RE, 6
	jnz	 EndOfInc
	ldi  RE, 0
	addi RD, 1
	cmpi RD, 10
	jnz  EndOfInc
	ldi  RD, 0
	addi RC, 1
	cmpi RC, 6
	jnz EndOfInc
	ldi  RC, 0
	addi RB, 1
	cmpi RB, 4
	jz   CheckFor24
	cmpi RB, 10
	jnz  EndOfInc
	ldi  RB, 0
	addi RA, 1
	jp   EndOfInc
CheckFor24:
	cmpi RA, 2
	jnz  EndOfInc
	ldi  RB, 0
	ldi  RA, 0
EndOfInc:
	ret

DrawTime:
	ldi  R0, 0
	ldi  R1, 70
	mov	 R8, RA
	call DrawNumber
	addi R0, 48
	mov	 R8, RB
	call DrawNumber
	addi R0, 48
	addi R0, 10
	mov	 R8, RC
	call DrawNumber
	addi R0, 48
	mov	 R8, RD
	call DrawNumber
	addi R0, 48
	addi R0, 10
	mov	 R8, RE
	call DrawNumber
	addi R0, 48
	mov	 R8, RF
	call DrawNumber
	flip 0,0
	ldi  R0, 92
	ldi  R1, 85
	drw  R0,R1,R7
	addi R1, 30
	drw  R0,R1,R7
	addi R0, 107
	drw  R0,R1,R7
	subi R1, 30
	drw  R0,R1,R7
	ret 

; simple time wait routines
include TimeLib.asm	

; Complete lib for drawing big chars using reusable
; small images. (only support for digits now..)
include MiniMaxiFontLib.asm
; importbin does not work in files included using 'include'
; thus these files is located here though they should
; have been located in MiniMaxiFontLib.asm
importbin MiniMaxiFont.bmp.bin 0 3075 FontSprites
importbin MiniMaxiFont.bmp.PAL 0 48 Palette

EndOfProgram:
	db 0x00
