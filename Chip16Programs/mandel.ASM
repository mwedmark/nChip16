;;    C16PASCAL
;;    Tronix (c) 2011

      jmp    MAIN
      db     #43,#31,#36,#50
MAIN:
      ldi   r0,160		; load const
      stm   r0,_XPIXELS		; store variable INTEGER
      ldi   r0,100		; load const
      stm   r0,_YPIXELS		; store variable INTEGER
      ldi   r0,#ff9c		; load const
      stm   r0,_XSTART		; store variable INTEGER
      ldi   r0,#ffce		; load const
      stm   r0,_YSTART		; store variable INTEGER
      ldi   r0,160		; load const
      stm   r0,_XSIZE		; store variable INTEGER
      ldi   r0,100		; load const
      stm   r0,_YSIZE		; store variable INTEGER
      ldi   r0,16		; load const
      stm   r0,_MAXITER		; store variable INTEGER
      ldm   r0,_XSIZE		; load INTEGER variable
      mov   r5,r0		; push to stack
      ldm   r0,_XPIXELS		; load INTEGER variable
      mov   r1,r0		; save V0->V1, get V0 from stack
      mov   r0,r5
      call  idiv
      stm   r0,_XSTEP		; store variable INTEGER
      ldm   r0,_YSIZE		; load INTEGER variable
      mov   r5,r0		; push to stack
      ldm   r0,_YPIXELS		; load INTEGER variable
      mov   r1,r0		; save V0->V1, get V0 from stack
      mov   r0,r5
      call  idiv
      stm   r0,_YSTEP		; store variable INTEGER
      ldi   r0,20		; load const
      stm   r0,_YY		; store variable
      ldi   r0,0		; load const
      stm   r0,_IY		; store variable INTEGER
      ldm   r0,_YPIXELS		; load INTEGER variable
      stm   r0,Lim_IY		; store variable WORD
L0:
      ldi   r0,0		; load const
      stm   r0,_XX		; store variable
      ldi   r0,0		; load const
      stm   r0,_IX		; store variable INTEGER
      ldm   r0,_XPIXELS		; load INTEGER variable
      stm   r0,Lim_IX		; store variable WORD
L2:
      ldm   r0,_XSTART		; load INTEGER variable
      mov   r5,r0		; push to stack
      ldm   r0,_IX		; load INTEGER variable
      mov   r6,r0		; push to stack
      ldm   r0,_XSTEP		; load INTEGER variable
      mov   r1,r0		; save V0->V1, get V0 from stack
      mov   r0,r6
      call  imul
      mov   r1,r0		; save V0->V1, get V0 from stack
      mov   r0,r5
      add   r0,r1		; make addiction
      stm   r0,_X		; store variable INTEGER
      ldm   r0,_YSTART		; load INTEGER variable
      mov   r5,r0		; push to stack
      ldm   r0,_IY		; load INTEGER variable
      mov   r6,r0		; push to stack
      ldm   r0,_YSTEP		; load INTEGER variable
      mov   r1,r0		; save V0->V1, get V0 from stack
      mov   r0,r6
      call  imul
      mov   r1,r0		; save V0->V1, get V0 from stack
      mov   r0,r5
      add   r0,r1		; make addiction
      stm   r0,_Y		; store variable INTEGER
      ldm   r0,_X		; load INTEGER variable
      stm   r0,_X0		; store variable INTEGER
      ldm   r0,_Y		; load INTEGER variable
      stm   r0,_Y0		; store variable INTEGER
      ldi   r0,0		; load const
      stm   r0,_ITERATION		; store variable INTEGER
