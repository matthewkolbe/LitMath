// Copyright Matthew Kolbe (2022)

using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace LitMath
{
    public static class LitConstants
    {
        public static class Double
        {
            public static class Exp
            {
                public static readonly Vector256<double> HIGH = Vector256.Create(709.0);
                public static readonly Vector256<double> POSITIVE_INFINITY = Vector256.Create(double.PositiveInfinity);
                public static readonly Vector256<double> LOW = Vector256.Create(-709.0);
                public static readonly Vector256<double> LOG2EF = Vector256.Create(1.4426950408889634);
                public static readonly Vector256<double> INVERSE_LOG2EF = Vector256.Create(0.693147180559945);
                public static readonly Vector256<double> P1 = Vector256.Create(1.3981999507E-3);
                public static readonly Vector256<double> P2 = Vector256.Create(8.3334519073E-3);
                public static readonly Vector256<double> P3 = Vector256.Create(4.1665795894E-2);
                public static readonly Vector256<double> P4 = Vector256.Create(1.6666665459E-1);
                public static readonly Vector256<double> P5 = Vector256.Create(5.0000001201E-1);
                public static readonly Vector256<double> ONE = Vector256.Create(1.0);
                public static readonly Vector256<double> MAGIC_LONG_DOUBLE_ADD = Vector256.Create(6755399441055744.0);
            }

            public static class Trig
            {
                public static readonly Vector256<double> TWOPI = Vector256.Create(6.283185307179586476925286766559006);
                public static readonly Vector256<double> PI = Vector256.Create(3.141592653589793238462643383279503);
                public static readonly Vector256<double> HALFPI = Vector256.Create(1.570796326794896619231321691639751);
                public static readonly Vector256<double> QUARTERPI = Vector256.Create(0.7853981633974483096156608458198757);
                public static readonly Vector256<double> THIRDPI = Vector256.Create(Math.PI/3);
                public static readonly Vector256<double> SIN_OF_QUARTERPI = Vector256.Create(0.7071067811865475244008443621);
                public static readonly Vector256<double> P15 = Vector256.Create(-7.64716373181981647590113198578807E-13);
                public static readonly Vector256<double> P14 = Vector256.Create(-1.147074559772972471385169797868211E-11);
                public static readonly Vector256<double> P13 = Vector256.Create(1.605904383682161459939237717015495E-10);
                public static readonly Vector256<double> P12 = Vector256.Create(2.087675698786809897921009032120143E-9);
                public static readonly Vector256<double> P11 = Vector256.Create(-2.505210838544171877505210838544172E-8);
                public static readonly Vector256<double> P10 = Vector256.Create(-2.755731922398589065255731922398589E-7);
                public static readonly Vector256<double> P9 = Vector256.Create(2.755731922398589065255731922398589E-6);
                public static readonly Vector256<double> P8 = Vector256.Create(2.48015873015873015873015873015873E-5);
                public static readonly Vector256<double> P7 = Vector256.Create(-1.984126984126984126984126984126984E-4);
                public static readonly Vector256<double> P6 = Vector256.Create(-0.001388888888888888888888888888888889);
                public static readonly Vector256<double> P5 = Vector256.Create(0.008333333333333333333333333333333333);
                public static readonly Vector256<double> P4 = Vector256.Create(0.04166666666666666666666666666666667);
                public static readonly Vector256<double> P3 = Vector256.Create(-0.1666666666666666666666666666666667);
                public static readonly Vector256<double>[] AT = new Vector256<double>[] {
                    Vector256.Create(0.0                ),
                    Vector256.Create(1.047197555256493   ),
                    Vector256.Create(1.0471971281459846  ),
                    Vector256.Create(0.6643308809370068  ),
                    Vector256.Create(-0.09566827771662459),
                    Vector256.Create(-1.1570749872663317 ),
                    Vector256.Create(1.0347918030424534  ),
                    Vector256.Create(-26.99574707616141  ),
                    Vector256.Create(176.73973590103338  ),
                    Vector256.Create(-831.3695197757582  ),
                    Vector256.Create(2766.973515882119   ),
                    Vector256.Create(-6289.349266587125  ),
                    Vector256.Create(9154.05439634745    ),
                    Vector256.Create(-6394.17188582853   ),
                    Vector256.Create(-2783.067458450766  ),
                    Vector256.Create(7948.1226794182685  ),
                    Vector256.Create(1726.6491309489963  ),
                    Vector256.Create(-18307.17330955125  ),
                    Vector256.Create(22861.86132284062   ),
                    Vector256.Create(-12445.937702363688 ),
                    Vector256.Create(1173.28348129363    ),
                    Vector256.Create(2242.4992978419814  ),
                    Vector256.Create(-1160.707161753395  ),
                    Vector256.Create(187.6185131370321   ) };

                public static readonly int ATORDER = AT.Length;
                public static readonly Vector256<double> HIGH = Vector256.Create(double.MaxValue);
                public static readonly Vector256<double> LOW = Vector256.Create(double.MinValue);
                public static readonly Vector256<double> ONE = Vector256.Create(1.0);
                public static readonly Vector256<double> NEGONE = Vector256.Create(-1.0);
                public static readonly Vector256<double> NEGATIVE_TWO = Vector256.Create(-2.0);
                public static readonly Vector256<double> NEGHALF = Vector256.Create(-0.5);
                public static readonly Vector256<double> HALF = Vector256.Create(0.5);
                public static readonly Vector256<double> ZERO = Vector256.Create(0.0);
                public static readonly Vector256<double> NEGZERO = Vector256.Create(-0.0);
                public static readonly Vector256<double> ALL_BUT_FIRST_ONES = Vector256.Create(BitConverter.ToDouble(new byte[] { 127, 255, 255, 255, 255, 255, 255, 255 }));
            }

            public static class Log
            {
                public static readonly Vector256<double> LOG2EF = Vector256.Create(1.4426950408889634);
                public static readonly Vector256<double> TWO_THIRDS = Vector256.Create(2.0 / 3.0);
                public static readonly Vector256<double> LOG_ONE_POINT_FIVE = Vector256.Create(0.58496250072115619);
                public static readonly Vector256<double> ZERO = Vector256<double>.Zero;
                public static readonly Vector256<double> POSITIVE_INFINITY = Vector256.Create(double.PositiveInfinity);
                public static readonly Vector256<double> TWO = Vector256.Create(2.0);
                public static readonly Vector256<double> ONE_THIRD = Vector256.Create(1.0 / 3.0);
                public static readonly Vector256<double> ONE_FIFTH = Vector256.Create(1.0 / 5.0);
                public static readonly Vector256<double> ONE_SEVENTH = Vector256.Create(1.0 / 7.0);
                public static readonly Vector256<double> ONE_NINTH = Vector256.Create(1.0 / 9.0);
                public static readonly Vector256<double> ONE_ELEVENTH = Vector256.Create(1.0 / 11.0);
                public static readonly Vector256<double> ONE_THIRTEENTH = Vector256.Create(1.0 / 13.0);
                public static readonly Vector256<double> ONE = Vector256.Create(1.0);
                public static readonly Vector256<double> LN2 = Vector256.Create(0.6931471805599453094172321214581766);
            }

            public static class NormDist
            {
                public static readonly Vector256<double> ONE = Vector256.Create(1.0);
                public static readonly Vector256<double> SQRT2 = Vector256.Create(1.4142135623730950488);
                public static readonly Vector256<double> HALF = Vector256.Create(0.5);
            }

            public static class Util
            {
                public static readonly Vector256<double> MAGIC_LONG_DOUBLE_ADD = Vector256.Create(6755399441055744.0);
                public static readonly Vector256<double> NEGATIVE_ZERO = Vector256.Create(-0.0);
            }
        }

        public static class Float
        {
            public static class Exp
            {
                public static readonly Vector256<float> HIGH = Vector256.Create(88.3762626647949f);
                public static readonly Vector256<float> POSITIVE_INFINITY = Vector256.Create(float.PositiveInfinity);
                public static readonly Vector256<float> LOW = Vector256.Create(-88.3762626647949f);
                public static readonly Vector256<float> LOG2EF = Vector256.Create(1.4426950408889634f);
                public static readonly Vector256<float> INVERSE_LOG2EF = Vector256.Create(0.693147180559945f);
                public static readonly Vector256<float> P1 = Vector256.Create(1.3981999507E-3f);
                public static readonly Vector256<float> P2 = Vector256.Create(8.3334519073E-3f);
                public static readonly Vector256<float> P3 = Vector256.Create(4.1665795894E-2f);
                public static readonly Vector256<float> P4 = Vector256.Create(1.6666665459E-1f);
                public static readonly Vector256<float> P5 = Vector256.Create(5.0000001201E-1f);
                public static readonly Vector256<float> ONE = Vector256.Create(1.0f);
            }

            public static class Log
            {
                public static readonly Vector256<float> ZERO = Vector256<float>.Zero;
                public static readonly Vector256<float> POSITIVE_INFINITY = Vector256.Create(float.PositiveInfinity);
                public static readonly Vector256<float> LOG2EF = Vector256.Create(1.4426950408889634f);
                public static readonly Vector256<float> TWO_THIRDS = Vector256.Create(0.66666666666666666f);
                public static readonly Vector256<float> LOG_ONE_POINT_FIVE = Vector256.Create(0.58496250072115619f);
                public static readonly Vector256<float> ONE_THIRD = Vector256.Create(1.0f / 3.0f);
                public static readonly Vector256<float> ONE_FIFTH = Vector256.Create(1.0f / 5.0f);
                public static readonly Vector256<float> ONE_SEVENTH = Vector256.Create(1.0f / 7.0f);
                public static readonly Vector256<float> ONE_NINTH = Vector256.Create(1.0f / 9.0f);
                public static readonly Vector256<float> ONE_ELEVENTH = Vector256.Create(1.0f / 11.0f);
                public static readonly Vector256<float> ONE = Vector256.Create(1.0f);
                public static readonly Vector256<float> LN2 = Vector256.Create(0.69314718055994530941f);
                public static readonly Vector256<float> TWO = Vector256.Create(2.0f);
            }

            public static class NormDist
            {
                public static readonly Vector256<float> SQRT2 = Vector256.Create(1.4142135623730950488f);
                public static readonly Vector256<float> HALF = Vector256.Create(0.5f);
                public static readonly Vector256<float> ONE = Vector256.Create(1.0f);
            }      
        }

        public static class Long
        {
            public static readonly Vector256<long> ONE_THOUSAND_TWENTY_THREE = Vector256.Create(0x3ffL);
            public static readonly Vector256<long> DECIMAL_MASK_FOR_DOUBLE = Vector256.Create(0xfffffffffffffL);
            public static readonly Vector256<long> EXPONENT_MASK_FOR_DOUBLE = Vector256.Create(1023L << 52);
            public static readonly Vector256<long> MAGIC_LONG_DOUBLE_ADD = Vector256.AsInt64<double>(Vector256.Create(6755399441055744.0));
        }

        public static class Int
        {
            public static readonly Vector256<int> ONE_HUNDRED_TWENTY_SEVEN = Vector256.Create(127);
            public static readonly Vector256<int> DECIMAL_MASK_FOR_FLOAT = Vector256.Create(8388607);
            public static readonly Vector256<int> EXPONENT_MASK_FOR_FLOAT = Vector256.Create(127 << 23);
            public static readonly int MAX_MINUS_THREE = int.MaxValue - 3;
            public static readonly int MAX_MINUS_SEVEN = int.MaxValue - 7;
            public static readonly int MAX_MINUS_FIFTEEN = int.MaxValue - 15;
        }
    }
}
