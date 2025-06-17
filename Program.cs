using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using testbutton;

//making a quick change to test git
var MyOrigins = "_MyOrigins";
var builder = WebApplication.CreateBuilder(args);
int currID = 0;

List<MyTask> tasks = new List<MyTask>();
tasks.Add(new MyTask(GetNextID(), "Task1", "This is the first task in the list"));
tasks.Add(new MyTask(GetNextID(), "Task2", "This is the second task in the list"));

MyTask At(int index)
{
    MyTask? task = tasks.Find(x => x.ID == index);
    if (task == null) return null;
    return task;
}

int GetNumTasks() => tasks.Count;

List<MyTask> GetAll()
{
    return tasks;
}

int GetNextID()
{
    return currID++;
}

IResult AddTask(string title, string desc)
{
    if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(desc)) return Results.BadRequest();
    MyTask task = new MyTask(GetNextID(), title, desc);
    tasks.Add(task);
    Console.WriteLine("New task added: ID: " + task.ID + ", Title: " + task.Title + ", Description: " + task.Description);
    return Results.Ok(new {id =task.ID, title = title, description = desc, isCompleted = task.IsCompleted});
}

bool IsValidID(int id)
{
    return (id >= 0 && id < tasks.Count);
}

IResult ToggleComplete(int id)
{
    Console.WriteLine("Toggling complete task, ID: " + id);
    if (!IsValidID(id)) return Results.NotFound();
    At(id).ToggleComplete();
    Console.WriteLine("Toggled task " + id + " to " + At(id).IsCompleted);
    return Results.Ok(At(id));
}

IResult DeleteTask(int id)
{
    if (!IsValidID(id)) return Results.NotFound();
    tasks.Remove(At(id));
    return Results.Ok();
}

IResult PrintAll()
{
    foreach (MyTask task in tasks)
    {
        Console.WriteLine(task.ID + " " + task.Title + " " + task.Description);
    }
    return Results.Ok(new {value = "102"});
}

//adding CORS middleware
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyOrigins, policy =>
    {
        policy.WithOrigins("http://localhost:5235",
            "http://localhost:3000",
            "192.168.0.128:3000",
            "192.168.0.131").AllowAnyHeader().WithMethods("GET", "POST", "PUT", "DELETE");
    });
});

var app = builder.Build();
app.MapGet("/", () => "Hello World!");
app.MapGet("api/value", PrintAll);
app.MapGet("api/time", () => new { time = DateTime.Now.ToString() });
app.MapGet("api/tasks", GetAll);
app.MapGet("api/tasks/{id:int}", (int id) =>
{
    if(id < 0) return Results.BadRequest("Invalid ID");
    return id >= tasks.Count ? Results.NotFound() : Results.Ok(At(id));
});
app.MapPost("api/tasks", ([FromBody] TaskIn task) => AddTask(task.title, task.desc));
app.MapDelete("api/tasks/{id:int}", (int id) => tasks.Remove(At(id)));
app.MapPut("api/tasks/{id:int}/Complete", ToggleComplete);
app.UseCors(MyOrigins);

app.Run();