using CoreLayer.Models;
using CoreLayer.Models.UserModel;
using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Data.Context;
using RepositoryLayer.IdentityData.identityContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RepositoryLayer.Data.Seeding
{
	public static class AddSeeding
	{
		// اول حاجة هعمل سييد لل كاتيجوريز ثم البرودكتس ثم الفوتوس
		// هجيب موقعهم الاول وبعدين هقراهم 
		public static async Task Seeding(ApplicationDbContext dbContext)
		{
			//get Path
			var categoriesPath = Path.Combine(Directory.GetCurrentDirectory(), "Seeding", "categories.json");
			//Read File
			var categoriesJson = File.ReadAllText(categoriesPath);
			//set as Categories
			var categories = JsonSerializer.Deserialize<List<Category>>(categoriesJson);
			//add to Db If column in DB has NO Data
			if (!dbContext.Categories.Any() && categories != null && categories.Any())
			{
				//await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM Categories;"); // لو عاوز امسح ال كاتيجوريز
				await dbContext.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Categories', RESEED, 0);"); // علشان يصفر ال id لو الجدول فاضي يبدأ من 1 يعني 
				await dbContext.Categories.AddRangeAsync(categories);
				await dbContext.SaveChangesAsync();
			}
			//Continue
			//get Path
			var productsPath = Path.Combine(Directory.GetCurrentDirectory(), "Seeding", "products.json");
			//Read File
			var productsJson = File.ReadAllText(productsPath);
			//set as products
			var products = JsonSerializer.Deserialize<List<Product>>(productsJson);
			//add to Db If column in DB has NO Data
			if (!dbContext.Products.Any() && products != null && products.Any())
			{
				//await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM Products;"); 
				await dbContext.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Products', RESEED, 0);");
				await dbContext.Products.AddRangeAsync(products);
				await dbContext.SaveChangesAsync();
			}
			//Continue
			//get Path
			var photosPath = Path.Combine(Directory.GetCurrentDirectory(), "Seeding", "photos.json");
			//Read File
			var photosJson = File.ReadAllText(photosPath);
			//set as products
			var photos = JsonSerializer.Deserialize<List<Photo>>(photosJson);
			//add to Db If column in DB has NO Data
			if (!dbContext.Photos.Any() && photos != null && photos.Any())
			{
				//await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM Photos;"); 
				await dbContext.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Photos', RESEED, 0);");
				await dbContext.Photos.AddRangeAsync(photos);
				await dbContext.SaveChangesAsync();
				//عاوز اعمل كوبي لل فوتوس من فولدر معين لفولدر معين
				var productFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "products");
				if (Directory.Exists(productFolderPath))
				{
					Directory.Delete(productFolderPath, true);
				}
				//move productFolderFrom source to destination
				var sourceDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "TestableProducts");
				CopyDirectory(sourceDir, productFolderPath);
			}

			// مينفعش اعمل كده .. لازم احفظ الاول ال كاتيجوري لان ال برودكتس مرتبط بيها وبعدها احفظ ال برودكتس لان ال فوتوس مرتبطه بيها ف مينفعش احفظ كله مرة واحده 

		}
		private static void CopyDirectory(string sourceDir, string destinationDir)
		{
			if (!Directory.Exists(sourceDir))
			{
				return;
			}
			if (!Directory.Exists(destinationDir))
			{
				Directory.CreateDirectory(destinationDir);
			}

			foreach (var fileSourcePath in Directory.GetFiles(sourceDir))
			{
				var fileDistinationPath = Path.Combine(destinationDir, Path.GetFileName(fileSourcePath));
				File.Copy(fileSourcePath, fileDistinationPath, true);
			}


			foreach (var directorySourcePath in Directory.GetDirectories(sourceDir))
			{
				var directoryDistinationPath = Path.Combine(destinationDir, Path.GetFileName(directorySourcePath));
				CopyDirectory(directorySourcePath, directoryDistinationPath);
			}

		}



		///seeding DeliveryMeyhods
		public static async Task SeedingDeliveryMethods(AppUserContext dbUserContext)
		{
			var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Seeding", "deliveryMethods.json");
			var json = File.ReadAllText(filePath);
			//convert to list of DeliveryMethod
			var deliveryMethods = JsonSerializer.Deserialize<List<DeliveryMethod>>(json);
			if (!dbUserContext.deliveryMethods.Any() && deliveryMethods != null && deliveryMethods.Any())
			{
				await dbUserContext.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('deliveryMethods', RESEED, 0);");
				await dbUserContext.deliveryMethods.AddRangeAsync(deliveryMethods);
				await dbUserContext.SaveChangesAsync();
			}


		}
	}
}
