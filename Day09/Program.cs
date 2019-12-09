using System;
using System.Diagnostics;
using System.IO;

namespace Day09
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
            // Part 1
            var m1 = LoadMemory("109,1,204,-1,1001,100,1,100,1008,100,16,101,1006,101,0,99");
            var output = "";
            Execute(m1, 0, new long[] {}, (o, _) => output += o);
            Debug.Assert(output == "1091204-1100110011001008100161011006101099");

            var m2 = LoadMemory("1102,34915192,34915192,7,4,7,99,0");
            Execute(m2, 0, new long[] {}, (o, _) => Debug.Assert(o == 1_219_070_632_396_864));

            var m3 = LoadMemory("104,1125899906842624,99");
            Execute(m3, 0, new long[] {}, (o, _) => Debug.Assert(o == 1125899906842624));
        }

        static void Part1()
        {
            var input = File.ReadAllText("Input.txt");
            var memory = LoadMemory(input);
            var output = "";
            Execute(memory, 0, new[] { 1L }, (o, _) => output += o);
            Debug.Assert(output == "3013554615");
        }

        static void Part2()
        {
            var input = File.ReadAllText("Input.txt");
            var memory = LoadMemory(input);
            var output = "";
            Execute(memory, 0, new[] { 2L }, (o, _) => output += o);
            Debug.Assert(output == "50158");
        }

        // Remaining code adapted from Day07.
        static long[] LoadMemory(string s)
        {
            var strings = s.Split(',');
            var memory = new long[2048];

            for (var i = 0; i < strings.Length; i++)
                memory[i] = long.Parse(strings[i]);

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
            RelativeBaseOffset = 9,
            Halt = 99
        }

        enum ParameterMode
        {
            Position = 0,
            Immediate,
            Relative
        }

        static long Digit(long instruction, long position) => (instruction / (long)Math.Pow(10, position - 1)) % 10;

        static ParameterMode GetParameterMode(long instruction, long position)
        {
            var mode = Digit(instruction, position);
            return mode switch
            {
                0 => ParameterMode.Position,
                1 => ParameterMode.Immediate,
                2 => ParameterMode.Relative,
                _ => throw new Exception($"Unsupported {nameof(ParameterMode)}: {mode}")
            };
        }

        static void Execute(long[] memory, long instructionPointer, long[] inputs, Action<long, long> outputFn)
        {
            var inputRequests = 0L;
            var ip = instructionPointer;
            var relativeOffset = 0L;

            long ResolveRead(long instruction, long position, long address)
            {
                var mode = GetParameterMode(instruction, position);
                return mode switch
                {
                    ParameterMode.Position => memory[address],
                    ParameterMode.Immediate => address,
                    ParameterMode.Relative => memory[relativeOffset + address],
                    _ => throw new Exception($"Unsupported {nameof(ParameterMode)}: {mode}")
                };
            }

            long ResolveWrite(long instruction, long position, long parameter)
            {
                var offset = GetParameterMode(instruction, position) == ParameterMode.Position 
                    ? 0
                    : relativeOffset;
                return parameter + offset;
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
                        var p1Value = ResolveRead(instruction, 3, p1);
                        var p2Value = ResolveRead(instruction, 4, p2);
                        p3 = ResolveWrite(instruction, 5, p3);
                        memory[p3] = p1Value + p2Value;
                        ip += 4;
                        break;
                    }
                    case Opcode.Mul:
                    {
                        var p1 = memory[ip + 1];
                        var p2 = memory[ip + 2];
                        var p3 = memory[ip + 3];
                        var p1Value = ResolveRead(instruction, 3, p1);
                        var p2Value = ResolveRead(instruction, 4, p2);
                        p3 = ResolveWrite(instruction, 5, p3);
                        memory[p3] = p1Value * p2Value;
                        ip += 4;
                        break;
                    }
                    case Opcode.Input:
                    {
                        var offset = GetParameterMode(instruction, 3) == ParameterMode.Position ? 0 : relativeOffset; 
                        var p = memory[ip + 1];
                        memory[p + offset] = inputs[inputRequests++];
                        ip += 2;
                        break;
                    }
                    case Opcode.Output:
                    {
                        var p = memory[ip + 1];
                        var pValue = ResolveRead(instruction, 3, p);
                        ip += 2;
                        outputFn(pValue, ip);
                        break;
                    }
                    case Opcode.JumpIfTrue:
                    {
                        var p1 = memory[ip + 1];
                        var p2 = memory[ip + 2];
                        var p1Value = ResolveRead(instruction, 3, p1);
                        var p2Value = ResolveRead(instruction, 4, p2);
                        Debug.Assert(GetParameterMode(instruction, 5) == ParameterMode.Position);
                        ip = p1Value != 0 ? p2Value : ip + 3;
                        break;
                    }
                    case Opcode.JumpIfFalse:
                    {
                        var p1 = memory[ip + 1];
                        var p2 = memory[ip + 2];
                        var p1Value = ResolveRead(instruction, 3, p1);
                        var p2Value = ResolveRead(instruction, 4, p2);
                        Debug.Assert(GetParameterMode(instruction, 5) == ParameterMode.Position);
                        ip = p1Value == 0 ? p2Value : ip + 3;
                        break;
                    }
                    case Opcode.LessThan:
                    {
                        var p1 = memory[ip + 1];
                        var p2 = memory[ip + 2];
                        var p3 = memory[ip + 3];
                        var p1Value = ResolveRead(instruction, 3, p1);
                        var p2Value = ResolveRead(instruction, 4, p2);
                        p3 = ResolveWrite(instruction, 5, p3);
                        memory[p3] = p1Value < p2Value ? 1 : 0;
                        ip += 4;
                        break;
                    }
                    case Opcode.Equals:
                    {
                        var p1 = memory[ip + 1];
                        var p2 = memory[ip + 2];
                        var p3 = memory[ip + 3];
                        var p1Value = ResolveRead(instruction, 3, p1);
                        var p2Value = ResolveRead(instruction, 4, p2);
                        p3 = ResolveWrite(instruction, 5, p3);
                        memory[p3] = p1Value == p2Value ? 1 : 0;
                        ip += 4;
                        break;
                    }
                    case Opcode.RelativeBaseOffset:
                    {
                        var p = memory[ip + 1];
                        var pValue = ResolveRead(instruction, 3, p);
                        relativeOffset += pValue;
                        ip += 2;
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
