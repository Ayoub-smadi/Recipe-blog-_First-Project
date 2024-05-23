using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Recipe__MVCProject.Models;

public partial class ModelContext : DbContext
{
    public ModelContext()
    {
    }

    public ModelContext(DbContextOptions<ModelContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Content> Contents { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Recipe> Recipes { get; set; }

    public virtual DbSet<Recipesale> Recipesales { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Testimonial> Testimonials { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseOracle("USER ID=C##ALI;PASSWORD=00000;DATA SOURCE=localhost:1521/xe");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasDefaultSchema("C##ALI")
            .UseCollation("USING_NLS_COMP");

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Categoryid).HasName("SYS_C008498");

            entity.ToTable("CATEGORIES");

            entity.Property(e => e.Categoryid)
                .HasColumnType("NUMBER")
                .HasColumnName("CATEGORYID");
            entity.Property(e => e.Categoryimage)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("CATEGORYIMAGE");
            entity.Property(e => e.Categoryname)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("CATEGORYNAME");
        });

        modelBuilder.Entity<Content>(entity =>
        {
            entity.HasKey(e => e.Contentid).HasName("SYS_C008568");

            entity.ToTable("CONTENT");

            entity.Property(e => e.Contentid)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER(38)")
                .HasColumnName("CONTENTID");
            entity.Property(e => e.Key)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("KEY");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("NAME");
            entity.Property(e => e.Value)
                .HasColumnType("CLOB")
                .HasColumnName("VALUE");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Paymentid).HasName("SYS_C008508");

            entity.ToTable("PAYMENTS");

            entity.HasIndex(e => e.Cardid, "SYS_C008509").IsUnique();

            entity.Property(e => e.Paymentid)
                .HasColumnType("NUMBER")
                .HasColumnName("PAYMENTID");
            entity.Property(e => e.Amount)
                .HasColumnType("NUMBER(10,2)")
                .HasColumnName("AMOUNT");
            entity.Property(e => e.Cardexpirydate)
                .HasColumnType("DATE")
                .HasColumnName("CARDEXPIRYDATE");
            entity.Property(e => e.Cardholdername)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("CARDHOLDERNAME");
            entity.Property(e => e.Cardid)
                .HasMaxLength(16)
                .IsUnicode(false)
                .HasColumnName("CARDID");
            entity.Property(e => e.Cardsecuritynumber)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasColumnName("CARDSECURITYNUMBER");
            entity.Property(e => e.Paymentstatus)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("PAYMENTSTATUS");
            entity.Property(e => e.Recipeid)
                .HasColumnType("NUMBER")
                .HasColumnName("RECIPEID");
            entity.Property(e => e.Transactiondate)
                .HasColumnType("DATE")
                .HasColumnName("TRANSACTIONDATE");
            entity.Property(e => e.Userid)
                .HasColumnType("NUMBER")
                .HasColumnName("USERID");

            entity.HasOne(d => d.Recipe).WithMany(p => p.Payments)
                .HasForeignKey(d => d.Recipeid)
                .HasConstraintName("SYS_C008511");

            entity.HasOne(d => d.User).WithMany(p => p.Payments)
                .HasForeignKey(d => d.Userid)
                .HasConstraintName("SYS_C008510");
        });

