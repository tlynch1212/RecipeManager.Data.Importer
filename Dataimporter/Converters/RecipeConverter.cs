using Dataimporter.Models;
using Dataimporter.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Dataimporter.Converters
{
    public static class RecipeConverter
    {
        public static void ProcessRecipes(string path)
        {
            Console.WriteLine("trying path: " + path);
            var tempPath = path;
            if (path.StartsWith("http"))
            {
                tempPath = @".\RAW_recipes.csv";
                var wc = new System.Net.WebClient();
                wc.DownloadFile(path, tempPath);
            }

            if (File.Exists(tempPath))
            {
                Console.WriteLine("File found. Proccessing Recipes...");
                var batch = 0;
                var totalCount = 0;
                using (var db = new DataContext())
                {
                    var Lines = File.ReadLines(tempPath).Select(a => a);
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

                            var row = CsvUtils.SplitCSV(recipeString);
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

        public static Recipe ConvertToRecipe(IEnumerable<string> data)
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

        public static List<Instruction> ConvertToInstructions(string rawData)
        {
            List<Instruction> convertedInstructions = new List<Instruction>();
            var instructions = CsvUtils.SplitSingleQuotes(rawData);
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

        public static List<Ingredient> ConvertToIngredients(string rawData)
        {
            List<Ingredient> convertedIngredient = new List<Ingredient>();
            var ingredients = CsvUtils.SplitSingleQuotes(rawData);
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
