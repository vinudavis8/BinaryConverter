using System;
using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics.Metrics;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace CP
{
    // Requirements, Assignment/Implementation Rules:
    //  You must work with 16 bits, hint: Use an int array of 16 as a field
    //  Binary operators: ~, <<, >>
    //  Binary, Arithmatic, and Logical operators: +, -, *, /, ==, !=, <, >, <=, >= algorithms
    //      must perform in binary. Algorthims that convert to denary, perform in denary, and convert
    //      back to binary carry 0 marks.

    class Binary
    {

        #region(Fields)
        private int[] bits;
        private int[] remainderBits;
        #endregion
        #region(Properties)
        private static int ArraySize = 16;
        #endregion
        #region(Constructor)
        public Binary(int[] value)
        {
            bits = value;
        }
        public Binary()
        {
            bits = new int[ArraySize];
        }

        #endregion
        #region(Index operator)
        public int this[int index]
        {
            get
            {
                int value = bits[index];
                return value;
            }
        }

        #endregion
        #region(Implicit Convertors: int to Binary, Binary to int)

        public static implicit operator Binary(int value)
        {

            try
            {
                //steps:
                //divide integer number by 2 repeatedly
                //find remainder of each division
                //reverse order of remainders will be the binary equivalent
                // if integer number  is negative,find 2s compliment of binary number to get actual binary number

                int[] elements = new int[ArraySize];
                int rem;
                int index = 15;
                bool isNegativeNumber = false;
                if (value < 0)
                {
                    isNegativeNumber = true;
                    value *= -1;
                }
                int temp = value;
                while (temp > 0)
                {
                    temp = value / 2;
                    rem = value % 2;
                    elements[index] = rem;
                    value = temp;
                    index--;
                }
                //since by default int array already has 0 while initialising, we are not adding 0 in rest of the places.
                if (isNegativeNumber)
                    return -(new Binary(elements));            //adjusting sign using 2's compliment
                else
                    return new Binary(elements);
            }
            catch (Exception ex)
            {
                return new Binary();
            }

        }

        public static implicit operator int(Binary value)
        {

            try
            {
                bool isNegative = false;
                if (value[0] == 1)          //check if binary number is negative
                {
                    isNegative = true;
                    value = -value;         //find 2s compliment of negative number and add sign later
                }
                int result = 0;
                int pow = 0;
                for (int i = 15; i >= 0; i--)
                {
                    result += value[i] * Power(2, pow);
                    pow++;
                }
                if (isNegative)             //adjust sign if number was negative
                    result = result * -1;
                return result;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        #endregion
        #region(Methods: ToDecimal, ToString)

        public decimal ToDecimal()
        {
            try
            {
                return (int)new Binary(bits);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public override string ToString()
        {
            try
            {
                string binaryString = "";
                for (int i = 0; i < bits.Length; i++)
                {
                    if (i % 4 == 0)
                    {
                        binaryString = binaryString + " " + bits[i];
                    }
                    else
                        binaryString = binaryString + bits[i];
                }
                return binaryString;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }


        #endregion
        #region(Shift Opertors: Shift to left by n (<<), Shift to right by n (>>))


        public static Binary operator <<(Binary x, int shiftBy)
        {
            try
            {
                int[] shiftedBits = new int[ArraySize];
                int startIndex = (x.bits.Count() - 1) - shiftBy;
                for (int i = startIndex; i >= 0; i--)
                {
                    shiftedBits[i] = x[i + shiftBy];
                }
                return new Binary(shiftedBits);

            }
            catch (Exception ex)
            {
                return new Binary();
            }

        }
        public static Binary operator >>(Binary x, int shiftBy)
        {
            try
            {
                int[] shiftedBits = new int[16];
                int startIndex = shiftBy;
                for (int i = shiftBy; i < x.bits.Count(); i++)
                {
                    shiftedBits[i] = x[i - shiftBy];
                }
                return new Binary(shiftedBits);

            }
            catch (Exception ex)
            {
                return new Binary();
            }

        }
        #endregion
        #region(Binary Operators: Ones' complement, Negation)
        public static Binary operator ~(Binary binary)     //ones compliment

        {
            try
            {
                int[] complimentBits = new int[16];
                for (int i = 0; i < binary.bits.Count(); i++)
                {
                    complimentBits[i] = binary[i] == 0 ? 1 : 0;
                }
                return new Binary(complimentBits);
            }
            catch (Exception ex)
            {
                return new Binary();
            }
        }

        public static Binary operator -(Binary x)     //2s compliment

        {
            try
            {
                x = ~x;
                Binary y = 1;
                Binary compliment = x + y;
                return compliment;
            }
            catch (Exception ex)
            {
                return new Binary();
            }
        }
        #endregion
        #region(Binary Arithmatic Opertors: +, -, *, /)

        public static Binary operator +(Binary x, Binary y)
        {
            try
            {
                int[] sumBits = new int[16];
                int carry = 0;
                int sum = 0;
                for (int i = x.bits.Count() - 1; i >= 0; i--)
                {
                    sum = x[i] + y[i] + carry;
                    int res = 0;
                    if (sum == 3)
                    {
                        carry = 1;
                        res = 1;
                    }
                    else
                    if (sum == 2)
                    {
                        carry = 1;
                        res = 0;
                    }
                    else
                    {
                        carry = 0;
                        res = sum;
                    }
                    sumBits[i] = res;
                }
                return new Binary(sumBits);
            }
            catch (Exception ex)
            {
                return new Binary();
            }

        }
        public static Binary operator -(Binary x, Binary y)
        {
            try
            {
                //A-B  is same as A + (-B) so we are reusing the logic of + operator
                Binary complimentY = (-y);
                Binary difference = x + complimentY;
                return difference;
            }
            catch (Exception ex)
            {
                return new Binary();
            }

        }

        public static Binary operator *(Binary x, Binary y)
        {
            try
            {
                int[] sumBits = new int[16];
                int[] resultTemp = new int[16];
                int shiftBy = 0;
                for (int b = y.bits.Count() - 1; b >= 0; b--)
                {
                    resultTemp = new int[16];
                    for (int a = x.bits.Count() - 1; a >= 0; a--)
                    {
                        resultTemp[a] = (y[b] * x[a]);
                    }
                    Binary temp = new Binary(resultTemp);
                    if (shiftBy != 0)
                        temp = temp <<= shiftBy;                        //shift 

                    Binary binary = new Binary(sumBits) + temp;           //add
                    sumBits = binary.bits;
                    shiftBy++;
                }
                return new Binary(sumBits);
            }
            catch (Exception ex)
            {
                return new Binary();
            }
        }
        public static Binary operator /(Binary x, Binary y)
        {
            try
            {
                //if number is negative, take 2s compliment and adjust sign at end
                int signX = 1;
                int signY = 1;
                if (x[0] == 1)
                {
                    x = -x;
                    signX = -1;
                }
                if (y[0] == 1)
                {
                    signY = -1;
                    y = -y;
                }
                int[] result = new int[ArraySize];
                int[] quotientBits = new int[ArraySize];
                result[ArraySize - 1] = x[0];
                int[] b = new int[ArraySize];
                Binary temp = new Binary(result);
                for (int i = 0; i < x.bits.Count(); i++)
                {
                    if (y < temp || y == temp)
                    {
                        quotientBits[i] = 1;
                        b = y.bits;
                    }
                    else
                    {
                        quotientBits[i] = 0;
                        b = new int[ArraySize];
                    }
                    temp = temp - new Binary(b);
                    if (i == ArraySize - 1)                      //skip the shifting in first iteration
                        break;
                    temp = temp << 1;                          // left shift bits by 1
                    temp.bits[ArraySize - 1] = x[i + 1];         // add the new bit from top

                }
                Binary binary = new Binary(quotientBits);      //adjusting sign using 2s compliment if it was negative number
                if ((signX * signY) == -1)
                {
                    binary = -binary;
                }
                Binary rem = new Binary(temp.bits);
                //calculating sign of remainder
                //If the left operand is negative, then make the result negative.
                //If the left operand is positive, then make the result positive.

                if (signX == -1)
                    rem = -rem;

                binary.remainderBits = rem.bits;                    //can be used for modulus operator
                return binary;
            }
            catch (Exception ex)
            {
                return new Binary();
            }
        }


        public static Binary operator %(Binary x, Binary y)
        {
            try
            {
                Binary result = x / y;
                return new Binary(result.remainderBits);            //reusing the logic of binary division
            }
            catch (Exception ex)
            {
                return new Binary(); ;
            }
        }
        #endregion
        #region(Logical Operators: ==, !=, <, >, <=, >=)
        public static bool operator ==(Binary x, Binary y)
        {

            bool flag = true;
            try
            {
                for (int i = 0; i < x.bits.Count(); i++)
                {
                    if (x[i] != y[i])
                    {
                        flag = false;
                        break;
                    }
                }
                return flag;

            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public static bool operator !=(Binary x, Binary y)
        {
            try
            {
                bool result = (x == y);                     //reusing the lofic of == operator
                return !result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public static bool operator <(Binary x, Binary y)
        {

            try
            {
                //If x < y, then y-x  will be positive, 
                //If x > y, then y-x  will be negative,

                bool flag = false;
                if (x == y)
                    return false;
                else
                {
                    Binary result = y - x;                  //reusing the logic of - operator
                    if (result[0] == 0)                     // checking if binary  number is positive
                        flag = true;
                    else
                        flag = false;
                }
                return flag;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public static bool operator >(Binary x, Binary y)
        {
            try
            {
                if (x == y)
                    return false;
                else
                    return !(x < y);                    //reusing the logic of < operator
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool operator <=(Binary x, Binary y)
        {
            try
            {
                bool flag = false;                       //reusing the logic of == and < operator
                if (x == y || x < y)
                    flag = true;
                return flag;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public static bool operator >=(Binary x, Binary y)
        {
            try
            {
                bool flag = false;
                if (x == y || x > y)                    //reusing the logic of == and > operator
                    flag = true;
                return flag;
            }
            catch (Exception ex)
            {
                return false;
            }
        }



        #endregion
        #region custom functions
        private static int Power(int number, int pow)           //methord to calculate power
        {
            try
            {
                if (pow == 0)
                    return 1;
                int result = 1;
                while (pow != 0)
                {
                    result *= number;
                    pow--;
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        #endregion

    }
}
