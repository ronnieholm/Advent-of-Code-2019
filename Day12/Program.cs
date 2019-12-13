using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

// TODO: Go through every day and compare with https://www.youtube.com/watch?v=ZSB8Xr4Z5Q4

namespace Day12
{
    struct Vec3
    {
        public int X;
        public int Y;
        public int Z;

        public Vec3(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

         public static Vec3 operator+(Vec3 a, Vec3 b) => new Vec3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    }

    class Moon
    {
        public Vec3 Position;
        public Vec3 Velocity;
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
            var lines = new[]
            {
                "<x=-1, y=0, z=2>",
                "<x=2, y=-10, z=-7>",
                "<x=4, y=-8, z=8>",
                "<x=3, y=5, z=-1>"
            };
            var moons = Parse(lines);

            {
                // After 0 steps
                var m1 = moons[0];
                var m2 = moons[1];
                var m3 = moons[2];
                var m4 = moons[3];
                Debug.Assert(m1.Position.Equals(new Vec3(-1, 0, 2)) && m1.Velocity.Equals(new Vec3(0, 0, 0)));
                Debug.Assert(m2.Position.Equals(new Vec3(2, -10, -7)) && m2.Velocity.Equals(new Vec3(0, 0, 0)));
                Debug.Assert(m3.Position.Equals(new Vec3(4, -8, 8)) && m3.Velocity.Equals(new Vec3(0, 0, 0)));
                Debug.Assert(m4.Position.Equals(new Vec3(3, 5, -1)) && m4.Velocity.Equals(new Vec3(0, 0, 0)));
            }
 
            {
                Step(moons);

                // After 1 steps
                var m1 = moons[0];
                var m2 = moons[1];
                var m3 = moons[2];
                var m4 = moons[3];
                Debug.Assert(m1.Position.Equals(new Vec3(2, -1, 1)) && m1.Velocity.Equals(new Vec3(3, -1, -1)));
                Debug.Assert(m2.Position.Equals(new Vec3(3, -7, -4)) && m2.Velocity.Equals(new Vec3(1, 3, 3)));
                Debug.Assert(m3.Position.Equals(new Vec3(1, -7, 5)) && m3.Velocity.Equals(new Vec3(-3, 1, -3)));
                Debug.Assert(m4.Position.Equals(new Vec3(2, 2, 0)) && m4.Velocity.Equals(new Vec3(-1, -3, 1)));
            }

            {                    
                Step(moons);

                // After 2 steps
                var m1 = moons[0];
                var m2 = moons[1];
                var m3 = moons[2];
                var m4 = moons[3];
                Debug.Assert(m1.Position.Equals(new Vec3(5, -3, -1)) && m1.Velocity.Equals(new Vec3(3, -2, -2)));
                Debug.Assert(m2.Position.Equals(new Vec3(1, -2, 2)) && m2.Velocity.Equals(new Vec3(-2, 5, 6)));
                Debug.Assert(m3.Position.Equals(new Vec3(1, -4, -1)) && m3.Velocity.Equals(new Vec3(0, 3, -6)));
                Debug.Assert(m4.Position.Equals(new Vec3(1, -4, 2)) && m4.Velocity.Equals(new Vec3(-1, -6, 2)));
            }

            for (var i = 0; i < 8; i++)
                Step(moons);

            {
                // After 10 steps
                var m1 = moons[0];
                var m2 = moons[1];
                var m3 = moons[2];
                var m4 = moons[3];
                Debug.Assert(m1.Position.Equals(new Vec3(2, 1, -3)) && m1.Velocity.Equals(new Vec3(-3, -2, 1)));
                Debug.Assert(m2.Position.Equals(new Vec3(1, -8, 0)) && m2.Velocity.Equals(new Vec3(-1, 1, 3)));
                Debug.Assert(m3.Position.Equals(new Vec3(3, -6, 1)) && m3.Velocity.Equals(new Vec3(3, 2, -3)));
                Debug.Assert(m4.Position.Equals(new Vec3(2, -0, 4)) && m4.Velocity.Equals(new Vec3(1, -1, -1)));
                Debug.Assert(CalculateTotalEnergy(moons) == 179);
            }

            // Part 2
            moons = Parse(lines);
            var answer = Cycle(moons);
            Debug.Assert(answer == 2772);
        }

        static List<Moon> Parse(string[] lines)
        {
            var moons = new List<Moon>();
            var regex = new Regex(@"<x=(?<x>(.+?)), y=(?<y>(.+?)), z=(?<z>(.+?))>");
            foreach (var l in lines)
            {
                var match = regex.Match(l);
                var g = match.Groups;
                var m = new Moon();
                m.Position = 
                    new Vec3(
                        int.Parse(g["x"].Value),
                        int.Parse(g["y"].Value),
                        int.Parse(g["z"].Value));
                moons.Add(m);
            }   
            return moons;
        }

