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

namespace Tournament.Data.Repositories;

public class UserRepository(TournamentContext context) : RepositoryBase<User>(context), IUserRepository
{
    //public async Task<IEnumerable<User>> GetEmployeesAsync(int companyId, bool trackChanges = false)
    //{
    //    // var employees = await _context.Employees.Where(e => e.CompanyId.Equals(companyId)).ToListAsync();


    //}

    public async Task<User?> GetUserAsync(string userId, bool trackChanges)
    {
        return await FindByCondition(e => e.Id.Equals(userId), trackChanges).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<User>> GetUsersAsync(bool trackChanges)
    {
        return  await FindAll(trackChanges).ToListAsync();
    }
}