        modelBuilder.Entity<Recipe>(entity =>
        {
            entity.HasKey(e => e.Recipeid).HasName("SYS_C008499");

            entity.ToTable("RECIPES");

            entity.Property(e => e.Recipeid)
                .HasColumnType("NUMBER")
                .HasColumnName("RECIPEID");
            entity.Property(e => e.Addedtime)
                .HasColumnType("DATE")
                .HasColumnName("ADDEDTIME");
            entity.Property(e => e.Categoryid)
                .HasColumnType("NUMBER")
                .HasColumnName("CATEGORYID");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("DESCRIPTION");
            entity.Property(e => e.Ingredients)
                .HasColumnType("CLOB")
                .HasColumnName("INGREDIENTS");
            entity.Property(e => e.Instructions)
                .HasColumnType("CLOB")
                .HasColumnName("INSTRUCTIONS");
            entity.Property(e => e.Mainimage)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("MAINIMAGE");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("NAME");
            entity.Property(e => e.Price)
                .HasColumnType("NUMBER(10,2)")
                .HasColumnName("PRICE");
            entity.Property(e => e.Status)
                  .IsRequired()
                  .HasMaxLength(50)
                  .IsUnicode(false)
                  .HasColumnName("STATUS")
                  .HasConversion(
                      v => v.ToString(),
                      v => (Recipe.RecipeStatus)Enum.Parse(typeof(Recipe.RecipeStatus), v));
            entity.Property(e => e.Userid)
                .HasColumnType("NUMBER")
                .HasColumnName("USERID");

            entity.HasOne(d => d.Category).WithMany(p => p.Recipes)
                .HasForeignKey(d => d.Categoryid)
                .HasConstraintName("SYS_C008501");

            entity.HasOne(d => d.User).WithMany(p => p.Recipes)
                .HasForeignKey(d => d.Userid)
                .HasConstraintName("SYS_C008500");
        });

        modelBuilder.Entity<Recipesale>(entity =>
        {
            entity.HasKey(e => e.Saleid).HasName("SYS_C008505");

            entity.ToTable("RECIPESALES");

            entity.Property(e => e.Saleid)
                .HasColumnType("NUMBER")
                .HasColumnName("SALEID");
            entity.Property(e => e.Amount)
                .HasColumnType("NUMBER(10,2)")
                .HasColumnName("AMOUNT");
            entity.Property(e => e.Recipeid)
                .HasColumnType("NUMBER")
                .HasColumnName("RECIPEID");
            entity.Property(e => e.Saledate)
                .HasColumnType("DATE")
                .HasColumnName("SALEDATE");
            entity.Property(e => e.Userid)
                .HasColumnType("NUMBER")
                .HasColumnName("USERID");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Roleid).HasName("SYS_C008492");

            entity.ToTable("ROLES");

            entity.Property(e => e.Roleid)
                .HasColumnType("NUMBER")
                .HasColumnName("ROLEID");
            entity.Property(e => e.Rolename)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ROLENAME");
        });

        modelBuilder.Entity<Testimonial>(entity =>
        {
            entity.HasKey(e => e.Testimonialid).HasName("SYS_C008502");

            entity.ToTable("TESTIMONIALS");

            entity.Property(e => e.Testimonialid)
                .HasColumnType("NUMBER")
                .HasColumnName("TESTIMONIALID");
            entity.Property(e => e.Recipeid)
                .HasColumnType("NUMBER")
                .HasColumnName("RECIPEID");
            entity.Property(e => e.Testimonialcontent)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("TESTIMONIALCONTENT");
            entity.Property(e => e.Testimonialdate)
                .HasColumnType("DATE")
                .HasColumnName("TESTIMONIALDATE");
            entity.Property(e => e.Userid)
                .HasColumnType("NUMBER")
                .HasColumnName("USERID");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Userid).HasName("SYS_C008494");

            entity.ToTable("USERS");

            entity.HasIndex(e => e.Username, "SYS_C008495").IsUnique();

            entity.HasIndex(e => e.Email, "SYS_C008496").IsUnique();

            entity.Property(e => e.Userid)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER(38)")
                .HasColumnName("USERID");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("EMAIL");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("PASSWORD");
            entity.Property(e => e.Profileimage)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("PROFILEIMAGE");
            entity.Property(e => e.Roleid)
                .HasColumnType("NUMBER")
                .HasColumnName("ROLEID");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("USERNAME");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.Roleid)
                .HasConstraintName("SYS_C008497");
        });
        modelBuilder.HasSequence("CATEGORY_SEQ");
        modelBuilder.HasSequence("CONTENTSEQ");
        modelBuilder.HasSequence("PAGECONTENTSEQ");
        modelBuilder.HasSequence("RECIPE_SEQ");
        modelBuilder.HasSequence("ROLE_SEQ");
        modelBuilder.HasSequence("USER_SEQ");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
