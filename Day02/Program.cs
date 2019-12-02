using System;
using System.IO;
using System.Diagnostics;

namespace Day02
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
            var m1 = LoadMemory("1,9,10,3,2,3,11,0,99,30,40,50");
            var m2 = LoadMemory("3500,9,10,70,2,3,11,0,99,30,40,50");
            Execute(m1);

            Debug.Assert(m1.Length == m2.Length);
            for (var i = 0; i < m1.Length; i++)
                Debug.Assert(m1[i] == m2[i]);
        }

        static int[] LoadMemory(string s)
        {
            var strings = s.Split(',');
            var memory = new int[strings.Length];

            for (var i = 0; i < strings.Length; i++)
                memory[i] = int.Parse(strings[i]);

            return memory;
        }

        static void Part1()
        {
            var input = File.ReadAllText("Input.txt");
            var memory = LoadMemory(input);
            memory[1] = 12;
            memory[2] = 2;
            Execute(memory);
            Debug.Assert(memory[0] == 3654868);
        }

        static void Part2()
        {
            var input = File.ReadAllText("Input.txt");
            for (var i = 0; i <= 99; i++)
            {
                for (var j = 0; j <= 99; j++)
                {
                    var memory = LoadMemory(input);
                    memory[1] = i;
                    memory[2] = j;                   
                    Execute(memory);
                    if (memory[0] == 19690720)
                    {
                        var nounVerb = $"{i}{j}";
                        Debug.Assert(nounVerb == "7014");
                        break;
                    }

                }
            }
        }

        static void Execute(int[] memory)
        {
            for (var i = 0; i < memory.Length; i += 4)
            {
                var opcode = memory[i];
                var inputAddress1 = memory[i + 1];
                var inputAddress2 = memory[i + 2];
                var outputAddress = memory[i + 3];

                switch (opcode)
                {
                    case 1:
                        memory[outputAddress] = memory[inputAddress1] + memory[inputAddress2];
                        break;
                    case 2:
                        memory[outputAddress] = memory[inputAddress1] * memory[inputAddress2];
                        break;
                    case 99:
                        return;
                    default:
                        throw new Exception($"Unknown opcode: {opcode}");
                }
            }            
        }
    }
}