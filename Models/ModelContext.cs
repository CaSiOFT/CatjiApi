using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CatjiApi.Models
{
    public partial class ModelContext : DbContext
    {
        public ModelContext()
        {
        }

        public ModelContext(DbContextOptions<ModelContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Admin> Admin { get; set; }
        public virtual DbSet<Block> Block { get; set; }
        public virtual DbSet<Blog> Blog { get; set; }
        public virtual DbSet<Blogcomment> Blogcomment { get; set; }
        public virtual DbSet<Blogimage> Blogimage { get; set; }
        public virtual DbSet<Cat> Cat { get; set; }
        public virtual DbSet<Favorite> Favorite { get; set; }
        public virtual DbSet<Follow> Follow { get; set; }
        public virtual DbSet<Likeblog> Likeblog { get; set; }
        public virtual DbSet<Likeblogcomment> Likeblogcomment { get; set; }
        public virtual DbSet<Likevideo> Likevideo { get; set; }
        public virtual DbSet<Likevideocomment> Likevideocomment { get; set; }
        public virtual DbSet<Message> Message { get; set; }
        public virtual DbSet<Reportblog> Reportblog { get; set; }
        public virtual DbSet<Reportvideo> Reportvideo { get; set; }
        public virtual DbSet<Searchhistory> Searchhistory { get; set; }
        public virtual DbSet<Tag> Tag { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<Video> Video { get; set; }
        public virtual DbSet<Videocomment> Videocomment { get; set; }
        public virtual DbSet<Videotag> Videotag { get; set; }
        public virtual DbSet<Watchhistory> Watchhistory { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                //optionsBuilder.UseOracle("Data Source=myweb1008.xyz:1521/orcl;User Id=Catji;Password=tongji;Persist Security Info=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:DefaultSchema", "CATJI");

            modelBuilder.Entity<Admin>(entity =>
            {
                entity.ToTable("ADMIN");

                entity.HasIndex(e => e.AdminId)
                    .HasName("USERV1_PK")
                    .IsUnique();

                entity.HasIndex(e => e.Email)
                    .HasName("USERV1_EMAIL_UN")
                    .IsUnique();

                entity.HasIndex(e => e.Nickname)
                    .HasName("USERV1_NICKNAME_UN")
                    .IsUnique();

                entity.HasIndex(e => e.Tel)
                    .HasName("USERV1_TEL_UN")
                    .IsUnique();

                entity.Property(e => e.AdminId)
                    .HasColumnName("ADMIN_ID")
                    .HasColumnType("NUMBER(6)");

                entity.Property(e => e.Avatar)
                    .HasColumnName("AVATAR")
                    .HasColumnType("VARCHAR2(256)");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("CREATE_TIME")
                    .HasColumnType("DATE");

                entity.Property(e => e.Email)
                    .HasColumnName("EMAIL")
                    .HasColumnType("VARCHAR2(256)");

                entity.Property(e => e.Gender)
                    .HasColumnName("GENDER")
                    .HasColumnType("VARCHAR2(1)");

                entity.Property(e => e.Nickname)
                    .IsRequired()
                    .HasColumnName("NICKNAME")
                    .HasColumnType("VARCHAR2(10)");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("PASSWORD")
                    .HasColumnType("VARCHAR2(20)");

                entity.Property(e => e.Tel)
                    .HasColumnName("TEL")
                    .HasColumnType("VARCHAR2(11)");
            });

            modelBuilder.Entity<Block>(entity =>
            {
                entity.HasKey(e => new { e.BlockUsid, e.Usid });

                entity.ToTable("BLOCK");

                entity.HasIndex(e => new { e.BlockUsid, e.Usid })
                    .HasName("BLOCK_PK")
                    .IsUnique();

                entity.Property(e => e.BlockUsid)
                    .HasColumnName("BLOCK_USID")
                    .HasColumnType("NUMBER(8)");

                entity.Property(e => e.Usid)
                    .HasColumnName("USID")
                    .HasColumnType("NUMBER(8)");

                entity.HasOne(d => d.BlockUs)
                    .WithMany(p => p.BlockBlockUs)
                    .HasForeignKey(d => d.BlockUsid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("BLOCK_USER_FKV2");

                entity.HasOne(d => d.Us)
                    .WithMany(p => p.BlockUs)
                    .HasForeignKey(d => d.Usid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("BLOCK_USER_FK");
            });

            modelBuilder.Entity<Blog>(entity =>
            {
                entity.HasKey(e => e.Bid);

                entity.ToTable("BLOG");

                entity.HasIndex(e => e.Bid)
                    .HasName("BLOG_PK")
                    .IsUnique();

                entity.Property(e => e.Bid)
                    .HasColumnName("BID")
                    .HasColumnType("NUMBER(8)");

                entity.Property(e => e.CommentNum)
                    .HasColumnName("COMMENT_NUM")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Content)
                    .HasColumnName("CONTENT")
                    .HasColumnType("VARCHAR2(4000)");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("CREATE_TIME")
                    .HasColumnType("DATE");

                entity.Property(e => e.IsPublic)
                    .HasColumnName("IS_PUBLIC")
                    .HasColumnType("NUMBER");

                entity.Property(e => e.LikeNum)
                    .HasColumnName("LIKE_NUM")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.TransmitNum)
                    .HasColumnName("TRANSMIT_NUM")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Usid)
                    .HasColumnName("USID")
                    .HasColumnType("NUMBER(8)");

                entity.HasOne(d => d.Us)
                    .WithMany(p => p.Blog)
                    .HasForeignKey(d => d.Usid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("BLOG_USER_FK");
            });

            modelBuilder.Entity<Blogcomment>(entity =>
            {
                entity.HasKey(e => e.Bcid);

                entity.ToTable("BLOGCOMMENT");

                entity.HasIndex(e => e.Bcid)
                    .HasName("BLOGCOMMENT_PK")
                    .IsUnique();

                entity.Property(e => e.Bcid)
                    .HasColumnName("BCID")
                    .HasColumnType("NUMBER(8)");

                entity.Property(e => e.Bid)
                    .HasColumnName("BID")
                    .HasColumnType("NUMBER(8)")
                    ;//.ValueGeneratedOnAdd();

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasColumnName("CONTENT")
                    .HasColumnType("VARCHAR2(100)");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("CREATE_TIME")
                    .HasColumnType("DATE");

                entity.Property(e => e.LikeNum)
                    .HasColumnName("LIKE_NUM")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.ParentBcid).HasColumnName("PARENT_BCID");

                entity.Property(e => e.ReplyBcid).HasColumnName("REPLY_BCID");

                entity.Property(e => e.Usid)
                    .HasColumnName("USID")
                    .HasColumnType("NUMBER(8)");

                entity.HasOne(d => d.B)
                    .WithMany(p => p.Blogcomment)
                    .HasForeignKey(d => d.Bid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("BLOGCOMMENT_BLOG_FK");

                entity.HasOne(d => d.ParentBc)
                    .WithMany(p => p.InverseParentBc)
                    .HasForeignKey(d => d.ParentBcid)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("BLOGCOMMENT_BLOGCOMMENT_BCID_FK");

                entity.HasOne(d => d.ReplyBc)
                    .WithMany(p => p.InverseReplyBc)
                    .HasForeignKey(d => d.ReplyBcid)
                    .HasConstraintName("BLOGCOMMENT_BLOGCOMMENT_FK");

                entity.HasOne(d => d.Us)
                    .WithMany(p => p.Blogcomment)
                    .HasForeignKey(d => d.Usid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("BLOGCOMMENT_USER_FK");
            });

            modelBuilder.Entity<Blogimage>(entity =>
            {
                entity.HasKey(e => new { e.ImgUrl, e.Bid });

                entity.ToTable("BLOGIMAGE");

                entity.HasIndex(e => new { e.ImgUrl, e.Bid })
                    .HasName("BLOGIMAGE_PK")
                    .IsUnique();

                entity.Property(e => e.ImgUrl)
                    .HasColumnName("IMG_URL")
                    .HasColumnType("VARCHAR2(256)");

                entity.Property(e => e.Bid)
                    .HasColumnName("BID")
                    .HasColumnType("NUMBER(8)");

                entity.HasOne(d => d.B)
                    .WithMany(p => p.Blogimage)
                    .HasForeignKey(d => d.Bid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("BLOGIMAGE_BLOG_BID_FK");
            });

            modelBuilder.Entity<Cat>(entity =>
            {
                entity.ToTable("CAT");

                entity.HasIndex(e => e.CatId)
                    .HasName("CAT_PK")
                    .IsUnique();

                entity.Property(e => e.CatId)
                    .HasColumnName("CAT_ID")
                    .HasColumnType("NUMBER(6)");

                entity.Property(e => e.Banner)
                    .HasColumnName("BANNER")
                    .HasColumnType("VARCHAR2(256)");

                entity.Property(e => e.Description)
                    .HasColumnName("DESCRIPTION")
                    .HasColumnType("VARCHAR2(2000)");

                entity.Property(e => e.Name)
                    .HasColumnName("NAME")
                    .HasColumnType("VARCHAR2(10)");

                entity.Property(e => e.Usid)
                    .HasColumnName("USID")
                    .HasColumnType("NUMBER(8)");

                entity.HasOne(d => d.Us)
                    .WithMany(p => p.Cat)
                    .HasForeignKey(d => d.Usid)
                    .HasConstraintName("CAT_USERS_USID_FK");
            });

            modelBuilder.Entity<Favorite>(entity =>
            {
                entity.HasKey(e => new { e.Usid, e.Vid });

                entity.ToTable("FAVORITE");

                entity.HasIndex(e => new { e.Usid, e.Vid })
                    .HasName("FAVORITE_PK")
                    .IsUnique();

                entity.Property(e => e.Usid)
                    .HasColumnName("USID")
                    .HasColumnType("NUMBER(8)");

                entity.Property(e => e.Vid)
                    .HasColumnName("VID")
                    ;//.ValueGeneratedOnAdd();

                entity.HasOne(d => d.Us)
                    .WithMany(p => p.Favorite)
                    .HasForeignKey(d => d.Usid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FAVORITE_USER_FK");

                entity.HasOne(d => d.V)
                    .WithMany(p => p.Favorite)
                    .HasForeignKey(d => d.Vid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FAVORITE_VIDEO_FK");
            });

            modelBuilder.Entity<Follow>(entity =>
            {
                entity.HasKey(e => new { e.Usid, e.FollowUsid });

                entity.ToTable("FOLLOW");

                entity.HasIndex(e => new { e.Usid, e.FollowUsid })
                    .HasName("FOLLOW_PK")
                    .IsUnique();

                entity.Property(e => e.Usid)
                    .HasColumnName("USID")
                    .HasColumnType("NUMBER(8)");

                entity.Property(e => e.FollowUsid)
                    .HasColumnName("FOLLOW_USID")
                    .HasColumnType("NUMBER(8)")
                    ;//.ValueGeneratedOnAdd();

                entity.HasOne(d => d.FollowUs)
                    .WithMany(p => p.FollowFollowUs)
                    .HasForeignKey(d => d.FollowUsid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FOLLOW_USER_FUSID");

                entity.HasOne(d => d.Us)
                    .WithMany(p => p.FollowUs)
                    .HasForeignKey(d => d.Usid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FOLLOW_USER_USID");
            });

            modelBuilder.Entity<Likeblog>(entity =>
            {
                entity.HasKey(e => new { e.Usid, e.Bid });

                entity.ToTable("LIKEBLOG");

                entity.HasIndex(e => new { e.Usid, e.Bid })
                    .HasName("LIKEBLOG_PK")
                    .IsUnique();

                entity.Property(e => e.Usid)
                    .HasColumnName("USID")
                    .HasColumnType("NUMBER(8)");

                entity.Property(e => e.Bid)
                    .HasColumnName("BID")
                    ;//.ValueGeneratedOnAdd();

                entity.HasOne(d => d.B)
                    .WithMany(p => p.Likeblog)
                    .HasForeignKey(d => d.Bid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("LIKEBLOG_BLOG_FK");

                entity.HasOne(d => d.Us)
                    .WithMany(p => p.Likeblog)
                    .HasForeignKey(d => d.Usid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("LIKEBLOG_USER_FK");
            });

            modelBuilder.Entity<Likeblogcomment>(entity =>
            {
                entity.HasKey(e => new { e.Usid, e.Bcid });

                entity.ToTable("LIKEBLOGCOMMENT");

                entity.HasIndex(e => new { e.Usid, e.Bcid })
                    .HasName("LIKEBLOGCOMMENT_PK")
                    .IsUnique();

                entity.Property(e => e.Usid)
                    .HasColumnName("USID")
                    .HasColumnType("NUMBER(8)");

                entity.Property(e => e.Bcid)
                    .HasColumnName("BCID")
                    .HasColumnType("NUMBER(8)")
                    ;//.ValueGeneratedOnAdd();

                entity.HasOne(d => d.Bc)
                    .WithMany(p => p.Likeblogcomment)
                    .HasForeignKey(d => d.Bcid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("LIKEBLOGCOMMENT_FK");

                entity.HasOne(d => d.Us)
                    .WithMany(p => p.Likeblogcomment)
                    .HasForeignKey(d => d.Usid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("LIKEBLOGCOMMENT_USER_FK");
            });

            modelBuilder.Entity<Likevideo>(entity =>
            {
                entity.HasKey(e => new { e.Usid, e.Vid });

                entity.ToTable("LIKEVIDEO");

                entity.HasIndex(e => new { e.Usid, e.Vid })
                    .HasName("LIKEVIDEO_PK")
                    .IsUnique();

                entity.Property(e => e.Usid)
                    .HasColumnName("USID")
                    .HasColumnType("NUMBER(8)");

                entity.Property(e => e.Vid)
                    .HasColumnName("VID")
                    ;//.ValueGeneratedOnAdd();

                entity.HasOne(d => d.Us)
                    .WithMany(p => p.Likevideo)
                    .HasForeignKey(d => d.Usid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("LIKEVIDEO_USER_FK");

                entity.HasOne(d => d.V)
                    .WithMany(p => p.Likevideo)
                    .HasForeignKey(d => d.Vid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("LIKEVIDEO_VIDEO_FK");
            });

            modelBuilder.Entity<Likevideocomment>(entity =>
            {
                entity.HasKey(e => new { e.Usid, e.Vcid });

                entity.ToTable("LIKEVIDEOCOMMENT");

                entity.HasIndex(e => new { e.Usid, e.Vcid })
                    .HasName("LIKEVIDEOCOMMENT_PK")
                    .IsUnique();

                entity.Property(e => e.Usid)
                    .HasColumnName("USID")
                    .HasColumnType("NUMBER(8)");

                entity.Property(e => e.Vcid)
                    .HasColumnName("VCID")
                    ;//.ValueGeneratedOnAdd();

                entity.HasOne(d => d.Us)
                    .WithMany(p => p.Likevideocomment)
                    .HasForeignKey(d => d.Usid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("LIKEVIDEOCOMMENT_USER_FK");

                entity.HasOne(d => d.Vc)
                    .WithMany(p => p.Likevideocomment)
                    .HasForeignKey(d => d.Vcid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("LIKEVIDEOCOMMENT_FK");
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasKey(e => e.Mid);

                entity.ToTable("MESSAGE");

                entity.HasIndex(e => e.Mid)
                    .HasName("MESSAGE_PK")
                    .IsUnique();

                entity.Property(e => e.Mid).HasColumnName("MID");

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasColumnName("CONTENT")
                    .HasColumnType("VARCHAR2(400)");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("CREATE_TIME")
                    .HasColumnType("DATE");

                entity.Property(e => e.ToUsid)
                    .HasColumnName("TO_USID")
                    .HasColumnType("NUMBER(8)");

                entity.Property(e => e.Usid)
                    .HasColumnName("USID")
                    .HasColumnType("NUMBER(8)");

                entity.HasOne(d => d.ToUs)
                    .WithMany(p => p.MessageToUs)
                    .HasForeignKey(d => d.ToUsid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("MESSAGE_USER_FKV2");

                entity.HasOne(d => d.Us)
                    .WithMany(p => p.MessageUs)
                    .HasForeignKey(d => d.Usid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("MESSAGE_USER_FK");
            });

            modelBuilder.Entity<Reportblog>(entity =>
            {
                entity.HasKey(e => e.Brid);

                entity.ToTable("REPORTBLOG");

                entity.HasIndex(e => e.Brid)
                    .HasName("REPORTBLOG_PK")
                    .IsUnique();

                entity.Property(e => e.Brid).HasColumnName("BRID");

                entity.Property(e => e.Admin)
                    .HasColumnName("ADMIN")
                    .HasColumnType("NUMBER(6)");

                entity.Property(e => e.Bid)
                    .HasColumnName("BID")
                    .HasColumnType("NUMBER(8)");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("CREATE_TIME")
                    .HasColumnType("DATE");

                entity.Property(e => e.Reason)
                    .HasColumnName("REASON")
                    .HasColumnType("VARCHAR2(400)");

                entity.Property(e => e.Reply)
                    .HasColumnName("REPLY")
                    .HasColumnType("VARCHAR2(400)");

                entity.Property(e => e.ReplyTime)
                    .HasColumnName("REPLY_TIME")
                    .HasColumnType("DATE");

                entity.Property(e => e.Usid)
                    .HasColumnName("USID")
                    .HasColumnType("NUMBER(8)");

                entity.HasOne(d => d.AdminNavigation)
                    .WithMany(p => p.Reportblog)
                    .HasForeignKey(d => d.Admin)
                    .HasConstraintName("REPORTBLOG_ADMIN_FK");

                entity.HasOne(d => d.B)
                    .WithMany(p => p.Reportblog)
                    .HasForeignKey(d => d.Bid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("REPORTBLOG_BLOG_FK");

                entity.HasOne(d => d.Us)
                    .WithMany(p => p.Reportblog)
                    .HasForeignKey(d => d.Usid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("REPORTBLOG_USER_FK");
            });

            modelBuilder.Entity<Reportvideo>(entity =>
            {
                entity.HasKey(e => e.Vrid);

                entity.ToTable("REPORTVIDEO");

                entity.HasIndex(e => e.Vrid)
                    .HasName("REPORTVIDEO_PK")
                    .IsUnique();

                entity.Property(e => e.Vrid).HasColumnName("VRID");

                entity.Property(e => e.Admin)
                    .HasColumnName("ADMIN")
                    .HasColumnType("NUMBER(6)");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("CREATE_TIME")
                    .HasColumnType("DATE");

                entity.Property(e => e.Reason)
                    .HasColumnName("REASON")
                    .HasColumnType("VARCHAR2(400)");

                entity.Property(e => e.Reply)
                    .HasColumnName("REPLY")
                    .HasColumnType("VARCHAR2(400)");

                entity.Property(e => e.ReplyTime)
                    .HasColumnName("REPLY_TIME")
                    .HasColumnType("DATE");

                entity.Property(e => e.Usid)
                    .HasColumnName("USID")
                    .HasColumnType("NUMBER(8)");

                entity.Property(e => e.Vid).HasColumnName("VID");

                entity.HasOne(d => d.AdminNavigation)
                    .WithMany(p => p.Reportvideo)
                    .HasForeignKey(d => d.Admin)
                    .HasConstraintName("REPORTVIDEO_ADMIN_FK");

                entity.HasOne(d => d.Us)
                    .WithMany(p => p.Reportvideo)
                    .HasForeignKey(d => d.Usid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("REPORTVIDEO_USER_FK");

                entity.HasOne(d => d.V)
                    .WithMany(p => p.Reportvideo)
                    .HasForeignKey(d => d.Vid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("REPORTVIDEO_VIDEO_FK");
            });

            modelBuilder.Entity<Searchhistory>(entity =>
            {
                entity.HasKey(e => new { e.Usid, e.Content, e.CreateTime });

                entity.ToTable("SEARCHHISTORY");

                entity.HasIndex(e => new { e.Usid, e.Content, e.CreateTime })
                    .HasName("SEARCHHISTORY_PK")
                    .IsUnique();

                entity.Property(e => e.Usid)
                    .HasColumnName("USID")
                    .HasColumnType("NUMBER(8)");

                entity.Property(e => e.Content)
                    .HasColumnName("CONTENT")
                    .HasColumnType("VARCHAR2(20)");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("CREATE_TIME")
                    .HasColumnType("TIMESTAMP(6)");

                entity.HasOne(d => d.Us)
                    .WithMany(p => p.Searchhistory)
                    .HasForeignKey(d => d.Usid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("SEARCHHISTORY_USERS_FK");
            });

            modelBuilder.Entity<Tag>(entity =>
            {
                entity.ToTable("TAG");

                entity.HasIndex(e => e.Name)
                    .HasName("TAG__UN_NAME")
                    .IsUnique();

                entity.HasIndex(e => e.TagId)
                    .HasName("TAG_PK")
                    .IsUnique();

                entity.Property(e => e.TagId)
                    .HasColumnName("TAG_ID")
                    .HasColumnType("NUMBER(6)");

                entity.Property(e => e.CatId)
                    .HasColumnName("CAT_ID")
                    .HasColumnType("NUMBER(6)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("NAME")
                    .HasColumnType("VARCHAR2(10)");

                entity.HasOne(d => d.Cat)
                    .WithMany(p => p.Tag)
                    .HasForeignKey(d => d.CatId)
                    .HasConstraintName("TAG_CAT_CAT_ID_FK");
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasKey(e => e.Usid);

                entity.ToTable("USERS");

                entity.HasIndex(e => e.Email)
                    .HasName("USERS__UN_EMAIL")
                    .IsUnique();

                entity.HasIndex(e => e.Nickname)
                    .HasName("USERS__UN_NICKNAME")
                    .IsUnique();

                entity.HasIndex(e => e.Tel)
                    .HasName("USERS__UN_TEL")
                    .IsUnique();

                entity.HasIndex(e => e.Usid)
                    .HasName("USERS_PK")
                    .IsUnique();

                entity.Property(e => e.Usid)
                    .HasColumnName("USID")
                    .HasColumnType("NUMBER(8)");

                entity.Property(e => e.Avatar)
                    .HasColumnName("AVATAR")
                    .HasColumnType("VARCHAR2(256)");

                entity.Property(e => e.Birthday)
                    .HasColumnName("BIRTHDAY")
                    .HasColumnType("DATE");

                entity.Property(e => e.CatId)
                    .HasColumnName("CAT_ID")
                    .HasColumnType("NUMBER(6)");

                entity.Property(e => e.ChangedTime)
                    .HasColumnName("CHANGED_TIME")
                    .HasColumnType("DATE");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("CREATE_TIME")
                    .HasColumnType("DATE");

                entity.Property(e => e.Email)
                    .HasColumnName("EMAIL")
                    .HasColumnType("VARCHAR2(256)");

                entity.Property(e => e.FollowerNum)
                    .HasColumnName("FOLLOWER_NUM")
                    .HasColumnType("NUMBER(8)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Gender)
                    .HasColumnName("GENDER")
                    .HasColumnType("VARCHAR2(5)");

                entity.Property(e => e.IsBanned)
                    .HasColumnName("IS_BANNED")
                    .HasColumnType("NUMBER")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Nickname)
                    .IsRequired()
                    .HasColumnName("NICKNAME")
                    .HasColumnType("VARCHAR2(10)");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("PASSWORD")
                    .HasColumnType("VARCHAR2(20)");

                entity.Property(e => e.Signature)
                    .HasColumnName("SIGNATURE")
                    .HasColumnType("VARCHAR2(40)");

                entity.Property(e => e.Tel)
                    .HasColumnName("TEL")
                    .HasColumnType("VARCHAR2(11)");

                entity.HasOne(d => d.CatNavigation)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.CatId)
                    .HasConstraintName("USERS_CAT_CAT_ID_FK");
            });

            modelBuilder.Entity<Video>(entity =>
            {
                entity.HasKey(e => e.Vid);

                entity.ToTable("VIDEO");

                entity.HasIndex(e => e.Vid)
                    .HasName("VIDEO_PK")
                    .IsUnique();

                entity.Property(e => e.Vid).HasColumnName("VID");

                entity.Property(e => e.CommentNum)
                    .HasColumnName("COMMENT_NUM")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Cover)
                    .HasColumnName("COVER")
                    .HasColumnType("VARCHAR2(256)");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("CREATE_TIME")
                    .HasColumnType("DATE");

                entity.Property(e => e.Description)
                    .HasColumnName("DESCRIPTION")
                    .HasColumnType("VARCHAR2(400)");

                entity.Property(e => e.FavoriteNum)
                    .HasColumnName("FAVORITE_NUM")
                    .HasColumnType("NUMBER(8)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.IsBanned)
                    .HasColumnName("IS_BANNED")
                    .HasColumnType("NUMBER");

                entity.Property(e => e.LikeNum)
                    .HasColumnName("LIKE_NUM")
                    .HasColumnType("NUMBER(8)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Path)
                    .HasColumnName("PATH")
                    .HasColumnType("VARCHAR2(256)");

                entity.Property(e => e.Time).HasColumnName("TIME");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnName("TITLE")
                    .HasColumnType("VARCHAR2(80)");

                entity.Property(e => e.Usid)
                    .HasColumnName("USID")
                    .HasColumnType("NUMBER(8)");

                entity.Property(e => e.WatchNum)
                    .HasColumnName("WATCH_NUM")
                    .HasDefaultValueSql("0");

                entity.HasOne(d => d.Us)
                    .WithMany(p => p.Video)
                    .HasForeignKey(d => d.Usid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("VIDEO_USERS_FK");
            });

            modelBuilder.Entity<Videocomment>(entity =>
            {
                entity.HasKey(e => e.Vcid);

                entity.ToTable("VIDEOCOMMENT");

                entity.HasIndex(e => e.Vcid)
                    .HasName("VIDEOCOMMENT_PK")
                    .IsUnique();

                entity.Property(e => e.Vcid).HasColumnName("VCID");

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasColumnName("CONTENT")
                    .HasColumnType("VARCHAR2(100)");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("CREATE_TIME")
                    .HasColumnType("DATE");

                entity.Property(e => e.LikeNum)
                    .HasColumnName("LIKE_NUM")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.ParentVcid).HasColumnName("PARENT_VCID");

                entity.Property(e => e.ReplyVcid).HasColumnName("REPLY_VCID");

                entity.Property(e => e.Usid)
                    .HasColumnName("USID")
                    .HasColumnType("NUMBER(8)");

                entity.Property(e => e.Vid)
                    .HasColumnName("VID")
                    ;//.ValueGeneratedOnAdd();

                entity.HasOne(d => d.ParentVc)
                    .WithMany(p => p.InverseParentVc)
                    .HasForeignKey(d => d.ParentVcid)
                    .HasConstraintName("VIDEOCOMMENT_VIDEOCOMMENT_VCID_FK");

                entity.HasOne(d => d.ReplyVc)
                    .WithMany(p => p.InverseReplyVc)
                    .HasForeignKey(d => d.ReplyVcid)
                    .HasConstraintName("VIDEOCOMMENT_VIDEOCOMMENT_FK");

                entity.HasOne(d => d.Us)
                    .WithMany(p => p.Videocomment)
                    .HasForeignKey(d => d.Usid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("VIDEOCOMMENT_USER_FK");

                entity.HasOne(d => d.V)
                    .WithMany(p => p.Videocomment)
                    .HasForeignKey(d => d.Vid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("VIDEOCOMMENT_VIDEO_FK");
            });

            modelBuilder.Entity<Videotag>(entity =>
            {
                entity.HasKey(e => new { e.Vid, e.TagId });

                entity.ToTable("VIDEOTAG");

                entity.HasIndex(e => new { e.Vid, e.TagId })
                    .HasName("VIDEOTAG_PK")
                    .IsUnique();

                entity.Property(e => e.Vid).HasColumnName("VID");

                entity.Property(e => e.TagId)
                    .HasColumnName("TAG_ID")
                    .HasColumnType("NUMBER(6)");

                entity.HasOne(d => d.Tag)
                    .WithMany(p => p.Videotag)
                    .HasForeignKey(d => d.TagId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("VIDEOTAG_TAG_FK");

                entity.HasOne(d => d.V)
                    .WithMany(p => p.Videotag)
                    .HasForeignKey(d => d.Vid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("VIDEOTAG_VIDEO_FK");
            });

            modelBuilder.Entity<Watchhistory>(entity =>
            {
                entity.HasKey(e => new { e.Usid, e.Vid, e.CreateTime });

                entity.ToTable("WATCHHISTORY");

                entity.HasIndex(e => new { e.Usid, e.Vid, e.CreateTime })
                    .HasName("WATCHHISTORY_PK")
                    .IsUnique();

                entity.Property(e => e.Usid)
                    .HasColumnName("USID")
                    .HasColumnType("NUMBER(8)");

                entity.Property(e => e.Vid).HasColumnName("VID");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("CREATE_TIME")
                    .HasColumnType("TIMESTAMP(6)");

                entity.HasOne(d => d.Us)
                    .WithMany(p => p.Watchhistory)
                    .HasForeignKey(d => d.Usid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("WHISTORY_USER_FK");

                entity.HasOne(d => d.V)
                    .WithMany(p => p.Watchhistory)
                    .HasForeignKey(d => d.Vid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("WHISTORY_VIDEO_FK");
            });

            modelBuilder.HasSequence("ADMIN_ID_SEQ");

            modelBuilder.HasSequence("BCID_SEQ");

            modelBuilder.HasSequence("BID_SEQ");

            modelBuilder.HasSequence("BRID_SEQ");

            modelBuilder.HasSequence("CAT_ID_SEQ");

            modelBuilder.HasSequence("MID_SEQ");

            modelBuilder.HasSequence("TAG_ID_SEQ");

            modelBuilder.HasSequence("USID_SEQ");

            modelBuilder.HasSequence("VCID_SEQ");

            modelBuilder.HasSequence("VID_SEQ");

            modelBuilder.HasSequence("VRID_SEQ");
        }
    }
}
