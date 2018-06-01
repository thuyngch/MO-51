;	This program sweeps 4 LED-7seg.
;	The number will be displayed on 4 LED-7seg: "1234"


ledAddr		equ		0000H		; Address of LED 7-seg
numStart	equ		1			;

ORG 0000H
; Init system--------------------------------------------------------
	mov		SP		,#5FH		; Init stack
	mov		R0		,#numStart	; Init number
	mov		R1		,#0			; Init ordinal
; Main loop----------------------------------------------------------
MAIN_L:
	call	display				; Display LED
	;call	delay				; Delay
	call	state				; Update state
	jmp		MAIN_L				; Main loop
; Display LED--------------------------------------------------------
display:
	mov		A		,R1			; Load the ordinal to A
	mov		DPTR	,#DEF		; Point to defined bytes
	movc	A		,@A+DPTR	; Get enable signal
	swap	A					; Swap ordinal to high nibble
	orl		A		,R0			; Number to be displayed
	mov		DPTR	,#ledAddr	; Load address of periph
	movx	@DPTR	,A			; Move data to periph
	ret
; State--------------------------------------------------------------
state:
	cjne	R1		,#3, SKIP	; If reach the end, skip
	mov		R0		,#numStart	; Reset number
	mov		R1		,#0			; Reset ordinal
	ret
SKIP:
	inc		R0					; Next number
	inc		R1					; Next ordinal
	ret
; Delay--------------------------------------------------------------
delay:
	mov		R7		,2
DL:	mov		R6		,250
	djnz	R6		,$
	djnz	R7		,DL
	ret
; Defined bytes------------------------------------------------------
DEF:
DB	07H ,0BH ,0DH, 0EH

end