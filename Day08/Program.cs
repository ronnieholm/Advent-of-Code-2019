using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Day08
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
            var s = "123456789012";
            var l = ToLayers(s, 3, 2);
            Debug.Assert(l[0] == "123456");
            Debug.Assert(l[1] == "789012");
        }

        static void Part1()
        {
            var imageData = File.ReadAllText("Input.txt");
            var layers = ToLayers(imageData, 25, 6);

            var fewestZerosLayer = 0;
            var zerosInLayer = Int32.MaxValue;
            for (var l = 0; l < layers.Count; l++)
            {
                var zeroCount = CountCharacter(layers[l], '0');
                if (zeroCount < zerosInLayer)
                {
                    fewestZerosLayer = l;
                    zerosInLayer = zeroCount;
                }
            }
            
            var check = CountCharacter(layers[fewestZerosLayer], '1') * CountCharacter(layers[fewestZerosLayer], '2');
            Debug.Assert(check == 1690);
        }

        static List<string> ToLayers(string imageData, int width, int height)
        {
            var size = width * height;
            var layers = new List<string>();
            for (var i = 0; i < imageData.Length; i += size)
            {
                var layer = imageData.Substring(i, size);
                layers.Add(layer);
            }

            return layers;
        }

        static int CountCharacter(string s, char character)
        {
            var count = 0;
            foreach (var c in s)
                if (c == character)
                    count++;
            return count;
        }

        static void Part2()
        {
            const int Width = 25;
            const int Height = 6;
            var imageData = File.ReadAllText("Input.txt");
            var layers = ToLayers(imageData, Width, Height);

            var i = 0;
            var image = new char[Width * Height];
            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    var offset = y * Width + x;
                    for (var l = 0; l < layers.Count; l++)
                    {
                        var pixel = layers[l][offset];
                        if (pixel == '2')
                            continue;
                        else if (pixel == '0')
                        {
                            image[i++] = ' ';
                            Console.Write(" ");                            
                            break;
                        }
                        else if (pixel == '1')
                        {
                            image[i++] = '*';
                            Console.Write("*");
                            break;
                        }
                        
                        if (l == layers.Count - 1)
                        {
                            image[i++] = '.';
                            Console.Write(pixel);
                        }
                    }
                }
                Console.WriteLine();
            }


                var expected = 
                    "**** ***  **** *  * ***  " +
                    "   * *  *    * *  * *  * " +
                    "  *  *  *   *  *  * ***  " +
                    " *   ***   *   *  * *  * " +
                    "*    *    *    *  * *  * " +
                    "**** *    ****  **  ***  ";

                for (i = 0; i < image.Length; i++)
                    Debug.Assert(image[i] == expected[i]);
        }
    }
}
