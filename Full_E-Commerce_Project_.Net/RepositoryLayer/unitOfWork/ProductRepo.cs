using AutoMapper;
using CoreLayer.Models;
using CoreLayer.Models.Dtos;
using Full_E_Commerce_Project.Handle_MiddleWares;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.FileIO;
using RepositoryLayer.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace RepositoryLayer.unitOfWork
{
	public class ProductRepo : GenericSpecRepository<Product>, IProductRepo
	{
		private readonly IMapper mapper;
		public bool isDeletedFromDB { get; set; }

		public ProductRepo(IMapper mapper, ApplicationDbContext dbContext) : base(dbContext)
		{
			this.mapper = mapper;
		}
		
		public async Task AddProduct(AddProductDto addProductDto)
		{
			// عاوز لو ال برودكتس او ال صور فاضيه يعمل ريسيت لل ID 
			if (!dbContext.Products.Any())
			{
				await dbContext.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT('Products', RESEED, 0);");
			}
			if (!dbContext.Photos.Any())
			{
				await dbContext.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT('Photos', RESEED, 0);");

			}
			using var transaction = await dbContext.Database.BeginTransactionAsync();
				var product = mapper.Map<Product>(addProductDto);
			try
			{
				var PhotosDic = getPhotosDic(addProductDto.Photos);

				//هضيف البرودكت لل  داتا بيز 
				await dbContext.AddAsync(product);

				var result = await dbContext.SaveChangesAsync(); // كده هينضاف ف ال برودكت ال اي دي بتاعت بعد م اتعمله سيف

				if (result == 0)
				{
					//await transaction.RollbackAsync();

					throw new MyException(400, "dont save correctly to DB"); // هينزل ف الكاتش تحت 
				}


				foreach (var PhotoDic in PhotosDic)
				{
					product.Photos.Add(
						new Photo()
						{
							ImageName = Path.Combine("images", "products", product.Id.ToString(), $"{Guid.NewGuid()}_{string.Concat(PhotoDic.Value.FileName.Split(Path.GetInvalidFileNameChars()))}")
							,
							Hash = PhotoDic.Key
						}
					);
				}
				 dbContext.Update(product); /// عملت كده علشان ينضاف لل برودكت ال اي دي بتاعه وابقي عارفه

				var result2 = await dbContext.SaveChangesAsync(); // كده هينضاف ف ال برودكت ال اي دي بتاعت بعد م اتعمله سيف

				if (result2 == 0)
				{
					//await transaction.RollbackAsync();

					throw new MyException(400, "dont add photos correctly to DB"); // هينزل ف الكاتش تحت 
				}				
				//product بعد الخطوة دي اتحط اي دي لل برودكت 
				// بعد الحفظ EF Core بيروح للداتابيز، يعمل INSERT
				// ويرجع الـ Id اللي اتولد ويحطه جوه الكائن product
				// بيعمل SELECT SCOPE_IDENTITY(); بيجيب آخر Id متولد في الجلسة دي  والراجع بيحطه ف ال برودكت اي دي 

				// عاوز اضيفهم ف اللوكال بقي بس بعد م اضيفهم ف الداتا بيز ف الاخر خالص علشان اضيف بمعلومية ال id 
				var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "products", product.Id.ToString());

				if (Directory.Exists(folderPath))
				{
					Directory.Delete(folderPath, recursive: true); // ✅ هيمسح كل حاجة جواه
				}
				Directory.CreateDirectory(folderPath);
				// عاوز اضيف الصور بقي للباس ده
				foreach (var photoDic in PhotosDic)
				{
					// هجيب الاسم من الداتا بيز علشان الجويد
					var photoPath = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot", product.Photos.First(p=>p.Hash==photoDic.Key).ImageName);

					using var stream = new FileStream(photoPath, FileMode.Create);
					await photoDic.Value.CopyToAsync(stream);
				}
				await transaction.CommitAsync();

			}
			catch (Exception ex) {
			{
				await transaction.RollbackAsync();
				var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "products", product.Id.ToString());
				// لو متحفظتش ف الداتا بيز ال برودكت اي دي هتبقي ب صفر 

				if (Directory.Exists(folderPath))
				{
					Directory.Delete(folderPath, recursive: true); // ✅ هيمسح كل حاجة جواه
				}

					if (ex is MyException myException)
					{
						throw new MyException(myException.code, myException.message); // علشان تروح للميدل وير

					}
					else
					{
						throw new MyException(500, "Error while add product!", ex.StackTrace);
					}



				}


			}
			

		}

		public async Task UpdateProduct(int productId, UpdateProductDto UpdateProduct)
		{

			var oldProduct = await GetByIdSpecAsync(productId, p => p.Photos);
			
				if (oldProduct == null)
				{
					throw new MyException(code: 400, "No Found Any Product With This Id To Update");

				}
			
			////////ألجوريزم
			///1- عاوزة يرجعلي ديكشينري من الصور 
			///1*- الصور ف اللوكال اسمها ثابت واخره اسم البرودكت ف انتا دور ع الفولدر ولو ملقتهوش اعمل فولدر جديد وضيف فيه الصور ف اللوكال 
			///2-عاوزة لو بعت صور يشوف الصور اللي موجوده ف البرودكت القديم ومش موجوده ف الجديد ويمسحها م الداتا بيز ومن ال لوكال
			///3- يضيف طبعا الصور اللي مش موجوده ف البرودكت القديم ف اللوكال وف ال داتا بيز
			/// لو بعت الصور ف البرودكت الجديد ب نال 
			/// يشوف الصور اللي ف البرودكت القديم لو موجوده كلها ف اللوكال تمام 
			/// ولو فيه منها مش موجود ف اللوكال امسحه م اللوكال وامسحه م الداتا بيز وابعتله رسالة خطأ قوله ان بعض الصور اتمسحت م اللوكال او كل الصور اتمسحت م اللوكال لو كلها مش موجود
			/// بعد كده اعمل ماب بين البرودكت القديم والجديدواعمل ابديت للداتا بيز 
			/// نفذ

			using var DBTransaction = await dbContext.Database.BeginTransactionAsync();
			var backupFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "productsBackUp", productId.ToString());
			Directory.CreateDirectory(backupFolderPath);
			var productFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "products", productId.ToString());
			if (Directory.Exists(productFolderPath) && Directory.GetFiles(productFolderPath).Any())
			{
				MoveDirectory(productFolderPath, backupFolderPath); // نسخناها احتياطي 
			}
			try
			{
				Dictionary<string, string> dicHashPathLocalPhotosDic = new Dictionary<string, string>();
				if (Directory.Exists(productFolderPath) && Directory.GetFiles(productFolderPath).Any())
				{
					using var sha256 = SHA256.Create();
					foreach (var filePath in Directory.GetFiles(productFolderPath))
					{
						using var fileStream = File.OpenRead(filePath);
						var photoHash = Convert.ToHexString(sha256.ComputeHash(fileStream));
						dicHashPathLocalPhotosDic.Add(photoHash, filePath);
					}
				}

				//TODO Comments
				// هعمل ديكشينري فيه ال كي هو الهاش وال فاليو هو ال صورة 
				if (UpdateProduct.Photos != null)
				{
					// ملهوش لازمة مقارنة الصور اللي عملتها ب ال ديكشينيرز , علشان كده اعملهم كومينت 
					var dicHashPhotoUpdatedProductDic = getPhotosDic(UpdateProduct.Photos);
					
						//عاوز اضيف لل برودكت فوتوز 
						var oldproductPhotos = oldProduct.Photos.ToList();
						oldProduct.Photos = new List<Photo>();
						dbContext.Photos.RemoveRange(oldproductPhotos);
						if (oldProduct == null)
						{
							throw new MyException(400, "Not Found Product With This Id");
						}
						//معايا ال updateDic -- dicHashPhotoUpdatedProductDic

						foreach (var dic in dicHashPhotoUpdatedProductDic)
						{
							oldProduct.Photos.Add( // انا كده ضفت ال فوتو علي ال فوتو اللي كانت موجوده ف ال اولد برودكت انا عاوز امسح الفوتوز اللي كانت متبطه بيه قبل كده 
							new Photo
							{
								ImageName = Path.Combine("images", "products", productId.ToString(), $"{Guid.NewGuid()}_{string.Concat(dic.Value.FileName.Split(Path.GetInvalidFileNameChars()))}")
								,Hash = dic.Key

							}
							);
						}
						//هعمل ماب 
						mapper.Map(UpdateProduct, oldProduct); 
						Update(oldProduct);


					// همسح بقي ال فولدر لو موجود وهضيف فيه الصور والاسم بتاعها هو الاسم اللي ف الداتا بيز 
					if (Directory.Exists(productFolderPath))
					{
						Directory.Delete(productFolderPath, true);
					}
					Directory.CreateDirectory(productFolderPath);
					foreach(var dic in dicHashPhotoUpdatedProductDic)
					{
						var photoPath = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot",oldProduct.Photos.First(p=>p.Hash==dic.Key).ImageName);
						using var photoStream = new FileStream(photoPath, FileMode.Create);
						dic.Value.CopyTo(photoStream);
						// وبكده ضفت ف اللوكال الصور واسمها ب الاسم اللي هيتخزن ف الداتا بيز 
					}




					///////هضيف هنا ف اللوكال علشان هيبقي معايا الاسم اللي هينضاف م ال داتابيز 

					// عاوز اضيف بقي للوكال الصور اللي موجوده ف ال ابديت ومش موجوده ف اللوكال
					// لو الصورة موجوده ف ال ابديت ومش موجوده ف اللوكال ضيفها ف اللوكال 
					//foreach (var updatePhoto in dicHashPhotoUpdatedProductDic.Keys) // كده ضاف كل اللي ف ال ابديت 
					//{
					//	//if (!dicHashPathLocalPhotosDic.ContainsKey(updatePhoto))
					//	//{
					//		var photoPath = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot", oldProduct.Photos.First(p=>p.Hash==updatePhoto).ImageName);
					//		using var photoStream = new FileStream(photoPath, FileMode.Create);
					//		// هضيف ال ابديت فوتو لل لوكال
					//		await dicHashPhotoUpdatedProductDic[updatePhoto].CopyToAsync(photoStream);
					//	//}
					//}
					//if (Directory.Exists(productFolderPath) && Directory.GetFiles(productFolderPath).Any())
					//{

					//	// كده معايا الصور اللي موجوده ف اللوكال والصور اللي موجوده ف ال ابديت 
					//	//عاوز امسح م اللوكال كل الصور اللي فيه م عدا الصور اللي موجوده ف ال ابديت بس 
					//	// لو الصورة موجوده ف اللوكال ومش موجوده ف الابديت امسحها 
					//	foreach (var localPhoto in dicHashPathLocalPhotosDic.Keys) // كده مسح كل اللي ف اللوكال 
					//	{
					//		//if (!dicHashPhotoUpdatedProductDic.ContainsKey(localPhoto))
					//		//{
					//			// ظبط دي 
					//			File.Delete(dicHashPathLocalPhotosDic[localPhoto]);
					//		//}
					//	}
					//}







					var result=await dbContext.SaveChangesAsync();
					if(result==0)
					{
						throw new MyException(code: 400, "Not Updateed Product!!");
					}
				}
					

				
				else
				{
					// لو بيساوي ال نال 
					// لو اللوكال مش موجود او موجود ومفهوش فايلات طلعله ايرور 
					if (!Directory.Exists(productFolderPath) || !Directory.GetFiles(productFolderPath).Any())
					{
						throw new MyException(code: 400, "you must add photo Beacaue There Are No Photo In Local");
					}
					// لو اللوكال موجود امسح من الداتا بيز ال فوتو اللي موجوده فيها ومش موجوده ف اللوكال
					// ولو مسح طلعله رسالة تحذير ولو ممسحش متطلعلهوش حاجة 
					else
					{
					
						foreach (var photo in oldProduct.Photos)
						{
							if (!dicHashPathLocalPhotosDic.Keys.Contains(photo.Hash)) // لو ال فوتو اللي ف الداتا بيز مش موجوده ف اللوكال امسحها م الداتا بيز 
							{
								dbContext.Photos.Remove(photo); // طبعا مش هيتمسح من ال  oldProduct.Photos لاني معملتش اي لوجيك يمسح منها انا مسحت م الداتا بيز بس 
								isDeletedFromDB = true;
							}
						}
						await dbContext.SaveChangesAsync();

						// وعاوزة كمان يمسح الصور اللي ف اللوكال لو مش موجوده ف الداتا بيز وبعد كده لو الصور اللي ف اللوكال فاضيه يطلع ايرور 
						foreach (var localPhoto in dicHashPathLocalPhotosDic.Keys)
						{
							if(!dbContext.Photos.ToList().Select(p=>p.Hash).Contains(localPhoto))
							{
								// امسح ال لوكال فوتو 
								File.Delete(dicHashPathLocalPhotosDic[localPhoto]);
							}
						}
						if (!Directory.GetFiles(productFolderPath).Any())
						{
							Directory.Delete(productFolderPath);
							throw new MyException(code: 500, "you must add photo Beacaue There Are No Photo In Local"); /////////
						}
						// نفذ
					}
					// لو المستخدم رفع 4 صور وانا لاقيت عندي ف اللوكال صورتين منهم 
					//  ف همسح م اللوكال الصور اللي مش موجوده ف الصور المرفوعة وهضيف ليها الصور اللي ف المرفوهة ومش ف اللوكال




				}

				await DBTransaction.CommitAsync();
				if (Directory.Exists(backupFolderPath))
				{
					Directory.Delete(backupFolderPath, true);
				}

			}
			catch (Exception ex)
			{
				//if (ex is MyException myException)
				//{
				//	if(myException.code==555)
				//	{


				//		return;	// هيخرج بره الكاتش واكيد مش هيرجع للتراي تاني ف كده مش هينفذ حاجة تاني 
				//	}
				//}
				if (ex is MyException myInnerException&& myInnerException.code==500)
				{
					await DBTransaction.CommitAsync(); // احفظ اللي حصل ف الداتا بيز 
					throw new MyException(myInnerException.code, myInnerException.message); // علشان تروح للميدل وير

				}



				await DBTransaction.RollbackAsync();
				if (Directory.Exists(productFolderPath))
				{
					Directory.Delete(productFolderPath, true); // لازم امسحه وهو هينقل الفولدر ال باك اب ف فولدر جديد هيعمله بنفس الاسم 
				}
				if (Directory.Exists(backupFolderPath))
				{
					MoveDirectory(backupFolderPath, productFolderPath);
					Directory.Delete(backupFolderPath, true);
				}
				if (ex is MyException myException)
				{
					throw new MyException(myException.code, myException.message); // علشان تروح للميدل وير

				}
				else
				{
					//throw; //// يحافظ على الـ stack trace الأصلي
					throw new MyException(500, "Error while add product!", ex.StackTrace);

				}

			}
		

		}

		public async Task DeleteProduct(int productId)
		{
			var product=await dbContext.Products.FirstOrDefaultAsync(p=>p.Id==productId);
			if(product == null)
			{
				throw new MyException(400, "Not Found Any Product!!");
			}
			// هجيب الباس بتاع ال برودكت ولو موجود همسحه 
			var productFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "products", productId.ToString());
		
			if(Directory.Exists(productFolderPath))
			{
				Directory.Delete(productFolderPath, true);
			}
			dbContext.Products.Remove(product);
			await dbContext.SaveChangesAsync();
		}

		private Dictionary<string, IFormFile> getPhotosDic(IFormFileCollection Photos)
		{
			foreach (var photo in Photos)
			{

				if (!photo.ContentType.StartsWith("image/"))
				{
					throw new MyException(400, "File Must Be Photo!");
				}
			}
			// عاوز افلتر الصور دي وامسح اللي متكرر 

			Dictionary<string, IFormFile> dic = new();
			//List<string> hash = new();
			using var sha256 = SHA256.Create();
			foreach (var photo in Photos)
			{
				using var stream = photo.OpenReadStream(); // خلي ال فوتو عبارة عن استريم وبعدين تحت عملها هاش
				var photoHash = Convert.ToHexString((sha256.ComputeHash(stream)));
				dic.TryAdd(photoHash, photo);
			}
			
			return dic;
		}
		private void MoveDirectory(string srcFolder, string backUpFolder)
		{
			//لو السوروس مش موجود اعمل ايرور 
			if(!Directory.Exists(srcFolder))
			{
				throw new MyException(400,"Not Found Src Dic Or Files To Copy From!");
			}
			// لو السورس موجود شوف يه ملفات ولا لا 
			// لو فيه ملفات انقلها لل باك اب 

			// اكريت مجلد
			Directory.CreateDirectory(backUpFolder);

			foreach (var srcFilePath in Directory.GetFiles(srcFolder))
			{
				// عاوز امسك الفايل 
				//var fileName= srcFilePath.Split('/').Last();
				var fileName = Path.GetFileName(srcFilePath);
				var destFilePath = Path.Combine(backUpFolder, fileName);	
			
			File.Copy(srcFilePath, destFilePath,true);
			}
			// شوف السورس فيه مجلدات ولا لا 
			// لو فيه انقلها لل باك 
			// عملت عليه كومينت لان ف حالتي مفيش فولدر جوه فولدر هو فولدر جواه ملفات بس
			//foreach (var dic in Directory.GetDirectories(srcFolder))
			//{
			//	var dicName = Path.GetFileName(dic);
			//	//Path.GetFileName(dir) بيرجع اسم آخر جزء في المسار، سواء كان فايل أو مجلد
			//	var destDicPath =Path.Combine(backUpFolder, dicName);
			//	MoveDirectory(dic, destDicPath);
			//}
			// امسح بقي الفولدر بعد م نقلت منه الملفات 
			//Directory.Delete(srcFolder, true);
			//-- نفذ

		}
		public async Task<int> GetCountAsync()
		{
			 return await dbContext.Products.CountAsync();
		}
		// Product With Search And SortBy
		public IQueryable<Product> AllProductsWithSearchAndSortByQuery(int? CategoryId,string? SortBy , string? Search , params Expression<Func<Product, object>>[] funcIncludes)
		{
			var query = GetAllSpecIncludeWithIncludec(funcIncludes);
			//CategoryId
			if(CategoryId!=null)
			{
				query = query.Where(p => p.CategoryId == CategoryId);
			}
			// Search
			if (!string.IsNullOrEmpty(Search))
			{
				var normalizedSearch = Search.Trim().Split(' ',StringSplitOptions.RemoveEmptyEntries);
				query = query.Where(p =>
				normalizedSearch.All(
					word =>
					EF.Functions.Collate(p.Name, "SQL_Latin1_General_CP1_CI_AS").Contains(word) ||
					EF.Functions.Collate(p.Description, "SQL_Latin1_General_CP1_CI_AS").Contains(word))
					//[Gold]
					// كده لو بعتت كلمه مثلا زي 
					//Hello From Other Side
					//  ف هيبتدي يبحث ف ال برودكت نيم  عن اول كلمة Hello لو ملقاش هيبتدي يبحث ف ال برودكت ديتيلز لو ملقاش مش هيرجع حاجة 
					//  ف هيبتدي يبحث ف ال برودكت نيم  عن اول كلمة Hello  لو ملقاش هيبتدي يبحث ف ال برودكت ديتيلز لو لقي هيرجع برودكتس اللي فيها الكلمة دي وبعد كده البرودكتش اللي رجعت دي هيبتدي يبحث فيها عن تاني كلمة لو ملقهاش ف النيم او ال ديسكريبشان مش هيرجع حاجة
					//  // الخلاصة انه لازم كل الكلمات اللي موجوده ف ال ارري دي ي يلاقيها ف ال برودكت بيم او ف ال برودكت ديسكريبشن 
					);
				// ده بيخليه مش حساس ل حالة الاحرف 
				
				/*
							عندك جدول فيه مليون منتج.

							عملت Index على العمود Name.

							لو كتبت LOWER(Name) → SQL Server هيضطر يراجع المليون صف.

							لو استخدمت Collate → SQL Server هيستخدم الـ Index ويلاقي النتايج في ثواني قليلة.
				 */
			}
			//sortBy
			if (!string.IsNullOrEmpty(SortBy))
			{
				query = SortBy.ToLowerInvariant() switch
				{
					"name_asc" =>  query.OrderBy(p => p.Name) ,
					"name_desc" =>  query.OrderByDescending(p => p.Name),
					"price_asc" =>  query.OrderBy(p => p.NewPrice),
					"price_desc" =>  query.OrderByDescending(p => p.NewPrice),
					_ => query.OrderBy(p => p.Id)
				};

			}
			return query;

		}
		public async Task<int> RetrunedProductsCountAsync(IQueryable<Product> query)
		{
			return await query.CountAsync();  // هيرجعها من غير الباجينيشن 
		}
		public async Task<IReadOnlyList<Product>> Get(IQueryable<Product> query, int PageNumber, int PageSize)
		{
			query = query.Skip((PageNumber - 1) * PageSize).Take(PageSize); //لازم دي ف الاخر 
			return await query.ToListAsync(); // هيرجعها ب الباجينيشن
		}


		// عاوز اجيب الهاش كود بتاع ع البرودكتس الصور اللي ف ال برودكتس اللي موجوده وارجعها كده 
		//(1,fsfsfsfsfsf) // ديكشنري ال كي هو اسم الصورة والفاليو هو الهاش بتاعها 


		public void Get_Photos_Name_Hash_InLocal()
		{
			//Dictionary<string,string> Dic_Photos_Name_Hash = new();
			foreach(var productId in dbContext.Products.Select(p=>p.Id))
			{
				var productFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "products", productId.ToString());
				if (Directory.Exists(productFolderPath)&&Directory.GetFiles(productFolderPath).Any())
				{
					Console.WriteLine($"-------------- For Product: {productId} -----------------");

					foreach (var filePath in Directory.GetFiles(productFolderPath))
					{
						var FileName=Path.GetFileName(filePath); //Key
						using var FileStream = File.OpenRead(filePath);           //read file stream in local
						using var SHa = SHA256.Create();
						var HashFile=Convert.ToHexString( SHa.ComputeHash(FileStream));
						//Dic_Photos_Name_Hash.Add(FileName,HashFile);
						Console.WriteLine($"PhotoName:{FileName}=>PhotoHash:{HashFile}");
					}
				}

			}

			//foreach(var Dic_Photo_Name_Hash in Dic_Photos_Name_Hash)
			//{
			//	Console.WriteLine($"PhotoName:{Dic_Photo_Name_Hash.Key}=>PhotoHash:{Dic_Photo_Name_Hash.Value}");
			//}
		}
	
	}
}
