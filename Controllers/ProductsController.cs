using IMS.Models;
using IMS.Services;
using Microsoft.AspNetCore.Mvc;

namespace IMS.Controllers;

public class ProductsController : Controller
{
    private readonly ApplicationDbContext context;
    private readonly IWebHostEnvironment environment;

    public ProductsController(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        this.context = context;
        this.environment = environment;
    }

    public IActionResult Index()
    {
        var products = context.Products.ToList();
        return View(products);
    }
    public IActionResult Create()
    {
        return View();
    }
    [HttpPost]
    public IActionResult Create(ProductDto productDto)
    {
        if (productDto.Image == null)
        {
            ModelState.AddModelError(key: "Image", errorMessage: "Image is required.");
        }

        if (!ModelState.IsValid)
        {
            return View(productDto);
        }
        // Save the image
        string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
        newFileName += Path.GetExtension(productDto.Image.FileName);

        string imageFullPath = environment.WebRootPath + "/products/" + newFileName;

        using (var stream = System.IO.File.Create(imageFullPath))
        {
            productDto.Image.CopyTo(stream);
        }

        Product product = new Product()
        {
            Name = productDto.Name,
            Images = newFileName,
        };

        context.Products.Add(product);
        context.SaveChanges();


        return RedirectToAction("Index", controllerName: "Products");

    }
    
    public IActionResult Edit(int id)
    {
        var product = context.Products.Find(id);

        if (product == null)
        {
            return RedirectToAction("Index", controllerName: "Products");
        }

        // Create ProductDto from product
        var productDto = new ProductDto()
        {
            Name = product.Name,
        };
        
        ViewData["ProductId"] = product.ProductId;
        ViewData["Images"] = product.Images;
        
        return View(productDto);
    }
    [HttpPost]
    public IActionResult Edit(int id, ProductDto productDto)
    {
        var product = context.Products.Find(id);

        if (product == null)
        {
            return RedirectToAction("Index", controllerName: "Products");
        }

        if (!ModelState.IsValid)
        {
            ViewData["ProductId"] = product.ProductId;
            ViewData["Image"] = product.Images;
            return View(productDto);
        }

        // Update the image
        string newFileName = product.Images;
        if (productDto.Image != null)
        {
            newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            newFileName += Path.GetExtension(productDto.Image.FileName);

            string imageFullPath = environment.WebRootPath + "/products/" + newFileName;

            using (var stream = System.IO.File.Create(imageFullPath))
            {
                productDto.Image.CopyTo(stream);
            }

            // Delete the old image
            string oldImageFullPath = environment.WebRootPath + "/products/" + product.Images;
            System.IO.File.Delete(oldImageFullPath);
        }

        // Update the product in the database
        product.Name = productDto.Name;
        product.Images = newFileName;

        context.SaveChanges();

        return RedirectToAction("Index", controllerName: "Products");
    }

    public IActionResult Delete(int id)
    {
        var product = context.Products.Find(id);

        if (product == null)
        {
            return RedirectToAction("Index", controllerName: "Products");
        }

        // Delete the image
        string imageFullPath = environment.WebRootPath + "/products/" + product.Images;
        System.IO.File.Delete(imageFullPath);

        context.Products.Remove(product);
        context.SaveChanges();

        return RedirectToAction("Index", controllerName: "Products");
    }

}