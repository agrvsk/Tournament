using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tournament.Core.Entities;
using Tournament.Core.Repositories;
using Tournament.Data.Data;
using Tournament.Shared.Requests;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Tournament.Data.Repositories;

public class UserRepository(TournamentContext context) : RepositoryBase<User>(context), IUserRepository
{
    public async Task<User?> GetUserAsync(string userId, bool trackChanges)
    {
        return await FindByCondition(e => e.Id.Equals(userId), trackChanges).FirstOrDefaultAsync();
    }

    public async Task<PagedList<User>> GetUsersAsync(UserRequestParams uParams, bool trackChanges)
    {
        var data = FindAll(trackChanges);
        return await PagedList<User>.CreateAsync(data, uParams.PageNumber, uParams.PageSize);

    }
}
