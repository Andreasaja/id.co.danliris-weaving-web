﻿using Infrastructure.Domain.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Manufactures.Domain.Orders.Commands
{
    public class RemoveOrderCommandOld : ICommand<ManufactureOrder>
    {
        public void SetId(Guid id) { Id = id; }
        public Guid Id { get; private set; }
    }
}
