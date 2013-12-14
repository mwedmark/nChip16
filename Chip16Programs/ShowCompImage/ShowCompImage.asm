PixelSpriteSize		equ		$0101        ; 2 x 1 pixel
PaletteSize			equ     48

FontSize   			equ		$0804        ; 8 x 8 pixel

Main:	
		ldm  RB, NumberOfPics
		andi RB, 0xFF 
		ldi  R9, Palette
		subi R9, PaletteSize
		spr  PixelSpriteSize
		ldi  R4, Picture
		ldi  R2, PixelBuffer ;Use high address for PixelBuffer
		ldi  R3, 0x00
		call InitString
NewPicture:
		cls
		addi R9, PaletteSize
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
		call DrawStringAndInc
; wait 5 seconds
		ldi  R0, 1
		call WaitSeconds
; make palette tone from Colors=>Black
		mov  R0,R9
		ldi  R1, ZeroPalette
		call TonePalette
		subi RB, 1
		jz   Main
		jmp  NewPicture
DataCollMode:
		addi R4, 1
		ldm  R5, R4 ; store nibble count in R5
		andi R5, 0xFF
		addi R4, 1
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
InitString:
	ldi  R0, PictureStrings
	stm  R0, CurrentStringAddr
	ret
	
DrawStringAndInc:
	spr  FontSize
	push R0
	push R1
	push R2
	push R3
	push R4
	ldi  R2, 72
	ldi  R3, 230
	ldi  R4, CurrentStringAddr
	ldm  R0, R4
StringLoop:
	call ReadByte
	cmpi R1, 0x20
	jnz  NoSpace
	addi R1, 0x20
NoSpace:
	subi R1, 0x30
	muli R1, 32
	addi R1, MarioFont
	addi R1, 3
	drw  R2, R3, R1
	addi R2, 8
	addi R0, 1
	cmpi R2, 248
	jnz  StringLoop
	stm  R0, R4
	pop  R4
	pop  R3
	pop  R2
	pop  R1
	pop  R0
	spr  PixelSpriteSize
	ret
	
include ..\Libraries\ByteAccessLib.asm		
include ..\Libraries\PaletteToneLib.asm
include ..\Libraries\TimeLib.asm

EndOfProgram:
db 0x00

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

PixelBuffer:
db 0x00, 0x10, 0x20, 0x30, 0x40, 0x50, 0x60, 0x70, 0x80, 0x90, 0xA0, 0xB0, 0xC0, 0xD0, 0xE0, 0xF0

CurrentStringAddr:
db 0x00, 0x00

; Use Full MarioFont - ASCII correct for alpha&digit => (CHAR-0x30)
importbin Graphics\MarioFontLong.bmp.bin 0 1539 MarioFont
NumberOfPics:
db 48

importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Albania.bmp.cci 0 2107 Picture
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Andorra.bmp.cci 0 3100 PIC1
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Armenia.bmp.cci 0 33 PIC2
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Austria.bmp.cci 0 39 PIC3
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Azerbaijan.bmp.cci 0 471 PIC4
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Belarus.bmp.cci 0 1558 PIC5
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Belgium.bmp.cci 0 1895 PIC6
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Bosnia_And_Herzegovina.bmp.cci 0 1946 PIC7
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Bulgaria.bmp.cci 0 29 PIC8
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Croatia.bmp.cci 0 1215 PIC9
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Cyprus.bmp.cci 0 1273 PIC10
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Czech_Republic.bmp.cci 0 897 PIC11
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Denmark.bmp.cci 0 533 PIC12
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Estonia.bmp.cci 0 443 PIC13
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Finland.bmp.cci 0 757 PIC14
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\France.bmp.cci 0 1069 PIC15
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Georgia.bmp.cci 0 667 PIC16
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Germany.bmp.cci 0 604 PIC17
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Greece.bmp.cci 0 710 PIC18
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Hungary.bmp.cci 0 33 PIC19
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Iceland.bmp.cci 0 1839 PIC20
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Ireland.bmp.cci 0 805 PIC21
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Italy.bmp.cci 0 1069 PIC22
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Kazakhstan.bmp.cci 0 2149 PIC23
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Latvia.bmp.cci 0 25 PIC24
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Liechtenstein.bmp.cci 0 595 PIC25
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Lithuania.bmp.cci 0 29 PIC26
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Luxembourg.bmp.cci 0 29 PIC27
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Macedonia.bmp.cci 0 2759 PIC28
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Moldova.bmp.cci 0 1945 PIC29
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Monaco.bmp.cci 0 37 PIC30
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Montenegro.bmp.cci 0 1946 PIC31
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Norway.bmp.cci 0 2083 PIC32
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Poland.bmp.cci 0 101 PIC33
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Portugal.bmp.cci 0 2114 PIC34
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Russia.bmp.cci 0 39 PIC35
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\San_Marino.bmp.cci 0 2487 PIC36
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Serbia.bmp.cci 0 2288 PIC37
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Slovakia.bmp.cci 0 1291 PIC38
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Slovenia.bmp.cci 0 497 PIC39
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Spain.bmp.cci 0 1187 PIC40
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Sweden.bmp.cci 0 615 PIC41
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Switzerland.bmp.cci 0 1151 PIC42
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\The_Netherlands.bmp.cci 0 39 PIC43
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Turkey.bmp.cci 0 1148 PIC44
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Ukraine.bmp.cci 0 35 PIC45
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\United_Kingdom.bmp.cci 0 2973 PIC46
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Vatican_City.bmp.cci 0 2769 PIC47

importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Albania.bmp.PAL 0 48 Palette
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Andorra.bmp.PAL 0 48 PAL1
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Armenia.bmp.PAL 0 48 PAL2
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Austria.bmp.PAL 0 48 PAL3
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Azerbaijan.bmp.PAL 0 48 PAL4
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Belarus.bmp.PAL 0 48 PAL5
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Belgium.bmp.PAL 0 48 PAL6
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Bosnia_And_Herzegovina.bmp.PAL 0 48 PAL7
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Bulgaria.bmp.PAL 0 48 PAL8
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Croatia.bmp.PAL 0 48 PAL9
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Cyprus.bmp.PAL 0 48 PAL10
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Czech_Republic.bmp.PAL 0 48 PAL11
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Denmark.bmp.PAL 0 48 PAL12
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Estonia.bmp.PAL 0 48 PAL13
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Finland.bmp.PAL 0 48 PAL14
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\France.bmp.PAL 0 48 PAL15
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Georgia.bmp.PAL 0 48 PAL16
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Germany.bmp.PAL 0 48 PAL17
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Greece.bmp.PAL 0 48 PAL18
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Hungary.bmp.PAL 0 48 PAL19
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Iceland.bmp.PAL 0 48 PAL20
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Ireland.bmp.PAL 0 48 PAL21
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Italy.bmp.PAL 0 48 PAL22
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Kazakhstan.bmp.PAL 0 48 PAL23
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Latvia.bmp.PAL 0 48 PAL24
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Liechtenstein.bmp.PAL 0 48 PAL25
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Lithuania.bmp.PAL 0 48 PAL26
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Luxembourg.bmp.PAL 0 48 PAL27
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Macedonia.bmp.PAL 0 48 PAL28
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Moldova.bmp.PAL 0 48 PAL29
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Monaco.bmp.PAL 0 48 PAL30
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Montenegro.bmp.PAL 0 48 PAL31
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Norway.bmp.PAL 0 48 PAL32
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Poland.bmp.PAL 0 48 PAL33
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Portugal.bmp.PAL 0 48 PAL34
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Russia.bmp.PAL 0 48 PAL35
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\San_Marino.bmp.PAL 0 48 PAL36
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Serbia.bmp.PAL 0 48 PAL37
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Slovakia.bmp.PAL 0 48 PAL38
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Slovenia.bmp.PAL 0 48 PAL39
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Spain.bmp.PAL 0 48 PAL40
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Sweden.bmp.PAL 0 48 PAL41
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Switzerland.bmp.PAL 0 48 PAL42
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\The_Netherlands.bmp.PAL 0 48 PAL43
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Turkey.bmp.PAL 0 48 PAL44
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Ukraine.bmp.PAL 0 48 PAL45
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\United_Kingdom.bmp.PAL 0 48 PAL46
importbin C:\Projects\nChip16\Chip16Programs\ShowCompImage\Graphics\Pictures\Vatican_City.bmp.PAL 0 48 PAL47

PictureStrings:
db "        ALBANIA       "
db "        ANDORRA       "
db "        ARMENIA       "
db "        AUSTRIA       "
db "      AZERBAIJAN      "
db "        BELARUS       "
db "        BELGIUM       "
db "BOSNIA AND HERZEGOVINA"
db "       BULGARIA       "
db "        CROATIA       "
db "        CYPRUS        "
db "    CZECH REPUBLIC    "
db "        DENMARK       "
db "        ESTONIA       "
db "        FINLAND       "
db "        FRANCE        "
db "        GEORGIA       "
db "        GERMANY       "
db "        GREECE        "
db "        HUNGARY       "
db "        ICELAND       "
db "        IRELAND       "
db "         ITALY        "
db "      KAZAKHSTAN      "
db "        LATVIA        "
db "     LIECHTENSTEIN    "
db "       LITHUANIA      "
db "      LUXEMBOURG      "
db "       MACEDONIA      "
db "        MOLDOVA       "
db "        MONACO        "
db "      MONTENEGRO      "
db "        NORWAY        "
db "        POLAND        "
db "       PORTUGAL       "
db "        RUSSIA        "
db "      SAN MARINO      "
db "        SERBIA        "
db "       SLOVAKIA       "
db "       SLOVENIA       "
db "         SPAIN        "
db "        SWEDEN        "
db "      SWITZERLAND     "
db "    THE NETHERLANDS   "
db "        TURKEY        "
db "        UKRAINE       "
db "    UNITED KINGDOM    "
db "     VATICAN CITY     "
; Sum of sizes for all files: 55727 bytes
; Max size for Chip16 target platform: 65536 bytes (64K)