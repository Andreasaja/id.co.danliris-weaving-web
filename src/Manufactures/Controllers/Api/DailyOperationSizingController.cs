﻿using Barebone.Controllers;
using Manufactures.Application.Helpers;
using Manufactures.Domain.Beams.Repositories;
using Manufactures.Domain.DailyOperations.Sizing.Commands;
using Manufactures.Domain.DailyOperations.Sizing.Repositories;
using Manufactures.Domain.DailyOperations.Sizing.ValueObjects;
using Manufactures.Domain.FabricConstructions.Repositories;
using Manufactures.Domain.Machines.Repositories;
using Manufactures.Domain.Shared.ValueObjects;
using Manufactures.Domain.Shifts.Repositories;
using Manufactures.Domain.Shifts.ValueObjects;
using Manufactures.Dtos;
using Manufactures.Dtos.Beams;
using Manufactures.Dtos.DailyOperations.Sizing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moonlay.ExtCore.Mvc.Abstractions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Manufactures.Controllers.Api
{
    [Produces("application/json")]
    [Route("weaving/daily-operations-sizing")]
    [ApiController]
    [Authorize]
    public class DailyOperationSizingController : ControllerApiBase
    {
        private readonly IDailyOperationSizingRepository
            _dailyOperationSizingDocumentRepository;
        private readonly IMachineRepository
            _machineRepository;
        private readonly IFabricConstructionRepository
            _constructionDocumentRepository;
        private readonly IShiftRepository
            _shiftDocumentRepository;
        private readonly IBeamRepository
            _beamDocumentRepository;

        public DailyOperationSizingController(IServiceProvider serviceProvider,
                                                 IWorkContext workContext)
            : base(serviceProvider)
        {
            _dailyOperationSizingDocumentRepository =
                this.Storage.GetRepository<IDailyOperationSizingRepository>();
            _machineRepository =
                this.Storage.GetRepository<IMachineRepository>();
            _constructionDocumentRepository =
                this.Storage.GetRepository<IFabricConstructionRepository>();
            _shiftDocumentRepository =
                this.Storage.GetRepository<IShiftRepository>();
            _beamDocumentRepository =
                this.Storage.GetRepository<IBeamRepository>();
        }

        [HttpGet]
        public async Task<IActionResult> Get(int page = 1,
                                             int size = 25,
                                             string order = "{}",
                                             string keyword = null,
                                             string filter = "{}")
        {
            page = page - 1;
            var domQuery =
                _dailyOperationSizingDocumentRepository
                    .Query
                    .OrderByDescending(item => item.CreatedDate);
            var dailyOperationSizingDocuments =
                _dailyOperationSizingDocumentRepository
                    .Find(domQuery.Include(d => d.Details));

            var resultDto = new List<DailyOperationSizingListDto>();

            foreach (var dailyOperation in dailyOperationSizingDocuments)
            {
                var machineDocument =
                   _machineRepository
                       .Find(e => e.Identity.Equals(dailyOperation.MachineDocumentId.Value))
                       .FirstOrDefault();

                var constructionDocument =
                    _constructionDocumentRepository
                        .Find(e => e.Identity.Equals(dailyOperation.ConstructionDocumentId.Value))
                        .FirstOrDefault();

                var shiftOnDetail = new ShiftValueObject();
                var dailyOperationEntryDateTime = dailyOperation.Details.OrderBy(e=>e.DateTimeOperation).FirstOrDefault().DateTimeOperation;
                var lastDailyOperationStatus = dailyOperation.OperationStatus;

                foreach (var detail in dailyOperation.Details)
                {
                    var shiftDocument =
                        _shiftDocumentRepository
                            .Find(e => e.Identity.Equals(detail.ShiftDocumentId)).LastOrDefault();

                    shiftOnDetail = new ShiftValueObject(shiftDocument.Name, shiftDocument.StartTime, shiftDocument.EndTime);
                }

                var dto = new DailyOperationSizingListDto(dailyOperation, machineDocument, constructionDocument, shiftOnDetail, lastDailyOperationStatus, dailyOperationEntryDateTime);

                resultDto.Add(dto);
            }

            if (!order.Contains("{}"))
            {
                Dictionary<string, string> orderDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(order);
                var key = orderDictionary.Keys.First().Substring(0, 1).ToUpper() + orderDictionary.Keys.First().Substring(1);
                System.Reflection.PropertyInfo prop =
                    typeof(DailyOperationSizingListDto).GetProperty(key);

                if (orderDictionary.Values.Contains("asc"))
                {
                    resultDto =
                        resultDto
                            .OrderBy(x => prop.GetValue(x, null))
                            .ToList();
                }
                else
                {
                    resultDto =
                        resultDto
                            .OrderByDescending(x => prop.GetValue(x, null))
                            .ToList();
                }
            }

            resultDto =
                resultDto.Skip(page * size).Take(size).ToList();
            int totalRows = resultDto.Count();
            page = page + 1;

            await Task.Yield();

            return Ok(resultDto, info: new
            {
                page,
                size,
                total = totalRows
            });
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> Get(string Id)
        {
            try
            {
                var Identity = Guid.Parse(Id);
                var query = _dailyOperationSizingDocumentRepository.Query;
                var dailyOperationalSizing =
                    _dailyOperationSizingDocumentRepository.Find(query.Include(p => p.Details))
                                                .Where(o => o.Identity == Identity)
                                                .FirstOrDefault();

                var machineDocument =
                       _machineRepository
                           .Find(e => e.Identity.Equals(dailyOperationalSizing.MachineDocumentId.Value))
                           .FirstOrDefault();

                var machineNumber = machineDocument.MachineNumber;

                var constructionDocument =
                        _constructionDocumentRepository
                            .Find(e => e.Identity.Equals(dailyOperationalSizing.ConstructionDocumentId.Value))
                            .FirstOrDefault();

                var constructionNumber = constructionDocument.ConstructionNumber;

                var warpingBeams = new List<BeamDto>();

                foreach(var beam in dailyOperationalSizing.WarpingBeamsId)
                {
                    var beamDocument = _beamDocumentRepository.Find(b => b.Identity.Equals(beam.Value)).FirstOrDefault();

                    var beamsDto = new BeamDto(beamDocument);

                    warpingBeams.Add(beamsDto);
                }

                var dto = new DailyOperationSizingByIdDto(dailyOperationalSizing, machineNumber, constructionNumber, warpingBeams);

                foreach (var detail in dailyOperationalSizing.Details)
                {
                    var shiftDocument =
                        _shiftDocumentRepository
                            .Find(e => e.Identity.Equals(detail.ShiftDocumentId))
                            .FirstOrDefault();

                    var shiftName = shiftDocument.Name;

                    var history = new DailyOperationSizingHistoryDto(detail.DateTimeOperation, detail.MachineStatus, detail.Information);

                    var detailCauses = detail.Causes.Deserialize<DailyOperationSizingCausesDto>();

                    var causes = new DailyOperationSizingCausesDto(detailCauses.BrokenBeam, detailCauses.MachineTroubled);

                    var detailsDto = new DailyOperationSizingDetailsDto(shiftName, history, causes);

                    dto.Details.Add(detailsDto);
                }
                dto.Details = dto.Details.OrderBy(history => history.DateTimeOperationHistory).ToList();

                await Task.Yield();

                if (Identity == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(dto);
                }
            }
            catch (Exception x)
            {
                throw x;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]NewEntryDailyOperationSizingCommand command)
        {
            var newDailyOperationSizingDocument = await Mediator.Send(command);

            return Ok(newDailyOperationSizingDocument.Identity);
        }

        [HttpPut("{Id}/start")]
        public async Task<IActionResult> Put(string Id,
                                            [FromBody]UpdateStartDailyOperationSizingCommand command)
        {
            if (!Guid.TryParse(Id, out Guid documentId))
            {
                return NotFound();
            }
            command.SetId(documentId);
            var updateStartDailyOperationSizingDocument = await Mediator.Send(command);

            return Ok(updateStartDailyOperationSizingDocument.Identity);
        }

        [HttpPut("{Id}/pause")]
        public async Task<IActionResult> Put(string Id,
                                             [FromBody]UpdatePauseDailyOperationSizingCommand command)
        {
            if (!Guid.TryParse(Id, out Guid documentId))
            {
                return NotFound();
            }
            command.SetId(documentId);
            var updatePauseDailyOperationSizingDocument = await Mediator.Send(command);

            return Ok(updatePauseDailyOperationSizingDocument.Identity);
        }

        [HttpPut("{Id}/resume")]
        public async Task<IActionResult> Put(string Id,
                                             [FromBody]UpdateResumeDailyOperationSizingCommand command)
        {
            if (!Guid.TryParse(Id, out Guid documentId))
            {
                return NotFound();
            }
            command.SetId(documentId);
            var updateResumeDailyOperationSizingDocument = await Mediator.Send(command);

            return Ok(updateResumeDailyOperationSizingDocument.Identity);
        }

        [HttpPut("{Id}/doff")]
        public async Task<IActionResult> Put(string Id,
                                             [FromBody]UpdateDoffFinishDailyOperationSizingCommand command)
        {
            if (!Guid.TryParse(Id, out Guid documentId))
            {
                return NotFound();
            }
            command.SetId(documentId);
            var updateDoffDailyOperationSizingDocument = await Mediator.Send(command);

            return Ok(updateDoffDailyOperationSizingDocument.Identity);
        }

        [HttpGet("size-pickup/daterange/start-date/{startDate}/end-date/{endDate}/unit-id/{weavingUnitId}/shift/{shiftId}")]
        public async Task<IActionResult> GetReportByDateRange(DateTimeOffset StartDate, DateTimeOffset EndDate, Guid WeavingUnitId, Guid ShiftId)
        {
            try
            {
                int totalCount = 0;

                var acceptRequest = Request.Headers.Values.ToList();
                //var index = acceptRequest.IndexOf("application/pdf") > 0;

                var resultData = new List<SizePickupListDto>();
                var query =
                    _dailyOperationSizingDocumentRepository
                        .Query.OrderByDescending(item => item.CreatedDate);
                var sizePickupDtos =
                    _dailyOperationSizingDocumentRepository
                        .Find(query).Where(sizePickup=> sizePickup.WeavingUnitId.Value.Equals(WeavingUnitId));

                foreach (var sizePickupDto in sizePickupDtos)
                {
                    foreach (var detail in sizePickupDto.Details)
                    {
                        var filterShiftDto = detail.ShiftDocumentId;
                        var filterDateDto = detail.DateTimeOperation;

                        if (filterShiftDto == ShiftId)
                        {
                            if (filterDateDto >= StartDate && filterDateDto >= EndDate)
                            {
                                var requestedDto = new SizePickupListDto(sizePickupDto, detail);
                                resultData.Add(requestedDto);
                            }
                        }
                    }
                }

                //var results = SizePickupReportXlsTemplate.GetDataByDateRange(startDate, endDate, weavingUnitId, shiftId);
                //data = results;
                //totalCount = results.Count;

                await Task.Yield();
                return Ok(resultData, info: new
                {
                    count = totalCount
                });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet("size-pickup/month/{Month}/unit-id/{WeavingUnitId}/shift/{ShiftId}")]
        public async Task<IActionResult> GetReportByMonth(int Month, int WeavingUnitId, string ShiftId)
        {
            try
            {
                int totalCount = 0;

                //var acceptRequest = Request.Headers.Values.ToList();
                //var index = acceptRequest.IndexOf("application/pdf") > 0;

                var resultData = new List<SizePickupListDto>();
                var query =
                    _dailyOperationSizingDocumentRepository
                        .Query.OrderByDescending(item => item.CreatedDate);
                var sizePickupDtos =
                    _dailyOperationSizingDocumentRepository
                        .Find(query).Where(sizePickup => sizePickup.WeavingUnitId.Equals(WeavingUnitId)&&sizePickup.OperationStatus.Equals(DailyOperationMachineStatus.ONFINISH));

                foreach (var sizePickupDto in sizePickupDtos)
                {
                    foreach (var detail in sizePickupDto.Details)
                    {
                        var filterShiftDto = detail.ShiftDocumentId.ToString();
                        var filterMonthDto = detail.DateTimeOperation.Month;

                        if(filterShiftDto == ShiftId)
                        {
                            if (filterMonthDto == Month)
                            {
                                var filteredDto = new SizePickupListDto(sizePickupDto, detail);
                                resultData.Add(filteredDto);
                            }
                        }
                    }
                }

                //var results = SizePickupReportXlsTemplate.GetDataByMonth(periodType, month, weavingUnitId, shiftId);
                //data = results;
                //totalCount = results.Count;

                await Task.Yield();
                return Ok(resultData, info: new
                {
                    count = totalCount
                });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
