; Test 32bit_add
call test_low_word_sub
test_low_word_add:
	ldi  R0,1
	ldi  R1,0
	ldi  R2,1
	ldi  R3,0
	call 32bit_addi
	cmpi R4,2
	jnz  error
	cmpi R5,0
	jnz  error

test_high_word_add:
	ldi  R0,0
	ldi  R1,1234
	ldi  R2,0
	ldi  R3,2345
	call 32bit_addi
	cmpi R4,0
	jnz  error
	cmpi R5,3579
	jnz  error

test_low+high_word_add:
	ldi  R0,$FFF0
	ldi  R1,0
	ldi  R2,$0010
	ldi  R3,0
	call 32bit_addi
	cmpi R4,0
	jnz  error
	cmpi R5,1
	jnz  error

test_neg_value_add:
	ldi  R0,$FFFF
	ldi  R1,$FFFF
	ldi  R2,5
	ldi  R3,0
	call 32bit_addi
	cmpi R4,4
	jnz  error
	cmpi R5,0
	jnz  error

test_low_word_sub:
	ldi  R0,123
	ldi  R1,0
	ldi  R2,12
	ldi  R3,0
	call 32bit_subi
	cmpi R4,111
	jnz  error
	cmpi R5,0
	jnz  error
	
test_high_word_sub:
	ldi  R0,0
	ldi  R1,345
	ldi  R2,0
	ldi  R3,33
	call 32bit_subi
	cmpi R4,0
	jnz  error
	cmpi R5,312
	jnz  error
	
test_low+high_word_sub:
	ldi  R0,0
	ldi  R1,1 ;65536
	ldi  R2,$78
	ldi  R3,0
	call 32bit_subi
	cmpi R4,$FF88
	jnz  error
	cmpi R5,0
	jnz  error
	
test_neg_value_sub:
	ldi  R0,$FF00 ;-256
	ldi  R1,$FFFF
	ldi  R2,$FFF0 ;-16
	ldi  R3,$FFFF
	call 32bit_subi
	cmpi R4,$FF10 ;-240 => 0xFFFFFF10
	jnz  error 
	cmpi R5,$FFFF
	jnz  error

test_16_16_mul:
	ldi  R0,$6230
	ldi  R2,$432E ;=> $19C434A0
	call 16_16_muli
	cmpi R4,$34A0
	jnz  error 
	cmpi R5,$19C4
	jnz  error

;test_16neg_16_mul: ;??
	; ldi  R0,$F230
	; ldi  R2,$432E ;=> $19C434A0
	; call 16_16_mul
	; cmpi R4,$34A0
	; jnz  error 
	; cmpi R5,$19C4
	; jnz  error
	
; Test 32bit_div 
test_32_32_div:
	ldi   R0,10
	ldi   R1,0
	ldi   R2,2
	ldi   R3,0
	call 32bit_divi
	cmpi R4,5
	jnz  error 
	cmpi R5,$0000
	jnz  error
test_32_32_highvals_div: ;$7584'7362 / $08CB 8E51 = $D
	ldi  R0,$7362
	ldi  R1,$7584
	ldi  R2,$8E51
	ldi  R3,$08CB
	call 32bit_divi
	cmpi R4,$000D
	jnz  error 
	cmpi R5,$0000
	jnz  error
	
test_32bit_shr: ;12345678>>1 => 091A2B3C
	ldi  R0,$5678
	ldi  R1,$1234
	ldi  R2,1
	call 32bit_shr ;091A2B3C
	cmpi R0,$2B3C
	jnz  error 
	cmpi R1,$091A
	jnz  error
	
test_32bit_shr_bigshift:
	ldi  R0,$5678
	ldi  R1,$1234
	ldi  R2,17
	call 32bit_shr
	cmpi R0,$091A
	jnz  error 
	cmpi R1,$0000
	jnz  error
	
test_32bit_shl:
	ldi  R0,$8678
	ldi  R1,$1234
	ldi  R2,1
	call 32bit_shl
	cmpi R0,$0CF0
	jnz  error 
	cmpi R1,$2469
	jnz  error
	
test_32bit_shl_bigshift:
	ldi  R0,$5678
	ldi  R1,$1234
	ldi  R2,17
	call 32bit_shl
	cmpi R0,$0000
	jnz  error 
	cmpi R1,$ACF0
	jnz  error
	
finished:
	jmp finished
error:
	jmp error

include ..\Libraries\32BitLib.asm