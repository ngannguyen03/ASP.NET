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
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Lấy toàn bộ đơn hàng (kèm thông tin khách hàng)
        [HttpGet]
        public IActionResult GetAll()
        {
            var orders = _context.Orders
                .Include(o => o.Customer)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new
                {
                    o.Id,
                    o.OrderDate,
                    CustomerName = o.Customer.FullName,
                    o.Status,
                    o.Notes
                })
                .ToList();
            return Ok(orders);
        }

        // 2. Lấy chi tiết đơn hàng (kèm sản phẩm)
        [HttpGet("{id}")]
        public IActionResult GetDetail(int id)
        {
            var order = _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .Include(o => o.Customer)
                .Where(o => o.Id == id)
                .Select(o => new
                {
                    o.Id,
                    o.OrderDate,
                    o.Status,
                    o.Notes,
                    CustomerName = o.Customer.FullName, // Chỉ lấy tên khách hàng (tránh vòng lặp)

                    // Dùng Select để biến đổi danh sách OrderDetails thành object đơn giản
                    OrderDetails = o.OrderDetails.Select(od => new
                    {
                        od.ProductId,
                        ProductName = od.Product.Name,
                        od.Quantity,
                        od.UnitPrice
                    })
                })
                .FirstOrDefault();

            if (order == null) return NotFound(new { message = "Không tìm thấy đơn hàng" });

            return Ok(order);
        }
    }
}