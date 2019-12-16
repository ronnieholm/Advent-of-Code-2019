using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Day14
{
    class Chemical
    {
        public long Quantity;
        public string Label;
    }

    class Reaction
    {
        public Chemical[] Left;
        public Chemical Right;
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
                var s = new string[]
                {
                    "10 ORE => 10 A",
                    "1 ORE => 1 B",
                    "7 A, 1 B => 1 C",
                    "7 A, 1 C => 1 D",
                    "7 A, 1 D => 1 E",
                    "7 A, 1 E => 1 FUEL"
                };

                var reactions = Parse(s);
                var goal = new Chemical { Quantity = 1, Label = "FUEL" };
                var store = new Dictionary<string, long>();
                var oreRequired = Produce(reactions, goal, store, 0);
                Debug.Assert(oreRequired == 31);
                Debug.Assert(store.Count == 1 && store["A"] == 2);
            }
            {
                var s = new string[]
                {
                    "9 ORE => 2 A",
                    "8 ORE => 3 B",
                    "7 ORE => 5 C",
                    "3 A, 4 B => 1 AB",
                    "5 B, 7 C => 1 BC",
                    "4 C, 1 A => 1 CA",
                    "2 AB, 3 BC, 4 CA => 1 FUEL"
                };

                var reactions = Parse(s);
                var goal = new Chemical { Quantity = 1, Label = "FUEL" };
                var store = new Dictionary<string, long>();
                var oreRequired = Produce(reactions, goal, store, 0);
                Debug.Assert(oreRequired == 165);
                Debug.Assert(store.Count == 2 && store["B"] == 1 && store["C"] == 3);
            }
            {
                var s = new string[]
                {
                    "157 ORE => 5 NZVS",
                    "165 ORE => 6 DCFZ",
                    "44 XJWVT, 5 KHKGT, 1 QDVJ, 29 NZVS, 9 GPVTF, 48 HKGWZ => 1 FUEL",
                    "12 HKGWZ, 1 GPVTF, 8 PSHF => 9 QDVJ",
                    "179 ORE => 7 PSHF",
                    "177 ORE => 5 HKGWZ",
                    "7 DCFZ, 7 PSHF => 2 XJWVT",
                    "165 ORE => 2 GPVTF",
                    "3 DCFZ, 7 NZVS, 5 HKGWZ, 10 PSHF => 8 KHKGT"
                };

                var reactions = Parse(s);
                var goal = new Chemical { Quantity = 1, Label = "FUEL" };
                var store = new Dictionary<string, long>();
                var oreRequired = Produce(reactions, goal, store, 0);
                Debug.Assert(oreRequired == 13312);
            }
            {
                var s = new string[]
                {
                    "2 VPVL, 7 FWMGM, 2 CXFTF, 11 MNCFX => 1 STKFG",
                    "17 NVRVD, 3 JNWZP => 8 VPVL",
                    "53 STKFG, 6 MNCFX, 46 VJHF, 81 HVMC, 68 CXFTF, 25 GNMV => 1 FUEL",
                    "22 VJHF, 37 MNCFX => 5 FWMGM",
                    "139 ORE => 4 NVRVD",
                    "144 ORE => 7 JNWZP",
                    "5 MNCFX, 7 RFSQX, 2 FWMGM, 2 VPVL, 19 CXFTF => 3 HVMC",
                    "5 VJHF, 7 MNCFX, 9 VPVL, 37 CXFTF => 6 GNMV",
                    "145 ORE => 6 MNCFX",
                    "1 NVRVD => 8 CXFTF",
                    "1 VJHF, 6 MNCFX => 4 RFSQX",
                    "176 ORE => 6 VJHF"
                };

                var reactions = Parse(s);
                var goal = new Chemical { Quantity = 1, Label = "FUEL" };
                var store = new Dictionary<string, long>();
                var oreRequired = Produce(reactions, goal, store, 0);
                Debug.Assert(oreRequired == 180697);
            } 
            {
                var s = new string[]
                {
                    "171 ORE => 8 CNZTR",
                    "7 ZLQW, 3 BMBT, 9 XCVML, 26 XMNCP, 1 WPTQ, 2 MZWV, 1 RJRHP => 4 PLWSL",
                    "114 ORE => 4 BHXH",
                    "14 VRPVC => 6 BMBT",
                    "6 BHXH, 18 KTJDG, 12 WPTQ, 7 PLWSL, 31 FHTLT, 37 ZDVW => 1 FUEL",
                    "6 WPTQ, 2 BMBT, 8 ZLQW, 18 KTJDG, 1 XMNCP, 6 MZWV, 1 RJRHP => 6 FHTLT",
                    "15 XDBXC, 2 LTCX, 1 VRPVC => 6 ZLQW",
                    "13 WPTQ, 10 LTCX, 3 RJRHP, 14 XMNCP, 2 MZWV, 1 ZLQW => 1 ZDVW",
                    "5 BMBT => 4 WPTQ",
                    "189 ORE => 9 KTJDG",
                    "1 MZWV, 17 XDBXC, 3 XCVML => 2 XMNCP",
                    "12 VRPVC, 27 CNZTR => 2 XDBXC",
                    "15 KTJDG, 12 BHXH => 5 XCVML",
                    "3 BHXH, 2 VRPVC => 7 MZWV",
                    "121 ORE => 7 VRPVC",
                    "7 XCVML => 6 RJRHP",
                    "5 BHXH, 4 VRPVC => 5 LTCX"
                };

                var reactions = Parse(s);
                var goal = new Chemical { Quantity = 1, Label = "FUEL" };
                var store = new Dictionary<string, long>();
                var oreRequired = Produce(reactions, goal, store, 0);
                Debug.Assert(oreRequired == 2210736);
            }                                      
        }

        static List<Reaction> Parse(string[] reactions)
        {
            var r = new List<Reaction>();
            foreach (var s in reactions)
            {
                var ab = s.Split("=>");
                var left = ab[0].Split(',');
                var left1 = new List<Chemical>();
                foreach (var x in ab[0].Trim().Split(','))
                {
                    var quantityUnit = x.Trim().Split(' ');
                    left1.Add(new Chemical { Quantity = long.Parse(quantityUnit[0]), Label = quantityUnit[1] });
                }
                var right = ab[1].Trim().Split(' ');

                r.Add(
                    new Reaction 
                    {
                        Left = left1.ToArray(), 
                        Right = new Chemical { Quantity = long.Parse(right[0]), Label = right[1] }
                    });
            }

            return r;
        }

        static void UpdateStore(List<Reaction> reactions, Chemical c, long unitToProduce, Dictionary<string, long> store)
        {
            var reaction = reactions.Single(r1 => r1.Right.Label == c.Label);
            var needed = unitToProduce;
            var count = 0;
            if (needed > 0)
            {
                do
                {
                    count++;
                }
                while (count * reaction.Right.Quantity < needed);
            }

            var produced = count * reaction.Right.Quantity; 
            var toAdd = produced - needed;
            
            if (toAdd > 0)
            {
                if (store.ContainsKey(c.Label))
                    store[c.Label] += (long)toAdd;
                else
                    store[c.Label] = (long)toAdd;
            }            
        }

        static long Produce(List<Reaction> reactions, Chemical goal, Dictionary<string, long> store, int level)
        {                    
            var reaction = reactions.Single(r => r.Right.Label == goal.Label);
            var multiple = (long)Math.Ceiling((double)goal.Quantity / reaction.Right.Quantity);

            // Base
            if (reaction.Left.Length == 1 && reaction.Left[0].Label == "ORE")
            {
                //Console.WriteLine($"{new string(' ', level)}{goal.Quantity} {goal.Label} <= {reaction.Left[0].Quantity} {reaction.Left[0].Label} ({multiple}x)");
                return reaction.Left[0].Quantity * multiple;
            }

            // Induction
            var ore = 0L;
            foreach (var r in reaction.Left)
            {                
                //Console.WriteLine($"{new string(' ', level)}{goal.Quantity} {goal.Label} <= {r.Quantity} {r.Label} ({multiple}x)");        
                var toProduce = r.Quantity * multiple;
                if (store.ContainsKey(r.Label) && store[r.Label] > 0)
                {
                    var x = Math.Min(store[r.Label], toProduce);
                    toProduce -= x;
                    store[r.Label] -= x;
                }

                ore += Produce(reactions, new Chemical { Quantity = toProduce, Label = r.Label }, store, level + 2);                    
                UpdateStore(reactions, r, toProduce, store);
            }

            // foreach (var s in store)
            //     Console.WriteLine($"{new string(' ', level)}Store: {s.Key} {s.Value}");                

            return ore;
        }

        static void Part1()
        {
            var reactions = Parse(File.ReadAllLines("./Input.txt"));
            var goal = new Chemical { Quantity = 1, Label = "FUEL" };
            var store = new Dictionary<string, long>();
            var oreRequired = Produce(reactions, goal, store, 0);
            Debug.Assert(oreRequired == 365768);
        }

        static void Part2()
        {
            var reactions = Parse(File.ReadAllLines("./Input.txt"));
            var goal = new Chemical { Quantity = 1_000_000_000_000, Label = "ORE" };
            var lower = 3700000;
            var upper = 3800000;

            var fuel = 0;
            while (true)
            {            
                var store = new Dictionary<string, long>();
                fuel = (lower + upper) / 2;
                var oreRequired = Produce(reactions, new Chemical { Quantity = fuel, Label = "FUEL" }, store, 0);
                //Console.WriteLine($"{lower} {upper} {fuel} {upper - lower} {oreRequired}");

                // Because fuel is a whole number we'll likely not end up
                // exactly at goal.Quantity. To verify that we're close enough,
                // we calculate fuel + 1.
                if (oreRequired == goal.Quantity || 
                   (oreRequired < goal.Quantity && Produce(reactions, new Chemical { Quantity = fuel + 1, Label = "FUEL" }, store, 0) > goal.Quantity))
                {
                    System.Console.WriteLine(fuel);
                    break;
                }

                if (oreRequired > goal.Quantity)
                    upper = (upper - ((upper - lower) / 4));
                else if (oreRequired < goal.Quantity)
                    lower = lower + ((upper - lower) / 4);
            }

            Debug.Assert(fuel == 3756877);
        }
    }
}