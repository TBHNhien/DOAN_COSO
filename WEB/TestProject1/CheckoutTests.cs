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
        private readonly string _checkoutUrl = "https://localhost:7053/Bills/Checkout"; // URL trang Checkout của bạn

        public CheckoutTests()
        {
            _driver = new ChromeDriver();
        }

    // Test case: Số điện thoại chứa chữ cái
    [Fact]
    public void Checkout_With_Invalid_PhoneNumber_Should_Show_Error()
    {
        // Điều hướng đến trang đăng nhập
        _driver.Navigate().GoToUrl("https://localhost:7053/Identity/Account/Login"); // URL đến trang đăng nhập

        // Đăng nhập vào hệ thống
        _driver.FindElement(By.Id("Input_Email")).SendKeys("tuanle324@gmail.com");
        // Điền tên đăng nhập
        _driver.FindElement(By.Id("Input_Password")).SendKeys("Aa123@"); // Điền mật khẩu
        _driver.FindElement(By.Id("login-submit")).Click(); // Nhấn nút đăng nhập

        // Điều hướng đến trang Checkout sau khi đăng nhập thành công
        _driver.Navigate().GoToUrl(_checkoutUrl);

        // Tiếp tục các bước kiểm tra
        WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));

        wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ShipAddress"))).SendKeys("123 Main St");
        wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ShipMobile"))).SendKeys("123ABC456");
        wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Notes"))).SendKeys("Ghi chú hợp lệ");
        wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector(".submit.check_out"))).Click();

        var error = wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("validation-error")));
        Assert.Contains("số điện thoại không hợp lệ", error.Text);
    }



    //// Test case: Địa chỉ để trống
    //[Fact]
    //    public void Checkout_With_Empty_Address_Should_Show_Error()
    //    {
    //        _driver.Navigate().GoToUrl(_checkoutUrl);

    //        // Để trống địa chỉ
    //        _driver.FindElement(By.Id("ShipAddress")).SendKeys("");

    //        // Nhập số điện thoại hợp lệ
    //        _driver.FindElement(By.Id("ShipMobile")).SendKeys("0384877304");

    //        // Nhập ghi chú hợp lệ
    //        _driver.FindElement(By.Id("Notes")).SendKeys("Ghi chú hợp lệ");

    //        // Bấm nút Make a Payment
    //        _driver.FindElement(By.CssSelector(".submit.check_out")).Click();

    //        // Kiểm tra thông báo lỗi xuất hiện
    //        var error = _driver.FindElement(By.ClassName("validation-error"));
    //        Assert.Contains("Địa chỉ không được để trống", error.Text);
    //    }

    //    // Test case: Ghi chú để trống
    //    [Fact]
    //    public void Checkout_With_Empty_Notes_Should_Show_Error()
    //    {
    //        _driver.Navigate().GoToUrl(_checkoutUrl);

    //        // Nhập địa chỉ hợp lệ
    //        _driver.FindElement(By.Id("ShipAddress")).SendKeys("123 Main St");

    //        // Nhập số điện thoại hợp lệ
    //        _driver.FindElement(By.Id("ShipMobile")).SendKeys("0384877304");

    //        // Để trống ghi chú
    //        _driver.FindElement(By.Id("Notes")).SendKeys("");

    //        // Bấm nút Make a Payment
    //        _driver.FindElement(By.CssSelector(".submit.check_out")).Click();

    //        // Kiểm tra thông báo lỗi xuất hiện
    //        var error = _driver.FindElement(By.ClassName("validation-error"));
    //        Assert.Contains("Ghi chú không được để trống", error.Text);
    //    }

    public void Dispose()
        {
            _driver.Quit();
        }
    }

