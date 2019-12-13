using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace Day10
{
    struct Point
    {
        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double X;
        public double Y;
    }

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
            {
                var lines = new string[]
                {
                    ".#..#",
                    ".....",
                    "#####",
                    "....#",
                    "...##"               
                };
                var map = GetMap(lines);
                var los = GetLinesOfSight(map);
                var max = GetMaxLineOfSight(los);
                Debug.Assert(max.point.Equals(new Point(3, 4)));
                Debug.Assert(max.count == 8);
            }

            {
                var lines = new string[]
                {
                    "......#.#.",
                    "#..#.#....",
                    "..#######.",
                    ".#.#.###..",
                    ".#..#.....",
                    "..#....#.#",
                    "#..#....#.",
                    ".##.#..###",
                    "##...#..#.",
                    ".#....####"
                };
                var map = GetMap(lines);
                var los = GetLinesOfSight(map);
                var max = GetMaxLineOfSight(los);
                Debug.Assert(max.point.Equals(new Point(5, 8)));
                Debug.Assert(max.count == 33);
            }

            {
                var lines = new string[]
                {
                    "#.#...#.#.",
                    ".###....#.",
                    ".#....#...",
                    "##.#.#.#.#",
                    "....#.#.#.",
                    ".##..###.#",
                    "..#...##..",
                    "..##....##",
                    "......#...",
                    ".####.###."
                };
                var map = GetMap(lines);
                var los = GetLinesOfSight(map);
                var max = GetMaxLineOfSight(los);
                Debug.Assert(max.point.Equals(new Point(1, 2)));
                Debug.Assert(max.count == 35);
            }

            {
                var lines = new string[]
                {
                    ".#..#..###",
                    "####.###.#",
                    "....###.#.",
                    "..###.##.#",
                    "##.##.#.#.",
                    "....###..#",
                    "..#.#..#.#",
                    "#..#.#.###",
                    ".##...##.#",
                    ".....#.#.."
                };
                var map = GetMap(lines);
                var los = GetLinesOfSight(map);
                var max = GetMaxLineOfSight(los);
                Debug.Assert(max.point.Equals(new Point(6, 3)));
                Debug.Assert(max.count == 41);
            }

            {
                var lines = new string[]
                {
                    ".#..##.###...#######",
                    "##.############..##.",
                    ".#.######.########.#",
                    ".###.#######.####.#.",
                    "#####.##.#.##.###.##",
                    "..#####..#.#########",
                    "####################",
                    "#.####....###.#.#.##",
                    "##.#################",
                    "#####.##.###..####..",
                    "..######..##.#######",
                    "####.##.####...##..#",
                    ".#####..#.######.###",
                    "##...#.##########...",
                    "#.##########.#######",
                    ".####.#.###.###.#.##",
                    "....##.##.###..#####",
                    ".#.#.###########.###",
                    "#.#.#.#####.####.###",
                    "###.##.####.##.#..##"
                };
                var map = GetMap(lines);
                var los = GetLinesOfSight(map);
                var max = GetMaxLineOfSight(los);
                Debug.Assert(max.point.Equals(new Point(11, 13)));
                Debug.Assert(max.count == 210);

                // Part 2
                var r = BlastAsteriodsAndAngles(new Point(11, 13), map).ToList();
                Debug.Assert(r[0].position.Equals(new Point(11, 12)));
                Debug.Assert(r[1].position.Equals(new Point(12, 1)));
                Debug.Assert(r[3].position.Equals(new Point(12, 2)));
                Debug.Assert(r[10].position.Equals(new Point(12, 8)));
                Debug.Assert(r[20].position.Equals(new Point(16, 0)));
                Debug.Assert(r[50].position.Equals(new Point(16, 9)));
                Debug.Assert(r[100].position.Equals(new Point(10, 16)));
                Debug.Assert(r[199].position.Equals(new Point(9, 6)));
                Debug.Assert(r[200].position.Equals(new Point(8, 2)));
                Debug.Assert(r[201].position.Equals(new Point(10, 9)));
                Debug.Assert(r[299].position.Equals(new Point(11, 1)));                
            }
        }

        static void Part1()
        {
            var lines = File.ReadAllLines("./Input.txt");
            var map = GetMap(lines);
            var los = GetLinesOfSight(map);
            var max = GetMaxLineOfSight(los);            
            Debug.Assert(max.point.Equals(new Point(11, 19)));
            Debug.Assert(max.count == 253);
        }

        static List<Point> GetMap(string[] lines)
        {
            var asteroids = new List<Point>();
            for (var y = 0; y < lines.Length; y++)
                for (var x = 0; x < lines[y].Length; x++)
                    if (lines[y][x] == '#')
                        asteroids.Add(new Point(x, y));
            return asteroids;
        }

        static Dictionary<Point, List<Point>> GetLinesOfSight(List<Point> map)
        {
            // TODO: imporove O(n^3) suboptimal algorithm: https://www.youtube.com/watch?v=5npf1JQYXs0            
            double Distance(Point a, Point b) => Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));
            bool IsBetween(Point a, Point c, Point b) => Distance(a, b) + Distance(b, c) - Distance(a, c) < 0.0001;

            var linesOfSight = new Dictionary<Point, List<Point>>();
            for (var src = 0; src < map.Count; src++)
            {       
                linesOfSight[map[src]] = new List<Point>(); 
                for (var dst = 0; dst < map.Count; dst++)
                {
                    if (src == dst)
                        continue;

                    var lineOfSight = new List<bool>();                   
                    for (var other = 0; other < map.Count; other++)
                    {
                        if (other == src || other == dst)
                            continue;

                        var between = IsBetween(map[src], map[dst], map[other]);
                        lineOfSight.Add(between);
                    }
                    if (lineOfSight.All(x => x == false))
                        linesOfSight[map[src]].Add(map[dst]);
                }
            }

            return linesOfSight;
        }

        static (Point point, int count) GetMaxLineOfSight(Dictionary<Point, List<Point>> asteroids)
        {
            Point p = new Point();
            int c = int.MinValue;
            foreach (var kvp in asteroids)
            {
                if (kvp.Value.Count > c)
                {
                    p = kvp.Key;
                    c = kvp.Value.Count;
                }
            }
            return (p, c);
        }

        static double AngleBetweenStationUpAndAsteriod(Point station, Point asteriod)
        {
            var v = new Point(asteriod.X - station.X, asteriod.Y - station.Y);
            var lengthV = Math.Sqrt(v.X * v.X + v.Y * v.Y);

            // Turn into unit vector      
            v = new Point(v.X / lengthV, v.Y / lengthV);     

            // subtracts -1.57 rad = -90 degress which is up vector of (0,-1)
            var rad = Math.Atan2(v.Y, v.X) - Math.Atan2(-1, 0);

            // Convert to and return degress for easier reasoning
            var degress = rad * (180 / Math.PI);

            // https://stackoverflow.com/questions/18106156/modulo-of-a-number-python-vs-c-sharp
            // In C# % is the remainder. To turn it into module, we need the addition.
            return (degress + 360) % 360;
        }

        static IEnumerable<(Point position, double angle)> BlastAsteriodsAndAngles(Point station, List<Point> map)
        {               
            const double epsilon = -0.0000000001;
            var los = GetLinesOfSight(map);
            var s = los[station];
            var positionAndAngles = s
                .Select(asteroid => (position: asteroid, angle: AngleBetweenStationUpAndAsteriod(station, asteroid)))
                .OrderBy(r => r.angle)
                .ToList();
            var angle = epsilon;

            while (map.Count > 1 /* station asteroid remains */)
            {              
                var head = positionAndAngles.FirstOrDefault(x => x.angle > angle);
                if (head.Equals((new Point(), 0.0)))
                {
                    angle = epsilon;
                    continue;
                }

                angle = head.angle;
                yield return head;
                Console.WriteLine($"({head.position.X},{head.position.Y}). {head.angle}");

                map.Remove(head.position);
                los = GetLinesOfSight(map);
                s = los[station];
                positionAndAngles = s
                    .Select(asteroid => (position: asteroid, angle: AngleBetweenStationUpAndAsteriod(station, asteroid)))
                    .OrderBy(r => r.angle)
                    .ToList();
            }

            yield break;
        }

        static void Part2()
        {
            var lines = File.ReadAllLines("./Input.txt");
            var map = GetMap(lines);            
            var station = new Point(11, 19);
            var asteriodsAndAngles = BlastAsteriodsAndAngles(station, map).ToList();
            var blast200 = asteriodsAndAngles[199];
            var answer = blast200.position.X * 100 + blast200.position.Y;
            Debug.Assert(answer == 815);
        }                
    }
}
