using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Xunit;
using SeleniumExtras.WaitHelpers; // Thêm dòng này để sử dụng ExpectedConditions


public class CheckoutTests : IDisposable
    {
    private IWebDriver _driver;
    private WebDriverWait _wait;

    public CheckoutTests()
    {
        // Khởi tạo trình điều khiển và thời gian chờ
        _driver = new ChromeDriver();
        _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
    }

    private void Login()
    {
        _driver.Navigate().GoToUrl("https://localhost:7053/Identity/Account/Login");
        _driver.FindElement(By.Id("Input_Email")).SendKeys("tuanle324@gmail.com");
        _driver.FindElement(By.Id("Input_Password")).SendKeys("Aa123@");
        _driver.FindElement(By.Id("login-submit")).Click();
    }

    [Fact]
    public void Checkout_With_Invalid_PhoneNumber_Should_Show_Error()
    {
        // Đăng nhập
        Login();

        // Điều hướng tới trang sản phẩm
        _driver.Navigate().GoToUrl("https://localhost:7053/shoes/shop");

        // Tăng thời gian chờ lên 20 giây
        WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(20)); // Tăng lên 20 giây

        // Chọn sản phẩm cụ thể (ví dụ: SRV SNEAKERS)
        wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//a[contains(@href, '/chi-tiet/giay-sneakers-xanh-trang/4')]"))).Click();

        // Tại trang chi tiết sản phẩm, thêm sản phẩm vào giỏ hàng
        wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("input[type='submit'][value='Add to cart']"))).Click();

        // Kiểm tra nếu đã thêm vào giỏ hàng thành công, sau đó chuyển tới trang thanh toán
        _driver.Navigate().GoToUrl("https://localhost:7053/Bills");

        // Nhấn nút "Thanh toán" để chuyển đến trang Checkout
        wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//a[contains(@href, '/Bills/Checkout')]"))).Click();

        // Điền các thông tin trong form Checkout
        wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ShipAddress"))).SendKeys("123 Main St");
        wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ShipMobile"))).SendKeys("123ABC456"); // Điền số điện thoại không hợp lệ
        wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Notes"))).SendKeys("Ghi chú hợp lệ");

        // Nhấn nút "Make a Payment"
        wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button.submit.check_out"))).Click();

        // Kiểm tra Lỗi số điện thoại không hợp lệ
        var error = wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("validation-error")));
        Assert.Contains("số điện thoại không hợp lệ", error.Text);
    }





    public void Dispose()
        {
            _driver.Quit();
        }
    }

