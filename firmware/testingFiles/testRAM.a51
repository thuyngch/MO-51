;	This program is to check RAM connection and its operation.
;
;	Program will write to RAM (addr 2000H) a data with value '0' (48 in dec and 30 in hex).
;	Then 8051 will read data from RAM at addr 2000H.
;
;	If data == '0', P1.0 will be turned on, the rest of P1 are off.
;	If data != '0', P1.1 will be turned on, the rest of P1 are off.

ORG 	0000H
	
	mov P1, #0

	mov	A, #'0'
	mov DPTR, #2000H
	movx @DPTR, A
	
	;mov	A, #'1'
	;inc DPTR
	;movx @DPTR, A
	
	;mov	A, #'2'
	;inc DPTR
	;movx @DPTR, A
	
	;mov	A, #'3'
	;inc DPTR
	;movx @DPTR, A
	
	mov DPTR, #2000H
	movx A, @DPTR
	
	cjne A, #'0', l1
	setb P1.0
	jmp exit
l1:
	setb P1.1
exit:
	jmp $
END
