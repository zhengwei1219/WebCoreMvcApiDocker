using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Zhengwei.Recommend.Api.Models;

namespace Zhengwei.Recommend.Api.Data
{
    public class RecommendDbContext:DbContext
    {
        public RecommendDbContext(DbContextOptions<RecommendDbContext> options):base(options)
        {
           
        }
        public DbSet<ProjectRecommend> ProjectRecommends { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProjectRecommend>().ToTable("ProjectRecommend")
                .HasKey(p => p.Id);

            base.OnModelCreating(modelBuilder);
        }
    }
}
