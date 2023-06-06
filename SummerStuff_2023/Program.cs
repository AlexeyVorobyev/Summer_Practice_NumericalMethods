using System;
using static System.Math;

namespace RectangleIntegral
{
    class Program
    {
        private static double Function(double x)
        {
            return Pow(x,3)/3 + Pow(x,4);
        }
        static double Calculate(double a, double b, double e)
        {
            double delta = b - a;
            double amountRectangles = 2;
            double result = 0, prevResult = 0;
            double step = delta / amountRectangles;
            do
            {
                prevResult = result;
                result = 0;
                for (double i = a; i < b - step/2; i += step)
                {
                    result += Function(i) * step;
                }
                amountRectangles++;
                step = delta / amountRectangles;
                Console.WriteLine(amountRectangles);
                Console.WriteLine(result);
            } while (Abs(result - prevResult) >= e);
            return result;
        }
        static void Main() {
            double a = 5;
            double b = 10;
            double e = 0.001;
            Console.WriteLine(Calculate(a, b, e));
        }
    }
}