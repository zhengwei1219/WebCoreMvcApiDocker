using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Zhengwei.Project.Domain.AggregatesModel;

namespace Zhengwei.Project.Infrastructure.EntityConfiguration
{
    public class ProjectContributorEntityConfiguration : IEntityTypeConfiguration<Zhengwei.Project.Domain.AggregatesModel.ProjectContributor>
    {
        public void Configure(EntityTypeBuilder<Domain.AggregatesModel.ProjectContributor> builder)
        {
            builder.ToTable("ProjectContributors")
                 .HasKey(p => p.Id);
        }
    }
}
