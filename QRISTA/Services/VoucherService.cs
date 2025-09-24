using Microsoft.EntityFrameworkCore;
using QRB.Data;
using QRB.Models;

namespace QRB.Services
{
    public interface IVoucherService
    {
        Task<List<Voucher>> GetAllVouchersAsync();
        Task<List<MaUuDai>> GetAllVouchersAsMaUuDaiAsync();
        Task<Voucher?> GetVoucherByIdAsync(Guid id);
        Task<MaUuDai?> GetVoucherByIdAsMaUuDaiAsync(Guid id);
        Task<List<Voucher>> GetVouchersByCustomerIdAsync(Guid customerId);
        Task<List<MaUuDai>> GetVouchersByCustomerIdAsMaUuDaiAsync(Guid customerId);
        Task<Voucher> AddVoucherAsync(Voucher voucher);
        Task<MaUuDai> AddVoucherFromMaUuDaiAsync(MaUuDai maUuDai);
        Task<Voucher?> UpdateVoucherAsync(Voucher voucher);
        Task<MaUuDai?> UpdateVoucherFromMaUuDaiAsync(MaUuDai maUuDai);
        Task<bool> DeleteVoucherAsync(Guid id);
    }

    public class VoucherService : IVoucherService
    {
        private readonly QRBDbContext _context;

        public VoucherService(QRBDbContext context)
        {
            _context = context;
        }

        public async Task<List<Voucher>> GetAllVouchersAsync()
        {
            return await _context.Vouchers
                .Where(v => !v.IsDelete)
                .Include(v => v.Customer)
                .ToListAsync();
        }

        public async Task<List<MaUuDai>> GetAllVouchersAsMaUuDaiAsync()
        {
            var vouchers = await GetAllVouchersAsync();
            return vouchers.Select(v => new MaUuDai(v)).ToList();
        }

        public async Task<Voucher?> GetVoucherByIdAsync(Guid id)
        {
            return await _context.Vouchers
                .Include(v => v.Customer)
                .FirstOrDefaultAsync(v => v.ID == id && !v.IsDelete);
        }

        public async Task<MaUuDai?> GetVoucherByIdAsMaUuDaiAsync(Guid id)
        {
            var voucher = await GetVoucherByIdAsync(id);
            return voucher != null ? new MaUuDai(voucher) : null;
        }

        public async Task<List<Voucher>> GetVouchersByCustomerIdAsync(Guid customerId)
        {
            return await _context.Vouchers
                .Where(v => v.IDCustomer == customerId && !v.IsDelete)
                .Include(v => v.Customer)
                .ToListAsync();
        }

        public async Task<List<MaUuDai>> GetVouchersByCustomerIdAsMaUuDaiAsync(Guid customerId)
        {
            var vouchers = await GetVouchersByCustomerIdAsync(customerId);
            return vouchers.Select(v => new MaUuDai(v)).ToList();
        }

        public async Task<Voucher> AddVoucherAsync(Voucher voucher)
        {
            voucher.ID = Guid.NewGuid();
            voucher.CreateTime = DateTime.Now;
            
            _context.Vouchers.Add(voucher);
            await _context.SaveChangesAsync();
            
            return voucher;
        }

        public async Task<MaUuDai> AddVoucherFromMaUuDaiAsync(MaUuDai maUuDai)
        {
            var voucher = maUuDai.ToVoucher();
            var addedVoucher = await AddVoucherAsync(voucher);
            return new MaUuDai(addedVoucher);
        }

        public async Task<Voucher?> UpdateVoucherAsync(Voucher voucher)
        {
            var existingVoucher = await _context.Vouchers.FindAsync(voucher.ID);
            if (existingVoucher == null || existingVoucher.IsDelete)
                return null;

            existingVoucher.IDCustomer = voucher.IDCustomer;
            existingVoucher.VoucherCode = voucher.VoucherCode;
            existingVoucher.Discount = voucher.Discount;
            existingVoucher.Status = voucher.Status;
            existingVoucher.UpdateTime = DateTime.Now;

            await _context.SaveChangesAsync();
            return existingVoucher;
        }

        public async Task<MaUuDai?> UpdateVoucherFromMaUuDaiAsync(MaUuDai maUuDai)
        {
            var voucher = maUuDai.ToVoucher();
            var updatedVoucher = await UpdateVoucherAsync(voucher);
            return updatedVoucher != null ? new MaUuDai(updatedVoucher) : null;
        }

        public async Task<bool> DeleteVoucherAsync(Guid id)
        {
            var voucher = await _context.Vouchers.FindAsync(id);
            if (voucher == null || voucher.IsDelete)
                return false;

            voucher.IsDelete = true;
            voucher.UpdateTime = DateTime.Now;
            
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
