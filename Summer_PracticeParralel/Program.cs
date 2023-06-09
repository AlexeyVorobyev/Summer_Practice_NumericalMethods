using System;
using static System.Math;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Diagnostics.Metrics;

namespace Summer_Practice
{
    class Calc
    {
        Func<double, double>? scriptDeleg;
        public double amountRectangles;
        double a, b, e, x0;
        int numOfThreads;

        void ParallelTask(double x,ref double result,int num,ref int counter)
        {
            double delta = x - this.a;
            double step = delta / this.amountRectangles;
            for (double i = this.a + step*Convert.ToInt32(this.amountRectangles * (num/this.numOfThreads) ); i < this.a + step * Convert.ToInt32(this.amountRectangles * ((num + 1) / this.numOfThreads)) - step / 2; i += step)
            {
                result += this.scriptDeleg(i + step / 2) * step;
                counter++;
            }
        }

        public Calc(string functionString, double a, double b, double e, double x0, int numOfThreads)
        {
            this.amountRectangles = 100;
            this.a = a;
            this.b = b;
            this.e = e;
            this.x0 = x0;
            this.numOfThreads = numOfThreads;
            initFunction(functionString);

            double delta = b - a;
            double result = 0, prevResult = 0;
            double step = delta / this.amountRectangles;
            do
            {
                prevResult = result;
                //result = 0;
                //for (double i = a; i < b - step / 2; i += step)
                //{
                //    result += this.scriptDeleg(i + step / 2) * step;
                //}

                double[] resMas = new double[this.numOfThreads];
                int[] counterMas = new int[this.numOfThreads];
                //Task t1 = Task.Run(() => ParallelTask(x, ref resMas[1],1,ref counterMas[1]));
                Task[] tasks = new Task[this.numOfThreads - 1];

                for (int i = 0; i < this.numOfThreads - 1; i++)
                {
                    int newI = 1 + i;
                    tasks[i] = new Task(() => ParallelTask(b, ref resMas[newI], newI, ref counterMas[newI]));
                }

                for (int i = 0; i < this.numOfThreads - 1; i++) tasks[i].Start();

                for (double i = this.a; i < this.a + step * Convert.ToInt32(this.amountRectangles * (1 / this.numOfThreads)) - step / 2; i += step)
                {
                    resMas[0] += this.scriptDeleg(i + step / 2) * step;
                    counterMas[0]++;
                }
                if (this.numOfThreads > 1) Task.WaitAll(tasks);

                result = 0;
                for (int i = 0; i < resMas.Length; i++) result += resMas[i];

                this.amountRectangles += 100;
                step = delta / this.amountRectangles;
                Console.WriteLine(this.amountRectangles);
                Console.WriteLine(Abs(result - prevResult));
                if (this.amountRectangles > 20000) break;
            } while (Abs(result - prevResult) >= e);
            this.numOfThreads = numOfThreads;
        }

        async private void initFunction(string functionString)
        {
            var options = ScriptOptions.Default.AddImports("System", "System.Math");
            this.scriptDeleg = await CSharpScript.EvaluateAsync<Func<double, double>>("x => " + functionString, options);
        }

        public double CalculateIntegralSquare(double x)
        {
            double delta = x - this.a;
            double result = 0;
            int counter = 0;
            double step = delta / this.amountRectangles;
            double[] resMas = new double[this.numOfThreads];
            int[] counterMas = new int[this.numOfThreads];
            //Task t1 = Task.Run(() => ParallelTask(x, ref resMas[1],1,ref counterMas[1]));
            Task[] tasks = new Task[this.numOfThreads - 1];

            for (int i = 0; i < this.numOfThreads - 1; i++)
            {
                Console.WriteLine(i);
                Console.WriteLine(tasks.Length);
                Console.WriteLine(tasks[i]);
                Console.WriteLine(resMas.Length);
                Console.WriteLine(resMas[i+1]);
                Console.WriteLine(counterMas.Length);
                Console.WriteLine(counterMas[i+1]);
                int newI = 1 + i;
                tasks[i] = new Task(() => ParallelTask(x, ref resMas[newI], newI, ref counterMas[newI]));
            }

            for (int i = 0; i < this.numOfThreads - 1; i++) tasks[i].Start();

            for (double i = this.a; i < this.a + step * Convert.ToInt32(this.amountRectangles * (1/this.numOfThreads)) - step / 2; i += step)
            {
                resMas[0] += this.scriptDeleg(i + step / 2) * step;
                counterMas[0]++;
            }
            if (this.numOfThreads > 1) Task.WaitAll(tasks);


            for (int i = 0; i < resMas.Length; i++) result += resMas[i];
            for (int i = 0; i < counterMas.Length; i++) counter += counterMas[i];
            Console.WriteLine(counter);
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
            double x = this.x0, xNext;
            double df;
            if (CalculateIntegralSquare(x) < this.b) df = (CalculateIntegralSquare(x) - CalculateIntegralSquare(x - this.e)) / (this.e);
            else df = (CalculateIntegralSquare(x) - CalculateIntegralSquare(x + this.e)) / (this.e);
            do
            {
                xNext = x - (CalculateIntegralSquare(x) - this.b) / df;
                df = (CalculateIntegralSquare(xNext) - CalculateIntegralSquare(x)) / (xNext - x);
                if (Abs(xNext - x) < this.e) break;
                x = xNext;
                //Console.WriteLine(x);
            } while (true);
            return x;
        }
    }

    class Program
    {
        static void Main()
        {
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
                    Calc calc = new Calc(functionString, a, b, e, x0,6);
                    Console.WriteLine("here");
                    //Console.WriteLine(calc.CalculateEquality());
                    Console.WriteLine(calc.CalculateEquality2());
                    Console.WriteLine(calc.amountRectangles);
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