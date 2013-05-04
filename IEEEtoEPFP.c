#include <stdio.h>
#include <string.h>

#define IEEE_BIAS    		1023

#define IEEE_SIGN_BIT_MASK		0x8000000000000000ULL
#define IEEE_EXPONENT_BIT_MASK		0x7ff0000000000000ULL
#define IEEE_MANTISSA_BIT_MASK		0x000fffffffffffffULL

#define IEEE_SIGN_BIT_POSITION		63
#define IEEE_EXPONENT_POSITION		52

#define EPFP_BIAS			128

#define MANTISSA_POSITION_DIFFERENCE	13 /*52 - 39*/

// IEEE Format: S Ex11 Mx52
// EPFP Format: S Mx39 Ex8

// use is in main, could use some improvement though.

struct EPFP 
{
	unsigned int sign;
	unsigned int exponent;
	unsigned long long mantissa;
};

unsigned int get_IEEE_exponent(double d)
{
	return (*(unsigned long long *)&d & IEEE_EXPONENT_BIT_MASK) >> IEEE_EXPONENT_POSITION;
}

unsigned long long get_IEEE_mantissa(double d)
{
	return (*(unsigned long long *)&d & IEEE_MANTISSA_BIT_MASK);
}

unsigned int get_IEEE_sign(double d)
{
	return (*(unsigned long long *)&d & IEEE_SIGN_BIT_MASK) >> IEEE_SIGN_BIT_POSITION;
}

unsigned int IEEE_exponent_to_EPFP_exponent(unsigned int e)
{
	return e + (EPFP_BIAS - IEEE_BIAS);
}

unsigned int EPFP_exponent_to_IEEE_exponent(unsigned int e)
{
	return e + (IEEE_BIAS - EPFP_BIAS);
}

unsigned long long IEEE_mantissa_to_EPFP_mantissa(unsigned long long d)
{
	return d >> MANTISSA_POSITION_DIFFERENCE;
}

void printf_EPFP(struct EPFP e)
{
	printf("sign: %x\n", e.sign);
	printf("exponent: %x\n", e.exponent);
	printf("mantissa: %llx\n", e.mantissa);
}

double hex_char_to_double(unsigned char *c)
{
	double d = 0.0;
	
	unsigned long long t = 0;
	
	t |= (unsigned long long)(*(c + (sizeof(unsigned char) * 0))) << 56;
	t |= (unsigned long long)(*(c + (sizeof(unsigned char) * 1))) << 48;
	t |= (unsigned long long)(*(c + (sizeof(unsigned char) * 2))) << 40;
	t |= (unsigned long long)(*(c + (sizeof(unsigned char) * 3))) << 32;
	t |= (*(c + (sizeof(unsigned char) * 4))) << 24;
	t |= (*(c + (sizeof(unsigned char) * 5))) << 16;
	t |= (*(c + (sizeof(unsigned char) * 6))) << 8;
	t |= (*(c + (sizeof(unsigned char) * 7))) << 0;
	
	memcpy((void *)&d, (void *)&t, sizeof(double));
	
	return d;
}

struct EPFP hex_char_to_EPFP(unsigned char *c)
{
	struct EPFP e;
	
	e.sign = (c[0] & 0x80) >> 7;
	
	e.exponent = c[5];
	
	e.mantissa = 
		((unsigned long long)(c[1] & 0x7f) << 32) |
		((c[1] & 0xff) << 24) |
		((c[2] & 0xff) << 16) |
		((c[3] & 0xff) << 8) |
		((c[4] & 0xff) << 0)
		;
		
	return e;
}

void double_to_char(double d, char* dest)
{
	sprintf(dest, "%llx", *(unsigned long long *)&d);
}

void EPFP_to_char(struct EPFP e, char* dest)
{
	unsigned long long sign_mantissa = 0;
	sign_mantissa = ((unsigned long long)e.sign << 63) | (e.mantissa);
	sprintf(dest, "%llx%x", (unsigned)(unsigned long long)sign_mantissa, (unsigned)(unsigned char)e.exponent);
}

struct EPFP IEEEtoEPFP(double d)
{
	struct EPFP result;
	memset(&result, 0, sizeof(struct EPFP));
	
	result.sign = get_IEEE_sign(d);
	result.exponent = IEEE_exponent_to_EPFP_exponent(get_IEEE_exponent(d));
	result.mantissa = IEEE_mantissa_to_EPFP_mantissa(get_IEEE_mantissa(d));
	
	return result;
}

double EPFPtoIEEE(struct EPFP e)
{
	unsigned long long temp = 0x0LL;
	// sign
	temp |= ((unsigned long long)e.sign) << IEEE_SIGN_BIT_POSITION;
	// exponent
	temp |= ((unsigned long long)EPFP_exponent_to_IEEE_exponent(e.exponent)) << IEEE_EXPONENT_POSITION;
	// mantissa
	temp |= e.mantissa << MANTISSA_POSITION_DIFFERENCE;
	
	double d;
	memcpy(&d, (void *)&temp, sizeof(double));
}

int main()
{
	int i;
	unsigned char da[] = {0x40, 0x09, 0x1E, 0xB8, 0x51, 0xEB, 0x85, 0x1F};
	
	printf("character array: \n");
	for (i=0; i<8; i++)
	{
		printf("%x", (unsigned)(unsigned char)da[i]);
	}
	printf("\n");
	
	double d = hex_char_to_double(da);
	
	printf("char to double: %llx\n", *(unsigned long long *)&d);
	
	unsigned char double_char[100];
	memset(double_char, 0, sizeof(unsigned char) * 100);
	double_to_char(d, double_char);
	
	printf("double to char[]: \n");
	for (i=0; i<16; i++)
	{
		printf("%c", (unsigned char)double_char[i]);
	}
	printf("\n");
	
	printf("double to epfp:\n");
	struct EPFP e = IEEEtoEPFP(d);
	
	printf_EPFP(e);
	
	unsigned char epfp_char[100];
	memset(epfp_char, 0, sizeof(unsigned char) * 100);
	EPFP_to_char(e, epfp_char);
	
	printf("epfp to double: ");
	double back = EPFPtoIEEE(e);
	printf("%f\n", back);
	
	return 0;
}
