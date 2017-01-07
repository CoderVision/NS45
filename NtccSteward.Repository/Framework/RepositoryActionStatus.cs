using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NtccSteward.Repository.Framework
{
    public enum RepositoryActionStatus
    {
        Ok,
        Created,
        Updated,
        NotFound,
        Deleted,
        NothingModified,
        Error
    }
}