using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Day04
{
    class Program
    {
        static void Main(string[] args)
        {
            Test();
            Part1();
            Part2();
        }

        static void Test()
        {
            bool AtLeastTwo(int i) => i >= 2;        
            Debug.Assert(Candidate("111111", AtLeastTwo));
            Debug.Assert(!Candidate("223450", AtLeastTwo));
            Debug.Assert(!Candidate("123789", AtLeastTwo));

            bool ExactlyTwo(int i) => i == 2;        
            Debug.Assert(Candidate("112233", ExactlyTwo));
            Debug.Assert(!Candidate("123444", ExactlyTwo));
            Debug.Assert(Candidate("111122", ExactlyTwo));
        }

        static void Part1()
        {
            var candidates = new List<int>();
            for (var pw = 134792; pw <= 675810; pw++)
                if (Candidate(pw.ToString(), i => i >= 2))
                    candidates.Add(pw);

            Debug.Assert(candidates.Count == 1955);
        }

        static void Part2()
        {
            var candidates = new List<int>();
            for (var pw = 134792; pw <= 675810; pw++)
                if (Candidate(pw.ToString(), i => i == 2))
                    candidates.Add(pw);

            Debug.Assert(candidates.Count == 1319);            
        }

        static bool Candidate(string password, Func<int, bool> adjacencyFn)
        {
            var adjacency = false;
            for (var adjacent = '0'; adjacent <= '9'; adjacent++)
            {
                var max = 0;
                var c = 0;
                for (var j = 0; j < password.Length; j++)
                {
                    if (password[j] == adjacent)
                        c++;
                    else
                    {
                        max = Math.Max(c, max);
                        c = 0;
                    }

                    if (j == password.Length - 1)
                        max = Math.Max(c, max);
                }

                if (adjacencyFn(max))
                {
                    adjacency = true;
                    break;
                }
            }

            if (!adjacency)
                return false;

            var next = password[0];
            for (var i = 1; i < password.Length; i++)
            {
                if (password[i] < next)
                    return false;
                next = password[i];
            }

            return true;
        }    
    }
}