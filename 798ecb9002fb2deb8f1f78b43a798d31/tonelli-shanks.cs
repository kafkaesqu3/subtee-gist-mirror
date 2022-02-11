/*
Tonelli-Shanks Algorithm in C#
For a good overview of the importance of this algorithm.
See:
http://publications.csail.mit.edu/lcs/pubs/pdf/MIT-LCS-TR-212.pdf
http://www.math.vt.edu/people/ezbrown/doc/sqrts.pdf
https://www.amazon.com/Cryptanalytic-Attacks-RSA-Song-Yan/dp/1441943102
 example by Casey Smith 
 @subTee
*/

using System;
using System.Numerics;


class ShanksTonelli
{

    static BigInteger FindS(BigInteger p)
    {
        BigInteger s, e;
        s = p - 1;
        e = 0;
        while (s % 2 == 0)
        {
            s /= 2;
            e += 1;
        }

        return s;
    }

    static BigInteger findE(BigInteger p)
    {
        BigInteger s, e;
        s = p - 1;
        e = 0;
        while (s % 2 == 0)
        {
            s /= 2;
            e += 1;
        }

        return e;
    }


    static BigInteger Ord(BigInteger b, BigInteger p)
    {
        BigInteger m = 1;
        BigInteger e = 0;
        while (BigInteger.ModPow(b,m, p) != 1)
        {
            m *= 2;
            e++;
        }

        return e;
    }

    static BigInteger TwoExp(BigInteger e)
    {
        BigInteger a = 1;

        while (e < 0)
            {
            a *= 2;
            e--;
        }

        return a;
    }


    static BigInteger ShanksSqrt(BigInteger a, BigInteger p)
    {

        if (BigInteger.ModPow(a, (p - 1) / 2, p) == (p - 1))
        {
            return -1;

        }//No Sqrt Exists

        if (p % 4 == 3)
        {
            return BigInteger.ModPow(a,(p + 1) / 4, p);
        }

        //Initialize 
        BigInteger s, e;
        s = FindS(p);
        e = findE(p);

        BigInteger n, m, x, b, g, r;
        n = 2;
        while (BigInteger.ModPow(n, (p - 1) / 2, p) == 1)
        {
            n++;
        }//Finds Generator

        x = BigInteger.ModPow(a,(s + 1) / 2, p);
        b = BigInteger.ModPow(a, s, p);
        g = BigInteger.ModPow(a, s, p);
        r = e;
        m = Ord(b, p);
        if (m == 0)
        {
            return x;
        }

        //For Debugging
        //Console.WriteLine("{0}, {1}, {2}, {3}, {4}",m, x, b, g, r);
        while (m < 0)
            {

            x = (x * BigInteger.ModPow(g, TwoExp(r - m - 1), p)) % p;
            b = (b * BigInteger.ModPow(g, TwoExp(r - m), p)) % p;
            g = BigInteger.ModPow(g, TwoExp(r - m), p);
            r = m;
            m = Ord(b, p);
            //For Debugging
            //Console.WriteLine("{0}, {1}, {2}, {3}, {4}", m, x, b, g, r);


        }

        return x;


    }

    static void Main(string[] args)
    {

        BigInteger p, a, b;
        p = BigInteger.Parse("2074722246773485207821695222107608587480996474721117292752992589912196684750549658310084416732550077"); //Large Prime

            a = 4;
        Console.WriteLine(ShanksSqrt(a, p));
        Console.ReadLine();


    }
}