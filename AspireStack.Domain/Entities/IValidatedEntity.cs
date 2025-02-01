using System.ComponentModel.DataAnnotations;

namespace AspireStack.Domain.Entities
{
    /// <summary>
    /// Interface for entities that need to be validated before saving to database.
    /// </summary>
    public interface IValidatedEntity
    {
        /// <summary>
        /// Validates the entity. Throws exception <see cref="ValidationException"/> if validation fails.
        /// </summary>
        void Validate();
    }
}
