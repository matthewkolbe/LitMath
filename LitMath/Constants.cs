// Copyright Matthew Kolbe (2022)

using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace LitMath
{
    public static partial class Lit
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
                public static readonly Vector256<double> ONE = Vector256.Create(1.0);
                public static readonly Vector256<double> MAGIC_LONG_DOUBLE_ADD = Vector256.Create(6755399441055744.0);
                public static readonly Vector256<double> INVE = Vector256.Create(0.367879441171442321595523770161);
                public static readonly Vector256<double> THIGH = Vector256.Create(709.0 * 1.4426950408889634);
                public static readonly Vector256<double> TLOW = Vector256.Create(-709.0 * 1.4426950408889634);
                public static readonly Vector256<double> NAN = Vector256.Create(double.NaN);
                public static readonly Vector256<double> T0 = Vector256.Create(1.0);
                public static readonly Vector256<double> T1 = Vector256.Create(0.6931471805599453087156032);
                public static readonly Vector256<double> T2 = Vector256.Create(0.240226506959101195979507231);
                public static readonly Vector256<double> T3 = Vector256.Create(0.05550410866482166557484);
                public static readonly Vector256<double> T4 = Vector256.Create(0.00961812910759946061829085);
                public static readonly Vector256<double> T5 = Vector256.Create(0.0013333558146398846396);
                public static readonly Vector256<double> T6 = Vector256.Create(0.0001540353044975008196326);
                public static readonly Vector256<double> T7 = Vector256.Create(0.000015252733847608224);
                public static readonly Vector256<double> T8 = Vector256.Create(0.000001321543919937730177);
                public static readonly Vector256<double> T9 = Vector256.Create(0.00000010178055034703);
                public static readonly Vector256<double> T10 = Vector256.Create(0.000000007073075504998510);
                public static readonly Vector256<double> T11 = Vector256.Create(0.00000000044560630323);
            }

            public static class Trig
            {
                public static readonly Vector256<double> TWOPI = Vector256.Create(2.0 * System.Math.PI);
                public static readonly Vector256<double> PI = Vector256.Create(System.Math.PI);
                public static readonly Vector256<double> HALFPI = Vector256.Create(0.5 * System.Math.PI);
                public static readonly Vector256<double> NEGHALFPI = Vector256.Create(-System.Math.PI * 0.5);
                public static readonly Vector256<double> QUARTERPI = Vector256.Create(0.25 * System.Math.PI);
                public static readonly Vector256<double> THIRDPI = Vector256.Create(System.Math.PI / 3);
                public static readonly Vector256<double> SIN_OF_QUARTERPI = Vector256.Create(0.7071067811865475244008443621);
                public static readonly Vector256<double> P3 = Vector256.Create(-0.166666666666663509013977  );
                public static readonly Vector256<double> P5 = Vector256.Create(0.008333333333299304989001   );
                public static readonly Vector256<double> P7 = Vector256.Create(-0.00019841269828860068271   );
                public static readonly Vector256<double> P9 = Vector256.Create(0.00000275573170815073144    );
                public static readonly Vector256<double> P11 = Vector256.Create(-0.00000002505191090496049   );
                public static readonly Vector256<double> P13 = Vector256.Create(0.000000000160490521296459   );
                public static readonly Vector256<double> P15 = Vector256.Create(-0.0000000000007384998082865);
                public static readonly Vector256<double> SQP3 = Vector256.Create(-0.1666666666666663969165095 );
                public static readonly Vector256<double> SQP5 = Vector256.Create(0.008333333333324419158220   );
                public static readonly Vector256<double> SQP7 = Vector256.Create(-0.00019841269831470328245   );
                public static readonly Vector256<double> SQP9 = Vector256.Create(0.0000027557314284120030     );
                public static readonly Vector256<double> SQP11 = Vector256.Create(-0.0000000250508528135474    );
                public static readonly Vector256<double> SQP13 = Vector256.Create(0.0000000001590238118466);
                public static readonly Vector256<double> AT2 = Vector256.Create(-0.3333333281594899);
                public static readonly Vector256<double> AT3 = Vector256.Create(0.199999714632415);
                public static readonly Vector256<double> AT4 = Vector256.Create(-0.14285112191172802);
                public static readonly Vector256<double> AT5 = Vector256.Create(0.11104357372799582);
                public static readonly Vector256<double> AT6 = Vector256.Create(-0.0904447441755764);
                public static readonly Vector256<double> AT7 = Vector256.Create(0.07480384892114726);
                public static readonly Vector256<double> AT8 = Vector256.Create(-0.059910413260166356);
                public static readonly Vector256<double> AT9 = Vector256.Create(0.043209392493582424);
                public static readonly Vector256<double> AT10 = Vector256.Create(-0.025599555430499406);
                public static readonly Vector256<double> AT11 = Vector256.Create(0.011187654858740102);
                public static readonly Vector256<double> AT12 = Vector256.Create(-0.0031140210764749898);
                public static readonly Vector256<double> AT13 = Vector256.Create(0.0004071627951457367);
                public static readonly Vector256<double> T3 = Vector256.Create(0.3333333333312579);
                public static readonly Vector256<double> T5 = Vector256.Create(0.1333333335097656);
                public static readonly Vector256<double> T7 = Vector256.Create(0.053968248283506354);
                public static readonly Vector256<double> T9 = Vector256.Create(0.02186958460693491);
                public static readonly Vector256<double> T11 = Vector256.Create(0.008862258714315669);
                public static readonly Vector256<double> T13 = Vector256.Create(0.003598548896841443);
                public static readonly Vector256<double> T15 = Vector256.Create(0.00142747990285674);
                public static readonly Vector256<double> T17 = Vector256.Create(0.0006754958507728294);
                public static readonly Vector256<double> T19 = Vector256.Create(6.40260905512687E-05);
                public static readonly Vector256<double> T21 = Vector256.Create(0.00033371720402669574);
                public static readonly Vector256<double> T23 = Vector256.Create(-0.00015721067265618978);
                public static readonly Vector256<double> T25 = Vector256.Create(9.864578277638557E-05);
                public static readonly Vector256<double> CT1 = Vector256.Create(1.0);
                public static readonly Vector256<double> CT3 = Vector256.Create(0.3333333333333346619643685131);
                public static readonly Vector256<double> CT5 = Vector256.Create(0.1333333333236799972803215674);
                public static readonly Vector256<double> CT7 = Vector256.Create(0.0539682703825024279957999835);
                public static readonly Vector256<double> CT9 = Vector256.Create(0.0218602603709103339870063369);
                public static readonly Vector256<double> CT11 = Vector256.Create(0.0104473875384802020842874186);
                public static readonly Vector256<double> SMALLCONDITION = Vector256.Create(0.07);
                public static readonly Vector256<double> HIGH = Vector256.Create(double.MaxValue);
                public static readonly Vector256<double> LOW = Vector256.Create(double.MinValue);
                public static readonly Vector256<double> ONE = Vector256.Create(1.0);
                public static readonly Vector256<double> NEGONE = Vector256.Create(-1.0);
                public static readonly Vector256<double> NEGATIVE_TWO = Vector256.Create(-2.0);
                public static readonly Vector256<double> NEGHALF = Vector256.Create(-0.5);
                public static readonly Vector256<double> HALF = Vector256.Create(0.5);
                public static readonly Vector256<double> ZERO = Vector256.Create(0.0);
                public static readonly Vector256<double> NEGZERO = Vector256.Create(-0.0);
            }

            public static class Log
            {
                public static readonly Vector256<double> LOG2EF = Vector256.Create(1.4426950408889634);
                public static readonly Vector256<double> TWO_THIRDS = Vector256.Create(2.0 / 3.0);
                public static readonly Vector256<double> LOG_ONE_POINT_FIVE = Vector256.Create(0.58496250072115619);
                public static readonly Vector256<double> ZERO = Vector256<double>.Zero;
                public static readonly Vector256<double> NAN = Vector256.Create(double.NaN);
                public static readonly Vector256<double> POSITIVE_INFINITY = Vector256.Create(double.PositiveInfinity);
                public static readonly Vector256<double> NEGATIVE_INFINITY = Vector256.Create(double.PositiveInfinity);
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
                public static readonly Vector256<double> NEGONE = Vector256.Create(-1.0);
                public static readonly Vector256<double> SQRT2 = Vector256.Create(1.4142135623730950488);
                public static readonly Vector256<double> HALF = Vector256.Create(0.5);
                public static readonly Vector256<double> NEGATIVE_ZERO = Vector256.Create(-0.0);
                public static readonly Vector256<double> ONE_OVER_PI = Vector256.Create(1.0/ System.Math.PI);
                public static readonly Vector256<double> E1 = Vector256.Create(-0.17916959767319535  );
                public static readonly Vector256<double> E2 = Vector256.Create(-0.18542742267595866  );
                public static readonly Vector256<double> E3 = Vector256.Create(-0.13452915843880847  );
                public static readonly Vector256<double> E4 = Vector256.Create(-0.2784782860163457   );
                public static readonly Vector256<double> E5 = Vector256.Create(0.14246708134992647   );
                public static readonly Vector256<double> E6 = Vector256.Create(-0.41925118422030655  );
                public static readonly Vector256<double> E7 = Vector256.Create(0.03746722734143839   );
                public static readonly Vector256<double> E8 = Vector256.Create(0.3009176755909412    );
                public static readonly Vector256<double> E9 = Vector256.Create(-0.6169463046791893   );
                public static readonly Vector256<double> E10 = Vector256.Create(0.4759112697935371   );
                public static readonly Vector256<double> E11 = Vector256.Create(-0.1651167117117661  );
                public static readonly Vector256<double> E12 = Vector256.Create(0.022155411339686473);
            }

            public static class Util
            {
                public static readonly Vector256<double> MAGIC_LONG_DOUBLE_ADD = Vector256.Create(6755399441055744.0);
                public static readonly Vector256<double> NEGATIVE_ZERO = Vector256.Create(-0.0);
                public static readonly Vector256<double> ONE = Vector256.Create(1.0);
                public static readonly Vector256<double> NEGONE = Vector256.Create(-1.0);
                public static readonly Vector256<double> ZERO = Vector256.Create(0.0);
            }

            public static class Polynomial
            {
                public static readonly Vector256<double> TWO = Vector256.Create(2.0);
                public static readonly Vector256<double> ONE = Vector256.Create(1.0);
                public static readonly Vector256<double> NEGONE = Vector256.Create(-1.0);
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
                public static readonly Vector256<float> THIGH = Vector256.Create(81.0f * 1.4426950408889634f);
                public static readonly Vector256<float> TLOW = Vector256.Create(-81.0f * 1.4426950408889634f);
                public static readonly Vector256<float> T0 = Vector256.Create(1.0f);
                public static readonly Vector256<float> T1 = Vector256.Create(0.6931471805500692f);
                public static readonly Vector256<float> T2 = Vector256.Create(0.240226509999339f);
                public static readonly Vector256<float> T3 = Vector256.Create(0.05550410925060949f);
                public static readonly Vector256<float> T4 = Vector256.Create(0.00961804886829518f);
                public static readonly Vector256<float> T5 = Vector256.Create(0.0013333465742372899f);
                public static readonly Vector256<float> T6 = Vector256.Create(0.000154631026827329f);
                public static readonly Vector256<float> T7 = Vector256.Create(1.530610536076361E-05f);
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
                public static readonly Vector256<float> ONE = Vector256.Create(1.0f);
                public static readonly Vector256<float> NEGONE = Vector256.Create(-1.0f);
                public static readonly Vector256<float> SQRT2 = Vector256.Create(1.4142135623730950488f);
                public static readonly Vector256<float> HALF = Vector256.Create(0.5f);
                public static readonly Vector256<float> NEGATIVE_ZERO = Vector256.Create(-0.0f);
                public static readonly Vector256<float> ONE_OVER_PI = Vector256.Create(1.0f / MathF.PI);
                public static readonly Vector256<float> E1 = Vector256.Create(-0.17916959767319535f);
                public static readonly Vector256<float> E2 = Vector256.Create(-0.18542742267595866f);
                public static readonly Vector256<float> E3 = Vector256.Create(-0.13452915843880847f);
                public static readonly Vector256<float> E4 = Vector256.Create(-0.2784782860163457f);
                public static readonly Vector256<float> E5 = Vector256.Create(0.14246708134992647f);
                public static readonly Vector256<float> E6 = Vector256.Create(-0.41925118422030655f);
                public static readonly Vector256<float> E7 = Vector256.Create(0.03746722734143839f);
                public static readonly Vector256<float> E8 = Vector256.Create(0.3009176755909412f);
                public static readonly Vector256<float> E9 = Vector256.Create(-0.6169463046791893f);
                public static readonly Vector256<float> E10 = Vector256.Create(0.4759112697935371f);
                public static readonly Vector256<float> E11 = Vector256.Create(-0.1651167117117661f);
                public static readonly Vector256<float> E12 = Vector256.Create(0.022155411339686473f);
                public static readonly Vector256<float> MAX = Vector256.Create(1e3f);
                public static readonly Vector256<float> MIN = Vector256.Create(-1e3f);
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
            public static readonly Vector256<int> NEGONE = Vector256.Create(-1);
        }
    }
}
