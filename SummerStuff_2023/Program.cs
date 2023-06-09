using System;
using static System.Math;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Summer_Practice
{
    class Calc
    {
        Func<double,double>? scriptDeleg;
        private double amountRectangles;
        double a, b, e,x0;

        public Calc(string functionString, double a, double b, double e,double x0)
        {
            this.amountRectangles = 100;
            this.a = a;
            this.b = b;
            this.e = e;
            this.x0 = x0;
            initFunction(functionString);

            double delta = b - a;
            double result = 0, prevResult = 0;
            double step = delta / this.amountRectangles;
            do
            {
                prevResult = result;
                result = 0;
                for (double i = a; i < b - step / 2; i += step)
                {
                    result += this.scriptDeleg(i + step/2) * step;
                }
                this.amountRectangles+=100;
                step = delta / this.amountRectangles;
                Console.WriteLine(this.amountRectangles);
                Console.WriteLine(Abs(result - prevResult));
            } while (Abs(result - prevResult) >= e);
        }

        async private void initFunction(string functionString)
        {
            var options = ScriptOptions.Default.AddImports("System", "System.Math");
            this.scriptDeleg = await CSharpScript.EvaluateAsync<Func<double, double>>("x => " + functionString,options);
        }

        public double CalculateIntegralSquare(double x)
        {
            double delta = x - this.a;
            double result = 0;
            double step = delta / this.amountRectangles;
            for (double i = this.a; i < x - step / 2; i += step)
            {
                result += this.scriptDeleg(i + step/2) * step;
                //counter++;
            }
            return result;
        }

        public double CalculateEquality()
        {
            double x = this.x0;
            double h = (CalculateIntegralSquare(x) - this.b) / this.scriptDeleg(x);
            while (Abs(h) >= this.e)
            {
                x -= h;
                h = (CalculateIntegralSquare(x) - this.b) / this.scriptDeleg(x);
                //Console.WriteLine(h);
                //Console.WriteLine("------");
                //Console.WriteLine(x);
            }
            return x;
        }

        public double CalculateEquality2()
        {
            double x = this.x0,xNext;
            double df;
            if (CalculateIntegralSquare(x) < this.b) df = (CalculateIntegralSquare(x) - CalculateIntegralSquare(x - this.e)) / (this.e);
            else df = (CalculateIntegralSquare(x) - CalculateIntegralSquare(x + this.e)) / (this.e);
            do 
            {
                xNext = x - (CalculateIntegralSquare(x) - this.b) / df;
                df = (CalculateIntegralSquare(xNext) - CalculateIntegralSquare(x)) / (xNext - x);
                if (Abs(xNext - x) < this.e) break;
                x = xNext;
                Console.WriteLine(x);
            } while (true);
            return x;
        }
    }

    class Program {
        static void Main() {
            double a;
            double b;
            double e;
            double x0;
            string functionString;

            bool flag = true;
            while (flag)
            {
                try
                {
                    Console.WriteLine("Введите функцию:");
                    functionString = Console.ReadLine();
                    Console.WriteLine("Введите a");
                    a = Convert.ToDouble(Console.ReadLine());
                    Console.WriteLine("Введите b:");
                    b = Convert.ToDouble(Console.ReadLine());
                    Console.WriteLine("Введите e:");
                    e = Convert.ToDouble(Console.ReadLine());
                    Console.WriteLine("Введите x0:");
                    x0 = Convert.ToDouble(Console.ReadLine());
                    if (x0 <= a) throw new Exception("x0 cannot be lower or equal a");
                    Calc calc = new Calc(functionString, a, b, e, x0);
                    Console.WriteLine("here");
                    Console.WriteLine(calc.CalculateEquality());
                    Console.WriteLine(calc.CalculateEquality2());
                    flag = false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }
}