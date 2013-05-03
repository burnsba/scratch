// don't have access to a C compile ATM, entirely untested

// IEEE Format: S Ex11 Mx52
// EPFP Format: S Mx39 Ex8
void IEEEtoEPFP(double d)
{
    // working variable, gonna be a 48 bit float when he grows up
    unsigned long long bits;

    memcpy(&bits, &d, sizeof(double));

    // extract exponent from double
    int exponent = (bits & 0x7ff0000000000000LL) >> 52;
    // adjust for IEEE 754 and 48 bit biases 
    exponent = exponent + (127 - 1023);

    // drop the exponent part of the IEEE double
    bits <<= 11;

    // set sign bit
    if(d < 0)
        bits |= (1 << 31);

    // fix endienness
    char result[6];
    result[0] = (bits & 0xff00000000000000LL) >> 56;
    result[1] = (bits & 0x00ff000000000000LL) >> 48;
    result[2] = (bits & 0x0000ff0000000000LL) >> 40;
    result[3] = (bits & 0x000000ff00000000LL) >> 32;
    result[4] = (bits & 0x00000000ff000000LL) >> 24;
    // truncating 32 bits to 8
    result[5] = exponent & 0xff;
            
}
