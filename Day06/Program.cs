using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

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
                
            var map = ConstructAdjacencyMatrix(orbits);
            Debug.Assert(CalculateOrbitCountChecksum(map) == 42);
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
                
            var map = ConstructAdjacencyMatrix(orbits);
            Debug.Assert(CalculateOrbitCountChecksum(map) == 186597);            
        }

        static void Part2()
        {
            var input = File.ReadAllLines("Input.txt");
            var orbits = input
                .Select(line => 
                {
                    var orbit = line.Trim().Split(')');
                    return (orbit[0], orbit[1]);

                }).ToArray();
                
            var map = ConstructAdjacencyMatrix(orbits);

            var youPath = new List<string>();
            PathToCenterOfMass(map, "YOU", youPath);

            var sanPath = new List<string>();
            PathToCenterOfMass(map, "SAN", sanPath);

            int orbitalTransfers = 0;
            foreach (var o in youPath)
            {
                if (sanPath.Contains(o))
                    break;
                orbitalTransfers++;
            }
            foreach (var o in sanPath)
            {
                if (youPath.Contains(o))
                    break;
                orbitalTransfers++;
            }

            Debug.Assert(orbitalTransfers == 412);
        }

        static void PathToCenterOfMass(Dictionary<string, List<string>> orbits, string orbiter, List<string> path)
        {            
            var orbit = orbits.Single(o => o.Value.Contains(orbiter));
            if (orbit.Key == "COM")
            {
                path.Add(orbit.Key);
                return;
            }

            path.Add(orbit.Key);
            PathToCenterOfMass(orbits, orbit.Key, path);
        }

        static Dictionary<string, List<string>> ConstructAdjacencyMatrix((string reference, string orbiter)[] orbits)
        {
            var map = new Dictionary<string, List<string>>();
            foreach (var (r, o) in orbits)
            {                
                var success = map.TryGetValue(r, out var orbiters);
                if (success)
                    orbiters.Add(o);
                else
                    map[r] = new List<string> { o };
            }
            return map;
        }

        static int CalculateOrbitCountChecksum(Dictionary<string, List<string>> orbits)
        {
            var orbitCountChecksum = 0;
            foreach (var r in orbits)
                orbitCountChecksum += IndirectOrbits(orbits, r.Key);
            return orbitCountChecksum;
        }

        static int IndirectOrbits(Dictionary<string, List<string>> orbits, string reference)
        {
            var orbitCountChecksum = 0;
            var success = orbits.TryGetValue(reference, out var orbiters);
            if (success)
                foreach (var orbiter in orbiters)
                    orbitCountChecksum += 1 + IndirectOrbits(orbits, orbiter);

            return orbitCountChecksum;
        }
    }
}