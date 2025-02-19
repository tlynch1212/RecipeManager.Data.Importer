﻿using Dataimporter.Models;
using Dataimporter.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Dataimporter.Converters
{
    public static class RatingConverter
    {
        public static void ProcessRatings(string path)
        {
            Console.WriteLine("trying path: " + path);
            var tempPath = path;
            if (path.StartsWith("http"))
            {
                tempPath = @".\RAW_ratings.csv";
                var wc = new System.Net.WebClient();
                wc.DownloadFile(path, tempPath);
            }

            if (File.Exists(tempPath))
            {
                Console.WriteLine("File found. Proccessing Ratings...");
                var batch = 0;
                var totalCount = 0;
                using (var db = new DataContext())
                {
                    var lines = File.ReadLines(tempPath).Select(a => a);
                    foreach (var rowString in lines.Skip(1))
                    {
                        var row = CsvUtils.SplitCsv(rowString);
                        var recipe = db.Recipes.FirstOrDefault(r => r.Id == int.Parse(row[1]));
                        if (recipe != null)
                        {
                            var rating = ConvertToRating(row, int.Parse(row[1]));
                            if (db.Ratings.AsNoTracking().FirstOrDefault(r => r.UserId == rating.UserId && rating.RecipeId == r.RecipeId) == null)
                            {
                                db.Ratings.Add(rating);
                                batch += 1;
                                if (batch == 1000)
                                {
                                    db.SaveChanges();
                                    totalCount += 1000;
                                    Console.WriteLine($"Added - {batch} Ratings");
                                    batch = 0;
                                }
                            }
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

        public static Rating ConvertToRating(IEnumerable<string> data, int recipeId)
        {
            var ratingData = data.ToArray();
            var userId = ratingData[0];
            var rating = int.Parse(ratingData[2]);

            return new Rating()
            {
                UserId = userId,
                Rate = rating,
                RecipeId = recipeId
            };
        }
    }
}
