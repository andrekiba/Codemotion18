namespace Xamrealm.Models
{
    public interface ICompletable
    {
        bool IsCompleted { get; }

        string Color { get; set; }
    }
}
