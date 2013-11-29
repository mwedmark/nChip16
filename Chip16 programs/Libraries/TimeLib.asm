WaitSecond:
	push R0
	ldi  R0, 60
WaitLoop:
	vblnk
	subi R0, 1
	jnz  WaitLoop
	pop  R0
	ret
