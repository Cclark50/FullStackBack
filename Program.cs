using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using testbutton;

var MyOrigins = "_MyOrigins";
var builder = WebApplication.CreateBuilder(args);
TaskService service = new TaskService();

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
app.MapGet("api/value", service.PrintAll);
app.MapGet("api/time", () => new { time = DateTime.Now.ToString() });
app.MapGet("api/tasks", service.GetAll);
app.MapGet("api/tasks/{id:int}", (int id) =>
{
    if(id < 0) return Results.BadRequest("Invalid ID");
    return id >= service.GetNumTasks() ? Results.NotFound() : Results.Ok(service.At(id));
});
app.MapPost("api/tasks", ([FromBody] TaskIn task) => service.AddTask(task.title, task.desc));
app.MapDelete("api/tasks/{id:int}", (int id) => service.DeleteTask(id));
app.MapPut("api/tasks/{id:int}/Complete", (int id) => service.ToggleComplete(id));
app.UseCors(MyOrigins);

app.Run();