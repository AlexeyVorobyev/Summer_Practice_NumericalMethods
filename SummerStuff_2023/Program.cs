using System;
using static System.Math;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace Summer_Practice
{
    class Calc
    {
        Func<double,double>? scriptDeleg;
        private double amountRectangles;
        double a, b, e,x0;

        public Calc(string functionString, double a, double b, double e,double x0)
        {
            this.amountRectangles = 2;
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
                    result += this.scriptDeleg(i) * step;
                }
                this.amountRectangles++;
                step = delta / this.amountRectangles;
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
                result += this.scriptDeleg(i) * step;
            }
            return result;
        }

        public double CalculateEquality()
        {
            double x = this.x0;
            double h = (CalculateIntegralSquare(x) - this.b) / this.scriptDeleg(x);
            double resFunc = CalculateIntegralSquare(x);
            while (Abs(h) >= this.e)
            {
                x -= h;
                h = (CalculateIntegralSquare(x) - this.b) / this.scriptDeleg(x);
            }
            return x;
        }
    }

    class Program {
        static void Main() {
            double a;
            double b;
            double e;

            Console.WriteLine("Введите функцию:");
            // string functionString = Console.ReadLine();
            string functionString = "x";
            Calc calc = new Calc(functionString,a,b,e,x0);
            Console.WriteLine(calc.CalculateEquality());
        }
    }
}