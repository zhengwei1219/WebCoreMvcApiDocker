using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zhengwei.Project.Domain.AggregatesModel;

namespace Zhengwei.Project.Infrastructure.EntityConfiguration
{
    public class ProjectEntityConfiguration : IEntityTypeConfiguration<Zhengwei.Project.Domain.AggregatesModel.Project>
    {
        public void Configure(EntityTypeBuilder<Zhengwei.Project.Domain.AggregatesModel.Project> builder)
        {
            builder.ToTable("Projects")
                 .HasKey(p=>p.Id);
        }
    }
}
