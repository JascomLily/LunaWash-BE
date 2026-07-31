using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LunaWash.BLL.DTOs;
using LunaWash.DAL.Data;
using LunaWash.DAL.Entities;

namespace LunaWash.BLL.Services;

public class VoucherService : IVoucherService
{
    private readonly ApplicationDbContext _context;

    public VoucherService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<VoucherDto>> GetAllVouchersAsync()
    {
        var vouchers = await _context.Vouchers
            .Where(v => !v.IsDeleted)
            .ToListAsync();
            
        return vouchers.Select(v => new VoucherDto
        {
            Id = v.Id,
            VoucherName = v.VoucherName,
            Description = v.Description,
            PointsRequired = v.PointsRequired,
            DiscountValue = v.DiscountValue,
            ExpiryDate = v.ExpiryDate,
            IsActive = v.IsActive
        });
    }

    public async Task<IEnumerable<VoucherDto>> GetActiveVouchersAsync()
    {
        var vouchers = await _context.Vouchers
            .Where(v => !v.IsDeleted && v.IsActive && v.ExpiryDate > DateTime.UtcNow)
            .ToListAsync();
            
        return vouchers.Select(v => new VoucherDto
        {
            Id = v.Id,
            VoucherName = v.VoucherName,
            Description = v.Description,
            PointsRequired = v.PointsRequired,
            DiscountValue = v.DiscountValue,
            ExpiryDate = v.ExpiryDate,
            IsActive = v.IsActive
        });
    }

    public async Task<VoucherDto> CreateVoucherAsync(CreateVoucherDto dto)
    {
        var idToUse = !string.IsNullOrWhiteSpace(dto.Id) ? dto.Id.Trim() : Guid.NewGuid().ToString();

        // Kiểm tra xem voucher đã tồn tại chưa
        var existing = await _context.Vouchers.FindAsync(idToUse);
        if (existing != null)
        {
            throw new Exception("Mã Code này đã tồn tại!");
        }

        var voucher = new Voucher
        {
            Id = idToUse,
            VoucherName = dto.VoucherName,
            Description = dto.Description,
            PointsRequired = dto.PointsRequired,
            DiscountValue = dto.DiscountValue,
            ExpiryDate = dto.ExpiryDate,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        _context.Vouchers.Add(voucher);
        await _context.SaveChangesAsync();

        return new VoucherDto
        {
            Id = voucher.Id,
            VoucherName = voucher.VoucherName,
            Description = voucher.Description,
            PointsRequired = voucher.PointsRequired,
            DiscountValue = voucher.DiscountValue,
            ExpiryDate = voucher.ExpiryDate,
            IsActive = voucher.IsActive
        };
    }

    public async Task<bool> DeleteVoucherAsync(string id)
    {
        var voucher = await _context.Vouchers.FindAsync(id);
        if (voucher == null || voucher.IsDeleted) return false;

        voucher.IsDeleted = true;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<CustomerVoucherDto>> GetMyVouchersAsync(string userId)
    {
        var customerVouchers = await _context.CustomerVouchers
            .Include(cv => cv.Voucher)
            .Where(cv => cv.CustomerId == userId && !cv.IsDeleted && !cv.IsUsed)
            .ToListAsync();

        return customerVouchers.Select(cv => new CustomerVoucherDto
        {
            Id = cv.Id,
            VoucherId = cv.VoucherId,
            Voucher = new VoucherDto
            {
                Id = cv.Voucher.Id,
                VoucherName = cv.Voucher.VoucherName,
                Description = cv.Voucher.Description,
                PointsRequired = cv.Voucher.PointsRequired,
                DiscountValue = cv.Voucher.DiscountValue,
                ExpiryDate = cv.Voucher.ExpiryDate,
                IsActive = cv.Voucher.IsActive
            },
            RedeemedDate = cv.RedeemedDate,
            IsUsed = cv.IsUsed,
            UsedAt = cv.UsedAt
        });
    }

    public async Task<bool> SaveVoucherAsync(string userId, string voucherId)
    {
        var voucher = await _context.Vouchers.FindAsync(voucherId);
        if (voucher == null || !voucher.IsActive || voucher.IsDeleted || voucher.ExpiryDate < DateTime.UtcNow)
        {
            return false;
        }

        // Check if user already has it and hasn't used it
        var existing = await _context.CustomerVouchers
            .FirstOrDefaultAsync(cv => cv.CustomerId == userId && cv.VoucherId == voucherId && !cv.IsDeleted && !cv.IsUsed);
        
        if (existing != null)
        {
            return false;
        }

        var customerVoucher = new CustomerVoucher
        {
            Id = Guid.NewGuid().ToString(),
            CustomerId = userId,
            VoucherId = voucherId,
            RedeemedDate = DateTime.UtcNow,
            IsUsed = false,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        _context.CustomerVouchers.Add(customerVoucher);
        await _context.SaveChangesAsync();

        return true;
    }
}
