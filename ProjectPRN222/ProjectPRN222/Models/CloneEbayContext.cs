using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ProjectPRN222.Models;

public partial class CloneEbayContext : DbContext
{
    public CloneEbayContext()
    {
    }

    public CloneEbayContext(DbContextOptions<CloneEbayContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<Bid> Bids { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Coupon> Coupons { get; set; }

    public virtual DbSet<Dispute> Disputes { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<Inventory> Inventories { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<OrderTable> OrderTables { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ReturnRequest> ReturnRequests { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<ShippingInfo> ShippingInfos { get; set; }

    public virtual DbSet<Store> Stores { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(local);Database=CloneEbayDB;User Id=sa;Password=1;TrustServerCertificate=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__Address__3213E83F2FECEB0A");

            entity.HasOne(d => d.user).WithMany(p => p.Addresses).HasConstraintName("FK__Address__userId__3A81B327");
        });

        modelBuilder.Entity<Bid>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__Bid__3213E83FB766AFE2");

            entity.HasOne(d => d.bidder).WithMany(p => p.Bids).HasConstraintName("FK__Bid__bidderId__5629CD9C");

            entity.HasOne(d => d.product).WithMany(p => p.Bids).HasConstraintName("FK__Bid__productId__5535A963");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__Category__3213E83F58AB6C66");
        });

        modelBuilder.Entity<Coupon>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__Coupon__3213E83F79017DB5");

            entity.HasOne(d => d.product).WithMany(p => p.Coupons).HasConstraintName("FK__Coupon__productI__60A75C0F");
        });

        modelBuilder.Entity<Dispute>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__Dispute__3213E83FDF7DF453");

            entity.HasOne(d => d.order).WithMany(p => p.Disputes).HasConstraintName("FK__Dispute__orderId__693CA210");

            entity.HasOne(d => d.raisedByNavigation).WithMany(p => p.Disputes).HasConstraintName("FK__Dispute__raisedB__6A30C649");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__Feedback__3213E83F3DA8C9F8");

            entity.HasOne(d => d.seller).WithMany(p => p.Feedbacks).HasConstraintName("FK__Feedback__seller__66603565");
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__Inventor__3213E83F489CE2BD");

            entity.HasOne(d => d.product).WithMany(p => p.Inventories).HasConstraintName("FK__Inventory__produ__6383C8BA");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__Message__3213E83FEA00B90E");

            entity.HasOne(d => d.receiver).WithMany(p => p.Messagereceivers).HasConstraintName("FK__Message__receive__5DCAEF64");

            entity.HasOne(d => d.sender).WithMany(p => p.Messagesenders).HasConstraintName("FK__Message__senderI__5CD6CB2B");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__OrderIte__3213E83FBC2B7939");

            entity.HasOne(d => d.order).WithMany(p => p.OrderItems).HasConstraintName("FK__OrderItem__order__46E78A0C");

            entity.HasOne(d => d.product).WithMany(p => p.OrderItems).HasConstraintName("FK__OrderItem__produ__47DBAE45");
        });

        modelBuilder.Entity<OrderTable>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__OrderTab__3213E83F68F2D0ED");

            entity.HasOne(d => d.address).WithMany(p => p.OrderTables).HasConstraintName("FK__OrderTabl__addre__440B1D61");

            entity.HasOne(d => d.buyer).WithMany(p => p.OrderTables).HasConstraintName("FK__OrderTabl__buyer__4316F928");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__Payment__3213E83F0C603670");

            entity.HasOne(d => d.order).WithMany(p => p.Payments).HasConstraintName("FK__Payment__orderId__4AB81AF0");

            entity.HasOne(d => d.user).WithMany(p => p.Payments).HasConstraintName("FK__Payment__userId__4BAC3F29");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__Product__3213E83FBAC3A8AA");

            entity.HasOne(d => d.category).WithMany(p => p.Products).HasConstraintName("FK__Product__categor__3F466844");

            entity.HasOne(d => d.seller).WithMany(p => p.Products).HasConstraintName("FK__Product__sellerI__403A8C7D");
        });

        modelBuilder.Entity<ReturnRequest>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__ReturnRe__3213E83FEC6C449A");

            entity.HasOne(d => d.order).WithMany(p => p.ReturnRequests).HasConstraintName("FK__ReturnReq__order__5165187F");

            entity.HasOne(d => d.user).WithMany(p => p.ReturnRequests).HasConstraintName("FK__ReturnReq__userI__52593CB8");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__Review__3213E83FF214C2AD");

            entity.HasOne(d => d.product).WithMany(p => p.Reviews).HasConstraintName("FK__Review__productI__59063A47");

            entity.HasOne(d => d.reviewer).WithMany(p => p.Reviews).HasConstraintName("FK__Review__reviewer__59FA5E80");
        });

        modelBuilder.Entity<ShippingInfo>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__Shipping__3213E83F61C22C15");

            entity.HasOne(d => d.order).WithMany(p => p.ShippingInfos).HasConstraintName("FK__ShippingI__order__4E88ABD4");
        });

        modelBuilder.Entity<Store>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__Store__3213E83F28CA74D7");

            entity.HasOne(d => d.seller).WithMany(p => p.Stores).HasConstraintName("FK__Store__sellerId__6D0D32F4");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__User__3213E83FF46A33AC");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
