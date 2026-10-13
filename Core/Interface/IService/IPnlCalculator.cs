using Shared.DTOs.Pnl;
using Shared.Results;

namespace Core.Interface.IService;

public interface IPnlCalculator
{
    Result<PnlResultDto> Calculate(PnlInputDto input);
}
