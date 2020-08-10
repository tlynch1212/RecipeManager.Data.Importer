using Dataimporter.Converters;
using System;

namespace Dataimporter
{
    class Program
    {
        static void Main(string[] args)
        {
            string processType = args[0];
            string path = args[1];

            if (processType.Equals("recipes"))
            {
                RecipeConverter.ProcessRecipes(path);
            }
            else if (processType.Equals("ratings"))
            {
                RatingConverter.ProcessRatings(path);
            }
            else if (processType.Equals("users"))
            {
                UserConverter.ProcessUsers(path);
            }
            else
            {
                Console.WriteLine($"Processing type {processType}, not valid.");
            }
        }
    }
}
