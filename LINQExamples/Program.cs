using LINQExamples.Lib.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LINQExamples
{
    class Program
    {
        static void Main(string[] args)
        {
            char input = 'b';
            while (input != 'q')
            {
                Console.Write("Select a test:");
                var testSelection = Console.ReadLine().ToLower().First();
                switch (testSelection)
                {
                    case 'b':
                        TestManager.BasicsTest();
                        break;

                    case 'e':
                        TestManager.EntitiesBasicsTest();
                        break;

                    case 's':
                        TestManager.SelectManyTest();
                        break;

                    case 'a':
                        Console.Write("Input: $");
                        var aggregateInput = int.Parse(Console.ReadLine());
                        TestManager.AggregatesTest(aggregateInput);
                        break;

                    case 'c':
                        TestManager.CollectionsTest();
                        break;

                    case 'j':
                        TestManager.JoinTest();
                        break;

                    case 'g':
                        TestManager.GroupingTest();
                        break;

                    case 't':
                        TestManager.SetTest();
                        break;

                    case 'r':
                        TestManager.ErrorTest();
                        break;

                    case 'q':
                        return;
                }

                Console.WriteLine(Environment.NewLine);
            }
        }
    }
}
