using System;
using System.Diagnostics;
using System.IO;

namespace Day13
{
    class Program
    {
        static void Main(string[] args)
        {
            Part1();
            Part2();
        }

        static (int, int) Play(long[] memory)
        {
            var padX = 0;
            var ballX = 0;                       
            long inputFn()
            {
                if (ballX < padX)
                    return -1;
                else if (ballX == padX)
                    return 0;
                else
                    return 1;
            }

            var x = -2;
            var y = -2;
            var tileId = -2;
            var segmentDisplay = 0;
            var blockTileCount = 0;
            void outputFn(long o)
            {
                if (x == -2)
                    x = (int)o;
                else if (y == -2)
                    y = (int)o;
                else
                {
                    if (x == -1 && y == 0)
                        segmentDisplay = (int)o;
                    else
                    {
                        // if (o == 4)
                        //     System.Threading.Thread.Sleep(10);

                        tileId = (int)o;    
                        //Console.SetCursorPosition(x, y);
                        switch (tileId)
                        {
                            case 0:
                                //Console.Write(" ");
                                break;
                            case 1:
                                //Console.Write("|");
                                break;
                            case 2:
                                //Console.Write("X");
                                blockTileCount++;
                                break;
                            case 3:
                                //Console.Write("_");
                                padX = x;
                                break;
                            case 4:
                                //Console.Write("o");
                                ballX = x;
                                break;
                            default:
                                throw new Exception($"Unknown tileId: {tileId}");
                        }
                    }

                    x = -2;
                    y = -2;
                    tileId = -2;
                }
            }

            Execute(memory, inputFn, outputFn);
            return (blockTileCount, segmentDisplay);
        }

        static void Part1()
        {
            var input = File.ReadAllText("Input.txt");
            var memory = LoadMemory(input);
            var (blockTileCount, _) = Play(memory);
            Debug.Assert(blockTileCount == 230);
        }

        static void Part2()
        {
            var input = File.ReadAllText("Input.txt");
            var memory = LoadMemory(input);
            memory[0] = 2;

            var (_, segmentDisplay) = Play(memory);
            Debug.Assert(segmentDisplay == 11140);
        }

        // Remaining code adapted from Day11.
        static long[] LoadMemory(string s)
        {
            var strings = s.Split(',');
            var memory = new long[4096];

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

        static void Execute(long[] memory, Func<long> inputFn, Action<long> outputFn)
        {
            var ip = 0L;
            var relativeOffset = 0L;

            long ResolveRead(long instruction, long position, long address)
            {
                var mode = GetParameterMode(instruction, position);
                return mode switch
                {
                    ParameterMode.Position => memory[address],
                    ParameterMode.Immediate => address,
                    ParameterMode.Relative => memory[address + relativeOffset],
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
                        memory[p + offset] = inputFn();
                        ip += 2;
                        break;
                    }
                    case Opcode.Output:
                    {
                        var p = memory[ip + 1];
                        var pValue = ResolveRead(instruction, 3, p);
                        ip += 2;
                        outputFn(pValue);
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
