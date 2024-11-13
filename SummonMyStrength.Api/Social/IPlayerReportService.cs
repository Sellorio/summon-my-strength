using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SummonMyStrength.Api.Social;

public interface IPlayerReportService
{
    event Func<Dictionary<string, InGameReportStatus>, Task> InGameReportStatusUpdated;
}