using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;
using Memory = System.Collections.Generic.Dictionary<int, int>;

namespace Day02
{
    class Program
    {
        static void Main(string[] args)
        {
            var m1 = LoadMemory("1,9,10,3,2,3,11,0,99,30,40,50");
            var m2 = LoadMemory("3500,9,10,70,2,3,11,0,99,30,40,50");
            Execute(m1);
            Debug.Assert(Compare(m1, m2));

            var input = File.ReadAllText("Input.txt");
            var memory = LoadMemory(input);
            memory[1] = 12;
            memory[2] = 2;
            Execute(memory);
            Debug.Assert(memory[0] == 3654868);
        }       

        static bool Compare(Memory m1, Memory m2)
        {
            if (m1.Count != m2.Count)
                return false;
            
            foreach (var kvp in m1)
                if (m1[kvp.Key] != m2[kvp.Key])
                    return false;

            return true;
        }

        static Memory LoadMemory(string s)
        {
            var memory = new Memory();
            var strings = s.Split(',');
            var ints = new int[strings.Length];            
            for (var i = 0; i < strings.Length; i++)
                memory[i] = int.Parse(strings[i]);

            return memory;
        }

        static void Execute(Memory memory)
        {
            for (var i = 0; i < memory.Count; i += 4)
            {
                var opcode = memory[i];
                var input1 = memory[i + 1];
                var input2 = memory[i + 2];
                var output = memory[i + 3];

                switch (opcode)
                {
                    case 1:
                        memory[output] = memory[input1] + memory[input2];
                        break;
                    case 2:
                        memory[output] = memory[input1] * memory[input2];
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