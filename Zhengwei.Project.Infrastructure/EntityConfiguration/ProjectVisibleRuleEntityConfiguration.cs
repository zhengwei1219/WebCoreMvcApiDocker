using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Zhengwei.Project.Domain.AggregatesModel;

namespace Zhengwei.Project.Infrastructure.EntityConfiguration
{
    public class ProjectVisibleRuleEntityConfiguration : IEntityTypeConfiguration<Zhengwei.Project.Domain.AggregatesModel.ProjectVisibleRule>
    {
        public void Configure(EntityTypeBuilder<Domain.AggregatesModel.ProjectVisibleRule> builder)
        {
            builder.ToTable("ProjectVisibleRule")
                 .HasKey(p => p.Id);
        }
    }
}
