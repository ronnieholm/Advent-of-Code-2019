using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Day03
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
            {
                var i1 = new[] { "R8","U5","L5","D3" };
                var i2 = new[] { "U7","R6","D4","L4" };
                var p1 = Path(i1);
                var p2 = Path(i2);
                Debug.Assert(ShortestManhattenDistance(p1, p2) == 6);
                Debug.Assert(ShortestSignalDelay(p1, p2) == 30);

            }
            {
                var i1 = new[] { "R75","D30","R83","U83","L12","D49","R71","U7","L72" };
                var i2 = new[] { "U62","R66","U55","R34","D71","R55","D58","R83" };
                var p1 = Path(i1);
                var p2 = Path(i2);
                Debug.Assert(ShortestManhattenDistance(p1, p2) == 159);
                Debug.Assert(ShortestSignalDelay(p1, p2) == 610);
            }
        }

        static int ShortestManhattenDistance(List<(int, int)> p1, List<(int, int)> p2)
        {
            static int Manhatten((int, int) p) => Math.Abs(p.Item1) + Math.Abs(p.Item2);
            return p1.Intersect(p2).Min(Manhatten);
        }

        static int ShortestSignalDelay(List<(int, int)> p1, List<(int, int)> p2)
        {
            var intersections = p1.Intersect(p2);
            var distances = new List<int>();
            foreach (var i in intersections)
            {
                var d1 = p1.IndexOf(i) + 1;
                var d2 = p2.IndexOf(i) + 1;
                distances.Add(d1 + d2);
            }

            var shortest = distances.Min();
            return shortest;
        }

        static List<(int, int)> Path(string[] input1)
        {
            var p = new List<(int, int)>();
            var x = 0;
            var y = 0;

            foreach (var move in input1)
            {
                var d = move[0];
                var l = int.Parse(move[1..]);
                switch (d)
                {
                    case 'U':
                        for (var i = 1; i <= l; i++) 
                            p.Add((x, ++y));
                        break;
                    case 'D':
                        for (var i = 1; i <= l; i++)
                            p.Add((x, --y));
                        break;
                    case 'L':
                        for (var i = 1; i <= l; i++)
                            p.Add((--x, y));
                        break;
                    case 'R':
                        for (var i = 1; i <= l; i++)
                            p.Add((++x, y));
                        break;
                    default:
                        throw new Exception($"Unsupported direction: {d}");
                }
            }

            return p;
        }

        static void Part1()
        {
            var input = File.ReadAllLines("Input.txt");
            var i1 = input[0].Split(",");
            var i2 = input[1].Split(",");
            var p1 = Path(i1);
            var p2 = Path(i2);
            var d = ShortestManhattenDistance(p1, p2);
            Debug.Assert(d == 209);
        }

        static void Part2()
        {
            var input = File.ReadAllLines("Input.txt");
            var i1 = input[0].Split(",");
            var i2 = input[1].Split(",");
            var p1 = Path(i1);
            var p2 = Path(i2);
            var d = ShortestSignalDelay(p1, p2);
            Debug.Assert(d == 43258);
        }
    }
}
