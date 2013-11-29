	
ReadByte: ;R1 = [R0]&0xFF
		ldm  R1,R0
		andi R1, 0xFF
		ret
WriteByte: ;R0 = (R0&0xFF00) + (R1&0xFF)
		push R2
		ldm  R2, R0
		andi R2, 0xFF00
		andi R1, 0xFF
		add  R2, R1
		stm  R2, R0
		pop  R2
		ret
