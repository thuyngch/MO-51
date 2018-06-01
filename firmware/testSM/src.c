#include "header.h"

/* State of file hex processing */
static tFilHexState	sState;

/******************************************************************************
 *	State machine.
 *	Input:
 *		ui8Data: Input data to process
 *	Output:	
 *		(bool) Whether a machine code is avaialbe
 *	
 *****************************************************************************/
bool mo51SM(uint8_t ui8Data)
{
	/* Declare */
	bool isDataAvai = false;

	/* Check valid line */
	if(ui8Data == ':')
	{
		sState.sData.isField = true;
		mo51JmpGlobal(Global(Len));
	}
	else if(ui8Data == '\n')
	{
		sState.sData.isField = false;
		sState.sData.dataLen = 0;
		sState.sData.addr = 0;
		sState.sData.type = 0;
		sState.sData.dat = 0;
		sState.sData.check = 0;
		return isDataAvai;
	}
	else
	{
		/* Evaluate state */
		switch(sState.ui8Global)
		{
			case Global(Len):
				mo51MakeHex((uint16_t*)&sState.sData.dataLen, ui8Data);
				sState.ui8Local++;
				if(sState.ui8Local == 2)
					mo51JmpGlobal(Global(Addr));
				break;
			case Global(Addr):
				mo51MakeHex(&sState.sData.addr, ui8Data);
				sState.ui8Local++;
				if(sState.ui8Local == 4)
					mo51JmpGlobal(Global(Type));
				break;
			case Global(Type):
				mo51MakeHex((uint16_t*)&sState.sData.type, ui8Data);
				sState.ui8Local++;
				if(sState.ui8Local == 2)
					mo51JmpGlobal(Global(Data));
				break;
			case Global(Data):
				if(sState.sData.type == Type(data))
				{
					if(sState.ui8Local % 2 == 0)
						sState.sData.dat = 0;
					mo51MakeHex((uint16_t*)&sState.sData.dat, ui8Data);
					if(sState.ui8Local % 2 == 1)
						isDataAvai = true;
					sState.ui8Local++;
					if(sState.ui8Local == 2 * sState.sData.dataLen)
						mo51JmpGlobal(Global(Check));
				}
				break;
			case Global(Check):
				break;
		}

		/* Return */
		return isDataAvai;
	}
}

/******************************************************************************
 *	Jump to a Gobal.
 *	Input:
 *		Global: Which global part in a line?
 *	Output:
 *		None
 *****************************************************************************/
void mo51JmpGlobal(tFilHexGlobal Global)
{
	sState.ui8Global = Global;
	sState.ui8Local = 0;
}

/******************************************************************************
 *	Make a hex value for a variable. The function add the next nibble (the next
 *	lower nibble) to the variable
 *	Input:
 *		pui8Des: Address of variable
 *		ui8Data: A nibble of hexa number
 *	Output:
 *		None
 *****************************************************************************/
void mo51MakeHex(uint16_t* pui8Des, uint8_t ui8Data)
{
	/* Number */
	if(ui8Data >= '0' && ui8Data <= '9')
		ui8Data -= '0';
	
	/* Upper case */
	else if(ui8Data >= 'A' && ui8Data <= 'F')
		ui8Data = ui8Data - 'A' + 10;

	/* Lower case */
	else if(ui8Data >= 'a' && ui8Data <= 'f')
		ui8Data = ui8Data - 'a' + 10;

	/* Calculate */
	*pui8Des = *pui8Des << 4;
	*pui8Des += ui8Data;
}

/******************************************************************************
 *	
 *	Input:
 *		
 *	Output:
 *		None
 *****************************************************************************/
void mo51WriteRAM(bool isDataAvai, FILE *fpOut)
{
	char str[11];

	if(isDataAvai)
	{
		sprintf(str, "%xH: %x\n", sState.sData.addr, sState.sData.dat);
		fputs(str, fpOut);
		mo51IncAddr();
	}
}

/******************************************************************************
 *	
 *	Input:
 *		
 *	Output:
 *		None
 *****************************************************************************/
void mo51IncAddr(void)
{
	sState.sData.addr++;
}