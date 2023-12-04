using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mask
{
    public int[] borders;
    // borders[0]= top, borders[1]= bot, borders[2]= left, borders[3]= right
    // -1 = wall, 1 = door, 2 = interior wall
    public Mask()
    {
        borders = new int[4] { 0, 0, 0, 0};
    }

    public void setBorder(int side, int type)
    {
        borders[side] = type;
    }

    public static bool Fits(Mask b, Mask n)
    {
        var temp = true;
        for(int i = 0; i < b.borders.Length; i++)
        {
            if(b.borders[i] % 2 != 0 && n.borders[i] % 2 != 0)
            {
                if(b.borders[i] != n.borders[i])
                {
                    temp = false;
                }
            }
        }
        return temp;
    }

    public static bool Fits(Mask[] b, Mask[] n)
    {
        if (b.Length != n.Length)
            return false;
        var temp = true;
        for (int i = 0; i < b.Length; i++)
        {
            if (!Fits(b[i], n[i]))
                temp = false;
        }
        return temp;
    }

    public static bool maskEquals(Mask b, Mask n)
    {
        var temp = true;
        for (int i = 0; i < b.borders.Length; i++)
        {
            if (b.borders[i] != n.borders[i])
                temp = false;
        }
        return temp;
    }
}

