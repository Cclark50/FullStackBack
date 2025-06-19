namespace testbutton;

public class TaskService
{
    int currID = 0;
    private List<MyTask> tasks;
    
    public TaskService()
    {
        tasks =
        [
            new MyTask(GetNextID(), "Task1", "This is the first task in the list"),
            new MyTask(GetNextID(), "Task2", "This is the second task in the list")
        ];
    }
    

    public MyTask At(int index)
    {
        var task = tasks.Find(x => x.ID == index);
        return task ?? null;
    }

    public int GetNumTasks() => tasks.Count;

    public List<MyTask> GetAll()
    {
        return tasks;
    }

    private int GetNextID()
    {
        return currID++;
    }

    public IResult AddTask(string title, string desc)
    {
        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(desc)) return Results.BadRequest();
        var task = new MyTask(GetNextID(), title, desc);
        tasks.Add(task);
        Console.WriteLine("New task added: ID: " + task.ID + ", Title: " + task.Title + ", Description: " + task.Description);
        return Results.Ok(new {id =task.ID, title = title, description = desc, isCompleted = task.IsCompleted});
    }

    private bool IsValidID(int id)
    {
        Console.WriteLine(tasks.Any(task => task.ID == id));
        return tasks.Any(task => task.ID == id);
    }

    public IResult ToggleComplete(int id)
    {
        Console.WriteLine("Toggling complete task, ID: " + id);
        if (!IsValidID(id)) return Results.NotFound();
        At(id).ToggleComplete();
        Console.WriteLine("Toggled task " + id + " to " + At(id).IsCompleted);
        return Results.Ok(At(id));
    }

    public IResult DeleteTask(int id)
    {
        Console.WriteLine("Deleting task with ID: " + id);
        if (!IsValidID(id)) return Results.NotFound();
        tasks.Remove(At(id));
        return Results.Ok();
    }

    public IResult PrintAll()
    {
        foreach (var task in tasks)
        {
            Console.WriteLine(task.ID + " " + task.Title + " " + task.Description);
        }
        return Results.Ok(new {value = "102"});
    }
}