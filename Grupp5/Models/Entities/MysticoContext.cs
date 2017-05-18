using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Grupp5.Models.Entities
{
    public partial class MysticoContext : DbContext
    {
        public virtual DbSet<Currency> Currency { get; set; }
        public virtual DbSet<Event> Event { get; set; }
        public virtual DbSet<Expense> Expense { get; set; }
        public virtual DbSet<ParticipantsInEvent> ParticipantsInEvent { get; set; }
        public virtual DbSet<PayersForExpense> PayersForExpense { get; set; }
        public virtual DbSet<User> User { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Currency>(entity =>
            {
                entity.ToTable("Currency", "split");

                entity.Property(e => e.CurrencyCode)
                    .IsRequired()
                    .HasColumnType("varchar(50)");
            });

            modelBuilder.Entity<Event>(entity =>
            {
                entity.ToTable("Event", "split");

                entity.Property(e => e.Description).HasColumnType("varchar(250)");

                entity.Property(e => e.EventName)
                    .IsRequired()
                    .HasColumnType("varchar(50)");

                entity.HasOne(d => d.ExpenseCurrency)
                    .WithMany(p => p.EventExpenseCurrency)
                    .HasForeignKey(d => d.ExpenseCurrencyId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Event_Expense_ToCurrency");

                entity.HasOne(d => d.StandardCurrency)
                    .WithMany(p => p.EventStandardCurrency)
                    .HasForeignKey(d => d.StandardCurrencyId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_EventToCurrency");
            });

            modelBuilder.Entity<Expense>(entity =>
            {
                entity.ToTable("Expense", "split");

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
                entity.ToTable("ParticipantsInEvent", "split");

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
                entity.ToTable("PayersForExpense", "split");

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
                entity.ToTable("User", "split");

                entity.Property(e => e.AspId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasColumnType("varchar(50)");
            });
        }
    }
}