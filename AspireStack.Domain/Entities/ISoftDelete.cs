namespace AspireStack.Domain.Entities
{
    public interface ISoftDelete
    {
        bool IsDeleted { get; set; }
        DateTime? DeletionTime { get; set; }
    }
}
