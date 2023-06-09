//using System;
//using static System.Math;
//using Microsoft.CodeAnalysis.CSharp.Scripting;
//using Microsoft.CodeAnalysis.Scripting;
//using Microsoft.CodeAnalysis.CSharp.Syntax;
#include <iostream>
#include <math.h>
using namespace std;


class Calc
{
    Func<double, double> ? scriptDeleg;
    double amountRectangles;
    double a, b, e, x0;

    Calc(string functionString, double a, double b, double e, double x0)
    {
        this->amountRectangles = 100;
        this->a = a;
        this->b = b;
        this->e = e;
        this->x0 = x0;
        initFunction(functionString);

        double delta = b - a;
        double result = 0, prevResult = 0;
        double step = delta / this->amountRectangles;
        do
        {
            prevResult = result;
            result = 0;
            for (double i = a; i < b - step / 2; i += step)
            {
                result += this->scriptDeleg(i + step / 2) * step;
            }
            this->amountRectangles += 100;
            step = delta / this->amountRectangles;
        } while (abs(result - prevResult) >= e);
    }

    double Function(double x)
    {
        var options = ScriptOptions.Default.AddImports("System", "System.Math");
        this.scriptDeleg = await CSharpScript.EvaluateAsync<Func<double, double>>("x => " + functionString, options);
    }

    double CalculateIntegralSquare(double x)
    {
        double delta = x - this.a;
        double result = 0;
        double step = delta / this.amountRectangles;
        for (double i = this.a; i < x - step / 2; i += step)
        {
            result += this.scriptDeleg(i + step / 2) * step;
            counter++;
        }
        return result;
    }

    double CalculateEquality()
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

    double CalculateEquality2()
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
            Console.WriteLine(x);
        } while (true);
        return x;
    }
};

int main() {
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
            cout << "Введите функцию:" << endl;
            cin>>functionString;
            cout << "Введите a" << endl;
            cin >> a;
            cout << "Введите b:" << endl;
            cin >> b;
            cout << "Введите e:" << endl;
            cin >> e;
            cout << "Введите x0:" << endl;
            cin >> x0;
            if (x0 <= a) throw new Exception("x0 cannot be lower or equal a");
            Calc calc = new Calc(functionString, a, b, e, x0);
            cout << "here" << endl;
            cout << calc.CalculateEquality() << endl;
            cout << calc.CalculateEquality2() << endl;
            flag = false;
        }
        catch (Exception ex)
        {
            cout<< ex.ToString() << endl;
        }
    }
}