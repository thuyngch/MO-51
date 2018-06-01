#include <AT89X52.h>
#include <stdio.h>

#define		LED		P1_0

void serial_init();
void serial_send(unsigned char dat);
unsigned char serial_read();
void serial_send_str(char* str);
//void delay();
char str[10];

int main(void)
{
	serial_init();
	serial_send_str("\nThis is the UART testing program: Baud=");
	sprintf(str, "%d.\r\n", 19200);	serial_send_str(str);
	serial_send_str("8051 will echos characters that are sent to 8051 through RXD pin.\r\n");
	serial_send_str("Moreover, P1.0 also toggles its state after a character received.\r\n");
	serial_send_str("Now, you should try to type and send some characters to 8051...\r\n");
	while (1)
	{
		serial_send(serial_read());
		LED ^= 1;
	}
}


void serial_init()
{
	TMOD = 0x20;
	SCON = 0x52;
	PCON = SMOD_;
	TH1  = 0xFD;
	TL1  = 0xFD;
	TR1  = 1;
}
void serial_send(unsigned char dat)
{
	while(!TI);
	TI = 0;
	SBUF = dat;
}
void serial_send_str(char* str)
{
	while(*str != 0)
		serial_send(*str++);
}
unsigned char serial_read()
{
	while(!RI);
	RI = 0;
	return SBUF;
}
//void delay()
//{
//	long int i;
//	for(i=0; i<10000; i++);
//}