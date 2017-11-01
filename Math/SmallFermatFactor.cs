#r "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.2\System.Numerics.dll"
using System.Numerics;

private const int N_MAX = 17;
private const int K_MIN = 1;
private const int K_MAX = 1584000;

for (int n=5; n < N_MAX; n++)
{
    var fermat = BigInteger.Pow(2, 1 << n) + 1;
    Console.WriteLine($"Fermat number n={n.ToString()}, f={fermat.ToString().PadRight(100).Substring(0, 100).Trim()}");

    var factorBase = BigInteger.Pow(2, n + 2);
    Console.WriteLine($"factorBase={factorBase.ToString()}");

    //for (int k = K_MIN; k < K_MAX; k++)
    Parallel.For(K_MIN, K_MAX, k =>
    {
        var factor = (k * factorBase) + 1;
        
        var remainder = BigInteger.Remainder(fermat, factor);
        
        if (remainder == BigInteger.Zero)
        {
            Console.WriteLine($"Found divisor for k={k}");
            Console.WriteLine($"Factor: {factor.ToString()}");
        }
    });
    
    Console.WriteLine(String.Empty);
}

Console.WriteLine("Done.");

/*

Console output:

-----

Fermat number n=5, f=4294967297
factorBase=128
Found divisor for k=5
Factor: 641
Found divisor for k=52347
Factor: 6700417

Fermat number n=6, f=18446744073709551617
factorBase=256
Found divisor for k=1071
Factor: 274177

Fermat number n=7, f=340282366920938463463374607431768211457
factorBase=512

Fermat number n=8, f=115792089237316195423570985008687907853269984665640564039457584007913129639937
factorBase=1024

Fermat number n=9, f=1340780792994259709957402499820584612747936582059239337772356144372176403007354697680187429816690342
factorBase=2048
Found divisor for k=1184
Factor: 2424833

Fermat number n=10, f=1797693134862315907729305190789024733617976978942306572734300811577326758055009631327084773224075360
factorBase=4096
Found divisor for k=11131
Factor: 45592577
Found divisor for k=1583748
Factor: 6487031809

Fermat number n=11, f=3231700607131100730071487668866995196044410266971548403213034542752465513886789089319720141152291346
factorBase=8192
Found divisor for k=39
Factor: 319489
Found divisor for k=119
Factor: 974849

Fermat number n=12, f=1044388881413152506691752710716624382579964249047383780384233483283953907971557456848826811934997558
factorBase=16384
Found divisor for k=7
Factor: 114689
Found divisor for k=1588
Factor: 26017793
Found divisor for k=3892
Factor: 63766529

Fermat number n=13, f=1090748135619415929462984244733782862448264161996232692431832786189721331849119295216264234525201987
factorBase=32768

Fermat number n=14, f=1189731495357231765085759326628007130763444687096510237472674821233261358180483686904488595472612039
factorBase=65536

Fermat number n=15, f=1415461031044954789001553027744951601348130711472388167234385748272366634240845253596025356476648415
factorBase=131072
Found divisor for k=9264
Factor: 1214251009

Fermat number n=16, f=2003529930406846464979072351560255750447825475569751419265016973710894059556311453089506130880933348
factorBase=262144
Found divisor for k=3150
Factor: 825753601

Done.

*/
