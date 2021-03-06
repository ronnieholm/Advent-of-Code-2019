﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Day07
{
    class Program
    {
        static void Main(string[] args)
        {
            Test();
            Part1();
            Part2();
        }

        static void Swap(ref int a, ref int b)
        {
            int temp = a;
            a = b;
            b = temp;
        }

        static void Permutations(int[] list, int a, int b, List<int[]> result)
        {
            if (a == b)
            { 
                var perm = new int[b + 1];
                for (var i = 0; i <= b; i++)
                    perm[i] = list[i];                    
                result.Add(perm);
            }
            else
            {
                for (var i = a; i <= b; i++)
                {
                    Swap (ref list[a], ref list[i]);
                    Permutations(list, a + 1, b, result);
                    Swap (ref list[a], ref list[i]);
                }
            }
        }

        static void Test()        
        {   
            // Part 1          
            Debug.Assert(TrusterSignalSinglePass("3,15,3,16,1002,16,10,16,1,16,15,15,4,15,99,0,0", 4, 3, 2, 1, 0) == 43210);
            Debug.Assert(TrusterSignalSinglePass("3,23,3,24,1002,24,10,24,1002,23,-1,23,101,5,23,23,1,24,23,23,4,23,99,0,0", 0, 1, 2, 3, 4) == 54321);
            Debug.Assert(TrusterSignalSinglePass("3,31,3,32,1002,32,10,32,1001,31,-2,31,1007,31,0,33,1002,33,7,33,1,33,31,31,1,32,31,31,4,31,99,0,0,0", 1, 0, 4, 3, 2) == 65210);

            // Part 2
            Debug.Assert(TrusterSignalFeedbackLoop("3,26,1001,26,-4,26,3,27,1002,27,2,27,1,27,26,27,4,27,1001,28,-1,28,1005,28,6,99,0,0,5", 9, 8, 7, 6, 5) == 139629729);
            Debug.Assert(TrusterSignalFeedbackLoop("3,52,1001,52,-5,52,3,53,1,52,56,54,1007,54,5,55,1005,55,26,1001,54,-5,54,1105,1,12,1,53,54,53,1008,54,0,55,1001,55,1,55,2,53,55,53,4,53,1001,56,-1,56,1005,56,6,99,0,0,0,0,10", 9, 7, 8, 5, 6) == 18216);
        }        

        static void Part1()
        {
            var program = File.ReadAllText("Input.txt");
            var maxTrusterSignal = int.MinValue;            

            var ints = new[] { 0, 1, 2, 3, 4 };
            var phaseSettings = new List<int[]>();
            Permutations(ints, 0, ints.Length - 1, phaseSettings);

            foreach (var p in phaseSettings)
            {
                var trusterSignal = TrusterSignalSinglePass(program, p[0], p[1], p[2], p[3], p[4]);
                maxTrusterSignal = Math.Max(maxTrusterSignal, trusterSignal);
            }

            Debug.Assert(maxTrusterSignal == 359142);
        }

        static int TrusterSignalSinglePass(string program, int a, int b, int c, int d, int e)
        {            
            var outA = 0;
            var outB = 0;
            var outC = 0;
            var outD = 0;
            var outE = 0;
            Execute(LoadMemory(program), 0, new[] { a, 0 }, (o, _) => outA = o);
            Execute(LoadMemory(program), 0, new[] { b, outA }, (o, _) => outB = o);
            Execute(LoadMemory(program), 0, new[] { c, outB }, (o, _) => outC = o);
            Execute(LoadMemory(program), 0, new[] { d, outC }, (o, _) => outD = o);
            Execute(LoadMemory(program), 0, new[] { e, outD }, (o, _) => outE = o);
            return outE;
        }

        static int TrusterSignalFeedbackLoop(string program, int a, int b, int c, int d, int e)
        {
            var outA = 0;
            var outB = 0;
            var outC = 0;
            var outD = 0;
            var outE = 0;
            var aIp = 0;
            var bIp = 0;
            var cIp = 0;
            var dIp = 0;
            var eIp = 0;
            var program1 = LoadMemory(program);
            var program2 = LoadMemory(program);
            var program3 = LoadMemory(program);
            var program4 = LoadMemory(program);
            var program5 = LoadMemory(program);

            Execute(program1, 0, new[] { a, 0 }, (o, ip) => { outA = o; aIp = ip; });
            Execute(program2, 0, new[] { b, outA }, (o, ip) => { outB = o; bIp = ip; });
            Execute(program3, 0, new[] { c, outB }, (o, ip) => { outC = o; cIp = ip; });
            Execute(program4, 0, new[] { d, outC }, (o, ip) => { outD = o; dIp = ip; });
            Execute(program5, 0, new[] { e, outD }, (o, ip) => { outE = o; eIp = ip; });

            while (true)
            {
                if (Execute(program1, aIp, new[] { outE }, (o, ip) => { outA = o; aIp = ip; }) == ExitCondition.Halt)
                    break;
                if (Execute(program2, bIp, new[] { outA }, (o, ip) => { outB = o; bIp = ip; }) == ExitCondition.Halt)
                    break;
                if (Execute(program3, cIp, new[] { outB }, (o, ip) => { outC = o; cIp = ip; }) == ExitCondition.Halt)
                    break;
                if (Execute(program4, dIp, new[] { outC }, (o, ip) => { outD = o; dIp = ip; }) == ExitCondition.Halt)
                    break;
                if (Execute(program5, eIp, new[] { outD }, (o, ip) => { outE = o; eIp = ip; }) == ExitCondition.Halt)
                    break;
            }

            return outE;
        }

        static void Part2()
        {
            var program = File.ReadAllText("Input.txt");
            var maxTrusterSignal = int.MinValue;            

            var ints = new[] { 5, 6, 7, 8, 9 };
            var phaseSettings = new List<int[]>();
            Permutations(ints, 0, ints.Length - 1, phaseSettings);

            foreach (var p in phaseSettings)
            {
                var trusterSignal = TrusterSignalFeedbackLoop(program, p[0], p[1], p[2], p[3], p[4]);
                maxTrusterSignal = Math.Max(maxTrusterSignal, trusterSignal);
            }

            Debug.Assert(maxTrusterSignal == 4374895);            
        }

        // Remaining code adapted from Day05.
        static int[] LoadMemory(string s)
        {
            var strings = s.Split(',');
            var memory = new int[strings.Length];

            for (var i = 0; i < strings.Length; i++)
                memory[i] = int.Parse(strings[i]);

            return memory;
        }

        enum Opcode
        {
            Add = 1,
            Mul = 2,
            Input = 3,
            Output = 4,
            JumpIfTrue = 5,
            JumpIfFalse = 6,
            LessThan = 7,
            Equals = 8,
            Halt = 99
        }

        enum ParameterMode
        {
            Position = 0,
            Immediate
        }

        enum ExitCondition
        {
            None,
            Output,
            Halt
        }

        static int Digit(int instruction, int position) => (instruction / (int)Math.Pow(10, position - 1)) % 10;

        static ParameterMode ToParameterMode(int instruction, int position)
        {
            return Digit(instruction, position) == 0 
                ? ParameterMode.Position 
                : ParameterMode.Immediate;
        }

        static ExitCondition Execute(int[] memory, int instructionPointer, int[] inputs, Action<int, int> outputFn)
        {
            var inputRequests = 0;
            var ip = instructionPointer;

            int Resolve(int instruction, int position, int address)
            {
                return ToParameterMode(instruction, position) switch
                    {
                        ParameterMode.Position => memory[address],
                        ParameterMode.Immediate => address,
                        _ => throw new Exception($"Unsupported {nameof(ParameterMode)}: {ToParameterMode(instruction, position)}")
                    };
            }

            while (ip < memory.Length)
            {
                var instruction = memory[ip];
                var opcode = (Opcode)(Digit(instruction, 1) + Digit(instruction, 2) * 10);
                switch (opcode)
                {
                    case Opcode.Add:
                    {
                        var p1 = memory[ip + 1];
                        var p2 = memory[ip + 2];
                        var p3 = memory[ip + 3];
                        var p1Value = Resolve(instruction, 3, p1);
                        var p2Value = Resolve(instruction, 4, p2);
                        Debug.Assert(ToParameterMode(instruction, 5) == 0);
                        memory[p3] = p1Value + p2Value;
                        ip += 4;
                        break;
                    }
                    case Opcode.Mul:
                    {
                        var p1 = memory[ip + 1];
                        var p2 = memory[ip + 2];
                        var p3 = memory[ip + 3];
                        var p1Value = Resolve(instruction, 3, p1);
                        var p2Value = Resolve(instruction, 4, p2);
                        Debug.Assert(ToParameterMode(instruction, 5) == 0);
                        memory[p3] = p1Value * p2Value;
                        ip += 4;
                        break;
                    }
                    case Opcode.Input:
                    {
                        var p1 = memory[ip + 1];
                        memory[p1] = inputs[inputRequests++];
                        ip += 2;
                        break;
                    }
                    case Opcode.Output:
                    {
                        var value = memory[ip + 1];                        
                        ip += 2;
                        outputFn(memory[value], ip);
                        return ExitCondition.Output;
                    }
                    case Opcode.JumpIfTrue:
                    {
                        var p1 = memory[ip + 1];
                        var p2 = memory[ip + 2];
                        var p1Value = Resolve(instruction, 3, p1);
                        var p2Value = Resolve(instruction, 4, p2);
                        Debug.Assert(ToParameterMode(instruction, 5) == 0);
                        ip = p1Value != 0 ? p2Value : ip + 3;
                        break;
                    }
                    case Opcode.JumpIfFalse:
                    {
                        var p1 = memory[ip + 1];
                        var p2 = memory[ip + 2];
                        var p1Value = Resolve(instruction, 3, p1);
                        var p2Value = Resolve(instruction, 4, p2);
                        Debug.Assert(ToParameterMode(instruction, 5) == 0);
                        ip = p1Value == 0 ? p2Value : ip + 3;
                        break;
                    }
                    case Opcode.LessThan:
                    {
                        var p1 = memory[ip + 1];
                        var p2 = memory[ip + 2];
                        var p3 = memory[ip + 3];
                        var p1Value = Resolve(instruction, 3, p1);
                        var p2Value = Resolve(instruction, 4, p2);
                        Debug.Assert(ToParameterMode(instruction, 5) == 0);
                        memory[p3] = p1Value < p2Value ? 1 : 0;
                        ip += 4;
                        break;
                    }
                    case Opcode.Equals:
                    {
                        var p1 = memory[ip + 1];
                        var p2 = memory[ip + 2];
                        var p3 = memory[ip + 3];
                        var p1Value = Resolve(instruction, 3, p1);
                        var p2Value = Resolve(instruction, 4, p2);
                        Debug.Assert(ToParameterMode(instruction, 5) == 0);
                        memory[p3] = p1Value == p2Value ? 1 : 0;
                        ip += 4;
                        break;
                    }
                    case Opcode.Halt:
                        return ExitCondition.Halt;
                    default:
                        throw new Exception($"Unknown opcode: {opcode}");
                }
            }    

            return ExitCondition.None;        
        }
    }
}
