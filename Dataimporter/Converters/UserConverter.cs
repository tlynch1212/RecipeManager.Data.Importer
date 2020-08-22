using Dataimporter.Models;
using Dataimporter.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dataimporter.Converters
{
    public static class UserConverter
    {
        public static void ProcessUsers(string path)
        {
            Console.WriteLine("trying path: " + path);
            var tempPath = path;
            if (path.StartsWith("http"))
            {
                tempPath = @".\RAW_users.csv";
                var wc = new System.Net.WebClient();
                wc.DownloadFile(path, tempPath);
            }

            if (File.Exists(tempPath))
            {
                Console.WriteLine("File found. Proccessing Users...");
                var batch = 0;
                var totalCount = 0;
                using (var db = new DataContext())
                {
                    var lines = File.ReadLines(tempPath).Select(a => a);
                    foreach (var rowString in lines.Skip(1))
                    {
                        var row = CsvUtils.SplitCsv(rowString);

                        if (db.Users.FirstOrDefault(u => u.Id == int.Parse(row[0])) == null)
                        {
                            var user = ConvertToUser(row);
                            db.Users.Add(user);
                            batch += 1;
                            if (batch == 1000)
                            {
                                db.SaveChanges();
                                totalCount += 1000;
                                Console.WriteLine($"Added - {batch} Users");
                                batch = 0;
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

        public static User ConvertToUser(IEnumerable<string> data)
        {
            var ratingData = data.ToArray();
            var userId = ratingData[0];

            return new User()
            {
                Id = int.Parse(userId),
                AuthId = userId
            };
        }
    }
}
