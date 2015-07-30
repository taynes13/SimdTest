#if WORKER

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimdTest.Worker
{
    class Program
    {
        static void Main(string[] args)
        {
            var testFixture = new SimdTestFixture();
            testFixture.RunTestFixture();
        }
    }
}

#endif