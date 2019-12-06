using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Tree = System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>>;

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
            
        }

        static int CalculateOrbitCountChecksum((string parent, string child)[] orbits)
        {
            // Construct adjacency matrix.
            var tree = new Tree();
            foreach (var o in orbits)
            {                
                if (!tree.ContainsKey(o.parent))
                    tree[o.parent] = new List<string>();
                tree[o.parent].Add(o.child);
            }

            var orbitCountChecksum = 0;
            foreach (var kvp in tree)
                foreach (var child in kvp.Value)
                    orbitCountChecksum += IndirectOrbits(tree, child);

            return orbitCountChecksum;
        }

        static int IndirectOrbits(Tree tree, string orbit)
        {
            var parent = tree.Keys.SingleOrDefault(k => tree[k].Contains(orbit));
            return parent == null ? 0 : 1 + IndirectOrbits(tree, parent);
        }
    }
}