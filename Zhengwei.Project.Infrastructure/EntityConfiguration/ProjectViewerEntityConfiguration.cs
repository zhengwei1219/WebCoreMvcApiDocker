using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Zhengwei.Project.Domain.AggregatesModel;

namespace Zhengwei.Project.Infrastructure.EntityConfiguration
{
    public class ProjectViewerEntityConfiguration : IEntityTypeConfiguration<Zhengwei.Project.Domain.AggregatesModel.ProjectViewer>
    {
        public void Configure(EntityTypeBuilder<Domain.AggregatesModel.ProjectViewer> builder)
        {
            builder.ToTable("ProjectViewer")
                 .HasKey(p => p.Id);
        }
    }
}
