#include "header.h"
#include <stdlib.h>

uint8_t ui8Data;
bool	isDataAvai;

int main()
{
	/* Declare */
	FILE* fpIn = fopen("C:/Users/Chinh Thuy/OneDrive/HCMUT/ACADEMY/SEMESTER IV/MICROPROCESSOR/EXPERIMENT/lab/3/Keil/Objects/lab3.hex", "r");
	FILE* fpOut = fopen("fileBin.bin", "w+");

	/* Read byte to byte in file */
	while (!feof(fpIn))
	{
		/* Read UART */
		ui8Data = fgetc(fpIn);

		/* State machine */
		isDataAvai = mo51SM(ui8Data);

		/* Write to RAM */
		mo51WriteRAM(isDataAvai, fpOut);
	}

	/* Verify RAM */
	printf("--Verify RAM data--\n");
	fseek(fpOut, 0, SEEK_SET);
	while(!feof(fpOut))
		printf("%c", fgetc(fpOut));
	printf("--Done!--\n");

	/* Exit */
	_fcloseall();
	getchar();
}