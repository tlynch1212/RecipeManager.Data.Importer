using Dataimporter.Models;
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
            string path = args[0];
            Console.WriteLine("trying path: " + path);
            if (File.Exists(path))
            {
                Console.WriteLine("File found. Proccessing...");
                var batch = 0;
                var totalCount = 0;
                using (var db = new DataContext())
                {
                    var Lines = File.ReadLines(path).Select(a => a);
                    var leftOvers = "";
                    foreach (var rowString in Lines.Skip(1))
                    {
                        if (rowString.EndsWith("]\""))
                        {
                            var recipeString = "";
                            if (!string.IsNullOrEmpty(leftOvers))
                            {
                                recipeString = leftOvers + rowString;
                                leftOvers = "";
                            }
                            else
                            {
                                recipeString = rowString;
                            }

                            var row = SplitCSV(recipeString);
                            var recipe = ConvertToRecipe(row);
                            recipe.CreatedDate = DateTime.Now;
                            recipe.IsPublic = true;
                            if (db.Recipes.FirstOrDefault(r => r.Id == recipe.Id) == null)
                            {
                                db.Recipes.Add(recipe);
                                batch += 1;
                                if (batch == 1000)
                                {
                                    db.SaveChanges();
                                    totalCount += 1000;
                                    Console.WriteLine($"Added - {batch} Recipes");
                                    batch = 0;
                                }
                            }
                        }
                        else
                        {
                            leftOvers += rowString;
                        }
                    }
                    db.SaveChanges();
                    totalCount += batch;
                    Console.WriteLine($"Processed - Total: {totalCount}");

                }
            }
            else
            {
                Console.WriteLine("File not found.");
            }

            Console.WriteLine("Done.");
        }


        private static string[] SplitCSV(string line)
        {
            List<string> result = new List<string>();
            StringBuilder currentStr = new StringBuilder("");
            bool inQuotes = false;
            for (int i = 0; i < line.Length; i++) // For each character
            {
                if (line[i] == '\"') // Quotes are closing or opening
                    inQuotes = !inQuotes;
                else if (line[i] == ',') // Comma
                {
                    if (!inQuotes) // If not in quotes, end of current string, add it to result
                    {
                        result.Add(currentStr.ToString());
                        currentStr.Clear();
                    }
                    else
                        currentStr.Append(line[i]); // If in quotes, just add it 
                }
                else // Add any other character to current string
                    currentStr.Append(line[i]);
            }
            result.Add(currentStr.ToString());
            return result.ToArray(); // Return array of all strings
        }

        private static string[] SplitSingleQuotes(string line)
        {
            List<string> result = new List<string>();
            StringBuilder currentStr = new StringBuilder("");
            bool inQuotes = false;
            for (int i = 0; i < line.Length; i++) // For each character
            {
                if (line[i] == '\'') // Quotes are closing or opening
                    inQuotes = !inQuotes;
                else if (line[i] == ',') // Comma
                {
                    if (!inQuotes) // If not in quotes, end of current string, add it to result
                    {
                        result.Add(currentStr.ToString());
                        currentStr.Clear();
                    }
                    else
                        currentStr.Append(line[i]); // If in quotes, just add it 
                }
                else // Add any other character to current string
                    currentStr.Append(line[i]);
            }
            result.Add(currentStr.ToString());
            return result.ToArray(); // Return array of all strings
        }

        private static Recipe ConvertToRecipe(IEnumerable<string> data)
        {
            var recipeData = data.ToArray();
            var recipeName = recipeData[0];
            var recipeId = int.Parse(recipeData[1]);
            var recipeTimeToCook = int.Parse(recipeData[2]);
            var recipeSteps = ConvertToInstructions(recipeData[3]);
            var recipeDescription = recipeData[4];
            var recipeIngredients = ConvertToIngredients(recipeData[5]);


            return new Recipe()
            {
                Id = recipeId,
                Name = recipeName,
                Description = recipeDescription,
                TimeToCook = recipeTimeToCook,
                Instructions = recipeSteps,
                Ingredients = recipeIngredients
            };
        }

        private static List<Instruction> ConvertToInstructions(string rawData)
        {
            List<Instruction> convertedInstructions = new List<Instruction>();
            var instructions = SplitSingleQuotes(rawData);
            foreach (var instruction in instructions)
            {
                var cleanedData = instruction.Replace("[", "").Replace("]", "").Trim();
                convertedInstructions.Add(new Instruction
                {
                    Value = cleanedData
                });
            }
            return convertedInstructions;
        }

        private static List<Ingredient> ConvertToIngredients(string rawData)
        {
            List<Ingredient> convertedIngredient = new List<Ingredient>();
            var ingredients = SplitSingleQuotes(rawData);
            foreach (var ingredient in ingredients)
            {
                var cleanedData = ingredient.Replace("[", "").Replace("]", "").Trim();
                convertedIngredient.Add(new Ingredient
                {
                    Value = cleanedData
                });
            }
            return convertedIngredient;
        }
    }
}
