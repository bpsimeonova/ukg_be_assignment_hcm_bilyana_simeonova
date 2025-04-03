using HCM.Domain.Entities;

namespace HCM.Application.Services.Interfaces
{
	public interface IUserService
	{
		Task<IEnumerable<User>> GetAllUsers();
		Task<IEnumerable<User>> GetAllHRs();
		Task<User> GetCurrentUser(string username);
		Task<User?> GetUserByEmail(string email);
		Task<bool> DeleteUser(Guid id);
		Task<bool> IsCircularRelationship(Guid employeeId, Guid? managerId);
		Task CheckIfUserIsManager(Guid? id);
		Task<bool> HasEnoughDays(User user, int requestedDays);
	}
}
