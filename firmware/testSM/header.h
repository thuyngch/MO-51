#include <stdio.h>
#include <stdint.h>
#include <stdbool.h>


/******************************************************************************
 *	The structure of a line in file hex:
 *	[start][len][addr][type][data][check]
 *
 *	[start]	:(1byte) ':'
 *	[len]	:(2byte) number of data bytes
 *	[addr]	:(4byte) address offset
 *	[type]	:(2byte) 00(data), 01(EOF)
 *	[data]	:(nbyte) data
 *	[check]	:(2byte) 
 *****************************************************************************/
/* Structure of a line in file hex */
typedef struct
{
	uint8_t isField;	// Indicate a new line
	uint8_t dataLen;	// Length of data
	uint16_t addr;		// Address of the first byte in line
	uint8_t type;		// Type of line
	uint8_t dat;		// Data
	uint8_t check;		// Checksum
} tFilHexData;

/* State of file hex processing */
typedef struct
{
	tFilHexData sData;		// Struct data of line
	uint8_t		ui8Global;	// Count part in a line
	uint8_t		ui8Local;	// Pointer byte in a part
} tFilHexState;
// extern	tFilHexState	sState;

/* Type of a line */
typedef enum
{
	eFilHexDataType_data 	= 0,	// Data
	eFilHexDataType_eof		= 1,	// End of file
	eFilHexDataType_extSeg	= 2,	// Extended Segment address
	eFilHexDataType_srtSeg	= 3,	// Start Segment address
	eFilHexDataType_extLin	= 4,	// Extended Linear address
	eFilHexDataType_srtLin	= 5,	// Start Linear address
} tFilHexDataType;
#define	Type(x)				(eFilHexDataType_##x)

/* Global part */
typedef enum
{
	eFilHexGlobal_Len 	= 0,
	eFilHexGlobal_Addr 	= 1,
	eFilHexGlobal_Type 	= 2,
	eFilHexGlobal_Data 	= 3,
	eFilHexGlobal_Check	= 4,
} tFilHexGlobal;
#define	Global(x)			(eFilHexGlobal_##x)


/******************************************************************************
 *	State machine
 *****************************************************************************/
bool
mo51SM
(uint8_t ui8Data);

void
mo51JmpGlobal
(tFilHexGlobal Global);

void
mo51MakeHex
(uint16_t* pui8Des, uint8_t ui8Data);

void
mo51WriteRAM
(bool isDataAvai, FILE *fpOut);

void
mo51IncAddr
(void);