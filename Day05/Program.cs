using System;
using System.IO;
using System.Diagnostics;

namespace Day05
{
    class Program
    {
        static void Main(string[] args)
        {
            var x = 12345;
            var x0 = x % 10;
            var x1 = (x / 10) % 10;
            var x2 = (x / 100) % 10;
            var x3 = (x / 1000) % 10;
            var x4 = (x / 10000) % 10;

            var y0 = Digit(x, 1);
            var y1 = Digit(x, 2);
            var y2 = Digit(x, 3);
            var y3 = Digit(x, 4);
            var y4 = Digit(x, 5);

            Test();
            Part1();
            //Part2();
        }       

        static void Test()
        {
            var m1 = LoadMemory("1002,4,3,4,33");
            var m2 = LoadMemory("1002,4,3,4,99");
            Execute(m1);

            Debug.Assert(m1.Length == m2.Length);
            for (var i = 0; i < m1.Length; i++)
                Debug.Assert(m1[i] == m2[i]);
        }

        static int[] LoadMemory(string s)
        {
            var strings = s.Split(',');
            var memory = new int[strings.Length];

            for (var i = 0; i < strings.Length; i++)
                memory[i] = int.Parse(strings[i]);

            return memory;
        }

        static void Part1()
        {
            var input = File.ReadAllText("Input.txt");
            var memory = LoadMemory(input);
            Execute(memory);                        
            // Output: 3000000006731945 with 6731945 being the diagnosics code.            
        }

        static void Part2()
        {
            var input = File.ReadAllText("Input.txt");
            for (var noun = 0; noun <= 99; noun++)
            {
                for (var verb = 0; verb <= 99; verb++)
                {
                    var memory = LoadMemory(input);
                    memory[1] = noun;
                    memory[2] = verb;  
                    Execute(memory);
                    if (memory[0] == 19690720)
                    {
                        Debug.Assert(7014 == 100 * noun + verb);
                        break;
                    }

                }
            }
        }

        enum Opcode
        {
            Add = 1,
            Mul,
            Input,
            Output,
            Halt = 99
        }

        enum ParameterMode
        {
            Position = 0,
            Immediate
        }

        static int Digit(int instruction, int position) => (instruction / (int)Math.Pow(10, position - 1)) % 10;

        static ParameterMode ToParameterMode(int instruction, int position)
        {
            return Digit(instruction, position) == 0 
                ? ParameterMode.Position 
                : ParameterMode.Immediate;
        }

        static void Execute(int[] memory)
        {
            int Resolve(int instruction, int position, int address)
            {
                return ToParameterMode(instruction, position) switch
                    {
                        ParameterMode.Position => memory[address],
                        ParameterMode.Immediate => address,
                        _ => throw new Exception($"Unsupported {nameof(ParameterMode)}: {ToParameterMode(instruction, position)}")
                    };
            }

            var ip = 0;
            while (ip < memory.Length)
            {
                var instruction = memory[ip];
                var opcode = (Opcode)(Digit(instruction, 1) + Digit(instruction, 2) * 10);
                switch (opcode)
                {
                    case Opcode.Add:
                    {
                        var input1 = memory[ip + 1];
                        var input2 = memory[ip + 2];
                        var output = memory[ip + 3];
                        var arg1 = Resolve(instruction, 3, input1);
                        var arg2 = Resolve(instruction, 4, input2);
                        Debug.Assert(ToParameterMode(instruction, 5) == 0);
                        memory[output] = arg1 + arg2;
                        ip += 4;
                        break;
                    }
                    case Opcode.Mul:
                    {
                        var input1 = memory[ip + 1];
                        var input2 = memory[ip + 2];
                        var output = memory[ip + 3];
                        var arg1 = Resolve(instruction, 3, input1);
                        var arg2 = Resolve(instruction, 4, input2);
                        Debug.Assert(ToParameterMode(instruction, 5) == 0);
                        memory[output] = arg1 * arg2;
                        ip += 4;
                        break;
                    }
                    case Opcode.Input:
                    {
                        var outputAddress = memory[ip + 1];
                        var input = Console.ReadLine();
                        memory[outputAddress] = int.Parse(input);
                        ip += 2;
                        break;
                    }
                    case Opcode.Output:
                    {
                        var inputAddress = memory[ip + 1];
                        Console.Write(memory[inputAddress]);
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