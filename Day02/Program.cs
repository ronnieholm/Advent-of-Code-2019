﻿using System;
using System.IO;
using static System.Diagnostics.Debug;
using static System.Console;

namespace Day02
{
    class Program
    {
        static void Main(string[] args)
        {
            Test();

            var input = File.ReadAllText("Input.txt");
            Part1(input);
            Part2(input);
        }       

        static void Test()
        {
            var m1 = LoadMemory("1,9,10,3,2,3,11,0,99,30,40,50");
            var m2 = LoadMemory("3500,9,10,70,2,3,11,0,99,30,40,50");
            Execute(m1);

            Assert(m1.Length == m2.Length);
            for (var i = 0; i < m1.Length; i++)
                Assert(m1[i] == m2[i]);
        }

        static int[] LoadMemory(string s)
        {
            var strings = s.Split(',');
            var memory = new int[strings.Length];
            for (var i = 0; i < strings.Length; i++)
                memory[i] = int.Parse(strings[i]);
            return memory;
        }

        static void Part1(string input)
        {
            var memory = LoadMemory(input);
            memory[1] = 12;
            memory[2] = 2;
            Execute(memory);
            Assert(memory[0] == 3654868);
        }

        static void Part2(string input)
        {
            for (var noun = 0; noun <= 99; noun++)
            {
                for (var verb = 0; verb <= 99; verb++)
                {
                    var memory = LoadMemory(input);
                    memory[1] = noun;
                    memory[2] = verb;  
                    Execute(memory);
                    if (memory[0] == 19690720)
                    {
                        Assert(7014 == 100 * noun + verb);
                        break;
                    }
                }
            }
        }

        static void Execute(int[] memory)
        {
            for (var ip = 0; ip < memory.Length; ip += 4)
            {
                var opcode = memory[ip];
                var inputAddress1 = memory[ip + 1];
                var inputAddress2 = memory[ip + 2];
                var outputAddress = memory[ip + 3];

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