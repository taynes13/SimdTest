#if WORKER

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SimdTest.Worker
{
    public class SimdTestFixture
    {
        public void RunTestFixture()
        {
            var vectorLength = 100004; // 04 in the end to ensure length is divisible by 4
            var multiplicationsCount = 100000;

            var rand = new Random();
            var vector1 = new float[vectorLength];
            var vector2 = new float[vectorLength];
            for (var i = 0; i < vectorLength; i++)
            {
                vector1[i] = (float)(100 * rand.NextDouble());
                vector2[i] = (float)(100 * rand.NextDouble());
            }

            var titlePrefix = string.Empty;
#if NET_4_5_2
            titlePrefix = ".NET 4.5.2\t";
#elif NET_4_6
            titlePrefix = ".NET 4.6\t";
#endif
            titlePrefix += (Environment.Is64BitProcess ? "x64" : "x86");

            Run(titlePrefix + "\tScalar\t1", Constants.TestRunCount, multiplicationsCount, vector1, vector2, DotProductScalar);
            Run(titlePrefix + "\tVector2\t2", Constants.TestRunCount, multiplicationsCount, vector1, vector2, DotProductVector2);
            Run(titlePrefix + "\tVector4\t4", Constants.TestRunCount, multiplicationsCount, vector1, vector2, DotProductVector4);
#if !NET_4_5_2
            Run(titlePrefix + "\tVectorT\t" + Vector<float>.Count, Constants.TestRunCount, multiplicationsCount, vector1, vector2, DotProductVectorT);
#endif
        }

        private static void Run(string title, int testRunsCount, int multiplicationsCount, float[] vector1, float[] vector2, Func<float[], float[], float> dotProduct)
        {
            var naiveTimeSpan = RepeateRunTestCase(testRunsCount, vector1, vector2, multiplicationsCount, dotProduct);
            Console.Write("{0}\t", title);
            for (var i = 0; i < testRunsCount; i++)
            {
                Console.Write("{0}\t", naiveTimeSpan[i]);
            }
            Console.WriteLine("{0}", TimeSpan.FromMilliseconds(naiveTimeSpan.Average(i => i.TotalMilliseconds)));
        }

        private static List<TimeSpan> RepeateRunTestCase(int repeatCount, float[] vector1, float[] vector2, int iterations, Func<float[], float[], float> dotProduct)
        {
            var result = new List<TimeSpan>();

            for (var i = 0; i < repeatCount; i++)
            {
                result.Add(RunTestCase(vector1, vector2, iterations, dotProduct));
            }

            return result;
        }

        private static TimeSpan RunTestCase(float[] vector1, float[] vector2, int iterations, Func<float[], float[], float> dotProduct)
        {
            var result = 0d;

            var stopwatch = Stopwatch.StartNew();
            for (var i = 0; i < iterations; i++)
            {
                result += dotProduct(vector1, vector2);
            }
            stopwatch.Stop();

            return stopwatch.Elapsed;
        }

        private static float DotProductScalar(float[] vector1, float[] vector2)
        {
            var result = 0f;

            for (var i = 0; i < vector1.Length; i++)
            {
                result += vector1[i] * vector2[i];
            }

            return result;
        }

        private static float DotProductVector2(float[] vector1, float[] vector2)
        {
            var chunkSize = 2;
            var result = 0f;

            Vector2 vectorChunk1;
            Vector2 vectorChunk2;
            for (var i = 0; i < vector1.Length; i += chunkSize)
            {
                vectorChunk1 = new Vector2(vector1[i], vector1[i + 1]);
                vectorChunk2 = new Vector2(vector2[i], vector2[i + 1]);

                result += Vector2.Dot(vectorChunk1, vectorChunk2);
            }

            return result;
        }

        private static float DotProductVector4(float[] vector1, float[] vector2)
        {
            var chunkSize = 4;
            var result = 0f;

            Vector4 vectorChunk1;
            Vector4 vectorChunk2;
            for (var i = 0; i < vector1.Length; i += chunkSize)
            {
                vectorChunk1 = new Vector4(vector1[i], vector1[i + 1], vector1[i + 2], vector1[i + 3]);
                vectorChunk2 = new Vector4(vector2[i], vector2[i + 1], vector2[i + 2], vector2[i + 3]);

                result += Vector4.Dot(vectorChunk1, vectorChunk2);
            }

            return result;
        }

#if !NET_4_5_2
        private static float DotProductVectorT(float[] vector1, float[] vector2)
        {
            var chunkSize = Vector<float>.Count;
            var result = 0f;

            Vector<float> vectorChunk1;
            Vector<float> vectorChunk2;
            for (var i = 0; i < vector1.Length; i += chunkSize)
            {
                vectorChunk1 = new Vector<float>(vector1, i);
                vectorChunk2 = new Vector<float>(vector2, i);

                result += Vector.Dot(vectorChunk1, vectorChunk2);
            }

            return result;
        }
#endif
    }
}

#endif