L4:
      ldm   r0,_X		; load INTEGER variable
      mov   r5,r0		; push to stack
      ldm   r0,_X		; load INTEGER variable
      mov   r1,r0		; save V0->V1, get V0 from stack
      mov   r0,r5
      call  imul
      mov   r5,r0		; push to stack
      ldi   r0,48		; load const
      mov   r1,r0		; save V0->V1, get V0 from stack
      mov   r0,r5
      call  idiv
      mov   r5,r0		; push to stack
      ldm   r0,_Y		; load INTEGER variable
      mov   r6,r0		; push to stack
      ldm   r0,_Y		; load INTEGER variable
      mov   r1,r0		; save V0->V1, get V0 from stack
      mov   r0,r6
      call  imul
      mov   r6,r0		; push to stack
      ldi   r0,48		; load const
      mov   r1,r0		; save V0->V1, get V0 from stack
      mov   r0,r6
      call  idiv
      mov   r1,r0		; save V0->V1, get V0 from stack
      mov   r0,r5
      sub   r0,r1		; make substraction
      mov   r5,r0		; push to stack
      ldm   r0,_X0		; load INTEGER variable
      mov   r1,r0		; save V0->V1, get V0 from stack
      mov   r0,r5
      add   r0,r1		; make addiction
      stm   r0,_XTEMP		; store variable INTEGER
      ldi   r0,2		; load const
      mov   r5,r0		; push to stack
      ldm   r0,_X		; load INTEGER variable
      mov   r6,r0		; push to stack
      ldm   r0,_Y		; load INTEGER variable
      mov   r1,r0		; save V0->V1, get V0 from stack
      mov   r0,r6
      call  imul
      mov   r6,r0		; push to stack
      ldi   r0,48		; load const
      mov   r1,r0		; save V0->V1, get V0 from stack
      mov   r0,r6
      call  idiv
      mov   r1,r0		; save V0->V1, get V0 from stack
      mov   r0,r5
      call  imul
      mov   r5,r0		; push to stack
      ldm   r0,_Y0		; load INTEGER variable
      mov   r1,r0		; save V0->V1, get V0 from stack
      mov   r0,r5
      add   r0,r1		; make addiction
      stm   r0,_Y		; store variable INTEGER
      ldm   r0,_XTEMP		; load INTEGER variable
      stm   r0,_X		; store variable INTEGER
      ldm   r0,_ITERATION		; load INTEGER variable
      mov   r5,r0		; push to stack
      ldi   r0,1		; load const
      mov   r1,r0		; save V0->V1, get V0 from stack
      mov   r0,r5
      add   r0,r1		; make addiction
      stm   r0,_ITERATION		; store variable INTEGER
      ldm   r0,_X		; load INTEGER variable
      mov   r5,r0		; push to stack
      ldm   r0,_X		; load INTEGER variable
      mov   r1,r0		; save V0->V1, get V0 from stack
      mov   r0,r5
      call  imul
      mov   r5,r0		; push to stack
      ldi   r0,48		; load const
      mov   r1,r0		; save V0->V1, get V0 from stack
      mov   r0,r5
      call  idiv
      mov   r5,r0		; push to stack
      ldm   r0,_Y		; load INTEGER variable
      mov   r6,r0		; push to stack
      ldm   r0,_Y		; load INTEGER variable
      mov   r1,r0		; save V0->V1, get V0 from stack
      mov   r0,r6
      call  imul
      mov   r6,r0		; push to stack
      ldi   r0,48		; load const
      mov   r1,r0		; save V0->V1, get V0 from stack
      mov   r0,r6
      call  idiv
      mov   r1,r0		; save V0->V1, get V0 from stack
      mov   r0,r5
      add   r0,r1		; make addiction
      stm   r0,_DIST		; store variable
      ldm   r0,_ITERATION		; load INTEGER variable
      mov   r5,r0		; push to stack
      ldm   r0,_MAXITER		; load INTEGER variable
      mov   r1,r0		; save V0->V1, get V0 from stack
      mov   r0,r5
      sub   r0,r1		; make substraction
      cmpi  r0,0		; logical true
      db    #c4,#00,#00,#00
      pop   r3
      shl   r3,13
      shr   r3,15
      ldi   r0,0
      sub   r0,r3
      or    r0,r0		; if NOT jump to
      jz    L5
      ldi   r0,4000		; load const
      stm   r0,_DIST		; store variable
