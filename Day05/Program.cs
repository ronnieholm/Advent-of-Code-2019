using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

namespace Day05
{
    class Program
    {
        static void Main(string[] args)
        {
            var x = 12345;
            var x0 = x % 10;
            var x1 = (x / 10) % 10;
            var x2 = (x / 100) % 10;
            var x3 = (x / 1000) % 10;
            var x4 = (x / 10000) % 10;

            var y0 = Digit(x, 1);
            var y1 = Digit(x, 2);
            var y2 = Digit(x, 3);
            var y3 = Digit(x, 4);
            var y4 = Digit(x, 5);

            Test();
            Part1();
            Part2();
        }       

        static void Test()
        {
            var m1 = LoadMemory("1002,4,3,4,33");
            var m2 = LoadMemory("1002,4,3,4,99");
            Execute(m1, new int[0] { }, (_) => throw new Exception("Unused"));

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
            var program = File.ReadAllText("Input.txt");
            var memory = LoadMemory(program);
            var output = "";
            Execute(memory, new[] { 1 }, (digit) => output += digit);
            Debug.Assert(output == "3000000006731945");
        }

        static void Part2()
        {
            var input = File.ReadAllText("Input.txt");
            var memory = LoadMemory(input);
            var output = "";
            Execute(memory, new[] { 5 }, (digit) => output += digit);
            Debug.Assert(output == "9571668");
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

        static int Digit(int instruction, int position) => (instruction / (int)Math.Pow(10, position - 1)) % 10;

        static ParameterMode ToParameterMode(int instruction, int position)
        {
            return Digit(instruction, position) == 0 
                ? ParameterMode.Position 
                : ParameterMode.Immediate;
        }

        static void Execute(int[] memory, int[] inputs, Action<int> outputFn)
        {
            var inputRequests = 0;

            int Resolve(int instruction, int position, int address)
            {
                return ToParameterMode(instruction, position) switch
                    {
                        ParameterMode.Position => memory[address],
                        ParameterMode.Immediate => address,
                        _ => throw new Exception($"Unsupported {nameof(ParameterMode)}: {ToParameterMode(instruction, position)}")
                    };
            }

            var ip = 0;
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
                        outputFn(memory[value]);
                        ip += 2;
                        break;
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
                        return;
                    default:
                        throw new Exception($"Unknown opcode: {opcode}");
                }
            }            
        }
    }
}
