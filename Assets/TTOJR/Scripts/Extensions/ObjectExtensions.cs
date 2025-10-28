using System;
using UnityEngine;

namespace Extensions
{
    public static class ObjectEX
    {
        //doenst work atm
        public static void NullCheck(this object script, object input)
        {
            if (input == null) script.Error($"Null Check FAILED. {input} resulted in NULL value");

        }
    }
}