L5:
L6:
      ldm   r0,_DIST		; load variable
      mov   r5,r0		; push to stack
      ldi   r0,192		; load const
      mov   r1,r0		; save V0->V1, get V0 from stack
      mov   r0,r5
      sub   r1,r0		; logical greater
      db    #c4,#00,#00,#00
      pop   r3
      shl   r3,14
      shr   r3,15
      ldi   r0,0
      sub   r0,r3
      cmpi  r0,0		; logical false
      db    #c4,#00,#00,#00
      pop   r3
      shl   r3,13
      shr   r3,15
      ldi   r0,#ffff
      add   r0,r3
      or    r0,r0		; if NOT jump to
      jz    L4
      ldm   r0,_ITERATION		; load INTEGER variable
      mov   r5,r0		; push to stack
      ldm   r0,_MAXITER		; load INTEGER variable
      mov   r1,r0		; save V0->V1, get V0 from stack
      mov   r0,r5
      sub   r0,r1		; make substraction
      cmpi  r0,0		; logical false
      db    #c4,#00,#00,#00
      pop   r3
      shl   r3,13
      shr   r3,15
      ldi   r0,#ffff
      add   r0,r3
      or    r0,r0		; if NOT jump to
      jz    L9
      ldm   r0,_ITERATION		; load INTEGER variable
      mov   r5,r0		; push to stack
      ldi   r0,1		; load const
      mov   r1,r0		; save V0->V1, get V0 from stack
      mov   r0,r5
      sub   r1,r0		; logical greater
      db    #c4,#00,#00,#00
      pop   r3
      shl   r3,14
      shr   r3,15
      ldi   r0,0
      sub   r0,r3
      or    r0,r0		; if NOT jump to
      jz    L12
      ldm   r0,_ITERATION		; load INTEGER variable
      mov   r5,r0		; push to stack
      ldi   r0,4		; load const
      mov   r1,r0		; save V0->V1, get V0 from stack
      mov   r0,r5
      shl   r0,r1
      mov   r5,r0		; push to stack
      ldm   r0,_ITERATION		; load INTEGER variable
      mov   r1,r0		; save V0->V1, get V0 from stack
      mov   r0,r5
      or    r0,r1
      mov   r5,r0		; push to stack
      ldi   r0,8		; load const
      mov   r1,r0		; save V0->V1, get V0 from stack
      mov   r0,r5
      shl   r0,r1
      stm   r0,_TEMP		; store variable
      ldm   r0,_TEMP		; load variable
      mov   r5,r0		; push to stack
      ldm   r0,_ITERATION		; load INTEGER variable
      mov   r6,r0		; push to stack
      ldi   r0,4		; load const
      mov   r1,r0		; save V0->V1, get V0 from stack
      mov   r0,r6
      shl   r0,r1
      mov   r6,r0		; push to stack
      ldm   r0,_ITERATION		; load INTEGER variable
      mov   r1,r0		; save V0->V1, get V0 from stack
      mov   r0,r6
      or    r0,r1
      mov   r1,r0		; save V0->V1, get V0 from stack
      mov   r0,r5
      or    r0,r1
      stm   r0,_TEMP		; store variable
      ldm   r0,_XX		; load variable
      mov   ra,r0
      ldm   r0,_YY		; load variable
      mov   rb,r0
      ldi   rf,_TEMP		; load pointer
      spr   513
      drw   ra,rb,rf
L12:
L13:
L9:
L10:
      ldm   r0,_XX		; load variable
      mov   r5,r0		; push to stack
      ldi   r0,2		; load const
      mov   r1,r0		; save V0->V1, get V0 from stack
      mov   r0,r5
      add   r0,r1		; make addiction
      stm   r0,_XX		; store variable
      ldm   r0,_IX		; load INTEGER variable
      mov   r5,r0		; push to stack
      ldm   r0,Lim_IX		; load variable DWORD
      mov   r1,r0		; save V0->V1, get V0 from stack
      mov   r0,r5
      sub   r0,r1		; logical less
      db    #c4,#00,#00,#00
      pop   r3
      shl   r3,14
      shr   r3,15
      ldi   r0,0
      sub   r0,r3
      or    r0,r0		; if NOT jump to
      jz    L3
      ldm   r0,_IX		; increase var by 1
      addi  r0,1
      stm   r0,_IX
      jmp   L2
