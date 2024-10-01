using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;

public class LoginTests : IDisposable
{
    private IWebDriver driver;
    private readonly string loginUrl = "https://localhost:7053/Admin/Login"; // Đường dẫn đến trang đăng nhập của bạn

    public LoginTests()
    {
        // Khởi tạo trình điều khiển Chrome
        driver = new ChromeDriver();
    }

    [Fact(Skip = "Skipping this test for now")]
    public void Login_With_Valid_Credentials_Should_Redirect_To_Admin_Home()
    {
        // Điều hướng đến trang đăng nhập
        driver.Navigate().GoToUrl(loginUrl);

        // Tìm các phần tử cần thiết và thực hiện nhập thông tin
        driver.FindElement(By.Id("UserName")).SendKeys("admin@gmail.com"); // Thay "validUsername" bằng tên người dùng hợp lệ // Tìm phần tử bằng tên (name)
        driver.FindElement(By.Id("Password")).SendKeys("123"); // Thay "validPassword" bằng mật khẩu hợp lệ 
        driver.FindElement(By.CssSelector("button[type='submit']")).Click(); // Tìm nút đăng nhập bằng CSS selector

        // Chờ trang chuyển hướng và kiểm tra URL
        Assert.Equal("https://localhost:7053/admin/HomeAdmin", driver.Url);
    }

    [Fact(Skip = "Skipping this test for now")]
    public void Login_With_Invalid_Credentials_Should_Show_Error_Message()
    {
        // Điều hướng đến trang đăng nhập
        driver.Navigate().GoToUrl(loginUrl);

        // Tìm các phần tử cần thiết và thực hiện nhập thông tin
        driver.FindElement(By.Id("UserName")).SendKeys("invalidUsername"); // Thay "invalidUsername" bằng tên người dùng không hợp lệ
        driver.FindElement(By.Id("Password")).SendKeys("invalidPassword"); // Thay "invalidPassword" bằng mật khẩu không hợp lệ
        driver.FindElement(By.CssSelector("button[type='submit']")).Click(); // Tìm nút đăng nhập bằng CSS selector

        // Kiểm tra xem thông báo lỗi có xuất hiện không
        var errorMessage = driver.FindElement(By.CssSelector(".alert.alert-danger")).Text; // Sử dụng selector phù hợp với thông báo lỗi
        Assert.Contains("đăng nhập không đúng", errorMessage);
    }

    public void Dispose()
    {
        // Đóng trình duyệt sau khi kiểm thử
        driver.Quit();
    }
}
