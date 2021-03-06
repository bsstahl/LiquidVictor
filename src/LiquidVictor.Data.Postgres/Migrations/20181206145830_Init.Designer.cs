﻿// <auto-generated />
using System;
using LiquidVictor.Data.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace LiquidVictor.Data.Postgres.Migrations
{
    [DbContext(typeof(Context))]
    [Migration("20181206145830_Init")]
    partial class Init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("LiquidVictor.Data.Postgres.ContentItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<byte[]>("Content")
                        .IsRequired()
                        .HasColumnName("content");

                    b.Property<string>("ContentType")
                        .IsRequired()
                        .HasColumnName("contenttype");

                    b.Property<DateTime>("CreateDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("createdate")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<string>("FileName")
                        .HasColumnName("filename")
                        .HasMaxLength(260);

                    b.Property<DateTime>("LastModifiedDate")
                        .HasColumnName("lastmodifieddate");

                    b.Property<string>("Title")
                        .HasColumnName("title")
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.ToTable("contentitems");
                });

            modelBuilder.Entity("LiquidVictor.Data.Postgres.Slide", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<DateTime>("CreateDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("createdate")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<DateTime>("LastModifiedDate")
                        .HasColumnName("lastmodifieddate");

                    b.Property<int>("Layout")
                        .HasColumnName("layout");

                    b.Property<string>("Notes")
                        .HasColumnName("notes");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnName("title");

                    b.Property<int>("TransitionIn")
                        .HasColumnName("transitionin");

                    b.Property<int>("TransitionOut")
                        .HasColumnName("transitionout");

                    b.HasKey("Id");

                    b.ToTable("slides");
                });

            modelBuilder.Entity("LiquidVictor.Data.Postgres.SlideContentItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<Guid>("ContentItemId")
                        .HasColumnName("contentitemid");

                    b.Property<DateTime>("CreateDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("createdate")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<DateTime>("LastModifiedDate")
                        .HasColumnName("lastmodifieddate");

                    b.Property<Guid>("SlideId")
                        .HasColumnName("slideid");

                    b.Property<int>("SortOrder")
                        .HasColumnName("sortorder");

                    b.HasKey("Id");

                    b.HasIndex("ContentItemId");

                    b.HasIndex("SlideId");

                    b.ToTable("slidecontentitems");
                });

            modelBuilder.Entity("LiquidVictor.Data.Postgres.SlideDeck", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<int>("AspectRatio")
                        .HasColumnName("aspectratio");

                    b.Property<DateTime>("CreateDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("createdate")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<DateTime>("LastModifiedDate")
                        .HasColumnName("lastmodifieddate");

                    b.Property<string>("Presenter")
                        .HasColumnName("presenter")
                        .HasMaxLength(200);

                    b.Property<string>("SubTitle")
                        .HasColumnName("subtitle")
                        .HasMaxLength(500);

                    b.Property<string>("ThemeName")
                        .HasColumnName("themename")
                        .HasMaxLength(50);

                    b.Property<string>("Title")
                        .HasColumnName("title")
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.ToTable("slidedeck");
                });

            modelBuilder.Entity("LiquidVictor.Data.Postgres.SlideDeckSlide", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<DateTime>("CreateDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("createdate")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<DateTime>("LastModifiedDate")
                        .HasColumnName("lastmodifieddate");

                    b.Property<Guid>("SlideDeckId")
                        .HasColumnName("slidedeckid");

                    b.Property<Guid>("SlideId")
                        .HasColumnName("slideid");

                    b.Property<int>("SortOrder")
                        .HasColumnName("sortorder");

                    b.HasKey("Id");

                    b.HasIndex("SlideDeckId");

                    b.HasIndex("SlideId");

                    b.ToTable("slidedeckslides");
                });

            modelBuilder.Entity("LiquidVictor.Data.Postgres.SlideContentItem", b =>
                {
                    b.HasOne("LiquidVictor.Data.Postgres.ContentItem", "ContentItem")
                        .WithMany()
                        .HasForeignKey("ContentItemId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("LiquidVictor.Data.Postgres.Slide", "Slide")
                        .WithMany("SlideContentItems")
                        .HasForeignKey("SlideId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("LiquidVictor.Data.Postgres.SlideDeckSlide", b =>
                {
                    b.HasOne("LiquidVictor.Data.Postgres.SlideDeck", "SlideDeck")
                        .WithMany("SlideDeckSlides")
                        .HasForeignKey("SlideDeckId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("LiquidVictor.Data.Postgres.Slide", "Slide")
                        .WithMany()
                        .HasForeignKey("SlideId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
