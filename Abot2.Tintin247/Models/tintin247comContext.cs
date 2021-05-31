using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace Abot2.Tintin247.Models
{
    public partial class tintin247comContext : DbContext
    {
        public tintin247comContext()
        {
        }

        public tintin247comContext(DbContextOptions<tintin247comContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Article> Articles { get; set; }
        public virtual DbSet<ArticleCategory> ArticleCategories { get; set; }
        public virtual DbSet<AuditLog> AuditLogs { get; set; }
        public virtual DbSet<Brand> Brands { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<RoleClaim> RoleClaims { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserClaim> UserClaims { get; set; }
        public virtual DbSet<UserLogin> UserLogins { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<UserToken> UserTokens { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=.;User Id =sa;Password=123456;Initial Catalog=tintin247.com;Integrated Security=True;MultipleActiveResultSets=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Article>(entity =>
            {
                entity.HasIndex(e => e.ArticleCategoryId, "IX_Articles_ArticleCategoryId");

                entity.HasOne(d => d.ArticleCategory)
                    .WithMany(p => p.Articles)
                    .HasForeignKey(d => d.ArticleCategoryId);
            });

            modelBuilder.Entity<ArticleCategory>(entity =>
            {
                entity.ToTable("ArticleCategory");
            });

            modelBuilder.Entity<Brand>(entity =>
            {
                entity.ToTable("Brand");

                entity.Property(e => e.Tax).HasColumnType("decimal(18, 2)");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasIndex(e => e.BrandId, "IX_Products_BrandId");

                entity.Property(e => e.Rate).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.Brand)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.BrandId);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Roles", "Identity");

                entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedName] IS NOT NULL)");

                entity.Property(e => e.Name).HasMaxLength(256);

                entity.Property(e => e.NormalizedName).HasMaxLength(256);
            });

            modelBuilder.Entity<RoleClaim>(entity =>
            {
                entity.ToTable("RoleClaims", "Identity");

                entity.HasIndex(e => e.RoleId, "IX_RoleClaims_RoleId");

                entity.Property(e => e.RoleId).IsRequired();

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.RoleClaims)
                    .HasForeignKey(d => d.RoleId);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users", "Identity");

                entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

                entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedUserName] IS NOT NULL)");

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.NormalizedEmail).HasMaxLength(256);

                entity.Property(e => e.NormalizedUserName).HasMaxLength(256);

                entity.Property(e => e.UserName).HasMaxLength(256);
            });

            modelBuilder.Entity<UserClaim>(entity =>
            {
                entity.ToTable("UserClaims", "Identity");

                entity.HasIndex(e => e.UserId, "IX_UserClaims_UserId");

                entity.Property(e => e.UserId).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserClaims)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<UserLogin>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

                entity.ToTable("UserLogins", "Identity");

                entity.HasIndex(e => e.UserId, "IX_UserLogins_UserId");

                entity.Property(e => e.UserId).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserLogins)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });

                entity.ToTable("UserRoles", "Identity");

                entity.HasIndex(e => e.RoleId, "IX_UserRoles_RoleId");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.RoleId);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<UserToken>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

                entity.ToTable("UserTokens", "Identity");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserTokens)
                    .HasForeignKey(d => d.UserId);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
