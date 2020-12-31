using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.Diagnostics.Debug;

namespace Day01
{
    class Program
    {
        static void Main(string[] args)
        {
            var massOfModules = File.ReadAllLines("Input.txt").Select(int.Parse);

            // Part 1
            Assert(FuelForModule(12) == 2);
            Assert(FuelForModule(14) == 2);
            Assert(FuelForModule(1969) == 654);

            var fuelForModules = Solve(FuelForModule, massOfModules);
            Assert(fuelForModules == 3295539);

            // Part 2
            Assert(FuelForModulesAndFuel(14) == 2);
            Assert(FuelForModulesAndFuel(1969) == 966);
            Assert(FuelForModulesAndFuel(100756) == 50346);

            var fuelForModulesAndFuel = Solve(FuelForModulesAndFuel, massOfModules);
            Assert(fuelForModulesAndFuel == 4940441);
        }

        static int Solve(Func<int, int> fuelFn, IEnumerable<int> masses) =>
            masses.Aggregate(0, (acc, m) => acc + fuelFn(m));

        static int FuelForModule(int mass) => mass / 3 - 2;

        static int FuelForModulesAndFuel(int mass)
        {
            var fuel = FuelForModule(mass);
            return fuel switch
            {
                <= 0 => 0,
                _ => fuel + FuelForModulesAndFuel(fuel)
            };
        }
    }
}