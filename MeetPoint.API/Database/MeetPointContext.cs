using MeetPoint.API.Database.Entities;
using MeetPoint.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MeetPoint.API.Database
{
	public class MeetPointContext : DbContext
	{
		private readonly IAuthService _authService;

		public MeetPointContext(DbContextOptions options, IAuthService authService) : base(options)
        {
			this._authService = authService;
		}

		public DbSet<CategoryEntity> Categories { get; set; }
		public DbSet<EventEntity> Events { get; set; }
		public DbSet<UserEntity> Users { get; set; }
		public DbSet<AttendanceEntity> Attendances { get; set; }
		public DbSet<CommentEntity> Comments { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			// Configuración para evitar errores en cascada
			builder.Entity<AttendanceEntity>()
				.HasOne(a => a.User)
				.WithMany(u => u.Attendances)
				.HasForeignKey(a => a.UserId)
				.OnDelete(DeleteBehavior.NoAction);

			builder.Entity<AttendanceEntity>()
				.HasOne(a => a.Event)
				.WithMany(e => e.Attendances)
				.HasForeignKey(a => a.EventId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.Entity<CommentEntity>()
				.HasOne(c => c.User)
				.WithMany(u => u.Comments)
				.HasForeignKey(c => c.UserId)
				.OnDelete(DeleteBehavior.NoAction);

			builder.Entity<CommentEntity>()
				.HasOne(c => c.Event)
				.WithMany(e => e.Comments)
				.HasForeignKey(c => c.EventId)
				.OnDelete(DeleteBehavior.Cascade);
		}

		// Configurar la función SaveChangesAsync para el CreatedBy y UpdatedBy
		public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			var entries = ChangeTracker.Entries().Where(e => e.Entity is BaseEntity && (
				e.State == EntityState.Added || e.State == EntityState.Modified
			));

			foreach (var entry in entries)
			{
				var entity = entry.Entity as BaseEntity;
				if (entity != null)
				{
					if (entry.State == EntityState.Added)
					{
						entity.CreatedBy = _authService.GetUserId();
						entity.CreatedDate = DateTime.Now;
					}
					else
					{
						entity.UpdatedBy = _authService.GetUserId();
						entity.UpdatedDate = DateTime.Now;
					}
				}
			}

			return base.SaveChangesAsync(cancellationToken);
		}
	}
}
