using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Day11
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
            var panels = new Dictionary<(int x, int y), long>();
            var position = (x: 0, y: 0);
            var current = 0; /* Up */;

            long inputFn()
            {
                var success = panels.TryGetValue(position, out var panel);
                if (!success)
                {
                    panel = 0;
                    panels[position] = panel;
                }

                return panel;
            }

            long outColor = -1;
            long outDirection = -1;
            void outputFn(long o)
            {
                if (outColor == -1)
                    outColor = o;
                else if (outDirection == -1)
                    outDirection = o;                

                if (outColor != -1 && outDirection != -1)
                {
                    var color = panels[position];
                    panels[position] = outColor;

                    if (outDirection == 0)
                        current--;
                    else if (outDirection == 1)
                        current++;

                    // In C# % is the remainder operator, not the modules
                    // For negative numbers, there's a difference.
                    if (current < 0)
                        current += 4;

                    position = (current % 4) switch
                    {
                        0 => (position.x, position.y + 1),
                        1 => (position.x + 1, position.y),
                        2 => (position.x, position.y - 1),
                        3 => (position.x - 1, position.y),
                        _ => throw new Exception()
                    };

                    outColor = -1;
                    outDirection = -1;
                }                    
            }

            var i1 = inputFn();
            Debug.Assert(i1 == 0);

            outputFn(1);
            outputFn(0);

            inputFn();
            outputFn(0);
            outputFn(0);

            inputFn();
            outputFn(1);
            outputFn(0);

            inputFn();
            outputFn(1);
            outputFn(0);

            inputFn();
            outputFn(0);
            outputFn(1);            

            inputFn();
            outputFn(1);
            outputFn(0);            

            inputFn();
            outputFn(1);
            outputFn(0);

            Debug.Assert(panels.Count == 6);
        }         

        static void Part1()
        {
            var input = File.ReadAllText("Input.txt");
            var memory = LoadMemory(input);
            var panels = new Dictionary<(int x, int y), long>();
            var position = (x: 0, y: 0);
            var current = 0; /* Up */;

            long inputFn()
            {
                var success = panels.TryGetValue(position, out var panel);
                if (!success)
                {
                    panel = 0;
                    panels[position] = panel;
                }

                return panel;
            }

            long outColor = -1;
            long outDirection = -1;
            void outputFn(long o)
            {
                if (outColor == -1)
                    outColor = o;
                else if (outDirection == -1)
                    outDirection = o;                

                if (outColor != -1 && outDirection != -1)
                {
                    var color = panels[position];
                    panels[position] = outColor;

                    if (outDirection == 0)
                        current--;
                    else if (outDirection == 1)
                        current++;

                    // In C# % is the remainder operator, not the modules
                    // For negative numbers, there's a difference.
                    if (current < 0)
                        current += 4;

                    position = (current % 4) switch
                    {
                        0 => (position.x, position.y + 1),
                        1 => (position.x + 1, position.y),
                        2 => (position.x, position.y - 1),
                        3 => (position.x - 1, position.y),
                        _ => throw new Exception()
                    };

                    outColor = -1;
                    outDirection = -1;
                }        
            }

            Execute(memory, inputFn, outputFn);
            Debug.Assert(panels.Count == 2539);
        }

        static void Part2()
        {
            var input = File.ReadAllText("Input.txt");
            var memory = LoadMemory(input);
            var panels = new Dictionary<(int x, int y), long>();
            var position = (x: 0, y: 0);
            var current = 0; /* Up */;

            // Start out on white panel and not black as in part 1
            panels[((0, 0))] = 1;

            long inputFn()
            {
                var success = panels.TryGetValue(position, out var panel);
                if (!success)
                {
                    panel = 0;
                    panels[position] = panel;
                }

                return panel;
            }

            long outColor = -1;
            long outDirection = -1;
            void outputFn(long o)
            {
                if (outColor == -1)
                    outColor = o;
                else if (outDirection == -1)
                    outDirection = o;                

                if (outColor != -1 && outDirection != -1)
                {
                    var color = panels[position];
                    panels[position] = outColor;

                    if (outDirection == 0)
                        current--;
                    else if (outDirection == 1)
                        current++;

                    // In C# % is the remainder operator, not the modules
                    // For negative numbers, there's a difference.
                    if (current < 0)
                        current += 4;

                    position = (current % 4) switch
                    {
                        0 => (position.x, position.y + 1),
                        1 => (position.x + 1, position.y),
                        2 => (position.x, position.y - 1),
                        3 => (position.x - 1, position.y),
                        _ => throw new Exception()
                    };

                    outColor = -1;
                    outDirection = -1;
                }        
            }

            Execute(memory, inputFn, outputFn);

            var xs = panels.Keys.Select(k => k.x).ToList();
            var ys = panels.Keys.Select(k => k.y).ToList();

            var answer = new List<string>();

            for (var y = ys.Max(); y >= ys.Min(); y--)
            {
                var s = "";
                for (var x = xs.Min(); x < xs.Max(); x++)
                {
                    if (!panels.ContainsKey((x, y)))
                        s += " ";
                    else
                    {
                        var p = panels[(x,y)];
                        var c = p == 0 ? ' ' : '#';
                        s += c;

                    }
                }
                answer.Add(s);
            }

            var expected = new[]
            {
                " #### #    #### ###  #  #   ## ###   ##   ",
                "    # #    #    #  # # #     # #  # #  #  ",
                "   #  #    ###  ###  ##      # #  # #  #  ",
                "  #   #    #    #  # # #     # ###  ####  ",
                " #    #    #    #  # # #  #  # # #  #  #  ",
                " #### #### #### ###  #  #  ##  #  # #  #  "
            };

            for (var i = 0; i < expected.Length; i++)
                Debug.Assert(expected[i] == answer[i]);
        }

        // Remaining code adapted from Day09.
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
