using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zhengwei.Project.Domain.Events;
using Zhengwei.Project.Domain.SeedWork;

namespace Zhengwei.Project.Domain.AggregatesModel
{
    public class Project:Entity, IAggregateRoot
    {
        public Project()
        {
            this.Viewers = new List<ProjectViewer>();
            this.Contributors = new List<ProjectContributor>();
            this.AddDomainEvent(new ProjectCreatedEvent { Project = this });
        }
        public int UserId { get; set; }

        /// <summary>
        /// 项目logo
        /// </summary>
        public string Avatar { get; set; }
        public string Company { get; set; }
        /// <summary>
        /// 原BP文件
        /// </summary>
        public string OriginBPFile{get;set;}
        /// <summary>
        /// 转换后的BP文件
        /// </summary>
        public string FormatBPFile { get; set; }
        /// <summary>
        /// 是否显示敏感信息
        /// </summary>

        public bool ShowSecurityInfo { get; set; }

        public int ProvinceId { get; set; }
        public string Province { get; set; }

        public int CityId { get; set; }
        public string City { get; set; }

        public int AreaId { get; set; }
        public string AreaName { get; set; }

        public DateTime RegisterTime { get; set; }
        public string Introduction { get; set; }
        /// <summary>
        /// 出让朌份比例
        /// </summary>
        public string FinPercentage { get; set; }
        /// <summary>
        /// 融资阶段
        /// </summary>
        public string FinStage { get; set; }
        /// <summary>
        /// 融资金额
        /// </summary>
        public int FinMoney { get; set; }

        public int Income { get; set; }

        /// <summary>
        /// 利润
        /// </summary>
        public int Revenue { get; set; }

        /// <summary>
        /// 估值
        /// </summary>
        public int Valuation { get; set; }
        /// <summary>
        /// 佣金分配方式 
        /// </summary>
        public int BrokerageOptions { get; set; }

        public bool OnPlatform { get; set; }

        public ProjectVisibleRule VisibleRule { get; set; }
        /// <summary>
        /// 根引用项目ID
        /// </summary>
        public int SourceId { get; set; }
        /// <summary>
        /// 上级引用项目ID
        /// </summary>
        public int ReferenceId { get; set; }
        public string Tags { get; set; }
        /// <summary>
        /// 项目属性
        /// </summary>
        public List<ProjectProperty> Properties { get; set; }
        /// <summary>
        /// 贡献者列表
        /// </summary>
        public List<ProjectContributor> Contributors { get; set; }
        /// <summary>
        /// 查看者
        /// </summary>
        public List<ProjectViewer> Viewers { get; set; }

        public DateTime UpdateTime { get; set; }
        public DateTime CreatedTime { get; set; }
        private Project CloneProject(Project source=null)
        {
            if(source==null)
            {
                source = this;
            }
            var newProject = new Project()
            {
                AreaId = source.AreaId,
                BrokerageOptions = source.BrokerageOptions,
                Avatar = source.Avatar,
                City = source.City,
                CityId = source.CityId,
                Company = source.Company,
                Contributors = new List<ProjectContributor> { },
                Viewers = new List<ProjectViewer> { },
                CreatedTime = DateTime.Now,
                FinMoney = source.FinMoney,
                FinPercentage = source.FinPercentage,
                FinStage = source.FinStage,
                FormatBPFile = source.FormatBPFile,
                Income = source.Income,
                Introduction = source.Introduction,
                OnPlatform = source.OnPlatform,
                OriginBPFile = source.OriginBPFile,
                Province = source.Province,
                ProvinceId = source.ProvinceId,
                VisibleRule = source.VisibleRule,
                Valuation = source.Valuation,
                ShowSecurityInfo = source.ShowSecurityInfo,
                RegisterTime = source.RegisterTime,
                Revenue = source.Revenue
            };

            newProject.Properties = new List<ProjectProperty>
            { };
                foreach(var item in source.Properties)
            {
                newProject.Properties.Add(new ProjectProperty
                    (
                   item.Key,
                    item.Text,
                    item.Value
                ));
            }
            return newProject;
           
        }

        public Project ContributorFork(int contributorId,Project source = null)
        {
            if (source == null)
                source = this;
            var newProject = CloneProject(source);
            newProject.UserId = contributorId;
            newProject.SourceId = source.SourceId;
            newProject.ReferenceId = source.ReferenceId;
            newProject.UpdateTime = DateTime.Now;
            return newProject;
        }

        public void AddViewer(int userId,string userName,string avatar)
        {
            var viewer = new ProjectViewer {
                UserId = userId,
                UserName =userName,
                Avatar = avatar,
                CreatedTime = DateTime.Now
                
            };
            if (!Viewers.Any(v => v.UserId == UserId))
            {
                Viewers.Add(viewer);
                AddDomainEvent(new ProjectViewedEvent {Company= this.Company,Introduction = this.Introduction, Viewer = viewer });
            }
                
        }

        public void AddContributor(ProjectContributor contributor)
        {
            if (!Contributors.Any(v => v.UserId == UserId))
            {
                Contributors.Add(contributor);
                AddDomainEvent(new ProjectJoinedEvent {
                    Company = this.Company,
                    Introduction = this.Introduction,
                    contributor = contributor
                });
            }
                
        }


    }
}
