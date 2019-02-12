﻿using Infrastructure.Domain;
using Manufactures.Domain.Events;
using Manufactures.Domain.GlobalValueObjects;
using Manufactures.Domain.Orders.ReadModels;
using Manufactures.Domain.Orders.ValueObjects;
using Moonlay;
using System;

namespace Manufactures.Domain.Orders
{
    public class WeavingOrderDocument : AggregateRoot<WeavingOrderDocument, WeavingOrderDocumentReadModel>
    {
        public WeavingOrderDocument(Guid id, string orderNumber,
                                    FabricConstructionDocument fabricConstructionDocument,
                                    DateTimeOffset dateOrdered,
                                    Period period,
                                    Composition composition,
                                    string warpOrigin,
                                    string weftOrigin,
                                    int wholeGrade,
                                    string yarnType,
                                    WeavingUnit weavingUnit, 
                                    string orderStatus) : base(id)
        {
            // Validate Properties
            Validator.ThrowIfNullOrEmpty(() => orderNumber);
            Validator.ThrowIfNull(() => fabricConstructionDocument);
            Validator.ThrowIfNull(() => period);
            Validator.ThrowIfNull(() => composition);
            Validator.ThrowIfNullOrEmpty(() => warpOrigin);
            Validator.ThrowIfNullOrEmpty(() => weftOrigin);
            Validator.ThrowIfNullOrEmpty(() => yarnType);
            Validator.ThrowIfNull(() => weavingUnit);
            Validator.ThrowIfNullOrEmpty(() => orderStatus);

            this.MarkTransient();

            // Set Initial Value
            Identity = id;
            OrderNumber = orderNumber;
            FabricConstructionDocument = fabricConstructionDocument;
            DateOrdered = dateOrdered;
            WarpOrigin = warpOrigin;
            WeftOrigin = weftOrigin;
            WholeGrade = wholeGrade;
            YarnType = yarnType;
            Period = period;
            Composition = composition;
            WeavingUnit = weavingUnit;
            OrderStatus = orderStatus;

            ReadModel = new WeavingOrderDocumentReadModel(Identity)
            {
                OrderNumber = this.OrderNumber,
                DateOrdered = this.DateOrdered,
                FabricConstructionDocument = this.FabricConstructionDocument.Serialize(),
                WarpOrigin = this.WarpOrigin,
                WeftOrigin = this.WeftOrigin,
                WholeGrade = this.WholeGrade,
                YarnType = this.YarnType,
                Period = this.Period.Serialize(),
                Composition = this.Composition.Serialize(),
                WeavingUnit = this.WeavingUnit.Serialize(),
                OrderStatus = this.OrderStatus
            };

            ReadModel.AddDomainEvent(new OnWeavingOrderPlaced(this.Identity));
        }

        public WeavingOrderDocument(WeavingOrderDocumentReadModel readModel) : base(readModel)
        {
            this.OrderNumber = readModel.OrderNumber;
            this.FabricConstructionDocument = readModel.FabricConstructionDocument.Deserialize<FabricConstructionDocument>();
            this.DateOrdered = readModel.DateOrdered;
            this.WarpOrigin = readModel.WarpOrigin;
            this.WeftOrigin = readModel.WeftOrigin;
            this.WholeGrade = readModel.WholeGrade;
            this.YarnType = readModel.YarnType;
            this.Period = readModel.Period.Deserialize<Period>();
            this.Composition = readModel.Composition.Deserialize<Composition>();
            this.WeavingUnit = readModel.WeavingUnit.Deserialize<WeavingUnit>();
            this.OrderStatus = readModel.OrderStatus;
        }

        public string OrderNumber { get; private set; }
        public FabricConstructionDocument FabricConstructionDocument { get; private set; }
        public DateTimeOffset DateOrdered { get; private set; }
        public string WarpOrigin { get; private set; }
        public string WeftOrigin { get; private set; }
        public int WholeGrade { get; private set; }
        public string YarnType { get; private set; }
        public Period Period { get; private set; }
        public Composition Composition { get; private set; }
        public WeavingUnit WeavingUnit { get; private set; }
        public string OrderStatus { get; private set; }

        public void SetOrderStatus(string orderStatus)
        {
            Validator.ThrowIfNullOrEmpty(() => orderStatus);

            if(OrderStatus != orderStatus)
            {
                OrderStatus = orderStatus;
                ReadModel.OrderStatus = OrderStatus;

                MarkModified();
            }
        }

        public void SetWarpOrigin(string warpOrigin)
        {
            Validator.ThrowIfNull(() => warpOrigin);

            if (warpOrigin != WarpOrigin)
            {
                WarpOrigin = warpOrigin;
                ReadModel.WarpOrigin = WarpOrigin;

                MarkModified();
            }
        }

        public void SetWeftOrigin(string weftOrigin)
        {
            Validator.ThrowIfNull(() => weftOrigin);

            if(weftOrigin != WeftOrigin)
            {
                WeftOrigin = weftOrigin;
                ReadModel.WeftOrigin = WeftOrigin;

                MarkModified();
            }
        }

        public void SetWholeGrade(int wholeGrade)
        {

            if (wholeGrade != WholeGrade)
            {
                WholeGrade = wholeGrade;
                ReadModel.WholeGrade = WholeGrade;

                MarkModified();
            }
        }

        public void SetYarnType(string yarnType)
        {

            if (yarnType != YarnType)
            {
                YarnType = yarnType;
                ReadModel.YarnType = YarnType;

                MarkModified();
            }
        }

        public void SetFabricConstructionDocument(FabricConstructionDocument fabricConstructionDocument)
        {
            Validator.ThrowIfNull(() => fabricConstructionDocument);

            if(fabricConstructionDocument != FabricConstructionDocument)
            {
                FabricConstructionDocument = fabricConstructionDocument;
                ReadModel.FabricConstructionDocument = FabricConstructionDocument.Serialize();

                MarkModified();
            }
        }

        public void SetPeriod(Period period)
        {
            Validator.ThrowIfNull(() => period);

            if (period != Period)
            {
                Period = period;
                ReadModel.Period = Period.Serialize();

                MarkModified();
            }
        }

        public void SetComposition(Composition composition)
        {
            Validator.ThrowIfNull(() => composition);

            if (composition != Composition)
            {
                Composition = composition;
                ReadModel.Composition = Composition.Serialize();

                MarkModified();
            }
        }

        public void SetWeavingUnit(WeavingUnit weavingUnit)
        {
            Validator.ThrowIfNull(() => weavingUnit);

            if(weavingUnit != WeavingUnit)
            {
                WeavingUnit = weavingUnit;
                ReadModel.WeavingUnit = WeavingUnit.Serialize();

                MarkModified();
            }
        }

        protected override WeavingOrderDocument GetEntity()
        {
            return this;
        }
    }
}
