PixelSpriteSize		equ		$0101        ; 2 x 1 pixel
PixelBuffer			equ		$8000
PaletteSize			equ     48

main:	
		ldm  RB, NumberOfPics
		andi RB, 0xFF 
		ldi  R9, Palette
		subi R9, PaletteSize
		spr  PixelSpriteSize
		ldi  R4, Picture
		ldi  R2, PixelBuffer ;Use high address for PixelBuffer
		ldi  R3, 0x00
LoopInitPixelBuffer:
		stm  R3, R2
		addi R3, 0x10
		addi R2, 1
		cmpi R3, 0x100
		jnz  LoopInitPixelBuffer
NewPicture:
		cls
		addi R9, PaletteSize
		;pal	 R9
		pal  ZeroPalette
		call ReadHeader ; after this call R4 points to data AFTER header
		add  RF, R4
		ldi  R0, 1
		ldm	 R0, XStartMiddle ; Xcoord start
		ldm  R1, YStartMiddle  ; Ycoord start
MainPixelLoop:
		ldm  R5, R4
		mov  R8, R5
		andi R8, 0xFF
		cmpi R8, 0x00
		jz   DataCollMode ;first byte in group=00 => DataColl
		mov  R6, R5
		andi R6, 0xF0
		shr  R6, 4 ; nibble count in R6
		andi R5, 0x0F ; nibble value in R5
		ldi  R2, PixelBuffer ; reset to initial pixel
		add  R2, R5 ; Choose color of pixel from buffer
		ldm  R7, R4
		andi R7, 0x80
		cnz  BigDataMode
NibblePixelLoop:		
		drw  R0,R1,R2
		call IncreasePixelPointer
		subi R6, 1
		cmpi R6, 0
		jnz  NibblePixelLoop
		addi R4, 1
		cmp  RF, R4
		jnz  MainPixelLoop
		cls
loop:   jmp  loop

IncreasePixelPointer:
		ldm  RD, XEndMiddle
		addi R0, 1
		cmp  R0, RD
		jnz  IPP_Return
		;ldi  R0, 0
		mov  R0, RC
		addi R1, 1
		ldm  RE, YEndMiddle
		cmp  R1, RE
		jz   EndOfPicture
IPP_Return:
		ret
EndOfPicture:
		pop  RA ; dummy pop to remove return address from stack
		addi R4, 1
; make palette tone from Black=>Colors
		;ldi  R0, ZeroPalette
		;mov  R1, R9
		;call TonePalette
		pal R9
; wait 5 seconds
		ldi  R0, 5
		call WaitSeconds
; make palette tone from Colors=>Black
		mov  R0,R9
		ldi  R1, ZeroPalette
		call TonePalette
		subi RB, 1
		jz   main
		jmp  NewPicture
DataCollMode:
		addi R4, 1
		ldm  R5, R4 ; store nibble count in R5
		andi R5, 0xFF
		addi R4, 1
		;addi R5, 1 ; TEMP fix to get correct nibble count below
		;shr  R5, 1 ; to get nibble count in bytes
DataCollLoop:
		push R1
		push R0
		mov  R0, R4
		call ReadByte
		pop  R0
		mov  R2, R1
		mov  R3, R1
		pop  R1
		andi R2, 0xF0
		shr  R2, 4 ; first high nibble as first byte
		addi R2, PixelBuffer
		drw  R0,R1,R2
		call IncreasePixelPointer
		subi R5, 1
		cmpi R5, 0
		jz   EndOfDataColl
		andi R3, 0x0F
		addi R3, PixelBuffer
		drw  R0,R1,R3
		call IncreasePixelPointer
		addi R4, 1
		subi R5, 1
		cmpi R5, 0
		jnz  DataCollLoop
		jmp  MainPixelLoop
EndOfDataColl:
		addi r4, 1
		jmp  MainPixelLoop
		
BigDataMode: ;extract full count of nibbles into R6
		push R7
		ldm  R7, R4 ;reread first byte
		andi R7, 0x70
		shl  R7, 4
		addi R4, 1
		ldm  R6,R4
		andi R6,0xFF
		add  R6, R7
		pop  R7
		ret

