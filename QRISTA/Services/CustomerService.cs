using Microsoft.EntityFrameworkCore;
using QRB.Data;
using QRB.Models;

namespace QRB.Services
{
    public interface ICustomerService
    {
        Task<List<Customer>> GetAllCustomersAsync();
        Task<List<KhachHang>> GetAllCustomersAsKhachHangAsync();
        Task<Customer?> GetCustomerByIdAsync(Guid id);
        Task<KhachHang?> GetCustomerByIdAsKhachHangAsync(Guid id);
        Task<Customer?> GetCustomerByPhoneAsync(string phone);
        Task<KhachHang?> GetCustomerByPhoneAsKhachHangAsync(string phone);
        Task<Customer> AddCustomerAsync(Customer customer);
        Task<KhachHang> AddCustomerFromKhachHangAsync(KhachHang khachHang);
        Task<Customer?> UpdateCustomerAsync(Customer customer);
        Task<KhachHang?> UpdateCustomerFromKhachHangAsync(KhachHang khachHang);
        Task<bool> DeleteCustomerAsync(Guid id);
    }

    public class CustomerService : ICustomerService
    {
        private readonly QRBDbContext _context;

        public CustomerService(QRBDbContext context)
        {
            _context = context;
        }

        public async Task<List<Customer>> GetAllCustomersAsync()
        {
            return await _context.Customers
                .Where(c => !c.IsDelete)
                .ToListAsync();
        }

        public async Task<List<KhachHang>> GetAllCustomersAsKhachHangAsync()
        {
            var customers = await GetAllCustomersAsync();
            return customers.Select(c => new KhachHang(c)).ToList();
        }

        public async Task<Customer?> GetCustomerByIdAsync(Guid id)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(c => c.ID == id && !c.IsDelete);
        }

        public async Task<KhachHang?> GetCustomerByIdAsKhachHangAsync(Guid id)
        {
            var customer = await GetCustomerByIdAsync(id);
            return customer != null ? new KhachHang(customer) : null;
        }

        public async Task<Customer?> GetCustomerByPhoneAsync(string phone)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(c => c.Phone == phone && !c.IsDelete);
        }

        public async Task<KhachHang?> GetCustomerByPhoneAsKhachHangAsync(string phone)
        {
            var customer = await GetCustomerByPhoneAsync(phone);
            return customer != null ? new KhachHang(customer) : null;
        }

        public async Task<Customer> AddCustomerAsync(Customer customer)
        {
            customer.ID = Guid.NewGuid();
            customer.CreateTime = DateTime.Now;
            
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            
            return customer;
        }

        public async Task<KhachHang> AddCustomerFromKhachHangAsync(KhachHang khachHang)
        {
            var customer = khachHang.ToCustomer();
            var addedCustomer = await AddCustomerAsync(customer);
            return new KhachHang(addedCustomer);
        }

        public async Task<Customer?> UpdateCustomerAsync(Customer customer)
        {
            var existingCustomer = await _context.Customers.FindAsync(customer.ID);
            if (existingCustomer == null || existingCustomer.IsDelete)
                return null;

            existingCustomer.CustomerName = customer.CustomerName;
            existingCustomer.Phone = customer.Phone;
            existingCustomer.GiaTriDonHang = customer.GiaTriDonHang;
            existingCustomer.UpdateTime = DateTime.Now;

            await _context.SaveChangesAsync();
            return existingCustomer;
        }

        public async Task<KhachHang?> UpdateCustomerFromKhachHangAsync(KhachHang khachHang)
        {
            var customer = khachHang.ToCustomer();
            var updatedCustomer = await UpdateCustomerAsync(customer);
            return updatedCustomer != null ? new KhachHang(updatedCustomer) : null;
        }

        public async Task<bool> DeleteCustomerAsync(Guid id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null || customer.IsDelete)
                return false;

            customer.IsDelete = true;
            customer.UpdateTime = DateTime.Now;
            
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
