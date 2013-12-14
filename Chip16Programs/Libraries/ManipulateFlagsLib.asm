; Lib for manipulating single flags in the flag register
; (C?2:0) + (Z?4:0) + (O?64:0) + (N?128:0))
; No registers were harmed during these functions.
; only R0 used and even that is stack restored. 
C_bit equ 0x2
Z_bit equ 0x4
O_bit equ 0x40
N_bit equ 0x80

SetC:
	push R0
	pushf
	pop  R0
	ori  R0, C_bit
	push R0
	popf
	pop  R0
	ret
UnsetC:
	push R0
	pushf
	pop  R0
	andi R0, 0xFFFD ; inverse 0x2
	push R0
	popf
	pop  R0
	ret
ToggleC:
	push R0
	pushf
	pop  R0
	xori R0, C_bit
	push R0
	popf
	pop  R0
	ret
SetZ:
	push R0
	pushf
	pop  R0
	ori  R0, Z_bit
	push R0
	popf
	pop  R0
	ret
UnsetZ:
	push R0
	pushf
	pop  R0
	andi R0, 0xFFFB ; inverse 0x4
	push R0
	popf
	pop  R0
	ret
ToggleZ:
	push R0
	pushf
	pop  R0
	xori R0, Z_bit
	push R0
	popf
	pop  R0
	ret
SetO:
	push R0
	pushf
	pop  R0
	ori  R0, O_bit
	push R0
	popf
	pop  R0
	ret
UnsetO:
	push R0
	pushf
	pop  R0
	andi R0, 0xFFBF ; inverse 0x40
	push R0
	popf
	pop  R0
	ret
ToggleO:
	push R0
	pushf
	pop  R0
	xori R0, O_bit
	push R0
	popf
	pop  R0
	ret
SetN:
	push R0
	pushf
	pop  R0
	ori  R0, N_bit
	push R0
	popf
	pop  R0
	ret
UnsetN:
	push R0
	push R1
	pushf
	pop  R0
	ldi  R1, N_bit
	xori R1, 0xFFFF
	and R0, R1 
	push R0
	popf
	pop  R1
	pop  R0
	ret
ToggleN:
	push R0
	pushf
	pop  R0
	xori R0, N_bit
	push R0
	popf
	pop  R0
	ret
	
SetAllFlags:
	call SetC
	call SetZ
	call SetO
	call SetN
	ret
UnsetAllFlags:
	call UnsetC
	call UnsetZ
	call UnsetO
	call UnsetN
	ret
	