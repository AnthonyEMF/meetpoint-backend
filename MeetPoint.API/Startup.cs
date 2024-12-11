using MeetPoint.API.Database;
using MeetPoint.API.Database.Entities;
using MeetPoint.API.Helpers;
using MeetPoint.API.Services;
using MeetPoint.API.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MeetPoint.API
{
	public class Startup
	{
		private IConfiguration Configuration { get; }

		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		// Services
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers();
			services.AddEndpointsApiExplorer();
			services.AddSwaggerGen();
			services.AddHttpContextAccessor();

			var name = Configuration.GetConnectionString("DefaultConnection");

			// Add DbContext
			services.AddDbContext<MeetPointContext>(options => 
			options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

			// Add Custom Services
			services.AddTransient<ICategoriesService, CategoriesService>();
			services.AddTransient<IEventsService, EventsService>();
			services.AddTransient<IAttendancesService, AttendancesService>();
			services.AddTransient<ICommentsService, CommentsService>();
			services.AddTransient<IReportsService, ReportsService>();
			services.AddTransient<IRatingsService, RatingsService>();
			services.AddTransient<IUsersService, UsersService>();
			services.AddTransient<IDashboardService, DashboardService>();
			services.AddTransient<IMembershipsService, MembershipsService>();

			// Security Services
			services.AddTransient<IAuthService, AuthService>();
			services.AddTransient<IAuditService, AuditService>();

			// Add AutoMapper
			services.AddAutoMapper(typeof(AutoMapperProfile));

			// Add Identity
			services.AddIdentity<UserEntity, IdentityRole>(options =>
			{
				options.SignIn.RequireConfirmedAccount = false;
			}).AddEntityFrameworkStores<MeetPointContext>()
			  .AddDefaultTokenProviders();

			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(options =>
			{
				options.SaveToken = true;
				options.RequireHttpsMetadata = false;
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = false,
					ValidAudience = Configuration["JWT:ValidAudience"],
					ValidIssuer = Configuration["JWT:ValidIssuer"],
					ClockSkew = TimeSpan.Zero,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Secret"]))
				};
			});

			// CORS Configuration
			services.AddCors(opt =>
			{
				var allowURLS = Configuration.GetSection("AllowURLS").Get<string[]>();

				opt.AddPolicy("CorsPolicy", builder => builder
				.WithOrigins(allowURLS)
				.AllowAnyMethod()
				.AllowAnyHeader()
				.AllowCredentials());
			});
		}

		// Middlewords
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseCors("CorsPolicy");

			app.UseAuthentication();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
