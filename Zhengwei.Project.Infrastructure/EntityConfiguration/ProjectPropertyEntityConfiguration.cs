using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Zhengwei.Project.Domain.AggregatesModel;

namespace Zhengwei.Project.Infrastructure.EntityConfiguration
{
    public class ProjectPropertyEntityConfiguration : IEntityTypeConfiguration<Zhengwei.Project.Domain.AggregatesModel.ProjectProperty>
    {
        public void Configure(EntityTypeBuilder<Domain.AggregatesModel.ProjectProperty> builder)
        {
            builder.ToTable("ProjectProperties")
                 .HasKey(p => new { p.Key,p.ProjectId,p.Value});
        }
    }
}
