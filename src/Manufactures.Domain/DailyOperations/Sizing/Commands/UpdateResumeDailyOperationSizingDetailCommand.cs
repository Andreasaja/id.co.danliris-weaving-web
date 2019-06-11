﻿using FluentValidation;
using Manufactures.Domain.Shared.ValueObjects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Manufactures.Domain.DailyOperations.Sizing.Commands
{
    public class UpdateResumeDailyOperationSizingDetailCommand
    {
        [JsonProperty(PropertyName = "ShiftDocumentId")]
        public ShiftId ShiftDocumentId { get; set; }

        [JsonProperty(PropertyName = "OperatorDocumentId")]
        public OperatorId OperatorDocumentId { get; set; }

        [JsonProperty(PropertyName = "ResumeDate")]
        public DateTimeOffset ResumeDate { get; set; }

        [JsonProperty(PropertyName = "ResumeTime")]
        public TimeSpan ResumeTime { get; set; }

        [JsonProperty(PropertyName = "Information")]
        public string Information { get; set; }
    }

    public class UpdateResumeDailyOperationSizingDetailCommandValidator : AbstractValidator<UpdateResumeDailyOperationSizingDetailCommand>
    {
        public UpdateResumeDailyOperationSizingDetailCommandValidator()
        {
            RuleFor(validator => validator.ShiftDocumentId.Value).NotEmpty();
            RuleFor(validator => validator.ResumeDate).NotEmpty();
            RuleFor(validator => validator.ResumeTime).NotEmpty();
            RuleFor(validator => validator.OperatorDocumentId.Value).NotEmpty();
        }
    }
}
