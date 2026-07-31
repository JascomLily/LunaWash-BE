using System.Collections.Generic;
using System.Threading.Tasks;
using LunaWash.BLL.DTOs;
using LunaWash.DAL.Entities;

namespace LunaWash.BLL.Services;

public interface IVoucherService
{
    Task<IEnumerable<VoucherDto>> GetAllVouchersAsync();
    Task<IEnumerable<VoucherDto>> GetActiveVouchersAsync();
    Task<VoucherDto> CreateVoucherAsync(CreateVoucherDto dto);
    Task<bool> DeleteVoucherAsync(string id);
    Task<IEnumerable<CustomerVoucherDto>> GetMyVouchersAsync(string userId);
    Task<bool> SaveVoucherAsync(string userId, string voucherId);
}
