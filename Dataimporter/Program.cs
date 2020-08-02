using Dataimporter.Converters;
using Dataimporter.Models;
using Dataimporter.Utils;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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
            else
            {
                Console.WriteLine($"Processing type {processType}, not valid.");
            }
        }
    }
}
