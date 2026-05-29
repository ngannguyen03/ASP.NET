/*
Họ Và Tên : Nguyễn Ngọc Bảo Ngân
Mssv: 2123110503
Lớp : CCQ2311D
*/
using Microsoft.AspNetCore.Mvc;
using CMS.Data;
using Microsoft.EntityFrameworkCore;

namespace CMS.Backend.Controllers
{
    // Định nghĩa đường dẫn API: api/products
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Lấy danh sách toàn bộ sản phẩm
        [HttpGet]
        public IActionResult GetAll()
        {
            // Lấy dữ liệu từ bảng Products, bao gồm thông tin danh mục (Category)
            var products = _context.Products
                .Include(p => p.CategoryProduct) // Include để lấy dữ liệu bảng liên quan
                .Select(p => new {
                    p.Id,
                    p.Name,
                    p.Price,
                    p.ImageUrl,
                    p.StockQuantity,
                    CategoryName = p.CategoryProduct.Name // Lấy tên danh mục từ khóa ngoại
                })
                .ToList();

            return Ok(products);
        }

        // 2. Lấy chi tiết sản phẩm theo ID
        [HttpGet("{id}")]
        public IActionResult GetDetail(int id)
        {
            var product = _context.Products
                .Include(p => p.CategoryProduct)
                .FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound(new { message = "Không tìm thấy sản phẩm này" });
            }

            return Ok(product);
        }
    }
}