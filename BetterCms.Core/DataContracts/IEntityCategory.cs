﻿using Devbridge.Platform.Core.DataContracts;

namespace BetterCms.Core.DataContracts
{
    public interface IEntityCategory: IEntity
    {
        ICategory Category { get; set; }

        void SetEntity(IEntity entity);
    }
}
