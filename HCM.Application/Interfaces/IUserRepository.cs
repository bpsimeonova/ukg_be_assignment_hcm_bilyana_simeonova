using HCM.Application.DTO;
using HCM.Domain.Entities;

namespace HCM.Application.Interfaces
{
	public interface IUserRepository : IRepository<User>
	{
		Task<List<UserDTO>> GetUsersAsync();
	}
}
