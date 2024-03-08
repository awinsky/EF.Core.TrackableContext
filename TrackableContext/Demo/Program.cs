using Demo;
using Demo.Model;

Console.WriteLine("Hello");

await using var context = new DemoContext();
//clean
var movies = context.Set<Movie>().ToArray();
foreach (var movie in movies)
{
    context.Set<Movie>().Remove(movie);
}

context.SaveChanges();

//add new 
var currentVersion = await context.GetLastVersion();
var starWars = new Movie { Title = "Star Wars", Id = 1 };
var harryPotter = new Movie { Title = "Harry Potter", Id = 2 };

context.Set<Movie>().Add(starWars);
context.Set<Movie>().Add(harryPotter);
context.SaveChanges();

var added = context.GetInserted<MovieVersioned, Movie>(currentVersion).ToArray();
var addedVersioned = context.GetInsertedVersioned<MovieVersioned, Movie>(currentVersion).ToArray();

foreach (var movie in addedVersioned)
{
    Console.WriteLine(
        $"Id:{movie.Entity.Id}; Title:{movie.Entity.Title}; Version:{movie.EntityVersion}; Operation:{movie.Operation}");
}

//update
currentVersion = await context.GetLastVersion();
var entityToUpdate = context.Set<Movie>().First(x => x.Id == 2);
entityToUpdate.Title = "Indiana Jones";
context.Set<Movie>().Update(entityToUpdate);
context.SaveChanges();
var updated = context.GetUpdatedVersioned<MovieVersioned, Movie>(currentVersion).ToArray();
foreach (var movie in updated)
{
    Console.WriteLine(
        $"Id:{movie.Entity.Id}; Title:{movie.Entity.Title}; Version:{movie.EntityVersion}; Operation:{movie.Operation}");
}

//delete
currentVersion = await context.GetLastVersion();
var entityToDelete = context.Set<Movie>().First(x=>x.Id == 1);
context.Set<Movie>().Remove(entityToDelete);
context.SaveChanges();
var deleted = context.GetDeleted<MovieVersioned, Movie>(currentVersion).ToArray();
foreach(var  movie in deleted)
{
    Console.WriteLine(
        $"Id:{movie.Id}; Version:{movie.EntityVersion}; Operation:{movie.Operation}");
}

var all = context.Set<MovieVersioned>().ToArray();


Console.ReadLine();