using System.Diagnostics;
using System.Security.AccessControl;
using EaFramework.Driver;
using Microsoft.Playwright;

namespace EaSpecflowTests.Pages;

public interface IProductListPage
{
    Task CreateProductAsync();
    Task ClickProductFromList(string name);
    ILocator IsProductCreated(string product);
}

public class ProductListPage : IProductListPage
{
    private readonly IPage _page;

    public ProductListPage(IPlaywrightDriver playwrightDriver) => _page = playwrightDriver.Page.Result;

    private ILocator _lnkProductList => _page.GetByRole(AriaRole.Link, new() { Name = "Product" });
    private ILocator _lnkCreate => _page.GetByRole(AriaRole.Link, new() { Name = "Create" });

    private ILocator _lnkLogin => _page.GetByRole(AriaRole.Link, new() { Name = "Login" });

    private ILocator _lnkEmployeeList => _page.GetByRole(AriaRole.Link, new() { Name = "Employee List" });
    private ILocator _lnkCreateNew => _page.GetByRole(AriaRole.Link, new() { Name = "Create New" });

     private ILocator _lnkBackToList => _page.GetByRole(AriaRole.Link, new() { Name = " Back to List" });

     private ILocator _Deletebtn => _page.Locator("#Delete");
    public async Task CreateProductAsync()
    {
       // await _lnkProductList.ClickAsync();
       //await _lnkCreate.ClickAsync();

       await _lnkLogin.ClickAsync();
       await _page.GetByRole(AriaRole.Textbox, new() { Name = "UserName" }).FillAsync("admin");
       await _page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).FillAsync("password");
       await _page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
       await _lnkEmployeeList.ClickAsync();
       await _lnkCreateNew.ClickAsync();
    }

    public async Task ClickProductFromList(string name)
    {
        await _page.GetByRole(AriaRole.Row, new() { Name = name })
            .GetByRole(AriaRole.Link, new() { Name = "Benefits" }).ClickAsync();
    }
    
    public ILocator IsProductCreated(string product)
    {
        
         _lnkBackToList.ClickAsync();

         _page.GetByRole(AriaRole.Row, new() { Name = product })
            .GetByRole(AriaRole.Link, new() { Name = "Delete" }).ClickAsync();
        _page.SetDefaultTimeout(10000);
    
        _page.Locator("//input[@value='Delete']").ClickAsync();
        _page.SetDefaultTimeout(10000); // 30 seconds
        
        return _page.GetByText(product, new() { Exact = true });

        
       
    }
}