L3:
      ldm   r0,_YY		; load variable
      mov   r5,r0		; push to stack
      ldi   r0,2		; load const
      mov   r1,r0		; save V0->V1, get V0 from stack
      mov   r0,r5
      add   r0,r1		; make addiction
      stm   r0,_YY		; store variable
      ldm   r0,_IY		; load INTEGER variable
      mov   r5,r0		; push to stack
      ldm   r0,Lim_IY		; load variable DWORD
      mov   r1,r0		; save V0->V1, get V0 from stack
      mov   r0,r5
      sub   r0,r1		; logical less
      db    #c4,#00,#00,#00
      pop   r3
      shl   r3,14
      shr   r3,15
      ldi   r0,0
      sub   r0,r3
      or    r0,r0		; if NOT jump to
      jz    L1
      ldm   r0,_IY		; increase var by 1
      addi  r0,1
      stm   r0,_IY
      jmp   L0
L1:
_PEND:
      jmp  _PEND		; end program
; ***** Library Code ***** 
;------------------
;IMUL r0,r1
;------------------
imul:
      tsti r0,#8000
      jmz do_check_r1_mul
      tsti r1,#8000
      jmz do_min_plus_mul

      ldi r3,#ffff
      xor r0,r3
      addi r0,1
      xor r1,r3
      addi r1,1
      mul r0,r1,r0
      jmp done_mul

do_min_plus_mul:
      ldi r3,#ffff
      xor r0,r3
      addi r0,1
      mul r0,r1
      xor r0,r3
      addi r0,1
      jmp done_mul

do_check_r1_mul:
      tsti r1,#8000
      jmz do_normal_mul
      ldi r3,#ffff
      xor r1,r3
      addi r1,1
      mul r0,r1
      xor r0,r3
      addi r0,1
      jmp done_mul
do_normal_mul:
      mul r0,r1
done_mul:
      ret      

;------------------
;IDIV r0,r1
;------------------
idiv:
      tsti r0,#8000
      jmz do_check_r1
      tsti r1,#8000
      jmz do_min_plus

      ldi r3,#ffff
      xor r0,r3
      addi r0,1
      xor r1,r3
      addi r1,1
      div r0,r1,r0
      jmp done_div

do_min_plus:
      ldi r3,#ffff
      xor r0,r3
      addi r0,1
      div r0,r1
      xor r0,r3
      addi r0,1
      jmp done_div

do_check_r1:
      tsti r1,#8000
      jmz do_normal
      ldi r3,#ffff
      xor r1,r3
      addi r1,1
      div r0,r1
      xor r0,r3
      addi r0,1
      jmp done_div
do_normal:
      div r0,r1
done_div:
      ret      
; ***** Library Ends *****
; Variable Area
_MAXITER:	DB	#00,#00
_YSIZE:	DB	#00,#00
_XSIZE:	DB	#00,#00
_YSTART:	DB	#00,#00
_XSTART:	DB	#00,#00
_YPIXELS:	DB	#00,#00
_XPIXELS:	DB	#00,#00
_YSTEP:	DB	#00,#00
_XSTEP:	DB	#00,#00
_XTEMP:	DB	#00,#00
_ITERATION:	DB	#00,#00
_Y0:	DB	#00,#00
_X0:	DB	#00,#00
_Y:	DB	#00,#00
_X:	DB	#00,#00
_IY:	DB	#00,#00
_IX:	DB	#00,#00
_DIST:	DB	#00,#00
_TEMP:	DB	#00,#00
_YY:	DB	#00,#00
_XX:	DB	#00,#00
Lim_IY:	DB	#00,#00
Lim_IX:	DB	#00,#00
; String constants


_STACK:
      db      0
