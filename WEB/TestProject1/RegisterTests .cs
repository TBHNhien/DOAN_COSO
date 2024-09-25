using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

public class RegisterTests : IDisposable
{
    private IWebDriver driver;
    private readonly string registerUrl = "https://localhost:7053/Admin/Register"; // Đường dẫn đến trang đăng ký của bạn

    public RegisterTests()
    {
        // Khởi tạo trình điều khiển Chrome
        driver = new ChromeDriver();
    }

    [Fact]
    public void Register_With_Short_Password_Should_Show_Error_Message()
    {
        // Điều hướng đến trang đăng ký
        driver.Navigate().GoToUrl(registerUrl);

        // Nhập thông tin đăng ký với mật khẩu ngắn hơn 6 ký tự
        driver.FindElement(By.Id("UserName")).SendKeys("testuser@gmail.com");
        driver.FindElement(By.Id("Password")).SendKeys("123");
        driver.FindElement(By.CssSelector("button[type='submit']")).Click();

        // Kiểm tra thông báo lỗi hiển thị
        var errorMessage = driver.FindElement(By.CssSelector(".alert-danger")).Text;
        Assert.Contains("Mật khẩu phải có tối thiểu 6 ký tự.", errorMessage);
    }

    [Fact]
    public void Register_With_Password_Missing_Requirements_Should_Show_Error_Message()
    {
        // Kiểm tra từng trường hợp không thỏa mãn điều kiện mật khẩu:
        // Mật khẩu >=6 ký tự nhưng không chứa đầy đủ yêu cầu

        string[] invalidPasswords = {
            "abcdef", // chỉ chứa chữ thường
            "123456", // chỉ chứa số
            "!@#$$$", // chỉ chứa ký tự đặc biệt
            "ABCDEF"  // chỉ chứa chữ in hoa
        };

        foreach (var password in invalidPasswords)
        {
            driver.Navigate().GoToUrl(registerUrl);
            driver.FindElement(By.Id("UserName")).SendKeys("testuser@gmail.com");
            driver.FindElement(By.Id("Password")).SendKeys(password);
            driver.FindElement(By.CssSelector("button[type='submit']")).Click();

            var errorMessage = driver.FindElement(By.CssSelector(".alert-danger")).Text;
            Assert.Contains("Mật khẩu phải có ít nhất 1 chữ cái thường, 1 chữ cái in hoa, 1 số và 1 ký tự đặc biệt.", errorMessage);
        }
    }

    [Fact]
    public void Register_With_Valid_Password_Combinations_Should_Pass()
    {
        // Các trường hợp lệ:
        string[] validPasswords = {
            "abcDEF1!", // chứa chữ thường, chữ hoa, số, ký tự đặc biệt
        };

        foreach (var password in validPasswords)
        {
            driver.Navigate().GoToUrl(registerUrl);
            driver.FindElement(By.Id("UserName")).SendKeys("validuser@gmail.com");
            driver.FindElement(By.Id("Password")).SendKeys(password);
            driver.FindElement(By.CssSelector("button[type='submit']")).Click();

            // Kiểm tra URL để xác định đã đăng ký thành công hay chưa
            Assert.Contains("/Login", driver.Url);
        }
    }

    public void Dispose()
    {
        // Đóng trình duyệt sau khi kiểm thử
        driver.Quit();
    }
}
