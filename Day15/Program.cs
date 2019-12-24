using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

/* Inspiration

    Lizthegrey:
    https://www.twitch.tv/videos/521727650

    Python Breath First Search Maze solving program
    https://www.youtube.com/watch?v=ZuHW4fS60pc

    Path Planning - A* (A-Star)
    https://www.youtube.com/watch?v=icZj67PTFhc

    https://github.com/jackmott/advent2019/blob/master/day_15/src/main.rs

    Use Dejstra path finding
    Use A* path finding
*/

namespace Day15
{
    struct Point
    {
        public int X;
        public int Y;

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Part1();
            Part2();
        }

        static void DrawMap(Dictionary<Point, int> map, Point current)
        {
            if (map.Count > 0)
            {
                var minX = int.MaxValue;
                var maxX = int.MinValue;
                var minY = int.MaxValue;
                var maxY = int.MinValue;

                foreach (var position in map.Keys)
                {
                    if (map[position] == 0)
                        continue;
                    if (position.X > maxX)
                        maxX = position.X;
                    if (position.X < minX)
                        minX = position.X;
                    if (position.Y > maxY)
                        maxY = position.Y;
                    if (position.Y < minY)
                        minY = position.Y;                    
                }

                for (var y = maxY; y >= minY; y--)
                //for (var y = minY; y <= maxY; y++)
                {
                    for (var x = minX; x <= maxX; x++)
                    {                        
                        var s = map.TryGetValue(new Point(x, y), out var v);
                        if (!s)
                            Console.Write("?");
                        else
                        {
                            // if (x == current.X && y == current.Y)
                            //     Console.Write("D");
                            if (v == 0)
                                Console.Write("#");
                            else if (v == 1)
                                Console.Write(".");
                            else if (v == 2)
                                Console.Write("S");                                                
                        }                        
                    }
                    Console.WriteLine();
                }
            }            

            Console.WriteLine("-----------------------------------------------");
        }

        static Point Move(Point v, int direction)
        {
            var r = v;
            if (direction == 1)
                r.Y++;
            else if (direction == 2)
                r.Y--;
            else if (direction == 3)
                r.X--;
            else if (direction == 4)
                r.X++;           
            return r;
        }

        static void Part1()
        {
            var location = new Point(0, 0);
            var supply = new Point(0, 0);
            var rng = new Random();
            var direction = 1;
            var candidate = Move(location, direction);
            
            long inputFn()
            {                
                return direction;
            }

            var map = new Dictionary<Point, int> { { new Point(0, 0), 1 } };
            void outputFn(long o)
            {               
                switch (o)
                {
                    case 0:
                        // hit wall
                        map[candidate] = 0;
                        break;
                    case 1:
                        // moved
                        map[candidate] = 1;
                        location = candidate;
                        break;
                    case 2:
                        // found oxygyn supply
                        map[candidate] = 2;
                        var supply = candidate;



                        DrawMap(map, candidate);
                        PerformBreadthFirstSearch(map, supply);
                        break;
                }

                

                //DrawMap(map, candidate);
                //direction = rng.Next(1, 5);
                candidate = Move(location, direction);                
            }

            var input = File.ReadAllText("Input.txt");
            var memory = LoadMemory(input);
            Execute(memory, inputFn, outputFn);
        }

        static void PerformDepthFirstSearch(Dictionary<Point, int> map, Point supply)
        {

        }

        static void PerformBreadthFirstSearch(Dictionary<Point, int> map, Point supply)
        {
            var start = new Point(0, 0);
            var shortest = new Dictionary<Point, int>();
            shortest[start] = 0;
            var worklist = new List<Point> { start };

            while (true)
            {
                var w = worklist[0];
                for (var direction = 1; direction <= 4; direction++)
                {
                    var moved = Move(w, direction);
                    if (map.ContainsKey(moved) && map[moved] == 0)
                    {
                        continue;
                    }
                    if (moved.Equals(supply))
                    {
                        Console.WriteLine(shortest[w] + 1);

                        // Running the code multiple times, 304 is the distance
                        // we encounter when most of the maze was reached by
                        // random walk. Most of the time, distance is lower due
                        // to parts of the maze not visited by our random walk.
                        // While it answers Part 1 it's a hacky approach. The
                        // better approach is to ensure the entire maze has been
                        // walked.
                        
                        Debug.Assert(shortest[w] + 1 == 304);
                        System.Environment.Exit(0);
                    }

                    if (shortest.ContainsKey(moved))
                    {
                        continue;
                    }
                    else
                    {
                        shortest[moved] = shortest[w] + 1;
                        worklist.Add(moved);
                    }
                }

                worklist = worklist.Skip(1).ToList();
            }        
        }

        static void Part2()
        {

        }

        // Remaining code adapted from Day13
        // TODO: make input an output work through a queue?
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
