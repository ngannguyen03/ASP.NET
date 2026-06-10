/*
Họ Và Tên : Nguyễn Ngọc Bảo Ngân
Mssv: 2123110503
Lớp : CCQ2311D
*/
using CMS.Data;
using CMS.Data.Entities; // Import namespace chứa model User
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity; // Import để dùng IPasswordHasher
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//Khai báo chính sách CORS 
builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", policy => {
        // Cho phép mọi nguồn (Origin), mọi phương thức (GET, POST...), mọi tiêu đề (Header)
        policy.AllowAnyOrigin() //Chấp nhận yêu cầu từ bất kỳ địa chỉ nào 
              .AllowAnyMethod() //Chấp nhận tất cả các kiểu gọi như lấy dữ liệu (GET), thêm mới (POST), sửa (PUT), xóa (DELETE).
              .AllowAnyHeader(); //Chấp nhận mọi thông tin đi kèm trong gói tin yêu cầu.
    });
});
// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add services to the container.
builder.Services.AddControllersWithViews();
// Đăng ký DbContext vào hệ thống
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// 3. Đăng ký dịch vụ PasswordHasher để mã hóa mật khẩu
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
// 1. Khai báo dịch vụ xác thực Cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // Đường dẫn nếu chưa đăng nhập
        options.AccessDeniedPath = "/Account/AccessDenied"; // Đường dẫn nếu vào trang không được phép
    });
builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", policy => {
        // Cho phép mọi nguồn cấp (Origin), mọi phương thức gọi (GET, POST...), và mọi thông tin đi kèm (Header)
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
// ---- CẤU HÌNH CORS (THÊM VÀO TRƯỚC builder.Build()) ----
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000") // Cho phép ReactJS ở port 3000 gọi tới
              .AllowAnyHeader()                     // Cho phép mọi loại Header (Content-Type, Authorization...)
              .AllowAnyMethod()                     // Cho phép mọi phương thức HTTP (GET, POST, PUT, DELETE)
              .AllowCredentials();                  // Hỗ trợ truyền Cookie/Session nếu cần sau này
    });
});
        


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


// Swagger middleware
app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseCors("AllowReactApp");
app.UseCors("AllowAll");
//Kích hoạt chính sách CORS (Phải nằm giữa app.UseRouting(); và app.UseAuthorization();. )
app.UseCors("AllowAll");

// 6. Kích hoạt Authentication và Authorization 
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();