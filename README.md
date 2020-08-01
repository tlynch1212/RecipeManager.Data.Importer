# RecipeManager.Data.Importer
This is a tool to import recipes and/or ratings into a postgres database. This supports the RecipeManager project.

## How to Run
1. First you need to publish the code - `dotnet publish -c Release -o <publishLocation> <SolutionFile>`
2. Then you can run it - `<publishLocation>\DataImporter.exe <recipe or rating> <CsvFileLocation>`

## CSV Format

### Recipes 
* columns
  `Name, RecipeId, Minutes, Steps, Description, Ingredients`
* example row 
  `Mac and Cheese, 137739, 55 , "['preheat oven', 'stick the mac and cheese in', 'cook for 40 minutes']","this is a description of mac and cheese","['cheese', 'macaroni']"`
 
### Ratings 
* columns
  `UserId, RecipeId, Rating`
* example row 
  `1, 137739, 5`
