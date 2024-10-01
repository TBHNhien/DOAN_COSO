using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Xunit;
using SeleniumExtras.WaitHelpers;

public class Qualitytest : IDisposable
{
    private IWebDriver _driver;
    private WebDriverWait _wait;

    public Qualitytest()
    {
        // Khởi tạo ChromeDriver và WebDriverWait với thời gian chờ
        _driver = new ChromeDriver();
        _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10)); // Thời gian chờ 10 giây
    }

    // Phương thức đăng nhập
    private void Login()
    {
        _driver.Navigate().GoToUrl("https://localhost:7053/Identity/Account/Login");
        _driver.FindElement(By.Id("Input_Email")).SendKeys("trantu@gmail.com");
        _driver.FindElement(By.Id("Input_Password")).SendKeys("Aa123@");
        _driver.FindElement(By.CssSelector("button[type='submit']")).Click(); // Tìm nút đăng nhập bằng CSS selector
    }
    // Điều hướng tới trang sản phẩm và thêm sản phẩm với số lượng tùy chọn
    
    // Điều hướng tới trang sản phẩm và thêm sản phẩm với số lượng tùy chọn
    private void NavigateToProductPageAndAddToCart(string quantity)
    {
        // Đăng nhập
        Login();

        // Điều hướng tới trang sản phẩm
        _driver.Navigate().GoToUrl("https://localhost:7053/shoes/shop");

        // Tăng thời gian chờ
        _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5)); // Tăng thời gian chờ lên 20 giây

        // Tìm và click vào sản phẩm
        _driver.Navigate().GoToUrl("https://localhost:7053/chi-tiet/giay-luoi-mau-da/10");

        // Kiểm tra và điền số lượng sản phẩm
        IWebElement quantityField = _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("hiddenQuantity")));
        quantityField.Clear(); // Đảm bảo rằng trường số lượng trống trước khi nhập
        quantityField.SendKeys(quantity); // Thêm số lượng tùy chọn vào giỏ hàng

        // Nhấn nút "Add to cart" bằng id
        _wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("addtocard"))).Click(); // Add to Cart


    }

    [Fact(Skip = "Skipping this test for now")]
    public void Checkout_With_Negative_Quantity_Should_Show_Error()
    {
        // Nhập số lượng âm
        NavigateToProductPageAndAddToCart("-5");

        // Kiểm tra lỗi
        IWebElement errorMessage = _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("error-quantity"))); // Assuming there is an error message element
        Assert.Equal("Invalid quantity", errorMessage.Text); // Kiểm tra xem có thông báo lỗi phù hợp không
    }

    [Fact(Skip = "Skipping this test for now")]
    public void Checkout_With_Real_Number_Quantity_Should_Show_Error()
    {
        // Nhập số lượng thực (e.g. 2.5)
        NavigateToProductPageAndAddToCart("2.5");

        // Kiểm tra lỗi
        IWebElement errorMessage = _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("error-quantity"))); // Assuming there is an error message element
        Assert.Equal("Invalid quantity", errorMessage.Text); // Kiểm tra xem có thông báo lỗi phù hợp không
    }

    [Fact(Skip = "Skipping this test for now")]
    public void Checkout_With_Invalid_Text_Quantity_Should_Show_Error()
    {
        // Nhập ký tự không hợp lệ (e.g. 'e')
        NavigateToProductPageAndAddToCart("e");

        // Kiểm tra lỗi
        IWebElement errorMessage = _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("error-quantity"))); // Assuming there is an error message element
        Assert.Equal("Invalid quantity", errorMessage.Text); // Kiểm tra xem có thông báo lỗi phù hợp không
    }

    [Fact(Skip = "Skipping this test for now")]
    public void Checkout_With_Zero_Quantity_Should_Show_Error()
    {
        // Nhập số lượng là 0
        NavigateToProductPageAndAddToCart("0");

        // Kiểm tra lỗi
        IWebElement errorMessage = _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("error-quantity"))); // Assuming there is an error message element
        Assert.Equal("Invalid quantity", errorMessage.Text); // Kiểm tra xem có thông báo lỗi phù hợp không
    }
    [Fact(Skip = "Skipping this test for now")]
    public void Checkout_With_Zero_Quantity_Should_Show_Error1()
    {
        // trường hợp ko nhập j
        NavigateToProductPageAndAddToCart("");

        // Kiểm tra lỗi
        IWebElement errorMessage = _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("error-quantity"))); // Assuming there is an error message element
        Assert.Equal("Invalid quantity", errorMessage.Text); // Kiểm tra xem có thông báo lỗi phù hợp không
    }
    [Fact(Skip = "Skipping this test for now")]
    public void Checkout_With_Zero_Quantity_Should_Show_Error2()
    {

        // trường hợp ko nhập j
        NavigateToProductPageAndAddToCart("1000000000000000");

        // Kiểm tra lỗi
        IWebElement errorMessage = _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("error-quantity"))); // Assuming there is an error message element
        Assert.Equal("Invalid quantity", errorMessage.Text); // Kiểm tra xem có thông báo lỗi phù hợp không
    }
    [Fact(Skip = "Skipping this test for now")]
    public void Checkout_With_Valid_Quantity_Should_Succeed()
    {
        // Nhập số lượng hợp lệ
        NavigateToProductPageAndAddToCart("4");

        // Điều hướng tới trang thanh toán
        _wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("thanhtoan"))).Click();
        // Điều hướng tới trang checkout
        _wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("checkout"))).Click();
        // Điều hướng tới trang checkout MOMO
        _wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("thanhtoanMomo"))).Click();


        // Kiểm tra số lượng giỏ hàng
        IWebElement cartQuantity = _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("cart-quantity")));
        Assert.Equal("4", cartQuantity.GetAttribute("value")); // Kiểm tra số lượng là 4
    }
    [Fact(Skip = "Skipping this test for now")]
    private void Nologin()
    {

        _driver.Navigate().GoToUrl("https://localhost:7053");

        // Điều hướng tới trang sản phẩm
        _driver.Navigate().GoToUrl("https://localhost:7053/shoes/shop");

        // Tăng thời gian chờ
        _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5)); // Tăng thời gian chờ lên 20 giây

        // Tìm và click vào sản phẩm
        _driver.Navigate().GoToUrl("https://localhost:7053/chi-tiet/giay-luoi-mau-da/10");

        // Kiểm tra và điền số lượng sản phẩm
        IWebElement quantityField = _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("hiddenQuantity")));
        quantityField.Clear(); // Đảm bảo rằng trường số lượng trống trước khi nhập
        quantityField.SendKeys("10"); // Thêm số lượng tùy chọn vào giỏ hàng

        // Nhấn nút "Add to cart" bằng id
        _wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("addtocard"))).Click(); // Add to Cart
    }

    public void Dispose()
    {
        // Đóng trình duyệt sau khi kiểm thử
        _driver.Quit();
    }
}