ReadHeader:
		push R5
		ldm  RF, R4 ; RF = compressed image data size in bytes
		addi R4, 2
		ldm  RD, R4 ; RD = X-size
		ldi  RC, XSize
		stm  RD, RC ; Store Xsize in XSize mem.
		ldi  R5, 320
		sub  R5, RD
		shr  R5, 1
		ldi  RC, XStartMiddle
		stm  R5, RC
		add  R5, RD
		ldi  RC, XEndMiddle
		stm  R5, RC		
		
		addi R4, 2
		ldm  RE, R4 
		andi RE, 0xFF ; RE = Y-size (only 8-bit used)
		ldi  RC, YSize
		stm  RE,RC ; Store Ysize in YSize mem. 
		ldi  R5, 240
		sub  R5, RE
		shr  R5, 1
		stm  R5, YStartMiddle
		add  R5, RE
		stm  R5, YEndMiddle
		addi R4, 1
; Calculate the support values for image centering
; XStart = (320-XSize)/2
		ldi  RC, 320
		sub  RC, RD
		shr  RC, 1
		add  RD, RC
		pop  R5
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

include byteaccess.asm		
include palettetone.asm

EndOfProgram:
db 0x00

NumberOfPics:
db 0x04

XStartMiddle: ; The X start coordinate to get image centered
db 0x00, 0x00
YStartMiddle: ; The Y start coordinate to get image centered
db 0x00, 0x00
XEndMiddle:	  ; The X end coordinate to get image centered
db 0x00, 0x00
YEndMiddle:	  ; The Y end coordinate to get image centered
db 0x00, 0x00
XSize:
db 0x00, 0x00
YSize:
db 0x00, 0x00

;include output.asm
;include C:\Projects\nChip16\CreateImportBinForPics\bin\Debug\output.asm
importbin C:\Projects\nChip16\CreateImportBinForPics\bin\Debug\Flag_of_Germany.bmp.cci 0 71 Picture
importbin C:\Projects\nChip16\CreateImportBinForPics\bin\Debug\Flag_of_UnitedKingdom.bmp.cci 0 2436 PIC1
importbin C:\Projects\nChip16\CreateImportBinForPics\bin\Debug\R36_320_240_16COL_DIT.bmp.cci 0 19658 PIC2
importbin C:\Projects\nChip16\CreateImportBinForPics\bin\Debug\R36_320_240_NODITHER.bmp.cci 0 12861 PIC3

importbin C:\Projects\nChip16\CreateImportBinForPics\bin\Debug\Flag_of_Germany.bmp.PAL 0 9 Palette
importbin C:\Projects\nChip16\CreateImportBinForPics\bin\Debug\Flag_of_UnitedKingdom.bmp.PAL 0 9 PAL1
importbin C:\Projects\nChip16\CreateImportBinForPics\bin\Debug\R36_320_240_16COL_DIT.bmp.PAL 0 48 PAL2
importbin C:\Projects\nChip16\CreateImportBinForPics\bin\Debug\R36_320_240_NODITHER.bmp.PAL 0 48 PAL3
; Sum of sizes for all files: 35140 bytes
; Max size for Chip16 target platform: 65536 bytes (64K)
; Sum of sizes for all files: 63794 bytes
; Max size for Chip16 target platform: 65536 bytes (64K)
;Picture:
;Palette:
;importbin MarioWorld.bmp.cci 0 6277 Picture
;importbin Landscape.bmp.cci 0 22823 Pic1
;importbin Flag_of_Denmark.bmp.cci 0 852 Pic2
;importbin Flag_of_Sweden.bmp.cci 0 663 Pic3

;importbin MarioWorld.bmp.PAL 0 48 Palette
;importbin landscape.bmp.PAL 0 48 Pal2
;importbin Flag_of_Denmark.bmp.pal 0 48 Pal3
;importbin Flag_of_Sweden.bmp.pal 0 48 Pal4