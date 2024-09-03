using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace UdemyRealWorldUnitTest.Web.Models;

public partial class UdemyRealWorldUnitTestContext : DbContext
{
    public UdemyRealWorldUnitTestContext()
    {
    }

    public UdemyRealWorldUnitTestContext(DbContextOptions<UdemyRealWorldUnitTestContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Product> Products { get; set; }
    public virtual DbSet<Category> Categories { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Product");

            entity.Property(e => e.Color).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
        });

        //seed data. migration yapıldığında (update-database olduğunda) eklenir.
        //bu data ile yeni veritabanı oluşturulduğunda bunlar default olarak işleniyor.
        //admin kullanıcısı için 0 noktası initialize için kullanılabilir.
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Kalemler" },
            new Category {Id=2,Name="Defterler" });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
