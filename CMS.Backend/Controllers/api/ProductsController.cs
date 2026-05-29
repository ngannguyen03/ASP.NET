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
                .Select(p => new
                {
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
            // Truy vấn dữ liệu và "gọt tỉa" ngay tại đây bằng Select
            var product = _context.Products
                .Include(p => p.CategoryProduct)
                .Where(p => p.Id == id) // Lọc theo ID
                .Select(p => new        // Sử dụng p. để truy cập các thuộc tính
                {
                    p.Id,
                    p.Name,
                    p.Price,
                    p.Description,
                    p.ImageUrl,
                    p.StockQuantity,
                    p.CategoryProductId,
                    CategoryName = p.CategoryProduct.Name // Vẫn dùng p. để lấy tên danh mục
                })
                .FirstOrDefault(); // Lấy bản ghi đầu tiên hoặc null

            // Kiểm tra nếu không tìm thấy
            if (product == null)
            {
                return NotFound(new { message = "Không tìm thấy sản phẩm này" });
            }

            // Trả về kết quả (lúc này 'product' đã là một object gọn gàng, không còn lỗi vòng lặp)
            return Ok(product);
        }
    }
}