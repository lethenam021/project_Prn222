using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccess.Models;

namespace DataAccess.IRepo
{
    public interface IDisputeRepo
    {
      
        Task<IEnumerable<Dispute>> GetDisputesForSellerAsync(int sellerId);


        Task<Dispute?> GetByIdAsync(int id);

        
        Task UpdateAsync(Dispute dispute);

      
        Task AddAsync(Dispute dispute);
    }
}
