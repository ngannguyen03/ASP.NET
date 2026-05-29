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
    // 1. Định nghĩa đường dẫn để gọi API. 
    // Khi chạy, địa chỉ sẽ là: https://localhost:xxxx/api/posts
    [Route("api/[controller]")]

    // 2. Đánh dấu đây là một API Controller để hệ thống tự động kiểm tra dữ liệu
    [ApiController]

    // 3. API Controller kế thừa từ ControllerBase (nhẹ hơn Controller truyền thống)
    public class PostsController : ControllerBase
    {
        // 4. Khai báo biến kết nối Database
        private readonly ApplicationDbContext _context;

        // 5. Hàm khởi tạo (Constructor): "Tiêm" kết nối Database vào để sử dụng
        public PostsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 6. Hàm GET: Lấy danh sách toàn bộ bài viết
        [HttpGet]
        public IActionResult GetAll()
        {
            // Lấy dữ liệu từ bảng Posts, sắp xếp theo bài mới nhất
            var posts = _context.Posts
                .OrderByDescending(p => p.Id)
                .Select(p => new
                { // "Gọt tỉa" dữ liệu, chỉ lấy những trường cần thiết để JSON nhẹ hơn
                    p.Id,
                    p.Title,
                    p.ImageUrl,
                    p.CreatedDate, // Lưu ý: Dùng CreatedDate đúng với tên trường trong Model của bạn
                    CategoryName = p.Category.Name // Lấy tên danh mục
                })
                .ToList();

            // Trả về JSON kèm mã trạng thái 200 (Thành công)
            return Ok(posts);
        }

        // 7. Hàm GET: Lấy chi tiết bài viết theo ID
        [HttpGet("{id}")]
        public IActionResult GetDetail(int id)
        {
            // Tìm bài viết theo Id
            var post = _context.Posts.FirstOrDefault(p => p.Id == id);

            // Xử lý trường hợp không tìm thấy (ID không tồn tại)
            if (post == null)
            {
                // Trả về lỗi 404 kèm thông báo dạng JSON
                return NotFound(new { message = "Không tìm thấy bài viết này trong hệ thống" });
            }

            // Trả về bài viết tìm thấy kèm mã 200
            return Ok(post);
        }

        // 8 Hàm get lọc bài viết theo danh mục (CategoryId)
        [HttpGet("category/{categoryId}")]
        public IActionResult GetByCategory(int categoryId)
        {
            // Lọc các bài viết có CategoryId trùng với ID truyền vào từ URL
            var posts = _context.Posts
                .Where(p => p.CategoryId == categoryId)
                .Select(p => new
                {
                    p.Id,
                    p.Title,
                    p.ImageUrl,
                    p.CreatedDate
                })
                .ToList();

            return Ok(posts);
        }



    }
}