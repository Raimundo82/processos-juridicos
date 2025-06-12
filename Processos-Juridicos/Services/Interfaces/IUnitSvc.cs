using Processos_Juridicos.DTOs;

namespace Processos_Juridicos.Services.Interfaces
{
    /// <summary>
    /// Provides an abstraction for operations related to units.
    /// </summary>
    public interface IUnitSvc
    {
        /// <summary>
        /// Retrieves all units asynchronously.
        /// </summary>
        /// <returns>A collection of unit DTOs.</returns>
        Task<IEnumerable<UnitDto>> GetAllUnits();

        /// <summary>
        /// Retrieves a unit by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the unit.</param>
        /// <returns>A unit DTO if found; otherwise, null.</returns>
        Task<UnitDto> GetUnitById(int id);

        /// <summary>
        /// Creates a new unit.
        /// </summary>
        /// <param name="unit">The DTO representing the unit to create.</param>
        /// <returns>The newly created unit DTO.</returns>
        Task<UnitDto> CreateUnit(UnitDto unit);

        /// <summary>
        /// Updates an existing unit asynchronously.
        /// </summary>
        /// <param name="unit">The DTO representing the unit with updated data.</param>
        /// <returns>The updated unit DTO.</returns>
        Task<UnitDto> EditUnit(UnitDto unit);

        /// <summary>
        /// Deletes a unit by its identifier asynchronously.
        /// </summary>
        /// <param name="id">The identifier of the unit to delete.</param>
        /// <returns><c>true</c> if the deletion was successful; otherwise, <c>false</c>.</returns>
        Task<bool> DeleteUnit(int id);
    }
}
