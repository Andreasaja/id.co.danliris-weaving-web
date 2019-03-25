﻿using Manufactures.Domain.GlobalValueObjects;
using Manufactures.Domain.Machines;
using Manufactures.Domain.Machines.ValueObjects;
using Manufactures.Domain.Shared.ValueObjects;
using Newtonsoft.Json;
using System;

namespace Manufactures.Dtos.Machine
{
    public class MachineDocumentDto
    {
        [JsonProperty(propertyName: "Id")]
        public Guid Id { get; }

        [JsonProperty(propertyName: "MachineNumber")]
        public string MachineNumber { get; private set; }

        [JsonProperty(propertyName: "Location")]
        public string Location { get; private set; }

        [JsonProperty(propertyName: "MachineTypeId")]
        public MachineTypeId MachineTypeId { get; private set; }

        [JsonProperty(propertyName: "WeavingUnitId")]
        public UnitId WeavingUnitId { get; private set; }

        public MachineDocumentDto(MachineDocument document)
        {
            Id = document.Identity;
            MachineNumber = document.MachineNumber;
            Location = document.Location;
            MachineTypeId = document.MachineTypeId;
            WeavingUnitId = document.WeavingUnitId;
        }

        public MachineDocumentDto(MachineDocument machine, 
                                  MachineTypeValueObject machineTypeValueObject, 
                                  WeavingUnit weavingUnitValueObject)
        {

        }
    }
}
