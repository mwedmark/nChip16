WaitSecond:
	push R0
	ldi  R0, 60
WaitLoop:
	vblnk
	subi R0, 1
	jnz  WaitLoop
	pop  R0
	ret

	; R0 = seconds to wait
WaitSeconds:
	push R0
	push R1
	push R2
	mov R1, R0
WaitSecondsLoop:
	ldi  R2, 60
WaitSecFrameLoop:
	vblnk
	subi R2, 1
	jnz  WaitSecFrameLoop
	subi R1, 1
	jnz  WaitSecondsLoop
	pop R2
	pop R1
	pop R0
	ret
	