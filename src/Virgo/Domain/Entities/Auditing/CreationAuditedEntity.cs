﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Virgo.Domain.Entities.Auditing
{
    /// <summary>
    /// 抽象实现<see cref =“ICreationAudited”/>实体
    /// </summary>
    /// <typeparam name =“TPrimaryKey”>实体主键的类型</ typeparam>
    [Serializable]
    public abstract class CreationAuditedEntity<TPrimaryKey> : Entity<TPrimaryKey>, ICreationAudited<TPrimaryKey> where TPrimaryKey : struct
    {
        /// <summary>
        /// 该实体的创建时间
        /// </summary>
        public virtual DateTime CreationTime { get; set; }

        /// <summary>
        /// 这个实体的创造者
        /// </summary>
        public virtual TPrimaryKey? CreatorUserId { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected CreationAuditedEntity()
        {
            CreationTime = DateTime.Now;
        }
    }
    [Serializable]
    public abstract class CreationAuditedEntity : Entity, ICreationAudited
    {
        public virtual DateTime CreationTime { get; set; }

        public virtual string CreatorUserId { get; set; }

        protected CreationAuditedEntity()
        {
            CreationTime = DateTime.Now;
        }
    }
}