        static void ApplyGravity(List<Moon> moons)
        {
            for (var i = 0; i < moons.Count - 1; i++)
            {                   
                for (var j = i + 1; j < moons.Count; j++)
                {
                    var m1 = moons[i];
                    var m2 = moons[j];
                    if (m1.Position.X > m2.Position.X)
                    {
                        m1.Velocity.X--;
                        m2.Velocity.X++;
                    }
                    else if (m1.Position.X < m2.Position.X) // else will not leave component unchanged if x == x
                    {
                        m1.Velocity.X++;
                        m2.Velocity.X--;
                    }
                    if (m1.Position.Y > m2.Position.Y)
                    {
                        m1.Velocity.Y--;
                        m2.Velocity.Y++;
                    }
                    else if (m1.Position.Y < m2.Position.Y)
                    {
                        m1.Velocity.Y++;
                        m2.Velocity.Y--;
                    }
                    if (m1.Position.Z > m2.Position.Z)
                    {
                        m1.Velocity.Z--;
                        m2.Velocity.Z++;
                    }
                    else if (m1.Position.Z < m2.Position.Z)
                    {
                        m1.Velocity.Z++;
                        m2.Velocity.Z--;
                    }
                }
            }
        }

        static void ApplyVelocity(List<Moon> moons)
        {
            for (var i = 0; i < moons.Count; i++)
            {
                var m = moons[i];
                m.Position += m.Velocity;
            }
        }

        static void Step(List<Moon> moons)
        {
            ApplyGravity(moons);
            ApplyVelocity(moons);
        }

        static double CalculateTotalEnergy(List<Moon> moons)
        {
            var total = 0.0;
            foreach(var m in moons)
            {
                var potential = Math.Abs(m.Position.X) + Math.Abs(m.Position.Y) + Math.Abs(m.Position.Z);
                var kinetic = Math.Abs(m.Velocity.X) + Math.Abs(m.Velocity.Y) + Math.Abs(m.Velocity.Z);
                total += potential * kinetic;
            }
            return total;
        }

        static void Part1()
        {
            var lines = File.ReadAllLines("./Input.txt");
            var moons = Parse(lines);
           
            for (var i = 0; i < 1000; i++)           
                Step(moons);

            Debug.Assert(CalculateTotalEnergy(moons) == 10028);
        }

        // Use long for arguments or method loops forever.
        static long GCD(long a, long b)
        {
            while (a != 0 && b != 0)
            {
                if (a > b)
                    a %= b;
                else
                    b %= a;
            }

            return a == 0 ? b : a;
        }

        // Use long for arguments or method loops forever.
        static long LCM(long a, long b) => (a * b) / GCD(a, b);

        static long Cycle(List<Moon> moons)    
        {
            var ox = false;
            var oy = false;
            var oz = false;
            var cx = 0;
            var cy = 0;
            var cz = 0;
            var initialX = (moons[0].Position.X,moons[1].Position.X,moons[2].Position.X, moons[3].Position.X);
            var initialY = (moons[0].Position.Y,moons[1].Position.Y,moons[2].Position.Y, moons[3].Position.Y);
            var initialZ = (moons[0].Position.Z,moons[1].Position.Z,moons[2].Position.Z, moons[3].Position.Z);

            var i = 2; /* one for initial element and one for repeat of initial element */
            while (ox != true || oy != true || oz != true)
            {         
                Step(moons);

                var xAppend = (moons[0].Position.X,moons[1].Position.X,moons[2].Position.X, moons[3].Position.X);
                var yAppend = (moons[0].Position.Y,moons[1].Position.Y,moons[2].Position.Y, moons[3].Position.Y);
                var zAppend = (moons[0].Position.Z,moons[1].Position.Z,moons[2].Position.Z, moons[3].Position.Z);

                if (xAppend == initialX && !ox)
                {
                    cx = i;
                    ox = true;
                }

                if (yAppend == initialY && !oy)
                {
                    cy = i;
                    oy = true;                    
                }

                if (zAppend == initialZ && !oz)
                {
                    cz = i;
                    oz = true;                    
                }

                i++;
            }

            return LCM(LCM(cx, cy), cz);
        }

        static void Part2()
        {
            // x, y, and z components are independent so we can look for a cycle
            // in x, a cycle in y, and a cycle in z values rather than a cycle
            // in the three combined. The least common multiple is then the
            // cycle count.
            //
            // https://www.calculatorsoup.com/calculators/math/lcm.php 
            //
            // Thinking in terms of increasing digits: if it takes 10 for one
            // digit to repeat and 11 for another,  then it takes LCM(10, 11) =
            // 110 for both to align. In part 2, a sigle digit above would be
            // the combined value of the three x components. Similarly for y and
            // z components.
            var lines = File.ReadAllLines("./Input.txt");
            var moons = Parse(lines);
            var answer = Cycle(moons);
            
            Debug.Assert(answer == 314610635824376);
        }
    }
}
