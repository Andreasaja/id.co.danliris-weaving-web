﻿using Infrastructure.Domain.ReadModels;
using Manufactures.Domain.DailyOperations.Loom.Entities;
using System;
using System.Collections.Generic;

namespace Manufactures.Domain.DailyOperations.Loom.ReadModels
{
    public class DailyOperationLoomReadModel : ReadModelBase
    {
        public Guid OrderDocumentId { get; internal set; }
        public string OperationStatus { get; internal set; }
        public List<DailyOperationLoomBeamHistory> LoomBeamHistories { get; internal set; }

        public DailyOperationLoomReadModel(Guid identity) : base(identity)
        {
        }
    }
}
