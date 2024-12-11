using MeetPoint.API.Database.Configuration;
using MeetPoint.API.Database.Entities;
using MeetPoint.API.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MeetPoint.API.Database
{
	public class MeetPointContext : IdentityDbContext<UserEntity>
	{
		private readonly IAuditService _auditService;

		public MeetPointContext(DbContextOptions options, IAuditService auditService) : base(options)
        {
			this._auditService = auditService;
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.HasDefaultSchema("security");

			// Tablas relacionadas a Usuarios
			modelBuilder.Entity<UserEntity>().ToTable("users");
			modelBuilder.Entity<IdentityRole>().ToTable("roles");
			modelBuilder.Entity<IdentityUserRole<string>>().ToTable("users_roles");
			modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("users_claims");
			modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("roles_claims");
			modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("users_logins");
			modelBuilder.Entity<IdentityUserToken<string>>().ToTable("users_tokens");

			// Configurations
			modelBuilder.ApplyConfiguration(new AttendanceConfiguration());
			modelBuilder.ApplyConfiguration(new CategoryConfiguration());
			modelBuilder.ApplyConfiguration(new CommentConfiguration());
			modelBuilder.ApplyConfiguration(new EventConfiguration());
			modelBuilder.ApplyConfiguration(new ReportConfiguration());
			modelBuilder.ApplyConfiguration(new RatingConfiguration());
			modelBuilder.ApplyConfiguration(new MembershipConfiguration());

			/* Configuración para las relaciones y la eliminación en cascada */
			// relación AttendanceEntity y UserEntity
			modelBuilder.Entity<AttendanceEntity>()
				.HasOne(a => a.User)
				.WithMany(u => u.Attendances)
				.HasForeignKey(a => a.UserId)
				.OnDelete(DeleteBehavior.NoAction);
			// relación AttendanceEntity y EventEntity
			modelBuilder.Entity<AttendanceEntity>()
				.HasOne(a => a.Event)
				.WithMany(e => e.Attendances)
				.HasForeignKey(a => a.EventId)
				.OnDelete(DeleteBehavior.Cascade);
			// relación CommentEntity y UserEntity
			modelBuilder.Entity<CommentEntity>()
				.HasOne(c => c.User)
				.WithMany(u => u.Comments)
				.HasForeignKey(c => c.UserId)
				.OnDelete(DeleteBehavior.NoAction);
			// relación CommentEntity y EventEntity
			modelBuilder.Entity<CommentEntity>()
				.HasOne(c => c.Event)
				.WithMany(e => e.Comments)
				.HasForeignKey(c => c.EventId)
				.OnDelete(DeleteBehavior.Cascade);
			// relación ReportEntity y Reporter
			modelBuilder.Entity<ReportEntity>()
				.HasOne(r => r.Reporter)
				.WithMany(u => u.MadeReports)
				.HasForeignKey(r => r.ReporterId)
				.OnDelete(DeleteBehavior.Restrict);
			// relación ReportEntity y Organizer
			modelBuilder.Entity<ReportEntity>()
				.HasOne(r => r.Organizer)
				.WithMany(u => u.Reports)
				.HasForeignKey(r => r.OrganizerId)
				.OnDelete(DeleteBehavior.Restrict);
			// relación RatingEntity y Organizer
			modelBuilder.Entity<RatingEntity>()
				.HasOne(r => r.Organizer)
				.WithMany(u => u.Ratings)
				.HasForeignKey(r => r.OrganizerId)
				.OnDelete(DeleteBehavior.Cascade);
			// relación RatingEntity y Rater
			modelBuilder.Entity<RatingEntity>()
				.HasOne(r => r.Rater)
				.WithMany(u => u.MadeRatings)
				.HasForeignKey(r => r.RaterId)
				.OnDelete(DeleteBehavior.NoAction);
			// relación de respuestas en Comentarios
			modelBuilder.Entity<CommentEntity>()
				.HasOne(c => c.ParentComment)
				.WithMany(c => c.Replies)
				.HasForeignKey(c => c.ParentId)
				.OnDelete(DeleteBehavior.NoAction);
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
						entity.CreatedBy = _auditService.GetUserId();
						entity.CreatedDate = DateTime.Now;
					}
					else
					{
						entity.UpdatedBy = _auditService.GetUserId();
						entity.UpdatedDate = DateTime.Now;
					}
				}
			}

			return base.SaveChangesAsync(cancellationToken);
		}

		public DbSet<CategoryEntity> Categories { get; set; }
		public DbSet<EventEntity> Events { get; set; }
		public DbSet<AttendanceEntity> Attendances { get; set; }
		public DbSet<CommentEntity> Comments { get; set; }
		public DbSet<ReportEntity> Reports { get; set; }
		public DbSet<RatingEntity> Ratings { get; set; }
		public DbSet<MembershipEntity> Memberships { get; set; }
	}
}
