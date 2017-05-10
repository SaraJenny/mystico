using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Grupp5.Models.Entities
{
    public partial class MysticoContext : DbContext
    {
        public virtual DbSet<AspNetRoleClaims> AspNetRoleClaims { get; set; }
        public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUserRoles> AspNetUserRoles { get; set; }
        public virtual DbSet<AspNetUserTokens> AspNetUserTokens { get; set; }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
        public virtual DbSet<Currency> Currency { get; set; }
        public virtual DbSet<Event> Event { get; set; }
        public virtual DbSet<Expense> Expense { get; set; }
        public virtual DbSet<ParticipantsInEvent> ParticipantsInEvent { get; set; }
        public virtual DbSet<PayersForExpense> PayersForExpense { get; set; }
        public virtual DbSet<User> User { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AspNetRoleClaims>(entity =>
            {
                entity.HasIndex(e => e.RoleId)
                    .HasName("IX_AspNetRoleClaims_RoleId");

                entity.Property(e => e.RoleId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetRoleClaims)
                    .HasForeignKey(d => d.RoleId);
            });

            modelBuilder.Entity<AspNetRoles>(entity =>
            {
                entity.HasIndex(e => e.NormalizedName)
                    .HasName("RoleNameIndex")
                    .IsUnique();

                entity.Property(e => e.Id).HasMaxLength(450);

                entity.Property(e => e.Name).HasMaxLength(256);

                entity.Property(e => e.NormalizedName)
                    .IsRequired()
                    .HasMaxLength(256);
            });

            modelBuilder.Entity<AspNetUserClaims>(entity =>
            {
                entity.HasIndex(e => e.UserId)
                    .HasName("IX_AspNetUserClaims_UserId");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserClaims)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserLogins>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey })
                    .HasName("PK_AspNetUserLogins");

                entity.HasIndex(e => e.UserId)
                    .HasName("IX_AspNetUserLogins_UserId");

                entity.Property(e => e.LoginProvider).HasMaxLength(450);

                entity.Property(e => e.ProviderKey).HasMaxLength(450);

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserLogins)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserRoles>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId })
                    .HasName("PK_AspNetUserRoles");

                entity.HasIndex(e => e.RoleId)
                    .HasName("IX_AspNetUserRoles_RoleId");

                entity.Property(e => e.UserId).HasMaxLength(450);

                entity.Property(e => e.RoleId).HasMaxLength(450);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.RoleId);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserTokens>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name })
                    .HasName("PK_AspNetUserTokens");

                entity.Property(e => e.UserId).HasMaxLength(450);

                entity.Property(e => e.LoginProvider).HasMaxLength(450);

                entity.Property(e => e.Name).HasMaxLength(450);
            });

            modelBuilder.Entity<AspNetUsers>(entity =>
            {
                entity.HasIndex(e => e.NormalizedEmail)
                    .HasName("EmailIndex");

                entity.HasIndex(e => e.NormalizedUserName)
                    .HasName("UserNameIndex")
                    .IsUnique();

                entity.Property(e => e.Id).HasMaxLength(450);

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.NormalizedEmail).HasMaxLength(256);

                entity.Property(e => e.NormalizedUserName)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.UserName).HasMaxLength(256);
            });

            modelBuilder.Entity<Currency>(entity =>
            {
                entity.Property(e => e.CurrencyCode)
                    .IsRequired()
                    .HasColumnType("varchar(50)");
            });

            modelBuilder.Entity<Event>(entity =>
            {
                entity.Property(e => e.Description).HasColumnType("varchar(250)");

                entity.Property(e => e.EventName)
                    .IsRequired()
                    .HasColumnType("varchar(50)");

                entity.HasOne(d => d.StandardCurrency)
                    .WithMany(p => p.Event)
                    .HasForeignKey(d => d.StandardCurrencyId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_EventToCurrency");
            });

            modelBuilder.Entity<Expense>(entity =>
            {
                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.AmountInStandardCurrency).HasColumnType("money");

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.Description).HasColumnType("varchar(50)");

                entity.HasOne(d => d.Currency)
                    .WithMany(p => p.Expense)
                    .HasForeignKey(d => d.CurrencyId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_ExpenseToCurrency");

                entity.HasOne(d => d.Event)
                    .WithMany(p => p.Expense)
                    .HasForeignKey(d => d.EventId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Expense_ToEvent");

                entity.HasOne(d => d.Purchaser)
                    .WithMany(p => p.Expense)
                    .HasForeignKey(d => d.PurchaserId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Expense_ToUser");
            });

            modelBuilder.Entity<ParticipantsInEvent>(entity =>
            {
                entity.HasOne(d => d.Event)
                    .WithMany(p => p.ParticipantsInEvent)
                    .HasForeignKey(d => d.EventId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_ParticipantsInEvent_ToEvent");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ParticipantsInEvent)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_ParticipantsInEvent_ToUser");
            });

            modelBuilder.Entity<PayersForExpense>(entity =>
            {
                entity.Property(e => e.ObjectionDescription).HasColumnType("varchar(250)");

                entity.HasOne(d => d.Expense)
                    .WithMany(p => p.PayersForExpense)
                    .HasForeignKey(d => d.ExpenseId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_PayersForExpense_ToExpense");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.PayersForExpense)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_PayersForExpense_ToUser");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.AspId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasColumnType("varchar(50)");

                entity.HasOne(d => d.Asp)
                    .WithMany(p => p.User)
                    .HasForeignKey(d => d.AspId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_User_ToAspNetUser");
            });
        }
    }
}