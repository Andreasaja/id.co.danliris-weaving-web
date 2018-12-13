﻿using Moonlay.Domain;
using System;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure
{
    public abstract class DanLirisReadModel : ReadModelExtCore, IAuditTrail, ISoftDelete
    {
        protected DanLirisReadModel(Guid identity)
        {
            Identity = identity;
        }

        #region IAuditTrail ISoftDelete
        [Timestamp]
        public byte[] RowVersion { get; set; }

        public string CreatedBy { get; set; }

        public DateTimeOffset CreatedDate { get; set; }

        public string ModifiedBy { get; set; }

        public DateTimeOffset? ModifiedDate { get; set; }

        public bool? Deleted { get; set; }

        public DateTimeOffset? DeletedDate { get; set; }

        public string DeletedBy { get; set; }
        #endregion
    }
}
