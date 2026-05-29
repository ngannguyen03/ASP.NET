/*
Họ Và Tên : Nguyễn Ngọc Bảo Ngân
Mssv: 2123110503
Lớp : CCQ2311D
*/
using Microsoft.AspNetCore.Mvc;
using CMS.Data;
using Microsoft.EntityFrameworkCore;

namespace CMS.Backend.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CustomersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Lấy danh sách khách hàng
        [HttpGet]
        public IActionResult GetAll()
        {
            var customers = _context.Customers
                .Select(c => new
                {
                    c.Id,
                    c.FullName,
                    c.Email,
                    c.Phone,
                    c.Address
                })
                .ToList();
            return Ok(customers);
        }

        // 2. Lấy chi tiết khách hàng
        [HttpGet("{id}")]
        public IActionResult GetDetail(int id)
        {
            var customer = _context.Customers.FirstOrDefault(c => c.Id == id);
            if (customer == null) return NotFound(new { message = "Không tìm thấy khách hàng" });
            return Ok(customer);
        }
    }
}