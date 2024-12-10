using MeetPoint.API.Constants;
using MeetPoint.API.Database.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace MeetPoint.API.Database
{
	public class MeetPointSeeder
	{
		public static async Task LoadDataAsync(
			MeetPointContext context,
			ILoggerFactory loggerFactory,
			UserManager<UserEntity> userManager,
			RoleManager<IdentityRole> roleManager)
		{
			try
			{
				await LoadUsersAndRolesAsync(userManager, roleManager, loggerFactory);

				await LoadCategoriesAsync(context, loggerFactory);
				await LoadEventsAsync(context, loggerFactory);
				await LoadAttendancesAsync(context, loggerFactory);
				await LoadCommentsAsync(context, loggerFactory);
			}
			catch (Exception ex)
			{
				var logger = loggerFactory.CreateLogger<MeetPointSeeder>();
				logger.LogError(ex, "Error al inicializar la Data del API.");
			}
		}

		public static async Task LoadUsersAndRolesAsync(
			UserManager<UserEntity> userManager,
			RoleManager<IdentityRole> roleManager,
			ILoggerFactory loggerFactory)
		{
			try
			{
				if (!await roleManager.Roles.AnyAsync())
				{
					await roleManager.CreateAsync(new IdentityRole(RolesConstant.ADMIN));
					await roleManager.CreateAsync(new IdentityRole(RolesConstant.ORGANIZER));
					await roleManager.CreateAsync(new IdentityRole(RolesConstant.USER));
				}

				if (!await userManager.Users.AnyAsync())
				{
					var userAdmin = new UserEntity
					{
						Id = "2a373bd7-1829-4bb4-abb7-19da4257891d",
						Email = "aemiranda@unah.hn",
						UserName = "aemiranda@unah.hn",
						FirstName = "Anthony",
						LastName = "Miranda",
						Location = "Santa Rosa de Copán"
					};

					var userAdmin2 = new UserEntity
					{
						Id = "b914d419-0dea-4117-a50f-4fc55b684901",
						Email = "isaacvides@unah.hn",
						UserName = "isaacvides@unah.hn",
						FirstName = "Danilo",
						LastName = "Vides",
						Location = "Santa Rita, Copán"
					};

					var normalUser = new UserEntity
					{
						Id = "704540fe-2eaa-412f-a635-d41a4ec17404",
						Email = "jperez@unah.hn",
						UserName = "jperez@unah.hn",
						FirstName = "Juan",
						LastName = "Perez",
						Location = "Gracias, Lempira"
					};

					await userManager.CreateAsync(userAdmin, "Temporal01*");
					await userManager.CreateAsync(userAdmin2, "Temporal01*");
					await userManager.CreateAsync(normalUser, "Temporal01*");

					await userManager.AddToRoleAsync(userAdmin, RolesConstant.ADMIN);
					await userManager.AddToRoleAsync(userAdmin2, RolesConstant.ADMIN);
					await userManager.AddToRoleAsync(normalUser, RolesConstant.USER);
				}
			}
			catch (Exception e)
			{
				var logger = loggerFactory.CreateLogger<MeetPointSeeder>();
				logger.LogError(e.Message);
			}
		}

		public static async Task LoadCategoriesAsync(MeetPointContext context, ILoggerFactory loggerFactory)
		{
			try
			{
				var jsonFilePatch = "SeedData/categories.json";
				var jsonContent = await File.ReadAllTextAsync(jsonFilePatch);
				var categories = JsonConvert.DeserializeObject<List<CategoryEntity>>(jsonContent);
				
				if (!await context.Categories.AnyAsync())
				{
					var user = await context.Users.FirstOrDefaultAsync();

					for (int i=0; i < categories.Count; i++)
					{
						categories[i].CreatedBy = user.Id;
						categories[i].CreatedDate = DateTime.Now;
						categories[i].UpdatedBy = user.Id;
						categories[i].UpdatedDate = DateTime.Now;
					}
					context.AddRange(categories);
					await context.SaveChangesAsync();
				}
			}
			catch (Exception ex)
			{
				var logger = loggerFactory.CreateLogger<MeetPointSeeder>();
				logger.LogError(ex, "Error al ejecutar el Seed de Categorias.");
			}
		}
		public static async Task LoadEventsAsync(MeetPointContext context, ILoggerFactory loggerFactory)
		{
			try
			{
				var jsonFilePatch = "SeedData/events.json";
				var jsonContent = await File.ReadAllTextAsync(jsonFilePatch);
				var events = JsonConvert.DeserializeObject<List<EventEntity>>(jsonContent);

				if (!await context.Events.AnyAsync())
				{
					var user = await context.Users.FirstOrDefaultAsync();

					for (int i = 0; i < events.Count; i++)
					{
						events[i].CreatedBy = user.Id;
						events[i].CreatedDate = DateTime.Now;
						events[i].UpdatedBy = user.Id;
						events[i].UpdatedDate = DateTime.Now;
					}
					context.AddRange(events);
					await context.SaveChangesAsync();
				}
			}
			catch (Exception ex)
			{
				var logger = loggerFactory.CreateLogger<MeetPointSeeder>();
				logger.LogError(ex, "Error al ejecutar el Seed de Eventos.");
			}
		}
		public static async Task LoadAttendancesAsync(MeetPointContext context, ILoggerFactory loggerFactory)
		{
			try
			{
				var jsonFilePatch = "SeedData/attendances.json";
				var jsonContent = await File.ReadAllTextAsync(jsonFilePatch);
				var attendances = JsonConvert.DeserializeObject<List<AttendanceEntity>>(jsonContent);

				if (!await context.Attendances.AnyAsync())
				{
					var user = await context.Users.FirstOrDefaultAsync();

					for (int i = 0; i < attendances.Count; i++)
					{
						attendances[i].CreatedBy = user.Id;
						attendances[i].CreatedDate = DateTime.Now;
						attendances[i].UpdatedBy = user.Id;
						attendances[i].UpdatedDate = DateTime.Now;
					}
					context.AddRange(attendances);
					await context.SaveChangesAsync();
				}
			}
			catch (Exception ex)
			{
				var logger = loggerFactory.CreateLogger<MeetPointSeeder>();
				logger.LogError(ex, "Error al ejecutar el Seed de Asistencias.");
			}
		}
		public static async Task LoadCommentsAsync(MeetPointContext context, ILoggerFactory loggerFactory)
		{
			try
			{
				var jsonFilePatch = "SeedData/comments.json";
				var jsonContent = await File.ReadAllTextAsync(jsonFilePatch);
				var comments = JsonConvert.DeserializeObject<List<CommentEntity>>(jsonContent);

				if (!await context.Comments.AnyAsync())
				{
					var user = await context.Users.FirstOrDefaultAsync();

					for (int i = 0; i < comments.Count; i++)
					{
						comments[i].CreatedBy = user.Id;
						comments[i].CreatedDate = DateTime.Now;
						comments[i].UpdatedBy = user.Id;
						comments[i].UpdatedDate = DateTime.Now;
					}
					context.AddRange(comments);
					await context.SaveChangesAsync();
				}
			}
			catch (Exception ex)
			{
				var logger = loggerFactory.CreateLogger<MeetPointSeeder>();
				logger.LogError(ex, "Error al ejecutar el Seed de Comentarios.");
			}
		}
	}
}
