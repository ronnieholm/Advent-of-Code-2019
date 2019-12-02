using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

namespace Day01
{
    class Program
    {
        static void Main(string[] args)
        {
            var massOfModules = new List<int>();
            var input = File.ReadAllLines("Input.txt");
            foreach (var line in input)
                massOfModules.Add(int.Parse(line));

            // Part 1
            Debug.Assert(FuelForModule(12) == 2);
            Debug.Assert(FuelForModule(14) == 2);
            Debug.Assert(FuelForModule(1969) == 654);

            var fuelForModules = 0;
            foreach (var m in massOfModules)
                fuelForModules += FuelForModule(m);
            Debug.Assert(fuelForModules == 3295539);

            // Part 2
            Debug.Assert(FuelForModulesAndFuel(14) == 2);
            Debug.Assert(FuelForModulesAndFuel(1969) == 966);
            Debug.Assert(FuelForModulesAndFuel(100756) == 50346);

            var fuelForModulesAndFuel = 0;
            foreach (var m in massOfModules)
                fuelForModulesAndFuel += FuelForModulesAndFuel(m);            
            Debug.Assert(fuelForModulesAndFuel == 4940441);
        }

        static int FuelForModule(int mass) => mass / 3 - 2;

        static int FuelForModulesAndFuel(int mass)
        {
            var fuel = mass / 3 - 2;
            if (fuel <= 0)
                return 0;
            return fuel + FuelForModulesAndFuel(fuel);
        }
    }
}