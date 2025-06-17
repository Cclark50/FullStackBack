namespace testbutton;

struct TaskIn
{
    public string title {set; get;}
    public string desc {set; get;}
}

public class MyTask(int id, string title, string description)
{
    public int ID
    {
        get;
    } = id;

    public string Title
    {
        get;
        set;
    } = title;

    public string Description
    {
        get;
        set;
    } = description;

    public bool IsCompleted
    {
        get;
        private set;
    } = false;

    public void ToggleComplete()
    {
        this.IsCompleted = !IsCompleted;
    }
}