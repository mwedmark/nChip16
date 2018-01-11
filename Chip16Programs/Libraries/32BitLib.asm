; Chip16 32-bit Arithmetic Library: 
; ADD/ADDI(32bit+32bit=32bit)
; SUB/SUBI(32bit+32bit=32bit)
; MUL/MULI(16bitx16bit=32bit)
; DIV/DIVI(32bit/32bit=32bit)
; SHR(32-bit>>5-bit=32-bit)
; SHL(32-bit<<5-bit=32-bit)
; CMP/TEST?
; AND?
; OR?
; XOR?
; MULS? 16- & 32-bit
; DIVS? 16- & 32-bit
; NOT?
; SWAPW?
; NEG?
;
; Magnus Wedmark (2015)

; ADD/ADDI
; affects R4/R5 as they are used for result
32bit_addi: ; 32bit+32bit=>32bit (R1,R0) + (R3,R2) = (R5,R4)
	push R2
	push R3
	add  R2, R0
	jnc  no_add_carry
	addi R3, 1
no_add_carry:
	mov  R4, R2
	add  R3, R1
	mov  R5, R3
	pop  R3
	pop  R2
	ret  
	
;SUB/SUBI
; affects R4/R5 as they are used for result
32bit_subi: ; 32bit-32bit=>32bit (R1,R0) + (R3,R2) = (R5,R4)
	push R0
	push R1
	sub  R0, R2
	jnc  no_sub_carry
	subi R1, 1
no_sub_carry:
	mov  R4, R0
	sub  R1, R3
	mov  R5, R1
	pop  R1
	pop  R0
	ret
	
;MUL/MULI
16_16_muli: ; 16bit*16bit=>32bit (--,R0) + (--,R2) = (R5,R4)
	; split 16bit values into 2* 8-bit regs and transpose the 8-MSB's down 8-bits
	mov  R1,R0
	andi R0,$FF
	shr  R1,8  
	mov  R3,R2
	andi R2,$FF
	shr  R3,8
	; then start mul 8+8=16-bit terms (4) into target-registers => R4-R5 using R8-RB inbetween
	; R0, R2 => R8 (lowest 16-bit, byte 1&2)
	mov  R8,R2
	mul  R8,R0
	; R1, R2 => R9 (middle 16-bit, byte 2&3)
	mov  R9,R1
	mul  R9,R2
	; R0, R3 => RA (middle 16-bit, byte 2&3)
	mov  RA,R0
	mul  RA,R3
	; R1, R3 => RB (high 16-bit, byte 3&4)
	mov  RB,R1
	mul  RB,R3
add_all_terms:
	; add all terms using 8-bit parts that fits together
	mov  R4,R8 ; added term1
	mov  R6,R9
	andi R6,$FF
	shl  R6,8
	add  R4,R6 
	jnc  no_carry_step1
	addi R5,1    
no_carry_step1:
	mov  R6,R9
	shr  R6, 8
	mov  R5,R6
	; added term2
	mov  R6,RA
	andi R6,$FF
	shl  R6,8
	add  R4,R6 
	jnc  no_carry_step2
	addi R5,1    
no_carry_step2:
	mov  R6,RA
	shr  R6,8
	add  R5,R6
	jnc  no_carry_step3
	addi R5, $100
no_carry_step3:
	; added term3
	add  R5,RB
	; added term4
	ret

;DIV/DIVI
32bit_divi: ;32bit*32bit=>32bit (R1,R0) / (R3,R2) = (R5,R4) - using R6 as temp
	ldi  R6,0
sub_loop:
	call 32bit_subi
	addi R6,1
	mov  R0,R4
	mov  R1,R5
	mov  R7,R5
	andi R7, $8000
	jz  sub_loop
	subi R6, 1
	ldi  R5, 0
	mov  R4, R6
	ret

32bit_div_adv:
	ret

;SHR
32bit_shr: ; 32bit>>5bit=32bit (R1,R0>>R2) (more than 5 bits => 0)
	andi  R2,$1F ; remove all bits >31
next_32bit_shr:
	tsti  R1, $0001 ; save the lowest bit
	jz    no_top_bit_shr
	ldi   R3, $8000
	jmp   after_shr
no_top_bit_shr:
	ldi   R3, 0
after_shr:
	shr   R1,1
	shr   R0,1
	or    R0,R3
	subi  R2,1
	jnz   next_32bit_shr
	ret

;SHL
32bit_shl: ; 32bit<<5bit=32bit (R1,R0<<R2) (more than 5 bits => 0)
	andi  R2,$1F ; remove all bits >31
next_32bit_shl:
	tsti  R0, $8000 ; save the lowest bit
	jz    no_top_bit_shl
	ldi   R3, $0001
	jmp   after_shl
no_top_bit_shl:
	ldi   R3, 0
after_shl:
	shl   R0,1
	shl   R1,1
	or    R1,R3
	subi  R2,1
	jnz   next_32bit_shl
	ret
	