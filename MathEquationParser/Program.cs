using System;
using System.Text.RegularExpressions;

namespace Parser
{
    public class Node
    {
        Node? left;
        Node? right;
        string value;

        public Node(string value, ref Dictionary<char, double> vars)
        {      
            this.left = null;
            this.right = null;
            calculateNode(value, ref vars);
            Console.WriteLine(this.value);
        }

        public void calculateNode(string input, ref Dictionary<char, double> vars)
        {
            Regex regexFunctionDual = new Regex(@"(?<!(\[|\()[a-z0-9]+)(\+|\*|\-|\/|\^)(?![a-z0-9]+(\]|\)))");
            Regex regexFunctionMono = new Regex(@"([a-zA-Z]+\[.+\])");
            if (regexFunctionDual.IsMatch(input))
            {
                Match matchInfo = regexFunctionDual.Match(input);
                //Console.WriteLine(matchInfo.Value);
                this.value = matchInfo.Value;
                //Console.WriteLine(matchInfo.Index);
                //Console.WriteLine(input.Substring(0, matchInfo.Index));
                string toCheck1 = input.Substring(0, matchInfo.Index), toCheck2 = input.Substring(matchInfo.Index + 1);
                //Console.WriteLine(toCheck1);
                //Console.WriteLine(toCheck2);
                Regex brackets = new Regex(@"\(.+\)");
                if (brackets.IsMatch(toCheck1))
                {
                    //Console.WriteLine(toCheck1.Substring(1, toCheck1.Length - 2));
                    this.left = new Node(toCheck1.Substring(1, toCheck1.Length - 2), ref vars);
                }
                else
                {
                    //Console.WriteLine(toCheck1);
                    this.left = new Node(toCheck1, ref vars);
                }
                //Console.WriteLine(input.Substring(matchInfo.Index + 1));
                if (brackets.IsMatch(toCheck2))
                {
                    //Console.WriteLine(toCheck2.Substring(1, toCheck2.Length - 2));
                    this.right = new Node(toCheck2.Substring(1, toCheck2.Length - 2), ref vars);
                }
                else
                {
                    //Console.WriteLine(toCheck2);
                    this.right = new Node(toCheck2, ref vars);
                }
            }
            else if (regexFunctionMono.IsMatch(input))
            {
                //Match matchInfo = regexFunctionMono.Match(input);
                //Console.WriteLine(matchInfo.Value);
                //Console.WriteLine(matchInfo.Length);
                Regex regexFunctionMonoDisasemble = new Regex(@"([a-zA-Z]+)");
                Match matchInfo2 = regexFunctionMonoDisasemble.Match(input);
                //Console.WriteLine(matchInfo2.Value);
                this.value = matchInfo2.Value;
                //Console.WriteLine(matchInfo2.Length);
                //Console.WriteLine(input.Substring(matchInfo2.Length + 1, input.Length - matchInfo2.Length - 2));
                this.left = new Node(input.Substring(matchInfo2.Length + 1, input.Length - matchInfo2.Length - 2),ref vars);
            }
            else
            {
                //Console.WriteLine(input);
                Regex isVar = new Regex(@"([a-zA-Z]+)");
                if (isVar.IsMatch(input)) if (!vars.ContainsKey(Convert.ToChar(input))) vars.Add(Convert.ToChar(input), 0);
                this.value = input;
            }
        }
    }

    public class Tree
    {
        Node root;
        Dictionary<char,double> vars = new Dictionary<char, double>();
        public Tree(string input)
        {
            root = new Node(input,ref vars);
            Console.WriteLine(vars.Count);
        }

    }

    class Program
    {
        static void Main(string[] args)
        {
            string input = "(sin[x+y]/cos[z^2])+4";
            Tree tree = new Tree(input);       
        }
    }
}
