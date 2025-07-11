using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tournament.Core.Entities;
using Tournament.Core.Requests;

namespace Tournament.Core.Repositories;

public interface IUserRepository
{
    void Create(User employee);
    void Delete(User employee);
    void Update(User employee);

    Task<PagedList<User>> GetUsersAsync(UserRequestParams uParams, bool trackChanges = false);
    Task<User?> GetUserAsync(string userId, bool trackChanges = false);
}
