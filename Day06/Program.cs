using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Orbits = System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>>;

namespace Day06
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
            var input = @"COM)B
                B)C
                C)D
                D)E
                E)F
                B)G
                G)H
                D)I
                E)J
                J)K
                K)L".Split("\r\n");
            var orbits = input
                .Select(line => 
                {
                    var orbit = line.Trim().Split(')');
                    return (orbit[0], orbit[1]);

                }).ToArray();
                
            Debug.Assert(CalculateOrbitCountChecksum(orbits) == 42);
        }

        static void Part1()
        {
            var input = File.ReadAllLines("Input.txt");
            var orbits = input
                .Select(line => 
                {
                    var orbit = line.Trim().Split(')');
                    return (orbit[0], orbit[1]);

                }).ToArray();
                
            Debug.Assert(CalculateOrbitCountChecksum(orbits) == 186597);            
        }

        static void Part2()
        {
            // 1. What am YOU orbiting?
            // 2. What is SAN orbiting?
            // 3. Find common node
            // 4. YOU to common + SAN to to common            
        }

        static int CalculateOrbitCountChecksum((string reference, string orbiter)[] orbits)
        {
            // Construct adjacency matrix.
            var tree = new Orbits();
            foreach (var (r, o) in orbits)
            {                
                var success = tree.TryGetValue(r, out var orbiters);
                if (success)
                    orbiters.Add(o);
                else
                    tree[r] = new List<string> { o };
            }            

            var orbitCountChecksum = 0;
            foreach (var r in tree)
                orbitCountChecksum += IndirectOrbits(tree, r.Key);
            return orbitCountChecksum;
        }

        static int IndirectOrbits(Orbits orbits, string reference)
        {
            var orbitCountChecksum = 0;
            var success = orbits.TryGetValue(reference, out var orbiters);
            if (success)
                foreach (var orbiter in orbiters)
                    orbitCountChecksum += IndirectOrbits(orbits, orbiter) + 1;

            return orbitCountChecksum;
        }
    }
}